using shine.libs.math;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static shine.libs.konst.Com;

namespace Bacro
{
    public partial class Sail : Player
    {
        void setPics_Lima()
        {
            pics["리마 맵 학자"] = "441 206 16 15::1 2,2 8,7 8,10 8,14 6::225 222 180,179 166 131,83 86 92,230 234 239,184 181 124";
            pics["리마 맵 항구"] = "378 272 14 15::1 6,5 11,8 7,12 6::255 252 153,255 255 127,1 1 1,255 229 78";
            pics["따라가기 바다"] = "639 596 23 23::7 5,8 16,14 12,14 10::255 255 255,123 224 236,234 211 148,37 37 37";
            pics["두캇 아이콘"] = "21 55 15 18::2 9,5 3,10 9::255 255 255,255 250 227,255 255 255";
        }
        void playLima(string command)
        {
            setPics_Lima();

            switch (command)
            {
                case "연습":
                    //playLima_studyAll();
                    break;
                case "연구LR":
                    playLima_Nonmun();
                    break;
                case "연구신청":
                    playLima_LimaJob();
                    break;
                case "바다로":
                    playLima_outOfLima();
                    break;
                case "루프1":
                    playLima_Loop(1);
                    break;
                case "루프N":
                    //playLima_Loop(10);
                    playLima_Loop(20);
                    break;
            }

            MacroRunning = false;
            Console.WriteLine($"종료 : playLima({command})");
        }
        void playLima_Loop(int loopCount)
        {
            setPics_Lima();

            var timeStart = KSys.CurrentMillis();
            string timeSpent;

            Console.WriteLine($"시작 : playLima_Loop({loopCount})");
            var count = 0;
            while (++count <= loopCount && MacroRunning)
            {
                var time0 = KSys.CurrentMillis();

                Thread.Sleep(1000);
                Console.WriteLine($"연구 : ({count} / {loopCount})");
                playLima_LimaJob();
                if (!MacroRunning)
                    break;

                Thread.Sleep(1000);
                Console.WriteLine($"출항 : ({count} / {loopCount})");
                playLima_outOfLima();
                if (!MacroRunning)
                    break;

                Thread.Sleep(1000);
                Console.WriteLine($"항해 : ({count} / {loopCount})");

                /* 항해 */
                SailingX(main.navi.coordsWorld, "리마");

                timeSpent = KSys.ConsumedTime(time0);
                Console.WriteLine($"  ▶▶ {timeSpent} (루프 타임)");
            }

            MacroRunning = false;
            Console.WriteLine($"종료 : playLima_Loop({loopCount})");

            timeSpent = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ {timeSpent} (총 걸린 시간)");
        }
        void setWorkDho(string LR, int sleepEnd)
        {
            main.SetWorkDho(LR);
            main.capDho.FocusWorkDho();
            LeftClick(PosBar, sleepEnd);
            LeftClick(PosBar, sleepEnd);
        }
        void playLima_outOfLima()
        {
            /* 왼쪽 */
            setWorkDho("LEFT", 500);
            OutOfPortLima();

            Thread.Sleep(1000);

            /* 오른쪽 */
            setWorkDho("RIGHT", 500);
            OutOfPortLima();

            /* 따라가기 */
            if (!DetectTarget("따라가기 바다", 200, 20, 30))
                return;
            LeftClick("따라가기 바다", null, 1000);

            /* 왼쪽 */
            setWorkDho("LEFT", 500);

            /* 퀵슬롯 열기 */
            if (!DetectTarget("퀵슬롯 우버튼", 200, 30, 30))
                return;
            LeftClick("퀵슬롯 우버튼", null, 500);
            MouseOffset(-200, 200, 500);

            /* 경계, 측량 */
            KeysClick("2", 0, 1000);
            KeysClick("8", 0, 1000);
            /* 행음 */
            KeysClick("1", 0, 1000);
            KeysClick("7", 0, 1000);
            KeysClick("7", 0, 1000);
            KeysClick("4", 0, 1000);
        }
        void OutOfPortLima()
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
        void playLima_LimaJob()
        {
            setWorkDho("LEFT", 500);
            playLima_Nonmun();

            Thread.Sleep(1000);
            playLima_IntoPort();

            Thread.Sleep(1000);

            setWorkDho("RIGHT", 500);
            playLima_Nonmun();

            Thread.Sleep(1000);
            playLima_IntoPort();
        }
        bool playLima_Nonmun()
        {
            if (!MacroRunning)
                return false;

            if (!DetectTarget("항구 앞", 300, 10, 30))
                return false;
            LeftClick("항구 앞", null, 3000);

            if (!DetectTarget("두캇 아이콘", 300, 10, 30))
                return false;

            runOnMapLima("학자", 4);

            if (!TabAndDetectNotFront("학자", 50, 10, 30, 1000))
                return false;

            if (DetectTarget("학자x연구", 200, 10, 20))
                LeftClick("학자x연구", null, 1000);
            else if (DetectTarget("학자 연구", 200, 10, 20))
                LeftClick("학자 연구", null, 1000);
            //else if (DetectTarget("학자x연구", 200, 10, 20))
            //    LeftClick("학자x연구", null, 1000);
            else
                return false;

            if (!DetectTarget("연구동 2번", 200, 10, 20))
                return false;
            LeftClick("연구동 2번", null, 1000);

            if (!DetectTarget("버튼 다음", 200, 10, 20))
                return false;
            LeftClick("버튼 다음", null, 500);
            
            krect rectHaenan = parsePicRect(pics["전공 1x1"]);

            var LR = main.GetWorkDho();
            if (LR == "LEFT")
                rectHaenan.offset(0, 56 * 1);//아랫줄
            else if (LR == "RIGHT")
                //rectHaenan.offset(56 * 4, 0);//오른쪽
                rectHaenan.offset(56 * 1, 0);//오른쪽
                //rectHaenan.offset(0, 56 * 1);//아랫줄

            if (!DetectTarget2(rectHaenan, "전공 해난사", 200, 10, 20))
                return false;
            LeftClick(rectHaenan.Cen, null, null, 1000);
            
            if (!DetectTarget("버튼 다음", 200, 10, 20))
                return false;
            LeftClick("버튼 다음", null, 500);

            MouseOffset(0, 30, 500);

            if (!DetectTarget("버튼 확인", 200, 10, 20))
                return false;
            LeftClick("버튼 확인", null, 1000);

            return true;
        }
        bool playLima_IntoPort()
        {
            if (!MacroRunning)
                return false; 
            
            runOnMapLima("항구", 3);

            if (!DetectTarget("두캇 아이콘", 300, 10, 30))
                return false;

            if (!TabAndDetectNotFront("항구관리", 50, 10, 30, 1000))
                return false;

            if (!DetectTarget("항구로 이동", 200, 10, 30))
                return false;
            LeftClick("항구로 이동", null, 3000);

            if (!DetectTarget("두캇 아이콘", 300, 10, 30))
                return false;

            //Console.WriteLine("항구에 들어옴.");
            return true;
        }
        void runOnMapLima(string place, int walkingSec)
        {
            if (!MacroRunning) return;

            if (place == "학자")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("리마 맵 학자", null, 1000 * walkingSec);
            }
            else if (place == "항구")
            {
                LeftClick("도시맵", null, 1000);
                LeftClick("리마 맵 항구", null, 1000 * walkingSec);
            }

            RightClick("맵 센터", 1000);
            RightClick("맵 센터", 1000);
        }
    }
}
