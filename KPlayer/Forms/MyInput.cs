using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KPlayer
{
    public partial class MyInput : Form
    {
        internal string InputText
        {
            get
            {
                return richInputa.Text.Trim();
            }
        }
        public MyInput(string command, string text, Point location)
        {
            InitializeComponent();
            this.Location = location;

            this.DoubleBuffered = true;
            this.Text = command;

            this.richInputa.Text = text;
            this.richInputa.KeyDown += RichInputa_KeyDown; ;
            this.richInputa.KeyUp += RichInputa_KeyUp;
        }
        private void RichInputa_KeyDown(object sender, KeyEventArgs e)
        {  
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; 
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void RichInputa_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
