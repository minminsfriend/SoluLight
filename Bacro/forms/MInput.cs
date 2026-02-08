using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bacro
{
    public partial class MInput : Form
    {
        public string InputValue;

        public MInput()
        {
            InitializeComponent();

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += BtnCancel_Click;

            txtInput.KeyUp += TxtInput_KeyUp;
        }

        private void TxtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                InputValue = txtInput.Text.Trim();
                DialogResult = DialogResult.OK;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                InputValue = txtInput.Text.Trim();
                DialogResult = DialogResult.Cancel;
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
        private void BtnOk_Click(object sender, EventArgs e)
        {
            InputValue = txtInput.Text.Trim();
            DialogResult = DialogResult.OK;
        }
    }
}
