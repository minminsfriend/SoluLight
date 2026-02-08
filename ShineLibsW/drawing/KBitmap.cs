/*
 * Created by SharpDevelop.
 * User: shine
 * Date: 2016-05-19
 * Time: 오후 11:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using shine.libs.math;

namespace shine.libs.drawing
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


        public static void SetAlpha(this Bitmap bmp, byte alpha)
        {
            if (bmp == null) throw new ArgumentNullException("bmp");

            var data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var line = data.Scan0;
            var eof = line + data.Height * data.Stride;
            while (line != eof)
            {
                var pixelAlpha = line + 3;
                var eol = pixelAlpha + data.Width * 4;
                while (pixelAlpha != eol)
                {
                    System.Runtime.InteropServices.Marshal.WriteByte(
                        pixelAlpha, alpha);
                    pixelAlpha += 4;
                }
                line += data.Stride;
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
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Set every third value to 255. A 24bpp bitmap will look red.  
            /*
            for (int counter = 2; counter < rgbValues.Length; counter += 3)
                rgbValues[counter] = 255;
            */

            int start = 0;

            /*
             
			내가 가장 이해가 가는 코드이다.
			스트라이드는 바이트가 단위이다.
			픽셀의 넓이는 단위가 바이트*3 이다.
			
			한픽셀에서 다음 픽셀로 가려면 
			3단위의 바이트 인덱스를 뛰어가야한다.
			픽셀의 높이를 바꾸려면 스트라이드 값의 바이트를 지나가야한다.
			
			스트라이드 = 넓이*3 + 나머지
			단위는 바이트.
			알지비 에이 각각 한 바이트
			
			다시 한번 말하지만
			스트라이드는 바이트값이고
			넓이는 픽셀의 갯수값이다.
			따라서 넓이에 3또는 4를 곱해주어야 바이트값이 된다.
             
             */

            for (int h = 0; h < bmp.Height; h++)
            {
                for (int x = 2; x < bmp.Width * 3; x += 3)
                    rgbValues[start + x] = 255;

                start += bmpData.Stride;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            // Draw the modified image.

            bmp.Save(newfile, ImageFormat.Jpeg);

        }
        public static void ToGrayScale(Bitmap bmp, string newfile)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            int start = 0;
            byte gray;
            byte r, g, b;

            for (int h = 0; h < bmp.Height; h++)
            {
                for (int x = 0; x < bmp.Width * 3; x += 3)
                {
                    r = rgbValues[start + x + 2];
                    g = rgbValues[start + x + 1];
                    b = rgbValues[start + x + 0];

                    gray = GrayValue2(r, g, b);

                    rgbValues[start + x + 2] = gray;
                    rgbValues[start + x + 1] = gray;
                    rgbValues[start + x + 0] = gray;
                }

                start += bmpData.Stride;
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

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