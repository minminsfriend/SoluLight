using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeoPad
{
    internal class GetOnBoat
    {
        //753 459,752 513,670 588,863 694,

        Dictionary<string, string> names = new Dictionary<string, string>();
        string password = "#Kospya5602";
        LeoPad main;
        public GetOnBoat(LeoPad leoPad) 
        {
            this.main = leoPad;

            names["빼아뜨리체"]="shinedgt";
            names["아델리야"] = "shinemax08";
            names["라꾸땐시"]= "shinemax09";
            names["훌리오"]= "shinemax10";
            names["다팔아"]= "shinemin";
        }
        internal void Macro(string title)
        {
            if (!names.ContainsKey(title))
                return;

            var gameid = names[title];
            Console.Write($"{gameid} {password}");

            /*
             * 753 459, 아이디
             * 752 513, 패쓰워드
             * 670 588, 팝업 아웃
             * 826 630, 사람 체크
             * 863 694, 로그인 버튼
             */

            var pId = new kvec(753, 459);
            var pPW = new kvec(753, 513);
            var pBlank = new kvec(670, 588);
            var pMen = new kvec(826 ,630);
            var pOk = new kvec(863, 694);

            MouseMove(pId, 500);
            MouseClick("left", 50, 500);
            PasteName(gameid, 50, 500);

            MouseMove(pPW, 500);
            MouseClick("left", 50, 500);
            PasteName(password, 50, 500);

            MouseMove(pBlank, 500);
            MouseClick("left", 50, 500);

            MouseMove(pMen, 500);
            MouseClick("left", 50, 500); 
            
            MouseMove(pOk, 500);
            MouseClick("left", 50, 500);

            Console.Write($"파파야 로그인.");
        }
        void MouseMove(kvec pos, int sleepEnd)
        {
            main.serialX.MouseMove(pos.X, pos.Y);
            Thread.Sleep(sleepEnd);
        }
        void PasteName(string name, int sleepPressed, int sleepEnd)
        {
            Clipboard.SetText(name);
            Thread.Sleep(10);
            main.serialX.keyClick(Keys.LControlKey, Keys.V, sleepPressed);
            Thread.Sleep(sleepEnd);
        }
        void MouseClick(string button, int sleepPressed, int sleepEnd)
        {
            main.serialX.MouseClick(button, sleepPressed);
            Thread.Sleep(sleepEnd);
        }
    }
}
