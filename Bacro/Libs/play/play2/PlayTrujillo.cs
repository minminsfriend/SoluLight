using shine.libs.math;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bacro
{
    public partial class PlayFactory : Player
    {
        void set_Trujillo()
        {
            /* 행동력 변수 */
            EnergyFull = 800;
            EnergyOfOnePizza = 70;
            FoodSlotNum = 4;

            Console.WriteLine($"개인체력 == {EnergyFull}");
            Console.WriteLine($"음식용량 == {EnergyOfOnePizza}");

            pics["트루히요 맵 교역소"] = "561 138 18 18::9 3,3 4,14 4,7 13,15 13::234 225 114,241 242 111,255 255 114,255 255 147,255 255 144";
            pics["트루히요 맵 은행"] = "611 117 17 18::10 2,7 9,15 12::254 246 231,5 12 32,131 94 46";
            pics["트루히요 맵 항구"] = "476 189 16 17::8 2,7 5,3 8,13 7,8 12,10 14::237 229 185,255 235 122,255 252 141,255 229 78,255 242 127,0 0 1";

            pics["트루히요 돼지"] = "76 247 48 48::7 27,10 25,26 12,36 11::216 197 186,22 14 13,212 195 185,242 223 209";
            pics["트루히요 어육"] = "76 191 48 48::5 33,9 32,21 23,38 14::104 80 86,74 64 81,229 225 231,175 165 181";
            pics["두캇 아이콘"] = "21 55 15 18::2 9,5 3,10 9::255 255 255,255 250 227,255 255 255";

            pics["아델리야 도시"] = "625 348 176 32::11 10,19 21,26 9,35 21,41 10,51 21,59 10,69 12,69 17::255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255";
            pics["개인상점 나무열매"] = "123 137 48 48::9 25,19 19,27 24,35 26::255 245 95,202 164 7,255 253 204,148 113 150";
        }
        void playmacro_Trujillo_Adelliya(int loopTarget, bool withStudy)
        {
            /* 매크로 시작 */
            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfOnePizza}");

            int loopcount = 0;
            var invest = true;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 햄생산☎  # {loopcount}/{loopTarget}");

                /* 돼지 사기, 3번 시도 */
                var count = 0;
                //while (++count <= 3 && !buyPigsTru(2))
                while (++count <= 3 && !buyPigsTru(3, false))
                        Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 돼지 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                /* 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
                product("돼지 고기", 0, 0, 4);
                product("돼지 고기", 0, 0, 2);
                product("햄", 0, 1, 4);
                product("햄", 0, 1, 4);
                product("햄", 0, 1, 2);

                //product("돼지 고기", 0, 0, 2);
                //product("햄", 0, 1, 2);
                //product("햄", 0, 1, 2);

                sell();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");

                if (withStudy)
                {
                    if (loopcount % 2 == 0)
                    {
                        StudyApply();

                        timeSpent = KSys.ConsumedTime(time0);
                        Console.WriteLine($"  ▶▶ {timeSpent} (연구까지)");
                    }
                    if (loopcount % 2 == 0)
                    {
                        if(product_foods_Trujillo(45, true))
                        {
                            timeSpent = KSys.ConsumedTime(time0);
                            Console.WriteLine($"  ▶▶ {timeSpent} (피자생산까지)");
                        }
                        else
                        {
                            Console.WriteLine($"  !! 음식 생산 실패, 작업 중단");
                            MacroRunning = false;
                        }
                    }
                }
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            //if (MacroRunning)
            //    main.VoiceSpeak("햄 생산이 모두 끝났습니다. 매크로를 종료합니다.");
            //else
            //    main.VoiceSpeak("매크로가 도중에 중단되었섭니다. 매크로가 중단되었습니다.");
        }
        void playmacro_Trujillo(int loopTarget, bool withStudy)
        {
            /* 매크로 시작 */
            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            int loopcount = 0;
            var invest = true;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 햄생산☎  # {loopcount}/{loopTarget}");
                RightClick("맵 센터", 1000);

                /* 돼지 사기, 3번 시도 */
                var count = 0;
                //while (++count <= 3 && !buyPigsTru(1, false))
                while (++count <= 3 && !buyPigsTru(3, true))
                    Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 돼지 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                playmacro_Trujillo_product();

                sell();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");

                if (withStudy)
                {
                    if (loopcount % 2 == 0)
                    {
                        StudyApply();

                        timeSpent = KSys.ConsumedTime(time0);
                        Console.WriteLine($"  ▶▶ {timeSpent} (연구까지)");
                    }
                    if (loopcount % 3 == 0)
                    {
                        if(product_foods_Trujillo(40, invest))
                        {
                            timeSpent = KSys.ConsumedTime(time0);
                            Console.WriteLine($"  ▶▶ {timeSpent} (피자생산까지)");
                        }
                        else
                        {
                            Console.WriteLine($"  !! 음식 생산 실패, 작업 중단");
                            MacroRunning = false;
                        }
                    }
                }
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");
        }
        void playmacro_Trujillo_product()
        {
            /* 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
            product("돼지 고기", 0, 0, 3);
            product("돼지 고기", 0, 0, 3);

            product("햄", 0, 1, 4);
            product("햄", 0, 1, 4);
            product("햄", 0, 1, 3);
            //product("햄", 0, 1, 2);
        }
        void playmacro_Trujillo_product00()
        {
            /* 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
            product("돼지 고기", 0, 0, 1);
            product("돼지 고기", 0, 0, 1);

            product("햄", 0, 1, 1);
            product("햄", 0, 1, 1);
            product("햄", 0, 1, 1);
            product("햄", 0, 1, 1);
        }
        void playmacro_Trujillo_G(int loopTarget, bool withStudy)
        {
            /* 매크로 시작 */
            var timeStart = KSys.CurrentMillis();
            int loopcount = 0;
            var invest = true;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 햄관리☎  # {loopcount}/{loopTarget}");

                playmacro_Trujillo_G_OutOfCity();
                
                Thread.Sleep(1000);
                playmacro_Trujillo_G_product();

                if (!goalIntoTrujilo())
                {
                    MacroRunning = false;
                    break;
                }

                Thread.Sleep(1000);
                playmacro_Trujillo_G_SellAndBuy();
                
                Thread.Sleep(1000);

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");

                if (withStudy)
                {
                    if (loopcount % 5 == 0)
                    {
                        StudyApply();

                        timeSpent = KSys.ConsumedTime(time0);
                        Console.WriteLine($"  ▶▶ {timeSpent} (연구까지)");
                    }
                    if (loopcount % 5 == 0)
                    {
                        if (product_foods_Trujillo(35, invest))
                        {
                            timeSpent = KSys.ConsumedTime(time0);
                            Console.WriteLine($"  ▶▶ {timeSpent} (피자생산까지)");
                        }
                        else
                        {
                            Console.WriteLine($"  !! 음식 생산 실패, 작업 중단");
                            MacroRunning = false;
                        }
                    }
                }
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");
        }
        void playmacro_Trujillo_G_product()
        {
            if (!MacroRunning)
                return;

            main.sail.SailFold(500);
            main.sail.SailSpread(1, 1000);

            product("돼지 고기", 0, 0, 1);
            product("돼지 고기", 0, 0, 1);
            product("햄", 0, 1, 1);

            //product("햄", 0, 1, 1);

            var posTar = new kvec(400, 600 - 50);

            LeftClick(posTar, 100);
            LeftClick(posTar, 500);
            Thread.Sleep(1000);
       
            product("햄", 0, 1, 2);
            product("햄", 0, 1, 1);

            main.sail.ShowFront(500);
            main.sail.ShowFront(1000);
        }
        void playmacro_Trujillo_G_product00()
        {
            if (!MacroRunning)
                return;
            
            main.sail.SailFold(500);
            main.sail.SailSpread(1, 1000);

            product("돼지 고기", 0, 0, 2);
            product("돼지 고기", 0, 0, 2);

            product("햄", 0, 1, 2);

            var posTar = new kvec(400, 600 - 50);

            LeftClick(posTar, 100);
            LeftClick(posTar, 500);
            Thread.Sleep(1000);

            product("햄", 0, 1, 2);
            product("햄", 0, 1, 2);
            product("햄", 0, 1, 1);

            main.sail.ShowFront(500);
            main.sail.ShowFront(1000);
        }
        void playmacro_Trujillo_G_SellAndBuy()
        {
            runOnMapTrujillo("교역소", 5);
            sell();
            //buyPigsTru(2, false);
            buyPigsTru(1, false);
        }
        void playmacro_Trujillo_G_OutOfCity()
        {
            playTrujillo_IntoPortFromCity();
            Thread.Sleep(1000);
            playTrujillo_OutOfPortToSea();

            /* 3초간 상태 확인 */
            if (!DetectTarget("두캇 아이콘", 300, 10, 30))
                return;
        }
        bool buyPigsTru(int nBalju, bool invest)
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

            //스크롤바 하단
            if (invest)
            {
                if (!DetectTarget("스크롤바 하단", 200, 5, 20))
                    return false;
                var posScrollDown = parsePicRect(pics["스크롤바 하단"]).Cen;
                LeftClickLong(posScrollDown, 1000, 1000);
            }

            /* 교역품 구입 */
            if (!DetectTarget("트루히요 돼지", 200, 5, 20))
                return false;

            LeftClickTwice(pMulGun3, 500);
            LeftClick(pMulGun3 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool product_foods_Trujillo(int FRUITs, bool invest)
        {
            /* 아델리야 개인상점 열매 사기 */
            //if (!TabAndDetectFront("아델리야 도시", 30, 10, 20, 500))
            if (!TabAndDetectFront("아델리야 도시", 30, 10, 20, 500))
                    return false;

            /* 개인상점 버튼 누르기 */
            var count = 0;
            while (++count <= 5 && !DetectTarget("버튼 개인상점", 200, 10, 20))
                KeysClick("space", 0, 500);
            if (count > 5)
                return false;

            LeftClick("버튼 개인상점", null, 1000);

            /* 나무열매 57개 구입 */
            if (!DetectTarget("개인상점 나무열매", 200, 10, 20))
                return false;
            LeftClick("개인상점 나무열매", null, 500);
            LeftClick("개인상점 나무열매", null, 500);//한번더 눌러준다, 화면을 기다려주는 것?

            KeysClick($"%{FRUITs}", 100, 1000);
            KeysClick("enter", 0, 1000);

            if (!DetectTarget("버튼 개인상점 구입", 200, 10, 20))
                return false; 
            LeftClick("버튼 개인상점 구입", null, 1000);

            /* 교역소 주인에게 어육 사기 */
            if (!TabAndDetectFront("교역소 주인", 30, 10, 20, 500))
                return false;

            /* 교역소 버튼 누르기 */
            count = 0;
            while (++count <= 5 && !DetectTarget("교역소 구입", 200, 10, 20))
                KeysClick("space", 0, 500);
            if (count > 5)
                return false;

            LeftClick("교역소 구입", null, 1000);

            //스크롤바 하단
            if (invest)
            {
                if (!DetectTarget("스크롤바 하단", 200, 5, 20))
                    return false;
                var posScrollDown = parsePicRect(pics["스크롤바 하단"]).Cen;
                LeftClickLong(posScrollDown, 1000, 1000);
            }
            /* 어육 사기 */
            if (!DetectTarget("트루히요 어육", 200, 10, 20))
                return false;
            LeftClick("트루히요 어육", null, 1000);

            KeysClick($"%{3 * FRUITs}", 100, 1000);
            KeysClick("enter", 0, 1000);

            LeftClick("구입 확인", null, 1000);

            /* 나무열매 음식 생산*/
            product("나무열매 음식", 0, -1, 1);

            return true;
        }
        bool goalIntoTrujilo()
        {
            string cityName = "트루히요";
            if (TabAndDetectNotFront(cityName, 10, 10, 30, 100))
            {
                Thread.Sleep(1000);
                Console.WriteLine($"☆☆  Tab 키로 [{cityName}] 인식 !  ☆☆ ");
                Console.WriteLine($"  바다에서 항구로 입항.");

                var count = 0;
                while (++count <= 5)
                {
                    if (DetectTarget("항구 앞", 300, 10, 30))
                    {
                        /* 항구로 들어온 상태 */
                        Console.WriteLine("DetectTarget [항구 앞]");
                        LeftClick("항구 앞", null, 2000);
                        return true;
                    }

                    KeysClick("space", 0, 3000);
                }

                Console.WriteLine(" ** 도시 인식은 했으나 들어오지 못함.");
                return false;
            }

            Console.WriteLine(" ** 도시 인식을 못함.");
            return false;
        }
        bool playTrujillo_IntoPortFromCity()
        {
            if (!MacroRunning)
                return false;

            runOnMapTrujillo("항구", 4);

            if (!DetectTarget("두캇 아이콘", 300, 10, 30))
                return false;

            if (!TabAndDetectNotFront("항구관리", 50, 10, 30, 1000))
                return false;

            if (!DetectTarget("항구로 이동", 200, 10, 30))
                return false;
            LeftClick("항구로 이동", null, 3000);

            if (!DetectTarget("두캇 아이콘", 300, 10, 30))
                return false;

            Console.WriteLine("  항구에서 출항준비.");
            return true;
        }
        void playTrujillo_OutOfPortToSea()
        {
            if (!MacroRunning)
                return;

            if (!DetectTarget("보급", 200, 20, 30))
                return;
            LeftClick("보급", null, 2000);

            LeftClick("기억 보급", null, 1000);
            LeftClick("보급 확인", null, 1000);

            if (!DetectTarget("출항", 200, 10, 30))
                RightClick(CenFront, 1000);

            LeftClick("출항", null, 1000);
            LeftClick("출항 2", null, 4000);
        }
        void runOnMapTrujillo(string place, int walkingSec)
        {
            if (!MacroRunning) return;

            if (place == "학자")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("트루히요 맵 학자", null, 1000 * walkingSec);
            }
            else if (place == "항구")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("트루히요 맵 항구", null, 1000 * walkingSec);
            }
            else if (place == "교역소")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("트루히요 맵 교역소", null, 1000 * walkingSec);
            }

            RightClick("맵 센터", 1000);
            RightClick("맵 센터", 1000);
        }
    }
}
