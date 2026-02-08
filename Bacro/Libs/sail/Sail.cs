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
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bacro
{
    public partial class Sail : Player
    {
        Dictionary<string, kvec> BPoss = new Dictionary<string, kvec>();
        Dictionary<string, string> shipPics = new Dictionary<string, string>();

        string fileships;
        public Sail(Bacro main) : base(main)
        {
            fileships = $@"{main.dirData}\pirate.txt";

            FoodSlotNum = 4;
            EnergyFull = 630;
            EnergyOfOnePizza = 50;

            LoadDatas();

            compas = new Compas();
            gArrow = new GreenArrow(bankArea);
        }
        void LoadDatas()
        {
            shipPics.Clear();
            BPoss.Clear();

            //LoadPics();
            setShipPics();

            /* roadpoints, BPoss */
            BasicCoords2();
        }
        internal void OnButtonClicked(string cate, string command)
        {
            if (!MacroRunning)
            {
                MacroRunning = true;

                if (cate == "항해")
                    onButtonClicked_Sail(command);
                else if (cate == "배틀")
                    onButtonClicked_Battle(command);
            }
        }
        void onButtonClicked_Sail(string command)
        {
            Console.WriteLine($"Sail OnButtonClicked : {command}");
            switch (command)
            {
                case "연구LR":
                case "바다로":
                case "연구신청":
                case "연습":
                case "루프1":
                case "루프N":
                    new Thread((c) => playLima(command)).Start();
                    break;
                case "항해":
                    new Thread(() => {
                        SailingX(main.navi.coordsWorld, main.navi.GoalCity);
                        MacroRunning = false;
                    }).Start();
                    break;
                case "항구에서":
                    new Thread(() => {
                        OutOfPort();
                        SailingX(main.navi.coordsWorld, main.navi.GoalCity);
                        MacroRunning = false;
                    }).Start();
                    break;
                case "신뢰도 쌓기":
                    new Thread(() => TrustBuilding()).Start();
                    break;
                case "로드 무브":
                    new Thread(() => roadMoveTest()).Start();
                    break;
                case "퀘스트 사그레스":
                    OnButtonClicked_quest(command);
                    break;
            }
        }
        void onButtonClicked_Battle(string command)
        {
            Console.WriteLine($"Battle OnButtonClicked : {command}");
            switch (command)
            {
                case "스캔":
                case "출항":
                case "연습":
                case "단어 검색":
                case "찰칵":
                case "기본각도":
                case "배틀":
                default:
                    new Thread(() => {

                        ShipBattle(command);

                        Console.WriteLine($"쓰레드 종료 : Battle({command})");
                        MacroRunning = false;
                    }).Start();

                    break;
            }
        }
        internal void OnButtonClicked_quest(string title)
        {
            Console.WriteLine($"Sail OnButtonClicked : {title}");
            //return;

            switch (title)
            {
                case "퀘스트 보고":
                    new Thread(() => ReportQuests()).Start();

                    break;
                case "꿀 퀘스트":
                    new Thread(() => HoneyQuest()).Start();

                    break;
                case "퀘스트 받기":
                    new Thread(() => TakeQuests()).Start();

                    break;
                case "사그레스 나옴":
                    new Thread(() => OutOfSagress()).Start();

                    break;
                case "세비야 조합0":
                    new Thread(() => MeetMaster0()).Start();

                    break;
                case "세비야 조합1":
                    new Thread(() => MeetMaster1()).Start();

                    break;
                case "세비야 서고":
                    new Thread(() => MeetProfessor()).Start();

                    break;
                case "말라가 주점0":
                    new Thread(() => MeetBattender0()).Start();

                    break;
                case "말라가 주점1":
                    new Thread(() => MeetBattender1()).Start();

                    break;
                case "에딘버러 항구":
                    new Thread(() => MeetOfficer()).Start();

                    break;
                case "퀘스트 말라가":
                    //new Thread(() => QuestMalaga()).Start();
                    new Thread(() => QuestAndSailing("말라가")).Start();

                    break;
                case "퀘스트 에딘버러":
                    //new Thread(() => QuestEdinburgh()).Start();
                    new Thread(() => QuestAndSailing("에딘버러")).Start();

                    break;
                case "퀘스트 사그레스":
                    new Thread(() => QuestAndSailing("사그레스")).Start();

                    break;
                case "퀘스트 세비야":
                    new Thread(() => QuestAndSailing("세비야")).Start();

                    break;
            }
        }
        void BasicCoords2()
        {
            /*사그레스 거리 좌표*/
            roadpoints["항구앞"] = kvec.Parse2d("138 66");
            roadpoints["행상인"] = kvec.Parse2d("109 68");
            roadpoints["상인교관"] = kvec.Parse2d("92 65");
            roadpoints["모험가교관"] = kvec.Parse2d("91 57");

            /*버튼 위치*/
            BPoss["배 함대"] = new kvec(617, 47);
            BPoss["함대 관리"] = new kvec(642, 182);
            BPoss["함대대원 2x1"] = new kvec(209, 214);
            BPoss["멘토 토글"] = new kvec(561, 165);

            BPoss["돛 펴기"] = new kvec(785, 549);
            BPoss["돛 +1"] = new kvec(785, 569);
            BPoss["돛 -1"] = new kvec(785, 586);
            BPoss["돛 접기"] = new kvec(785, 611);
            BPoss["말건네기 창"] = new kvec(706, 393);

        }
 
    }
}