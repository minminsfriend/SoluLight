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
            //EngeryFull = 460;
            //EngeryFull = 925;
            EnergyFull = 575;
            EnergyOfPizza = 70;
            FoodSlotNum = 4;

            pics["트루히요 맵 교역소"] = "561 138 18 18::9 3,3 4,14 4,7 13,15 13::234 225 114,241 242 111,255 255 114,255 255 147,255 255 144";
            pics["트루히요 맵 은행"] = "611 117 17 18::10 2,7 9,15 12::254 246 231,5 12 32,131 94 46";
            pics["트루히요 맵 항구"] = "476 189 16 17::8 2,7 5,3 8,13 7,8 12,10 14::237 229 185,255 235 122,255 252 141,255 229 78,255 242 127,0 0 1";
        }
        void playmacro_Trujillo(int loopTarget, bool withStudy)
        {
            /* 매크로 시작 */
            MacroRunning = true;

            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 햄생산☎  # {loopcount}/{loopTarget}");

                /* 돼지 사기, 3번 시도 */
                var count = 0;
                //while (++count <= 3 && !buyPigsTru(2))
                while (++count <= 3 && !buyPigsTru(3))
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
                        if(product_foods_Trujillo())
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

            MacroRunning = false;
        }
        void playmacro_Trujillo_1()
        {
            EnergyFull = 925;
   
            MacroRunning = true;

            Console.WriteLine(" ** 햄생산 한번만 시작 ** ");

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            product("돼지 고기", 0, 0, 4);
            product("돼지 고기", 0, 0, 2);

            product("햄", 0, 1, 4);
            product("햄", 0, 1, 4);
            product("햄", 0, 1, 2);

            sell();

            Console.WriteLine(" ** 햄생산 한번만 완료 **");
            MacroRunning = false;
        }
        void playmacro_Trujillo_G(int loopTarget, bool withStudy)
        {
            /* 행동력 변수 */
            //EngeryFull = 460;
            EnergyFull = 925;
            EnergyOfPizza = 70;
            FoodSlotNum = 4;

            /* 매크로 시작 */
            MacroRunning = true;

            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 햄생산☎  # {loopcount}/{loopTarget}");

                /* 돼지 사기, 3번 시도 */
                var count = 0;
                //while (++count <= 3 && !buyPigsTru(2))
                while (++count <= 3 && !buyPigsTru(3))
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
                        if(product_foods_Trujillo())
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

            MacroRunning = false;
        }
        void playmacro_Trujillo_G_1()
        {
            MacroRunning = true;

            Console.WriteLine(" ** 햄관리 한번만 시작 ** ");

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            if (EnergyFull > 900)
            {
                product("돼지 고기", 0, 0, 4);
                product("돼지 고기", 0, 0, 2);

                product("햄", 0, 1, 4);
                product("햄", 0, 1, 4);
                product("햄", 0, 1, 2);
            }
            else if (EnergyFull > 500)
            {
                product("돼지 고기", 0, 0, 2);
                product("돼지 고기", 0, 0, 2);

                product("햄", 0, 1, 2);

                var posTar = new kvec(400, 600 - 50);

                LeftClick(posTar, 100);
                LeftClick(posTar, 500);

                product("햄", 0, 1, 2);
                product("햄", 0, 1, 2);
                product("햄", 0, 1, 1);

                RightClick(PosCen, 1000);
            }

            //sell();

            Console.WriteLine(" ** 햄관리 한번만 완료 **");
            MacroRunning = false;
        }
        bool buyPigsTru(int nBalju)
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

            if(!DetectTarget("진열품 돼지", 200, 5, 20))
                return false;

            LeftClickTwice(pMulGun3, 500);
            LeftClick(pMulGun3 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool product_foods_Trujillo()
        {
            //var FRUITs = 33;//140 개 생산
            var FRUITs = 30;//140 개 생산

            /* 아델리야 개인상점 열매 사기 */
            //if (!TabAndDetectFront("아델리야 도시", 30, 10, 20, 500))
            if (!TabAndDetectFront("빼아뜨리체 도시", 30, 10, 20, 500))
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

            /* 어육 사기 */
            if (!DetectTarget("진열품 어육", 200, 10, 20))
                return false;
            LeftClick("진열품 어육", null, 1000);

            var posMax = parsePicCen(pics["진열품 어육"]);
            posMax.offset(vecMax);
            LeftClickTwice(posMax, 1000);
            KeysClick("enter", 0, 1000);

            LeftClick("구입 확인", null, 1000);

            /* 나무열매 음식 생산*/
            product("나무열매 음식", 0, -1, 1);

            return true;
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
