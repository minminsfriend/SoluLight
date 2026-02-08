using Bacro;
using shine.libs.drawing;
using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Bacro
{
    public partial class Sail : Player
    {
        Dictionary<string, RectPossColors> shipsData = new Dictionary<string, RectPossColors>();
        
        Color colorGreen = Color.FromArgb(48,118,33);
        public readonly krect RectScan = new krect(150, 30, 510, 250);
        int Sleep490 = 490;
        //int Sleep490 = 200;
        bool EnablePrintShips=false;
        string NameGangZuc = "방랑 해적단";
        Numbers numbers;

        void ShotDhoByOneMinute()
        {
            ON_BATTLE = true;

            var dirCap = @"c:\Works\vs\_Images\dho cap 날짜용";

            var count = -1;
            while (++count <= 70 && ON_BATTLE)
            {
                var filecap = $@"{dirCap}\dho {count:d02}.png";
                Console.WriteLine($"화일저장 : {filecap}");

                main.capDho.SearchDhoWindows();
                Thread.Sleep(1000);

                Bitmap image = main.capDho.CapImage();
                image.Save(filecap, ImageFormat.Png);

                Thread.Sleep(59 * 1000);//60초 1분
            }

            ON_BATTLE = false;
        }
  
        void BasicEyeView()
        {
            //줌 아웃
            KeyClickLong("pgdn", 2* 1000, 200);

            // 카메라 올렸다가 천천히 내린다
            KeyClickLong("control s", 2 * 1000, 200);
            KeyClickLong("control w", 1000, 500);

            // 360도 시야 회전
            KeyClickLong("control a", 6 * 1000, 200);
            RightClick(CenFront, 200);
        }
        void setShipPics()
        {
            if (shipPics == null)
                shipPics = new Dictionary<string, string>();

            shipPics.Clear();
            var lines = File.ReadAllLines(fileships, Encoding.UTF8);

            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length == 0)
                    continue;

                // name /==/ offset x y / W H / 좌표,좌표,좌표 / 색, 색, 색
                var ss = Regex.Split(line, " /==/ ");

                var name = ss[0].Trim();
                var picx = ss[1].Trim().Replace(" / ", "::");

                shipPics[name] = picx;
            }
        }
        public List<ShipNameRect> ScanningShipsOfFront(Bitmap bmpScan, ref List<kvec> pGrs, ref bool EnemyFound)
        {
            /*검색할 배들, 데이타 채우기*/
            if (shipsData.Count == 0)
                foreach (var name in shipPics.Keys)
                    shipsData[name] = new RectPossColors(shipPics[name]);

            /* 결과 변수들 */
            List<ShipNameRect> ships = new List<ShipNameRect>();
            pGrs.Clear();

            krect rect0 = new krect(0, 0, 100, 15);

            for (int r = 0; r < 15; r++)//15*15=225
            {
                for (int c = 0; c < 5; c++)//5*100=500
                {
                    krect rectBox = rect0.copy();
                    rectBox.offset(c * 100, r * 15);

                    kvec pGreen = new kvec();

                    /*녹색점을 찾는다*/
                    if(searchGreen(bmpScan, ref pGreen, rectBox))
                    {
                        pGrs.Add(pGreen.copy());

                        /* 발견된 녹색점을 기준으로 배 찾기 */
                        foreach (var name in shipsData.Keys)
                        {
                            RectPossColors sdata = shipsData[name];
                            kvec p0 = pGreen + sdata.offx;
                            sdata.rect.x = p0.x;
                            sdata.rect.y = p0.y;

                            if (isTheShipPixlels(bmpScan, sdata))
                            {
                                /* 강적 발견!! 강적 발견!! */
                                if (name == NameGangZuc)
                                    EnemyFound = true;

                                var rect = sdata.rect.copy();
                                ships.Add(new ShipNameRect(name, rect));
                            }
                        }
                    }
                    if (EnemyFound)
                        break;
                }
                if (EnemyFound)
                    break;
            }

            return ships;
        }
        Dictionary<int, List<ShipNameRect>> ScanShipsAllAngles(ref bool EnemyFound)
        {
            Dictionary<int, List<ShipNameRect>> shipsScanned = new Dictionary<int, List<ShipNameRect>>();

            var count = 0;
            while (++count <= 11 && ON_BATTLE)
            {
                Bitmap bmpScan = macro.requestImage(RectScan);

                Thread.Sleep(100);
                List<kvec> pgreens = new List<kvec>();

                var ships = ScanningShipsOfFront(bmpScan, ref pgreens, ref EnemyFound);
            
                var index = count - 1;
                shipsScanned[index] = ships ;

                if (EnemyFound)
                {
                    /* 스캔을 위한 회전이 멈춘다 */
                    break;
                }

                printOnCapView(bmpScan, pgreens, ships, index);

                Thread.Sleep(50);

                /*다음을 위해 회전*/
                if (count <= 10)
                    EyeRotate("a", 50);
                /*마지막은 샷 찍고 멈춤*/
                else if (count == 11)
                { }
            }

            return shipsScanned;
        }
        int GangZucSearch(string word)
        {
            /*
             "강적이 기습할 때를", 
             "강한 적이 도전", 
             "강적을 격퇴", 
             "현재 격파수는"
             */
     
            var nline = DetectWord5LinesFixedX($"{word}", 20, 5, 20);

            if (nline > -1)
            {
                if (word == "강한 적이 도전")
                {
                    Console.WriteLine($"발견!! : <{word}>");
                }
            }
            else//실패할 경우
            {
                if (word == "강한 적이 도전")
                {
                    Console.WriteLine($"아직 못발견 : <{word}>");
                }
            }

            return nline;
        }
        void EyeRotate(string direc, int sleepEnd)
        {
            if (direc == "left") direc = "a";
            if (direc == "right") direc = "d";

            KeyClickLong($"control {direc}", Sleep490, sleepEnd);
        }
        bool isTheShipPixlels(Bitmap bmpScan, RectPossColors rpc)
        {
            kvec p0 = rpc.rect.pos();
            int n = -1;

            foreach (var pxx in rpc.poss)
            {
                kvec px = p0 + pxx;
                var X = px.X;
                var Y = px.Y;

                if (X < 0 || Y < 0)
                    return false;
                if (X > bmpScan.Width - 1 || Y > bmpScan.Height - 1)
                    return false;

                Color color = bmpScan.GetPixel(X, Y);

                /*한개라도 틀리면 끝이다*/
                if (color != rpc.colors[++n])
                    return false;
            }

            return true;
        }
        void getMaxYofShips(ref int nrot, ref string name, ref krect rect, Dictionary<int, List<ShipNameRect>> shipsScanned)
        {
            float yMax = 0;
            int n = -1;

            foreach(var ships in shipsScanned.Values)
            {
                n++;
                foreach (var ship in ships)
                {
                    if (ship.name == "실비아")
                        continue;
 
                    if (ship.rect.y > yMax)
                    {
                        name = ship.name;
                        rect = ship.rect.copy();
                        yMax = rect.y;
                        nrot = n;
                    }
                }
            }
        }
        void getMaxYofShip(ref string nameShip, ref krect rectShip, List<ShipNameRect> ships)
        {
            float yMax = 0;

            foreach (var s in ships)
            {
                if (s.name == "실비아")
                    continue;

                if (s.rect.y > yMax)
                {
                    nameShip = s.name;
                    rectShip = s.rect.copy();
                    yMax = rectShip.y;
                }
            }
        }
        bool checkLeftUpColors(Bitmap bmpScan, int x, int y)
        {
            for (int i = 1; i <= 5; i++)
            {
                kvec posL = new kvec(x - i, y);
                if (posL.X < 0)
                    return false;

                var colorL = bmpScan.GetPixel(posL.X, posL.Y);
                if (colorL == colorGreen || colorL == Color.White)
                    return false;
            }
            for (int i = 1; i <= 5; i++)
            {
                kvec posT = new kvec(x, y - 1);
                if (posT.Y < 0)
                    return false;

                var colorT = bmpScan.GetPixel(posT.X, posT.Y);
                if (colorT == colorGreen || colorT == Color.White)
                    return false;
            }

            return true;
        }
        bool searchGreen(Bitmap bmpScan, ref kvec pos, krect rectBox)
        {
            for (int x = rectBox.X; x < rectBox.Right; x++)
            {
                if (x > bmpScan.Width - 1)
                    break;

                for (int y = rectBox.Y; y < rectBox.Bottom; y++)
                {
                    if (y > bmpScan.Height - 1)
                        break;

                    if (colorGreen == bmpScan.GetPixel(x, y))
                    {
                        if (checkLeftUpColors(bmpScan, x, y))
                        {
                            pos = new kvec(x, y);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        void printOnCapView(Bitmap bmpScan, List<kvec> pgreens, List<ShipNameRect> ships, int index)
        {
            var text = $"[{index}] ";
            List<krect> rects = new List<krect>();
            foreach (var ship in ships)
            {
                rects.Add(ship.rect);
                var pos = ship.rect.Cen;
                text += $"<{ship.name} ({pos.X} {pos.Y})>";
            }
            Console.WriteLine($"탐지: {text}");

            if (EnablePrintShips)
                main.capView.PrintShipBoxs(bmpScan, rects, pgreens);
        }
    }
}
