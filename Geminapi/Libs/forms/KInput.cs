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

using shine.libs.math;

namespace Geminapi
{
    public partial class KInput : Form
    {
        MementoStory main;
        string command = null;
        string nameCurr = null;

        public KInput(MementoStory main)
        {
            this.main = main;

            InitializeComponent();
            this.DoubleBuffered = true;

            this.textInput.KeyDown += TextInput_KeyDown;
            this.textInput.KeyPress += TextInput_KeyPress;
            this.textInput.KeyUp += TextInput_KeyUp;

            this.Paint += KInput_Paint;
        }
        void KInput_Paint(object sender, PaintEventArgs e)
        {
            Size size = this.ClientSize;

            Graphics g = e.Graphics;

            krect rect = new krect(size.Width,size.Height/3);
            rect.inflate(-5,-5);

            Font font = new Font("맑은 고딕", rect.h / 2f);

            using (Pen pen = new Pen(Color.FromArgb(100, 100, 100), 2))
            {
                //g.DrawRectangle(pen, rect.R);

                krect rectT = rect.copy();
                rectT.offset(15, 10);

                g.DrawString(command, font, Brushes.Black, rectT.R);
            }
        }
        void TextInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;

                this.Hide();
                return;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                main.kinput_doSomething(command, textInput.Text);

                Thread.Sleep(200);
                this.Hide();

                return;
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;

                this.nameCurr = this.textInput.Text;

                this.textInput.Select(0, nameCurr.Length);
                this.textInput.SelectionColor = Color.Black;
                this.textInput.SelectionStart = nameCurr.Length;

                return;
            }
        }
        void TextInput_KeyUp(object sender, KeyEventArgs e)
        {
            
        }
        void TextInput_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }
        internal void Command(string command, string nameCurr)
        {
            if(nameCurr == null)
                nameCurr = "";

            this.command = command;
            this.nameCurr = nameCurr;
            this.Text = command;

            this.textInput.Text = nameCurr;
            this.textInput.SelectionStart = this.textInput.Text.Length;

            this.Invalidate();
        }
    }
}
