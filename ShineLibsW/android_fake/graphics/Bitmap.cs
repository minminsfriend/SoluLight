using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using WinBitmap = System.Drawing.Bitmap;

namespace Android.Graphics
{
    public class Bitmap //fake
    {
        public WinBitmap winBmp;

        public int Width { get { return winBmp.Width; } }
        public int Height { get { return winBmp.Height; } }

        public Bitmap()
        {
        }
        public void Dispose()
        {
            winBmp.Dispose();
        }

        public Color GetPixel(int x, int y)
        {
            return winBmp.GetPixel(x, y);
        }
        public static class Config
        {
            public const int Argb8888 = 8888;

        }
        public void setNull()
        {
            if (winBmp != null)
            {
                winBmp.Dispose();
                winBmp = null;
            }
        }
        public bool IsNull
        {
            get { return winBmp == null;}
        }
        public static Bitmap CreateBitmap(int width, int height, int config)
        {
            Bitmap andBmp = new Bitmap();
            andBmp.winBmp = new WinBitmap(width, height, PixelFormat.Format32bppArgb);

            return andBmp;
        }
    }


    public class BitmapFactory
    {
        public static Bitmap DecodeFile(string file)
        {
            if (System.IO.File.Exists(file))
            {
                Bitmap abmp = new Bitmap();
                abmp.winBmp = (WinBitmap)Image.FromFile(file);
                return abmp;
            }
            else
                return null;
        }
        public static Bitmap DecodeByteArray(byte[] imageData, int start, int length)
        {
            Bitmap abmp = new Bitmap();

            MemoryStream ms = new MemoryStream(imageData, start, length);
            abmp.winBmp = (WinBitmap)Image.FromStream(ms);

            return abmp;
        }
    }
}