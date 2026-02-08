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
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Bacro
{
    public partial class Sail
    {
        void CheckOutOfBaseZone()
        {
            /*안해주면 에러발생*/
            main.CapViewActivate();
            /* 좌표 읽기 위해 이름창 제거 */
            ClearNameBox(1000);

            var coordCurr = new kvec();
            if (!ScanCoordCurr(ref coordCurr))
            {
                /* 측량 켜기 시도 */
                QuickSlots_Open(500);
                QuickSlot_Heat(8, 1000);
            }
            if (!ScanCoordCurr(ref coordCurr))
            {
                Console.WriteLine("실패!! 현재 좌표 읽기");
                SailFold(1000);
                SailFold(1000);
                return;
            }

            /* 벗어났다 */
            if ((coordCurr - CoordBase).length() > (CoordBarier - CoordBase).length())
            {
                Thread.Sleep(500);
                sailToBaseZone(CoordBase);
            }
        }
        void FirstToBaseZone()
        {
            /*안해주면 에러발생*/
            main.CapViewActivate();
            /* 좌표 읽기 위해 이름창 제거 */
            ClearNameBox(1000);

            var coordCurr = new kvec();
            if (!ScanCoordCurr(ref coordCurr))
            {
                /* 측량 켜기 시도 */
                QuickSlots_Open(500);
                QuickSlot_Heat(8, 1000);
            }
            if (!ScanCoordCurr(ref coordCurr))
            {
                Console.WriteLine("실패!! 현재 좌표 읽기");
                SailFold(1000);
                SailFold(1000);
                return;
            }

            Thread.Sleep(500);
     
            sailToBaseZone(CoordBase);
        }
        void sailToBaseZone(kvec coordBase)
        {
            //안해주면 에러발생
            main.CapViewActivate();
            UpdateCompasImage();

            Coords = new List<kvec>();
            Coords.Add(coordBase);

            QuickSlots_Open(500);
            QuickSlot_Heat(2, 1000);//경계
            
            SailSpreadFull(500);
            SailSpreadFull(1000);

            Console.WriteLine($"목표 좌표 : {coordBase.toString2()}");

            /* 베이스 좌표로 이동 */
            var coordArrived = false;
            var coordArrived2 = false;
            var scanFailure = false;

            SailingUntilNextPostB(coordBase, 12, ref coordArrived, ref scanFailure);

            if (coordArrived)
            {
                Console.WriteLine($"감속..");

                SailFold(1000);
                SailSpread(1, 1000);

                SailingUntilNextPostB(coordBase, 4, ref coordArrived2, ref scanFailure);
            }

            if (coordArrived2)
            {
                Console.WriteLine($"좌표 도달 : {coordBase.toString2()}");
            }

            SailFold(1000);
        }
        void BackToLondon()
        {
            //안해주면 에러발생
            main.CapViewActivate();

            QuickSlots_Open(500);
            QuickSlot_Heat(2, 1000);//경계
            QuickSlot_Heat(8, 1000);

            SailSpreadFull(1000);
            SailSpreadFull(500);

            Console.WriteLine($"목표 좌표 : {CoordBase.toString2()}");

            /* 베이스 좌표로 이동 */
            var coordArrived = false;
            var coordArrived2 = false;
            var coordArrived3 = false;
            var scanFailure = false;

            SailingUntilNextPost(CoordBase, 12, ref coordArrived, ref scanFailure);

            if (coordArrived)
            {
                Console.WriteLine($"감속..");

                SailFold(1000);
                SailSpread(2, 1000);
                SailingUntilNextPost(CoordBase, 4, ref coordArrived2, ref scanFailure);
            }

            if (coordArrived2)
            {
                Console.WriteLine($"좌표 도달 : {CoordBase.toString2()}");
                
                SailFold(1000);
                SailSpread(1, 1000);
                SailingUntilNextPost(CoordLondon, 2, ref coordArrived3, ref scanFailure);
            }

            if (coordArrived3)
            {
                Console.WriteLine($"좌표 도달 : {CoordLondon.toString2()}");
       
                SailFold(1000);
                MacroRunning = false;

                var nameGate = "런던";
                var cityFound = false;
                var count = 0;

                /* 일단 도시이름 박스를 띄운다 */
                while (!cityFound && ++count <= 20)
                {
                    /*스페이스 타격이 디폴트로 들어있다*/
                    cityFound = TabAndDetectSeaNotFront(nameGate, 40, 10, 30, 1000);
                    if (cityFound) break;

                    EyeRotate("a", 200);
                }

                /* 도시박스 떴다 */
                if (cityFound)
                {
                    count = 0;
                    
                    /* 도시박스가 안보일 때까지 */
                    while (++count <= 10 && DetectTarget(nameGate, 100, 10, 40))
                    {
                        /* 스페이스 입장 */
                        macro.KeyClick(null, "space", 1000);

                        //LeftClick(nameGate, null, 2000);
                    }
                }
            }
        }
        void OutOfPort4Battle()
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

            QuickSlots_Open(500);
            QuickSlot_Heat(1, 1000);//감시
            QuickSlot_Heat(2, 1000);//경계
            QuickSlot_Heat(8, 1000);//측량
        }
        public void SailFold(int sleepEnd)
        {
            LeftClick(BPoss["돛 접기"], null, null, sleepEnd);
        }
        public void SailSpread(int n, int sleepEnd)
        {
            if (n == 1)
                LeftClick(BPoss["돛 +1"], null, null, sleepEnd);
            else if (n == 2)
            {
                LeftClick(BPoss["돛 +1"], null, null, 500);
                LeftClick(BPoss["돛 +1"], null, null, sleepEnd);
            }
        }
        public void SailSpreadFull(int sleepEnd)
        {
            LeftClick(BPoss["돛 펴기"], null, null, sleepEnd);
        }
        public void ShowFront(int sleepEnd)
        {
            kvec posClick = CenFront.copy();
            posClick.offset(200, 200);

            RightClick(posClick, sleepEnd);
        }
    }
}
