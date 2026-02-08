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
        void set_Herder()
        {
            /* 행동력 변수 */
            //EngeryFull = 460;
            //EngeryFull = 925;
            //EnergyFull = 685;
            EnergyFull = 340;
            EnergyOfOnePizza = 70;
            FoodSlotNum = 4;

            Console.WriteLine($"개인체력 == {EnergyFull}");
            Console.WriteLine($"음식용량 == {EnergyOfOnePizza}");

            pics["헤르데르 어육"] = "76 135 48 48::5 33,9 32,21 23,38 14::104 80 86,74 64 81,229 225 231,175 165 181";
            pics["헤르데르 양"] = "76 247 48 48::4 14,8 12,13 11,31 8,41 18::218 216 209,4 2 2,195 195 194,226 225 220,148 138 117";

            pics["아델리야 도시"] = "625 348 176 32::11 10,19 21,26 9,35 21,41 10,51 21,59 10,69 12,69 17::255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255";
            pics["개인상점 나무열매"] = "123 137 48 48::9 25,19 19,27 24,35 26::255 245 95,202 164 7,255 253 204,148 113 150";
        }
        void playmacro_Herder(int loopTarget, bool withStudy)
        {
            /* 매크로 시작 */
            var timeStart = KSys.CurrentMillis();

            RightClick("맵 센터", 1000);

            int loopcount = 0;

            while (++loopcount <= loopTarget && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Console.WriteLine($"☎ 햄생산☎  # {loopcount}/{loopTarget}");
                RightClick("맵 센터", 1000);

                /* 양 구입, 3번 시도 */
                var count = 0;
                while (++count <= 3 && !buySheepsHerder(1, true))
                    //while (++count <= 3 && !buySheepsHerder(3))
                    Thread.Sleep(1000);

                if (count > 3)
                {
                    Console.WriteLine($"▶▶ 양 구입 실패 !!");
                    MacroRunning = false;
                    break;
                }

                playmacro_Herder_product();

                sell();

                var timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent}");

            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");
        }
        void playmacro_Herder_product()
        {
            /* 변수 의미: 마이레시피(0) 레시피2인덱스 대량생산클릭1번*/
            product("양고기", 0, 0, 1);
            product("양고기", 0, 0, 1);
            product("양고기", 0, 0, 1);

            product("소세지", 0, 1, 1);
            product("소세지", 0, 1, 1);
            product("소세지", 0, 1, 1);
            product("소세지", 0, 1, 1);
        }
        bool buySheepsHerder(int nBalju, bool doScrolldown)
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
            if (doScrolldown)
            {
                if (!DetectTarget("스크롤바 하단", 200, 5, 20))
                    return false;
                var posScrollDown = parsePicRect(pics["스크롤바 하단"]).Cen;
                LeftClickLong(posScrollDown, 1000, 1000);
            }

            /* 교역품 구입 */
            if (!DetectTarget("헤르데르 양", 200, 5, 20))
                return false;

            LeftClickTwice(pMulGun3, 500);
            LeftClick(pMulGun3 + vecMax, null, null, 500);
            KeysClick("enter", 100, 1000);

            LeftClick("구입 확인", null, 1000);
            return true;
        }
        bool product_foods_Herder(int FRUITs)
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
            if (false)
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
    }
}
