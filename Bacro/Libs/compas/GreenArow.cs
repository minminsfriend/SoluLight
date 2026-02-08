using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using shine.libs.math;
using System.Reflection;
using System.Runtime.CompilerServices;

using shine.libs.graphics;
using System.Security.Cryptography;

namespace Bacro
{
    public class GreenArrow
    {
        public kvec head = new kvec();
        public kvec tail = new kvec(0, 1);

        List<kvec> bankarea;
        Bitmap ImageMap;

        public GreenArrow(List<kvec> bankarea)
        {
            this.bankarea = bankarea;
        }
        public bool SetImage(Bitmap imageCap)
        {
            if (ImageMap != null)
            {
                ImageMap.Dispose();
                ImageMap = null;
            }

            ImageMap = new Bitmap(imageCap);

            List<kvec> possGreen = scanGreenPixels(ImageMap);
            if (possGreen.Count == 0)
                return false;

            head = searchHead(possGreen);
            tail = searchTail(ImageMap, head);

            return true;
        }
        kvec pickupFirstGreen(Bitmap imageMap)
        {
            for (int x = 0; x < imageMap.Width; x++)
                for (int y = 0; y < imageMap.Height; y++)
                    if (isColorGreen(imageMap, x, y))
                        return new kvec(x, y);

            return null;
        }
        List<kvec> scanGreenPixels(Bitmap imageMap)
        {
            kvec posGreen0 = pickupFirstGreen(imageMap);
            if (posGreen0 == null)
                return new List<kvec>();

            List<kvec> possGreen = new List<kvec>();

            for (int x = posGreen0.X - 10; x <= posGreen0.X + 10; x++)
                for (int y = posGreen0.Y - 10; y <= posGreen0.Y + 10; y++)
                    if (isColorGreen(imageMap, x, y))
                        possGreen.Add(new kvec(x, y));

            return possGreen;
        }
        kvec searchHead(List<kvec> possGreen)
        {
            var headx = new kvec();

            foreach (var pos in possGreen)
                headx = headx + pos;

            if (possGreen.Count > 0)
                headx.scale(1f / possGreen.Count);

            return headx;
        }
        kvec searchTail(Bitmap image, kvec posCen)
        {
            List<kvec> possGreen = new List<kvec>();

            int W = 15;

            for (int x = posCen.X - W; x <= posCen.X + W; x++)
                for (int y = posCen.Y - W; y <= posCen.Y + W; y++)
                    if (isColorGreen2(image, x, y))
                    {
                        kvec pos = new kvec(x, y);
                        if (!kmath.isInPolygon(pos, bankarea, bankarea.Count))
                            possGreen.Add(pos);
                    }

            kvec posTar = new kvec();
            float disMax = 0;

            foreach (var pos in possGreen)
            {
                var dislen = (pos - posCen).length();

                if (dislen > disMax)
                {
                    posTar = pos.copy();
                    disMax = dislen;
                }
            }

            return posTar;
        }
        bool isColorGreen(Bitmap image, int x, int y)
        {
            // 핵심 color == 61 233 151

            kvec pos = null;

            if (0 <= x && x < image.Width)
                if (0 <= y && y < image.Height)
                {
                    Color cx = image.GetPixel(x, y);

                    if (cx.R < 70 && cx.G > 200 && cx.B < 160)
                        pos = new kvec(x, y);
                }

            if (pos != null)
                if (!kmath.isInPolygon(pos, bankarea, bankarea.Count))
                    return true;

            return false;
        }
        bool isColorGreen2(Bitmap image, int x, int y)
        {
            kvec pos = null;
            if (0 <= x && x < image.Width)
                if (0 <= y && y < image.Height)
                {
                    Color cx = image.GetPixel(x, y);

                    if (cx.R < 135 && cx.G > 150 && cx.B < 160)
                        pos = new kvec(x, y);
                }

            if (pos != null)
                if (!kmath.isInPolygon(pos, bankarea, bankarea.Count))
                    return true;

            return false;
        }
        kvec CoordFix(kvec vex)
        {
            return new kvec(vex.x, -vex.y);
        }
        int toDeg360(int deg)
        {
            deg = 360 - deg;

            while (deg < 0)
                deg += 360;
            while (deg > 360)
                deg -= 360;

            return deg;
        }
        internal int Deg_ToRotate(kvec posToGo)
        {
            kvec xhead = CoordFix(head);
            kvec togo = CoordFix(posToGo);
            kvec axis = togo - xhead;
            axis.normalize();

            var deg = DegBetween_Front_yAxis(axis);

            if (deg < 180)
                return -deg;
            else
                return (360 - deg);
        }
        internal int DegBetween_Front_yAxis(kvec axis)
        {
            kvec xtail = CoordFix(tail);
            kvec xhead = CoordFix(head);

            kvec front = xtail - xhead;
            front.normalize();

            var deg=(int)kvec.DegBetween(axis, front);

            return toDeg360(deg);
        }
    }
}
