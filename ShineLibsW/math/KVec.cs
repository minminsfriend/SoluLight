using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Text.RegularExpressions;

namespace shine.libs.math
{
    public class kvec
    {
        const float deg2rad = (float)Math.PI / 180.0f;
        const float rad2deg = 180.0f / (float)Math.PI;

        public float x, y, z;

        public kvec()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }
        public kvec(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public kvec(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }
        public kvec(kvec v1)
        {
            this.x = v1.x;
            this.y = v1.y;
            this.z = v1.z;
        }
        public kvec(Point p)
        {
            this.x = p.X;
            this.y = p.Y;
            this.z = 0;
        }
        public kvec(PointF p)
        {
            this.x = p.X;
            this.y = p.Y;
            this.z = 0;
        }
        public void set(kvec p)
        {
            this.x = p.x;
            this.y = p.y;
        }
        public void set(Point p)
        {
            this.x = p.X;
            this.y = p.Y;
        }
        public void set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public void set(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public kvec copy()
        {
            return new kvec(x, y, z);
        }
        public void add(kvec v1)
        {
            this.x += v1.x;
            this.y += v1.y;
            this.z += v1.z;
        }
        public void add(float dx, float dy, float dz)
        {
            this.x += dx;
            this.y += dy;
            this.z += dz;
        }
        public void add(kvec v1, float s)
        {
            this.x += s * v1.x;
            this.y += s * v1.y;
            this.z += s * v1.z;
        }
        public void sub(kvec v1)
        {
            this.x -= v1.x;
            this.y -= v1.y;
            this.z -= v1.z;
        }
        public static kvec add(kvec a, kvec b)
        {
            return new kvec(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static kvec operator +(kvec a, kvec b)
        {
            return new kvec(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static kvec sub(kvec a, kvec b)
        {
            return new kvec(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static kvec operator -(kvec a, kvec b)
        {
            return new kvec(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public void scale(float s)
        {
            this.x *= s;
            this.y *= s;
            this.z *= s;
        }
        public void scale(float sx, float sy)
        {
            this.x *= sx;
            this.y *= sy;
            this.z *= 1f;
        }
        public void scale(kvec s)
        {
            this.x *= s.x;
            this.y *= s.y;
            this.z *= s.z;
        }
        public void offset(float dx, float dy)
        {
            this.x += dx;
            this.y += dy;
        }
        public void offset(kvec dis)
        {
            this.x += dis.x;
            this.y += dis.y;
            this.z += dis.z;
        }
        public static kvec scale(kvec v0, float s)
        {
            return new kvec(s * v0.x, s * v0.y, s * v0.z);
        }
        public static kvec scale(kvec v0, kvec s)
        {
            return new kvec(s.x * v0.x, s.y * v0.y, s.z * v0.z);
        }
        public static kvec operator *(float s, kvec v0)
        {
            return new kvec(s * v0.x, s * v0.y, s * v0.z);
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
        public int Z
        {
            get
            {
                return (int)Math.Round(z);
            }
        }
        public Point P
        {
            get
            {
                return new Point(X, Y);
            }
        }
        public PointF PF
        {
            get
            {
                return new PointF(x, y);
            }
        }
        public static kvec Zero
        {
            get { return new kvec(0, 0); }
        }
        public float length()
        {
            return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
        }
        public float getDeg()
        {
            double angle = Math.Atan2(y, x);
            return (float)angle * rad2deg;
        }
        public static float getDotDeg(kvec a, kvec b)
        {
            float dot_ab = dot(a, b) / (a.length() * b.length());
            float angle = (float)Math.Acos(dot_ab);

            return angle * rad2deg;
        }
        public static double DegBetween(kvec A, kvec B)
        {
            double sin = A.x * B.y - B.x * A.y;
            double cos = A.x * B.x + A.y * B.y;

            double deg = Math.Atan2(sin, cos) * (180 / Math.PI);

            return deg;
        }
        public static kvec cross(kvec a, kvec b)
        {
            float x = a.y * b.z - a.z * b.y;
            float y = a.z * b.x - a.x * b.z;
            float z = a.x * b.y - a.y * b.x;

            return new kvec(x, y, z);
        }
        public static float dot(kvec a, kvec b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }
        public static float dis(kvec v0, kvec v1)
        {
            kvec dvec = kvec.sub(v0, v1);
            return dvec.length();
        }
        public void normalize()
        {
            float len = length();
            len = len < 0.0001f ? 0.000f : len;

            x /= len;
            y /= len;
            z /= len;
        }
        public static kvec normalize(kvec v0)
        {
            float len = v0.length();

            if (len < 0.00001f)
                return new kvec();
            else
                return new kvec(v0.x / len, v0.y / len, v0.z / len);
        }
        public static PointF[] toPointFs(kvec[] poss)
        {
            PointF[] poss_ = new PointF[poss.Length];
            for (int i = 0; i < poss.Length; i++)
                poss_[i] = poss[i].PF;

            return poss_;
        }
        public static List<PointF> toPointFs(List<kvec> points)
        {
            List<PointF> pfs = new List<PointF>();

            foreach (var p in points)
            {
                pfs.Add(new PointF(p.x, p.y));
            }

            return pfs;
        }
        public string toString2()
        {
            return $"{X} {Y}";
        }
        public string toString3()
        {
            return $"{X} {Y} {Z}";
        }
        public bool equals(kvec p0)
        {
            return X == p0.X && Y == p0.Y;
        }
        public static kvec Parse2d(string tex)
        {
            var nn = Regex.Split(tex, " ");
            float x, y;

            if (nn.Length == 2)
                if (float.TryParse(nn[0], out x))
                    if (float.TryParse(nn[1], out y))
                        return new kvec(x, y);

            return new kvec();
        }
        public static kvec Parse3d(string tex)
        {
            var nn = Regex.Split(tex, " ");
            float x, y, z;

            if (nn.Length == 3)
                if (float.TryParse(nn[0], out x))
                    if (float.TryParse(nn[1], out y))
                        if (float.TryParse(nn[2], out z))
                            return new kvec(x, y, z);

            return new kvec();
        }
        public static List<kvec> ParseList(string tex)
        {
            List<kvec> poss = new List<kvec>();
            var ss = Regex.Split(tex, ",");

            foreach(var s in ss)
                poss.Add(Parse2d(s));

            return poss;
        }
        public static void ListOffset(List<kvec> poss, int dx, int dy)
        {
            for (int i = 0; i < poss.Count; i++)
                poss[i].offset(dx, dy);
        }
        public static List<kvec> ListCopy(List<kvec> poss)
        {
            List<kvec> possnew = new List<kvec>();

            for (int i = 0; i < poss.Count; i++)
                possnew.Add(poss[i].copy());

            return possnew;
        }
    }
}
