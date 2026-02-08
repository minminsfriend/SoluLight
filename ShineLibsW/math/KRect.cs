using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Rectangle = System.Drawing.Rectangle;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace shine.libs.math
{
    public class ksize
    {
        public float w, h;
        public ksize(float w, float h)
        {
            this.w = w;
            this.h = h;
        }
        public void scale(float sx, float sy)
        {
            w *= sx;
            h *= sy;
        }
        public void scale(float s)
        {
            w *= s;
            h *= s;
        }
    }

    public class krect
    {
        public float x, y, w, h;

        public krect()
        {
            x = y = w = h = 1;
        }
        public krect(float x, float y, float w, float h)
        {
            setxywh(x, y, w, h);
        }
        public krect(float w, float h)
        {
            setxywh(0, 0, w, h);
        }
        public krect(krect r)
        {
            setxywh(r.x, r.y, r.w, r.h);
        }
        public krect(Size size)
        {
            setxywh(0, 0, size.Width, size.Height);
        }
        public krect(Rectangle r)
        {
            setltrb(r.Left, r.Top, r.Right, r.Bottom);
        }
        public krect(kvec cen, float w, float h)
        {
            x = cen.x - w / 2f;
            y = cen.y - h / 2f;

            setxywh(x, y, w, h);
        }
        public krect(PointF[] poss)
        {
            float l, r, t, b;

            l = t = 1000000;
            r = b = -1000000;

            for (int i = 0; i < poss.Length; i++)
            {
                l = l > poss[i].X ? poss[i].X : l;
                r = r < poss[i].X ? poss[i].X : r;
                t = t > poss[i].Y ? poss[i].Y : t;
                b = b < poss[i].Y ? poss[i].Y : b;
            }

            setltrb(l, t, r, b);
        }
        public krect(List<kvec> poss)
        {
            float l, r, t, b;

            l = t = 1000000;
            r = b = -1000000;

            for (int i = 0; i < poss.Count; i++)
            {
                l = l > poss[i].x ? poss[i].x : l;
                r = r < poss[i].x ? poss[i].x : r;
                t = t > poss[i].y ? poss[i].y : t;
                b = b < poss[i].y ? poss[i].y : b;
            }

            x = l;
            y = t;
            w = r - l + 1; w = Math.Max(1, w);
            h = b - t + 1; h = Math.Max(1, h);
        }
        public void setxywh(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
        public void setrect(krect r0)
        {
            this.x = r0.x;
            this.y = r0.y;
            this.w = r0.w;
            this.h = r0.h;
        }
        public void setRect(Rectangle r0)
        {
            setltrb(r0.Left, r0.Top, r0.Right, r0.Bottom);
        }
        public void setltrb(float l, float t, float r, float b)
        {
            this.x = l;
            this.y = t;
            this.w = r - l;
            this.h = b - t;
        }
        public void setxy(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public int X
        {
            get
            {
                return (int)Math.Round(x);
            }
        }
        public int Y
        {
            get
            {
                return (int)Math.Round(y);
            }
        }
        public int W
        {
            get
            {
                return (int)Math.Round(w);
            }
        }
        public int H
        {
            get
            {
                return (int)Math.Round(h);
            }
        }
        public int Right
        {
            get
            {
                return (int)Math.Round(x + w);
            }
        }
        public int Bottom
        {
            get
            {
                return (int)Math.Round(y + h);
            }
        }
        public kvec pos()
        {
            return new kvec(x, y);
        }
        public ksize kvec()
        {
            return new ksize(w, h);
        }

        public void offset(float dx, float dy)
        {
            x += dx;
            y += dy;
        }
        public void offset(kvec vec)
        {
            x += vec.x;
            y += vec.y;
        }
        public void scale(float sx)
        {
            scale(sx, sx);
        }
        public void scale(float sx, float sy)
        {
            x = x * sx;
            y = y * sy;
            w = w * sx;
            h = h * sy;
        }
        public void scaleCen(float sx)
        {
            scaleCen(sx, sx);
        }
        public void scaleCen(float sx, float sy)
        {
            float w0 = w;
            float h0 = h;

            w = w0 * sx;
            h = h0 * sy;

            float dx = w0 - w;
            float dy = h0 - h;

            x = x + dx / 2.0f;
            y = y + dy / 2.0f;
        }
        public void inflate(float dx, float dy)
        {
            w += 2 * dx;
            h += 2 * dy;

            x -= dx;
            y -= dy;
        }
        public bool contains(float px, float py)
        {
            float right = x + w;
            float bottom = y + h;

            if (x <= px && px <= right)
                if (y <= py && py <= bottom)
                    return true;

            return false;
        }
        public bool contains(Point p)
        {
            return contains(p.X, p.Y);
        }
        public bool contains(kvec p)
        {
            return contains(p.X, p.Y);
        }
        public krect copy()
        {
            float x1 = this.x;
            float y1 = this.y;
            float w1 = this.w;
            float h1 = this.h;

            return new krect(x1, y1, w1, h1);
        }
        public bool equals(krect rectx)
        {
            if (X != rectx.X)
                return false;
            if (Y != rectx.Y)
                return false;
            if (W != rectx.W)
                return false;
            if (H != rectx.H)
                return false;

            return true;
        }
        public kvec Cen
        {
            get
            {
                return new kvec(x + 0.5f * w, y + 0.5f * h);
            }
        }
        public float Aspect
        {
            get
            {
                if (w < 0.0001f)
                    return 1f;

                return w / h;
            }
        }
        public Rectangle R // windows
        {
            get
            {
                return new Rectangle(X, Y, W, H);
            }
        }
        public Size Size
        {
            get
            {
                return new Size(W, H);
            }
        }
        public SizeF SizeF
        {
            get
            {
                return new SizeF(w, h);
            }
        }
        public string toString()
        {
            return $"{X} {Y} {W} {H}";
        }
        public static krect Parse(string trect)
        {
            var ss = Regex.Split(trect.Trim(), " ");

            if (ss.Length == 2)
            {
                int w, h;
                if (int.TryParse(ss[0], out w))
                    if (int.TryParse(ss[1], out h))
                        return new krect(w, h);
            }
            else if (ss.Length == 4)
            {
                int x, y, w, h;
                if (int.TryParse(ss[0], out x))
                    if (int.TryParse(ss[1], out y))
                        if (int.TryParse(ss[2], out w))
                            if (int.TryParse(ss[3], out h))
                                return new krect(x, y, w, h);
            }

            return new krect();
        }
        public bool contains(MouseEventArgs e)
        {
            return contains(e.X, e.Y);
        }
    }
}
