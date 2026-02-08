using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bacro
{
    public class ShipNameRect
    {
        public string name;
        public krect rect;

        public ShipNameRect(string name, krect rect)
        {
            this.name = name;
            this.rect = rect;
        }
    }
    public class RectPossColors
    {
        public kvec offx;
        public krect rect;
        public List<kvec> poss;
        public List<Color> colors;

        readonly string SP = " ";
        public RectPossColors(string data)
        {
            var ss = Regex.Split(data, "::");

            var dd = Regex.Split(ss[0], SP);
            var rr = Regex.Split(ss[1], SP);
            var pp = Regex.Split(ss[2], ",");
            var cc = Regex.Split(ss[3], ",");

            int dx = int.Parse(dd[0]);
            int dy = int.Parse(dd[1]);
            int w = int.Parse(rr[0]);
            int h = int.Parse(rr[1]);

            offx = new kvec(dx, dy);
            rect = new krect(w, h);

            poss = new List<kvec>();
            colors = new List<Color>();

            foreach (var p in pp)
            {
                var nn = Regex.Split(p, SP);
                var x = int.Parse(nn[0]);
                var y = int.Parse(nn[1]);

                poss.Add(new kvec(x, y));
            }

            foreach (var c in cc)
            {
                var nn = Regex.Split(c, SP);
                int r = int.Parse(nn[0]);
                int g = int.Parse(nn[1]);
                int b = int.Parse(nn[2]);

                colors.Add(Color.FromArgb(r, g, b));
            }
        }
    }
    public class RectPoss
    {
        public krect rect;
        public List<kvec> poss;

        readonly string SP = " ";
        public RectPoss(string data)
        {
            var ss = Regex.Split(data, "::");

            var rr = Regex.Split(ss[0], SP);
            var pp = Regex.Split(ss[1], ",");

            int w = int.Parse(rr[2]);
            int h = int.Parse(rr[3]);

            rect = new krect(w, h);
            poss = new List<kvec>();

            foreach (var p in pp)
            {
                var nn = Regex.Split(p, SP);
                var x = int.Parse(nn[0]);
                var y = int.Parse(nn[1]);

                poss.Add(new kvec(x, y));
            }
        }
    }
}
