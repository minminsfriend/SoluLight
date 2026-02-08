using shine.libs.capture;
using shine.libs.math;
using shine.libs.pad;
using shine.libs.window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static shine.libs.konst.Com;

namespace Bacro
{
    public partial class Bacro : Form
    {
        public string dirData;
        public string dirImage;
        public string filecfg;

        string filepics;

        SpeechSynthesizer voice;
        dReceiver client;

        public CapView capView;
        public Makro macro;
        public CapDho capDho;
        public GameInput gInput;
        public Navi navi;
        public MunZZa munzza;

        public Sail sail;
        PlayFactory factory;
        PlayOxford oxford;
        public Dictionary<string, string> pics = new Dictionary<string, string>();

        bool CTRL, SHIFT, ALT;
        public BPad buttons;
        private krect rectPad;
        private StringFormat stringFormat;

        public Bacro()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            CTRL = SHIFT = ALT = false;

            SetDirsAndFiles();
            LoadPics();

            this.Paint += Bacro_Paint;
            this.KeyDown += Bacro_KeyDown;
            this.KeyUp += Bacro_KeyUp;

            this.MouseDown += Bacro_MouseDown;
            this.MouseUp += Bacro_MouseUp;
            this.MouseMove += Bacro_MouseMove;

            this.Load += Bacro_Load;
            this.FormClosing += Bacro_Closing;
            this.TopMost = false;
        }
        void SetDirsAndFiles()
        {
            if (Directory.Exists(@"D:\Works\vs\SoluLight"))
            {
                dirImage = @"d:\Games\대항온\_Images";
                dirData = @"D:\Works\vs\SoluLight\Bacro\data";
                this.Location = new System.Drawing.Point(680, 1000 - 30); //1920x1028
            }
            else
            {
                dirImage = @"C:\Works\vs\_Images";
                dirData = @"C:\Works\vs\data";

                //this.Location = new System.Drawing.Point(50, 790); // 1600x900
                //this.Location = new System.Drawing.Point(500, 1000 - 30); //1920x1028
                this.Location = new System.Drawing.Point(20, 1000 - 80); //1280x1024
            }

            filecfg = $@"{dirData}\config.txt";
            filepics = $@"{dirData}\pics.txt";
        }
        void Bacro_Load(object sender, EventArgs e)
        {
            createButtonsPad();

            capDho = new CapDho(this);
            macro = new Makro(this);

            capView = new CapView();
            gInput = new GameInput();
            munzza = new MunZZa(this);
            sail = new Sail(this);
            navi = new Navi(this);

            oxford = new PlayOxford(this);
            factory = new PlayFactory(this);

            voice = new SpeechSynthesizer();
            voice.SelectVoiceByHints(VoiceGender.Female);
        }
        void createButtonsPad()
        {
            Size size = this.ClientSize;

            buttons = new BPad(1, 7);
            //buttons.HideButtons(new int[] { 0, 15, 17 });

            //string names = "탑폴스,종료,연결,대항해시대,RIGHT,편집,네비";
            string names = "탑폴스,종료,연결,대항해시대,LEFT,편집,네비";
            string memos = "x,x,x,x,x,x,x";

            rectPad = new krect(size.Width, size.Height);
            rectPad.inflate(-5, -5);

            buttons.SetRects(rectPad);
            buttons.SetNames(names);
            buttons.SetMemos(memos);
        }
        public void PlayMacro(string cate, string command)
        {
            switch (command)
            {
                case "찰칵":
                    shotAndShow();
                    return;
                case "중단":
                    if (oxford != null)
                        oxford.MacroRunning = false;
                    if (sail != null)
                    {
                        sail.ON_BATTLE = false;
                        sail.MacroRunning = false;
                    }
                    if (factory != null)
                        factory.MacroRunning = false;

                    return;
            }
            
            if (cate == "대학")
            {
                if (!oxford.MacroRunning)
                {
                    oxford.MacroRunning = true;

                    new Thread(() => {
                        capDho.FocusWorkDho();
                        oxford.PlayMacro("대학", command);
                    }).Start();
                }
                else
                {
                    Console.WriteLine("매크로 아직 실행중 ...");
                }
            }
            else if (cate == "배틀")
            {
                if (sail != null)
                    sail.OnButtonClicked(cate, command);
            }
            else if (cate == "항해")
            {
                if (sail != null)
                    sail.OnButtonClicked(cate, command);
            }
            else if (cate == "통신")
            {
                if(client != null)
                    client.Test(cate, command);
            }
            else if (cate.IndexOf("생산") > -1 || cate.IndexOf("관리") > -1)
            {
                if (!factory.MacroRunning)
                {
                    factory.MacroRunning = true;
                    new Thread(() => {
                        capDho.FocusWorkDho();
                        factory.PlayMacro(cate, command);
                    }).Start();
                }
                else
                {
                    Console.WriteLine("매크로 아직 실행중 ...");
                }
            }
        }
        void shotAndShow()
        {
            if (munzza != null && munzza.IsDisposed)
                munzza = null;

            if (munzza == null)
            {
                munzza = new MunZZa(this);
                munzza.Show();
            }

            capDho.SearchDhoWindows();
            Bitmap image = capDho.CapImage();

            munzza.PrintImageDhoCap(image);
        }
        void Bacro_Closing(object sender, FormClosingEventArgs e)
        {
            if (factory != null && factory.MacroRunning)
                factory.MacroRunning = false;

            if (sail != null && sail.MacroRunning)
                sail.MacroRunning = false;

            if (client != null)
                client.Close();

            Thread.Sleep(1000);
            Console.WriteLine("### 모든 쓰레드 종료. ###");
        }
        void Bacro_MouseMove(object sender, MouseEventArgs e)
        {
            if (rectPad.contains(e))
            {
                if (buttons.OnMouseMove(new kvec(e.X, e.Y)))
                    this.Invalidate();

                return;
            }
        }
        void Bacro_MouseDown(object sender, MouseEventArgs e)
        {
            if (rectPad.contains(e))
            {
                if (buttons.OnMouseDown(new kvec(e.X, e.Y)))
                    this.Invalidate();

                return;
            }
        }
        async void Bacro_MouseUp(object sender, MouseEventArgs e)
        {
            if (rectPad.contains(e))
            {
                if (buttons.OnMouseUp(new kvec(e.X, e.Y)))
                {
                    BBox button = buttons.GetClicked();

                    if (button != null)
                    {
                        pad_OnButtonClicked(button);

                    }

                    this.Invalidate();

                    await Task.Delay(500);

                    buttons.resetStates();
                    this.Invalidate();
                }
            }
        }
        void pad_OnButtonClicked(BBox button)
        {
            var command = button.title;
            //string names = "탑고정,종료,연결,대항해시대,RIGHT,편집,네비";

            switch (command)
            {
                case "탑고정":
                case "탑해제":
                    buttons.bboxs[0].title = command == "탑폴스" ? "탑트루" : "탑폴스";
                    this.TopMost = buttons.bboxs[0].title == "탑트루";

                    break;
                case "연결":
                case "끊기":
                    buttons.bboxs[2].title = command == "연결" ? "끊기" : "연결";
                    if (command == "연결")
                    {
                        if (client == null)
                            client = new dReceiver(this);

                        string ip = "192.168.1.13";
                        int port = 3300;

                        client.connect(ip, port);
                    }
                    else if (command == "끊기")
                    {
                        if (client != null)
                            client.Close();
                    }
                    break;
                case "종료":
                    Console.WriteLine("<< 종료 >> 클릭!");
                    Console.WriteLine(" 1초간 대기...");

                    if (oxford != null)
                        oxford.MacroRunning = false;
                    if (sail != null)
                    {
                        sail.MacroRunning = false;
                        sail.ON_BATTLE = false;
                    }
                    if (factory != null)
                        factory.MacroRunning = false;

                    Thread.Sleep(1000);
                    Application.Exit();

                    break;
                case "대항해시대":
                    capDho.SearchDhoWindows();
                    capDho.FocusWorkDho();

                    break;
                case "RIGHT":
                case "LEFT":
                    buttons.bboxs[4].title = command == "RIGHT" ? "LEFT" : "RIGHT";
                    capDho.SearchDhoWindows();
                    capDho.FocusWorkDho();

                    break;
                case "편집":
                    if (munzza != null && munzza.IsDisposed)
                        munzza = null;
                    if (munzza == null)
                        munzza = new MunZZa(this);

                    if (munzza.Visible)
                        munzza.Hide();
                    else
                        munzza.Show();
                    break;
                case "네비":
                    if (navi != null && navi.IsDisposed)
                        navi = null;
                    if (navi == null)
                        navi = new Navi(this);

                    if (navi.Visible)
                        navi.Hide();
                    else
                    {
                        capDho.FocusWorkDho();
                        navi.Show();
                    }
                    break;
            }
        }
        void Bacro_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ControlKey:
                    CTRL = true;
                    return;
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                    SHIFT = true;
                    return;
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Menu:
                    ALT = true;
                    return;
            }
        }
        void Bacro_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ControlKey:
                    CTRL = false;
                    return;
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                    SHIFT = false;
                    return;
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Menu:
                    ALT = false;
                    return;
            }

            if(CTRL)
            {
              
            }
            else
            {
               
            }
        }
        void Bacro_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (stringFormat == null)
            {
                stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
            }

            if (buttons == null)
                createButtonsPad();

            if (buttons != null)
                drawButtons(g);
        }
        void drawButtons(Graphics g)
        {
            Font font10 = new Font(new FontFamily("맑은 고딕"), 10f);
            Font font9 = new Font(new FontFamily("맑은 고딕"), 9f);

            SolidBrush brText, brBack;

            for (int i = 0; i < buttons.bboxs.Count; i++)
            {
                BBox box = buttons.bboxs[i];
                //Console.WriteLine($"{box.title} {box.rect.toString()}");

                //brText = new SolidBrush(box.colorText);
                brText = new SolidBrush(Color.Violet);

                Color colorBack = Color.Gray;
                Color colorText = Color.Yellow;

                if (box.hovered)
                {
                    colorBack = Color.SkyBlue;
                    colorText = Color.Red;
                }
                if (box.pressed)
                {
                    colorBack = Color.Coral;
                    colorText = Color.SkyBlue;
                }
                if (box.released)
                {
                    colorBack = Color.Yellow;
                    colorText = Color.Lime;
                }

                brBack = new SolidBrush(Color.FromArgb(100, colorBack));
                //g.FillRectangle(brBack, box.rect.R);
                g.DrawRectangle(new Pen(brBack), box.rect.R);

                var rtxt = box.rect.copy();

                var font = box.title.Length >= 5 ? font9 : font10;
                SolidBrush brushText = new SolidBrush(colorText);

                g.DrawString(box.title, font, brushText, rtxt.R, stringFormat);

            }
        }
        public void InvokePaint()
        {
            this.Invoke(new Action(() => this.Invalidate()));
        }
        public void LoadPics()
        {
            pics.Clear();
            var lines = File.ReadAllLines(filepics, Encoding.UTF8);

            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length == 0)
                    continue;
                if (line.Substring(0, 1) == "#")
                {
                    var cate = line.Substring(1).Trim();
                    pics[$"▶▶▶{cate}"] = "0 0 1 1::0 0::0 0 0";
                }
                else
                {
                    var ss = Regex.Split(line, " /==/ ");

                    var name = ss[0].Trim();
                    var picx = ss[1].Trim().Replace(" / ", "::");

                    pics[name] = picx;
                }

                //Console.WriteLine($"{name} == {picx}");
            }
        }
        public void CapViewActivate()
        {
            this.Invoke(new Action(() => {
                if (capView != null && capView.IsDisposed)
                    capView = null;

                if (capView == null)
                    capView = new CapView();

                if (capView.WindowState != FormWindowState.Normal)
                    capView.WindowState = FormWindowState.Normal;

                capView.Show();
                capView.Activate();
            }));
        }
        public void VoiceSpeak(string text)
        {
            new Thread(() => voice.SpeakAsync(text)).Start();
        }
        internal void socket_gotMessage(Mess m)
        {
           
        }
        protected override void WndProc(ref Message m)
        {
            //Console.WriteLine(m);

            if (m.Msg == wop.WM_COPYDATA)
            {
                DataForSend recvData = (DataForSend)Marshal.PtrToStructure(m.LParam, typeof(DataForSend));
                
                if ((int)recvData.dwData == 2)
                {
                    string text = Marshal.PtrToStringAnsi(recvData.lpData);
                    var ss = Regex.Split(text, "::");

                    if (ss.Length == 3)
                    {
                        var cate = ss[0].Replace(" ","");
                        var command = ss[1].Replace(" ", "");

                        Console.WriteLine($"받은명령 == {cate} :: {command}");
                        PlayMacro(cate, command);
                    }
                    else
                        Console.WriteLine($"통메세지: {text}");

                    //FileNameFromClipboard();
                }
                else
                {
                    Console.WriteLine($"Unrecognized data type = {recvData.dwData}.");
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }
        internal Dictionary<string, string> GetPics()
        {
            throw new NotImplementedException();
        }
        public void SetWorkDho(string LR)
        {
            buttons.bboxs[4].title = LR;
        }
        public string GetWorkDho()
        {
            return buttons.bboxs[4].title;
        }
    }
}
