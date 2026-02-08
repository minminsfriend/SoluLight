using shine.libs.math;
using shine.libs.pad;
using shine.libs.serial;
using shine.libs.window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeoPad
{
    public partial class LeoPad : Form
    {
        public BPad buttons;
        krect rectPad;
        public SerialX serialX;
        ZucsucMacro zucsuc;
        IntPtr hwndBacro = IntPtr.Zero;
        IntPtr hwndMaster = IntPtr.Zero;

        string APP = "Bacro";
        string CATE = "항해";

        public string dirData;
        kvec LocStart;
        bool CTRL = false;

        Dictionary<string, string> namecaps=new Dictionary<string, string>();
        public LeoPad()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            SetDirsAndFiles();

            this.Paint += LeoPad_Paint;
            this.KeyDown += LeoPad_KeyDown;
            this.KeyUp += LeoPad_KeyUp;
            this.MouseDown += LeoPad_MouseDown;
            this.MouseUp += LeoPad_MouseUp;
            this.MouseMove += LeoPad_MouseMove;
            this.MouseWheel += LeoPad_MouseWheel;

            this.Load += LeoPad_Load;
            this.Resize += LeoPad_Resize;
            this.FormClosing += LeoPad_Closing;
            
            this.TopMost = false;
            this.Opacity = 0.8;
            //this.TransparencyKey = System.Drawing.Color.Green;
        }
        void SetDirsAndFiles()
        {
            if (Directory.Exists(@"D:\Works\vs\SoluLight"))
            {
                dirData = @"D:\Works\vs\SoluLight\Bacro\data";
                LocStart = new kvec(2400, 480);
            }
            else
            {
                dirData = @"C:\Works\vs\data";
                //LocStart = new kvec(50, 440);
                LocStart = new kvec(50, 620);
            }
        }
        void LeoPad_Closing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("시리얼 닫기...");

            serialX.Close();
            Thread.Sleep(1000);
        }
        void LeoPad_Resize(object sender, EventArgs e)
        {

        }
        void LeoPad_Load(object sender, EventArgs e)
        {
            Size size = this.ClientSize;
            this.Location = LocStart.P;

            createButtonsPad(APP);

            serialX = new SerialX(this);
            zucsuc = new ZucsucMacro(this);
        }
        string fillCommands(string app, string cate)
        {
            /*여기서 cate 기억*/
            CATE = cate;

            /*
            keycaps = "x,/,*,←";
            keycaps += ",7,8,9,-";
            keycaps += ",4,5,6,+";
            keycaps += ",1,2,3,x";
            keycaps += ",0,x,.,↙";
            */

            string commands = null;

            if (app == "Bacro")
            {
                APP = app;
                commands = $"{app},@,@,{cate}";

                if (cate == "항해")
                {
                    commands += ",찰칵,@,네비저장,@";
                    commands += ",연구LR,@,네비열기,항구에서";
                    commands += ",연구신청,바다로,루프1,x";
                    commands += ",루프N,x,중단,항해";
                }
                else if (cate == "배틀")
                {
                    commands += ",찰칵,@,네비저장,해상에서";
                    commands += ",@,@,네비열기,항구에서";
                    commands += ",@,기본각도,강적배틀,x";
                    commands += ",출항,x,중단,배틀";
                }
                else if (cate == "대학")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,닭부화,달걀생산,@";
                    commands += ",생산반복,@,대화,x";
                    commands += ",연구,x,중단,생산";
                }
                else if (cate == "심플")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,@,@,@";
                    commands += ",@,@,@,x";
                    commands += ",연습,x,중단,5번연타";
                }
                else if (cate == "통신")
                {
                    commands += ",찰칵,8,9,A";
                    commands += ",4,5,6,B";
                    commands += ",1,2,3,x";
                    commands += ",연습,x,중단,5번연타";
                }
                else if (cate == "햄생산페낭")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,@,@,햄한번만";
                    commands += ",은행으로,교역소로,@,x";
                    commands += ",연구,x,중단,생산";
                }
                else if (cate == "햄생산트루히요")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",은행으로,교역소로,햄다섯번,햄한번만";
                    commands += ",연구,@,생산만한번,x";
                    commands += ",음식만들기,x,중단,생산";
                }
                else if (cate == "햄관리트루히요")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,@,관리다섯번,관리한번만";
                    commands += ",팔고사기,도시밖으로,생산만한번,x";
                    commands += ",음식만들기,x,중단,관리";
                }
                else if (cate == "햄생산히혼")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,@,햄다섯번,햄한번만";
                    commands += ",@,돼지구입,생산만한번,x";
                    commands += ",음식만들기,x,중단,생산";
                }
                else if (cate == "놋쇠생산뤼베크")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,@,오직생산1,생산1";
                    commands += ",@,광석구입,생산5,x";
                    commands += ",음식생산,x,중단,생산";
                }
                else if (cate == "소세지생산헤르데르")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,@,오직생산1,생산1";
                    commands += ",@,양구입,생산5,x";
                    commands += ",음식생산,x,중단,생산";
                }
                else if (cate == "피자생산포르투")
                {
                    commands += ",찰칵,@,밀가루준비,즉석제작판매";
                    commands += ",교역소로,도구점으로,오직생산1,생산1";
                    commands += ",@,재료구입,생산5,x";
                    commands += ",판매,x,중단,생산";
                }
                else if (cate == "학술햄생산")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,@,@,햄한번만";
                    commands += ",@,@,@,x";
                    commands += ",연습,x,중단,생산";
                }
                else if (cate == "통조림생산")
                {
                    commands += ",찰칵,@,@,@";
                    commands += ",@,@,@,통조림한번만";
                    commands += ",@,@,@,x";
                    commands += ",@,x,중단,생산";
                }
            }

            else if (app == "Master")
            {
                APP = app;
                commands = $"{app},@,@,{cate}";

                if (cate == "메모장")
                {
                    commands += ",@,@,앞장,메모삭제";
                    commands += ",@,@,@,메모추가";
                    commands += ",@,@,뒷장,x";
                    commands += ",@,x,@,@";
                }
                else if (cate == "공백")
                {
                    commands += ",@,@,@,@";
                    commands += ",@,@,@,@";
                    commands += ",@,@,@,x";
                    commands += ",@,x,@,@";
                }
            }
            else if (app == "Leo")
            {
                APP = app;
                commands = $"{app},@,@,{cate}";

                if (cate == "기본")
                {
                    commands += ",@,@,@,@";
                    commands += ",@,@,@,@";
                    commands += ",@,@,5분클릭,x";
                    commands += ",@,x,중단,100초클릭";
                }
                else if (cate == "공백")
                {
                    commands += ",@,@,@,@";
                    commands += ",@,@,@,@";
                    commands += ",@,@,@,x";
                    commands += ",@,x,@,@";
                }
            }
            return commands;
        }
        void createButtonsPad(string app)
        {
            Size size = this.ClientSize;

            buttons = new BPad(5, 4);
            //buttons.HideButtons(new int[] { 0, 15, 17 });

            string names = fillCommands(app, "항해");
            string memos = "★,/,*,←";
            memos += ",7,8,9,-";
            memos += ",4,5,6,+";
            memos += ",1,2,3,x";
            memos += ",0,x,.,↙";

            rectPad = new krect(size.Width, size.Height);
            rectPad.inflate(-5, -5);

            buttons.SetRects(rectPad);
            buttons.SetNames(names);
            buttons.SetMemos(memos);
        }
        void LeoPad_MouseWheel(object sender, MouseEventArgs e)
        {

        }
        void LeoPad_MouseMove(object sender, MouseEventArgs e)
        {
            if (rectPad.contains(e))
            {
                if (buttons.OnMouseMove(new kvec(e.X, e.Y)))
                    this.Invalidate();

                return;
            }
        }
        IntPtr findWindow(string proname)
        {
            /* Process 열려있나 확인 */
            Process[] processRunning = Process.GetProcesses();

            foreach (Process p in processRunning)
            {
                //열려있다.
                if (p.ProcessName == proname && p.MainWindowTitle != "")
                {
                    var hwndApp = p.MainWindowHandle;

                    Console.WriteLine($"{proname} hwnd == {hwndApp}");

                    return hwndApp;
                }
            }

            return IntPtr.Zero;
        }
        void sendCommandToBacro(string text)
        {
            if (hwndBacro == IntPtr.Zero || !wop.IsWindow(hwndBacro))
                hwndBacro = findWindow("Bacro");
            else
            {
                //pass
            }

            if (hwndBacro != IntPtr.Zero)
            {
                sendCommand(hwndBacro, text);
            }
        }
        void sendCommandToMaster(string text)
        {
            if (hwndMaster == IntPtr.Zero || !wop.IsWindow(hwndMaster))
                hwndMaster = findWindow("MemoMaster");
            else
            {
                //pass
            }

            if (hwndMaster != IntPtr.Zero)
            {
                sendCommand(hwndMaster, text);
            }
        }
        void sendCommand(IntPtr hwndApp, string text)
        {
            IntPtr pData = IntPtr.Zero;

            try
            {
                var textx = $"{text}::_________________________________";

                DataForSend data = new DataForSend();
                data.dwData = new IntPtr(2);
                data.dataSize = textx.Length + 100;
                data.lpData = Marshal.StringToHGlobalAnsi(textx);

                pData = Marshal.AllocCoTaskMem(Marshal.SizeOf(data));
                Marshal.StructureToPtr(data, pData, false);

                wop.SendMessage(hwndApp, wop.WM_COPYDATA, IntPtr.Zero, pData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
            finally
            {
                if (pData != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pData);
            }
        }
        async void LeoPad_MouseUp(object sender, MouseEventArgs e)
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
        void LeoPad_MouseDown(object sender, MouseEventArgs e)
        {
            if (rectPad.contains(e))
            {
                if (buttons.OnMouseDown(new kvec(e.X, e.Y)))
                    this.Invalidate();

                return;
            }
        }
        void LeoPad_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    CTRL = true;
                    return;
            }
        }
        void LeoPad_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    CTRL = false;
                    return;
            }

            Console.WriteLine($"키업 {e.KeyCode}");

            if (!CTRL) 
                return;

            Keys key = e.KeyCode;

            switch (key)
            {
                case Keys.Space:
                    findWindow("Bacro");
                    return;
                case Keys.Down:
                    this.TopMost = false;
                    return;
                case Keys.Up:
                    this.TopMost = true;
                    return;
                case Keys.Escape:
                    Application.Exit();
                    return;
            }

            string commands = null;

            switch (key)
            {
                case Keys.D1:
                    commands = fillCommands("Bacro", "항해");
                    break;
                case Keys.D2:
                    commands = fillCommands("Bacro", "배틀");
                    break;
                case Keys.D3:
                    //commands = fillCommands("Bacro", "햄생산히혼");
                    commands = fillCommands("Bacro", "햄생산트루히요");
                    //commands = fillCommands("Bacro", "햄생산페낭");
                    //commands = fillCommands("Bacro", "소세지생산");
                    //commands = fillCommands("Bacro", "학술햄생산");
                    break;
                case Keys.D4:
                    commands = fillCommands("Bacro", "햄관리트루히요");
                    break;
                case Keys.D5:
                    commands = fillCommands("Bacro", "소세지생산헤르데르");
                    break;
                case Keys.D6:
                    commands = fillCommands("Bacro", "놋쇠생산뤼베크");
                    break;
                case Keys.D8:
                    commands = fillCommands("Bacro", "통조림");
                    break;
                case Keys.D9:
                    commands = fillCommands("Bacro", "대학");
                    break;
                case Keys.D0:
                    commands = fillCommands("Bacro", "통신");
                    break;
         
                case Keys.M:
                    commands = fillCommands("Master", "메모장");
                    break;
                case Keys.L:
                    commands = fillCommands("Leo", "기본");
                    break;
            }

            if (commands != null)
            {
                buttons.SetNames(commands);
                this.Invalidate();
                return;
            }

        
        }
        void LeoPad_Paint(object sender, PaintEventArgs e)
        {
            if (buttons == null)
                return;

            Font font = new Font(new FontFamily("맑은 고딕"), 10f);
            Font fontM = new Font(new FontFamily("맑은 고딕"), 14f);

            SolidBrush brText, brBack;
            krect rtxt, rmem;

            Graphics g = e.Graphics;

            for (int i = 0; i < buttons.bboxs.Count; i++)
            {
                BBox box = buttons.bboxs[i];
                if (!box.visible) continue;

                //brText = new SolidBrush(box.colorText);
                brText = new SolidBrush(Color.Violet);

                Color colorBack = Color.Gray;

                if (box.hovered)
                {
                    colorBack = Color.SkyBlue;
                }
                if (box.pressed)
                {
                    colorBack = Color.Coral;
                }
                if (box.released)
                {
                    colorBack = Color.Yellow;
                }

                brBack = new SolidBrush(Color.FromArgb(100, colorBack));
                //g.FillRectangle(brBack, box.rect.R);
                g.DrawRectangle(new Pen(brBack), box.rect.R);

                if (box.memo == "x")
                    continue;

                rtxt = box.rect.copy();
                rmem = box.rect.copy();
                rmem.w /= 2.5f;
                rmem.h /= 2.5f;
                rmem.offset(rtxt.w / 2f, rtxt.h / 2f);

                if (i == 0)
                {
                    g.DrawString(box.title, font, Brushes.SkyBlue, rtxt.R);
                }
                else
                {
                    g.DrawString(box.memo, fontM, Brushes.Violet, rmem.R);
                    g.DrawString(box.title, font, Brushes.Yellow, rtxt.R);
                }
            }
        }
        internal void serial_doSomething(string actionx, int hkey)
        {
            var action = actionx == "DN" ? "down" : "up";
            string memo = null;

            switch ((HKEY)hkey)
            {
                case HKEY.BAK:
                    memo = "←";
                    break;
                case HKEY.LOC:
                    memo = "L";
                    break;
                case HKEY.DIV:
                    memo = "/";
                    break;
                case HKEY.MUL:
                    memo = "*";
                    break;
                case HKEY.SUB:
                    memo = "-";
                    break;
                case HKEY.ADD:
                    memo = "+";
                    break;
                case HKEY.ENT:
                    memo = "↙";
                    break;
                case HKEY.N1:
                    memo = "1";
                    break;
                case HKEY.N2:
                    memo = "2";
                    break;
                case HKEY.N3:
                    memo = "3";
                    break;
                case HKEY.N4:
                    memo = "4";
                    break;
                case HKEY.N5:
                    memo = "5";
                    break;
                case HKEY.N6:
                    memo = "6";
                    break;
                case HKEY.N7:
                    memo = "7";
                    break;
                case HKEY.N8:
                    memo = "8";
                    break;
                case HKEY.N9:
                    memo = "9";
                    break;
                case HKEY.N0:
                    memo = "0";
                    break;
                case HKEY.DEL:
                    memo = ".";
                    break;
            }

            if (memo != null)
            {
                PadKeyAct(action, memo);
            }
        }
        internal void PadKeyAct(string action, string memo)
        {
            if (action == "down")
                return;

            BBox box = null;

            foreach (var bx in this.buttons.bboxs)
            {
                if (bx.memo == memo)
                {
                    Console.WriteLine($"패드키 [{bx.memo}] {bx.title}");
                    box = bx;
                    break;
                }
            }

            if (box.memo != null)
            {
                this.Invoke(new Action(() =>
                {
                    pad_OnButtonClicked(box);
                }));
            }
        }
        void pad_OnButtonClicked(BBox button)
        {
            var command = button.title;

            if (APP == "Bacro")
            {
                var cate = buttons.bboxs[3].title;
                //sendCommandToBacro($"{CATE}::{command}");
                sendCommandToBacro($"{cate}::{command}");

            }
            else if (APP == "Master")
            {
                var cate = buttons.bboxs[3].title;
                //sendCommandToBacro($"{CATE}::{command}");
                sendCommandToMaster($"{cate}::{command}");

            }
            else if (APP == "Leo")
            {
                if (command == "중단")
                    zucsuc.OnMacro = false;
                
                else if (!zucsuc.OnMacro)
                {
                    var cate = buttons.bboxs[3].title;

                    new Thread(() =>
                    {
                        zucsuc.OnMacro = true;

                        zucsuc.PlayMacro(cate, command);

                        zucsuc.OnMacro = false;

                    }).Start();
                }
            }
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
                        //Console.WriteLine($"받은명령: {command}");

                        this.Invoke(new Action(() =>
                        {
                            serialX.KeysRemote(command);
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
    }
}
