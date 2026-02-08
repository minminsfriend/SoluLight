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
    public class NumPatt
    {
        public Dictionary<int, List<kvec>> pixels = new Dictionary<int, List<kvec>>();
        public Dictionary<int, List<Color>> colors = new Dictionary<int, List<Color>>();
        public Dictionary<int, krect> rects = new Dictionary<int, krect>();

        public NumPatt()
        {
            fillData();
        }
        void fillData()
        {
            Dictionary<int, string> patterns = new Dictionary<int, string>();

            patterns[2] = "5 14::0 3,0 5,0 11,0 13,3 13,2 8,3 7,3 5,3 3,2 2::255 255 255,255 255 255,240 240 240,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,240 240 240";
            patterns[3] = "5 14::0 3,0 4,3 2,4 4,4 5,2 7,4 9,4 11,3 13,0 11::255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255";
            patterns[9] = "5 14::0 3,0 5,0 7,1 8,1 2,3 3,3 5,4 7,4 8,4 9,3 11,3 12,1 13,0 12,0 11::255 255 255,255 255 255,255 255 255,255 255 255,240 240 240,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,240 240 240,255 255 255,255 255 255";
            patterns[1] = "2 14::0 3,0 6,0 10,0 13,1 2,1 3,1 6,1 10,1 13::255 255 255,255 255 255,255 255 255,255 255 255,240 240 240,255 255 255,255 255 255,255 255 255,255 255 255";

            patterns[7] = "7 14::0 4,1 2,3 2,5 2,4 3,3 7,3 8,2 10,2 12,2 13::240 240 240,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255";
            patterns[8] = "5 14::0 4,0 5,0 9,0 11,1 13,3 13,4 11,4 9,3 7,2 7,4 4,3 2,1 2::255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,240 240 240,240 240 240";
            patterns[0] = "4 14::0 4,0 8,0 11,1 13,3 12,3 8,3 4,1 2::255 255 255,255 255 255,255 255 255,240 240 240,255 255 255,255 255 255,255 255 255,240 240 240";
            patterns[6] = "6 14::0 5,0 7,0 9,0 10,1 12,2 13,4 12,5 10,5 9,4 7,3 7,4 4,4 3,2 2,1 4::255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,240 240 240,255 255 255,255 255 255,255 255 255,240 240 240,240 240 240,240 240 240,255 255 255,240 240 240,255 255 255";
            patterns[4] = "2 14::0 5,0 10,1 3,1 5,1 7,1 10,1 13::255 255 255,240 240 240,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255";
            patterns[5] = "6 14::0 12,1 13,4 12,4 11,5 9,4 8,4 7,3 7,4 2,2 2,1 2,1 3,3 3,4 3::240 240 240,240 240 240,255 255 255,255 255 255,255 255 255,255 255 255,240 240 240,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255,255 255 255";

            for (int k = 0; k <= 9; k++)
            {
                List<kvec> xpixels = new List<kvec>();
                List<Color> xcolors = new List<Color>();
                krect rect = new krect();

                parseData(patterns[k], ref rect, ref xpixels, ref xcolors);

                rects[k] = rect;
                pixels[k] = xpixels;
                colors[k] = xcolors;
            }
        }
        void parseData(string datatex, ref krect rect, ref List<kvec> xpixels, ref List<Color> xcolors)
        {
            string[] tt, ss, nn;
            tt = Regex.Split(datatex, "::");
            xpixels = kvec.ParseList(tt[1].Trim());

            nn = Regex.Split(tt[0], " ");
            rect = new krect(int.Parse(nn[0]), int.Parse(nn[1]));

            ss = Regex.Split(tt[2], ",");
            foreach (var s in ss)
            {
                nn = Regex.Split(s, " ");
                var r = int.Parse(nn[0].Trim());
                var g = int.Parse(nn[1].Trim());
                var b = int.Parse(nn[2].Trim());

                xcolors.Add(Color.FromArgb(r, g, b));
            }
        }
    }
}
