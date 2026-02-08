
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using shine.libs.math;

namespace shine.libs.capture
{
    public static class kbitmap
    {
        public static byte[] Image2Byte(Image img, ImageFormat imageFormat)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, imageFormat);
                ms.Close();

                byteArray = ms.ToArray();
            }

            return byteArray;
        }
        public static byte[] Image2Bytex(Image img, ImageFormat imageFormat)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, imageFormat);
                ms.Close();

                byteArray = ms.ToArray();

                ms.Read(byteArray, 0, (int)ms.Length);

            }

            return byteArray;
        }
        public static Bitmap getPartialArea(Bitmap bmpSrc, krect rectArea)
        {
            Bitmap bmpDst = new Bitmap(rectArea.W, rectArea.H, bmpSrc.PixelFormat);
            krect rectSrc = new krect(0, 0, bmpSrc.Width, bmpSrc.Height);
            krect rectDst = new krect(0, 0, bmpDst.Width, bmpDst.Height);

            BitmapData dataSrc =
                bmpSrc.LockBits(rectSrc.R, ImageLockMode.ReadOnly,
                bmpSrc.PixelFormat);
            BitmapData dataDst =
                bmpDst.LockBits(rectDst.R, ImageLockMode.WriteOnly,
                bmpDst.PixelFormat);

            IntPtr ptrSrc = dataSrc.Scan0;
            IntPtr ptrDst = dataDst.Scan0;

            int sizeSrc = Math.Abs(dataSrc.Stride) * bmpSrc.Height;
            int sizeDst = Math.Abs(dataDst.Stride) * bmpDst.Height;

            byte[] rgbSrc = new byte[sizeSrc];
            byte[] rgbDst = new byte[sizeDst];

            Marshal.Copy(ptrSrc, rgbSrc, 0, sizeSrc);

            int s0 = 0;
            int s1 = 0;
            s0 = dataSrc.Stride * rectArea.Y;
            s0 += 3 * rectArea.X;

            int lenDst = rectDst.W * 3;

            for (int h = 0; h < bmpDst.Height; h++)
            {
                System.Array.Copy(rgbSrc, s0, rgbDst, s1, lenDst);

                s0 += Math.Abs(dataSrc.Stride);
                s1 += Math.Abs(dataDst.Stride);
            }
            //test test 
            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbDst, 0, ptrDst, sizeDst);

            // Unlock the bits.
            bmpSrc.UnlockBits(dataSrc);
            bmpDst.UnlockBits(dataDst);

            return bmpDst;
        }

        public static void SetAlpha(this Bitmap bmp, byte alpha)
        {
            if (bmp == null) 
                throw new ArgumentNullException("bmp");

            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            var eof = data.Scan0 + data.Height * data.Stride;
            var pos = data.Scan0;

            while (pos != eof)
            {
                var posA = pos + 3;
                var eol = posA + data.Width * 4;

                while (posA != eol)
                {
                    Marshal.WriteByte(posA, alpha);
                    posA += 4;
                }

                pos += data.Stride;
            }

            bmp.UnlockBits(data);
        }

        /*
          Usage:

              var pngImage = new Bitmap("filename.png");
              pngImage.SetAlpha(255);
          */

        public static void LockUnlockBitsExample(Bitmap bmp, string newfile)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
                bmp.LockBits(rect, ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Set every third value to 255. A 24bpp bitmap will look red.  
            /*
            for (int counter = 2; counter < rgbValues.Length; counter += 3)
                rgbValues[counter] = 255;
            */

            int start = 0;

            /*
			width == 픽셀 갯수
			한줄 바이트 갯수 == width * (3 or 4) + [....]
             */

            for (int h = 0; h < bmp.Height; h++)
            {
                for (int x = 2; x < bmp.Width * 3; x += 3)
                    rgbValues[start + x] = 255;

                start += bmpData.Stride;
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            // Draw the modified image.

            bmp.Save(newfile, ImageFormat.Jpeg);

        }
        public static void ToGrayScale(Bitmap bmp, string newfile)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
                bmp.LockBits(rect, ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int countBytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbData = new byte[countBytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbData, 0, countBytes);

            int start = 0;
            byte gray;
            byte r, g, b;

            for (int h = 0; h < bmp.Height; h++)
            {
                for (int x = 0; x < bmp.Width * 3; x += 3)
                {
                    r = rgbData[start + x + 2];
                    g = rgbData[start + x + 1];
                    b = rgbData[start + x + 0];

                    gray = GrayValue2(r, g, b);

                    rgbData[start + x + 2] = gray;
                    rgbData[start + x + 1] = gray;
                    rgbData[start + x + 0] = gray;
                }

                start += bmpData.Stride;
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbData, 0, ptr, countBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            bmp.Save(newfile, ImageFormat.Jpeg);

        }
        static byte GrayValue(int R, int G, int B)
        {
            int g = (int)Math.Round(0.299f * R + 0.587f * G + 0.114f * B);
            return (byte)Math.Min(255, g);
        }
        static byte GrayValue2(int R, int G, int B)
        {
            //return (byte)Math.Max((Math.Max(R,G)),B);
            return (byte)((R + G + B) / 3);
        }

    }
}