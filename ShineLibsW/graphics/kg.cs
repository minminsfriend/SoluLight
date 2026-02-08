using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using shine.libs.math;

namespace shine.libs.graphics
{
    public class HSV
    {
        public int h, s, v;

        public HSV(double h, double s, double v)
        {
            this.h = (int)h;
            this.s = (int)s;
            this.v = (int)v;
        }
        public HSV(int h, int s, int v)
        {
            this.h = h;
            this.s = s;
            this.v = v;
        }
    }
    public class kg
    {
        public static HSV ColorToHSV(Color color)
        {
            double max, min;

            var r = color.R;
            var g = color.G;
            var b = color.B;
            double h = 0;

            max = Math.Max(r, g);
            max = Math.Max(max, b);
            min = Math.Min(r, g);
            min = Math.Min(min, b);
            var v = max;                                 // 명도(V) = max(r,g,b)
            var s = (max != 0.0) ? (max - min) / max : 0.0;  // 채도(S)을 계산, S=0이면 R=G=B=0
            if (s == 0.0)
                h = 0;
            else
            {                                                      // 채도(S)는 != 0
                double delta = max - min;              // 색상(H)를 구한다.
                if (r == max)
                    h = (g - b) / delta;                // 색상이 Yello와 Magenta사이
                else if (g == max)
                    h = 2.0 + (b - r) / delta;         // 색상이 Cyan와 Yello사이
                else if (b == max)
                    h = 4.0 + (r - g) / delta;         // 색상이 Magenta와 Cyan사이
                h *= 60.0;
                if (h < 0.0)                                   // 색상값을 각도로 바꾼다.
                    h += 360.0;
            }

            return new HSV(h, s, v);
        }
        public static string EngName(Color color)
        {
            var nn = Regex.Split($"{color}", "\\[");
            var eng = nn[1].Substring(0, nn[1].Length - 1);

            return eng;
        }
        public static Color AlphaColor(int alpha, Color color)
        {
            alpha = Math.Min(alpha, 255);
            alpha = Math.Max(alpha, 0);

            return Color.FromArgb(alpha, color);
        }
        public static krect ImageIntoCanvas(krect rectCanvas, krect rectImage)
        {
            krect rectDst = new krect();

            float aImage = rectImage.Aspect;
            float aCanvas = rectCanvas.Aspect;

            if (aCanvas > aImage)//그림이 길쭉하다
            {
                rectDst.h = rectCanvas.h;
                rectDst.w = rectDst.h * aImage;
                rectDst.x = 0.5f * (rectCanvas.w - rectDst.w);
            }
            else
            {
                rectDst.w = rectCanvas.w;
                rectDst.h = rectDst.w / aImage;
                rectDst.y = 0.5f * (rectCanvas.h - rectDst.h);
            }

            rectDst.offset(rectCanvas.pos());

            return rectDst;
        }
        public static krect CanvasIntoImage(krect rectScreen, krect rectimg)
        {
            krect rectCut = rectimg.copy();

            if (rectimg.Aspect > rectScreen.Aspect)
            {
                //그림이 퍼졌다.
                // rectCut h 높이 고정
                rectCut.w = rectCut.h * rectScreen.Aspect;
                rectCut.x = (rectimg.w - rectCut.w) / 2f;
            }
            else
            {
                //그림이 길쭉하다.
                // rectCut w 넓이 고정
                rectCut.h = rectCut.w / rectScreen.Aspect;
                rectCut.y = (rectimg.h - rectCut.h) / 2f;
            }

            return rectCut;
        }
        public static Color ParseColor(string colorx)
        {
            var ss = Regex.Split(colorx, " ");

            if (ss.Length == 3)
            {
                var r = int.Parse(ss[0].Trim());
                var g = int.Parse(ss[0].Trim());
                var b = int.Parse(ss[0].Trim());


                return Color.FromArgb(r, g, b);
            }

            return Color.Black;
        }
        public static Bitmap ChangeOpacity(Image img, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();   // Releasing all resource used by graphics 
            
            return bmp;
        }
    }
}
