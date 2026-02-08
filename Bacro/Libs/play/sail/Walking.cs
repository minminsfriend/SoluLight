using shine.libs.graphics;
using shine.libs.math;
using shine.libs.pad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bacro
{
    public partial class Sail
    {
        List<kvec> bankArea = kvec.ParseList("112 57,103 57,103 50,112 50");
        bool WalkingOnStreet = false;

        void roadMoveTest()
        {
            roadMove("항구앞", "모험가교관");

        }

        void roadMove(string placeA, string placeB)
        {
            List<kvec> poss = new List<kvec>();

            if (placeA == "항구앞" && placeB == "모험가교관")
            {
                poss.Add(roadpoints["행상인"].copy());
                poss.Add(roadpoints["상인교관"].copy());
                poss.Add(roadpoints["모험가교관"].copy());
            }
            else if (placeA == "모험가교관" && placeB == "상인교관")
            {
                poss.Add(roadpoints["상인교관"].copy());
            }
            else if (placeA == "상인교관" && placeB == "모험가교관")
            {
                poss.Add(roadpoints["모험가교관"].copy());
            }

            RightClick("맵 센터", 1000);

            /* 거리단위당 키 프레스 타임 (도시마다 다르다) */
            int PRESSMILSEC = 220;// == 포르투

            int degCurrMe = 0;
            int degToRotate = 0;
            float remToMove;

            foreach (var posNext in poss)
            {
                remToMove = 100;//초기값 필요

                /* 현재 나의 위치 방향 확인 */
                ScanCityMap(posNext, ref degCurrMe, ref degToRotate, ref remToMove);
                //dakro.cityMap.InvokePaint();

                while (remToMove > 3)
                    roadMoveForTarget(posNext, degToRotate, ref remToMove, PRESSMILSEC);

                Console.WriteLine($"[좌표도달] ({posNext.toString2()})  남은거리 {(int)remToMove}㎞");
                Thread.Sleep(100);
            }
        }
        void roadMoveForTarget(kvec posTarget, int degToRotate, ref float remToMove, int PRESSMILSEC)
        {
            int degCurrMe = 0;
            int timepress;

            /* 나아갈 방향 잡기 */
            int k = 0;
            while (Math.Abs(degToRotate) > 3 && ++k < 5)
            {
                /* 회전 */
                timepress = (int)Math.Abs(degToRotate * (500f / 30));
                var L_R = degToRotate < 0 ? "a" : "d";
                macro.KeyLongClick(null, L_R, timepress, 100);

                /* 현재 나의 위치 방향 확인 */
                ScanCityMap(posTarget, ref degCurrMe, ref degToRotate, ref remToMove);
                //dakro.cityMap.InvokePaint();

                /* 살짝 전진 */
                timepress = (int)(0.5f * timepress);
                macro.KeyLongClick(null, "w", timepress, 100);

                /* 현재 나의 위치 방향 확인 */
                ScanCityMap(posTarget, ref degCurrMe, ref degToRotate, ref remToMove);
                //dakro.cityMap.setHeadAndTail(gArrow.head, gArrow.tail);
                //dakro.cityMap.InvokePaint();
            }//--while

            /* 이젠 뛰어라. */
            if (remToMove > 0.5f)
            {
                /* 전진 */
                timepress = (int)(remToMove * PRESSMILSEC);
                macro.KeyLongClick(null, "w", timepress, 100);

                /* 현재 나의 위치 방향 확인 */
                ScanCityMap(posTarget, ref degCurrMe, ref degToRotate, ref remToMove);
                //dakro.cityMap.InvokePaint();
            }
        }
        void ScanCityMap(kvec posTarget, ref int degMeFront, ref int degToRotate, ref float remToMove)
        {
            krect rectMap = krect.Parse("622 487 180 140");
            kvec axis = new kvec(0, 1);

            degToRotate = 0;
            remToMove = 0f;

            Bitmap bmpCity= macro.requestImage(rectMap);
            gArrow.SetImage(bmpCity);

            main.capView.PrintImage(bmpCity);

            degMeFront = gArrow.DegBetween_Front_yAxis(axis);
            degToRotate = gArrow.Deg_ToRotate(posTarget);
            remToMove = (posTarget - gArrow.head).length();

            //콘솔에서 보기
            var datax = $"[시티맵현재] (正) {degMeFront:d03}º (角) {degToRotate:d02}º (里) {(int)remToMove:d02}㎞";
            Console.WriteLine(datax);
        }

    }
}