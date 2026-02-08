using shine.libs.system;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using shine.libs.math;

namespace Bacro
{
    public partial class PlayFactory : Player
    {
        void setEdinburgh()
        {
            //pics["에딘버러 어육"] = "76 191 48 48::4 35,10 32,21 24,40 15::160 131 134,107 96 115,222 217 226,166 137 137";
            //pics["에딘버러 양"] = "76 303 48 48::4 18,8 12,13 11,15 24,31 7,43 14::214 214 207,4 2 2,195 195 194,146 132 113,229 226 221,148 138 117";
            
            pics["비스뷔 양"] = "76 303 48 48::4 14,8 12,13 11,31 8,41 18::218 216 209,4 2 2,195 195 194,226 225 220,148 138 117";
            pics["개인상점 어육"] = "123 137 48 48::4 35,9 31,23 23,39 14::160 131 134,64 53 75,243 241 244,162 148 174";
            pics["개인상점 나무 열매"] = "123 193 48 48::10 26,17 29,20 21,26 26,33 25::255 246 116,167 84 82,254 252 140,199 132 114,163 123 168";
            pics["신비한 향신료"] = "747 74 48 48::16 12,20 13,23 17,33 15::225 220 215,184 136 96,225 220 215,195 151 57";

            /* 행동력 변수 */
            //EngeryFull = 460;
            EnergyFull = 440;
            EnergyOfOnePizza = 70;
            FoodSlotNum = 4;
        }
        void products_Edinburgh00()
        {
            product("양고기", 0, 0, 1);
            product("양고기", 0, 0, 1);
            product("양고기", 0, 0, 1);

            product("소세지", 0, 1, 1);
            product("소세지", 0, 1, 1);
            product("소세지", 0, 1, 1);
            product("소세지", 0, 1, 1);
        }
        void products_Edinburgh()
        {
            product("양고기", 6, 2, 1);
            product("양고기", 6, 2, 1);
            product("양고기", 6, 2, 1);

            product("소세지", 6, -1, 1);
            product("소세지", 6, -1, 1);
            product("소세지", 6, -1, 1);
            product("소세지", 6, -1, 1);
        }
        void playmacro_Edinburgh(int loopTarget, bool doFoodProduct)
        {
            /* 매크로 시작 */
            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"풀파워 == {EnergyFull}");
            Console.WriteLine($"한개파워 == {EnergyOfOnePizza}");

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 소세지생산☎  # {loopcount}/{loopTarget}");

                /* 재료구입, 3번 시도 */
                var count = 0;
                while (++count <= 3 && !buyEdinburgh(1))
                    Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 양 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                /* 슬롯넘버, 레시피인덱스 대량생산횟수 */
                //product("양고기", 0, 0, 1);
                //product("양고기", 0, 0, 1);
                //product("양고기", 0, 0, 1);

                //product("소세지", 0, 1, 1);
                //product("소세지", 0, 1, 1);
                //product("소세지", 0, 1, 1);

                products_Edinburgh();

                sell();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");

                if (doFoodProduct && loopcount % 5 == 0)
                {
                    if (product_foods_Edinburgh(55))
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

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");
        }
        bool buyEdinburgh(int nBalju)//비스뷔
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

            LeftClickLong(pScrollDown, 500, 1000);

            //if (!DetectTarget("에딘버러 양", 200, 5, 20))
            if (!DetectTarget("비스뷔 양", 200, 5, 20))
                    return false;

            LeftClickTwice(pMulGun4, 500);
            LeftClick(pMulGun4 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool buyEdinburgh00(int nBalju)
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

            kvec scrollDown = parsePicCen(pics["스크롤바 하단"]);

            var yangFound = false;
            for (int i = 0; i < 3; i++)
            {
                LeftClick(scrollDown, null, null, 1000);

                if (DetectTarget("에딘버러 양", 200, 5, 20))
                {
                    yangFound = true;
                    break;
                }
            }
            if (!yangFound)
                return false;

            LeftClickTwice(pMulGun4, 500);
            LeftClick(pMulGun4 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool product_foods_Edinburgh(int FRUITs)
        {
            if (!MacroRunning)
                return false;

            /* 아델리야 개인상점 열매 사기 */
            if (!TabAndDetectFront("라꾸땐시 도시", 30, 10, 20, 500))
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
            if (!DetectTarget("에딘버러 어육", 200, 10, 20))
                return false;
            LeftClick("에딘버러 어육", null, 1000);

            var posMax = parsePicCen(pics["에딘버러 어육"]);
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
