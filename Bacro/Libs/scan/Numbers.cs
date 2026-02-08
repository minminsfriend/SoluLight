using shine.libs.math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Bacro
{
    public class Numbers
    {
        List<string> numpics = new List<string>();
        string dirData;
        public Numbers(string dirData) 
        {
            this.dirData = dirData;
            setNumPics();
        }
        void setNumPics()
        {
            if (numpics == null)
                numpics = new List<string>();

            var filenums = $@"{dirData}\numbers.txt";

            numpics.Clear();
            var lines = File.ReadAllLines(filenums, Encoding.UTF8);

            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length == 0)
                    continue;

                /* 숫자9 /==/ 18 498 6 11 / 4 6,1 6,0 5,0 3,0 1,1 0,4 0,5 1,5 4,5 6,5 9,4 10,1 10,0 9 */
                var ss = Regex.Split(line, " /==/ ");

                var name = ss[0].Trim();
                var picx = ss[1].Trim().Replace(" / ", "::");

                numpics.Add(picx);
            }
        }
        public string NumsInSik(Bitmap bmp, krect rectLine, Color color0)
        {
            List<List<bool>> boolpixels = new List<List<bool>>();
            List<bool> blanksAllY = new List<bool>();
            
            /* 흑백으로 픽셀값 저장하기 */
            for (int x = 0; x < bmp.Width; x++)
            {
                List<bool> hLine = new List<bool>();

                for (int y = 0; y < bmp.Height; y++)
                {
                    if (color0 == bmp.GetPixel(x, y))
                        hLine.Add(true);
                    else
                        hLine.Add(false);
                }

                boolpixels.Add(hLine);
            }

            /* 줄이 비어있는 가를 예스 노로 리스트만들기 */
            for (int x = 0; x < boolpixels.Count; x++)
            {
                var hLine = boolpixels[x];
                var isblank = true;

                for (int y = 0; y < hLine.Count; y++)
                {
                    if (hLine[y])
                        isblank = false;
                }

                blanksAllY.Add(isblank);
            }

            /* 문자 박스 찾기 */
            List<krect> boxs = new List<krect>();
            krect rect0 = new krect(0, 0, 6, 13);

            int X = 0;
            int startX = X;

            while (startX < bmp.Width - 10)
            {
                krect rect = rect0.copy();
                rect.x = startX;

                while (blanksAllY[X])
                {
                    if (X + 1 > bmp.Width - 1)
                        break;

                    X++;
                    rect.x += 1;
                }

                rect.x = X;
                rect.w = 0;

                while (!blanksAllY[X])
                {
                    if (X + 1 > bmp.Width - 1)
                        break;

                    X++;
                    rect.w += 1;
                }

                if (isNumBox(boolpixels, rect))
                    boxs.Add(rect);

                startX = X;
            }

            /* 인식된 숫자열 리턴 */
            var numtext = "";
            foreach (var box in boxs)
            {
                if (box.W > 6)
                    continue;

                int num = InSikNum(boolpixels, box);
                numtext += $"{num} ";
            }

            return numtext;
        }
        bool isNumBox(List<List<bool>> boolpixels, krect box)
        {
            for (int x = box.X; x < box.Right; x++)
            {
                if (x > boolpixels.Count - 1)
                    return false;

                if (boolpixels[x][0])
                    return false;
            }

            return true;
        }
        int InSikNum(List<List<bool>> boolpixels, krect box)
        {
            if (box.W == 2)
                return 1;

            /* 읽는 순서, 바뀌면 안된다 */
            List<int> nn = new List<int> { 9, 6, 0, 1, 2, 3, 4, 5, 6, 7, 8 };

            for (int i = 0; i < nn.Count; i++)
            {
                var n = nn[i];
                var numx = numpics[n];

                RectPoss rp = new RectPoss(numx);
                var isCorrect = true;

                foreach (var pNum in rp.poss)
                {
                    var px = pNum.copy();
                    px.offset(box.x, 1);

                    if (px.X > boolpixels.Count - 1)
                    {
                        isCorrect = false;
                        break;
                    }

                    if (!boolpixels[px.X][px.Y])
                    {
                        isCorrect = false;
                        break;
                    }
                }

                if (isCorrect)
                    return n;
            }

            return -1;
        }
    }
}
