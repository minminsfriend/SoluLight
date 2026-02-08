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
    public partial class PickPIcs : Form
    {
        MunZZa munzza;
        public PickPIcs(MunZZa munzza)
        {
            this.munzza = munzza;
            InitializeComponent();
            this.ShowInTaskbar = false;

            picsView.ItemActivate += PicsView_ItemActivate;
        }
        private void PicsView_ItemActivate(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;

            if (lv.SelectedItems.Count > 0)
            {
                var tem = lv.SelectedItems[0];

                Console.WriteLine($"선택된 탬 : {tem.Text}");

                munzza.picpic_LoadPic(tem.Text);
            }
        }
        public void PeedTems(Dictionary<string, string> pics)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    peedTems(pics);
                    this.Invalidate();
                }));
            }
            else
            {
                peedTems(pics);
                this.Invalidate();
            }
        }
        void peedTems(Dictionary<string, string> pics)
        {
            picsView.Items.Clear();

            foreach (string name in pics.Keys)
                picsView.Items.Add(name);
        }
        internal void ShowFront()
        {
            if (!this.Visible)
                this.Show();

            if (!this.Focused)
                this.Activate();
        }
    }
}
