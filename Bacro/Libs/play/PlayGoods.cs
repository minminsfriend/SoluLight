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
    public class PlayGoods : Player
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

        Dictionary<string, kvec> poss = new Dictionary<string, kvec>();
        Dictionary<string, krect> rects = new Dictionary<string, krect>();

        kvec scrollOffset = new kvec(0, 56 * 4);
        List<kvec> pixelsPizza = new List<kvec>();
        List<Color> colorsPizza = new List<Color>();
        kvec vecMax = new kvec(200, 431) - new kvec(107, 330);

        public PlayGoods(Bacro main) : base(main)
        {


        }
        void fillPoss()
        {
            pixelsPizza = kvec.ParseList("4 13,11 16,13 20,16 19");
            colorsPizza = gx.ParseColors("219 218 216,234 230 183,239 180 112,93 104 28");

            poss["아이템 사용"] = kvec.Parse2d("493 332");
            poss["사용 확인"] = kvec.Parse2d("470 447");
            poss["발주서 숫자 OK"] = kvec.Parse2d("377 336");

            poss["진열품 1번"] = kvec.Parse2d("100 160");

            poss["스크롤 A"] = kvec.Parse2d("343 158");
            poss["스크롤 B"] = kvec.Parse2d("343 210");

            poss["OK Vec"] = kvec.Parse2d("193 389") - kvec.Parse2d("98 213");

            poss["교역소 구입"] = kvec.Parse2d("652 386");
            poss["구입 OK"] = kvec.Parse2d("666 448");

            poss["교역소 매각"] = kvec.Parse2d("689 386");
            poss["매각 OK"] = kvec.Parse2d("660 450");
            poss["모두 매각"] = kvec.Parse2d("304 113");

            // 도구점 
            poss["도구점 매각 확인"] = new kvec(652, 444);
            poss["도구점 매각 예정판"] = new kvec(583, 245);
            //소유 아이템
            poss["보관함 버튼"] = new kvec(550, 411);
            poss["보관함 보충"] = new kvec(374, 102);
            poss["보관함 확인"] = new kvec(626, 455);
            //은행
            poss["대여금고 보충"] = new kvec(349, 116);
            poss["대여금고 확인"] = new kvec(597, 464);

            rects["판매아이템 1x1"] = krect.Parse("81 137 48 48");
            rects["소유아이템 1x1"] = krect.Parse("187 129 48 48");
            rects["은보아이템 1x1"] = krect.Parse("81 143 48 48");
        }
        void readData(string fileinput, ref int loopTarget, ref string dataPower, ref string dataTry)
        {
            var lines = File.ReadAllLines(fileinput, Encoding.UTF8);
            string[] ss, nn;

            foreach (var linex in lines)
            {
                var line = linex.Trim();

                if (line.Length < 3) continue;
                if (line.Substring(0, 2) == "//") continue;

                ss = Regex.Split(line, "::");
                if (ss.Length == 2)
                {
                    var name = ss[0].Trim();
                    var text = ss[1].Trim();

                    if (name == "LoopTarget")
                        loopTarget = int.Parse(text);

                    else if (name == "Power Pizza")
                    {
                        dataPower = text.Trim();
                    }
                    //생산 변수 :: bal10 A0 B2
                    else if (name == "@생산 변수")
                    {
                        dataTry = text.Trim();
                    }
                }
            }
        }
        public override void PlayMacro(string cmd)
        {
            if (cmd == "와인비니거")
                playMacro_vineger();
            else if (cmd == "피자")
                playMacro_pizza();
            else if (cmd == "놋쇠")
                playMacro_notshye();

            if (!MacroRunning)
            {
                Console.WriteLine("중단 매크로!");
            }

            VoiceSpeak("매크로가 종료되었습니다. 매크로가 종료되었습니다.");

            MacroRunning = false;
        }
        void playMacro_pizza()
        {
            int LoopTarget = 1;
            string dataTry = null;
            string dataPower = null;

            readData(fileinput, ref LoopTarget, ref dataPower, ref dataTry);
            AddPics();

            int BalCount = 1;
            int nTryMulA = 0;
            int nTryMulB = 0;
            FoodSlotNum = 4;

            bool doSell = true;

            if (dataPower != null)
            {
                var nn = Regex.Split(dataPower, " ");

                EngeryFull = int.Parse(nn[0].Trim());
                EnergyOfPizza = int.Parse(nn[1].Trim());
                FoodSlotNum = int.Parse(nn[2].Trim());
            }
            //생산 변수 :: bal10 x0 y2
            if (dataTry != null)
            {
                var nn = Regex.Split(dataTry, " ");

                BalCount = int.Parse(nn[0].Trim().Substring(3));
                nTryMulA = int.Parse(nn[1].Trim().Substring(1));
                nTryMulB = int.Parse(nn[2].Trim().Substring(1));
                doSell = nn[3].Trim() == "sell";
            }

            fillPoss();

            //LoopTarget = 1;
            var loopCount = 0;

            while (loopCount < LoopTarget)
            {
                Console.WriteLine($"해물피자 # {loopCount + 1}/{LoopTarget}");

                RightClick("맵 센터", 1000);
                buyForPizza(BalCount);

                //loopCount++;
                //continue;

                gotox("도구점", 3);

                product("밀가루", 0, -2, 2);
                product("해물피자", 0, -1, 1);
                storePizza();
                sellPizza();

                for (int pz = 0; pz < 3; pz++)
                {
                    product("해물피자", 0, -1, 1);
                    sellPizza();
                }

                gotox("교역소", 3);

                product("햄", 0, 0, 2);
                product("햄", 0, 0, 2);
                product("햄", 0, 0, 1);

                sell();

                loopCount++;
            }
        }
        void playMacro_vineger()
        {
            int LoopTarget = 1;
            string dataTry = null;
            string dataPower = null;

            readData(fileinput, ref LoopTarget, ref dataPower, ref dataTry);
            AddPics();

            int BalCount = 1;
            int nTryMulA = 0;
            int nTryMulB = 0;
            FoodSlotNum = 4;

            bool doSell = true;

            if (dataPower != null)
            {
                var nn = Regex.Split(dataPower, " ");

                EngeryFull = int.Parse(nn[0].Trim());
                EnergyOfPizza = int.Parse(nn[1].Trim());
                FoodSlotNum = int.Parse(nn[2].Trim());
            }
            //생산 변수 :: bal10 x0 y2
            if (dataTry != null)
            {
                var nn = Regex.Split(dataTry, " ");

                BalCount = int.Parse(nn[0].Trim().Substring(3));
                nTryMulA = int.Parse(nn[1].Trim().Substring(1));
                nTryMulB = int.Parse(nn[2].Trim().Substring(1));
                doSell = nn[3].Trim() == "sell";
            }

            //BalCount = 7;
            //doSell = false;
            //nTryMulA = 6;
            /* 발주서 7장 854개 140x6*/

            fillPoss();

            var loopCount = 0;

            while (loopCount < LoopTarget)
            {
                Console.WriteLine($"와인비니거 # {loopCount + 1}/{LoopTarget}");

                RightClick("맵 센터", 1000);
                buyWines(BalCount);

                //loopCount++;
                //continue;

                for (int k = 0; k < nTryMulA; k++)
                    product("와인비니거", 0, 0, 1);

                sell();

                loopCount++;
            }
        }
        void playMacro_notshye()
        {
            int LoopTarget = 1;
            string dataTry = null;
            string dataPower = null;

            readData(fileinput, ref LoopTarget, ref dataPower, ref dataTry);
            AddPics();

            int BalCount = 1;
            int nTryMulA = 0;
            int nTryMulB = 0;
            FoodSlotNum = 4;

            bool doSell = true;

            if (dataPower != null)
            {
                var nn = Regex.Split(dataPower, " ");

                EngeryFull = int.Parse(nn[0].Trim());
                EnergyOfPizza = int.Parse(nn[1].Trim());
                FoodSlotNum = int.Parse(nn[2].Trim());
            }
            //생산 변수 :: bal10 x0 y2
            if (dataTry != null)
            {
                var nn = Regex.Split(dataTry, " ");

                BalCount = int.Parse(nn[0].Trim().Substring(3));
                nTryMulA = int.Parse(nn[1].Trim().Substring(1));
                nTryMulB = int.Parse(nn[2].Trim().Substring(1));
                doSell = nn[3].Trim() == "doSell";
            }

            if(false)
            {
                LoopTarget = 1;
                BalCount = 7;
                nTryMulA = 4;
                doSell = false;
            }

            /* 발주서 7장 532개 140x4*/

            fillPoss();

            var loopCount = 0;

            while (loopCount < LoopTarget)
            {
                Console.WriteLine($"놋쇠 # {loopCount + 1}/{LoopTarget}");

                RightClick("맵 센터", 1000);
                buyGuris(BalCount);

                //loopCount++;
                //continue;

                product("놋쇠", 0, 0, 2);

                if (doSell)
                    sell();

                loopCount++;
            }
        }
        void storePizza()
        {
            var posStore = poss["보관함 버튼"];
            var posFill = poss["보관함 보충"];
            var posOk = poss["보관함 확인"];

            KeysClick("o", 100, 2000);
            LeftClick(posStore, null, null, 1000);
            LeftClick(posFill, null, null, 1000);
            LeftClick(posOk, null, null, 1000);
        }
        bool sellPizza()
        {
            TabAndDetectNotFront("도구점 주인", 50, 10, 20, 1000);
            if (!DetectTarget("도구점 매각", 200, 10, 20))
                return false;

            LeftClick("도구점 매각", null, 2000);

            int YOFF = 56 * 2;

            krect rectShot = rects["판매아이템 1x1"].copy();
            rectShot.offset(0, YOFF);
            rectShot.w = 56 * 4;

            Bitmap bmp4= macro.requestImage(rectShot, "pizza");

            krect recttem = null;
            var ocha = 30;

            for (int k = 0; k < 4; k++)
            {
                List<kvec> pixelsx = kvec.ListCopy(pixelsPizza);
                kvec.ListOffset(pixelsx, 56 * k, 0);

                if (gx.ComparePixelsOfImage(bmp4, pixelsx, colorsPizza, ocha, 0))
                {
                    recttem = rects["판매아이템 1x1"].copy();
                    recttem.offset(56 * k, YOFF);
                    break;
                }
            }

            if (recttem != null)
            {
                Console.WriteLine("pizza Found");

                LeftClick2(recttem.Cen, 1000);
                LeftClick(recttem.Cen + vecMax, null, null, 1000);
                KeysClick("enter", 100, 1000);

                LeftClick(poss["도구점 매각 확인"], null, null, 1000);
            }
            else
            {
                Console.WriteLine("not pizza Found");

                RightClick(poss["도구점 매각 예정판"], 1000);
            }

            return true;
        }
        void sell()
        {
            TabAndDetectNotFront("교역소 주인", 20, 10, 30, 1000);
            if (!DetectTarget("교역소 매각", 200, 10, 20))
                return;

            LeftClick("교역소 매각", null, 1000);

            LeftClick("전부매각", null, 1000);

            //최종 매각
            LeftClick("교역소 매각 확인", null, 1000);
        }
        public override void AddPics()
        {
            pics["도시맵"] = "757 571 10 7::4 3,5 3::221 56 54,220 56 54";

            //리스본
            //npcs["맵 교역소"] = "536 195 18 18::8 2,6 4,12 4,6 14,14 13::51 46 16,178 175 69,230 228 106,255 255 150,255 255 120";
            //npcs["맵 서고"] = "536 172 17 16::3 1,1 7,10 6::82 20 24,212 210 177,218 219 223";
            //npcs["맵 센터"] = "563 163 11 14::5 2,5 3,5 5,5 8::183 165 129,102 55 30,234 57 31,228 213 171";
            //npcs["맵 발디"] = "563 163 11 14::24 22,5 3,5 6,5 8,5 11::255 255 255,102 55 30,217 60 35,228 213 171,72 37 10";

            pics["적재화물 닭"] = "80 245 48 48::10 4,11 7,14 13::224 47 31,69 24 14,218 168 113";
            pics["적재화물 오리"] = "136 245 48 48::7 9,12 5,16 7::187 91 37,97 79 77,254 254 254";
            pics["적재화물 달걀"] = "192 245 48 48::5 17,16 12,23 20::224 212 207,240 227 220,207 163 138";
            pics["적재화물 닭고기"] = "80 245 48 48::6 20,14 19,13 25::143 130 117,198 176 167,167 160 155";

            pics["적재화물 5칸"] = "80 245 272 48::13 6,72 7,136 10::234 96 76,254 254 254,191 171 125";
            pics["판매물품 4칸"] = "76 135 48 216::0 0::0 0 0";

            //포르투
            pics["맵 교역소"] = "422 178 18 18::9 2,4 4,12 4,6 14,14 14::191 186 81,223 221 101,230 228 106,255 255 150,255 255 147";
            pics["맵 도구점"] = "476 187 17 18::8 2,3 7,6 7,10 7,14 7,8 11::200 199 199,214 213 212,207 114 0,215 147 1,225 224 223,88 50 26";



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

            Bitmap bmp5= macro.requestImage(rect5kan, "적재화물 5칸");

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
            krect rect0 = new krect(bmp5kan.Height, bmp5kan.Height);

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
        void gotox(string place, int walkingSec)
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
                LeftClick("맵 교역소", null, walkingSec * 1000);
            }
            else if (place == "도구점")
            {
                RightClick("맵 센터", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 도구점", null, walkingSec * 1000);
            }

            RightClick("맵 센터", 1000);
            RightClick("맵 센터", 1000);
        }
        bool buyForPizza(int nBalju)
        {
            TabAndDetectNotFront("교역소 주인", 20, 10, 20, 1000);
            if (!DetectTarget("교역소 구입", 200, 10, 20))
                return false;

            kvec BalIcon = poss["아이템 사용"];
            kvec BalUseOk = poss["사용 확인"];
            kvec BalNumOk = poss["발주서 숫자 OK"];

            kvec scrollA = poss["스크롤 A"];

            kvec mul_1 = poss["진열품 1번"];
            kvec mul_2 = mul_1 + new kvec(0, 56 * 1);
            kvec mul_3 = mul_1 + new kvec(0, 56 * 2);
            kvec mul_4 = mul_1 + new kvec(0, 56 * 3);

            kvec buyButton = poss["교역소 구입"];
            kvec buyOk = poss["구입 OK"];

            kvec sellButton = poss["교역소 매각"];
            kvec sellOk = poss["매각 OK"];
            kvec sellAll = poss["모두 매각"];

            scrollOffset = new kvec(0, 56 * 4);

            LeftClick(buyButton, null, null, 2000);

            // 발주서 1장 사용
            LeftClick(BalIcon, null, null, 1000);
            LeftClick(BalUseOk, null, null, 1000);
            KeysClick($"%{nBalju}", 100, 1500);
            LeftClick(BalNumOk, null, null, 1000);
            // 어육 200 밀 80

            int COUNT_CHIZ = 360;
            int COUNT_MILL = 145;

            LeftClick2(mul_1, 1000);
            EnterCount(mul_1, COUNT_CHIZ, 1000);
            LeftClick2(mul_2, 1000);
            EnterCount(mul_2, COUNT_MILL, 1000);

            LeftClick2(mul_4, 1000);
            LeftClick(mul_4 + vecMax, null, null, 1000);
            KeysClick("enter", 100, 2000);

            var offset = new kvec(0, 56 * 2);
            var scrollB = scrollA + offset;

            ScrollDownOff(scrollA, offset, 500, 500);
            ScrollDownOff(scrollB, offset, 500, 1000);

            LeftClick2(mul_3, 1000);
            EnterCount(mul_3, COUNT_CHIZ, 1000);

            LeftClick(buyOk, null, null, 1000);

            return true;
        }
        bool buyGuris(int nBalju)
        {
            TabAndDetectNotFront("교역소 주인", 20, 10, 20, 1000);
            if (!DetectTarget("교역소 구입", 200, 10, 20))
                return false;

            LeftClick("교역소 구입", null, 1000);

            /* 발주서 사용 */
            LeftClick("아이템 사용", null, 1000);
            LeftClick("아이템 확인", null, 1000);
            KeysClick($"%{nBalju}", 10, 1000);
            LeftClick("아이템 OK", null, 1000);

            ScrollDown("스크롤바 상단", "스크롤바 하단", 500, 1000);

            LeftClick("진열품 1번", "ctrl멍", 2000);
            LeftClick("진열품 3번", "ctrl멍", 2000);

            LeftClick("구입 확인", null, 1000);

            return true;
        }
        bool buyWines(int balCount)
        {
            TabAndDetectNotFront("교역소 주인", 20, 10, 20, 1000);
            if (!DetectTarget("교역소 구입", 200, 10, 20))
                return false;

            kvec BalIcon = poss["아이템 사용"];
            kvec BalUseOk = poss["사용 확인"];
            kvec BalNumOk = poss["발주서 숫자 OK"];

            kvec mul_1 = poss["진열품 1번"];
            kvec mul_2 = mul_1 + new kvec(0, 56 * 1);
            kvec mul_3 = mul_1 + new kvec(0, 56 * 2);
            kvec mul_4 = mul_1 + new kvec(0, 56 * 3);

            kvec buyButton = poss["교역소 구입"];
            kvec buyOk = poss["구입 OK"];

            LeftClick(buyButton, null, null, 2000);

            // 발주서 1장 사용
            LeftClick(BalIcon, null, null, 1000);
            LeftClick(BalUseOk, null, null, 1000);
            KeysClick($"%{balCount}", 100, 1500);
            LeftClick(BalNumOk, null, null, 1000);
            // 어육 200 밀 80

            var posScrollArrow = new kvec(344, 348);
            LeftClick(posScrollArrow, null, null, 1000);

            LeftClick2(mul_4, 1000);
            LeftClick(mul_4 + vecMax, null, null, 1000);
            KeysClick("enter", 100, 2000);

            LeftClick(buyOk, null, null, 1000);

            return true;
        }
        bool buySheeps(int nBalju)
        {
            TabAndDetectNotFront("교역소 주인", 20, 10, 20, 1000);
            if (!DetectTarget("교역소 구입", 200, 10, 20))
                return false;

            LeftClick("교역소 구입", null, 1000);

            /* 발주서 사용 */
            LeftClick("아이템 사용", null, 1000);
            LeftClick("아이템 확인", null, 1000);
            KeysClick($"%{nBalju}", 10, 1000);
            LeftClick("아이템 OK", null, 1000);

            ScrollDown("스크롤바 상단", "스크롤바 하단", 500, 1000);

            LeftClick("진열품 4번", "ctrl멍", 2000);

            LeftClick("구입 확인", null, 1000);

            return true;
        }
        void product(string tem, int slotnum, int index, int repeat)
        {
            int keyINV = 200;

            if (DetectTarget("퀵슬롯 우버튼", 200, 10, 20))
            {
                LeftClick("퀵슬롯 우버튼", null, 1000);
                MouseOffset(-300, 0, 1000);
            }

            ChargeEnergyBar("행동력바", FoodSlotNum, 2000);

            if (slotnum == 0)
                //마이 레시피
                KeysClick("control b", 10, 2000);
            else
                //퀵슬롯 넘버
                KeysClick($"{slotnum}", 10, 2000);

            string updownkey = "";
            int timeonclicks = keyINV * Math.Abs(index);

            if (index == 0) { }
            else if (index > 0)
            {
                for (int k = 1; k <= index; k++)
                    updownkey += "▼";
            }
            else if (index < 0)
            {
                for (int k = 1; k <= Math.Abs(index); k++)
                    updownkey += "▲";
            }

            if (updownkey != "")
                KeysClick($"%{updownkey}", keyINV, timeonclicks + 1000);
            KeysClick("return", 10, 1000);

            for (int k = 0; k < repeat; k++)
            {
                LeftClick("횟수 지정", null, 1000);
                LeftClick("횟수 Max", null, 1000);
                LeftClick("횟수 OK", null, 1000);
            }

            LeftClick("생산종료", null, 1000);
        }
        void ClickCount(int count)
        {
            KeysClick($"%{count}", 100, 1500);
            KeysClick("enter", 100, 1000);
        }
        void EnterCount(kvec posMul, int count, int sleep)
        {
            if (count == -1)
            {
                LeftClick(posMul + vecMax, null, null, 1000);
                KeysClick("enter", 100, 1000);
            }
            else
            {
                KeysClick($"%{count}", 100, 1500);
                KeysClick("enter", 100, 1000);
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
    }
}
