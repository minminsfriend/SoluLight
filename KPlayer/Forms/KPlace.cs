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
    public partial class KPlace : Form
    {
        const string 입력키 = "입력키";
        const string 기능키 = "기능키";
        string KeyMode = 입력키;

        KPlayer main;
        string command = null;
        private string nameCurr = null;

        FilesBox lbox, rbox;
        LabelBox llbox, rlbox;
        bool isCtrlOnly = false;
        bool CTRL = false, SHIFT = false;
        public KPlace(KPlayer main)
        {
            this.main = main;

            InitializeComponent();
            this.DoubleBuffered = true;

            this.textInput.KeyDown += TextInput_KeyDown;
            this.textInput.KeyPress += TextInput_KeyPress;
            this.textInput.KeyUp += TextInput_KeyUp;

            this.Load += KPlace_Load;
            this.Paint += KInput_Paint;
        }

        private void KPlace_Load(object sender, EventArgs e)
        {
            Size size = this.ClientSize;

            krect rectInput = new krect(textInput.Location.X, textInput.Location.Y, textInput.Width, textInput.Height);

            var dirLeft = @"d:\Works\vs\SoluLight\KPlayer\Data\kidis";
            var dirRight = @"d:\Works\vs\SoluLight\KPlayer\Data\midis";

            lbox = new FilesBox(dirLeft);
            lbox.rect = new krect(0, 0, size.Width / 2, rectInput.y - 10);
            lbox.rect.inflate(-10, -10);

            rbox = new FilesBox(dirRight);
            rbox.rect = new krect(size.Width / 2, 0, size.Width / 2, rectInput.y - 10);
            rbox.rect.inflate(-10, -10);

            llbox = new LabelBox();
            llbox.rect = new krect(20, rectInput.y, 110, rectInput.h);
            
            rlbox = new LabelBox();
            rlbox.rect = new krect(20 + 110 + 20, rectInput.y, 140, rectInput.h);
        }

        private void KInput_Paint(object sender, PaintEventArgs e)
        {
            if (lbox == null)
                return;
            
            Size size = this.ClientSize;
            Graphics g = e.Graphics;
            g.Clear(this.BackColor);

            g.DrawRectangle(Pens.DarkGray, lbox.rect.R);
            g.DrawRectangle(Pens.DarkGray, rbox.rect.R);

            g.DrawRectangle(Pens.DarkGray, llbox.rect.R);
            g.DrawRectangle(Pens.Black, rlbox.rect.R);

            Font font = new Font("맑은 고딕", rlbox.rect.h / 2.7f, FontStyle.Bold);

            using (Pen pen = new Pen(Color.FromArgb(100, 100, 100), 2))
            {
                krect rectT = rlbox.rect.copy();
                rectT.offset(3, 6);

                g.DrawString(command, font, Brushes.Black, rectT.R);
            }

            lbox.Paint(g);
            rbox.Paint(g);
        }
        private void TextInput_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;

            switch(keyCode)
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

            if (KeyMode == 기능키)
            {
                e.SuppressKeyPress = true;

                switch (keyCode)
                {
                    case Keys.Escape:
                        this.Hide();
                        return;
                    case Keys.Enter:
                        main.kplace_doSomething(command, textInput.Text);

                        Thread.Sleep(200);
                        this.Hide();
                        return;
                    case Keys.Back:
                        lbox.FolderBack();

                        this.Invalidate();
                        return;
                }
            }
        }
        private void TextInput_KeyUp(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            switch (keyCode)
            {
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    if(isCtrlOnly)
                    {
                        KeyMode = KeyMode == 입력키 ? 기능키 : 입력키;
                    }

                    isCtrlOnly = false;
                    CTRL = false;
                    return;
            }
        }

        private void TextInput_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
