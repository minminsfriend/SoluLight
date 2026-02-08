
using shine.libs.capture;
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
    public partial class Player
    {
        public int EnergyFull = 445;
        public int EnergyOfOnePizza = 50;
        public int GotoBankTerm = 3;
        public int FoodSlotNum = 3;
        public int nFruitConsumed = 30;
        
        public bool MacroRunning = false;
        public bool TestRunning = false;
        public bool onSuspendingDelayClick = false;

        public NumPatt numPatt;
        public GreenArrow gArrow;

        protected Makro macro;
        protected Bacro main;
        public string fileinput = @"";

        public Dictionary<string, string> pics = new Dictionary<string, string>();
        public Dictionary<string, kvec> roadpoints = new Dictionary<string, kvec>();
        List<kvec> pixelsNameOutline = new List<kvec>();
        List<kvec> pixelsCityOutline = new List<kvec>();
        List<Color> colorsNameOutline = new List<Color>();
        List<Color> colorsCityOutline = new List<Color>();
        krect rectNameOutline = new krect();
        krect rectCityOutline = new krect();
        
        protected kvec PosBar = new kvec(200, 10);
        protected kvec PosCen = new kvec(400, 300);

        public Player(Bacro main)
        {
            numPatt = new NumPatt();
            this.main = main;
            this.macro = main.macro;
            this.pics = main.pics;
        }
        public virtual void AddPics()
        {
          
        }
        public virtual void PlayMacro(string cate, string command)
        {
            if (cate == "후후후")
            {
                switch(command)
                {
                    case "하하하":
                        break;
                }
            }
        }
        public void readMacroData(string fileinput, ref int loopTarget, ref int power, ref int pizza)
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
                        nn = Regex.Split(text, " ");
                        power = int.Parse(nn[0].Trim());
                        pizza = int.Parse(nn[1].Trim());
                    }
                }
            }
        }
        int searchPirodo()
        {
            //krect rectBox = krect.Parse("50 123 100 7");

            //Color colorRed = gx.ParseColor("255 128 0");
            //Color colorBack = gx.ParseColor("68 68 69");

            if (!DetectTarget("피로드", 200, 5, 20))
                return 0;

            krect rectPirodo = new krect();

            Bitmap bmpPirodo = macro.requestImage(rectPirodo);

            int redwidth = -1;
            int py = 5;

            for (int px = 0; px < bmpPirodo.Width; px++)
            {
                Color c = bmpPirodo.GetPixel(px, py);

                //분홍색에서 검정색으로 변하는 곳
                if (c.R < 200 && c.G < 100)// && c.B < 50)
                {
                    redwidth = px;
                    break;
                }
            }

            if (redwidth == -1)
                redwidth = 100;

            return redwidth;
        }
        public bool RightDrag(string posAx, string posBx, int sec, int sleepEnd)
        {
            kvec offset = main.capDho.RectDho.pos();

            krect rectA = parsePicRect(pics[posAx]);
            krect rectB = parsePicRect(pics[posBx]);

            kvec posA = rectA.Cen;
            kvec posB = rectB.Cen;

            macro.MouseMove(posA, offset);

            Thread.Sleep(100);
            /* 마우스 우 down */
            macro.MouseAct("right", "down");

            /* sleep 1 sec */
            Thread.Sleep(1000);

            macro.MouseMove(posB, offset);

            Thread.Sleep(100);
            /* 마우스 우 up */
            macro.MouseAct("right", "up");

            Thread.Sleep(sleepEnd);
            return true;
        }
        public void RightClick(string name, int sleepEnd)
        {
            krect rectNpc = parsePicRect(pics[name]);
            RightClick(rectNpc.pos(), sleepEnd);
        }
        public void RightClick(kvec pos, int sleepEnd)
        {
            kvec offset = main.capDho.RectDho.pos();
            macro.MouseMove(pos, offset);
            Thread.Sleep(100);

            macro.MouseClickX("right", "click");

            Thread.Sleep(sleepEnd);
        }
        public bool ScrollDownOff(kvec posA, kvec offset, int sleepPressed, int sleepEnd)
        {
            kvec offsetx = main.capDho.RectDho.pos();
            macro.MouseMove(posA, offsetx);
            Thread.Sleep(100);

            /* 마우스 down */
            macro.MouseAct("left", "down");

            /* sleep n sec */
            Thread.Sleep(sleepPressed);

            /* 마우스 move */
            macro.MouseMove(posA + offset, offsetx);
            Thread.Sleep(100);

            /* 마우스 우 up */
            macro.MouseAct("left", "up");

            Thread.Sleep(sleepEnd);
            return true;
        }
        public bool ScrollDown(string posAx, string posBx, int interval, int sleepEnd)
        {
            krect rectA = parsePicRect(pics[posAx]);
            krect rectB = parsePicRect(pics[posBx]);

            kvec posA = rectA.Cen;
            kvec posB = rectB.Cen;
            kvec offset = posB - posA;

            return ScrollDownOff(posA, offset, interval, sleepEnd);
        }
        public bool MouseOffset(int dx, int dy, int sleepEnd)
        {
            macro.MouseOffset(dx, dy);

            Thread.Sleep(sleepEnd);
            return true;
        }
        public bool LeftClick(string name, string option, int sleepEnd)
        {
            krect rectNpc = parsePicRect(pics[name]);
            return LeftClick(rectNpc.Cen, name, option, sleepEnd);
        }
        public bool LeftClickAndOff(string name, string option, kvec offx, int sleepEnd)
        {
            krect rectNpc = parsePicRect(pics[name]);
            LeftClick(rectNpc.Cen, name, option, sleepEnd / 2);
            MouseOffset(offx.X, offx.Y, sleepEnd / 2);

            return true;
        }
        public bool LeftClick(kvec pPic, string name, string option, int sleepEnd)
        {
            kvec offset = main.capDho.RectDho.pos();
            int ocha = 20;

            if (name == null)
                name = "___"; 
            if (option == null)
                option = "멍";

            switch (option)
            {
                case "인식":
                case "ctrl":
                    if (!DetectTarget(name, 500, 5, ocha))
                    {
                        Console.WriteLine($"{name} :: 감지 실패!");
                        return false;
                    }
                    break;
            }

            string action = null;

            switch (option)
            {
                case "double":
                    action = "double-click"; break;
                case "ctrl멍":
                case "ctrl":
                    action = "ctrl-click"; break;
                case "멍":
                case "인식":
                    action = "click"; break;
            }

            if (action != null)
            {
                macro.MouseMove(pPic, offset);
                Thread.Sleep(100);

                macro.MouseClickX("left", action);
            }

            Thread.Sleep(sleepEnd);
            return true;
        }
        public bool LeftClick(kvec pPic, int sleepEnd)
        {
            kvec offset = main.capDho.RectDho.pos();

            macro.MouseMove(pPic, offset);
            Thread.Sleep(100);

            macro.MouseClickX("left", "click");

            Thread.Sleep(sleepEnd);
            return true;
        }
        public void LeftClickTwice(kvec pos, int sleepEnd)
        {
            LeftClick(pos, null, null, sleepEnd);
            LeftClick(pos, null, null, sleepEnd);
        }
        public void LeftClickLong(kvec pos, int sleepPressed, int sleepEnd)
        {
            kvec offsetx = main.capDho.RectDho.pos();
            macro.MouseMove(pos, offsetx);

            Thread.Sleep(100);

            macro.MouseAct("left", "down");

            Thread.Sleep(sleepPressed);

            macro.MouseAct("left", "up");

            Thread.Sleep(sleepEnd);
        }
        public bool KeysClick(string keysx, int sleepIn, int sleepEnd)
        {
            var ss = Regex.Split(keysx, " ");

            if (ss.Length == 2)
            {
                var mkey = ss[0].Trim();
                var keys = ss[1].Trim();

                macro.KeysClick(mkey, keys, sleepIn, sleepEnd);
            }
            else
            {
                macro.KeysClick(null, keysx, sleepIn, sleepEnd);
            }

            return true;
        }
        public bool KeyClickLong(string keyx, int sleepPressed, int sleepEnd)
        {
            var ss = Regex.Split(keyx, " ");

            if (ss.Length == 1)
                macro.KeyLongClick(null, keyx, sleepPressed, sleepEnd);
            else if (ss.Length == 2)
                macro.KeyLongClick(ss[0], ss[1], sleepPressed, sleepEnd);

            return true;
        }
        public bool ChargeEnergyBar(string name, int slotNum, int sleepEnd)
        {
            var SensorPress = 500;
            return ChargeEnergyBar2(name, slotNum, SensorPress, sleepEnd);
        }
        public bool ChargeEnergyBar2(string name, int slotNum, int SensorPress, int sleepEnd)
        {
            if (!DetectTarget(name, 200, 5, 20))
                return false;

            krect rectPower = parsePicRect(pics[name]);
            Bitmap bmpPower = macro.requestImage(rectPower);

            int px = 1;
            int py = 2;

            for (int i = 1; i < bmpPower.Width; i++)
            {
                Color c = bmpPower.GetPixel(i, py);

                //분홍색에서 검정색으로 변하는 곳
                if (c.R < 150 && c.G < 150 && c.B < 150)
                {
                    px = i;
                    break;
                }
            }

            var pinkWidth = px - 1;
            float neededEnergy = EnergyFull * (100 - pinkWidth) / 100f;
            int NeededPizzas = (int)(neededEnergy / EnergyOfOnePizza);

            int sleepPressed = SensorPress * (NeededPizzas + 1);

            if (NeededPizzas == 0)
                return true;

            macro.KeyLongClick(null, $"{slotNum}", sleepPressed, 1000);

            Thread.Sleep(sleepEnd);
            return true;
        }
        public bool GetEnergyBar(string name, ref int pinkWidth)
        {
            if (!DetectTarget(name, 200, 5, 20))
                return false;

            //행동력핑크바 /==/ 49 141 102 6 / 1 2,11 2,28 2,42 2 / 247 124 247,247 124 247,247 124 247,247 124 247
            //행동력바     /==/ 49 141 102 6 / 0 0,0 5,101 0,101 5 / 0 13 76,0 13 76,0 13 76,17 28 81

            krect rectPower = parsePicRect(pics[name]);
            //Console.WriteLine($"{rectPower.toString()}");

            Bitmap bmpPower = macro.requestImage(rectPower);

            int px = 1;
            int py = 2;

            for (int x = 1; x < bmpPower.Width; x++)
            {
                Color c = bmpPower.GetPixel(x, py);

                //분홍색에서 검정색으로 변하는 곳
                if (c.R < 200 && c.G < 100 && c.B < 200)//247 124 247
                {
                    //Console.WriteLine($"pixel({x},{py}) color({c.R},{c.G},{c.B})");
                    px = x;
                    break;
                }
            }

            pinkWidth = px - 1;
            return true;
        }
        public bool DetectTarget(string name, int sleepInShots, int shotTries, int ocha, kvec offset)
        {
            if (!pics.Keys.Contains(name))
            {
                Console.WriteLine($" ??? [{name}] <-- 존재하지 않는 pic");
                return false;
            }

            List<kvec> pixels = new List<kvec>();
            List<Color> colors = new List<Color>();
            krect rectPic = new krect();

            parsePicData(pics[name], ref rectPic, ref pixels, ref colors);
            rectPic.offset(offset);

            bool detectTarget = false;

            int count = 0;
            while (++count <= shotTries)
            {
                Bitmap bmp = macro.requestImage(rectPic);

                if (gx.ComparePixelsOfImage(bmp, pixels, colors, ocha, 0))
                {
                    detectTarget = true;
                    break;
                }

                Thread.Sleep(sleepInShots);
            }

            return detectTarget;
        }
        public bool DetectTarget(string name, int sleepInShots, int shotTries, int ocha)
        {
            kvec shift = kvec.Zero;
            return DetectTarget(name, sleepInShots, shotTries, ocha, shift);
        }
        public bool DetectTarget2(krect rectPic, string namePic, int sleepInShots, int shotTries, int ocha)
        {
            return detectTarget2(rectPic, namePic, sleepInShots, shotTries, ocha);
        }
        private bool detectTarget2(krect rectPic, string namePic, int sleepInShots, int shotTries, int ocha)
        {
            if (!pics.Keys.Contains(namePic))
            {
                Console.WriteLine($" ??? [{namePic}] <-- 존재하지 않는 pic");
                return false;
            }

            List<kvec> pixels = new List<kvec>();
            List<Color> colors = new List<Color>();
            krect rectTemp = new krect(); //형식적인 변수

            parsePicData(pics[namePic], ref rectTemp, ref pixels, ref colors);

            bool detectTarget = false;

            int count = 0;
            while (++count <= shotTries)
            {
                Bitmap bmp = macro.requestImage(rectPic);

                if (gx.ComparePixelsOfImage(bmp, pixels, colors, ocha, 0))
                {
                    detectTarget = true;
                    break;
                }

                Thread.Sleep(sleepInShots);
            }

            return detectTarget;
        }
        public int DetectWord5LinesFixedX(string name, int sleepInShot, int shotTries, int ocha)
        {
            if (!pics.Keys.Contains(name))
            {
                Console.WriteLine($" ??? [{name}] <-- 존재하지 않는 pic");
                return -1;
            }

            List<kvec> pixels = new List<kvec>();
            List<Color> colors = new List<Color>();
            krect rectPic = new krect();

            parsePicData(pics[name], ref rectPic, ref pixels, ref colors);
            var rectCap = rectPic.copy();
            //var rectCap = new krect(18, 497, 230, 13);
            rectCap.y = 497;
            rectCap.h = 93;

            int count = 0;
            while (++count <= shotTries)
            {
                Bitmap bmp = macro.requestImage(rectCap);

                /* 다섯 줄 검색해야한다 497 517 537 557 577 */
                for (int h = 4; h >= 0; h--)
                {
                    if (gx.ComparePixelsOfImage(bmp, pixels, colors, ocha, 20 * h))
                        return h;
                }

                Thread.Sleep(sleepInShot);

                if (!MacroRunning)
                    return -1;
            }
        
            return -1;
        }
        public bool DetectWord5LinesFloatedX(string name, int sleepInShot, int shotTries, int ocha)
        {
            if (!pics.Keys.Contains(name))
            {
                Console.WriteLine($" ??? [{name}] <-- 존재하지 않는 pic");
                return false;
            }

            List<kvec> pixels = new List<kvec>();
            List<Color> colors = new List<Color>();
            krect rectPic = new krect();//사용하지 않는다

            parsePicData(pics[name], ref rectPic, ref pixels, ref colors);
            /* pics["승리"] = rectPic: 18 497 230 13  */
            var rectCap = new krect(18, 497, 230, 13);
            /* 다섯 줄 검색해야한다 497 517 537 557 577 */
            rectCap.h = 577 - 497 + 13;

            bool detectTarget = false;

            int count = 0;
            while (++count <= shotTries)
            {
                Bitmap bmp = macro.requestImage(rectCap);

                for (int h = 4; h >= 0; h--)
                {
                    if (gx.ComparePixelsOfWordFloatedX(bmp, pixels, colors, 20 * h))
                    {
                        detectTarget = true;
                        break;
                    }
                }

                Thread.Sleep(sleepInShot);

                if (!MacroRunning)
                    return false;

                if (detectTarget)
                    break;//while

            }

            return detectTarget;
        }
        public bool TabAndDetectFront(string name, int tabTries, int shotTries, int ocha, int sleepEnd)
        {
            if (!checkPicName(name)) return false;
            var homekeyClick = true;

            if (pixelsNameOutline.Count == 0)
                parsePicData(pics["인물이름테두리"], ref rectNameOutline, ref pixelsNameOutline, ref colorsNameOutline);

            return tabAndDectectWrap(name, tabTries, shotTries, ocha, sleepEnd, homekeyClick, pixelsNameOutline, colorsNameOutline);
        }
        public bool TabAndDetectNotFront(string name, int tabTries, int shotTries, int ocha, int sleepEnd)
        {
            if (!checkPicName(name)) return false;
            var homekeyClick = false;

            if (pixelsNameOutline.Count == 0)
                parsePicData(pics["인물이름테두리"], ref rectNameOutline, ref pixelsNameOutline, ref colorsNameOutline);
            
            return tabAndDectectWrap(name, tabTries, shotTries, ocha, sleepEnd, homekeyClick, pixelsNameOutline, colorsNameOutline);
        }
        public bool TabAndDetectSeaFront(string name, int tabTries, int shotTries, int ocha, int sleepEnd)
        {
            if (!checkPicName(name)) return false;
            var homekeyClick = true;

            if (pixelsCityOutline.Count == 0)
                parsePicData(pics["도시이름테두리"], ref rectCityOutline, ref pixelsCityOutline, ref colorsCityOutline);

            return tabAndDectectWrap(name, tabTries, shotTries, ocha, sleepEnd, homekeyClick, pixelsCityOutline, colorsCityOutline);
        }
        public bool TabAndDetectSeaNotFront(string name, int tabTries, int shotTries, int ocha, int sleepEnd)
        {
            if (!checkPicName(name)) return false;
            var homekeyClick = false;

            if (pixelsCityOutline.Count == 0)
                parsePicData(pics["도시이름테두리"], ref rectCityOutline, ref pixelsCityOutline, ref colorsCityOutline);

            return tabAndDectectWrap(name, tabTries, shotTries, ocha, sleepEnd, homekeyClick, pixelsCityOutline, colorsCityOutline);
        }
        bool tabAndDectectWrap(string name, int tabTries, int shotTries, int ocha, int sleepEnd, bool homekeyclick, List<kvec> pixelsOutline, List<Color> colorsOutline)
        {
            if (tabAndDetect(name, tabTries, shotTries, ocha, pixelsOutline, colorsOutline))
            {
                Thread.Sleep(100);
                macro.KeyClickX5("space", 100, 5);

                if (homekeyclick)
                    macro.KeyClick(null, "home", 100); // 정면보기

                Thread.Sleep(sleepEnd);
                return true;
            }

            return false;
        }
        bool tabAndDetect(string name, int tabTries, int shotTries, int ocha, List<kvec> pixelsOutline, List<Color> colorsOutline)
        {
            List<kvec> pixels = new List<kvec>();
            List<Color> colors = new List<Color>();
            krect rectPic = new krect();

            parsePicData(pics[name], ref rectPic, ref pixels, ref colors);

            /*여러번 탭 탭 */
            var countTab = 0;
            while (++countTab <= tabTries && MacroRunning)
            {
                /* tab 을 누른다. */
                macro.KeyClick(null, "tab", 500);

                /* rectPic 여러번 캡춰 */
                int countShot = 0;
                while (++countShot <= shotTries)
                {
                    /* rectPic 캡춰, 뭐가 찍혔을지 아직 모른다 */
                    Bitmap bmp = macro.requestImage(rectPic);

                    /* 테두리 네임박스 인식*/
                    if (gx.ComparePixelsOfImage(bmp, pixelsOutline, colorsOutline, 100, 0))
                    {
                        /* 테두리 안 문자 인식 */
                        if (gx.ComparePixelsOfImage(bmp, pixels, colors, ocha, 0))
                            /* 타겟 이름 확인, 함수 종료 */
                            return true;

                        /* 네임박스 인식, 그러나 다른 이름 */
                        break;
                    }

                    /* sleepInShots */
                    Thread.Sleep(100);
                }
            }

            return false;
        }
        public void LeftClickDelayed(string slot, string option, int delay, int interval, int repeat)
        {
            Thread.Sleep(delay);

            for (int k = 0; k < repeat; k++)
            {
                LeftClick(slot, option, 1000);

                if (k < repeat - 1)
                    Thread.Sleep(interval);
            }

            //한번 실행하면 5분안에 다시 실행안되게 한다.
            Thread.Sleep(5 * 60 * 1000);

            onSuspendingDelayClick = false;
        }
        public krect parsePicRect(string picdata)
        {
            //Console.WriteLine(picdata);

            var tt = Regex.Split(picdata, "::");
            return krect.Parse(tt[0]);
        }
        public kvec parsePicCen(string picdata)
        {
            var rect = parsePicRect(picdata);
            return rect.Cen;
        }
        public void parsePicData(string picdata, ref krect rectDhoCut, ref List<kvec> pixels, ref List<Color> colors)
        {
            string[] ss, nn, tt;
            tt = Regex.Split(picdata, "::");

            if (tt.Length != 3)
                return;

            rectDhoCut = krect.Parse(tt[0]);

            pixels.Clear();
            colors.Clear();

            // fill pixels
            ss = Regex.Split(tt[1], ",");

            foreach (var s in ss)
            {
                nn = Regex.Split(s, " ");

                int x = int.Parse(nn[0]);
                int y = int.Parse(nn[1]);

                pixels.Add(new kvec(x, y));
            }

            // fill colors
            ss = Regex.Split(tt[2], ",");

            foreach (var s in ss)
            {
                nn = Regex.Split(s, " ");

                int r = int.Parse(nn[0]);
                int g = int.Parse(nn[1]);
                int b = int.Parse(nn[2]);

                colors.Add(Color.FromArgb(r, g, b));
            }
        }
        bool checkPicName(string name)
        {
            if (!pics.Keys.Contains(name))
            {
                Console.WriteLine($" ??? [{name}] <-- 존재하지 않는 이름");
                return false;
            }

            return true;
        }
        public virtual void ClearNameBox(int sleepEnd)
        {
            var picShipName = pics["인물이름테두리"];
            var rectBox = parsePicRect(picShipName);

            kvec posClick = rectBox.pos();
            posClick.offset(10, 10);

            RightClick(posClick, sleepEnd);
        }
        protected virtual bool QuickSlots_Open(int sleepEnd)
        {
            if (DetectTarget("퀵슬롯 우버튼", 10, 10, 20))
            {
                LeftClick("퀵슬롯 우버튼", null, 1000);
                MouseOffset(-200, 200, 500);

                if (DetectTarget("퀵슬롯 좌버튼", 10, 10, 20))
                {
                    Console.WriteLine($"퀵슬롯 열었다");
                    Thread.Sleep(sleepEnd);
                    return true;
                }
            }
            else if (DetectTarget("퀵슬롯 좌버튼", 10, 10, 20))
            {
                Console.WriteLine($"원래 열려있었다");
                Thread.Sleep(sleepEnd);
                return true;
            }

            return false;
        }
        protected void QuickSlot_Heat(int nSlot, int sleepEnd)
        {
            KeysClick($"{nSlot}", 0, sleepEnd);
        }
        public void VoiceSpeak(string msg)
        {

        }
        protected string scan_numtext(krect rectMul, Bitmap bmp5bang, int ymin, int ymax)
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
                    Color color = bmp5bang.GetPixel(xScan, y);

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
                    int num = getNumByPatt(pWhite, ref texlen, bmp5bang, ymin);

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
        protected int getNumByPatt(kvec pWhite, ref int texlen, Bitmap bmp5bang, int ymin)
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

                List<Color> colorsScan = scanColors(pWhite, pixelsPatt, bmp5bang, ymin);

                if (compare_colors(colorsScan, colorsPatt))
                {
                    texlen = numPatt.rects[n].W + 1;
                    return n;
                }
            }

            return -1;
        }
        List<Color> scanColors(kvec pWhite, List<kvec> pixelsPatt, Bitmap bmp5bang, int ymin)
        {
            kvec pivot = pWhite.copy();
            pivot.y = ymin;

            List<Color> colors = new List<Color>();

            foreach (var px in pixelsPatt)
            {
                kvec p = px.copy();
                p.offset(pivot);

                Color color = bmp5bang.GetPixel(p.X, p.Y);

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
