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
        void setLuebeck()
        {
            pics["개인상점 어육"] = "123 137 48 48::4 35,9 31,23 23,39 14::160 131 134,64 53 75,243 241 244,162 148 174";
            pics["개인상점 나무 열매"] = "123 193 48 48::10 26,17 29,20 21,26 26,33 25::255 246 116,167 84 82,254 252 140,199 132 114,163 123 168";
            pics["신비한 향신료"] = "747 74 48 48::16 12,20 13,23 17,33 15::225 220 215,184 136 96,225 220 215,195 151 57";

            pics["뤼베크 납광석"] = "76 247 48 48::9 15,19 15,34 17,38 20::188 182 189,142 151 156,230 222 181,148 166 174";
            pics["뤼베크 철광석"] = "76 303 48 48::11 25,16 18,17 16,24 15,28 16::138 156 169,121 135 145,220 220 219,220 220 219,189 204 214";
            pics["뤼베크 동광석"] = "76 135 48 48::10 24,20 22,23 16,35 15::70 126 105,96 95 96,99 156 134,183 171 164";
            pics["뤼베크 아연광석"] = "76 247 48 48::10 24,20 22,23 16,35 15::71 67 65,90 84 83,156 134 124,96 68 66";

            /* 행동력 변수 */
            EnergyFull = 460;
            EnergyOfOnePizza = 70;
            FoodSlotNum = 4;

            /* 반복 변수 */
            nFruitConsumed = 15;
            nLoopCount = 60 * 2;
        }
        void products_Luebeck()
        {
            product("놋쇠", 0, 0, 2);
            product("철재", 0, 1, 2);

            //product("납", 0, 2, 2);
        }
        void playmacro_Luebeck(int loopTarget, bool doFoodProduct)
        {
            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"캐릭 체력 == {EnergyFull}");
            Console.WriteLine($"행음 에너지 == {EnergyOfOnePizza}");

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"♨ 놋쇠생산♨  # {loopcount}/{loopTarget}");

                /* 재료구입, 3번 시도 */
                var count = 0;
                while (++count <= 3 && !buyLuebeck(1))
                    Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 동광석 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                products_Luebeck();

                sell();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");

                if (doFoodProduct && loopcount % 6 == 0)
                {
                    if (product_foods_TwoOne(nFruitConsumed))
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
        bool buyLuebeck(int nBalju)
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

            /* 반칸 스크롤 */
            var posDown = new kvec(pScrollUp.x, pMulGun1.y);
            ScrollDownOff(posDown, new kvec(0, 56 * 0.5f), 500, 1000);

            //if (!DetectTarget("뤼베크 납광석", 200, 5, 20))
            //    return false;
            //var posNap = parsePicCen(pics["뤼베크 납광석"]);
            //LeftClick2(posNap, 500);
            //LeftClick(posNap + vecMax, null, null, 500);
            //KeysClick("enter", 100, 1000);

            if (!DetectTarget("뤼베크 철광석", 200, 5, 20))
                return false;
            var posChul = parsePicCen(pics["뤼베크 철광석"]);
            LeftClickTwice(posChul, 500);
            LeftClick(posChul + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            /* 스크롤바 하단, 완전히 내린다 */
            LeftClickLong(pScrollDown, 500, 500);

            if (!DetectTarget("뤼베크 동광석", 200, 5, 20))
                return false;
            var posDong = parsePicCen(pics["뤼베크 동광석"]);
            LeftClickTwice(posDong, 500);
            LeftClick(posDong + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            if (!DetectTarget("뤼베크 아연광석", 200, 5, 20))
                return false;
            var posAYeon = parsePicCen(pics["뤼베크 아연광석"]);
            LeftClickTwice(posAYeon, 500);
            LeftClick(posAYeon + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            //
            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool product_foods_TwoOne(int FRUITs)
        {
            if (!MacroRunning)
                return false;

            /* 라꾸땐시 개인상점 열매 사기 */
            if (!TabAndDetectFront("아델리야 도시", 30, 10, 20, 500))
            //if (!TabAndDetectFront("라꾸땐시 도시", 30, 10, 20, 500))
                    return false;

            /* 개인상점 버튼 누르기 */
            var count = 0;
            while (++count <= 5 && !DetectTarget("버튼 개인상점", 200, 10, 20))
                KeysClick("space", 0, 500);
            if (count > 5)
                return false;

            LeftClick("버튼 개인상점", null, 1000);

            /* 나무열매 구입 */
            if (!DetectTarget("개인상점 나무 열매", 200, 10, 20))
                return false;
            LeftClick("개인상점 나무 열매", null, 500);
            LeftClick("개인상점 나무 열매", null, 500);//확실하게 두번 클릭

            KeysClick($"%{FRUITs}", 100, 1000);
            KeysClick("enter", 0, 1000);

            /* 어육 구입 */
            if (!DetectTarget("개인상점 어육", 200, 10, 20))
                return false;
            LeftClick("개인상점 어육", null, 500);
            LeftClick("개인상점 어육", null, 500);//확실하게 두번 클릭

            KeysClick($"%{FRUITs * 3}", 100, 1000);
            KeysClick("enter", 0, 1000);

            /* 구입 확인 */
            if (!DetectTarget("버튼 개인상점 구입", 200, 10, 20))
                return false;
            LeftClick("버튼 개인상점 구입", null, 1000);

            /* 나무열매 음식 생산*/
            product("나무열매 음식", 0, -1, 1);

            /* 신비한 향신료 사용해서 생산 */
            //product_fx("나무열매 음식", 0, -1, 1, "신비한 향신료");

            return true;
        }
    }
}
