using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPlayer
{
    public class FilesBox
    {
        public krect rect;
        public string dir;
        public List<string> files;
        public List<string> exts;

        public int IndexStart = 0;
        public int IndexCurr = 0;

        int ROWS = 15;

        public FilesBox(string dir)
        {
            this.dir = dir;
            files = new List<string>();
            exts = new List<string>() { ".txt", ".mid", ".midi" };

            Update();
        }
        public void Update()
        {
            if (!Directory.Exists(dir))
                return;

            var dirsx = Directory.GetDirectories(dir);
            var filesx = Directory.GetFiles(dir);

            files.Clear();

            foreach (var d in dirsx)
            {
                files.Add(d);
            }
            foreach (var f in filesx)
            {
                var ext = Path.GetExtension(f);

                if (exts.Contains(ext))
                    files.Add(f);
            }
        }
        public void Paint(Graphics g)
        {
            var H = rect.h / ROWS;

            int n = -1;

            krect rec0 = new krect(rect.x, rect.y, rect.w, H);

            Font font = new Font("맑은 고딕", H / 3f);

            while (++n < ROWS)
            {
                var N = IndexStart + n;
                if (N > files.Count - 1)
                    break;

                var rec = rec0.copy();
                rec.offset(0, H * n);

                var recB = rec.copy();
                recB.inflate(-1, -1);
                g.FillRectangle(Brushes.LightGray, recB.R);

                var recT = rec.copy();
                recT.offset(3, 3);

                var file = files[N];

                if (Directory.Exists(file))
                {
                    DirectoryInfo dinfo = new DirectoryInfo(file);
                    g.DrawString(dinfo.Name, font, Brushes.Black, recT.R);
                }
                else
                {
                    var filename = Path.GetFileName(file);
                    g.DrawString(filename, font, Brushes.Black, recT.R);
                }
            }
        }

        internal void FolderBack()
        {
            dir = Path.GetDirectoryName(dir);
            Update();
        }
    }
    public class LabelBox
    {
        public krect rect;
        public string text;

        public LabelBox()
        {

        }
        public void Paint(Graphics g)
        {



        }
    }
}
