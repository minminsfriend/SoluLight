using shine.libs.capture;
using shine.libs.window;
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
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Bacro
{
    public partial class Bakro : Form
    {
        public string dirData;
        public string dirImage;
        public string filecfg;

        string filepics;

        kvec posMove = new kvec();
        kvec posPivot = new kvec();
        bool mouseDowned = false;
        object lockImagePoints = new object();
        bool blockResizeEvent = false;

        krect rectForm;

        SpeechSynthesizer voice;

        public CapView capView;
        public Makro macro;
        public GameInput gInput;
        public Navi navi;
        public MunZZa munzza;

        public Sail sail;
        PlayFactory factory;
        PlayOxford oxford;
        public PlayTest testa;
        public Dictionary<string, string> pics = new Dictionary<string, string>();

        bool CTRL, SHIFT, ALT;

        public Bakro()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            CTRL= SHIFT= ALT=false;

            SetDirsAndFiles();
            LoadPics();

            this.Paint += Bacro_Paint;
            this.KeyDown += Bacro_KeyDown;
            this.KeyUp += Bacro_KeyUp;

            this.Load += Bacro_Load;
            this.FormClosing += Bacro_Closing;
            this.TopMost = true;

            macro = new Makro(this);
            macro.SearchDhoWindows();

            capView = new CapView();
            gInput = new GameInput();
            munzza = new MunZZa(this);
            sail = new Sail(this);
            navi = new Navi(this);

            oxford = new PlayOxford(this);
            //factory = new PlayFactory(this);

            voice = new SpeechSynthesizer();
            voice.SelectVoiceByHints(VoiceGender.Female);

            //navi.Show();
        }
        void SetDirsAndFiles()
        {
            if (Directory.Exists(@"D:\Works\vs\SoluLight"))
            {
                dirImage = @"d:\Games\대항온\_Images";
                dirData = @"D:\Works\vs\SoluLight\Bacro\data";
            }
            else
            {
                dirImage = @"C:\Works\vs\_Images";
                dirData = @"C:\Works\vs\data";
            }

            filecfg = $@"{dirData}\config.txt";
            filepics = $@"{dirData}\pics.txt";
        }
        void Bacro_Load(object sender, EventArgs e)
        {
            
        }
        public void leopad_OnButtonClicked(string command)
        {
            switch (command)
            {
                case "찰칵":
                     shotAndShow();

                    break;
                default:
                    if (!oxford.OnRunning)
                    {
                        macro.SearchDhoWindows();
                        oxford.OnButtonClicked(command);
                    }

                    break;
            }
        }
        void shotAndShow()
        {
            if (munzza != null || munzza.IsDisposed)
                munzza = null;
            if (munzza == null)
                munzza = new MunZZa(this);

            this.Invoke(new Action(() => 
            {
                if (!munzza.Visible)
                    munzza.Show();
                munzza.Activate();

                macro.capDho.SearchDhoWindows();
                Bitmap image = macro.capDho.CapImage();

                munzza.ShotDho(image);
            }));

            //InvokePaint();
        }
        void Bacro_Closing(object sender, FormClosingEventArgs e)
        {
            if (factory != null && factory.MacroRunning)
            {
                factory.MacroRunning = false;
                Thread.Sleep(1000);
                Console.WriteLine("생산 쓰레드 종료");
            }
            if (sail != null && sail.MacroRunning)
            {
                sail.MacroRunning = false;
                Thread.Sleep(1000);
                Console.WriteLine("항해 쓰레드 종료");
            }
        }
    
        void Bacro_KeyDown(object sender, KeyEventArgs e)
        {
            if (CTRL && e.KeyCode == Keys.M)
            {
                //main.navimap_StartSailing();
                return;
            }
            else if (CTRL && e.KeyCode == Keys.N)
            {
                //main.navimap_StopSailing();
                return;
            }
            
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
                switch (e.KeyCode)
                {
                    case Keys.O:
                        CTRL = false;

                        return;
                    case Keys.P:
                        CTRL = false;

                        return;
                    case Keys.M:
                        CTRL = false;

                        if (munzza != null && munzza.IsDisposed)
                        {
                            munzza = null;
                            munzza = new MunZZa(this);
                        }

                        if(!munzza.Visible)
                            munzza.Show();

                        return;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        if (!mouseDowned)
                        {
                        }
                        return;
                    case Keys.E:
                        if (!mouseDowned)
                        {
                          
                        }
                        return;
                    case Keys.X:
                        if (!mouseDowned)
                        {
                            
                        }
                        return;
                    case Keys.C:
                        if (!mouseDowned)
                        {

                            this.Invalidate();
                        }
                        return;
                    case Keys.T:
                        if (!mouseDowned)
                        {
                          
                            this.Invalidate();
                        }
                        return;
                    case Keys.I:
                        if (!mouseDowned)
                        {

                            this.Invalidate();
                        }
                        return;
                }

                switch (e.KeyCode)
                {
                    case Keys.OemOpenBrackets:
                        if (!mouseDowned)
                        {
                            macro.ActivateWindow("left");

                        }
                        return;
                    case Keys.OemCloseBrackets:
                        if (!mouseDowned)
                        {
                            macro.ActivateWindow("right");
                        }
                        return;
                    case Keys.OemBackslash:
                        if (!mouseDowned)
                        {
                            macro.PrintCursor();

                        }
                        return;
                }
            }
        }
        
        void Bacro_Paint(object sender, PaintEventArgs e)
        {
           
        }
       
        public void InvokePaint()
        {
            this.Invoke(new Action(() => this.Invalidate()));
        }
       
        void LoadPics()
        {
            pics.Clear();
            var lines = File.ReadAllLines(filepics, Encoding.UTF8);

            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length == 0)
                    continue;

                var ss = Regex.Split(line, " /==/ ");

                var name = ss[0].Trim();
                var picx = ss[1].Trim().Replace(" / ", "::");

                pics[name] = picx;

                //Console.WriteLine($"{name} == {picx}");
            }
        }
        void readRectInfo(string fileinput, out krect rectform, out krect rectcut )
        {
            rectform = null;
            rectcut = null;

            var lines = File.ReadAllLines(fileinput, Encoding.UTF8);

            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length < 3) continue;
                if (line.Substring(0, 2) == "//") continue;

                var ss = Regex.Split(line, "::");
                if (ss.Length == 2)
                {
                    var title = ss[0].Trim();
                    var text = ss[1].Trim();

                    if (title == "rect map" && text != "null")
                        rectcut = krect.Parse(text);
                    else if (title == "rect form" && text != "null")
                        rectform = krect.Parse(text);
                }
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
        void ShowGameInput()
        {
            if (gInput != null && gInput.IsDisposed)
                gInput = null;

            if (gInput == null)
                gInput = new GameInput();

            gInput.Show();
            gInput.Activate();
        }

        private void btnNavi_Click(object sender, EventArgs e)
        {
            if (navi != null && navi.IsDisposed)
                navi = null;
            if (navi == null)
                navi = new Navi(this);

            if (navi.Visible)
                navi.Hide();
            else
                navi.Show();
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (munzza != null && munzza.IsDisposed)
                munzza = null;
            if (munzza == null)
                munzza = new MunZZa(this);

            if (munzza.Visible)
                munzza.Hide();
            else
                munzza.Show();
        }

        private void btnTemp_Click(object sender, EventArgs e)
        {
            this.TopMost = !this.TopMost;

            this.btnTemp.Text = this.TopMost ? "해제" : "고정";
        }

        private void btnDho_Click(object sender, EventArgs e)
        {
            macro.capDho.SetForeWindow();
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

                    if (ss.Length == 2)
                    {
                        string command = ss[0];
                        Console.WriteLine($"받은명령: {command}");

                        this.Invoke(new Action(() =>
                        {
                            leopad_OnButtonClicked(command);
                        }));
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
    }
}
