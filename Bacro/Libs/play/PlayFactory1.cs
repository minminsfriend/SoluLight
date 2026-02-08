
using shine.libs.graphics;
using shine.libs.math;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Speech.Synthesis.TtsEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
namespace Bacro
{
    public partial class PlayFactory : Player
    {
        void playmacro_hakham(int loopTarget)
        {
            MacroRunning = true;

            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 학술햄생산☎  # {loopcount}/{loopTarget}");

                /* 논문 신청 */
                studyApplyHakSul();

                /* 생산 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
                product("돼지고기", 0, 0, 1);
                product("햄", 0, 1, 1);
                product("햄", 0, 1, 1);

                sellHakSulHam();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            MacroRunning = false;
        }
        void playmacro_sausage_test()
        {
            MacroRunning = true;
            buySheeps(1);
            MacroRunning = false;
        }
        void playmacro_sausage(int loopTarget)
        {
            MacroRunning = true;

            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 소세지생산☎  # {loopcount}/{loopTarget}");

                /* 돼지 사기, 3번 시도 */
                var count = 0;
                while (++count <= 3 && !buySheeps(1))
                    Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 양 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                /* 생산 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
                product("양고기", 5, 2, 1);
                product("양고기", 5, 2, 1);

                product("소세지", 5, -1, 1);
                product("소세지", 5, -1, 1);
                product("소세지", 5, -1, 1);
                product("소세지", 5, -1, 1);

                sell();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            if (MacroRunning)
                main.VoiceSpeak("소세지 생산이 모두 끝났습니다. 매크로를 종료합니다.");
            else
                main.VoiceSpeak("매크로가 도중에 중단되었섭니다. 매크로가 중단되었습니다.");

            MacroRunning = false;
        }
        void playmacro_Pernam(int loopTarget)
        {
            MacroRunning = true;

            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            int loopcount = 0;
            var isStudyTime = true;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 햄생산☎  # {loopcount}/{loopTarget}");
                
                /* 돼지 사기, 3번 시도 */
                var count = 0;
                while (++count<=3 && !buyPigs(3, 200))//230
                    Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 돼지 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                /* 옥수수칩 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
                product("옥수수칩", 0, 2, 1);
                product("돼지 고기", 0, 1, 5);
                product("돼지 고기", 0, 1, 2);

                //product("옥수수칩", 0, 2, 1);
                product("햄", 0, 0, 5);
                product("햄", 0, 0, 5);
                product("햄", 0, 0, 2);

                sell();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");

                isStudyTime = !isStudyTime;//최초값 true, 첫생산 false 로 맞춤
                if (isStudyTime)
                {
                    StudyApply();

                    timeSpent = KSys.ConsumedTime(time0);
                    Console.WriteLine($"  ▶▶ {timeSpent} (연구포함)");
                }
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            if (MacroRunning)
                main.VoiceSpeak("햄 생산이 모두 끝났습니다. 매크로를 종료합니다.");
            else
                main.VoiceSpeak("매크로가 도중에 중단되었섭니다. 매크로가 중단되었습니다.");

            MacroRunning = false;
        }
        void playmacro_foodCan(int loopTarget)
        {
            MacroRunning = true;
            
            var count = 0;
            while(++count<=loopTarget && MacroRunning)
            {
                if (!productFood())
                    break;

                //Thread.Sleep(1000);
                
                if (!productCan())
                    break; 
           
                //Thread.Sleep(1000);

                Console.WriteLine($"☎ 통조림 생산☎  # {count}/{loopTarget}");
            }

            MacroRunning = false;
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");
        }
        bool productFood()
        {
            if (!MacroRunning)  ;

            if (DetectTarget("퀵슬롯 우버튼", 200, 10, 20))
            {
                LeftClick("퀵슬롯 우버튼", null, 1000);
                MouseOffset(-300, 0, 1000);
            }

            ChargeEnergyBar("행동력바", FoodSlotNum, 1000);
            ChargeEnergyBar("행동력바", FoodSlotNum, 1000);

            //마이 레시피
            KeysClick("control b", 10, 1000);

            if (!DetectTarget("버튼 확인 레시피", 200, 10, 20))
                return false;
            KeysClick($"%▲▲", 500, 1000);

            if (!DetectTarget("버튼 확인 레시피", 200, 10, 20))
                return false;
            LeftClick("버튼 확인 레시피", null, 1000);

            if (!DetectTarget("횟수 지정", 200, 10, 20))
                return false;
            LeftClick("횟수 지정", null, 1000);

            if (!DetectTarget("횟수 Max", 200, 10, 20))
                return false;
            LeftClick("횟수 Max", null, 1000);

            if (!DetectTarget("횟수 OK", 200, 10, 20))
                return false;
            LeftClick("횟수 OK", null, 1000);

            if (!DetectTarget("생산종료", 200, 10, 20))
                return false;
            LeftClick("생산종료", null, 1000);

            return true;
        }
        bool productCan()
        {
            if (!MacroRunning) return false;

            if (!TabAndDetectFront("통조림 기계", 30, 10, 20, 500))
                return false;

            var count = 0;
            while (++count <= 5 && !DetectTarget("버튼 생산 통조림", 200, 10, 20))
                KeysClick("space", 0, 500);
            if (count > 5)
                return false;

            LeftClick("버튼 생산 통조림", null, 1000);

            if (!DetectTarget("버튼 확인 레시피", 200, 10, 20))
                return false;
            KeysClick($"%▼▼", 500, 1000);

            if (!DetectTarget("버튼 확인 레시피", 200, 10, 20))
                return false;
            LeftClick("버튼 확인 레시피", null, 1000);

            if (!DetectTarget("횟수 지정", 200, 10, 20))
                return false;
            LeftClick("횟수 지정", null, 1000);

            if (!DetectTarget("횟수 Max", 200, 10, 20))
                return false;
            LeftClick("횟수 Max", null, 1000);

            if (!DetectTarget("횟수 OK", 200, 10, 20))
                return false;
            LeftClick("횟수 OK", null, 1000);

            if (!DetectTarget("생산종료", 200, 10, 20))
                return false;
            LeftClick("생산종료", null, 1000); 
            
            return true;
        }
        void playmacro_foodCan00()
        {
            MacroRunning = true;

            kvec posMachine = new kvec(391,250);
            kvec vec = new kvec(38, 48); 
            
            kvec posSwitch = posMachine + vec;

            kvec posReceipi = new kvec(159, 271);
            kvec posOneZori = new kvec(457, 333);

            EnergyFull = 665;
            EnergyOfPizza = 70;
            FoodSlotNum = 4;

            var loopTarget = 6;
            var loopcount = 0;

            LeftClick("상단바", null, 1000);

            while (++loopcount <= loopTarget && MacroRunning)
            {
                if (loopcount % 3 == 1)
                {
                    if (DetectTarget("퀵슬롯 우버튼", 200, 10, 20))
                    {
                        LeftClick("퀵슬롯 우버튼", null, 1000);
                        MouseOffset(-300, 0, 1000);
                    }

                    ChargeEnergyBar("행동력바", FoodSlotNum, 1000);

                }
                
                KeysClick("control b", 100, 1000);
                KeysClick("enter", 100, 1000);

                LeftClick("횟수 지정", null, 1000);
                LeftClick("횟수 Max", null, 1000);
                LeftClick("횟수 OK", null, 1000);

                LeftClick("생산종료", null, 1000);

                LeftClickTwice(posMachine, 500);
                Thread.Sleep(1000);
                LeftClick(posSwitch, null, null, 1000);
                LeftClick(posReceipi, null, "double", 1000);

                LeftClick(posOneZori, null, null, 1000);
                LeftClick(posOneZori, null, null, 1000);
                LeftClick("생산종료", null, 1000);
                
                Thread.Sleep(1000);
                Console.WriteLine($"통조림 # {loopcount}/{loopTarget}");
            }

            MacroRunning = false;
        }
        void playmacro_twitter()
        {
            MacroRunning = true;

            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            EnergyFull = 640;
            FoodSlotNum = 4;

            EnergyOfPizza = main.gInput.getPizzaPower();
            int loopTarget = main.gInput.getLoopTarget();
            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎양모☎ # {loopcount}/{loopTarget}");

                buySheeps(20);
                //continue;

                product("양모", 0, 0, 2);
                product("양모", 0, 0, 2);
                product("양모", 0, 0, 2);
                product("양모", 0, 0, 2);
                product("양모", 0, 0, 2);
                product("양모", 0, 0, 2);
                //continue;

                product("트위터", 0, 1, 2);
                product("트위터", 0, 1, 2);
                product("트위터", 0, 1, 2);
                product("트위터", 0, 1, 2);

                sell();

                var ctimex = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {ctimex}");
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            if (MacroRunning)
                main.VoiceSpeak("트위터 생산이 모두 끝났습니다. 매크로를 종료합니다.");
            else
                main.VoiceSpeak("매크로가 도중에 중단되었섭니다. 매크로가 중단되었습니다.");

            MacroRunning = false;

        }
        void playmacro_feather()
        {
            MacroRunning = true;

            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            FoodSlotNum = 4;

            EnergyFull = main.gInput.getFullPower();
            EnergyOfPizza = main.gInput.getPizzaPower();

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            int loopTarget = main.gInput.getLoopTarget();
            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎깃털☎ # {loopcount}/{loopTarget}");

                buyChicken(13, 170);

                product("옥수수콘", 0, 1, 1);

                product("깃털", 0, 0, 2);
                product("깃털", 0, 0, 2);
                product("깃털", 0, 0, 2);
                product("깃털", 0, 0, 2);
                //product("깃털", 0, 0, 1);

                sell();

                var ctimex = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {ctimex}");
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            if (MacroRunning)
                main.VoiceSpeak("깃털 생산이 모두 끝났습니다. 매크로를 종료합니다.");
            else
                main.VoiceSpeak("매크로가 도중에 중단되었섭니다. 매크로가 중단되었습니다.");

            MacroRunning = false;

        }
        void playmacro_onship()
        {
            MacroRunning = true;

            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            EnergyFull = 665;
            //PowerFull = 635;
            EnergyOfPizza = 70;
            FoodSlotNum = 4;

            int loopTarget = 4;
            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎와인비니거☎ # {loopcount}/{loopTarget}");

                buyWines(10);

                OutOfCity("비아나두");//

                product_onship();

                intoCity("비아나두");//

                sell();

                var ctimex = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {ctimex}");
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            //if (MacroRunning)
            //    main.VoiceSpeak("트위터 생산이 모두 끝났습니다. 매크로를 종료합니다.");
            //else
            //    main.VoiceSpeak("매크로가 도중에 중단되었섭니다. 매크로가 중단되었습니다.");

            MacroRunning = false;

        }
        void product_onship()
        {
            var posBack= new kvec(406, 458);

            product("와인비니거", 0, 0, 2);
            product("와인비니거", 0, 0, 2);
            product("와인비니거", 0, 0, 2);
            product("와인비니거", 0, 0, 2);

            macro.KeyLongClick("control", "s", 4000, 100);
            LeftClick(posBack, null, "double", 1000);

            Thread.Sleep(7000);
            RightClick("맵 센터", 1000);

            macro.KeyLongClick("control", "w", 1000, 100);

            product("와인비니거", 0, 0, 2);
            product("와인비니거", 0, 0, 2);
            product("와인비니거", 0, 0, 2);
            product("와인비니거", 0, 0, 1);
        }
        void intoCity(string cityname)
        {
            TabAndDetectNotFront("비아나두", 20, 10, 40, 1000);
            while (!DetectTarget("항구 앞", 100, 10, 30))
                Thread.Sleep(1000);

            LeftClick("항구 앞", null, 1000);
            Thread.Sleep(3000);

            while (!DetectTarget("도시 인식", 100, 10, 30))
                Thread.Sleep(1000);

            LeftClick("도시맵", null, 1000);
            LeftClick($"맵 교역소 {cityname}", null, 3 * 1000);

            RightClick("맵 센터", 1000);
        }
        void OutOfCity(string cityname)
        {
            RightClick("맵 센터", 1000);
            LeftClick("도시맵", null, 1000);
            LeftClick($"맵 항구 {cityname}", null, 3 * 1000);

            if (!TabAndDetectNotFront("항구관리", 50, 10, 30, 1000))
                Thread.Sleep(1000);

            while (!DetectTarget("항구로 이동", 100, 10, 30))
                KeysClick("space", 100, 1000);

            LeftClick("항구로 이동", null, 4000);

            while (!DetectTarget("보급", 100, 10, 30))
                Thread.Sleep(1000);//
            LeftClick("보급", null, 2000);

            LeftClick("기억 보급", null, 1000);
            LeftClick("보급 확인", null, 1000);

            while (!DetectTarget("출항", 200, 10, 30))
                RightClick("맵 센터", 1000);

            LeftClick("출항", null, 1000);
            LeftClick("출항 2", null, 4000);

            while (!DetectTarget("퀵슬롯 우버튼", 200, 10, 30))
                Thread.Sleep(1000);

            KeysClick("w", 100, 1000);//한칸 항해
        }
        void sell()
        {
            if (!MacroRunning) return;

            while (!TabAndDetectNotFront("교역소 주인", 20, 10, 30, 500))
                Thread.Sleep(1000);
            while (!DetectTarget("교역소 매각", 200, 10, 20))
                KeysClick("space", 100, 500);

            LeftClick("교역소 매각", null, 1000);
            LeftClick("전부매각", null, 1000);

            //최종 매각
            LeftClick("교역소 매각 확인", null, 1000);
        }
        void sellHakSulHam()
        {
            if (!MacroRunning) return;

            while (!TabAndDetectNotFront("교역소 도제", 20, 10, 30, 500))
                Thread.Sleep(1000);
            while (!DetectTarget("교역소 매각", 200, 10, 20))
                KeysClick("space", 100, 500);

            LeftClick("교역소 매각", null, 1000);
        
            if (!DetectTarget("교역물 매각 햄", 200, 10, 20))
                return;
            LeftClick("교역물 매각 햄", null, 1000);

            //LeftClick2(MUL_2, 500);
            LeftClick(pMulGun2 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("교역소 매각 확인", null, 1000);
        }
        bool buyChicken(int nBalju, int CORNS)
        {

            return true;
        }
        bool buyWines(int nBalju)
        {

            return true;
        }
        bool buySheeps(int nBalju)
        {
            if (!TabAndDetectFront("교역소 주인", 30, 10, 20, 500))
                return false;

            var count = 0;
            while (++count <= 5 && !DetectTarget("교역소 구입", 200, 10, 20))
                KeysClick("space", 0, 500);
            if (count > 5)
                return false;

            LeftClick("교역소 구입", null, 1000);

            /* 아이템 == 발주서 */
            if (!UseBalju(nBalju))
                return false;

            /* 교역품 구입 */

            var scrollA = new kvec(pScrollUp.x, pMulGun1.y);

            // 스크롤
            var offset = new kvec(0, 56 * 2);
            var scrollB = scrollA.copy();
            scrollB.offset(0, 56 * 2);

            ScrollDownOff(scrollA, offset, 1000, 1000);

            count = 0;
            while (++count <= 3 && !DetectTarget("진열품 양 헤르데르", 100, 5, 20))
                ScrollDownOff(scrollB, offset, 1000, 500);
            if (count > 3)
                return false;

            LeftClickTwice(pMulGun3, 500);
            LeftClick(pMulGun3 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool buyPigs(int nBalju, int COUNT_CORNS)
        {
            if (!TabAndDetectFront("교역소 주인", 30, 10, 20, 500))
                return false;

            var count = 0;
            while (++count <= 5 && !DetectTarget("교역소 구입", 200, 10, 20))
                KeysClick("space", 0, 500);
            if (count > 5)
                return false;

            LeftClick("교역소 구입", null, 1000);

            /* 아이템 == 발주서 */
            if (!UseBalju(nBalju))
                return false;

            /* 교역품 구입 */

            kvec scrollA = parsePicCen(pics["스크롤바 상단"]);
            scrollA.offset(0, 56 * 1);

            LeftClickTwice(pMulGun2, 500);
            KeysClick($"%{COUNT_CORNS}", 100, 500);
            KeysClick("enter", 100, 1000);

            // 스크롤
            var offset = new kvec(0, 56 * 2);
            var scrollB = scrollA.copy();
            scrollB.offset(0, 56 * 2);

            ScrollDownOff(scrollA, offset, 1000, 1000);

            count = 0;
            while(++count<=3 && !DetectTarget("진열품 돼지", 100, 5, 20))
                ScrollDownOff(scrollB, offset, 1000, 500);
            if (count > 3)
                return false;

            LeftClickTwice(pMulGun4, 500);
            LeftClick(pMulGun4 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
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
        void StudyApply()
        {
            Thread.Sleep(1000);
            Thread.Sleep(1000);
            RunOnMap("은행", 4);

            studyApply();

            Thread.Sleep(1000);
            RunOnMap("교역소", 4);
        }
        bool studyApply()
        {
            if (!MacroRunning) return false;

            if (!TabAndDetectNotFront("학자", 50, 10, 30, 1000))
                return false;

            if (DetectTarget("학자 연구", 200, 10, 20))
                LeftClick("학자 연구", null, 1000);
            else if (DetectTarget("학자x연구", 200, 10, 20))
                LeftClick("학자x연구", null, 1000);
            else
                return false;

            if (!DetectTarget("특별 연구동", 200, 10, 20))
                return false;
            LeftClick("특별 연구동", null, 1000);

            if (!DetectTarget("기본 생산기술3", 200, 10, 20))
                return false;
            LeftClick("버튼 다음", null, 1000);

            if (!DetectTarget("기본x생산기술3", 200, 10, 20))
                return false;
            LeftClick("기본x생산기술3", null, 1000);

            if (!DetectTarget("버튼 다음", 200, 10, 20))
                return false;
            LeftClickAndOff("버튼 다음", null, new kvec(0, 30), 1000);

            if (!DetectTarget("버튼 확인", 200, 10, 20))
                return false;
            LeftClick("버튼 확인", null, 1000);

            return true;
        }
        bool studyApplyHakSul()
        {
            if (!MacroRunning) return false;

            if (!TabAndDetectNotFront("상인 의뢰인", 50, 10, 30, 1000))
                return false;

            if (!DetectTarget("직업 연구", 200, 10, 20))
                return false;
            LeftClick("직업 연구", null, 1000);

            if (!DetectTarget("초급 연구동 학술", 200, 10, 20))
                return false;
            LeftClick("버튼 다음", null, 1000);

            if (!DetectTarget("조리사의x연구 1", 200, 10, 20))
                if (!DetectTarget("조리사의 연구 1", 200, 10, 20))
                    return false;
            LeftClick("조리사의 연구 1", null, 1000);

            if (!DetectTarget("버튼 다음", 200, 10, 20))
                return false;
            LeftClick("버튼 다음", null, 500);
            MouseOffset(0, 30, 500);

            if (!DetectTarget("버튼 확인", 200, 10, 20))
                return false;
            LeftClick("버튼 확인", null, 1000);

            return true;
        }
        void RunOnMap(string place, int walkingSec)
        {
            if (!MacroRunning) return;

            if (place == "은행")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("맵 은행 페낭", null, 1000 * walkingSec);
            }
            else if (place == "교역소")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("맵 교역소 페낭", null, 1000 * walkingSec);
            }

            RightClick("맵 센터", 1000);
            RightClick("맵 센터", 1000);
        }
    }
}
