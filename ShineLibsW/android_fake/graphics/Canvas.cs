using System;
using System.Drawing;
using System.Drawing.Imaging;

using shine.libs.math;

using wBitmap = System.Drawing.Bitmap;
using Bitmap = Android.Graphics.Bitmap;
using wColor = System.Drawing.Color;

namespace Android.Graphics
{
    public class PorterDuff
    {
        public static class Mode
        {
            public static readonly int Clear=0;
        }
    }

    public class Canvas
    {
        public System.Drawing.Graphics g;

        public Canvas(Bitmap image)
        {
            g = System.Drawing.Graphics.FromImage(image.winBmp);

        }
        public Canvas(System.Drawing.Graphics g)
        {
            this.g = g;
        }

        public void DrawColor(Color color)
        {
            g.Clear(color);
        }
        public void DrawColor(Color color, int mode)
        {
            g.Clear(color);
        }

        public void DrawBitmap(Bitmap image, 
            Rectangle rectSrc, Rectangle rectDst, Paint paint)
        {
            g.DrawImage(image.winBmp, rectDst, rectSrc, GraphicsUnit.Pixel);
        }

        public void DrawRect(Rectangle rect, Paint paint)
        {
            if (paint.style == Paint.Style.Stroke)
                g.DrawRectangle(paint.pen, rect);

            else if (paint.style == Paint.Style.Fill)
                g.FillRectangle(paint.brush, rect);
        }
        public void DrawRect2(Rectangle rect, Paint paint)
        {
            g.FillRectangle(paint.brush, rect);
        }
        public void DrawText(string text, float x, float y, Paint paint)
        {
            g.DrawString(text, paint.font, paint.brush, x, y);
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Paint paint)
        {
            g.DrawLine(paint.pen, x0, y0, x1, y1);
        }

        public void DrawCircle(int x, int y, int r, Paint paint)
        {
            krect rect = new krect(x - r, y - r, 2 * r, 2 * r);

            if (paint.style == Paint.Style.Stroke)
                g.DrawEllipse(paint.pen, rect.R);
            else if (paint.style == Paint.Style.Fill)
                g.FillEllipse(paint.brush, rect.R);
        }

        public void DrawPath(Path path, Paint paint)
        {
            if(path.poss.Count>1)
            {
                if (paint.style == Paint.Style.Stroke)
                    g.DrawPath(paint.pen, path.gPath);
                else if (paint.style == Paint.Style.Fill)
                    g.FillPath(paint.brush, path.gPath);
            }
        }
        public void DrawTextOnPath(string text, Path path, int x, int y, Paint pnt)
        {
            //throw new NotImplementedException();
        }

        public void DrawLines(float[] pp, Paint paint)
        {
            int pcount = pp.Length / 4;
            float x0, y0, x1, y1;

            for (int i = 0; i < pcount; i++)
            {
                x0 = pp[4 * i + 0];
                y0 = pp[4 * i + 1];
                x1 = pp[4 * i + 2];
                y1 = pp[4 * i + 3];

                g.DrawLine(paint.pen, x0, y0, x1, y1);
            }
        }
    }
}