using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using shine.libs.math;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace shine.libs.pad
{
    public interface IAllpads
    {
        void setSize(int width, int height);
        void OnMouseDown(int x, int y, ref string padname, string button);
        void OnMouseMove(int x, int y, ref string padname, string button);
        void OnMouseUp(int x, int y, ref string padname, string button);
        void Paint(Graphics g);
    }
    public class BasePad
    {
        public string name;
        public krect rect;
        protected krect rectCanvas;

        public List<GBox> gboxs;
        public int IndexDown, IndexUp;
        int indexRecent = -1;
        int indexHover = -1;
        public int IndexHover
        {
            get
            {
                if (indexHover > gboxs.Count - 1)
                    return -1;
                else
                    return indexHover;
            }
            set
            {
                indexHover = value;
            }
        }
        public int IndexRecent
        {
            get
            {
                if (indexRecent > gboxs.Count - 1)
                    return -1;
                else
                    return indexRecent;
            }
            set
            {
                indexRecent = value;
            }
        }
        public int ROWS, COLS;

        public bool JustEntered = false;
        protected bool JustLeaved;
        public bool visible = true;
        public float FontSize;

        public BasePad(string name, int rows, int cols, float fontsize)
        {
            this.name = name;

            ROWS = rows;
            COLS = cols;
            FontSize = fontsize;

            createGBoxs();

            IndexDown = IndexUp = -1;
        }
        protected virtual void createGBoxs()
        {
            gboxs = new List<GBox>();

            int count = ROWS * COLS;

            for (int i = 0; i < count; i++)
            {
                GBox gbox = new GBox();
                gbox.title = "";

                gbox.colorBack = Color.FromArgb(180,Color.Blue);
                gbox.colorText = Color.White;
                gbox.visible = true;

                gboxs.Add(gbox);
            }
        }
        public virtual void SetRects(krect rectCanvas)
        {
            this.rectCanvas = rectCanvas.copy();

            rect = new krect(600, 200, 400, 400);

            setRects();
        }
        protected virtual void setRects()
        {
            float W = 100;
            float H = 44;

            krect rect0 = new krect(W, H);
            rect0.x = rect.x;
            rect0.y = rect.y;

            for (int i = 0; i < gboxs.Count; i++)
            {
                krect rectg = rect0.copy();

                int r = i / COLS;
                int c = i % COLS;

                rectg.offset(c * W, r * H);
                rectg.inflate(-0.07f * W, -0.1f * H);

                gboxs[i].rect = rectg.copy();
            }
        }
        public bool OnMouseMove(kvec pos)
        {
            int indexOld = IndexHover;
            IndexHover = -1;

            JustEntered = false;
            JustLeaved = false;

            for (int i = 0; i < gboxs.Count; i++)
            {
                gboxs[i].hovered = false;
            }
            for (int i = 0; i < gboxs.Count; i++)
            {
                if (!gboxs[i].visible) continue;

                if (gboxs[i].Contains(pos))
                {
                    IndexHover = i;
                    break;
                }
            }

            // 같은 박스위 하버
            if (IndexHover > -1 && IndexHover == indexOld)
            {
                gboxs[IndexHover].hovered = true;
                return false;
            }
            // entered
            else if (IndexHover > -1 && IndexHover != indexOld)
            {
                gboxs[IndexHover].hovered = true;
                JustEntered = true;
                return true;
            }
            // leaved
            else if (IndexHover == -1 && indexOld > -1)
            {
                JustLeaved = true;
                return true;
            }
            else
                return false;
        }
        public bool OnMouseDown(kvec pos)
        {
            IndexDown = -1;//초기화

            for (int i = 0; i < gboxs.Count; i++)
            {
                gboxs[i].pressed = false;
            }

            for (int i = 0; i < gboxs.Count; i++)
            {
                if (!gboxs[i].visible) continue;
                if (gboxs[i].Contains(pos))
                {
                    gboxs[i].pressed = true;
                    IndexDown = i;
                    return true;
                }
            }

            return false;
        }
        public bool OnMouseUp(kvec pos)
        {
            IndexUp = -1;//초기화
          
            for (int i = 0; i < gboxs.Count; i++)
            {
                gboxs[i].released = false;
            }

            for (int i = 0; i < gboxs.Count; i++)
            {
                if (!gboxs[i].visible) continue;
                if (gboxs[i].Contains(pos))
                {
                    gboxs[i].pressed = false;
                    gboxs[i].released = true;
                    IndexUp = i;
                    return true;
                }
            }

            return false;
        }
        public virtual void Paint(Graphics g)
        {
            Font font = new Font(new FontFamily("맑은 고딕"), FontSize);

            for(int i = 0;i < gboxs.Count;i++)
            {
                if (!gboxs[i].visible) continue;
                GBox gbox = gboxs[i];

                SolidBrush brBack = new SolidBrush(gbox.colorBack);
                SolidBrush brText = new SolidBrush(gbox.colorText);

                if(gbox.hovered)
                {
                    //brText = new SolidBrush(Color.PaleVioletRed);
                    brBack = new SolidBrush(Color.SeaGreen);
                }
                if (gbox.pressed)
                {
                    brBack = new SolidBrush(Color.Brown); 
                }
                if (gbox.released)
                {
                    brBack = new SolidBrush(Color.DarkGray);
                    //brText = new SolidBrush(Color.Black);
                }

                g.FillRectangle(brBack, gbox.rect.R);

                krect recT = gbox.rect.copy();
                recT.offset(10, 5);

                g.DrawString(gbox.title, font, brText, recT.R);
            }
        }
        public void resetStates()
        {
            for (int i = 0; i < gboxs.Count; i++)
            {
                gboxs[i].pressed = false;
                gboxs[i].released = false;
                gboxs[i].hovered = false;
                gboxs[i].moveG = false;
            }
        }
        public bool Clicked(out GBox gbox, out int indexDown)
        {
            if (IndexDown > -1 && IndexUp > -1)
            {
                if (IndexDown == IndexUp)
                {
                    IndexRecent = IndexDown;

                    gbox = gboxs[IndexDown];

                    indexDown = IndexDown;
                    IndexDown = IndexUp = -1;//초기화 중복하는 듯
                    return true;
                }
            }

            gbox = null;
            indexDown = -1;
            return false;
        }
        public void move(kvec pos)
        {
            rect.x = pos.X;
            rect.y = pos.Y;

            if (rect.Right > rectCanvas.w)
                rect.x = rectCanvas.w - rect.w;

            if (rect.Bottom > rectCanvas.h)
                rect.y = rectCanvas.h - rect.h;


            setRects();
        }
    }
    public class BasePad2
    {
        public string name;
        public krect rect;
        protected krect rectCanvas;

        public List<GBox> gboxs;
        public int IndexDown, IndexUp;
        int indexRecent = -1;
        int indexHover = -1;
        public int IndexHover
        {
            get
            {
                if (indexHover > gboxs.Count - 1)
                    return -1;
                else
                    return indexHover;
            }
            set
            {
                indexHover = value;
            }
        }
        public int IndexRecent
        {
            get
            {
                if (indexRecent > gboxs.Count - 1)
                    return -1;
                else
                    return indexRecent;
            }
            set
            {
                indexRecent = value;
            }
        }
        public int ROWS, COLS;

        public bool JustEntered = false;
        protected bool JustLeaved;
        public bool visible = true;
        public float FontSize;

        public BasePad2(string name, int rows, int cols, float fontsize)
        {
            this.name = name;

            ROWS = rows;
            COLS = cols;
            FontSize = fontsize;

            createGBoxs();

            IndexDown = IndexUp = -1;
        }
        protected virtual void createGBoxs()
        {
           
        }
        public virtual void SetRects(krect rectCanvas)
        {
            this.rectCanvas = rectCanvas.copy();

            rect = new krect(600, 200, 400, 400);

            setRects();
        }
        protected virtual void setRects()
        {
            float W = 100;
            float H = 44;

            krect rect0 = new krect(W, H);
            rect0.x = rect.x;
            rect0.y = rect.y;

            for (int i = 0; i < gboxs.Count; i++)
            {
                krect rectg = rect0.copy();

                int r = i / COLS;
                int c = i % COLS;

                rectg.offset(c * W, r * H);
                rectg.inflate(-0.07f * W, -0.1f * H);

                gboxs[i].rect = rectg.copy();
            }
        }
        public bool OnMouseMove(kvec pos)
        {
            int indexOld = IndexHover;
            IndexHover = -1;

            JustEntered = false;
            JustLeaved = false;

            for (int i = 0; i < gboxs.Count; i++)
            {
                gboxs[i].hovered = false;
            }
            for (int i = 0; i < gboxs.Count; i++)
            {
                if (!gboxs[i].visible) continue;

                if (gboxs[i].Contains(pos))
                {
                    IndexHover = i;
                    break;
                }
            }

            // 같은 박스위 하버
            if (IndexHover > -1 && IndexHover == indexOld)
            {
                gboxs[IndexHover].hovered = true;
                return false;
            }
            // entered
            else if (IndexHover > -1 && IndexHover != indexOld)
            {
                gboxs[IndexHover].hovered = true;
                JustEntered = true;
                return true;
            }
            // leaved
            else if (IndexHover == -1 && indexOld > -1)
            {
                JustLeaved = true;
                return true;
            }
            else
                return false;
        }
        public bool OnMouseDown(kvec pos)
        {
            IndexDown = -1;//초기화

            for (int i = 0; i < gboxs.Count; i++)
            {
                gboxs[i].pressed = false;
            }

            for (int i = 0; i < gboxs.Count; i++)
            {
                if (!gboxs[i].visible) continue;
                if (gboxs[i].Contains(pos))
                {
                    gboxs[i].pressed = true;
                    IndexDown = i;
                    return true;
                }
            }

            return false;
        }
        public bool OnMouseUp(kvec pos)
        {
            IndexUp = -1;//초기화

            for (int i = 0; i < gboxs.Count; i++)
            {
                gboxs[i].released = false;
            }

            for (int i = 0; i < gboxs.Count; i++)
            {
                if (!gboxs[i].visible) continue;
                if (gboxs[i].Contains(pos))
                {
                    gboxs[i].pressed = false;
                    gboxs[i].released = true;
                    IndexUp = i;
                    return true;
                }
            }

            return false;
        }
        public virtual void Paint(Graphics g)
        {
            Font font = new Font(new FontFamily("맑은 고딕"), FontSize);

            for (int i = 0; i < gboxs.Count; i++)
            {
                if (!gboxs[i].visible) continue;
                GBox gbox = gboxs[i];

                SolidBrush brBack = new SolidBrush(gbox.colorBack);
                SolidBrush brText = new SolidBrush(gbox.colorText);

                if (gbox.hovered)
                {
                    //brText = new SolidBrush(Color.PaleVioletRed);
                    brBack = new SolidBrush(Color.SeaGreen);
                }
                if (gbox.pressed)
                {
                    brBack = new SolidBrush(Color.Brown);
                }
                if (gbox.released)
                {
                    brBack = new SolidBrush(Color.DarkGray);
                    //brText = new SolidBrush(Color.Black);
                }

                g.FillRectangle(brBack, gbox.rect.R);

                krect recT = gbox.rect.copy();
                recT.offset(10, 5);

                g.DrawString(gbox.title, font, brText, recT.R);
            }
        }
        public void resetStates()
        {
            for (int i = 0; i < gboxs.Count; i++)
            {
                gboxs[i].pressed = false;
                gboxs[i].released = false;
                gboxs[i].hovered = false;
                gboxs[i].moveG = false;
            }
        }
        public bool Clicked(out GBox gbox, out int indexDown)
        {
            if (IndexDown > -1 && IndexUp > -1)
            {
                if (IndexDown == IndexUp)
                {
                    IndexRecent = IndexDown;

                    gbox = gboxs[IndexDown];

                    indexDown = IndexDown;
                    IndexDown = IndexUp = -1;//초기화 중복하는 듯
                    return true;
                }
            }

            gbox = null;
            indexDown = -1;
            return false;
        }
        public void move(kvec pos)
        {
            rect.x = pos.X;
            rect.y = pos.Y;

            if (rect.Right > rectCanvas.w)
                rect.x = rectCanvas.w - rect.w;

            if (rect.Bottom > rectCanvas.h)
                rect.y = rectCanvas.h - rect.h;


            setRects();
        }
    }
}
