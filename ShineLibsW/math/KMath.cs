using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shine.libs.math
{
    public static class kmath
    {
        public const float PI = 3.14159f;

        //int pointInPolygon(float p[2], float poly[10][2], int count) {
        public static bool isInPolygon(kvec p, kvec[] poly, int count)
        {
            int k = count - 1;
            bool innnnn = false;

            for (int i = 0; i < count; i++)
            {
                if ((poly[i].y < p.y && p.y <= poly[k].y) || (p.y <= poly[i].y && poly[k].y < p.y))
                {
                    if (poly[i].x + (p.y - poly[i].y) / (poly[k].y - poly[i].y) * (poly[k].x - poly[i].x) < p.x)
                    {
                        innnnn = !innnnn;
                    }
                }

                k = i;
            }

            return innnnn;
        }
        public static bool isInPolygon(kvec p, List<kvec> poly, int count)
        {
            int k = count - 1;
            bool innnnn = false;

            for (int i = 0; i < count; i++)
            {
                if ((poly[i].y < p.y && p.y <= poly[k].y) || (p.y <= poly[i].y && poly[k].y < p.y))
                {
                    if (poly[i].x + (p.y - poly[i].y) / (poly[k].y - poly[i].y) * (poly[k].x - poly[i].x) < p.x)
                    {
                        innnnn = !innnnn;
                    }
                }

                k = i;
            }

            return innnnn;
        }
        public static bool fEquals(float a, float b)
        {
            if (Math.Abs(a - b) < 0.0000001f)
                return true;
            else
                return false;
        }

        public static int setBound(int val, int min, int max)
        {
            return Math.Min(max, Math.Max(min, val));
        }
        public static float setBound(int val, float min, float max)
        {
            return Math.Min(max, Math.Max(min, val));
        }
    }
}
