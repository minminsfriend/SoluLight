using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

using shine.libs.math;
using shine.libs.konst;
using shine.libs.window;

namespace shine.libs.capture
{
    public static class CapDll
    {
        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest,
                                    IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr ptr);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);
        public static Bitmap CaptureScreen(krect rectCap)
        {
            IntPtr hDC = GetDC(IntPtr.Zero);
            IntPtr hMemDC = CreateCompatibleDC(hDC);
            IntPtr hBitmap = CreateCompatibleBitmap(hDC, rectCap.W, rectCap.H);

            int Width = rectCap.X + rectCap.W;
            int Height = rectCap.Y + rectCap.H;

            IntPtr hOld = (IntPtr)SelectObject(hMemDC, hBitmap);
            BitBlt(hMemDC, -rectCap.X, -rectCap.Y, Width, Height, hDC, 0, 0,
                CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
            SelectObject(hMemDC, hOld);

            DeleteDC(hMemDC);
            ReleaseDC(IntPtr.Zero, hDC);

            Bitmap memImage = System.Drawing.Image.FromHbitmap(hBitmap);
            DeleteObject(hBitmap);

            return memImage;
        }

        public static byte[] getImageData(Bitmap bmp, ImageFormat format)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, format);
                ms.Flush();
                ms.Position = 0;

                int imageSize = (int)ms.Length;
                data = new byte[imageSize];

                ms.Read(data, 0, imageSize);
                ms.Close();
            }

            return data;
        }
        public static int getImageData(Bitmap bmp, ImageFormat format, ref byte[] data)
        {
            int imageSize = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, format);
                ms.Flush();
                ms.Position = 0;

                imageSize = (int)ms.Length;

                ms.Read(data, 0, imageSize);
                ms.Close();
            }

            return imageSize;
        }

        public static byte[] getCapData(krect rectCap, ImageFormat imgFormat)
        {
            Bitmap image = CapDll.CaptureScreen(rectCap);
            return getImageData(image, imgFormat);
        }

        public static byte[] getImageData2(Bitmap bmp)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, bmp);
                return ms.ToArray();
            }
        }
  
    }
}