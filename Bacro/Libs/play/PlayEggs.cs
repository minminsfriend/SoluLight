using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Speech.Synthesis;
using static System.Net.Mime.MediaTypeNames;

using System.Speech.Synthesis.TtsEngine;
using System.Data.SqlTypes;

using shine.libs.graphics;
using shine.libs.system;
using shine.libs.math;
using shine.libs.pad;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Bacro
{
    public class PlayEggs : Player
    {
        Bitmap bmp5kan, bmp4kan;
        int ymin = 28;
        int ymax = 41;
        int CountYori = 0;

        List<string> animals = new List<string> { "닭", "달걀", "닭고기", "오리" };

        Dictionary<string, List<kvec>> pixelsAnimal = new Dictionary<string, List<kvec>>();
        Dictionary<string, List<Color>> colorsAnimal = new Dictionary<string, List<Color>>();
        Dictionary<string, int> changsAnimal = new Dictionary<string, int>();
        Dictionary<string, int> countsAnimal = new Dictionary<string, int>();
        Dictionary<string, int> mulbunsAnimal = new Dictionary<string, int>();
        public PlayEggs(Bacro main) : base(main)
        {


        }
        public override void PlayMacro(string cmd)
        {
            if (cmd == "에그스")
                macroEggs();

        }
        void macroEggs()
        {
            int LoopTarget = 1;

            readMacroData(fileinput, ref LoopTarget, ref EngeryFull, ref EnergyOfPizza);
            AddPics();

            var workmode = "생산 판매 신청";
            //var workmode = "판매 신청";
            //var workmode = "신청";

            if (workmode == "생산 판매 신청")
            {
                var loopCount = 0;

                while (MacroRunning && loopCount < LoopTarget)
                {
                    playAll();

                    loopCount++;
                    Thread.Sleep(1000);
                }
            }
            else if (workmode == "판매 신청")
            {
                gotox("교역소");
                sell();

                gotox("교역소-서고");
                studyAsk();
            }
            else if (workmode == "신청")
            {
                gotox("교역소-서고");
                studyAsk();
            }

            MacroRunning = false;
        }
        void playAll()
        {
            RightClick("맵 센터", 1000);
            RightClick("맵 센터", 1000);

            CountYori = 0;
            int countDak, countGal, countOri;
            
            while (MacroRunning)
            {
                if (CountYori >= 20)
                    break;

                KeysClick("n", 10, 2000);

                research(1000);
                RightClick("맵 센터", 1000);
                RightClick("맵 센터", 1000);

                countDak = countsAnimal["닭"];
                countGal = countsAnimal["달걀"];
                countOri = countsAnimal["오리"];

                if (countOri >= 100)
                {
                    CountYori += 1;
                    yori("오리알");
                }
                else if (countGal >= 200 && countDak >= 100)
                {
                    CountYori += 1;
                    yori("닭고기");
                }
                else if (countDak >= 200)
                {
                    CountYori += 1;
                    yori("닭고기");
                }
                else if (countGal >= 200)
                {
                    CountYori += 1;
                    yori("닭");
                }
                else if (countDak >= 100)
                {
                    CountYori += 1;
                    yori("달걀");
                }
                else
                    break;

                Thread.Sleep(1000);
            }

            if (CountYori >= 20)
            {
                gotox("교역소");
                sell();

                gotox("교역소-서고");

                if (!studyAsk())
                    MacroRunning = false;
            }
        }
        void sell()
        {
            KeysClick("n", 10, 2000);

            research(1000);
            RightClick("맵 센터", 1000);
            RightClick("맵 센터", 1000);

            var countDak = countsAnimal["닭"];
            var countGal = countsAnimal["달걀"];

            if (countGal <= 200 && countDak <= 100)
                return;

            TabAndDetectNotFront("교역소 주인", 50, 10, 30, 1000);
            if (!DetectTarget("교역소 매각", 200, 10, 20))
                return;

            LeftClick("교역소 매각", null, 1000);

            //스캔이미지 요구
            krect rect4kan = parsePicRect(pics["판매물품 4칸"]);
            Bitmap bmp4= macro.requestImage(rect4kan, "판매물품 4칸");

            Console.WriteLine($"rect4kan == {rect4kan.toString()}");

            scanMulBuns(bmp4, ref mulbunsAnimal);
       
            //순서중요. 오리를 모두 팔면, 오리칸이 사라져서 rect가 달라질 수 있다.
            //List<string> animalssell = new List<string> { "달걀", "닭", "닭고기", "오리" };

            krect rectMul0 = rect4kan.copy();
            rectMul0.h = rectMul0.w;

            int gogicount = countsAnimal["닭고기"];

            if (gogicount > 1)
            {
                krect rectMul = rectMul0.copy();
                int gogibun = mulbunsAnimal["닭고기"];
                rectMul.offset(0, gogibun * 56);

                LeftClick(rectMul.Cen, null, null, 1000);
                KeysClick($"%{gogicount - 1}", 50, 1000);
                KeysClick("return", 50, 1000);

                //최종 매각 버튼
                LeftClick("교역소 매각 확인", null, 1000);
            }
            else
            {
                RightClick("맵 센터", 1000);
            }
        }

        void scanMulBuns(Bitmap image4kan, ref Dictionary<string, int> mulbuns)
        {
            int ocha = 30;

            //reset
            foreach (var animal in animals)
                mulbuns[animal] = -1;

            foreach (var animal in animals)
            {
                List<Color> colors = colorsAnimal[animal];
                List<kvec> pixels = pixelsAnimal[animal];

                for (int k = 0; k < 4; k++)
                {
                    List<kvec> pixelsx = kvec.ListCopy(pixels);
                    kvec.ListOffset(pixelsx, 0, 56 * k);

                    if (gx.ComparePixelsOfImage(image4kan, pixelsx, colors, ocha, 0))
                    {
                        Console.WriteLine($"{animal} : {k}번 물품");
                        mulbuns[animal] = k;
                        break;
                    }
                }
            }
        }
        public override void AddPics()
        {
            pics["도시맵"] = "757 571 10 7::4 3,5 3::221 56 54,220 56 54";

            pics["맵 교역소"] = "536 195 18 18::8 2,6 4,12 4,6 14,14 13::51 46 16,178 175 69,230 228 106,255 255 150,255 255 120";
            pics["맵 서고"] = "536 172 17 16::3 1,1 7,10 6::82 20 24,212 210 177,218 219 223";
            pics["맵 센터"] = "563 163 11 14::5 2,5 3,5 5,5 8::183 165 129,102 55 30,234 57 31,228 213 171";
            pics["맵 발디"] = "563 163 11 14::24 22,5 3,5 6,5 8,5 11::255 255 255,102 55 30,217 60 35,228 213 171,72 37 10";

            pics["적재화물 닭"] = "80 245 48 48::10 4,11 7,14 13::224 47 31,69 24 14,218 168 113";
            pics["적재화물 오리"] = "136 245 48 48::7 9,12 5,16 7::187 91 37,97 79 77,254 254 254";
            pics["적재화물 달걀"] = "192 245 48 48::5 17,16 12,23 20::224 212 207,240 227 220,207 163 138";
            pics["적재화물 닭고기"] = "80 245 48 48::6 20,14 19,13 25::143 130 117,198 176 167,167 160 155";

            pics["적재화물 5칸"] = "80 245 272 48::13 6,72 7,136 10::234 96 76,254 254 254,191 171 125";
            pics["판매물품 4칸"] = "76 135 48 216::0 0::0 0 0";

            /*넘버 패턴 w h :: pixels :: colors */
        
            foreach (var animal in animals)
            {
                var npcanimal = $"적재화물 {animal}";

                List<kvec> pixels = new List<kvec>();
                List<Color> colors = new List<Color>();
                krect rect__ = new krect();
                parsePicData(pics[npcanimal], ref rect__, ref pixels, ref colors);

                pixelsAnimal[animal] = pixels;
                colorsAnimal[animal] = colors;
                changsAnimal[animal] = -1;
                mulbunsAnimal[animal] = -1;
                countsAnimal[animal] = 0;
            }
        }
        void research(int sleep)
        {
            krect rect5kan = parsePicRect(pics["적재화물 5칸"]);

            Bitmap bmp5 = macro.requestImage(rect5kan, "적재화물 5칸");

            Console.WriteLine($"rect5kan == {rect5kan.toString()}");

            int ocha = 30;

            //reset
            foreach (var animal in animals)
            {
                countsAnimal[animal] = 0;
                changsAnimal[animal] = -1;
            }

            foreach (var animal in animals)
            {
                List<kvec> pixels = pixelsAnimal[animal];
                List<Color> colors = colorsAnimal[animal];

                for (int k = 0; k < 5; k++)
                {
                    List<kvec> pixelsx = kvec.ListCopy(pixels);
                    kvec.ListOffset(pixelsx, 56 * k, 0);

                    if (gx.ComparePixelsOfImage(bmp5, pixelsx, colors, ocha, 0))
                    {
                        Console.WriteLine($"{animal} : {k}번 창고");
                        changsAnimal[animal] = k;
                        break;
                    }
                }
            }
            //
            krect rect0 = new krect(bmp5.Height, bmp5.Height);

            foreach (var animal in animals)
            {
                int k = changsAnimal[animal];
                if (k < 0)
                    continue;

                krect rectMul = rect0.copy();
                rectMul.offset(56 * k, 0);

                string numscanx = scan_numtex(rectMul);

                Console.WriteLine($"{animal} count == {numscanx}");

                int numscan;
                if (int.TryParse(numscanx, out numscan))
                    countsAnimal[animal] = numscan;
                else
                    Console.WriteLine($"parse error : numscanx == [{numscanx}]");
            }

            Thread.Sleep(sleep);
        }
        string scan_numtex(krect rectMul)
        {
            kvec pivot = rectMul.pos();

            var numtext = "";
            int xScan = pivot.X;

            while (xScan < pivot.X + 48)
            {
                bool foundBlack = false;
                bool foundWhite = false;
                kvec pWhite = new kvec();

                for (int y = ymin; y <= ymax; y++)
                {
                    Color color = bmp5kan.GetPixel(xScan, y);

                    if (colorIsBlack(color))
                    {
                        foundBlack = true;
                    }
                    else if (foundBlack && colorIsWhite(color))
                    {
                        foundWhite = true;
                        pWhite.set(xScan, y);

                        break;
                    }
                }

                if (!foundWhite)
                    xScan += 1;

                else
                {
                    int texlen = 1;
                    int num = getNumByPatt(pWhite, ref texlen);

                    if (num > -1)
                        numtext += $"{num}";
                    else
                    {
                        numtext += "x";

                        Console.WriteLine($"pWhite == {pWhite.toString2()}");
                    }

                    xScan += texlen;
                }
            }

            return numtext;
        }
        int getNumByPatt(kvec pWhite, ref int texlen)
        {
            var dy = pWhite.Y - ymin;
            List<int> nn = new List<int>();

            if (dy == 3)
                nn = new List<int> { 2, 3, 9, 1 };
            else if (dy == 4)
                nn = new List<int> { 7, 8, 0 };
            else if (dy == 5)
                nn = new List<int> { 6, 4 };
            else if (dy == 12)
                nn = new List<int> { 5 };

            foreach (var n in nn)
            {
                List<kvec> pixelsPatt = numPatt.pixels[n];
                List<Color> colorsPatt = numPatt.colors[n];

                List<Color> colorsScan = scanColors(pWhite, pixelsPatt);

                if (compare_colors(colorsScan, colorsPatt))
                {
                    texlen = numPatt.rects[n].W + 1;
                    return n;
                }
            }

            return -1;
        }
        List<Color> scanColors(kvec pWhite, List<kvec> pixelsPatt)
        {
            kvec pivot = pWhite.copy();
            pivot.y = ymin;

            List<Color> colors = new List<Color>();

            foreach (var px in pixelsPatt)
            {
                kvec p = px.copy();
                p.offset(pivot);

                Color color = bmp5kan.GetPixel(p.X, p.Y);

                colors.Add(color);
            }

            return colors;
        }
        bool compare_colors(List<Color> colorsScan, List<Color> colorsPatt)
        {
            var OCHA = 16;
            for (int k = 0; k < colorsPatt.Count; k++)
            {
                Color c0 = colorsPatt[k];
                Color c = colorsScan[k];

                if (Math.Abs(c0.R - c.R) > OCHA)
                    return false;
                if (Math.Abs(c0.G - c.G) > OCHA)
                    return false;
                if (Math.Abs(c0.B - c.B) > OCHA)
                    return false;
            }

            return true;
        }
        bool colorIsWhite(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            if (r == g && g == b)
                if (r > 230 && g > 230 && b > 230)
                    return true;

            return false;
        }
        bool colorIsBlack(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;

            if (r == g && g == b)
                if (r < 60 && g < 60 && b < 60)
                    return true;

            return false;
        }
        bool studyAsk()//서고 들어가기- 연구 신청- 서고 나오기
        {
            TabAndDetectNotFront("서고 문", 50, 10, 30, 1000);
            KeyClickLong("w", 1000, 500);

            TabAndDetectNotFront("학자", 50, 10, 30, 1000);

            if (!DetectTarget("학자 연구", 200, 10, 20))
                return false;

            LeftClick("학자 연구", null, 1000);
            LeftClick("연구동 4번", null, 1000);
            LeftClick("버튼 다음", null, 1000);
            LeftClick("전공 1x1", null, 1000);
            LeftClick("버튼 다음", null, 1000);
            LeftClick("버튼 확인", null, 1000);

            KeyClickLong("s", 2000, 500);

            RightClick("맵 센터", 1000);

            TabAndDetectNotFront("출구", 20, 10, 30, 1000);

            KeyClickLong("w", 2000, 1000);

            return true;
        }
        void gotox(string place)
        {
            if (place == "은행")
            {
                RightClick("맵 센터", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 은행", null, 3000);
            }
            else if (place == "서고")
            {
                RightClick("맵 센터", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 서고", null, 10 * 1000);
            }
            else if (place == "교역소-서고")
            {
                RightClick("맵 센터", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 발디", null, 5 * 1000);
                LeftClick("맵 서고", null, 3 * 1000);
            }
            else if (place == "교역소")
            {
                RightClick("맵 센터", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 교역소", null, 7 * 1000);
            }
            else if (place == "도구점")
            {
                RightClick("맵 센터", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 도구점", null, 3000);
            }

            RightClick("맵 센터", 1000);
            RightClick("맵 센터", 1000);
        }
        bool buy(int nBalju)
        {
            TabAndDetectNotFront("교역소 주인", 20, 10, 20, 1000);
            if (!DetectTarget("교역소 구입", 200, 10, 20))
                return false;

            LeftClick("교역소 구입", null, 1000);

            /* 발주서 사용 */
            LeftClick("아이템 사용", null, 1000);
            LeftClick("아이템 확인", null, 1000);
            KeysClick($"{nBalju}", 10, 1000);
            LeftClick("아이템 OK", null, 1000);

            LeftClick("진열품 3번", "ctrl멍", 2000);//감자
            MouseOffset(300, 0, 1000);
            LeftClick("진열품 4번", "ctrl멍", 2000);//옥수수

            ScrollDown("스크롤바 상단", "스크롤바 하단", 500, 1000);

            LeftClick("진열품 4번", "ctrl멍", 2000);//알파카

            LeftClick("구입 확인", null, 1000);

            return true;
        }
        void yori(string tem)
        {
            FoodSlotNum = 4;

            string receipi = null;
            switch (tem)
            {
                case "달걀": receipi = "레시피 1번"; break;
                case "닭": receipi = "레시피 2번"; break;
                case "닭고기": receipi = "레시피 3번"; break;
                case "오리알": receipi = "레시피 4번"; break;
            }
            if (receipi != null)
            {
                if (DetectTarget("퀵슬롯 우버튼", 200, 10, 20))
                {
                    LeftClick("퀵슬롯 우버튼", null, 1000);
                    MouseOffset(-300, 0, 1000);
                }

                ChargeEnergyBar("행동력바", FoodSlotNum, 2000);

                KeysClick("control b", 10, 2000);
                LeftClick(receipi, "double", 1000);
                LeftClick("횟수 지정", null, 1000);
                LeftClick("횟수 Max", null, 1000);
                LeftClick("횟수 OK", null, 1000);

                LeftClick("생산종료", null, 1000);
            }
        }
    }
}
