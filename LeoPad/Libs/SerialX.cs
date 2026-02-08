//using shine.libs.simul;
using Microsoft.Win32;
using shine.libs.hangul;
using shine.libs.konst;
using shine.libs.math;
using shine.libs.serial;
using shine.libs.simul;
using shine.libs.system;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static shine.libs.capture.KPacket.Keyvals;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace LeoPad
{
    public class SerialX
    {
        public static readonly string FIRSTKEY = "firstKey";
        public static readonly string SECONDKEY = "secondKey";

        SerialPort serial;
        string PortName = "COM9";
        float RealScale = 2.0f;
        string fileCfg;
        LeoPad main;

        public SerialX(LeoPad main)
        {
            this.main = main;
            fileCfg = $@"{main.dirData}\config.txt";
            readConfig();

            serial = new SerialPort();
            //serial.DataReceived += serial_DataReceived;
            ConnectSerial();
        }
        void readConfig()
        {
            if (fileCfg != null)
            {
                var lines = File.ReadAllLines(fileCfg, Encoding.UTF8);

                foreach (var linex in lines)
                {
                    var line = linex.Trim();
                    if (line.Length == 0)
                        continue;

                    var ss = Regex.Split(line, " ");

                    if (ss.Length >= 2)
                    {
                        if (ss[0] == "PortName")
                            PortName = ss[1].Trim();
                    }
                }
            }
        }
        void ConnectSerial()
        {
            try
            {
                if (serial.IsOpen)
                {
                    serial.Close();
                    Thread.Sleep(1000);
                }

                if (!serial.IsOpen)
                {
                    serial.PortName = PortName; // "COM9"
                    serial.BaudRate = 9600;
                    serial.DataBits = 8;
                    serial.StopBits = StopBits.One;
                    serial.Parity = Parity.None;
                    serial.DtrEnable = true;

                    serial.DataReceived += serial_DataReceived;
                    serial.Open();
                    Console.WriteLine($"시리얼 [{PortName}] 연결!!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("연결에러", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);

                getSerialList();
            }
        }
        void getSerialList()
        {
        }
        void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string message = serial.ReadExisting().Trim();

            message = message.Replace("\r\n", "\n");
            //message= message.Replace("\n", " ");

            var ss = Regex.Split(message, "\n");

            foreach (var sx in ss)
            {
                var s = sx.Trim();
                if (s.Contains("83"))
                    continue;
                if (s.Length == 0)
                    continue;

                Console.WriteLine($"<{s}>");

                var nn = Regex.Split(s.Trim(), " ");
                var action = nn[1];

                int hkey;

                if (int.TryParse(nn[2], out hkey))
                    main.serial_doSomething(action, hkey);
            }
        }
        kvec getCursor()
        {
            MouseSimul.MousePoint mp = MouseSimul.GetCursorPosition();
            return new kvec(mp.X, mp.Y);
        }
        byte[] getSerialMoveData(kvec dis)
        {
            //var offlen = dis.length();
            //RealScale = 2.5f;

            //if (offlen > 100)
            //    RealScale = 2.0f; 
            //else if (offlen > 50)
            //    RealScale = 1.5f; 
            //else 
            //    RealScale = 1.0f;

            float RealLimit = 127 * RealScale; // 화면에서 시리얼로 움직일 수 있는 최대값

            dis.x = dis.x < 0f ? Math.Max(dis.x, -RealLimit) : Math.Min(dis.x, RealLimit);
            dis.y = dis.y < 0f ? Math.Max(dis.y, -RealLimit) : Math.Min(dis.y, RealLimit);

            dis = (1f / RealScale) * dis; // dis.x, dis.y 모두 127.0 이하로 맞춘다.

            byte[] data = new byte[8];
            for (int i = 0; i < data.Length; i++)
                data[i] = 0;

            data[0] = 4;//사용된 데이타 길이
            data[1] = (byte)AActions.MouseMove;
            data[2] = (byte)(dis.X + 127);
            data[3] = (byte)(dis.Y + 127);//아두이노 에서 -127 해서 처리한다.
            return data;
        }
        public void KeyActX(Keys key, string action)
        {
            ArduKeys akey = ToArduKeys.Ardu(key);

            switch (key)
            {
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.ShiftKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.ControlKey:
                case Keys.LMenu:
                case Keys.RMenu:
                case Keys.Menu:
                    if (action == "down")
                        KeyActWrite(AActions.KeyPress, akey, ArduKeys.NONE);
                    else if (action == "up")
                        KeyActWrite(AActions.KeyRelease, akey, ArduKeys.NONE);

                    break;
                default://if (action == "down") "down"만 있다.
                    if (action == "down")
                    {
                        KeyActWrite(AActions.KeyPress, akey, ArduKeys.NONE);
                        Thread.Sleep(5);
                        KeyActWrite(AActions.KeyRelease, akey, ArduKeys.NONE);
                    }

                    break;
            }
        }
        public void KeyAct(Keys key, string action)
        {
            ArduKeys akey = ToArduKeys.Ardu(key);

            if (action == "down")
                KeyActWrite(AActions.KeyPress, akey, ArduKeys.NONE);
            else if (action == "up")
                KeyActWrite(AActions.KeyRelease, akey, ArduKeys.NONE);
        }
        public void MouseAct(string action, string button)
        {
            if (button == "left")
            {
                if (action == "down")
                    MouseActWrite(AActions.LMousePress);
                else if (action == "up")
                    MouseActWrite(AActions.LMouseRelease);
            }
            else if (button == "right")
            {
                if (action == "down")
                    MouseActWrite(AActions.RMousePress);
                else if (action == "up")
                    MouseActWrite(AActions.RMouseRelease);
            }
        }
        public void KeyActWrite(AActions button, ArduKeys key, ArduKeys mkey)
        {
            byte[] data = new byte[8];
            for (int i = 0; i < data.Length; i++)
                data[i] = 0;

            if (mkey == ArduKeys.NONE)
            {
                data[0] = 3;//uselen
                data[1] = (byte)button;
                data[2] = (byte)key;
            }
            else
            {
                data[0] = 4;//uselen
                data[1] = (byte)button;
                data[2] = (byte)key;
                data[3] = (byte)mkey;
            }

            serial.Write(data, 0, data.Length);
        }
        public void MouseActWrite(AActions button)
        {
            byte[] data = new byte[8];
            for (int i = 0; i < data.Length; i++)
                data[i] = 0;

            data[0] = 2;//uselen
            data[1] = (byte)button;

            serial.Write(data, 0, data.Length);
        }
        public void MouseWheel(int delta)
        {
            byte[] data = new byte[8];
            for (int i = 0; i < data.Length; i++)
                data[i] = 0;

            data[0] = 3;//uselen
            data[1] = (byte)AActions.Wheel;
            data[2] = (byte)(delta / 120 + 127);

            serial.Write(data, 0, data.Length);
        }
        public bool MouseMove(int posx, int posy)
        {
            kvec pGoal = new kvec(posx, posy);

            byte[] data;

            kvec remToGoal = pGoal - getCursor();

            int moveSpent = 0;
            int SLEEP_2 = 2;

            bool actionBreak = false;

            while (remToGoal.length() > 3f)
            {
                //Console.WriteLine($"  {remToGoal.toString2()}");

                data = getSerialMoveData(remToGoal);
                serial.Write(data, 0, data.Length);

                Thread.Sleep(SLEEP_2);
                remToGoal = pGoal - getCursor();// for next compare

                moveSpent += SLEEP_2;
                if (moveSpent > 1000 * 2)//1무브를 2초에 못끝내면 포기
                {
                    actionBreak = true;
                    break;
                }
            }

            if (actionBreak)
            {
                Console.WriteLine("move 브레이크, 2초 초과!");
                return false;
            }
            else
            {
                //Console.WriteLine("----< move 완수! >----");
                return true;
            }
        }
        public void MouseOffset(float disX, float disY)
        {
            var pos0 = getCursor();
            var posTar = pos0 + new kvec(disX, disY);

            MouseMove(posTar.X, posTar.Y);
        }
        public void MouseOffset00(float disX, float disY)
        {
            byte[] data = new byte[8];
            for (int i = 0; i < data.Length; i++)
                data[i] = 0;

            data[0] = 4;//사용된 데이타 길이
            data[1] = (byte)AActions.MouseMove;
            data[2] = (byte)(disX / RealScale + 127);
            data[3] = (byte)(disY / RealScale + 127);//아두이노 에서 -127 해서 처리한다.

            serial.Write(data, 0, data.Length);

            //Console.WriteLine($"mouse offset == {data[2]} {data[3]}");
        }
        internal void MouseClick(string button, int sleepPressed)
        {
            MouseAct("down", button);
            Thread.Sleep(sleepPressed);
            MouseAct("up", button);
        }
        public void keyClick(Keys mkey, Keys key, int sleepPressed)
        {
            if (mkey == Keys.None)
            {
                KeyAct(key, "down");
                Thread.Sleep(sleepPressed);
                KeyAct(key, "up");
            }
            else
            {
                KeyAct(mkey, "down");
                Thread.Sleep(10);

                KeyAct(key, "down");

                /* 길게 눌러 줘야 한다 */
                Thread.Sleep(sleepPressed);

                KeyAct(key, "up");
                Thread.Sleep(10);

                KeyAct(mkey, "up");
            }
        }
        void MouseClickX(string button, string option)
        {
            //Console.WriteLine($"{action} {button}");

            if (option == "click")
            {
                MouseAct("down", button);// left 또는 right
                                         //Thread.Sleep(5);
                Thread.Sleep(10);
                MouseAct("up", button);
            }
            else if (option == "ctrl-click")
            {
                KeyAct(Keys.LControlKey, "down"); Thread.Sleep(5);
                MouseAct("down", "left"); Thread.Sleep(5);
                MouseAct("up", "left"); Thread.Sleep(5);
                KeyAct(Keys.LControlKey, "up"); Thread.Sleep(5);
            }
            else if (option == "double-click")
            {
                MouseAct("down", "left"); Thread.Sleep(5);
                MouseAct("up", "left"); Thread.Sleep(5);
                MouseAct("down", "left"); Thread.Sleep(5);
                MouseAct("up", "left"); Thread.Sleep(5);
            }
        }
        public void KeysRemote(string commandx)
        {
            var nullxprint = commandx.Replace("null", "__");
            Console.WriteLine($"☞  {nullxprint}");

            var ss = Regex.Split(commandx, " ");
            for (int i = 0; i < ss.Length; i++)
                ss[i] = ss[i].Trim();

            var command = ss[0];

            /* "KeyAct {action} {key}" */
            if (command == "KeyAct")
            {
                var action = ss[1];
                var key = MacroKeys.parseKey(ss[2]);
                KeyAct(key, action);
            }
            /* "KeyClick {mkeyx} {keyx}" */
            else if (command == "KeyClick")
            {
                var mkey = MacroKeys.parseKey(ss[1]);
                var key = MacroKeys.parseKey(ss[2]);

                /* 길게 눌러 줘야 한다 */
                int sleepPressed = mkey != Keys.None ? 200 : 10;

                keyClick(mkey, key, sleepPressed);
            }
            /* "KeyLongClick {mkeyx} {keyx} {sleepPressed}" */
            else if (command == "KeyLongClick")
            {
                var mkey = MacroKeys.parseKey(ss[1]);
                var key = MacroKeys.parseKey(ss[2]);
                var sleepPressed = int.Parse(ss[3]);

                keyClick(mkey, key, sleepPressed);
            }
            /* "KeysClick {mkeyx} {keyx} {sleepIn}" */
            else if (command == "KeysClick")
            {
                var mkey = MacroKeys.parseKey(ss[1]);
                var keys = MacroKeys.parseKeys(ss[2]);
                var sleepIn = int.Parse(ss[3]);
                int sleepPressed = mkey != Keys.None ? 100 : 10;

                int n = -1;
                foreach (var key in keys)
                {
                    keyClick(mkey, key, sleepPressed);

                    if (++n < keys.Count - 1)
                        Thread.Sleep(sleepIn);
                }
            }
            /* "KeyClickX5 {keyx} {sleepIn} {count}" */
            else if (command == "KeyClickX5")
            {
                var key = MacroKeys.parseKey(ss[1]);
                var sleepIn = int.Parse(ss[2]);
                var count = int.Parse(ss[3]);
                var k = 0;

                while (++k <= count)
                {
                    keyClick(Keys.None, key, 10);
                    Thread.Sleep(sleepIn);
                }
            }
            /* "MouseAct {button} {action}" */
            else if (command == "MouseAct")
            {
                var button = ss[1];
                var action = ss[2];
                MouseAct(action, button);
            }
            /* "MouseMove {x} {y}" */
            else if (command == "MouseMove")
            {
                var x = int.Parse(ss[1]);
                var y = int.Parse(ss[2]);

                MouseMove(x, y);
            }
            /* "MouseClick {button} {sleepPressed}" */
            else if (command == "MouseClick")
            {
                var button = ss[1];
                var sleepPressed = int.Parse(ss[2]);

                MouseClick(button, sleepPressed);
            }
            /* "MouseClickX {button} {option}" */
            else if (command == "MouseClickX")
            {
                var button = ss[1];
                var option = ss[2];
                MouseClickX(button, option);
            }
            /* "MouseOffset {dx} {dy}" */
            else if (command == "MouseOffset")
            {
                var dx = int.Parse(ss[1]);
                var dy = int.Parse(ss[2]);

                MouseOffset(dx, dy);
            }
        }
        internal void Close()
        {
            if (serial != null && serial.IsOpen)
            {
                serial.Close();
            }
        }
    }
}
