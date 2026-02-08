using shine.libs.math;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bacro
{
    public partial class Sail : Player
    {
        void TrustBuilding()
        {
            kvec offset = main.capDho.RectDho.pos();
            var timeStart = KSys.CurrentMillis();

            int MinutesOnWater = 30;
            string CityName = "사그레스";

            var loopcount = 0;
            var LoopTarget = main.gInput.getLoopTarget();

            MacroRunning = true;

            while (++loopcount <= LoopTarget && MacroRunning)
            {
                Console.WriteLine($"♥신뢰도 쌓기♥ # {loopcount}/{LoopTarget}");

                OutOfPort();
                Thread.Sleep(1000);

                //회전
                KeyClickLong("control a", 3000, 1000);
                //TabAndDetect(CityName, 50, 10, 30);
                //FillPowerGage("행동력바", 4, 2000);

                var mins = 0;
                while (++mins <= MinutesOnWater)
                {
                    Thread.Sleep(60 * 1000);

                    Console.Write("■");
                    if (mins % 10 == 0)
                        Console.WriteLine();
                }

                //게임화면 활성화. 키클릭 가능하게
                //(마우스 오른쪽을 쓸 수 없기 때문에 이렇게 한다.)
                LeftClick("상단 캐릭터", null, 1000);
                LeftClick("상단 캐릭터", null, 1000);

                macro.MouseMove(new kvec(400, 300), offset);
                Thread.Sleep(1000);

                /* 탭 접촉 */
                while (!TabAndDetectNotFront(CityName, 50, 10, 30, 100))
                    Thread.Sleep(1000);

                /* 이름만 선택되고, 접촉안된 상태  */
                while (!DetectTarget("항구 앞", 200, 10, 40))
                    macro.KeysClick(null, "space", 200, 500);

                Thread.Sleep(1000);
            }

            var ctime = KSys.ConsumedTime(timeStart);
            Console.WriteLine($"  ▶▶ 총 {ctime}");
            Console.WriteLine($"▶▶▶ 매크로 완료 !!");

            if (MacroRunning)
                main.VoiceSpeak("신뢰의 시간이 모두 끝났습니다. 매크로를 종료합니다.");
            else
                main.VoiceSpeak("매크로가 도중에 중단되었습니다. 매크로가 중단되었습니다.");

            MacroRunning = false;
        }
        void ReportQuests()
        {
            main.CapViewActivate();

            LeftClick("항구 앞", null, 1000);
            Thread.Sleep(3000);

            while (!DetectTarget("도시 인식", 100, 10, 30))
                Thread.Sleep(1000);

            MentoToggle(true);

            roadMove("항구앞", "모험가교관");

            TabAndDetectFront("모험가 교관", 50, 10, 30, 1000);
            Thread.Sleep(1000);
            for (int k = 0; k < 5; k++)
                macro.KeysClick(null, "space", 200, 500);

            RightClick(BPoss["말건네기 창"], 1000);
            TabAndDetectFront("모험가 교관", 50, 10, 30, 1000);
            Thread.Sleep(1000);
            for (int k = 0; k < 5; k++)
                macro.KeysClick(null, "space", 200, 500);

        }
        void OutOfSagress()
        {
            MentoToggle(false);

            //
            RightClick(CenFront, 1000);
            LeftClick("도시맵", null, 1000);
            LeftClick("맵 항구 사그레스", null, 3 * 1000 + 1000);

            if (TabAndDetectNotFront("항구안내원", 50, 10, 30, 1000))
            {
                while (!DetectTarget("항구로 이동", 100, 10, 30))
                    Thread.Sleep(1000);

                LeftClick("항구로 이동", null, 1000);
            }
        }
        void TakeQuests()
        {
            roadMove("상인교관", "모험가교관");

            bool gotMalagaQuest = false;
            bool gotEdinburghQuest = false;

            int nContact = 0;

            while (true)
            {
                RightClick(BPoss["말건네기 창"], 1000);
                if (!TabAndDetectFront("모험가 교관", 50, 10, 30, 1000))
                    Thread.Sleep(1000);

                while (!DetectTarget("의뢰 퀘스트", 100, 10, 30))
                    macro.KeysClick(null, "space", 200, 1000);

                LeftClick("의뢰 퀘스트", null, 1000);

                nContact++;
                TakeQuests_core(ref gotMalagaQuest, ref gotEdinburghQuest, nContact);

                /* 퀘 2개 다 받았다. */
                if (gotMalagaQuest && gotEdinburghQuest)
                    break;
            }// while (true)
        }
        void TakeQuests_core(ref bool gotMalagaQuest, ref bool gotEdinburghQuest, int nContact)
        {
            bool gotQuest = false;
            bool isFirst = true;

            while (!gotQuest)
            {
                if (nContact == 1 && isFirst)
                {/*처음에만 쓰지 않는다.*/}
                else
                {
                    LeftClick("알선서 사용", null, 1000);
                    LeftClick("알선서 진행 예", null, 1000);
                    Thread.Sleep(2000);
                }

                if (isFirst) isFirst = false;

                for (int k = 1; k <= 8; k++)
                {
                    // 키 다운
                    if (k == 1) { }
                    else if (2 <= k && k <= 5)
                        KeysClick("▼", 100, 500);
                    else if (7 <= k && k <= 8)
                        KeysClick("▼", 100, 500);
                    else if (k == 6)
                    {
                        KeysClick("▼", 100, 500);
                        KeysClick("▶", 100, 500);
                    }

                    // 의뢰 내용 확인
                    if (!gotMalagaQuest)
                    {
                        if (DetectTarget("의뢰 중급", 100, 3, 30))
                            if (DetectTarget("의뢰 인식", 100, 3, 30))
                            {
                                LeftClick("의뢰 확인", null, 1000);

                                if (DetectTarget("의뢰 스킬 부족", 100, 10, 30))
                                    LeftClick("의뢰 스킬 부족", null, 1000);

                                /*일단 퀘를 하나 받으면 다시 교관을 접촉해야한다.*/
                                gotMalagaQuest = true;
                                gotQuest = true;
                                break;//for
                            }
                    }

                    if (!gotEdinburghQuest)
                    {
                        if (DetectTarget("의뢰 상급", 100, 3, 30))
                            if (DetectTarget("의뢰 생태조사", 100, 3, 30))
                            {
                                LeftClick("의뢰 확인", null, 1000);

                                if (DetectTarget("의뢰 스킬 부족", 100, 10, 30))
                                    LeftClick("의뢰 스킬 부족", null, 1000);

                                /*일단 퀘를 하나 받으면 다시 교관을 접촉해야한다.*/
                                gotEdinburghQuest = true;
                                gotQuest = true;
                                break;//for
                            }
                    }

                    Thread.Sleep(200);
                }//for
            }//while (!gotQuest)
        }
        void HoneyQuest()
        {
            roadMove("모험가교관", "상인교관");

            TabAndDetectFront("상인 교관", 50, 10, 30, 1000);

            while (!DetectTarget("의뢰 퀘스트", 100, 10, 30))
                Thread.Sleep(1000);
            LeftClick("의뢰 퀘스트", null, 1000);

            bool gotHoneyQuest = false;
            bool isFirst = true;

            while (!gotHoneyQuest)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    LeftClick("알선서 사용", null, 1000);
                    LeftClick("알선서 진행 예", null, 1000);
                    Thread.Sleep(2000);
                }

                for (int k = 1; k <= 8; k++)
                {
                    // 키 다운
                    if (k == 1)
                    {
                    }
                    else if (2 <= k && k <= 5)
                        KeysClick("▼", 100, 500);
                    else if (7 <= k && k <= 8)
                        KeysClick("▼", 100, 500);
                    else if (k == 6)
                    {
                        KeysClick("▼", 100, 500);
                        KeysClick("▶", 100, 500);
                    }

                    // 의뢰 내용 확인
                    if (DetectTarget("의뢰 꿀배송", 100, 3, 30))
                    {
                        gotHoneyQuest = true;
                        LeftClick("의뢰 확인", null, 1000);
                        break;//for
                    }

                    Thread.Sleep(200);
                }//for (int k = 1; k <= 8; k++)

            }//while (!gotHoneyQuest)

            Thread.Sleep(1000);

            RightClick(BPoss["말건네기 창"], 1000);
            //
            TabAndDetectFront("상인 교관", 50, 10, 30, 1000);

            Thread.Sleep(1000);
            for (int k = 0; k < 5; k++)
            {
                macro.KeysClick(null, "space", 200, 500);
            }

            RightClick(BPoss["말건네기 창"], 1000);
            RightClick(BPoss["말건네기 창"], 1000);
            //
            TabAndDetectFront("상인 교관", 50, 10, 30, 1000);
            Thread.Sleep(1000);

            for (int k = 0; k < 5; k++)
            {
                macro.KeysClick(null, "space", 200, 500);
            }
        }
        void MeetMaster0()
        {
            LeftClick("상업지구", null, 1000);
            Thread.Sleep(3000);

            while (!DetectTarget("도시 인식", 100, 10, 30))
                Thread.Sleep(1000);

            CameraDefault("도시");

            LeftClick("도시맵", null, 1000);
            LeftClick("맵 모험가조합", null, 4 * 1000);

            RightClick(CenFront, 1000);
            RightClick(CenFront, 1000);
            TabAndDetectFront("모험가조합 입구", 50, 10, 30, 1000);

            Thread.Sleep(3000);

            kvec posMaster = new kvec(151, 56);
            kvec posExit = new kvec(412, 44);

            LeftClick(posMaster, null, null, 5000);


            TabAndDetectNotFront("모험가조합 마스터", 50, 10, 30, 1000);

            int nloop = 0;
            int nquesttem = 0;

            while (++nloop < 5)
            {
                if (DetectTarget("모험가조합 오케이", 100, 10, 30))
                {
                    LeftClick("모험가조합 오케이", null, 1000);
                    if (++nquesttem == 2)
                        break;
                }
                else if (DetectTarget("모험가조합 말걸기", 100, 10, 30))
                    LeftClick("모험가조합 말걸기", null, 1000);

                Thread.Sleep(500);
            }
        }
        void MeetMaster1()
        {
            kvec posMaster = new kvec(151, 56);
            kvec posExit = new kvec(255, 44);

            LeftClick("상단바", null, 1000);

            KeyClickLong("a", 3000, 1000);

            LeftClick(posExit, null, null, 5000);

            TabAndDetectFront("모험가조합 출구", 50, 10, 30, 1000);

            Thread.Sleep(3000);

            LeftClick("도시맵", null, 1000);
            LeftClick("맵 항구 상업", null, 3 * 1000 + 1000);

            TabAndDetectFront("항구안내원", 50, 10, 30, 1000);

            while (!DetectTarget("항구로 이동", 100, 10, 30))
                Thread.Sleep(1000);

            LeftClick("항구로 이동", null, 1000);
        }
        void MeetProfessor()
        {
            LeftClick("광장", null, 1000);
            Thread.Sleep(3000);

            while (!DetectTarget("도시 인식", 100, 10, 30))
                Thread.Sleep(1000);

            LeftClick("도시맵", null, 1000);
            LeftClick("맵 세비야 서고", null, 20 * 1000);
            LeftClick("맵 파르네제", null, 4 * 1000);
            LeftClick("맵 세비야 서고", null, 3 * 1000 + 1000);

            RightClick(CenFront, 1000);
            //RightClick(CenFront, 1000);
            TabAndDetectFront("서고 입구", 50, 10, 30, 1000);

            Thread.Sleep(3000);

            TabAndDetectNotFront("학자", 50, 10, 30, 1000);

            while (!DetectTarget("학자 연구", 200, 10, 20))
                Thread.Sleep(1000);

            LeftClick("학자 연구", null, 1000);
            Thread.Sleep(1000);
            LeftClick("연구동 2번", null, 1000);
            LeftClick("버튼 다음", null, 1000);
            LeftClick("전공 2x1", null, 1000);
            LeftClick("버튼 다음", null, 1000);
            LeftClick("버튼 확인", null, 1000);

            TabAndDetectNotFront("서고 출구", 50, 10, 30, 1000);

            Thread.Sleep(3000);

            LeftClick("도시맵", null, 1000);
            LeftClick("맵 항구 상업", null, 3 * 1000);

            TabAndDetectFront("항구안내원", 50, 10, 30, 1000);

            while (!DetectTarget("항구로 이동", 100, 10, 30))
                Thread.Sleep(1000);

            LeftClick("항구로 이동", null, 1000);
        }
        void MeetBattender0()
        {
            LeftClick("항구 앞", null, 1000);
            Thread.Sleep(3000);

            while (!DetectTarget("도시 인식", 100, 10, 30))
                Thread.Sleep(1000);

            LeftClick("도시맵", null, 1000);
            LeftClick("맵 주점 말라가", null, 3 * 1000 + 1000);

            TabAndDetectFront("주점 입구", 50, 10, 30, 1000);

            Thread.Sleep(3000);

            kvec posHost = new kvec(12, 86);
            kvec posExit = new kvec(412, 44);

            LeftClick(posHost, null, null, 5000);

            TabAndDetectNotFront("주점 주인", 50, 10, 30, 1000);

            int nloop = 0;
            int nquesttem = 0;

            while (++nloop < 5)
            {
                if (DetectTarget("주점 오케이", 100, 10, 30))
                {
                    LeftClick("주점 오케이", null, 1000);
                    if (++nquesttem == 1)
                        break;
                }
                else if (DetectTarget("주점 말걸기", 100, 10, 30))
                    LeftClick("주점 말걸기", null, 1000);

                Thread.Sleep(500);
            }
        }
        void MeetBattender1()
        {
            kvec posHost = new kvec(12, 86);
            kvec posExit = new kvec(255, 44);

            KeyClickLong("a", 3000, 1000);

            LeftClick(posExit, null, null, 5000);

            TabAndDetectFront("주점 출구", 50, 10, 30, 1000);

            Thread.Sleep(2000);

            LeftClick("도시맵", null, 1000);
            LeftClick("맵 항구 말라가", null, 3 * 1000 + 1000);

            TabAndDetectFront("항구관리", 50, 10, 30, 1000);

            while (!DetectTarget("항구로 이동", 100, 10, 30))
            {
                macro.KeysClick(null, "space", 200, 1000);
            }

            LeftClick("항구로 이동", null, 1000);
        }
        void MeetOfficer()
        {
            LeftClick("항구 앞", null, 1000);
            Thread.Sleep(3000);

            while (!DetectTarget("도시 인식", 100, 10, 30))
                Thread.Sleep(1000);

            //LeftClick("도시맵", null, 1000);
            //LeftClick("맵 항구 에딘버러", null, 2 * 1000 + 1000);

            TabAndDetectFront("항구관리", 50, 10, 30, 1000);

            Thread.Sleep(1000);
            int count = 0;
            while (!DetectTarget("항구 오케이", 100, 3, 30))
            {
                macro.KeysClick(null, "space", 200, 1000);

                if (++count > 10)
                    break;
            }

            if (DetectTarget("항구 오케이", 100, 3, 30))
                LeftClick("항구 오케이", null, 1000);

            RightClick(BPoss["말건네기 창"], 1000);
            RightClick(BPoss["말건네기 창"], 1000);

            TabAndDetectFront("항구관리", 50, 10, 30, 1000);

            while (!DetectTarget("항구로 이동", 100, 10, 30))
            {
                macro.KeysClick(null, "space", 200, 1000);
            }

            LeftClick("항구로 이동", null, 1000);
        }
        void QuestAndSailing(string city)
        {
            if (city == "에딘버러")
            {
                MeetOfficer();
                Thread.Sleep(1000);

                QuestEdinburgh();

                var file = $@"{main.navi.dirCoords}\에딘버러 사그레스.txt";

                main.navi.openCoords_core(file);
                Console.WriteLine($"좌표읽음 : {file}");

                main.InvokePaint();

                MacroRunning = true;
                SailingX(main.navi.coordsWorld, "사그레스");
                MacroRunning = false;
            }
            else if (city == "말라가")
            {
                MeetBattender0();
                Thread.Sleep(1000);

                MeetBattender1();
                Thread.Sleep(1000);

                QuestMalaga();

                var file = $@"{main.navi.dirCoords}\말라가 에딘버러.txt";

                main.navi.openCoords_core(file);
                Console.WriteLine($"좌표읽음 : {file}");

                main.InvokePaint();

                MacroRunning = true;
                SailingX(main.navi.coordsWorld, "에딘버러");
                MacroRunning = false;
            }
            else if (city == "사그레스")
            {
                ReportQuests();
                Thread.Sleep(1000);

                HoneyQuest();
                Thread.Sleep(1000);

                TakeQuests();
                Thread.Sleep(1000);

                OutOfSagress();
                Thread.Sleep(1000);

                var file = $@"{main.navi.dirCoords}\사그레스 세비야.txt";

                main.navi.openCoords_core(file);
                Console.WriteLine($"좌표읽음 : {file}");

                main.InvokePaint();

                MacroRunning = true;
                OutOfPort();
                SailingX(main.navi.coordsWorld, "세비야");
                MacroRunning = false;
            }
            else if (city == "세비야")
            {
                MeetMaster0();
                Thread.Sleep(1000);

                MeetMaster1();
                Thread.Sleep(1000);

                if (false)
                {
                    MeetProfessor();
                    Thread.Sleep(1000);
                }

                var file = $@"{main.navi.dirCoords}\세비야 말라가.txt";

                main.navi.openCoords_core(file);
                Console.WriteLine($"좌표읽음 : {file}");

                main.InvokePaint();

                MacroRunning = true;
                OutOfPort();
                SailingX(main.navi.coordsWorld, "말라가");
                MacroRunning = false;
            }
        }
        void QuestMalaga()
        {
            MacroRunning = true;

            var coord = kvec.Parse2d("16049 3290");

            OutOfPort();

            RightClick(CenFront, 1000);

            KeysClick("6", 100, 1000);//경계
            KeysClick("8", 100, 1000);//측량

            LeftClick(BPoss["돛 +1"], null, null, 500);
            LeftClick(BPoss["돛 +1"], null, null, 1000);

            int requestCount = 0;

            while (++requestCount < 10)
            {
                Bitmap bmpCompass = macro.requestImage(RectCompas);
                compas.FeedImagePas(bmpCompass);

                if (compas.SucceedScanPas)
                    break;

                Thread.Sleep(1000);
            }

            /* 나침반 스캔 실패 */
            if (!compas.SucceedScanPas)
            {
                MacroRunning = false;
                Console.WriteLine("항해 중단! 나침반 스캔 실패.");

                return;
            }

            Console.WriteLine($"각도 == {compas.DegShipHead:d03}");
            main.InvokePaint();

            /*다음 좌표 도달까지 지속되는 함수다.*/
            var coordArrived = false;
            var scanFailure = false;

            SailingUntilNextPost(coord, 4, ref coordArrived, ref scanFailure);

            LeftClick(BPoss["돛 접기"], null, null, 1000);
            MacroRunning = false;

            var offset = new kvec(0, -20);

            KeysClick("1", 100, 1000);//인식

            while (true)
            {
                if (DetectTarget("퀘스트 달성", 100, 5, 50))
                    break;
                if (DetectTarget("퀘스트 달성", 100, 5, 50, offset))
                    break;
                if (DetectTarget("퀘스트 달성", 100, 5, 50, 2 * offset))
                    break;

                Thread.Sleep(5000);
                KeysClick("1", 100, 1000);//인식
            }
        }
        void QuestEdinburgh()
        {
            MacroRunning = true;

            var coord = kvec.Parse2d("16237 2130");

            OutOfPort();

            RightClick(CenFront, 1000);

            KeysClick("6", 100, 1000);//경계
            KeysClick("8", 100, 1000);//측량

            LeftClick(BPoss["돛 +1"], null, null, 500);
            LeftClick(BPoss["돛 +1"], null, null, 1000);

            int requestCount = 0;

            while (++requestCount < 10)
            {
                Bitmap bmpCompass = macro.requestImage(RectCompas);
                compas.FeedImagePas(bmpCompass);

                if (compas.SucceedScanPas)
                    break;

                Thread.Sleep(1000);
            }

            /* 나침반 스캔 실패 */
            if (!compas.SucceedScanPas)
            {
                MacroRunning = false;
                Console.WriteLine("항해 중단! 나침반 스캔 실패.");

                return;
            }

            Console.WriteLine($"각도 == {compas.DegShipHead:d03}");
            main.InvokePaint();

            /*다음 좌표 도달까지 지속되는 함수다.*/
            var coordArrived = false;
            var scanFailure = false;
            SailingUntilNextPost(coord, 4, ref coordArrived, ref scanFailure);

            LeftClick(BPoss["돛 접기"], null, null, 1000);

            MacroRunning = false;

            var offset = new kvec(0, -20);

            KeysClick("2", 100, 1000);//생태조사

            while (!DetectTarget("퀘스트 달성", 100, 5, 50))
            {
                if (DetectTarget("퀘스트 달성", 100, 5, 50, offset))
                    break;

                KeysClick("2", 100, 1000);//생태조사
                Thread.Sleep(7 * 1000);
            }
        }
    }
}
