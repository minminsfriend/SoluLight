using shine.libs.graphics;
using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Bacro
{
    public class Compas
    {
        Bitmap ImageCoord = null;
        Bitmap ImagePass = null;
        internal Bitmap ImageOut = null;

        public int Deg360 = 0;
        public int Deg360Tar = 0;
        public kvec CoordTar = new kvec(0, 0);
        kvec CoordShip = new kvec(0, 0);
        readonly kvec cenPass = kvec.Parse2d("63 44");
        readonly kvec axisNorth = new kvec(0, 1);

        public bool SucceedScanCoord = false;
        public bool SucceedScanPas = false;

        public Compas()
        {

        }
        internal void setImageCoord(Bitmap theImage)
        {
            if (ImageCoord != null)
            {
                ImageCoord.Dispose();
                ImageCoord = null;
            }

            ImageCoord = new Bitmap(theImage);

            var coord = scanMyCoord();
            if (coord != null)
                CoordShip = coord;

            SucceedScanCoord = coord != null;
        }
        internal void setImagePas(Bitmap theImage)
        {
            if (ImagePass != null)
            {
                ImagePass.Dispose();
                ImageCoord = null;
            }

            ImagePass = new Bitmap(theImage);

            var deg = scanCompassDeg();
            if (deg > -1)
                Deg360 = deg;

            SucceedScanPas = deg > -1;
        }
        internal void BuildImageOut()
        {
            //throw new NotImplementedException();
        }
        kvec scanMyCoord()
        {
            if (ImageCoord == null)
                return new kvec();

            krect rectImage = new krect(ImageCoord.Width, ImageCoord.Height);

            int colScan = 0;
            var numtex = "";

            while (colScan < rectImage.W)
            {
                kvec pivot = null;

                for (int h = 0; h < rectImage.H; h++)
                {
                    kvec p = new kvec(colScan, h);

                    if (isPixelWhite(0, 0, ImageCoord, p))
                    {
                        pivot = p;
                        break;
                    }
                }

                if (pivot != null)
                {
                    int scanShift = 1;
                    int num = findNumberByPattNavi(ImageCoord, pivot, ref scanShift);
                    if (num > -1)
                    {
                        if (num == 100)
                            numtex += $",";
                        else
                            numtex += $"{num}";

                        colScan += scanShift;
                    }
                    else
                        colScan += 1;
                }
                else
                    colScan += 1;
            }

            var nn = Regex.Split(numtex, ",");
            int x, y;

            if (nn.Length == 2)
            {
                if (int.TryParse(nn[0].Trim(), out x))
                    if (int.TryParse(nn[1].Trim(), out y))
                        return new kvec(x, y);
            }

            return null;
        }
        bool isPixelWhite(int dx, int dy, Bitmap bmp, kvec pivot)
        {
            kvec px = pivot + new kvec(dx, dy);
            Color cpx;

            if (gx.GetPixel(bmp, px, out cpx))
            {
                if (cpx.R > 230 && cpx.G > 230 && cpx.B > 230)
                {
                    return true;
                }
            }

            return false;
        }
        int findNumberByPattNavi(Bitmap image, kvec pivot, ref int scanShift)
        {
            int number = -1;
            int h = pivot.Y;

            if (h == 7)//comma
            {
                number = 100;
                scanShift += 3;
            }
            else if (h == 5)//4
            {
                number = 4;
                scanShift += 5;
            }
            else if (h == 0)//5,7
            {
                if (isPixelWhite(0, 1, image, pivot))
                {
                    number = 5;
                    scanShift += 4;
                }
                else
                {
                    number = 7;
                    scanShift += 4;
                }
            }
            else if (h == 1)
            {
                int k = -1;
                List<int> nums = new List<int> { 1, 2, 0, 3, 8, 6, 9 };
                List<int> shift = new List<int> { 2, 4, 4, 4, 4, 4, 4 };

                while (number == -1)
                {
                    k++;
                    switch (nums[k])
                    {
                        case 1:
                            if (!isPixelWhite(0, 1, image, pivot))
                            {
                                number = nums[k];
                                scanShift += shift[k];
                            }
                            break;
                        case 2:
                            if (isPixelWhite(0, 7, image, pivot))
                            {
                                number = nums[k];
                                scanShift += shift[k];
                            }
                            break;
                        case 0:
                            if (isPixelWhite(0, 3, image, pivot))
                                if (isPixelWhite(3, 3, image, pivot))
                                {
                                    number = nums[k];
                                    scanShift += shift[k];
                                }

                            break;
                        case 3:
                            if (isPixelWhite(0, 1, image, pivot))
                                if (!isPixelWhite(0, 2, image, pivot))
                                    if (!isPixelWhite(0, 3, image, pivot))
                                        if (!isPixelWhite(0, 4, image, pivot))
                                            if (isPixelWhite(0, 5, image, pivot))
                                            {
                                                number = nums[k];
                                                scanShift += shift[k];
                                            }

                            break;
                        case 8:
                            if (isPixelWhite(0, 1, image, pivot))
                                if (isPixelWhite(0, 2, image, pivot))
                                    if (!isPixelWhite(0, 3, image, pivot))
                                        if (isPixelWhite(0, 4, image, pivot))
                                            if (isPixelWhite(0, 5, image, pivot))
                                            {
                                                number = nums[k];
                                                scanShift += shift[k];
                                            }

                            break;
                        case 6:
                            if (isPixelWhite(0, 3, image, pivot))
                                if (!isPixelWhite(3, 3, image, pivot))
                                {
                                    number = nums[k];
                                    scanShift += shift[k];
                                }

                            break;
                        case 9:
                            if (!isPixelWhite(0, 3, image, pivot))
                                if (isPixelWhite(3, 3, image, pivot))
                                {
                                    number = nums[k];
                                    scanShift += shift[k];
                                }

                            break;
                    }
                }
            }

            return number;
        }
        int scanCompassDeg()
        {
            if (ImagePass == null)
                return -1;

            int CountMax = 0;
            int Deg = -1;

            for (int deg = 0; deg <= 359; deg++)
            {
                float area = getArea(deg);
                kvec arrow = getArrow(deg);
                List<kvec> pixels = getYellowPixels(ImagePass, cenPass, arrow, area);

                if (pixels.Count > CountMax)
                {
                    CountMax = pixels.Count;
                    Deg = deg;
                }

                if (Deg > 15)//이미 걸렸는데
                {
                    //다시 안나오면 끝난 것.
                    //Deg > 15 : 350~10 사이에 걸리는 경우 때문에
                    if (pixels.Count == 0)
                        break;
                }
            }

            if (Deg > -1)
                return fixDeg360(360 - Deg);
            else
                return -1;
        }
        List<kvec> getYellowPixels(Bitmap imageCompass, kvec center, kvec arrow, float area)
        {
            arrow.y *= -1f;
            kvec pos = center + area * arrow;

            List<kvec> pixels = new List<kvec>();

            for (int x = pos.X - 3; x <= pos.X + 3; x++)
            {
                for (int y = pos.Y - 3; y <= pos.Y + 3; y++)
                {
                    kvec px = new kvec(x, y);

                    if (isPixelYellow(imageCompass, px))
                    {
                        pixels.Add(px);
                    }
                }
            }

            return pixels;
        }
        bool isPixelYellow(Bitmap bmp, kvec px)
        {
            Color cpx;

            if (gx.GetPixel(bmp, px, out cpx))
            {
                if (cpx.R > 200 && cpx.G > 200 && cpx.B < 100)
                {
                    return true;
                }
            }

            return false;
        }
        kvec getArrow(int deg)
        {
            kvec arrowx = new kvec(0, 1, 0);
            kmat matRot = new kmat();

            kvec axis = new kvec(0, 0, -1);
            matRot.setRotate(axis, deg);

            kvec arrow = matRot.Transform(arrowx);

            return arrow;
        }
        float getArea(int deg)
        {
            float slope = (54 - 38) / 90f;
            float radius;

            if (0 <= deg && deg <= 90)
                radius = 38 + slope * deg;
            else if (270 <= deg && deg <= 359)
                radius = 54 - slope * (deg - 270);
            else
                radius = 54;

            return radius;
        }
        internal kvec getCoord()
        {
            return CoordShip.copy();
        }
        internal int DegToRotate()
        {
            Deg360Tar = DegTarFromNorth();

            var degRot = Deg360Tar - Deg360;
            degRot = fixDeg360(degRot);

            if (degRot < 180)
                return degRot;
            else
                return -(360 - degRot);
        }
        void CoordFixx(ref kvec head, ref kvec tail)
        {
            if (head.x < 1000 && tail.x > 15383)// 우에서 좌로 넘어갈 때
                head.x += 16383;

            else if (head.x > 15383 && tail.x < 1000)// 좌에서 우로 넘어갈 때
                tail.x += 16383;

            head = new kvec(head.x, -1f * head.y);
            tail = new kvec(tail.x, -1f * tail.y);
        }
        int fixDeg360(int deg)
        {
            //deg = 360 - deg;

            while (deg < 0)
                deg += 360;
            while (deg > 360)
                deg -= 360;

            return deg;
        }
        internal int DegTarFromNorth()
        {
            kvec xhead = CoordShip.copy();
            kvec xtail = CoordTar.copy();

            CoordFixx(ref xhead, ref xtail);

            kvec front = xtail - xhead;
            front.normalize();

            var deg = (int)kvec.DegBetween(axisNorth, front);

            return fixDeg360(360 - deg);
        }
        internal float LenToMove()
        {
            kvec xtail = CoordTar.copy();
            kvec xhead = CoordShip.copy();
            CoordFixx(ref xhead, ref xtail);

            kvec dis = xtail - xhead;

            return dis.length();
        }
        internal string testCoord(kvec posTar)
        {
            kvec posMade = kvec.Parse2d("15474 3432");
            //kvec posTar = kvec.Parse2d("15255 3403");

            kvec posAzo = kvec.Parse2d("14740 3145");

            kvec xtail = posTar.copy();
            kvec xhead = posAzo.copy();
            CoordFixx(ref xhead, ref xtail);

            kvec front = xtail - xhead;
            front.normalize();

            var deg = (int)kvec.DegBetween(axisNorth, front);

            deg = fixDeg360(360 - deg);

            return $"{deg}";
        }
        void testCalc()
        {
            Dictionary<string, kvec> poss = new Dictionary<string, kvec>();
            poss["마데이라"] = kvec.Parse2d("15476 3439");

            poss["아조레스"] = kvec.Parse2d("14736 3147");
            poss["리스본"] = kvec.Parse2d("15800 3191");
            poss["아르긴"] = kvec.Parse2d("15610 3975");
            poss["카보베르데"] = kvec.Parse2d("15466 4205");
            poss["산후안"] = kvec.Parse2d("13608 4047");

            foreach (var key in poss.Keys)
            {
                if (key + "" != "마데이라")
                {
                    kvec vec = poss[key] - poss["마데이라"]; ;
                    //vec.normalize();

                    kvec compass = new kvec(0, 1);

                    var deg = (int)kvec.getDotDeg(compass, vec);
                    Console.WriteLine($"Deg {key} == {deg}");
                }
            }

            Console.ReadLine();
        }
    }
}

/*
    나의 정면 각도 : 나침반이 기준
    타겟 각도: 나침반이 기준

    회전할 각도: 타겟 각도 - 내 정면 각도

    내 정면 각도: 나침반을 스캔해서 알아낸다.
    타겟 각도: 벡터 좌표 계산으로 알아낸다.

    #나침판

    rect 650 517 126 109

    63 44,  센터
    9 44,   좌 r==54
    63 6,   상 r==38
    116 43, 우 r==54
    63 98,  하 r==54
*/
