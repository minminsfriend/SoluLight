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
    public partial class KShow : Form
    {
        private string message;

        public KShow()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true); 
            this.SetStyle(ControlStyles.UserPaint, true);           
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true); 
            this.SetStyle(ControlStyles.ResizeRedraw, true);        
            this.UpdateStyles();

            this.DoubleBuffered = true; 

            //this.label1.Text = message;   // 라벨 하나 있다고 가정
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Paint += KShow_Paint;
        }

        internal void ShowMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    ShowMessage(message);
                }));
            }
            else
            {
                this.Opacity = 1;

                this.message = message;
                this.Invalidate();
            }
        }
        internal void HideX()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    HideX();
                }));
            }
            else
            {
                this.message = "";
                this.Hide();
                this.Opacity = 0;
            }
        }
        private void KShow_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Green);

            if (message == null)
                message = "";
            if (message == "")
                return;

            Size size = this.ClientSize;

            krect rect = new krect(size.Width, size.Height);
            rect.inflate(-20, -5);
            rect.offset(10, 5);

            var fontSize = rect.h / 2.0f;

            Font font = new Font("맑은 고딕", (int)fontSize);

            e.Graphics.DrawString(message, font, Brushes.Coral, rect.R);
        }
    }
}
