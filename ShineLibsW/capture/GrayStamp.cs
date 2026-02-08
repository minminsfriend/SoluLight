/*
* Created by SharpDevelop.
* User: shine
* Date: 2016-08-23
* Time: 오후 4:51
* 
* To change this template use Tools | Options | Coding | Edit Standard Headers.
*/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using shine.libs.math;

namespace shine.libs.capture
{
    public class GrayStamp
    {
        const int _MB_ = 1000 * 1000;

        public byte[] data;
        public byte[] rgb;
        //public int count;
        public int Width, Height;

        public GrayStamp(int maxsize)
        {
            data = new byte[maxsize];
            rgb = new byte[4 * _MB_];
            Width = 1;
            Height = 1;

            setZero();
        }
        public int Count
        {
            get
            {
                return Width * Height;
            }
        }
        void setZero()
        {
            byte[] temp = new byte[1000];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = 0;

            int start;
            int len, rem;
            int sum = 0;

            while (true)
            {
                start = sum;
                rem = data.Length - sum;
                len = rem > temp.Length ? temp.Length : rem;

                System.Array.Copy(temp, 0, data, start, len);
                sum += len;

                if (sum >= data.Length)
                    break;
            }
        }

        public bool buildGrayPixel(Bitmap bmpCap)
        {
            if (Width != bmpCap.Width || Height != bmpCap.Height)
            {
                Width = bmpCap.Width;
                Height = bmpCap.Height;
            }

            Rectangle rectLock;
            ImageLockMode READONLY = ImageLockMode.ReadOnly;
            BitmapData dataCap;
            int dataSize;

            rectLock = new Rectangle(0, 0, Width, Height);
            dataCap = bmpCap.LockBits(rectLock, READONLY, PixelFormat.Format24bppRgb);
            dataSize = Math.Abs(dataCap.Stride) * Height;

            if (dataSize > rgb.Length)
            {
                Console.WriteLine("<GrayStamp> dataSize > rgb.Length ::  dataSize/rgb.Length == {0}/{1}",
                    dataSize, rgb.Length);

                bmpCap.UnlockBits(dataCap);
                return false;
            }

            Marshal.Copy(dataCap.Scan0, rgb, 0, dataSize);

            byte r, g, b, m;
            int start = 0;
            int n = -1;

            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    r = rgb[start + 3 * w + 2];
                    g = rgb[start + 3 * w + 1];
                    b = rgb[start + 3 * w + 0];

                    m = (byte)((r + g + b) / 3);
                    data[++n] = m;
                }

                start += dataCap.Stride;
            }

            //Marshal.Copy(rgb, 0, bd.Scan0, bdSize);
            bmpCap.UnlockBits(dataCap);
            return true;
        }
        public bool detectChanged(GrayStamp g0, ref krect RectDiff)
        {
            if (Count == 1)
                return false;

            int left, right, top, bottom;
            left = top = 10000;
            right = bottom = -1;

            int p;
            bool detectDifference = false;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    p = Width * y + x;

                    if (5 < Math.Abs(g0.data[p] - data[p]))
                    {
                        if (x < left)
                            left = x;
                        if (y < top)
                            top = y;
                        if (x > right)
                            right = x;
                        if (y > bottom)
                            bottom = y;

                        detectDifference = true;
                    }
                }
            }

            if (detectDifference)
            {
                int w = right - left;
                int h = bottom - top;

                if (w * h > 1)
                {
                    //살짝 넓혀준다
                    left = Math.Max(0, left - 5);
                    right = Math.Min(Width, right + 5);
                    top = Math.Max(0, top - 5);
                    bottom = Math.Min(Height, bottom + 5);

                    w = right - left;
                    h = bottom - top;
                    RectDiff.setxywh(left, top, w, h);

                    return true;
                }
            }

            return false;
        }
        public void updateGrayPixel(GrayStamp graySent)
        {
            Array.Copy(graySent.data, data, Count);
        }
    }
}