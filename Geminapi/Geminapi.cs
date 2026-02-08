using Android.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json; // 이 줄이 반드시 있어야 합니다.
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geminapi
{
    public partial class Geminapi : Form
    {
        const string 저장 = "저장";
        const string 열기 = "열기";
        const string 업로드 = "업로드";
        const string 업로드이미지 = "업로드이미지";
        const string 화일아이디 = "화일아이디";
        const string 캐싱 = "캐싱";
        const string 삭제 = "삭제";
        const string 대화기록 = "대화기록";
        const string 대화모드 = "대화모드";
        const string 참조텍스트 = "참조텍스트";
        const string 참조이미지 = "참조이미지";
        const string 참조캐시 = "참조캐시";
        const string 참조검색 = "참조검색";
        const string 참조없다 = "참조없다";
        const string 이미지수정 = "이미지수정";
        const string 이미지축소 = "이미지축소";

        string dirCurr = @"d:\Works\ApiGemini";
        string nameCurr = 대화기록;
        //string fileId= "3hwc1udgv1sx";
        string fileId = "x75pe2u1e22y";

        bool CTRL, SHIFT, ALT;
        bool OnSending = false;
        bool OnShowMessage = false;
        bool OnShowingWaitingForm = false;

        KInput kinput;
        KPostSync kps;
        KShow kshow, ksend;

        string DialMode = 참조없다;
        string imageOutName = "이미지출력.jpg";

        public Geminapi()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.textWords.KeyDown += TextWords_KeyDown;
            this.textWords.KeyUp += TextWords_KeyUp;
            this.Load+= Geminapi_Load;

            CTRL = SHIFT = ALT = false;

            // TLS 1.2 강제
            System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)3072; 

            kps = new KPostSync(dirCurr, nameCurr);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }
        void Geminapi_Load(object sender, EventArgs e)
        {
            textWords.Text = "◇ 유료토큰 사용중이다. 대화는 15줄이내로 한다.\r\n문단앞에 숫자나, 특수기호를 붙이지 않는다.\r\n사용자에 대한 호칭은 작가님이다.\r\n";
            textWords.SelectionStart = textWords.Text.Length;

            this.Location= new Point(637, 23);
            this.ClientSize = new Size(800, 950);

            this.TextTitle();
        }
        void TextWords_KeyDown(object sender, KeyEventArgs e)
        {
            var keyCode = e.KeyCode;
            switch (keyCode)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    CTRL = true;
                    return;
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    SHIFT = true;
                    return;
            }
          
            if (SHIFT && keyCode == Keys.Escape) 
            { 
                e.SuppressKeyPress = true; 
                return;
            }

            //if (keyCode == Keys.OemBackslash)
            if (keyCode == Keys.Oem5)
            {
                e.SuppressKeyPress = true;
                return;
            }
        }
        async void TextWords_KeyUp(object sender, KeyEventArgs e)
        {
            var keyCode = e.KeyCode;
            //Console.WriteLine($"KeyUp: {keyCode}");

            switch (keyCode)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    CTRL = false;
                    return;
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    SHIFT = false;
                    return;
            }

            string respond;

            if (keyCode == Keys.Oem5)//Keys.OemBackslash
            {
                if (!OnSending)
                {
                    OnSending = true;
                    showWaitingForm();

                    var text4Send = textWords.Text;

                    if (DialMode == 참조텍스트)
                        respond = await kps.SendDialogRef(text4Send, fileId);
                    else if (DialMode == 참조이미지)
                        respond = await kps.SendDialogRefImage(text4Send, fileId);
                    else if (DialMode == 이미지수정)
                        respond = await kps.GenerateImageWithRef(text4Send, fileId, imageOutName);
                    else if (DialMode == 참조검색)
                        respond = await kps.SendDialogSearch(text4Send);
                    else
                        respond = await kps.SendDialog(text4Send);

                    invokeAddText(respond);
                    OnShowingWaitingForm = false;
                    OnSending = false;
                }

                return;
            }

            if (SHIFT && keyCode == Keys.Escape)
            {
                if (!OnSending)
                    Application.Exit();

                SHIFT = false;
                return;
            }

            if(CTRL)
            {
                switch (keyCode)
                {
                    case Keys.A:
                        break;
                    case Keys.O:
                        SendCommand(열기);
                        break;
                    case Keys.S:
                        SendCommand(저장);
                        break;
                    case Keys.L:
                        respond = await kps.GetFileListAsync();
                        invokeAddText(respond);
                        break;
                    case Keys.OemSemicolon:
                        respond = await kps.GetCacheListAsync();
                        invokeAddText(respond);
                        break;
                    case Keys.K:
                        viewFilesListInDirCurr();
                        break;
                    case Keys.M:
                        respond = kps.ChangeModel();
                        showMessage(respond, 1000);
                        this.TextTitle();
                        break;
                }

                switch (keyCode)
                {
                    case Keys.U:
                        SendCommand(업로드);
                        break;
                    case Keys.I:
                        SendCommand(화일아이디);
                        break;
                    case Keys.D:
                        SendCommand(삭제);
                        break;
                    case Keys.T:
                        SendCommand(대화모드);
                        break;
                }

                switch (keyCode)
                {
                    case Keys.D7:
                        SendCommand(이미지축소);
                        break;
                    case Keys.D8:
                        SendCommand(업로드이미지);
                        break;
                }

                CTRL = false;
                return;
            }
        }
        void LoadPreText(int index)
        {
            string textPre = null;

            if (index == 0)
            {
                textPre = "";
                textPre += "◇ 유료토큰 사용중이다\r\n";
                textPre += "대화는 15줄이내로 한다.\r\n";
                textPre += "접속은, c# api 환경이다.\r\n";
                textPre += "문단앞에 숫자나, 특수기호를 붙이지 않는다.\r\n";
                textPre += "사용자에 대한 호칭은 작가님 이다.";
            }
            else if (index == 1)
            {
                textPre = "";
                textPre += "◇ 유료토큰 사용중이다\r\n";
                textPre += "대화는 15줄이내로 한다.\r\n";
                textPre += "접속은, c# api 환경이다.\r\n";
                textPre += "문단앞에 숫자나, 특수기호를 붙이지 않는다.\r\n";
                textPre += "사용자에 대한 호칭은 작가님 이다.\r\n";
                textPre += "참조문서는 대화에 이용하는 것이니, 요약 언급을 별도로 하지않는다.";
            }
            else if (index == 10)
            {
                textPre = "";
                textPre += "";
                textPre += "";
                textPre += "";
            }

            if (textPre != null)
            {
                textWords.Text = textPre;
                textWords.SelectionStart = textWords.Text.Length;
            }
        }
        void invokeAddText(string chatResText)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    invokeAddText(chatResText);
                }));
            }
            else
            {
                textWords.AppendText("\r\n\r\n◆ " + chatResText + "\r\n\r\n◇ ");
                textWords.ScrollToCaret();

                SendKeys.Send("{DOWN}");
                Thread.Sleep(100);
                textWords.SelectionStart = textWords.Text.Length;
                textWords.SelectionLength = 0;
            }
        }
        void SendCommand(string command)
        {
            if (kinput != null && kinput.IsDisposed)
                kinput = null;

            if (kinput == null)
                kinput = new KInput(this);

            kinput.Show();
            kinput.Focus();

            switch(command)
            {
                case 열기:
                    kinput.Command(command, nameCurr);
                    break;
                case 저장:
                    kinput.Command(command, nameCurr);
                    break;
                case 대화모드:
                    kinput.Command(command, DialMode);
                    break;
                case 업로드:
                case 화일아이디:
                case 캐싱:
                case 삭제:
                case 이미지축소:
                case 업로드이미지:
                    kinput.Command(command, "");
                    break;
            }
        }
        public async void kinput_doSomething(string command, string text)
        {
            if (text != null)
                text = text.Trim();

            if (text != null && text != "")
            {

            }
            else return;

            if (command == 열기)
            {
                showMessage($"{열기} : {text}", 1000);

                var fileopen = $@"{dirCurr}\{text}.txt";

                if(!File.Exists(fileopen))
                {
                    showMessage($"파일없다 : {fileopen}", 1000);
                    return;
                }

                var textfull = File.ReadAllText(fileopen, Encoding.UTF8);
                
                textWords.Text = textfull;
                textWords.ScrollToCaret();

                SendKeys.Send("{DOWN}");
                Thread.Sleep(100);
                textWords.SelectionStart = textWords.Text.Length;
                textWords.SelectionLength = 0;
                
                nameCurr = text;
                this.Text = $"[{DialMode}] {nameCurr}";

            }
            else if (command == 저장)
            {
                showMessage($"{저장} : {text}", 1000);

                var filesave = $@"{dirCurr}\{text}.txt";
                var fw = new FileStream(filesave, FileMode.Create, FileAccess.Write);
                var data = Encoding.UTF8.GetBytes(textWords.Text);

                fw.Write(data, 0, data.Length);
                fw.Close();

                nameCurr = text;
                this.Text = $"[{DialMode}] {nameCurr}";

            }
            else if (command == 업로드)
            {
                showMessage($"{업로드} : {text}", 1000);

                var fileName = text;
                var respond = await kps.UploadFile(fileName,"text/plain");
                invokeAddText(respond);
            }
            else if (command == 업로드이미지)
            {
                showMessage($"{업로드이미지} : {text}", 1000);

                var imageName = text;
                var respond = await kps.UploadFile(imageName,"image/jpeg");
                invokeAddText(respond);
            }
            else if (command == 화일아이디)
            {
                showMessage($"{화일아이디} : {text}", 1000);

                fileId = text;
            }
            else if (command == 캐싱)
            {
                showMessage($"{캐싱} : {text}", 1000);

                var fileId = text;
                var respond = await kps.CreateContextCacheAsync(fileId);
                invokeAddText(respond);
            }
            else if (command == 삭제)
            {
                showMessage($"{삭제} : {text}", 1000);

                var fileId = text;
                var respond = await kps.DeleteGeminiFile(fileId);
                invokeAddText(respond);
            }
            else if (command == 이미지축소)
            {
                showMessage($"{이미지축소} : {text}", 1000);

                var imageSrc = @"d:\Works\ApiGemini\images\HakShip00.png";
                var imageDst = $@"d:\Works\ApiGemini\images\{text}.jpg";

                kps.ResizeImage(imageSrc, imageDst, 800);
            }
            else if (command == 대화모드)
            {
                showMessage($"{대화모드} : {text}", 1000);
                List<string> dials = new List<string>() { 참조없다, 참조검색, 참조이미지, 참조텍스트, 참조캐시 };

                if (dials.Contains(text))
                {
                    DialMode = text;
                    this.TextTitle();
                }
                else
                {
                    this.Text = $"[{text}],  모드에 해당되지않는 문구.";
                    new Thread(() => {
                        Thread.Sleep(1000);
                        this.TextTitle();
                    }).Start();
                }
            }
        }

        private void TextTitle()
        {
            var modelName = "플래시";

            if(kps.MODEL.Contains("image"))
                modelName = "바나나";
            else if (kps.MODEL.Contains("pro"))
                modelName = "프로";

            this.Text = $"[{DialMode}] {nameCurr} ★ {modelName} ★ ";
        }

        void viewFilesListInDirCurr()
        {
            if (Directory.Exists(dirCurr))
            {
                var files = Directory.GetFiles(dirCurr, "*.txt");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("파일목록:");

                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    sb.AppendLine($"- {fi.Name} ({fi.Length} bytes)");
                }

                invokeAddText(sb.ToString());
            }
            else
            {
                showMessage($"폴더 없다 : {dirCurr}", 1000);
            }
        }
        void showMessage(string msg, int timeSleep)
        {
            if (kshow != null && kshow.IsDisposed)
                kshow = null;
            if (kshow == null)
                kshow = new KShow();

            if (OnShowMessage)
                return;

            kshow.Show();

            new Thread(() =>
            {
                OnShowMessage = true;

                kshow.ShowMessage(msg);
                Thread.Sleep(timeSleep);

                kshow.HideX();
                OnShowMessage = false;

                this.Invoke(new Action(() => {
                    this.Focus();     
                    this.Activate();  
                }));

            }).Start();
        }
        void showWaitingForm()
        {
            if (ksend != null && ksend.IsDisposed)
                ksend = null;
            if (ksend == null)
                ksend = new KShow();

            if (OnShowingWaitingForm)
                return;

            ksend.Show();

            new Thread(() =>
            {
                OnShowingWaitingForm = true;

                long timeSpent = 0;

                while (OnShowingWaitingForm && timeSpent < 50 * 1000)
                {
                    if (timeSpent % 1000 == 0)
                    {
                        string msg = $"응답대기중 ... {timeSpent / 1000} 초";
                        ksend.ShowMessage(msg);
                    }

                    Thread.Sleep(100);
                    timeSpent += 100;
                }

                OnShowingWaitingForm = false;
                ksend.HideX();

                this.Invoke(new Action(() => {
                    this.Focus();     
                    this.Activate();   
                }));

            }).Start();
        }
    }
}
