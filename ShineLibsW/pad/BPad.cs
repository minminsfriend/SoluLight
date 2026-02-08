using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using shine.libs.math;

namespace shine.libs.pad
{
    public class BPad
    {
        public string name = "bpad";
        public krect rect;
        krect rectPad;

        public List<BBox> bboxs;
        public int IndexDown, IndexUp;
        public int IndexHover;
        public int IndexClicked;
        public int ROWS, COLS;

        public bool JustEntered = false;
        public bool JustLeaved = false;
        public bool visible = true;

        int Alpha = 127;
        float FontSize = 12f;
        private Color colorNormal;
        private Color colorHovered;
        private Color colorPressed;
        private Color colorReleased;

        public string Names2;
        string Names1;
        int Page = 1;
        Font font;

        public BPad(string namestex, int cols)
        {
            Names1 = namestex;
            var names = Regex.Split(namestex, ",");

            COLS = cols;
            ROWS = names.Length / cols;
            if (names.Length % cols > 0)
                ROWS += 1;

            createGBoxs();
            IndexDown = IndexUp = -1;

            for (int i = 0; i < names.Length; i++)
            {
                if (i < bboxs.Count)
                    bboxs[i].title = names[i].Trim();
                else
                    bboxs[i].title = "empty";
            }
        }
        public BPad(int rows, int cols)
        {
            ROWS = rows;
            COLS = cols;

            createGBoxs();
            IndexDown = IndexUp = -1;
        }
        void createGBoxs()
        {
            bboxs = new List<BBox>();
            int count = ROWS * COLS;

            for (int i = 0; i < count; i++)
            {
                BBox bbox = new BBox();
                bbox.title = $"버튼 {i:d02}";
                bbox.index = i;

                bbox.colorBack = Color.DarkBlue;
                bbox.colorText = Color.White;
                bbox.visible = true;

                bboxs.Add(bbox);
            }
        }
        public void SetRects(krect rectPad)
        {
            this.rectPad = rectPad.copy();

            rect = rectPad.copy();
            rect.inflate(-5, -5);

            float W = rect.w / COLS;
            float H = rect.h / ROWS;

            FontSize = H * 0.3f;
            font = new Font(new FontFamily("맑은 고딕"), FontSize);

            //
            krect rect0 = new krect(W, H);
            rect0.x = rect.x;
            rect0.y = rect.y;
            //rect0.offset(W * 0.0f, -H * 0.1f);

            for (int i = 0; i < bboxs.Count; i++)
            {
                krect rectb = rect0.copy();

                int r = i / COLS;
                int c = i % COLS;

                rectb.offset(c * W, r * H);
                //rectb.inflate(-0.07f * H, -0.07f * H);
                rectb.inflate(-0.1f * H, -0.1f * H);

                bboxs[i].rect = rectb.copy();
            }
        }
        public void OnMouseLeave()
        {
            if (IndexHover > -1)
            {
                bboxs[IndexHover].hovered = false;
                IndexHover = -1;
            }
        }
        public bool OnMouseMove(kvec pos)
        {
            if (!rectPad.contains(pos))
            {
                if (IndexHover > -1)
                {
                    bboxs[IndexHover].hovered = false;
                    IndexHover = -1;
                }

                return false;
            }

            int indexHoverOld = IndexHover;
            int indexHover = -1;

            JustEntered = false;
            JustLeaved = false;

            for (int i = 0; i < bboxs.Count; i++)
            {
                bboxs[i].hovered = false;
            }
            for (int i = 0; i < bboxs.Count; i++)
            {
                if (!bboxs[i].visible) continue;

                if (bboxs[i].Contains(pos))
                {
                    indexHover = i;
                    break;
                }
            }

            if (indexHover == -1)
            {
                if (indexHoverOld > -1)
                {
                    IndexHover = -1;
                    JustLeaved = true;
                    return true;
                }
            }
            else
            {

                if (indexHover != indexHoverOld)
                {
                    bboxs[indexHover].hovered = true;
                    IndexHover = indexHover;
                    JustEntered = true;
                    return true;
                }
            }

            return false;
        }
        public bool OnMouseDown(kvec pos)
        {
            if (!rectPad.contains(pos))
                return false;

            IndexDown = -1;//초기화

            for (int i = 0; i < bboxs.Count; i++)
            {
                bboxs[i].pressed = false;
            }

            for (int i = 0; i < bboxs.Count; i++)
            {
                if (!bboxs[i].visible) continue;
                if (bboxs[i].Contains(pos))
                {
                    bboxs[i].pressed = true;
                    IndexDown = i;
                    return true;
                }
            }

            return false;
        }
        public bool OnMouseUp(kvec pos)
        {
            if (!rectPad.contains(pos))
                return false;

            IndexUp = -1;//초기화
          
            for (int i = 0; i < bboxs.Count; i++)
            {
                bboxs[i].released = false;
            }

            for (int i = 0; i < bboxs.Count; i++)
            {
                if (!bboxs[i].visible) continue;
                if (bboxs[i].Contains(pos))
                {
                    bboxs[i].pressed = false;
                    bboxs[i].released = true;
                    IndexUp = i;
                    IndexClicked = i;
                    return true;
                }
            }

            return false;
        }
        public void Paint(Graphics g, int alpha)
        {
            this.Alpha = alpha;
            Paint(g);
        }
        public void Paint(Graphics g)
        {
            if (font == null) return;

            SolidBrush brText, brBack;
            krect rtxt;

            for (int i = 0; i < bboxs.Count; i++)
            {
                if (!bboxs[i].visible) continue;
                BBox bbox = bboxs[i];

                brText = new SolidBrush(bbox.colorText);

                Color colorBack = colorNormal;

                if (bbox.hovered)
                {
                    colorBack = colorHovered;
                }
                if (bbox.pressed)
                {
                    colorBack = colorPressed;
                }
                if (bbox.released)
                {
                    colorBack = colorReleased;
                }

                brBack = new SolidBrush(Color.FromArgb(Alpha, colorBack));
                g.FillRectangle(brBack, bbox.rect.R);

                rtxt = bbox.rect.copy();
                rtxt.offset(10, 3);

                g.DrawString(bbox.title, font, brText, rtxt.R);
            }

        }
        public void resetStates()
        {
            for (int i = 0; i < bboxs.Count; i++)
            {
                bboxs[i].pressed = false;
                bboxs[i].released = false;
                bboxs[i].hovered = false;
            }

            IndexDown = IndexDown = IndexHover = IndexClicked = -1;
        }
        public bool Clicked(out BBox bbox, out int indexDown)
        {
            if (IndexDown > -1 && IndexUp > -1)
            {
                if (IndexDown == IndexUp)
                {
                    bbox = bboxs[IndexDown];

                    indexDown = IndexDown;
                    IndexDown = IndexUp = -1;//초기화 중복하는 듯
                    return true;
                }
            }

            bbox = null;
            indexDown = -1;
            return false;
        }
        public void SetColors(Color colorNormal, Color colorPressed, Color colorReleased, Color colorHovered)
        {
            this.colorNormal = colorNormal;
            this.colorPressed = colorPressed;
            this.colorReleased = colorReleased;
            this.colorHovered = colorHovered;
        }
        public void SetTexts(string texts)
        {
            if (texts == null)
            {
                for (int i = 0; i < bboxs.Count; i++)
                {
                    bboxs[i].title = "";
                }
            }
            else
            {
                var names = Regex.Split(texts, ",");

                for (int i = 0; i < names.Length; i++)
                {
                    if (i < bboxs.Count)
                        bboxs[i].title = names[i].Trim();
                    else
                        bboxs[i].title = "empty";
                }
            }
        }
        public BBox GetClicked()
        {
            if (0 <= IndexClicked && IndexClicked < bboxs.Count)
                return bboxs[IndexClicked];

            return null;
        }
        public void setKeyDown(int nkey)
        {
            for (int i = 0; i < bboxs.Count; i++)
            {
                bboxs[i].pressed = false;
                bboxs[i].released = false;
            }

            bboxs[nkey].pressed = true;

            IndexDown = nkey;
        }
        public void setKeyUp(int nkey)
        {
            for (int i = 0; i < bboxs.Count; i++)
            {
                bboxs[i].pressed = false;
                bboxs[i].released = false;
            }

            bboxs[nkey].released = true;

            IndexUp = nkey;
            IndexClicked = nkey;
        }
        public void NextPage()
        {
            Page = Page == 1 ? 2 : 1;
            string[] names = null;

            if (Page == 1)
                names = Regex.Split(Names1, ",");
            else if (Page == 2)
                names = Regex.Split(Names2, ",");

            if (names != null)
            {
                int n = -1;

                foreach (var name in names)
                {
                    n++;
                    if (n > bboxs.Count - 1)
                        break;

                    bboxs[n].title = name;
                }
            }
        }
        public void SetNames(string namestex)
        {
            if (namestex == null)
                return;

            var names = Regex.Split(namestex, ",");

            for (int i = 0; i < names.Length; i++)
            {
                if (i < bboxs.Count)
                    bboxs[i].title = names[i].Trim();
                else
                    bboxs[i].title = "empty";
            }
        }
        public void SetMemos(string memostex)
        {
            var memos = Regex.Split(memostex, ",");

            for (int i = 0; i < memos.Length; i++)
            {
                if (i < bboxs.Count)
                    bboxs[i].memo = memos[i].Trim();
                else
                    bboxs[i].memo = "empty";
            }
        }

    }
}
