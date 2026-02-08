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
        readonly kvec CenFront = new kvec(405, 225);
        kvec vecMax = new kvec(200, 431) - new kvec(107, 330);
        kvec pMulGun1, pMulGun2, pMulGun3, pMulGun4;
        kvec pScrollUp, pScrollDown;
        int nLoopCount = 5;
        public PlayFactory(Bacro main) : base(main)
        {
            FoodSlotNum = 4;
            EnergyFull = 630;
            EnergyOfOnePizza = 50;

            pMulGun1 = parsePicCen(pics["진열품 1번"]);
            pMulGun2 = parsePicCen(pics["진열품 2번"]);
            pMulGun3 = parsePicCen(pics["진열품 3번"]);
            pMulGun4 = parsePicCen(pics["진열품 4번"]);
            pScrollUp = parsePicCen(pics["스크롤바 상단"]);
            pScrollDown = parsePicCen(pics["스크롤바 하단"]);
        }
        public override void PlayMacro(string cate, string command)
        {
            WindowDhoActivate(1000);

            if (cate == "햄생산페낭")
            {
                switch (command)
                {
                    case "생산":
                        playmacro_Pernam(10 * 5);// 발주서 30개 * ?
                        break;
                    case "햄한번만":
                        playmacro_Pernam(1);// 발주서 3
                        break;
                    case "교역소로":
                    case "은행으로":
                    case "연구":

                        break;
                }
            }
            else if (cate == "햄생산트루히요")
            {
                set_Trujillo();
                MacroRunning = true;
                var invest = true;

                switch (command)
                {
                    case "생산":
                        //playmacro_Trujillo(60, true); // 연구 병행, 60*2=120장

                        //playmacro_Trujillo(5 * 8, true); // 
                        playmacro_Trujillo(3 * 20, true); // 발주서 3장씩 모두 20번
                        break;
                    case "햄다섯번":
                        playmacro_Trujillo(5, false);
                        break;
                    case "햄한번만":
                        playmacro_Trujillo(1, false);
                        break;
                    case "생산만한번":
                        playmacro_Trujillo_product();
                        break;
                    case "음식만들기":
                        product_foods_Trujillo(35, invest);
                        break;
                    case "교역소로":
                        RunOnMap("교역소", 4);
                        break;
                    case "은행으로":
                        RunOnMap("은행", 4);
                        break;
                    case "연구":
                        studyApply();
                        break;
                }

                MacroRunning = false;
            }
            else if (cate == "햄관리트루히요")
            {
                set_Trujillo();
                MacroRunning = true;
                var invest= true;

                switch (command)
                {
                    case "관리":
                        playmacro_Trujillo_G(5 * 7, true);
                        break;
                    case "관리다섯번":
                        playmacro_Trujillo_G(5 + 2, false);
                        break;
                    case "관리한번만":
                        playmacro_Trujillo_G(1, false);
                        break;
                    case "생산만한번":
                        playmacro_Trujillo_G_product();

                        break;
                    case "음식만들기":
                        product_foods_Trujillo(35, invest);
                        break;
                    case "팔고사기":
                        playmacro_Trujillo_G_SellAndBuy();

                        break;
                    case "도시밖으로":
                        playmacro_Trujillo_G_OutOfCity();

                        break;
                }

                MacroRunning = false;
            }
            else if (cate == "햄생산히혼")
            {
                MacroRunning = true;
                setXixon();

                switch (command)
                {
                    case "생산":
                        playmacro_Xixon(10);
                        break;
                    case "햄다섯번":
                        playmacro_Xixon(5);
                        break;
                    case "햄한번만":
                        playmacro_Xixon(1);// 발주서 1 장
                        break;
                    case "생산만한번":
                        playmacro_Xixon2();
                        break;
                    case "음식만들기":
                        //product_foods_Xixon();
                        break;
                    case "돼지구입":
                        buyPigsXixon(2);
                        break;
                }

                MacroRunning = false;
            }
            else if (cate == "소세지생산헤르데르")
            {
                set_Herder();
                MacroRunning = true;

                switch (command)
                {
                    case "생산":
                        playmacro_Herder(12, true);
                        break;
                    case "생산1":
                        playmacro_Herder(1, false);
                        break;
                    case "생산5":
                        playmacro_Herder(5, false);
                        break;
                    case "오직생산1":
                        playmacro_Herder_product();
                        break;
                    case "음식생산":
                        product_foods_Herder(50);
                        break;
                    case "양구입":
                        buySheepsHerder(1, true);
                        break;
                }

                MacroRunning = false;
            }
            else if (cate == "소세지생산에딘버러")
            {
                setEdinburgh();
                MacroRunning = true;

                switch (command)
                {
                    case "생산":
                        playmacro_Edinburgh(60, true);
                        break;
                    case "생산1":
                        playmacro_Edinburgh(1, false);
                        break;
                    case "생산5":
                        playmacro_Edinburgh(5, false);
                        break;
                    case "오직생산1":
                        products_Edinburgh();
                        break;
                    case "음식생산":
                        product_foods_TwoOne(50);
                        break;
                    case "양구입":
                        buyEdinburgh(1);
                        break;
                }

                MacroRunning = false;
            }
            else if (cate == "피자생산포르투")
            {
                MacroRunning = true;
                setPorto();

                switch (command)
                {
                    case "생산":
                        playmacro_Porto(30, true);
                        break;
                    case "생산1":
                        playmacro_Porto(1, false);
                        break;
                    case "생산5":
                        playmacro_Porto(5, false);
                        break;
                    case "오직생산1":
                        products_Porto();
                        break;
                    case "재료구입":
                        buyPorto(5);
                        break;
                    case "판매":
                        sell_pizza();
                        break;
                    case "밀가루준비":
                        productMilGaru();
                        break;
                    case "즉석제작판매":
                        productAndSellPizza();
                        break;
                    case "교역소로":
                        RunOnMap_Porto("교역소", 3);
                        break;
                    case "도구점으로":
                        RunOnMap_Porto("도구점", 3);
                        break;
                }

                MacroRunning = false;
            }
            else if (cate == "놋쇠생산뤼베크")
            {
                MacroRunning = true;
                setLuebeck();

                switch (command)
                {
                    case "생산":
                        playmacro_Luebeck(nLoopCount, true);
                        break;
                    case "생산1":
                        playmacro_Luebeck(1, false);
                        break;
                    case "생산5":
                        playmacro_Luebeck(5, false);
                        break;
                    case "오직생산1":
                        products_Luebeck();
                        break;
                    case "광석구입":
                        buyLuebeck(1);
                        break;
                    case "음식생산":
                        product_foods_TwoOne(nFruitConsumed);
                        break;
                }

                MacroRunning = false;
            }
            else if (cate == "학술햄생산")
            {
                switch (command)
                {
                    case "생산":
                        playmacro_hakham(8);
                        break;
                    case "햄한번만":
                        playmacro_hakham(1);// 발주서 3
                        break;
                    case "연습":

                        break;
                }
            }
            else if (cate == "소세지생산")
            {
                switch (command)
                {
                    case "생산":
                        playmacro_sausage(10);
                        break;
                    case "소세지한번만":
                        playmacro_sausage(1);// 발주서 3
                        break;
                    case "소세지연습":
                        playmacro_sausage_test();
                        break;
                }
            }
            else if (cate == "통조림생산")
            {
                switch (command)
                {
                    case "생산":
                        playmacro_foodCan(20);
                        break;
                    case "통조림한번만":
                        playmacro_foodCan(1); 
                        break;
                }
            }
         
        }
        bool UseBalju(int nBalju)
        {
            if (!DetectTarget("아이템 사용", 200, 10, 20))
                return false;
            LeftClick("아이템 사용", null, 1000);

            if (!DetectTarget("아이템 사용 확인", 200, 10, 20))
                return false;
            LeftClick("아이템 사용 확인", null, 1000);

            if (!DetectTarget("아이템 사용 Max", 200, 10, 20))
                return false;
            KeysClick($"%{nBalju}", 100, 1500);

            if (!DetectTarget("아이템 사용 OK", 200, 10, 20))
                return false;
            LeftClick("아이템 사용 OK", null, 1000);

            return true;
        }
        void product(string mulgun, int slotnum, int index, int repeat)
        {
            product_fx(mulgun, slotnum, index, repeat, null);
        }
        void product_fx(string mulgun, int slotnum, int index, int repeat, string temFx)
        {
            if (!MacroRunning) return;

            if (DetectTarget("퀵슬롯 우버튼", 200, 10, 20))
            {
                LeftClick("퀵슬롯 우버튼", null, 1000);
                MouseOffset(-300, 0, 1000);
            }

            ChargeEnergyBar2("행동력바", FoodSlotNum, 550, 1000);
            ChargeEnergyBar2("행동력바", FoodSlotNum, 500, 1000);

            if (temFx == "신비한 향신료")
            {
                if (!DetectTarget("신비한 향신료", 200, 10, 20))
                    return;
                KeysClick("5", 0, 1000);
            }
            else if (temFx == "강욕상인의 철쇄")
            {
                if (!DetectTarget("강욕상인의 철쇄", 200, 10, 20))
                    return;
                KeysClick("5", 0, 1000);
            }

            //마이 레시피
            if (slotnum == 0)
                KeysClick("control b", 10, 1000);
            //퀵슬롯 넘버
            else
                KeysClick($"{slotnum}", 10, 1000);

            var ndu = 0;
            string updownkey = "";
            while (++ndu <= Math.Abs(index))
                updownkey += index > 0 ? "▼" : "▲";

            if (updownkey != "")
                KeysClick($"%{updownkey}", 500, 1000);

            //KeysClick("return", 0, 1000);

            if (DetectTarget("버튼 확인 레시피", 200, 10, 20))
                LeftClick("버튼 확인 레시피", null, 1000);

            var rep = 0;
            while (++rep <= repeat)
            {
                LeftClick("횟수 지정", null, 1000);

                if (DetectTarget("횟수 Max", 200, 10, 20))
                {
                    LeftClick("횟수 Max", null, 1000);

                    if (DetectTarget("횟수 OK", 200, 10, 20))
                        LeftClick("횟수 OK", null, 1000);
                }
            }

            if (DetectTarget("생산종료", 200, 10, 20))
                LeftClick("생산종료", null, 1000);
        }

        void WindowDhoActivate(int sleepEnd)
        {
            LeftClick(PosBar, sleepEnd);
            LeftClick(PosBar, sleepEnd);
        }
    }
}
