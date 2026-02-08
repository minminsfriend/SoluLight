using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bacro.Libs.play
{
    internal class PlayMaka : Player
    {
        public PlayMaka(Bacro main) : base(main)
        {
            EngeryFull = 525;
            EnergyOfPizza = 25;

            pics["도시맵"] = "757 571 10 7::4 3,5 3::221 56 54,220 56 54";

            pics["맵 교역소"] = "469 201 18 18::9 4::182 180 69";
            pics["맵 도구점"] = "442 187 17 18::8 2,10 7,7 11::200 199 199,215 147 1,188 106 2";
            pics["맵 은행"] = "500 130 18 18::8 2,10 7,7 11::224 203 155,140 105 50,5 12 32";

            pics["추뇨"] = "193 137 48 48::6 7,8 11,9 12,16 13,24 12::35 64 33,35 62 34,193 154 126,183 134 82,70 43 25";
            pics["초클로"] = "249 137 48 48::6 7,8 11,9 12,16 13,24 12::35 64 33,33 61 32,157 121 96,211 203 168,235 232 205";
        }
        public override void PlayMacro(string cmd)
        {
            if (cmd == "마카")
            {
                int LoopTarget = 1;
                int power = 540;
                int pizza = 50;

                readMacroData(fileinput, ref LoopTarget, ref power, ref pizza);

                playMacro_Maka(LoopTarget);
            }
        }
        void playMacro_Maka(int LoopTarget)
        {
            int loopCount = 0;

            while (loopCount < LoopTarget && MacroRunning)
            {
                Console.WriteLine($"loopCount / LoopTarget == [{loopCount}] / {LoopTarget}");

                //
                Maka_goto("은행");
                Maka_study();

                List<int> baljus = new List<int> { 4, 3 };
                int ntry;

                foreach (var nval in baljus)
                {
                    Maka_goto("교역소");
                    if (!Maka_buy(nval))
                    {
                        MacroRunning = false;
                        break;
                    }

                    //
                    Maka_goto("도구점");
                    TabAndDetectNotFront("도구점 주인", 20, 10, 50, 1000);

                    ntry = nval == 4 ? 4 : 4;

                    for (int k = 0; k < ntry; k++)
                    {
                        if (!MacroRunning)
                            break;

                        bool bochung = k == 3 ? true : false;

                        Maka_yori("추뇨");
                        Maka_sell("도구점", bochung);
                    }
                }

                if (!MacroRunning)
                    break;

                Maka_goto("교역소");
                ntry = 4;
                for (int k = 0; k < ntry; k++)
                {
                    if (!MacroRunning)
                        break;

                    Maka_yori("알파카");
                }

                Maka_sell("교역소", true);

                //
                loopCount++;
                Thread.Sleep(1000);
            }

            if (!MacroRunning)
                Console.WriteLine(">>>> 매크로 중단! <<<<");
            else
            {
                Console.WriteLine(">>>> 매크로 종료! <<<<");
                MacroRunning = false;
            }
        }
        void Maka_study()
        {
            TabAndDetectNotFront("학자", 20, 10, 30, 1000);
            if (!DetectTarget("학자 연구", 200, 10, 20))
                return;

            LeftClick("학자 연구", null, 1000);
            LeftClick("연구동 4번", null, 1000);
            LeftClick("버튼 다음", null, 1000);
            LeftClick("전공 1x1", null, 1000);
            LeftClick("버튼 다음", null, 1000);
            LeftClick("버튼 확인", null, 1000);
        }
        void Maka_goto(string place)
        {
            if (place == "은행")
            {
                RightClick("맵 은행", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 은행", null, 3000);
            }
            else if (place == "교역소")
            {
                RightClick("맵 교역소", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 교역소", null, 3000);
            }
            else if (place == "도구점")
            {
                RightClick("맵 도구점", 1000);

                LeftClick("도시맵", null, 1000);
                LeftClick("맵 도구점", null, 3000);
            }

            RightClick("맵 교역소", 1000);
            RightClick("맵 교역소", 1000);
        }
        bool Maka_buy(int nBalju)
        {
            TabAndDetectNotFront("교역소 주인", 20, 10, 20, 1000);
            if (!DetectTarget("교역소 구입", 200, 10, 20))
                return false;

            LeftClick("교역소 구입", null, 1000);

            /* 발주서 사용 */
            LeftClick("아이템 사용", null, 1000);
            LeftClick("아이템 확인", null, 1000);
            KeysClick($"{nBalju}", 10, 1000);
            LeftClick("아이템 OK", null, 1000);

            LeftClick("진열품 3번", "ctrl멍", 2000);//감자
            MouseOffset(300, 0, 1000);
            LeftClick("진열품 4번", "ctrl멍", 2000);//옥수수

            ScrollDown("스크롤바 상단", "스크롤바 하단", 500, 1000);

            LeftClick("진열품 4번", "ctrl멍", 2000);//알파카

            LeftClick("구입 확인", null, 1000);

            return true;
        }
        void Maka_yori(string tem)
        {
            if (tem == "추뇨")
            {
                int[] recepiNums = new int[] { 2, 3 };

                foreach (var N in recepiNums)
                {
                    if (DetectTarget("퀵슬롯 우버튼", 200, 10, 20))
                    {
                        LeftClick("퀵슬롯 우버튼", null, 1000);
                        MouseOffset(-300, 0, 1000);
                    }

                    FoodSlotNum = FoodSlotNum == 3 ? 4 : 3;
                    ChargeEnergyBar("행동력바", FoodSlotNum, 2000);

                    //LeftClick("퀵슬롯 6번", null, 2000);
                    KeysClick("6", 10, 2000);
                    LeftClick($"레시피 {N}번", "double", 1000);
                    LeftClick("횟수 지정", null, 1000);
                    LeftClick("횟수 Max", null, 1000);
                    LeftClick("횟수 OK", null, 1000);

                    LeftClick("생산종료", null, 1000);
                }
            }
            else if (tem == "알파카")
            {
                if (DetectTarget("퀵슬롯 우버튼", 200, 10, 20))
                {
                    LeftClick("퀵슬롯 우버튼", null, 1000);
                    MouseOffset(-300, 0, 1000);
                }

                FoodSlotNum = FoodSlotNum == 3 ? 4 : 3;
                ChargeEnergyBar("행동력바", FoodSlotNum, 2000);

                KeysClick("6", 10, 2000);
                //LeftClick("퀵슬롯 6번", null, 2000);
                LeftClick($"레시피 1번", "double", 1000);
                LeftClick("횟수 지정", null, 1000);
                LeftClick("횟수 Max", null, 1000);
                LeftClick("횟수 OK", null, 1000);

                LeftClick("생산종료", null, 1000);
            }
        }
        void Maka_sell(string place, bool bochungTime)
        {
            if (place == "도구점")
            {
                if (bochungTime)
                {
                    LeftClick("상단 캐릭터", null, 1000);
                    LeftClick("소유물품 목록", null, 2000);
                    LeftClick("보관함", null, 1000);
                    LeftClick("보관함 보충", null, 1000);
                    LeftClick("보관함 보충 확인", null, 1000);
                }

                TabAndDetectNotFront("도구점 주인", 20, 10, 50, 1000);
                if (!DetectTarget("도구점 매각", 200, 10, 20))
                    return;

                LeftClick("도구점 매각", null, 1000);

                //kvec shift = new kvec(56, 0);//추뇨, 초클로 위치가 서로 바뀔 수 있다.

                bool cukiSold = false;

                if (DetectTarget("초클로", 200, 10, 30))
                {
                    LeftClick("초클로", "ctrl멍", 2000);
                    cukiSold = true;
                }
                if (DetectTarget("추뇨", 200, 10, 30))
                {
                    LeftClick("추뇨", "ctrl멍", 2000);
                    cukiSold = true;
                }

                if (cukiSold)
                {
                    LeftClick("도구점 매각 확인", null, 1000);
                }
                else //하나도 안팔았으면, 오른쪽 클릭으로 빠져나간다.
                {
                    RightClick("맵 도구점", 1000);
                    RightClick("맵 도구점", 1000);
                }
            }
            else if (place == "교역소")
            {
                TabAndDetectNotFront("교역소 주인", 20, 10, 30, 1000);
                if (!DetectTarget("교역소 매각", 200, 10, 20))
                    return;

                LeftClick("교역소 매각", null, 1000);

                LeftClick("전부매각", null, 1000);

                //최종 매각
                LeftClick("교역소 매각 확인", null, 1000);
            }
        }
    }
}
