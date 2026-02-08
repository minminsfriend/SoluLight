using shine.libs.math;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bacro
{
    public partial class PlayFactory : Player
    {
        private void setXixon()
        {
            pics["개인상점 어육"] = "123 137 48 48::4 35,9 31,23 23,39 14::160 131 134,64 53 75,243 241 244,162 148 174";
            pics["개인상점 나무 열매"] = "123 193 48 48::10 26,17 29,20 21,26 26,33 25::255 246 116,167 84 82,254 252 140,199 132 114,163 123 168";

            pics["히혼 돼지"] = "76 191 48 48::4 33,10 25,8 16,29 13::223 173 143,22 14 13,237 230 223,238 219 204";
            pics["빌바오 양"] = "76 303 48 48::3 16,8 12,13 11,28 10,39 12::221 217 209,4 2 2,195 195 194,223 221 216,197 192 183";


            /* 행동력 변수 */
            EnergyFull = 445;
            EnergyOfPizza = 70;
            FoodSlotNum = 4;

            /* 반복 변수 */
            nFruitConsumed = 15;
            nLoopCount = 60 * 2;
        }
        void products_Xixon00()
        {
            /* 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
            product("돼지 고기", 0, 1, 1);
            product("돼지 고기", 0, 1, 2);

            product("햄", 0, 0, 1);
            product("햄", 0, 0, 1);
            product("햄", 0, 0, 1);
            product("햄", 0, 0, 1);
        }
        void products_Xixon()
        {
            /* 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
            product("양고기", 5, 2, 1);
            product("양고기", 5, 2, 1);

            product("소세지", 5, -1, 1);
            product("소세지", 5, -1, 1);
            product("소세지", 5, -1, 1);
            product("소세지", 5, -1, 1);
        }
        void playmacro_Xixon(int loopTarget)
        {
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
                while (++count <= 3 && !buyPigsXixon(1))
                        Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 돼지 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                products_Xixon();
            
                sell();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            //if (MacroRunning)
            //    main.VoiceSpeak("햄 생산이 모두 끝났습니다. 매크로를 종료합니다.");
            //else
            //    main.VoiceSpeak("매크로가 도중에 중단되었섭니다. 매크로가 중단되었습니다.");
        }
        void playmacro_Xixon2()
        {
            Console.WriteLine(" ** 햄생산 한번만 시작 ** ");

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfPizza}");

            products_Xixon();

            //sell();

            Console.WriteLine(" ** 햄생산 한번만 완료 **");
        }
        bool buyPigsXixon00(int nBalju)
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

            if(!DetectTarget("히혼 돼지", 200, 5, 20))
                return false;

            LeftClickTwice(pMulGun2, 500);
            LeftClick(pMulGun2 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool buyPigsXixon(int nBalju)
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

            if (!DetectTarget("빌바오 양", 200, 5, 20))
                return false;

            LeftClickTwice(pMulGun4, 500);
            LeftClick(pMulGun4 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool product_foods_Xixon()
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
    }
}
