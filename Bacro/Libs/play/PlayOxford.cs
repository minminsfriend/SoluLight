using shine.libs.graphics;
using shine.libs.math;
using shine.libs.pad;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Speech.Synthesis;
using System.Speech.Synthesis.TtsEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static shine.libs.konst.Com;
using static System.Net.Mime.MediaTypeNames;

namespace Bacro
{
    public class PlayOxford : Player
    {
        int ymin = 28;
        int ymax = 41;

        List<string> animals = new List<string> { "닭", "달걀", "닭고기", "오리" };

        Dictionary<string, List<kvec>> pixelsAnimal = new Dictionary<string, List<kvec>>();
        Dictionary<string, List<Color>> colorsAnimal = new Dictionary<string, List<Color>>();
        Dictionary<string, int> bangsAnimal = new Dictionary<string, int>();
        Dictionary<string, int> countsAnimal = new Dictionary<string, int>();
        Dictionary<string, int> mulbunsAnimal = new Dictionary<string, int>();

        Dictionary<string, kvec> poss = new Dictionary<string, kvec>();
        Dictionary<string, krect> rects = new Dictionary<string, krect>();

        kvec scrollOffset = new kvec(0, 56 * 4);
        kvec vecMax = new kvec(200, 431) - new kvec(107, 330);

        Dictionary<string, string> oxpics = new Dictionary<string, string>();
        public PlayOxford(Bacro main) : base(main)
        {
            /*
            base(main)

            numPatt = new NumPatt();
            this.main = main;
            this.macro = main.macro;
            this.pics = main.pics;             
            */

            AddPics();
            fillPicsOxford();
        }
        public override void AddPics()
        {
            pics["적재화물 닭"] = "78 231 48 48::10 4,11 7,14 13::224 47 31,69 24 14,218 168 113";
            pics["적재화물 오리"] = "78 231 48 48::7 9,12 5,16 7::187 91 37,97 79 77,254 254 254";
            pics["적재화물 달걀"] = "78 231 48 48::5 17,16 12,23 20::224 212 207,240 227 220,207 163 138";
            pics["적재화물 닭고기"] = "78 231 48 48::6 20,14 19,13 25::143 130 117,198 176 167,167 160 155";
            pics["적재화물 5칸"] = "78 231 272 48::13 6,72 7,136 10::234 96 76,254 254 254,191 171 125";

            //pics["적재화물 5칸"] = "80 245 272 48::13 6,72 7,136 10::234 96 76,254 254 254,191 171 125";
            //적재 1번 /==/ 78 231 258 48 / 0 0,47 47 / 179 179 179,179 179 179

            foreach (var animal in animals)
            {
                var npcanimal = $"적재화물 {animal}";

                List<kvec> pixels = new List<kvec>();
                List<Color> colors = new List<Color>();
                krect rect__ = new krect();
                parsePicData(pics[npcanimal], ref rect__, ref pixels, ref colors);

                pixelsAnimal[animal] = pixels;
                colorsAnimal[animal] = colors;
                bangsAnimal[animal] = -1;
                mulbunsAnimal[animal] = -1;
                countsAnimal[animal] = 0;
            }
        }
        public void research(int sleep)
        {
            krect rect5bang = parsePicRect(pics["적재화물 5칸"]);

            Bitmap bmp5bang = macro.requestImage(rect5bang);

            Console.WriteLine($"rect5kan == {rect5bang.toString()}");

            main.munzza.PrintPic(rect5bang);

            int ocha = 30;

            //reset
            foreach (var animal in animals)
            {
                countsAnimal[animal] = 0;
                bangsAnimal[animal] = -1;
            }

            foreach (var animal in animals)
            {
                List<kvec> pixels = pixelsAnimal[animal];
                List<Color> colors = colorsAnimal[animal];

                for (int nbang = 0; nbang < 5; nbang++)
                {
                    List<kvec> pixelsOff = kvec.ListCopy(pixels);
                    kvec.ListOffset(pixelsOff, 56 * nbang, 0);

                    if (gx.ComparePixelsOfImage(bmp5bang, pixelsOff, colors, ocha, 0))
                    {
                        Console.WriteLine($"{animal} : {nbang}번 창고");
                        bangsAnimal[animal] = nbang;
                        break;
                    }
                }
            }

            var H48 = 48; // bmp5bang.Height
            krect rectBang0 = new krect(H48, H48);

            foreach (var animal in animals)
            {
                int nbang = bangsAnimal[animal];
                if (nbang < 0)
                    continue;

                krect rectBang = rectBang0.copy();
                rectBang.offset(56 * nbang, 0);

                string numscanx = scan_numtext(rectBang, bmp5bang, ymin, ymax);

                Console.WriteLine($"{animal} count == {numscanx}");

                int numscan;
                if (int.TryParse(numscanx, out numscan))
                    countsAnimal[animal] = numscan;
                else
                    Console.WriteLine($"parse error : numscanx == [{numscanx}]");
            }

            Thread.Sleep(sleep);
        }
        private void fillPicsOxford()
        {
            oxpics.Clear();

            var fileoxford = $@"{main.dirData}\oxford.txt";

            var lines = File.ReadAllLines(fileoxford, Encoding.UTF8);

            //적재칸 0 /==/ 80 245 48 48 / 0 0,0 47,47 47,47 0 / 179 179 179,179 179 179,179 179 179,179 179 179
            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length == 0)
                    continue;

                var ss = Regex.Split(line, " /==/ ");

                if (ss.Length == 2)
                {
                    var name = ss[0].Trim();
                    var value = ss[1].Replace(" / ", "::").Trim();

                    oxpics[ss[0]] = value;
                }
            }

            //foreach(var nam in oxpics.Keys)
            //{
            //    var val = oxpics[nam];

            //    Console.WriteLine($"{nam} == {val}");
            //}

        }
        public override void PlayMacro(string cate, string command)
        {
            EnergyFull = 290;
            FoodSlotNum = 4;
            EnergyOfOnePizza = 70;
            
            Console.WriteLine($"옥스포드 명령어 : {command}");

            switch (command)
            {
                case "연구":
                    studyApply();

                    break;
                case "닭부화":
                    productDaks(50);
                    break;
                case "달걀생산":
                    productEggs(50);
                    break;
                case "적재축소":
                    reduceGoods();

                    break;
                case "연습":
                    test();
                    break;
                case "대화":
                    manyCliks();
                    break;
                case "생산":
                    studyAndProduct(1);

                    break;
                case "생산반복":
                    studyAndProduct(10);

                    break;
            }

            Console.WriteLine($"쓰레드({command}) 종료");
            MacroRunning = false;

            //VoiceSpeak("매크로가 종료되었습니다. 매크로가 종료되었습니다.");
        }
        private void manyCliks()
        {
            var count = 0;

            while (++count <= 1000 && MacroRunning)
            {
                Thread.Sleep(300);
                macro.MouseClickX("left", "click");
            }

            Console.WriteLine("연타 끝!!");
        }
        private void manyCliks00()
        {
            var rectDial = parsePicRect(pics["대학 대화 선택"]);

            var posClick = rectDial.pos();
            posClick.offset(main.capDho.RectDho.pos());
            posClick.offset(0, 5);

            macro.MouseMove(posClick, kvec.Zero);

            Thread.Sleep(1000);
            var count = 0;

            while (++count <= 1000 && MacroRunning)
            {
                Thread.Sleep(300);
                macro.MouseClickX("left", "click");
            }

            Console.WriteLine("연타 끝!!");
        }
        private void Slot5Cliks()
        {
            var posClick = new kvec(400, 300);
            posClick.offset(main.capDho.RectDho.pos());
            macro.MouseMove(posClick, kvec.Zero);

            Thread.Sleep(1000);
            var count = 0;

            var sleep200 = 200;
            var MaxLoop = 5 * 60 * 60 * (1000 / sleep200);//5분

            while (++count <= MaxLoop && MacroRunning)
            {
                Thread.Sleep(sleep200);
                macro.KeysClick(null, "5", 0, 10);
            }

            Console.WriteLine("슬롯5번 연타 끝!!");
        }
        private void Slot5Cliks00()
        {
            var rectDial = parsePicRect(pics["퀵슬롯 5번"]);

            var posClick = rectDial.pos();
            posClick.offset(main.capDho.RectDho.pos());
            //posClick.offset(0, 5);

            macro.MouseMove(posClick, kvec.Zero);

            Thread.Sleep(1000);
            var count = 0;

            while (++count <= 1000 && MacroRunning)
            {
                Thread.Sleep(300);
                macro.MouseClickX("left", "click");
            }

            Console.WriteLine("슬롯5번 연타 끝!!");
        }
        void test()
        {
           
        }
        private void studyAndProduct(int LoopTarget)
        {
            var count = 0;

            while (++count <= LoopTarget && MacroRunning)
            {
                Console.WriteLine($"#{count}/{LoopTarget} 번째 생산");

                if (!studyApply())
                {
                    Console.WriteLine($"!! 논문 신청 실패.");
                    MacroRunning = false;
                    break;
                }

                Thread.Sleep(1000);

                Console.WriteLine($"==> 닭 부화");

                //productDaks(25);
                //Thread.Sleep(1000);
                productDaks(50);
                Thread.Sleep(1000);

                Console.WriteLine($"==> 달걀 생산");
                
                //productEggs(25);
                //Thread.Sleep(1000);
                productEggs(50);
                Thread.Sleep(1000);
            }
        }
        private void reduceGoods()
        {
           
        }
        private void productEggs(int MulCount)
        {
            if (!MacroRunning) return;
            if (!QuickSlots_Open(1000))
                return;

            ChargeEnergyBar2("행동력바", FoodSlotNum, 600, 1000);
            ChargeEnergyBar2("행동력바", FoodSlotNum, 500, 1000);

            QuickSlot_Heat(6, 2000);

            if (DetectTarget("버튼 확인 레시피", 200, 10, 20))
            {
                LeftClick("버튼 확인 레시피", null, 1000);
                MouseOffset(0, 40, 1000);

                if (DetectTarget("횟수 지정", 200, 10, 20))
                {
                    LeftClick("횟수 지정", null, 1000);
                    MouseOffset(0, 40, 1000);

                    //KeysClick("%50", 100, 500);
                    KeysClick($"%{MulCount}", 100, 500);
                    KeysClick("enter", 0, 1000);

                    if (DetectTarget("생산종료", 200, 10, 20))
                    {
                        LeftClick("생산종료", null, 1000);
                        MouseOffset(0, 40, 500);
                    }
                }
            }

            Console.WriteLine("x productEggs");
        }
        private void productDaks(int MulCount)
        {
            if (!MacroRunning) return;
            if (!QuickSlots_Open(1000))
                return;

            ChargeEnergyBar2("행동력바", FoodSlotNum, 600, 1000);
            ChargeEnergyBar2("행동력바", FoodSlotNum, 500, 1000);

            QuickSlot_Heat(5, 2000);

            if (DetectTarget("버튼 확인 레시피", 200, 10, 20))
            {
                KeysClick("down", 0, 500);
                KeysClick("down", 0, 500);

                LeftClick("버튼 확인 레시피", null, 1000);
                MouseOffset(0, 40, 1000);
                
                if (DetectTarget("횟수 지정", 200, 10, 20))
                {
                    LeftClick("횟수 지정", null, 1000);
                    MouseOffset(0, 40, 1000);

                    KeysClick($"%{MulCount}", 100, 500);
                    //KeysClick("%50", 100, 500);
                    KeysClick("enter", 0, 1000);

                    if (DetectTarget("생산종료", 200, 10, 20))
                    {
                        LeftClick("생산종료", null, 1000);
                        MouseOffset(0, 40, 500);
                    }
                }
            }
        }

        private bool studyApply()
        {
            if (!MacroRunning) 
                return false;

            if (!TabAndDetectFront("교수", 10, 10, 20, 1000))
                return false;

            var enableStudy = false;

            /* 여러번 클릭해야하는 경우가 있다 */
            var count= 0;
            while (DetectTarget("교수 연구", 200, 10, 20) && MacroRunning && ++count <= 5)
            {
                if (!enableStudy)
                    enableStudy = true;

                LeftClick("교수 연구", null, 500);//2초
                MouseOffset(0, 40, 500);
                
                Thread.Sleep(500);
            }
            if (!enableStudy)
                return false;
            if (!MacroRunning)
                return false;

            if (!DetectTarget("논문 테마 조타 a", 200, 10, 20))
                return false;

            if (!DetectTarget("버튼 다음", 200, 10, 20))
                return false;
            LeftClick("버튼 다음", null, 1000);
            MouseOffset(0, 40, 100);

            if (!DetectTarget("논문 테마 조타 b", 200, 10, 20))
                return false;

            var posNonmun = parsePicCen(pics["논문 테마 조타 b"]);
            /* 자주 수정을 해야한다, 위치가 달라지므로 */
            posNonmun.offset(56 * 2, 56 * 0);
            LeftClick(posNonmun, null, null, 1000);

            if (!DetectTarget("버튼 다음", 200, 10, 20))
                return false;
            LeftClick("버튼 다음", null, 1000);
            MouseOffset(0, 40, 100);

            if (!DetectTarget("버튼 확인", 200, 10, 20))
                return false;
            LeftClick("버튼 확인", null, 1000);
            MouseOffset(0, 40, 500);

            ClearNameBox(1000);

            return true;
        }
        private void InsikNums()
        {
            krect rect = new krect();
            List<kvec> pixels = new List<kvec>();
            List<Color> colors = new List<Color>();


            foreach (var nam in oxpics.Keys)
            {
                if (nam == "적재칸 3") continue;

                Thread.Sleep(2000);

                parsePicData(oxpics[nam], ref rect, ref pixels, ref colors);

                main.munzza.PrintPic(rect);

                Console.WriteLine($"{nam} <{rect.toString()}> ");
            }
        }
    }
}
