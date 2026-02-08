using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shine.libs.math
{
    public static class KGeom
    {
        public static krect GetRectInRect(krect rectBase, float aspectIn)
        {
            krect rectIn = rectBase.copy();

            float aspectBase = rectBase.w / rectBase.h;

            if (aspectBase > aspectIn) //베이스 렉트가 더 퍼졌다, h,y 일치
            {
                rectIn.w = rectIn.h * aspectIn;
                float dx = 0.5f * (rectBase.w - rectIn.w);

                rectIn.offset(dx, 0);
            }
            else//위와는 반대로 한다 w, x 일치
            {
                rectIn.h = rectIn.w / aspectIn;
                float dy = 0.5f * (rectBase.h - rectIn.h);

                rectIn.offset(0,dy);
            }

            return rectIn;
        }
        static kvec pointOnLine(kvec vecA, kvec vecB, kvec point, ref float h)
        {
            float cos = kvec.dot(vecA, vecB) / (vecA.length() * vecB.length());
            float sin = (float)Math.Sqrt(1.0 - Math.Pow(cos, 2));
            h = vecB.length() * sin;

            kvec cross = kvec.cross(vecA, vecB);
            cross.normalize();
            if (cross.z < 0f)
                h *= -1f;

            kmat mRot = kmat.zRotate(-90);
            kvec vecX = mRot.Transform(vecA);
            vecX.normalize();

            return point + (h * vecX);
        }
        public static bool lineTouch(kvec p0, kvec p1, kvec p, float w, ref kvec pOnLine)
        {
            float dis = 0;
            kvec[] poly = getPoly(p0, p1, w);

            if (kmath.isInPolygon(p, poly, 4))
            {
                kvec vecLine = p1 - p0;
                kvec vecP = p - p0;
                pOnLine = pointOnLine(vecLine, vecP, p, ref dis);

                return true;
            }

            return false;
        }
        static kvec[] getPoly(kvec p0, kvec p1, float w)
        {
            kvec vec = p1 - p0;
            kmat mRot = kmat.zRotate(-90);
            kvec vecX = mRot.Transform(vec);
            vecX.normalize();

            kvec[] poly = new kvec[4];

            poly[0] = p0 + w * vecX;
            poly[1] = p0 - w * vecX;
            poly[2] = p1 - w * vecX;
            poly[3] = p1 + w * vecX;

            return poly;
        }
        public static krect getPointRect(kvec p, float W)
        {
            return new krect(p.x - W / 2f, p.y - W / 2f, W, W);
        }
    }
}
