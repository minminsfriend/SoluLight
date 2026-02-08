using shine.libs.graphics;
using shine.libs.math;
using shine.libs.pad;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Bacro
{
    public partial class Sail
    {
        public bool ON_BATTLE = false;
        bool GANGZUC_STANDBY = false;
        //bool GANGZUC_ATTACK = false;

        kvec CoordBase = kvec.Parse2d("16340 2454");
        kvec CoordLondon = kvec.Parse2d("16323 2417");
        kvec CoordBarier = kvec.Parse2d("16350 2478");
        krect CityNameBox;
        public void ShipBattle(string title)
        {
            title = title.Replace(" ", "");

            switch(title)
            {
                case "출항":
                case "배틀":
                case "연습":

                    main.capDho.FocusWorkDho();
                    Thread.Sleep(200);

                    HeatBar(200);
                    break;
            }
        
            if (title == "연습")
            {
                //if (!ON_BATTLE) 
                //    ShotDhoByOneMinute();

                BackToLondon();

                //QuickSlots4Battle();
            }
            else if (title == "베이스로이동")
            {
                FirstToBaseZone();
            }
            else if (title == "출항")
            {
                OutOfPort4Battle();

                Thread.Sleep(500);
                BasicEyeView();
                
                Thread.Sleep(500);
                FirstToBaseZone();
            }
            else if (title == "배틀")
            {
                if (!ON_BATTLE)
                    SeaTohBul();
            }
            else if (title == "기본각도")
            {
                BasicEyeView();
            }
            else if (title == "데이타저장")
            {
                var filepics = $@"{main.dirData}\pics.txt";

                var text = "";

                int n = -1;

                foreach(var name in pics.Keys)
                {
                    var picx = pics[name].Replace("::", " / ");

                    text += $"{name} /**/ {picx}";

                    if (++n < pics.Keys.Count - 1)
                        text += "\n";
                }

                var data = Encoding.UTF8.GetBytes(text);

                var fw=new FileStream(filepics, FileMode.Create, FileAccess.Write);

                fw.Write(data,0,data.Length);

                fw.Close();
            }
            else if (title == "단어검색1")
            {
                List<string> words = new List<string> { "교전에","승리했"};

                foreach (var word in words)
                {
                    var onBattle = DetectWord5LinesFloatedX($"{word}", 20, 5, 20);

                    Console.WriteLine($"{word} 검색 : {onBattle}");
                }
            }
            else if (title == "단어검색")
            {
                List<string> words = new List<string> { "강적이 기습할 때를", "강한 적이 도전", "강적을 격퇴", "현재 격파수는" };

                foreach (var word in words)
                {
                    var nline = DetectWord5LinesFixedX($"{word}", 20, 5, 20);
                    if (nline > -1)
                        Console.WriteLine($"발견 : {word} : {nline} 줄");
                }
            }
        }
        void SeaTohBul()
        {
            ON_BATTLE = true;

            int countLoop = 0;
            int LoopTarget = 70;
            //readMacroData(fileinput, ref LoopTarget, ref EngeryFull, ref EnergyOfPizza);

            EnergyOfOnePizza = 70;
            EnergyFull = 195;
            //EngeryFull = 700;

            GANGZUC_STANDBY = false;

            //FirstToBaseZone();

            while (ON_BATTLE && ++countLoop <= LoopTarget)
            {
                Console.WriteLine($"## {countLoop} 번째 전투 ##");

                ClearNameBox(500);
                ClearNameBox(500);

                //if (GANGJEOK_ATTACK)
                //    BattleGanjeok();
                //else
                //{
          
                //}

                checkEnergyBar();

                CheckOutOfBaseZone();

                Thread.Sleep(1000);

                if (QuickSlots4Battle())
                    ScanAndBattle();
                else
                    ON_BATTLE = false;

                SleepLong_If_PokPoong();

                Thread.Sleep(1000);
            }

            BackToLondon();

            ON_BATTLE = false;
        }
        void SleepLong_If_PokPoong()
        {
            var nline = DetectWord5LinesFixedX("폭풍이", 20, 5, 20);
            if (nline > -1)
            {
                /* 30초 그냥 보낸다*/
                Thread.Sleep(30 * 1000);

                /* 3초마다 폭풍 지났나 확인 */
                var spentSecs = 0;
                int spentMax = 5 * 60; //(5분);
                var nlinex = -1;

                while (nlinex == -1 && spentSecs <= spentMax)
                {
                    nlinex = DetectWord5LinesFixedX("폭풍이 지나", 20, 5, 20);

                    spentSecs += 3;
                    Thread.Sleep(3 * 1000);
                }
            }
        }
        bool QuickSlots4Battle()
        {
            SailFold(1000);

            if (OpenQuickSlots(3))
            {
                MouseOffset(-200, 200, 100);

                QuickSlot_Heat(1, 1000);//감시
                QuickSlot_Heat(3, 1000);//쥐잡기
                QuickSlot_Heat(8, 1000);//측량
                if (!GANGZUC_STANDBY)
                    QuickSlot_Heat(2, 1000);//경계

                Thread.Sleep(1000);
                return true;
            }
            else
            {
                Console.WriteLine("스킬창이 열리지 않았다.");

                Thread.Sleep(1000);
                return false;
            }
        }
        bool OpenQuickSlots(int heatTimes)
        {
            //bool slotClosed = DetectTarget("퀵슬롯 우버튼", 50, 10, 30);

            Thread.Sleep(500);

            var count = 0;
            while (++count <= heatTimes)
            {
                Console.WriteLine("Click 퀵슬롯 우버튼");
                LeftClick("퀵슬롯 우버튼", "멍", 1000);
                //Thread.Sleep(1000);
                //slotClosed = DetectTarget("퀵슬롯 우버튼", 50, 10, 30);
            }

            return true;
        }
        bool OpenQuickSlotsx()
        {
            bool slotClosed = DetectTarget("퀵슬롯 우버튼", 50, 10, 30);

            Thread.Sleep(500);

            var count = 0;
            while (++count <= 10 && slotClosed)
            {
                LeftClick("퀵슬롯 우버튼", "멍", 1000);
                Thread.Sleep(1000);
                slotClosed = DetectTarget("퀵슬롯 우버튼", 50, 10, 30);
            }

            if (slotClosed)
                return false;
            else
                return true;
        }
        void checkEnergyBar()
        {
            int pinkWidth = 0;
            if (GetEnergyBar("행동력바", ref pinkWidth))
            {
                Console.WriteLine($">>> 행동력바 길이 : <{pinkWidth}>");

                if (pinkWidth < 50)
                    ChargeEnergyBar("행동력바", 4, 2000);
            }
        }
        void ScanAndBattle()
        {
            var EnemyFound = false;
            
            Dictionary<int, List<ShipNameRect>> shipsScanned = ScanShipsAllAngles(ref EnemyFound);
            if (EnemyFound)
            {
                BattleGangZuc();
                return;
            }

            if (shipsScanned.Count < 11)
                return;

            string nameShip = null;
            krect rectShip = null;
            var nrot = -1;

            getMaxYofShips(ref nrot, ref nameShip, ref rectShip, shipsScanned);

            if (!ON_BATTLE)
                return;

            if (nameShip != null)
            {
                //Console.WriteLine($"★★★ {nameShip} ★★★ [{nrot}] <{rectShip.toString()}>");

                Thread.Sleep(1000);
                string keyx = nrot <= 5 ? "a" : "d";
                var nRot = nrot <= 5 ? nrot : 10 - nrot;

                /*우회전만 정면을 맞춘다*/
                if (keyx == "a")
                    ShowFront(500);

                var A_D = keyx.ToUpper();
                Console.WriteLine($"배 회전 >> [{nrot}]  <CTRL + {A_D}>  {nRot}회");

                int countx = 0;
                while (++countx <= nRot)
                    EyeRotate(keyx, 50);

                if (!ON_BATTLE)
                    return;

                if (rectShip.y > 140)
                {
                    /* 정지상태, 그냥 쟁을 건다 */
                    BattleStart(nameShip, rectShip, 0);
                }
                else
                {
                    Bitmap bmpScan = macro.requestImage(RectScan);
                    List<kvec> pgreens = new List<kvec>();

                    Thread.Sleep(100);

                    var enemyFound = false;//형식적
                    List<ShipNameRect> ships = ScanningShipsOfFront(bmpScan, ref pgreens, ref enemyFound); ;
                    //printOnCapView(bmpScan, pgreens, ships, -1);

                    Thread.Sleep(100);

                    if (ships.Count == 0)
                    {
                        Console.WriteLine("★★★  (없어졌다, 꾀꼬리, 꾀꼬리,,,) ★★★");
                    }
                    else if (ships.Count > 0)
                    {
                        getMaxYofShip(ref nameShip, ref rectShip, ships);
                        Console.WriteLine($"★★★  {nameShip} ({rectShip.pos().toString2()}) ★★★");

                        DoubleClickTheShip(rectShip, 500);

                        /* 평속으로 */
                        Thread.Sleep(nRot * 1000);

                        /* 저속으로 */
                        SailFold(300);//작으면 안 접히는 듯하다
                        SailSpread(1, 100);

                        BattleStart(nameShip, rectShip, nRot);
                    }
                }
            }
        }
        void DoubleClickTheShip(krect rectShip, int sleepEnd)
        {
            var rect4Click = rectShip.Cen.copy();
            rect4Click.offset(0, 30);
            rect4Click.offset(RectScan.pos());
            LeftClick(rect4Click, null, "double", sleepEnd);
        }
        void BattleStart(string name, krect rect, int nRot)
        {
            var justBattle = false;
            var detectShip = false;

            /* 30초안에 접촉해야한다. */
            var timeStart = KSys.CurrentMillis();
            var overTime = false;
            var enemyAppear = false;

            while (ON_BATTLE && !detectShip)
            {
                if (GANGZUC_STANDBY)
                {
                    var nline = GangZucSearch("강한 적이 도전");
                    if (nline > -1)
                    {
                        enemyAppear = true;
                        break;
                    }
                }

                detectShip = TabAndDetectSeaFront(name, 20, 10, 50, 1000);
                if (detectShip)
                    break;

                var timeNow = KSys.CurrentMillis();
                if (timeNow - timeStart > 30 * 1000)
                {
                    overTime = true;
                    break;
                }

                Thread.Sleep(100);
            }

            /* 적 발견, 다음 스캔 루프로 넘김 */
            if (enemyAppear)
            {
                Console.WriteLine(">>>>> 강적이, 출현했습니다 !!");
                SailFold(1000);
                //ShowFront(1000);
                return;
            }

            /* 시간 지연으로 끝 */
            if (overTime)
            {
                Console.WriteLine(">>>>> 30초 초과 !!");
                SailFold(1000);
                ShowFront(1000);
                return;
            }

            if (detectShip)
            {
                /*일단 정지*/
                ShowFront(200);
                SailFold(100);
                KeysClick("space", 0, 500);

                if (DetectTarget("교전", 20, 5, 20))
                {
                    LeftClick("교전", null, 500);

                    justBattle = DetectWord5LinesFloatedX("교전에", 20, 5, 20);

                    if (!justBattle)
                    {
                        /*다시 전진*/
                        SailFold(100);
                        SailSpread(1, 100);

                        int countClick = 0;
                        while (ON_BATTLE && !justBattle && ++countClick <= 10)
                        {
                            Thread.Sleep(500);

                            LeftClick("교전", null, 500);
                            justBattle = DetectWord5LinesFloatedX("교전에", 20, 5, 20);
                        }
                    }
                }
            }

            if (justBattle)
            {
                Thread.Sleep(3000);

                QuickSlots_Open(1000);
                QuickSlot_Heat(5, 1000);
                QuickSlot_Heat(6, 1000);//회피

                int countCheck = 0;
                countCheck = 0;
                var nChuck = 0;

                while (++countCheck <= 5)
                {
                    Console.WriteLine("승리확인 중...");
                    
                    if (!GANGZUC_STANDBY)//덤으로 체크
                    {
                        var nline = GangZucSearch("강적이 기습할 때를");
                        GANGZUC_STANDBY = nline > -1;
                    }

                    nChuck = VictoCountInSik();
                    if (nChuck > 0)
                    {
                        var gangMsg = GANGZUC_STANDBY ? "준비하라." : "멀었다.";
                        Console.WriteLine("승리확인!");
                        Console.WriteLine($"현재 격파수 >>> {nChuck} 척,  강적 {gangMsg}");
                        Console.WriteLine("##############################################");
                        break;
                    }

                    Thread.Sleep(1000);
                }
            }
        }
        void BattleGangZuc()
        {
            /* 이미 접혀있다 */
            //SailFold(1000);

            OpenQuickSlots(3);
            QuickSlot_Heat(6, 500);//회피

            var count = 0;
            var victory = false;
            var targeted = false;

            while (++count <= 60 && ON_BATTLE)
            {
                QuickSlot_Heat(5, 500);
                QuickSlot_Heat(7, 200);//수리

                while (!targeted && ON_BATTLE)
                {
                    KeyClickLong("tab", 50, 100);
                    targeted = DetectTarget("해전상대타겟팅", 50, 5, 30);
                    if (targeted)
                        break;

                    EyeRotate("a", 100);
                }

                if (targeted)
                    ShowFront(100);

                if (GangZucSearch("강적을 격퇴") > -1)
                {
                    Console.WriteLine($"확인 : <강적을 격퇴>");
                    victory = true;
                    break;
                }

                Thread.Sleep(100);
            }

            if (victory)
            {
       
            }

            /* 다시 일반 전투 준비 */
            GANGZUC_STANDBY = false;
            /* 디폴트 */
            Thread.Sleep(3000);
        }
        int VictoCountInSik()
        {
            var nline = GangZucSearch($"현재 격파수는");

            if (nline > -1)
            { }//   Console.WriteLine($"발견 : {word} : {nline} 줄");
            else
                return -1;

            List<kvec> pixels = new List<kvec>();
            List<Color> colors = new List<Color>();
            krect rectPic = new krect();

            parsePicData(pics["격파슷자"], ref rectPic, ref pixels, ref colors);

            rectPic.y = 497 + 20 * nline;
            var color0 = Color.FromArgb(128, 170, 255);

            if (numbers == null)
                numbers = new Numbers(main.dirData);

            Bitmap bmp = macro.requestImage(rectPic);
            var numtext = numbers.NumsInSik(bmp, rectPic, color0);

            numtext = numtext.Trim().Replace(" ", "");

            int num;
            if (int.TryParse(numtext, out num))
                return num;
            else
                return -1;
        }
        protected override bool QuickSlots_Open(int sleepEnd)
        {
            /* 이미 열려있다 */
            //if (DetectTarget("퀵슬롯 좌버튼", 50, 10, 30))
            //{
            //    Thread.Sleep(sleepEnd);
            //    return true;
            //}

            if (DetectTarget("퀵슬롯 우버튼", 50, 10, 30))
            {
                LeftClick("퀵슬롯 우버튼", "멍", 1000);
                MouseOffset(-200, 200, sleepEnd);
                return true;
            }

            return false;
        }
        public override void ClearNameBox(int sleepEnd)
        {
            if (CityNameBox == null)
            {
                var picShipName = pics["도시이름테두리"];
                CityNameBox = parsePicRect(picShipName);
            }

            kvec posClick = CityNameBox.pos();
            posClick.offset(10, 10);

            RightClick(posClick, sleepEnd);
        }
        void HeatBar(int sleepEnd)
        {
            var posBar = CenFront.copy();
            posBar.y = 15;

            LeftClick(posBar, null, null, sleepEnd);
        }
    }
}