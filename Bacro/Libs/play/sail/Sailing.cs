using shine.libs.graphics;
using shine.libs.math;
using shine.libs.pad;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Bacro
{
    public partial class Sail
    {
        //16383 8191 max값 in dho online map
        public Compas compas;
        public krect RectWorld = new krect(16383, 8192);

        readonly kvec CenFront = new kvec(405, 225);
        readonly krect RectCoord = krect.Parse("731 356 67 9");//좌표 박스
        readonly krect RectCompas = krect.Parse("650 517 126 109");//나침반 박스
        
        /*public*/ int IndexPost = 0;
        List<kvec> Coords;
        bool ReduceSpeed50Harry, ReduceSpeed25Harry;
        long startTime = 0;
        long timeChecked = 0;
        long timeScanCompas = 0;
        int countCheckTime = 0;

        void CameraDefault(string area)
        {
            if (area == "도시")
            {
                KeyClickLong("pgdn", 2000, 500);
                KeyClickLong("control s", 3000, 500);
                KeyClickLong("control w", 1200, 500);
            }
            else if (area == "해상")
            {
                KeyClickLong("pgdn", 2000, 500);
                KeyClickLong("control s", 3000, 500);
                KeyClickLong("control w", 1000, 500);
            }
        }
        void MentoToggle(bool setmento)
        {
            LeftClick(BPoss["배 함대"], null, null, 1000);
            LeftClick(BPoss["함대 관리"], null, null, 1000);
            LeftClick(BPoss["함대대원 2x1"], null, null, 500);
            LeftClick(BPoss["함대대원 2x1"], null, null, 1000);

            if (setmento)
            {
                if (DetectTarget("멘토", 100, 10, 50))
                    LeftClick(BPoss["멘토 토글"], null, null, 1000);
                else
                    RightClick(CenFront, 1000);
            }
            else
            {
                if (!DetectTarget("멘토", 100, 10, 50))//'멘토 해제'가 보인다
                    LeftClick(BPoss["멘토 토글"], null, null, 1000);
                else
                    RightClick(CenFront, 1000);
            }
        }
        void SailingX(List<kvec> coords, string GoalCity)
        {
            this.Coords = coords;
            if (coords == null)
                return;

            main.CapViewActivate();

            IndexPost = 0;
            startTime = KSys.CurrentMillis(); ;
            timeChecked = startTime;
            countCheckTime = 0;

            /* 마지막 감속 2단계 */
            ReduceSpeed50Harry = false; 
            ReduceSpeed25Harry = false; 

            Console.WriteLine("항해가 다시 시작되다.");

            if (GoalCity != null)
                Console.WriteLine($"  우하하핫! <{GoalCity}> 를 향하여!");
            else
                Console.WriteLine($"  이히히힛! <이름모를 바다> 를 향하여!");

            KeysClick("2", 100, 1000);//경계
            KeysClick("8", 100, 1000);//측량

            SailSpreadFull(1000);
            SailSpreadFull(1000);

            /* 필요할까? */
            //RightClick(CenFront, 2000);
            if (!UpdateCompasImage())
                MacroRunning = false;
            timeScanCompas = startTime;

            while (MacroRunning && IndexPost < coords.Count)
            {
                var coordDest = coords[IndexPost];
                var PrintNum = IndexPost + 1;

                Console.WriteLine($"★★  다음 [{PrintNum}] ★★ ({coordDest.toString2()})");

                var DestArrived = false;
                var ScanFailure = false;
                var ZoneOfPostArrived = getZoneOfPostArrived(GoalCity, IndexPost, coords.Count);

                /* SailingUntilCoordNext : 좌표 구간동안 지속되는 함수 */
                SailingUntilNextPost(coordDest, ZoneOfPostArrived, ref DestArrived, ref ScanFailure);

                if (ScanFailure)
                {
                    MacroRunning = false;
                    break;
                }

                if (DestArrived)
                {
                    Console.WriteLine($"★★  도달 [{PrintNum}] ★★ ({coordDest.toString2()})");

                    /*next coord index*/
                    /* 마지막은 인덱스초과로 while 탈출 */
                    IndexPost++;
                }
            }

            /* 항해가 중단된 경우 */
            if (IndexPost < coords.Count - 1 || !MacroRunning)
            {
                MacroRunning = false;
                Console.WriteLine("▶▶▶ ! 항해가 도중에 중단됨.");
                return;
            }

            /* 입항 시도 */
            /* 돛을 완전히 접고, */
            SailFold(1000);
            SailFold(1000);

            if (GoalCity == null)
                Console.WriteLine($"### 목적지 <좌표(해상)> 에 도달 ###");
            else
            {
                Console.WriteLine($"### 목적지 <{GoalCity}> 에 입항 시도 ###");

                if (!GoalIntoCity(GoalCity))
                {
                    Console.WriteLine($"### 입항 실패!! ###");

                    if (setTempCoords(ref coords, GoalCity))
                    {
                        /* 입항 재도전 */
                        ClearNameBox(2000);
                        SailingX(coords, GoalCity);
                    }
                }
            }

            Console.WriteLine("▶▶▶ 항해가 종료되다...");
        }
        bool setTempCoords(ref List<kvec> coords, string goalCity)
        {
            if (goalCity == "리마")
            {
                Console.WriteLine("◀◀◀ 리마 이므로 다시 입항에 도전한다..");

                kvec posLima = kvec.Parse2d("12835 5258");
                kvec posWest = posLima.copy();
                /* 리마 서쪽, 200해리 로 이동 */
                posWest.offset(-200, 0);

                coords.Clear();
                coords.Add(posWest);
                coords.Add(posLima);

                return true;
            }

            return false;
        }
        int getZoneOfPostArrived(string goalCity, int indexPost, int coordsCount)
        {
            var ZoneOfPostArrived = indexPost == coordsCount - 1 ? 6 : 20;

            if (goalCity == "리마" && coordsCount == 2) // 해난사 모렙업 항로
                ZoneOfPostArrived = indexPost == coordsCount - 1 ? 7 : 40;

            return ZoneOfPostArrived;
        }
        bool UpdateCompasImage()
        {
            int MaxCount = 10;
            var count = 0;

            /* 10초간 스캔 */
            while (++count <= MaxCount)
            {
                Bitmap bmpCompas = macro.requestImage(RectCompas);
                compas.FeedImagePas(bmpCompas);

                main.capView.PrintImage(bmpCompas);

                if (compas.SucceedScanPas)
                {
                    Console.WriteLine("** UpdateCompasImage(); ");                    
                    return true;
                }

                Thread.Sleep(1000);
            }
            if (!compas.SucceedScanPas)
            {
                Console.WriteLine("** 나침반 스캔 실패!");
            }

            return false;
        }
        void SailingUntilNextPost(kvec coordDest, float radiusTargetZone, ref bool DestArrived, ref bool ScanFailure)
        {
            /* 나침반에 목표 좌표 입력 */
            compas.CoordDest = coordDest.copy();
            int degNeedlePre = compas.DegShipHead;

            /* 스캔 실패가 잦으면, 항해 중단 */
            int TotalCoordFailure = 0;
            int TotalCompasFailure = 0;

            while (MacroRunning)
            {
                /* 쥐잡기 등등 */
                var timeNow = KSys.CurrentMillis();
                if (!ReduceSpeed50Harry && timeNow - startTime > 180000)// 18만 밀리초 == 3분
                {
                    if (timeNow - timeChecked > 20 * 1000)//20초마다 확인
                    {
                        timeChecked = timeNow;
                        CatchMice(++countCheckTime);
                    }
                }

                //var ScanFailure = false;

                int Deg4Handle = CalcBoatCoord(radiusTargetZone, ref TotalCoordFailure, ref DestArrived, ref ScanFailure);
                if (DestArrived)
                {
                    break; // 종료 : SailingUntilNextPost
                }
                if (ScanFailure)
                {
                    MacroRunning = false;
                    break;
                }

                /* 회전없이 계속 직진, 0.5초 sleep */
                if (ReduceSpeed25Harry)
                {
                    Thread.Sleep(500);
                    continue;
                }
              
                timeNow = KSys.CurrentMillis();

                /* 회전없이 계속 직진 */
                if (Math.Abs(Deg4Handle) <= 2)
                {
                    if (timeNow - timeScanCompas < 10 * 1000)
                    {
                        /* 10초동안 그냥 직진, 0.5초 sleep */
                        Thread.Sleep(500);
                    }
                    else
                    {
                        /* 돌풍 상정 */
                        RightClick(CenFront, 1000);
                        UpdateCompasImage();
                        timeScanCompas = timeNow;
                    }
                }
                /* 회전 */
                else
                {
                    timeScanCompas = timeNow;

                    /* 배 운전대 */
                    RightClick(CenFront, 500);
                    BoatHandling(Deg4Handle);

                    /* 배 각도 계산 */
                    CalcBoatAngle(ref degNeedlePre, ref TotalCompasFailure, ref ScanFailure);
                    if (ScanFailure)
                    {
                        MacroRunning = false;
                        break;
                    }
                    else
                    {
                        /* 구간 항해 계속 */
                    }
                }
            }
        }
        void SailingUntilNextPostB(kvec coordDest, float radiusTargetZone, ref bool DestArrived, ref bool ScanFailure)
        {
            /* 나침반에 목표 좌표 입력 */
            compas.CoordDest = coordDest.copy();
            int degNeedlePre = compas.DegShipHead;

            /* 스캔 실패가 잦으면, 항해 중단 */
            int TotalCoordFailure = 0;
            int TotalCompasFailure = 0;

            while (MacroRunning)
            {
                /* 쥐잡기 등등 */
                var timeNow = KSys.CurrentMillis();
                if (!ReduceSpeed50Harry && timeNow - startTime > 180000)// 18만 밀리초 == 3분
                {
                    if (timeNow - timeChecked > 20 * 1000)//20초마다 확인
                    {
                        timeChecked = timeNow;
                        CatchMice(++countCheckTime);
                    }
                }

                int Deg4Handle = 0;
                int MaxCount = 10;

                /*좌표*/
                var count = 0;
                while (++count <= MaxCount)
                {
                    Bitmap bmpCoord = macro.requestImage(RectCoord);
                    compas.setImageCoord(bmpCoord);
                    if (compas.SucceedScanCoord)
                        break;//성공 완료

                    Thread.Sleep(1000);
                }

                /* 좌표 스캔 성공 */
                if (compas.SucceedScanCoord)
                {
                    /*남은 거리 계산 */
                    var lenToTarget = compas.LenToMove();
                    Console.WriteLine($"* 배틀 베이스존  ☞  앞으로 {lenToTarget:f01} 海里");

                    /* 서브루프 종료*/
                    if (lenToTarget < radiusTargetZone)
                        return;
                    /* 서브루프 지속, 회전할 각도 리턴 */
                    else
                        Deg4Handle = compas.DegToRotate();
                }
                else
                {
                    Thread.Sleep(500);
                    return;
                }

                /* 회전없이 계속 직진 */
                if (Math.Abs(Deg4Handle) <= 2)
                {
                    Thread.Sleep(500);
                }
                /* 회전 */
                else
                {
                    /* 배 운전대 */
                    RightClick(CenFront, 500);
                    BoatHandling(Deg4Handle);

                    /* 배 각도 계산 */
                    CalcBoatAngle(ref degNeedlePre, ref TotalCompasFailure, ref ScanFailure);
                    if (ScanFailure)
                    {
                        MacroRunning = false;
                    }
                    else
                    {
                        /* 구간 항해 계속 */
                    }
                }
            }
        }
        int CalcBoatCoord(float radiusTargetZone, ref int totalCoordFailure, ref bool destArrived, ref bool scanFailure)
        {
            int FailureLimit = 10;
            int ShotsMax = 10;
       
            /*좌표*/
            var count = 0;
            while (++count <= ShotsMax)
            {
                Bitmap bmpCoord = macro.requestImage(RectCoord);
                compas.setImageCoord(bmpCoord);
                if (compas.SucceedScanCoord)
                    break;//성공 완료

                Thread.Sleep(1000);
            }

            /* 좌표 스캔 실패 */
            if (!compas.SucceedScanCoord)
            {
                Console.WriteLine($"좌표 스캔 실패! #{++totalCoordFailure}번째.");

                if (totalCoordFailure > FailureLimit)
                {
                    Console.WriteLine("항해중단! <좌표> 스캔 실패...");
                    /* 항해 중단 */
                    scanFailure = true;
                }
            }
            /* 좌표 스캔 성공 */
            else
            {
                /*남은 거리 계산 */
                var lenToTarget = compas.LenToMove();

                /*현재 좌표 기록*/
                main.navi.SetShipGps(compas.getShipGps());

                var coordIndexx = $"[{IndexPost} ~ {IndexPost + 1}/{Coords.Count}]";
                Console.WriteLine($"{coordIndexx}  ☞  앞으로 {lenToTarget:f01} 海里");

                /* 종점 직전, 2단계 감속 */
                if (IndexPost == Coords.Count - 1)
                {
                    if (!ReduceSpeed50Harry && lenToTarget <= 50f)
                    {
                        ReduceSpeed50Harry = true;
                        Console.WriteLine("▶▶▶ 50 海里 감속 돛2칸");

                        SailFold(1000);
                        SailSpread(2, 1000);
                    }
                    if (!ReduceSpeed25Harry && lenToTarget <= 25f)
                    {
                        ReduceSpeed25Harry = true;
                        Console.WriteLine("▶▶ 25 海里 감속 돛1칸 ");

                        SailFold(1000);
                        SailSpread(1, 1000);
                    }
                }

                /* 서브루프 종료*/
                if (lenToTarget < radiusTargetZone)
                    destArrived = true;
                /* 서브루프 지속, 회전할 각도 리턴 */
                else
                    return compas.DegToRotate();
            }

            return 0;
        }
        void CalcBoatAngle(ref int degAheadPre, ref int totalCompasFailure, ref bool scanFailure)
        {
            var FailureLimit = 10;
            var MaxCount = 5;
       
            /*정면 유지*/
            RightClick(CenFront, 2000);

            /*회전후 나침반 스캔*/
            var count = 0;
            while (++count <= MaxCount)
            {
                Bitmap bmpCompass = macro.requestImage(RectCompas);
                compas.FeedImagePas(bmpCompass);

                main.capView.PrintImage(bmpCompass);

                if (compas.SucceedScanPas)
                    break;//성공 완료

                Thread.Sleep(1000);
            }

            /* 나침반 스캔 실패 */
            if (!compas.SucceedScanPas)
            {
                Console.WriteLine($"나침반 스캔 실패! #{++totalCompasFailure}번째.");
                /* n 번의 스캔 실패 허용 */
                if (totalCompasFailure > FailureLimit)
                {
                    Console.WriteLine("항해중단! <나침반> 스캔 실패...");
                    scanFailure = true;
                }
            }
            /* 나침반 스캔 성공 */
            else
            {
                var degAhead = compas.DegShipHead;
                var degAheadTar = compas.DegDest;
                var degRot = degAhead - degAheadPre;
                degAheadPre = degAhead;

                var ovar = degAhead - degAheadTar;
                var degRotX = degRot >= 0 ? $"↘↘ {degRot}º" : $"↙↙ {Math.Abs(degRot)}º";
                var ovarx = ovar >= 0 ? $"초과{ovar}º" : $"부족{Math.Abs(ovar)}º";
                if (ovar == 0) ovarx = "일치";

                /*각도 출력*/
                Console.WriteLine($"¶ 船回 {degRotX}  ☞  船首 {degAhead}º  ({ovarx})");

                //
                main.InvokePaint();
                Thread.Sleep(500);
            }
        }
        void CatchMice(int count)
        {
            switch (count % 3)
            {
                case 0:
                    KeysClick("%333333", 200, 1000);
                    Console.WriteLine("** 쥐잡는 날! **");
                    break;
                case 1:
                    KeysClick("%666666", 200, 1000);//치료
                    Console.WriteLine("** 약먹는 날! **");
                    break;
                case 2:
                    KeysClick("%555", 200, 1000);//소화
                    Console.WriteLine("** 불끄는 날! **");
                    break;
            }
        }
        bool GoalIntoCity(string goalName)
        {
            /* 미등록 항구 */
            if (goalName.Substring(0, 1) == "x")
            {
                var gName = goalName.Substring(1);
                Console.WriteLine($" 미등록 항구 : <{gName}>");
                return false;
            }

            /* f필드 : 이름이 다 상륙지점 */
            var picName = goalName.Substring(0, 1) == "f" ? "상륙지점" : goalName;
            var a_d = Array.IndexOf(new string[] { "런던", "리마" }, goalName) > -1 ? "d" : "a";

            var detectByTab = false;

            /* 회전하면서 도시이름을 띄운다 */
            var indexRotate = 0;
            while (++indexRotate <= 11 && MacroRunning)
            {
                /*스페이스 타격 : 디폴트로 들어있다*/
                detectByTab = TabAndDetectNotFront(picName, 10, 10, 30, 100);
                if (detectByTab)
                    break;

                EyeRotate(a_d, 200);
            }

            if (detectByTab)
            {
                Thread.Sleep(1000);
                Console.WriteLine($"☆☆  Tab 키로 [{picName}] 인식 !  ☆☆ ");

                if (picName == "상륙지점")
                {
                }
                else
                {
                    /* 항구로 들어온 상태 */
                    if (DetectTarget("항구 앞", 300, 10, 30))
                        Console.WriteLine("DetectTarget [항구 앞]");
                }

                return true;
            }

            return false;
        }
        void OutOfPort()
        {
            while (!DetectTarget("보급", 100, 10, 30))
                Thread.Sleep(1000);
            LeftClick("보급", null, 2000);

            LeftClick("기억 보급", null, 1000);
            LeftClick("보급 확인", null, 1000);

            while (!DetectTarget("출항", 200, 10, 30))
                RightClick("맵 센터", 1000);

            LeftClick("출항", null, 1000);
            LeftClick("출항 2", null, 4000);

            while (!DetectTarget("퀵슬롯 우버튼", 200, 10, 30))
                Thread.Sleep(1000);
            if (DetectTarget("퀵슬롯 우버튼", 200, 10, 30))
                LeftClick("퀵슬롯 우버튼", "멍", 2000);

            //MonitorMessages("화재 발생,쥐가 발생,갑판이", 5000, MinutesOnWater);
        }
        bool ScanCoordCurr(ref kvec shipGps)
        {
            int ShotsMax = 10;
       
            /*좌표*/
            var count = 0;
            while (++count <= ShotsMax)
            {
                Bitmap bmpCoord = macro.requestImage(RectCoord);
                compas.setImageCoord(bmpCoord);
                if (compas.SucceedScanCoord)
                    break;//성공 완료

                Thread.Sleep(1000);
            }

            /* 좌표 스캔 실패 */
            if (!compas.SucceedScanCoord)
            {
                Console.WriteLine($"좌표 스캔 실패!");
                return false;
            }
            /* 좌표 스캔 성공 */
            else
            {
                shipGps = compas.getShipGps();
            }

            return true;
        }
        void BoatHandling(int Deg4Handle)
        {
            int timeForDeg90 = 300;
            int degABS = Math.Abs(Deg4Handle);

            var L_R = Deg4Handle < 0 ? "a" : "d";
            var sleepPressed = (int)(timeForDeg90 * (degABS / 90f));

            var sleepEnd = 5000 + (degABS - 30) * 50;
            if (sleepEnd < 5000)
                sleepEnd = 5000;

            macro.KeyLongClick(null, L_R, sleepPressed, sleepEnd);
        }
    }
}