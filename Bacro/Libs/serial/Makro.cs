#define PLAYON3770
using shine.libs.capture;
using shine.libs.hangul;
using shine.libs.math;
using shine.libs.serial;
using shine.libs.simul;
using shine.libs.window;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static shine.libs.capture.KPacket.Keyvals;
using static System.Windows.Forms.LinkLabel;
using Encoding = System.Text.Encoding;

namespace Bacro
{
    public class Makro
    {
        SerialD serialD;
        Bacro main;
        //CapDho capDho;

        public Makro(Bacro main)
        {
            this.main = main;
            serialD = new SerialD(this);
        }
        internal void PrintCursor()
        {
            main.capDho.printCursor();
        }
        internal Bitmap requestImage(krect rectDhoSel)
        {
            krect rectCap = rectDhoSel.copy();
            rectCap.offset(main.capDho.RectDho.pos());

            Bitmap bmp = ScreenCopy.GetImage(rectCap);

            //Console.WriteLine($"{bmp.Width} {bmp.Height}");

            return bmp;
        }
        internal void KeyLongClick(string mkeyx, string keyx, int sleepPressed, int sleepEnd)
        {
            serialD.KeyLongClick(mkeyx, keyx, sleepPressed);
            Thread.Sleep(sleepEnd);
        }
        internal void KeysClick(string mkeyx, string keyx, int sleepIn, int sleepEnd)
        {
            serialD.KeysClick(mkeyx, keyx, sleepIn);
            Thread.Sleep(sleepEnd);
        }
        internal void KeyClick(string mkeyx, string keyx, int sleepEnd)
        {
            serialD.KeyClick(mkeyx, keyx);
            Thread.Sleep(sleepEnd);
        }
        internal void KeyClickX5(string keyx, int sleepEnd, int count)
        {
            serialD.KeyClickX5(keyx, sleepEnd, count);
            Thread.Sleep(sleepEnd);
        }
        internal void MouseMove(kvec posInDho, kvec locDho)
        {
            kvec pos = posInDho.copy();
            pos.offset(locDho);
            serialD.MouseMove(pos.X, pos.Y);
        }
        internal void MouseClickX(string button, string option)
        {
           serialD.MouseClickX(button, option);
        }
        internal void MouseAct(string button, string action)
        {
            serialD.MouseAct(button, action);
        }
        internal void MouseOffset(int dx, int dy)
        {
            serialD.MouseOffset(dx, dy);
        }
        internal void KeyAct(string action, string keyx)
        {
            serialD.KeyAct(action, keyx);
        }
        internal void ClickBarAppDho(int offX, int offY)
        {
            if (!main.capDho.DhoExists)
                return;

            main.capDho.FocusWorkDho();

            kvec posTar = main.capDho.RectDho.pos().copy();
            posTar.offset(offX, offY);

            serialD.MouseMove(posTar.X, posTar.Y);
            Thread.Sleep(100);

            serialD.MouseClick("left", 10);
            Thread.Sleep(100);
            serialD.MouseClick("left", 10);
            Thread.Sleep(100);
        }
    }
}
