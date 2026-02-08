using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

//using shine.libs.simul;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Threading;
using System.Net.Sockets;

using shine.libs.math;
using shine.libs.simul;

namespace shine.libs.serial
{
    public class MouseSik
    {
        public string button;
        public string action;
        public kvec pos;
        public int key;
        public int delta;

        public MouseSik(string mm)
        {
            // LD09001200
            button = mm.Substring(0, 1);
            action = mm.Substring(1, 1);

            switch (button)
            {
                case "L": button = "left"; break;
                case "R": button = "right"; break;
                case "M": button = "middle"; break;
                case "K": button = "key"; break;
                case "N": button = "none"; break;
            }
            switch (action)
            {
                case "D": action = "down"; break;
                case "U": action = "up"; break;
                case "M": action = "move"; break;
                case "W": action = "wheel"; break;
            }

            var x = mm.Substring(2, 4);
            var y = mm.Substring(6, 4);

            pos = new kvec(int.Parse(x), int.Parse(y));
            
            key = button == "key" ? int.Parse(x) : 0;
            delta = action == "wheel" ? int.Parse(y) - 1000 : 0;
        }
    }
    public class SerialPut
    {
        float RealScale = 2f;

        SerialPort serial = new SerialPort();
        string PortName = "COM11";
        string fileconfig;
        public SerialPut(string fileconfig)
        {
            this.fileconfig = fileconfig;

            PortName = null;
            readConfig();

            if (PortName != null)
                connect();
        }
        public SerialPut(string portname, float realscale)
        {
            PortName = portname;
            RealScale = realscale;

            Console.WriteLine($"PortName == {PortName}");
            Console.WriteLine($"RealScale == {RealScale:f01}");

            if (PortName != null)
                connect();
        }
        void readConfig()
        {
            if (!File.Exists(fileconfig))
                return;

            var lines = File.ReadAllLines(fileconfig, Encoding.UTF8);

            foreach (var linex in lines)
            {
                var line = linex.Trim();
                if (line.Length < 2) continue;
                if (line.Substring(0, 2) == "//") continue;

                var ss = Regex.Split(line, "\t");

                if (ss.Length != 2)
                    continue;

                switch (ss[0])
                {
                    case "PortName":
                        PortName = ss[1].Trim(); 
                        break;
                    case "RealScale":
                        RealScale = float.Parse(ss[1].Trim());
                        break;
                }

                Console.WriteLine(line);
            }
        }
        void connect()
        {
            try
            {
                if (serial.IsOpen)
                {
                    serial.Close();
                }
                else
                {
                    serial.PortName = PortName; // "COM7"
                    serial.BaudRate = 9600;
                    serial.DataBits = 8;
                    serial.StopBits = StopBits.One;
                    serial.Parity = Parity.None;
                    serial.DtrEnable = true;

                    serial.DataReceived += serial_DataReceived;

                    serial.Open();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("연결에러", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string message = serial.ReadExisting().Trim();

            Console.WriteLine($"message == [{message}]");
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
        public void KeyAct(Keys key, string action)
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
                        keyActWrite(AActions.KeyPress, akey, ArduKeys.NONE);
                    else if (action == "up")
                        keyActWrite(AActions.KeyRelease, akey, ArduKeys.NONE);
                    
                    break;
                default://if (action == "down") "down"만 있다.
                    if (action == "down")
                    {
                        keyActWrite(AActions.KeyPress, akey, ArduKeys.NONE);
                        Thread.Sleep(5);
                        keyActWrite(AActions.KeyRelease, akey, ArduKeys.NONE);
                    }

                    break;
            }
        }
        public void KeyActMacro(Keys key, string action)
        {
            ArduKeys akey = ToArduKeys.Ardu(key);

            if (action == "down")
                keyActWrite(AActions.KeyPress, akey, ArduKeys.NONE);
            else if (action == "up")
                keyActWrite(AActions.KeyRelease, akey, ArduKeys.NONE);
        }
        public void MouseAct(string action, string button)
        {
            if (button == "left")
            {
                if (action == "down")
                    mouseActWrite(AActions.LMousePress);
                else if (action == "up")
                    mouseActWrite(AActions.LMouseRelease);
            }
            else if (button == "right")
            {
                if (action == "down")
                    mouseActWrite(AActions.RMousePress);
                else if (action == "up")
                    mouseActWrite(AActions.RMouseRelease);
            }
        }
        void keyActWrite(AActions button, ArduKeys key, ArduKeys mkey)
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
        void mouseActWrite(AActions button)
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
            kvec pGoal=new kvec(posx,posy);

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
    }
}

