using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using System.Drawing;

using shine.libs.system;
using shine.libs.math;
using Android.Views;

namespace shine.libs.bpad
{
    public static class TouchMode
    {
        public const int none = 0;
        public const int mouse = 1;
        public const int move = 2;

        public const int point_add = 10;
        public const int point_select = 11;
        public const int point_move = 12;
        public const int path_select = 20;
        public const int path_move = 21;

        public const int button = 30;
        public const int layer = 31;
    }
    public static class DragType
    {
        public const int none = 0;
        public const int slow = 1;
        public const int fast = 2;

    }

    public class TouchData
    {
        public MotionEventActions preAction;
        public kvec posDown, posUp, posMove;
        public bool mouseDowned;
        public long timeDown, timeMove, timeUp, timeCurr;
        public int downPointerMax = 0;

        public int touchMode;
        public bool isCtrl, isShift, isAlt;
        public int keyval;
        public List<kvec> poss;

        public kvec[] muPosDown, muPosMove, muPosUp;
        public int[] muID;
        float dis_lenDown2p_lenMove2p = 0;

        public TouchData()
        {
            posDown = new kvec();
            posUp = new kvec();
            posMove = new kvec();

            preAction = MotionEventActions.Cancel;
            poss = new List<kvec>();

            touchMode = TouchMode.mouse;
            mouseDowned = false;

            isCtrl = isShift = isAlt = false;
            keyval = 0;

            timeDown = timeCurr = KSys.CurrentMillis();
            timeMove = timeUp = timeDown;

            setMultiPos();
        }
        public bool OverThreshold(kvec p, float threshold)
        {
            kvec dv = kvec.sub(posMove, p);
            if (dv.length() > threshold)
            {
                posMove.set(p);
                return true;
            }
            else
                return false;
        }
        public bool OverThreshold2P(float val)
        {
            float len_down_p0_p1 = kvec.sub(muPosDown[0], muPosDown[1]).length();
            float len_move_p0_p1 = kvec.sub(muPosMove[0], muPosMove[1]).length();

            float xdis_lenDown2p_lenMove2p = Math.Abs(len_move_p0_p1 - len_down_p0_p1);

            if (xdis_lenDown2p_lenMove2p > dis_lenDown2p_lenMove2p + val)
            {
                dis_lenDown2p_lenMove2p = xdis_lenDown2p_lenMove2p;
                return true;
            }

            return false;
        }
        public kvec getVec(kvec p)
        {
            return kvec.sub(p, posMove);
        }
        public static Point toPoint(MotionEvent e)
        {
            return new Point((int)e.GetX(), (int)e.GetY());
        }
        public static kvec toVec(MotionEvent e)
        {
            return new kvec(e.GetX(), e.GetY());
        }

        public void setMultiPosDown(MotionEvent e)
        {
            dis_lenDown2p_lenMove2p = 0;

            for (int i = 0; i < e.PointerCount; i++)
            {
                int ID = e.GetPointerId(i);

                muID[i] = ID;
                muPosDown[i] = new kvec(e.GetX(i), e.GetY(i));
            }
        }
        public void setMultiPosMove(MotionEvent e)
        {
            for (int i = 0; i < e.PointerCount; i++)
            {
                int n = getIdIndex(e.GetPointerId(i));
                if (n > -1)
                    muPosMove[n] = new kvec(e.GetX(i), e.GetY(i));
            }
        }
        public void setMultiPosUp(MotionEvent e)
        {
            for (int i = 0; i < e.PointerCount; i++)
            {
                int n = getIdIndex(e.GetPointerId(i));
                if (n > -1)
                    muPosUp[n] = new kvec(e.GetX(i), e.GetY(i));
            }
        }

        int getIdIndex(int id)
        {
            for (int i = 0; i < muID.Length; i++)
            {
                if (muID[i] == id)
                    return i;
            }

            return -1;
        }
        void setMultiPos()
        {
            muPosDown = new kvec[5];
            muPosMove = new kvec[5];
            muPosUp = new kvec[5];
            muID = new int[5];

            for (int i = 0; i < muPosDown.Length; i++)
            {
                muPosDown[i] = new kvec();
                muPosMove[i] = new kvec();
                muPosUp[i] = new kvec();

                muID[i] = i;
            }
        }
    }
}