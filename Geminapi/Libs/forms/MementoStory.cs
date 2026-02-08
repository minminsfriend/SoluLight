using Android.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
    public partial class MementoStory : Form
    {
        bool CTRL, SHIFT, ALT;
        bool OnSending = false;
        bool OnShowMessage = false;
        bool OnShowingWaitingForm = false;

        KShow kwaiting;
        Geminapi main;

        public MementoStory(Geminapi main)
        {
            this.main = main;

            InitializeComponent();

            this.DoubleBuffered = true;

            this.textWords.KeyDown += TextWords_KeyDown;
            this.textWords.KeyUp += TextWords_KeyUp;
            this.Load += MStory_Load;
            this.Activated += MStory_Activated;

            CTRL = SHIFT = ALT = false;
        }
        void MStory_Activated(object sender, EventArgs e)
        {
            this.Text = main.MakeTitleMStory();
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
        void MStory_Load(object sender, EventArgs e)
        {
            textWords.Text = "◇ 유료토큰 사용중이다. 대화는 15줄이내로 한다.\r\n문단앞에 숫자나, 특수기호를 붙이지 않는다.\r\n사용자에 대한 호칭은 작가님이다.\r\n";
            textWords.SelectionStart = textWords.Text.Length;

            this.Location= new Point(637, 23);
            this.ClientSize = new Size(800, 950);

            this.Text = main.MakeTitleMStory();
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

            if (CTRL && keyCode == Keys.B)
            {
                var text = Clipboard.GetText();
                textWords.SelectedText = text;
            }
            else if (SHIFT && keyCode == Keys.Escape) 
            { 
                e.SuppressKeyPress = true; 
                return;
            }
            else if (keyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                main.Show();
                main.Focus();
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

            string result;

            if (SHIFT && keyCode == Keys.Escape)
            {
                if (!OnSending)
                    Application.Exit();

                SHIFT = false;
                return;
            }
            else if (keyCode == Keys.Oem5)//Keys.OemBackslash
            {
                if (!OnSending)
                {
                    OnSending = true;
                    showWaitingForm();

                    var text4Send = textWords.Text;

                    result = await main.인공지능과의대화(text4Send);

                    if (result == "")
                        result = "(무응답)";

                    main.인공지능답변저장(result);

                    invokeAddText(result);
                    OnShowingWaitingForm = false;
                    OnSending = false;
                }

                return;
            }
        }
        internal void invokeAddText(string chatResText)
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
        void showWaitingForm()
        {
            if (kwaiting != null && kwaiting.IsDisposed)
                kwaiting = null;
            if (kwaiting == null)
                kwaiting = new KShow();

            if (OnShowingWaitingForm)
                return;

            kwaiting.Show();

            new Thread(() =>
            {
                OnShowingWaitingForm = true;

                long timeSpent = 0;

                while (OnShowingWaitingForm && timeSpent < 50 * 1000)
                {
                    if (timeSpent % 1000 == 0)
                    {
                        string msg = $"응답대기중 ... {timeSpent / 1000} 초";
                        kwaiting.ShowMessage(msg);
                    }

                    Thread.Sleep(100);
                    timeSpent += 100;
                }

                OnShowingWaitingForm = false;
                kwaiting.HideX();

                this.Invoke(new Action(() => {
                    this.Focus();     
                    this.Activate();   
                }));

            }).Start();
        }
        internal void ShowX()
        {
            this.Show();
            this.Focus();
        }
        internal void ShowText(string title, string textfull)
        {
            ShowX();

            textWords.Text = textfull;
            textWords.ScrollToCaret();

            SendKeys.Send("{DOWN}");
            Thread.Sleep(100);
            textWords.SelectionStart = textWords.Text.Length;
            textWords.SelectionLength = 0;

            this.Text = title;
        }
        internal string GetText()
        {
            return textWords.Text;
        }
    }
}
