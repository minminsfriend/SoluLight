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
        void setPorto()
        {
            pics["포르투 어육"] = "76 135 48 48::5 34,8 33,18 27,32 23,39 14::134 111 120,68 57 79,236 233 237,201 192 206,162 148 174";
            pics["포르투 밀"] = "76 191 48 48::12 15,14 24,17 26,26 24,30 18,28 9::203 179 136,163 130 80,255 233 134,203 179 136,222 183 111,239 226 200";
            pics["포르투 치즈"] = "76 247 48 48::11 12,31 9,27 17,27 19::217 185 119,209 171 101,156 127 51,218 146 45";

            pics["맵 교역소 포르투"] = "423 179 16 16::8 2,2 3,13 3,4 14,12 14::234 225 114,241 242 111,255 255 114,255 255 149,255 255 116";
            pics["맵 도구점 포르투"] = "477 188 15 16::7 1,2 7,13 7,6 10,7 10::200 199 199,201 199 199,204 204 203,188 106 2,88 50 26";

            pics["아이템 해물피자"] = "249 305 48 48::11 16,18 15,31 16,31 19,38 22::234 230 183,239 237 182,239 237 222,110 114 46,239 238 237";
            pics["아이템 판매 Max"] = "341 421 52 20::6 4,27 5,43 7,45 10,26 13::255 255 255,204 221 221,187 204 221,119 153 170,170 204 221";
            pics["아이템 판매 확인"] = "625 435 52 20::6 4,15 6,15 8,32 6,39 7::255 255 255,0 0 0,183 203 215,196 213 223,48 51 53";

            pics["도구점 매각"] = "673 376 38 22::7 5,18 4,16 11,13 13,18 17::247 248 248,210 202 46,210 128 57,29 29 29,210 206 163";

            /* 행동력 변수 */
            EnergyFull = 745;
            EnergyOfPizza = 50;
            FoodSlotNum = 4;
        }
        void products_Porto()
        {
            /* 슬롯넘버, 레시피인덱스 대량생산횟수 */
       
            product("밀가루", 5, 2, 1);

            product("해물피자", 6, 0, 2);
        }
        private bool productAndSellPizza()
        {
            for (int i = 0; i < 6; i++)
            {
                product("해물피자", 6, 0, 2);

                Thread.Sleep(1000);

                if (!DetectTarget("도구점 매각", 200, 10, 20))
                    return false;
                LeftClick("도구점 매각", null, 1000);

                /* 소유 아이템 창고에서 해물피자 선택 */
                if (!DetectTarget("아이템 해물피자", 200, 15, 20))
                    return false;
                LeftClick("아이템 해물피자", null, 1000);

                /* 숫자버튼이 뜨면 170 입력 */
                if (!DetectTarget("아이템 판매 Max", 200, 10, 20))
                    return false;
                KeysClick("%180", 300, 1000);
                KeysClick("enter", 0, 1000);

                /* 최종 매각 */
                if (!DetectTarget("아이템 판매 확인", 200, 5, 20))
                    return false;
                LeftClick("아이템 판매 확인", null, 1000);
            }

            product("해물피자", 6, 0, 1);

            return true;
        }
        private bool productMilGaru()
        {
            product("밀가루", 5, 2, 2);
            product("밀가루", 5, 2, 2);
            product("밀가루", 5, 2, 1);

            return true;
        }
        void playmacro_Porto(int loopTarget, bool doFoodProduct)
        {
            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"캐릭 체력 == {EnergyFull}");
            Console.WriteLine($"행음 에너지 == {EnergyOfPizza}");

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"♨ 피자생산♨  # {loopcount}/{loopTarget}");

                /* 재료구입, 3번 시도 */
                var count = 0;
                while (++count <= 3 && !buyPorto(5))
                    Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 재료 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                /* 밀가루 만들기 */
                productMilGaru();

                RunOnMap_Porto("도구점", 3);

                if (!TabAndDetectFront("도구점 주인", 30, 10, 20, 500))
                    break;

                /* 매각 버튼이 보이는 상태 유지 */
                count = 0;
                while (++count <= 5 && !DetectTarget("도구점 매각", 200, 10, 20))
                    KeysClick("space", 100, 500);
                if (count > 5)
                    break;

                productAndSellPizza();

                RunOnMap_Porto("교역소", 3);
                sell();//교역소에 남은 재료들 넘긴다

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");
        }
        void playmacro_Porto00(int loopTarget, bool doFoodProduct)
        {
            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            Console.WriteLine($"캐릭 체력 == {EnergyFull}");
            Console.WriteLine($"행음 에너지 == {EnergyOfPizza}");

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"♨ 피자생산♨  # {loopcount}/{loopTarget}");

                /* 재료구입, 3번 시도 */
                var count = 0;
                while (++count <= 3 && !buyPorto(5))
                    Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 재료 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                products_Porto();

                if (loopcount % 5 == 0)
                    sell();//교역소에 남은 재료들 넘긴다

                RunOnMap_Porto("도구점", 3);

                if (!sell_pizza())
                {
                    Console.WriteLine($" ▶ 피자 판매 실패 !!");
                    MacroRunning = false;
                    break;
                }

                RunOnMap_Porto("교역소", 3);

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");
        }
        bool buyPorto(int nBalju)
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
            if (!DetectTarget("포르투 어육", 200, 5, 20))
                return false;
            var posFish = parsePicCen(pics["포르투 어육"]);
            LeftClickTwice(posFish, 500);
            KeysClick("%740", 300, 1000);
            KeysClick("enter", 0, 1000);

            var posMil = parsePicCen(pics["포르투 밀"]);
            LeftClickTwice(posMil, 500);
            KeysClick("%300", 300, 1000);
            KeysClick("enter", 0, 1000);

            /* 스크롤바 하단, 완전히 내린다 */
            LeftClickLong(pScrollDown, 1000, 500);

            if (!DetectTarget("포르투 치즈", 200, 5, 20))
                return false;
            var posChizu = parsePicCen(pics["포르투 치즈"]);
            LeftClickTwice(posChizu, 500);
            KeysClick("%740", 300, 1000);
            KeysClick("enter", 0, 1000);

            //
            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool sell_pizza()
        {
            if (!MacroRunning) return false;

            if (!TabAndDetectFront("도구점 주인", 30, 10, 20, 500))
                return false;

            var count = 0;
            while (++count <= 5 && !DetectTarget("도구점 매각", 200, 10, 20))
                KeysClick("space", 100, 500);
            if (count > 5)
                return false;

            Thread.Sleep(500);
            LeftClick("도구점 매각", null, 1000);

            /* 소유 아이템 창고에서 해물피자 선택 */
            if (!DetectTarget("아이템 해물피자", 200, 5, 20))
                return false;
            LeftClick("아이템 해물피자", null, 1000);

            /* 숫자버튼이 뜨면 170 입력 */
            if (!DetectTarget("아이템 판매 Max", 200, 10, 20))
                return false;
            KeysClick("%170", 300, 1000);
            KeysClick("enter", 0, 1000);

            /* 최종 매각 */
            if (!DetectTarget("아이템 판매 확인", 200, 5, 20))
                return false;
            LeftClick("아이템 판매 확인", null, 1000);

            return true;
        }
        void RunOnMap_Porto(string place, int walkingSec)
        {
            if (!MacroRunning) return;

            if (place == "도구점")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("맵 도구점 포르투", null, 1000 * walkingSec);
            }
            else if (place == "교역소")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("맵 교역소 포르투", null, 1000 * walkingSec);
            }

            RightClick("맵 센터", 1000);
            RightClick("맵 센터", 1000);
        }
    }
}
