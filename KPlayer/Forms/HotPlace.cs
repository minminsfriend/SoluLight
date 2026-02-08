using shine.libs.math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KPlayer
{
    public partial class HotPlace : Form
    {
        const string 입력키 = "입력키";
        const string 기능키 = "기능키";
        string KeyMode = 입력키;

        KPlayer main;
        string command = null;
        private string nameCurr = null;

        bool isCtrlOnly = false;
        bool CTRL = false, SHIFT = false;
        public HotPlace(KPlayer main)
        {
            this.main = main;

            InitializeComponent();
            this.DoubleBuffered = true;

            this.Load += HotPlace_Load;
            this.Paint += HotPlace_Paint;

            this.KeyDown += HotPlace_KeyDown;
            this.KeyUp += HotPlace_KeyUp;

            this.MouseDown += HotPlace_MouseDown;
            this.MouseUp += HotPlace_MouseUp;
            this.MouseMove += HotPlace_MouseMove;
        }

        private void HotPlace_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void HotPlace_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void HotPlace_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void HotPlace_KeyUp(object sender, KeyEventArgs e)
        {
           
        }
        private void HotPlace_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;

            switch (keyCode)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    isCtrlOnly = true;
                    CTRL = true;
                    return;
                default:
                    isCtrlOnly = false;
                    break;
            }

            //e.SuppressKeyPress = true;

            switch (keyCode)
            {
                case Keys.Escape:
                    this.Hide();
                    return;
                case Keys.Enter:
                    main.hotPlace_doSomething(command, "hot place");

                    Thread.Sleep(200);
                    this.Hide();
                    return;
            }
        }
        private void HotPlace_Load(object sender, EventArgs e)
        {
            Size size = this.ClientSize;

         
        }
        private void HotPlace_Paint(object sender, PaintEventArgs e)
        {
            
        }
       
        internal void Command(string command, string nameCurr)
        {
            if(nameCurr == null)
                nameCurr = "";

            this.command = command;
            this.nameCurr = nameCurr;
            this.Text = command;

            this.nameCurr = nameCurr;
            this.Invalidate();
        }

        private void textInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
