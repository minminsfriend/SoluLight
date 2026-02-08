using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using shine.libs.math;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace shine.libs.graphics
{
    public class gx
    {
        public static bool MatchColors(Color a, Color b, int ocha)
        {
            int dR = Math.Abs(a.R - b.R);
            int dG = Math.Abs(a.G - b.G);
            int dB = Math.Abs(a.B - b.B);

            return dR <= ocha && dG <= ocha && dB <= ocha;
        }
        public static bool GetPixel(Bitmap bmp, kvec p, out Color color)
        {
            color = Color.White;

            if (-1 < p.X && p.X < bmp.Width)
                if (-1 < p.Y && p.Y < bmp.Height)
                {
                    color = bmp.GetPixel(p.X, p.Y);
                    return true;

                }

            return false;
        }
        public static string ColorToString(Color color)
        {
            return $"{color.R} {color.G} {color.B}";
        }
        public static Color ParseColor(string text)
        {
            var ss = Regex.Split(text, " ");
            if (ss.Length < 3)
                return Color.White;

            int r, g, b;

            if (int.TryParse(ss[0], out r))
                if (int.TryParse(ss[1], out g))
                    if (int.TryParse(ss[2], out b))
                        return Color.FromArgb(r, g, b);

            return Color.White;
        }
        public static string DetectLetters(Bitmap bmp, int LineHei, int ocha,
            Dictionary<string, List<kvec>> pixs, Dictionary<string, List<Color>> clrs)
        {
            foreach (var ke in pixs.Keys)
            {
                List<kvec> pxs = pixs[ke];
                List<Color> crs = clrs[ke];

                /* 3줄 로 시도한다.  */
                for (int L = 0; L < 3; L++)
                {
                    int count = 0;

                    for (int k = 0; k < pxs.Count; k++)
                    {
                        kvec pPixel = pxs[k].copy();
                        Color cPixel = crs[k];

                        /* 줄 바꾸기 기능  */
                        pPixel.y += LineHei * L;

                        if (gx.MatchPixelColor(bmp, pPixel, cPixel, ocha))
                            count++;
                    }

                    if (count == pxs.Count)
                    {
                        return ke;
                    }
                }
            }

            return null;
        }

        public static bool MatchPixelColor(Bitmap bmp, kvec pPixel, Color cPixel, int ocha)
        {
            Color cGot;
            if (gx.GetPixel(bmp, pPixel, out cGot))
            {
                if (gx.MatchColors(cPixel, cGot, ocha))
                    return true;
            }

            return false;
        }
        public static bool ComparePixelsOfImage(Bitmap bmp, List<kvec> pixels, List<Color> colors, int ocha, int yOff)
        {
            int n = -1;
            int count = 0;

            foreach (var px in pixels)
            {
                var p = px.copy();
                p.y += yOff;

                Color c0 = colors[++n];

                if (gx.MatchPixelColor(bmp, p, c0, ocha))
                    count++;
            }

            if (count == pixels.Count)
                return true;
            else
                return false;
        }
        /*
            rect=18 497 230 13
            h=13
            width=230
            글자색 128 170 255

         */
        public static bool ComparePixelsOfWordFloatedX(Bitmap bmp, List<kvec> pixels, List<Color> colors, int yOff)
        {
            var color0 = colors[0];
            var preAllBlank = true;

            bool[] bools = new bool[13];
            List<int> yy = new List<int>();
            var y0 = pixels[0].y;

            foreach (var p in pixels)
            {
                if (p.X == 0)
                {
                    if (!yy.Contains(p.Y))
                        yy.Add(p.Y);
                }
            }
            for (int y = 0; y < 13; y++)
            {
                bools[y] = yy.Contains(y) ? true : false;
            }

            for (int x = 0; x < 200; x++)
            {
                List<Color> colorsLineY = new List<Color>();

                var allblank = checkBlankLineY(bmp, x, color0, ref colorsLineY, yOff);

                if (allblank)//빈줄 현재
                {
                    preAllBlank = true;//다음에 전줄이 빈줄이다
                    continue;
                }
                else
                {
                    if (preAllBlank)//빈줄 아님, 전줄이 빈줄, 이때만 인식을 시도한다
                    {
                        preAllBlank = false;//reset

                        var startPointMaybe = true;
                        for (int y = 0; y < 13; y++)
                        {
                            if (bools[y])
                                startPointMaybe = colorsLineY[y] == color0;
                            else
                                startPointMaybe = colorsLineY[y] != color0;
                        }

                        if (startPointMaybe)
                        {
                            var pos1 = new kvec(x, y0);
                            if (comparePixelsOfWordLocal(bmp, pixels, colors, pos1, yOff))
                                return true;
                        }
                        else
                        {
                            // 인식 못함. 다음 빈줄을 찾아 이동한다.
                        }
                    }
                }
            }

            return false;
        }
        static bool checkBlankLineY(Bitmap bmp, int x, Color color0, ref List<Color> colorsx, int yOff)
        {
            for (int y = 0; y < 13; y++)
                colorsx.Add(bmp.GetPixel(x, y + yOff));

            foreach (var c in colorsx)
            {
                if (c == color0)
                    return false;
            }

            return true;
        }
        static bool comparePixelsOfWordLocal(Bitmap bmp, List<kvec> pixels, List<Color> colors, kvec pos1, int yOff)
        {
            var pixel0 = pixels[0];
            var color0 = colors[0];

            for (int i = 1; i < pixels.Count; i++)
            {
                var vec = pixels[i] - pixel0;
                var px1 = pos1 + vec;
                var X = px1.X;
                var Y = px1.Y + yOff;

                if (X < 0 || Y < 0)
                    return false;
                if (X > bmp.Width - 1 || Y > bmp.Height - 1)
                    return false;

                var color = bmp.GetPixel(X, Y);
                if (color != color0)
                    return false;
            }

            return true;
        }
        public static List<Color> ParseColors(string colorstex)
        {
            List<Color> colors = new List<Color>();
            var ss = Regex.Split(colorstex, ",");

            foreach (var s in ss)
                colors.Add(ParseColor(s.Trim()));

            return colors;
        }
    }
}
