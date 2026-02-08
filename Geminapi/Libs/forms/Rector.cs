using shine.libs.math;
using shine.libs.window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geminapi
{
    public partial class Shotter : Form
    {
        krect rectApp = new krect(-1, -1, 10, 10);
        IntPtr hwndApp = IntPtr.Zero;
        private string message;
        private bool CTRL, ALT, SHIFT;
        List<kvec> poss=new List<kvec>();
        private bool transparent=true;

        public Shotter()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            this.Paint += Rector_Paint;
            this.KeyDown += Rector_KeyDown;
            this.KeyUp += Rector_KeyUp;
            this.MouseDown += Rector_MouseDown;
            this.MouseUp += Rector_MouseUp;
            this.MouseMove += Rector_MouseMove;
            this.MouseWheel += Rector_MouseWheel;

            this.Load += Rector_Load;
            this.Resize += Rector_Resize;
            this.Move += Rector_Move;
            this.FormClosing += Rector_Closing;

            CTRL = ALT = SHIFT = false;
        }
        private void Rector_Move(object sender, EventArgs e)
        {
            var pos = this.Location;
            var size = this.ClientSize;
            krect rect = new krect(pos.X, pos.Y, size.Width, size.Height);

            this.Text = rect.toString();
        }
        private void Rector_Resize(object sender, EventArgs e)
        {
            var pos = this.Location;
            var size = this.ClientSize;
            krect rect = new krect(pos.X, pos.Y, size.Width, size.Height);

            this.Text = rect.toString();
        }

        private void Rector_Closing(object sender, FormClosingEventArgs e)
        {
            
        }

  
        private void Rector_Load(object sender, EventArgs e)
        {
            
        }

        private void Rector_MouseWheel(object sender, MouseEventArgs e)
        {
            
        }

        private void Rector_MouseMove(object sender, MouseEventArgs e)
        {
            
        }
        private void Rector_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        private void Rector_MouseDown(object sender, MouseEventArgs e)
        {
            
        }


        private void Rector_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;
            switch (key)
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
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    ALT = true;
                    return;
            }
        }
        private void Rector_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;
            switch (key)
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
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    ALT = false;
                    return;
            }

            if(CTRL)
            {
                switch (key)
                {
                    case Keys.R:
                        recodeRect();
                        message = rectApp.toString();

                        this.Invalidate();
                        break;
                    case Keys.C:
                        Clipboard.SetText(message);

                        this.Text = "메세지 클립보드 복사";
                        break;
                }

                return;
            }

            switch (key)
            {
                case Keys.Escape:
                    this.WindowState = FormWindowState.Minimized;

                    break;
                case Keys.Space:
                    recodePos();

                    this.Invalidate();
                    break;
                case Keys.Back:
                    delLastPos();

                    this.Invalidate();
                    break;
                case Keys.X:
                    clearPoss();

                    this.Invalidate();
                    break;
                case Keys.G:
                    transparent = !transparent;

                    this.Invalidate();
                    break;
            }
        }


        private void delLastPos()
        {
            if (poss.Count == 0)
                return;

            if (poss.Count == 1)
            {
                poss.Clear();
                message = null;
                return;
            }

            poss.RemoveAt(poss.Count - 1);

            var text = "";
            foreach (var p in poss)
            {
                var posx = $"{p.X} {p.Y}";
                text += $"{posx},";
            }

            message = text;
        }

        private void clearPoss()
        {
            poss.Clear();
            message = null;
        }

        private void recodePos()
        {
            var text = "";

            recodeRect();

            poss.Add(rectApp.pos());

            foreach(var p in poss)
            {
                var posx = $"{p.X} {p.Y}";
                text += $"{posx},";
            }

            message = text;
        }

        private void recodeRect()
        {
            if (hwndApp == IntPtr.Zero || wop.IsWindow(hwndApp))
                hwndApp = findHwnd("Rector");

            if (hwndApp != IntPtr.Zero)
                wop.getWindowRect(hwndApp, ref rectApp);
        }

        private IntPtr findHwnd(string ProcessName)
        {
            Process[] processRunning = Process.GetProcesses();

            foreach (Process p in processRunning)
            {
                //열려있다.
                if (p.ProcessName == ProcessName && p.MainWindowTitle != "")
                {
                    return p.MainWindowHandle;
                }

                if (p.MainWindowTitle != "")
                    Console.WriteLine($"{p.ProcessName} : {p.MainWindowHandle} : {p.MainWindowTitle}");
            }

            return IntPtr.Zero;
        }

        private void Rector_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (transparent)
            {
                g.Clear(Color.Green);
                return;
            }

            g.Clear(Color.Gray);

            if (message == null)
            {
                return;
            }

            Font font = new Font(new FontFamily("맑은 고딕"), 14f);

            var size = this.ClientSize;
            krect rect=new krect(size.Width, size.Height);
            rect.inflate(-10, -20);

            g.DrawString(message, font, Brushes.Blue, rect.R);

        }
    }
}
