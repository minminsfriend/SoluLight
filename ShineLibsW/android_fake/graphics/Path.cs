using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using shine.libs.math;

namespace Android.Graphics
{
    public class Path
    {
        GraphicsPath gpath;
   
        public List<kvec> poss;

        public Path()
        {
            gpath = new GraphicsPath();
            poss = new List<kvec>();
        }
        public void SetPoss(List<kvec> poss, kmat mat)
        {
            this.poss.Clear();

            for(int i=0; i<poss.Count; i++)
            {
                this.poss.Add(mat.Transform(poss[i]));
            }
        }

        public GraphicsPath gPath
        {
            get
            {
                gpath.Reset();
                if (poss.Count > 2)
                {
                    //gpath.AddLines(possX());
                    gpath.AddClosedCurve(possX());
                    return gpath;
                }
                else
                    return null;
            }
        }
        PointF[] possX()
        {
            PointF[] pp = new PointF[poss.Count];
            for (int i = 0; i < pp.Length; i++)
                pp[i] = new PointF(poss[i].x, poss[i].y);

            return pp;
        }

        public void MoveTo(float x, float y)
        {
            poss.Clear();
            poss.Add(new kvec(x, y));
        }

        public void LineTo(float x, float y)
        {
            poss.Add(new kvec(x, y));
        }

        public void QuadTo(float x, float y, float midX, float midY)
        {
            

        }

        public void AddCircle(int v1, int v2, int v3, object cw)
        {
            throw new NotImplementedException();
        }

        public void AddRect(int v1, int v2, int v3, int v4, object cw)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void RLineTo(int v1, int v2)
        {
            throw new NotImplementedException();
        }

        public void CubicTo(int v1, int v2, int v3, int v4, int v5, int v6)
        {
            throw new NotImplementedException();
        }

        public sealed class Direction
        {
            public static Direction Ccw { get; }
            public static Direction Cw { get; }
        }
        public sealed class FillType
        {
            public static FillType EvenOdd { get; }
            public static FillType InverseEvenOdd { get; }
            public static FillType InverseWinding { get; }
            public static FillType Winding { get; }
        }
        public sealed class Op
        {
            public static Op Difference { get; }
            public static Op Intersect { get; }
            public static Op ReverseDifference { get; }
            public static Op Union { get; }
            public static Op Xor { get; }
        }
    }
}