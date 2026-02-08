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
    public partial class KConsole : Form
    {
        bool CTRL, SHIFT, ALT;
        Geminapi main;

        public KConsole(Geminapi main)
        {
            this.main = main;

            InitializeComponent();

            this.DoubleBuffered = true;

            this.textWords.KeyDown += TextWords_KeyDown;
            this.textWords.KeyUp += TextWords_KeyUp;
            this.Load += KConsole_Load;
            this.Activated += KConsole_Activated;

            CTRL = SHIFT = ALT = false;
        }
        public RichTextBox TextWords
        {
            get => textWords;
        }
        void KConsole_Activated(object sender, EventArgs e)
        {
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
        void KConsole_Load(object sender, EventArgs e)
        {
            
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
                SHIFT = false;
                return;
            }
            else if (keyCode == Keys.Oem5)//Keys.OemBackslash
            {
                
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
