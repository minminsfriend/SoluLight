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
    public partial class GameInput : Form
    {
        public GameInput()
        {
            InitializeComponent();

            this.Location = new Point(250, 500);
            this.tbPizzaPower.Text = "25";
        }
        internal int getLoopTarget()
        {
            var text = tbLoopTarget.Text;

            int num;

            if (int.TryParse(text, out num))
                return num;
            else
                return 1;
        }
        internal int getFullPower()
        {
            var text = tbFullPower.Text;

            int num;

            if (int.TryParse(text, out num))
                return num;
            else
                return 1;
        }
        internal int getPizzaPower()
        {
            var text = tbPizzaPower.Text;

            int num;

            if (int.TryParse(text, out num))
                return num;
            else
                return 1;
        }
    }
}
