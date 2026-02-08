using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Windows.Input;
using System.IO;
using System.Windows.Markup;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using shine.libs.math;

namespace Bacro
{
    class dReceiver
    {
        const int CHUNK1024 = 1024;

        string XCLIENT = "코드컴";

        public bool SHIFT = false;
        public bool CTRL = false;
        public bool ALT = false;

        Dictionary<string, Socket> sockets;

        DataBank bank;
        Bacro main;
        Bitmap CapImage;
        Socket socket;

        string ip = "192.168.1.13";
        int port = 3300;

        bool EnableListen = false;
        public dReceiver(Bacro main)
        {
            this.main = main;
            bank = new DataBank(2 * 1000 * 1000);

            if (File.Exists(@"c:\Works\ClientA.txt"))
                XCLIENT = "코드컴";
            else
                XCLIENT = "작업컴";
        }
        public void connect(string serverip,int port)
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Close();
                    Thread.Sleep(1000);
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //리시브타임아웃을 설정하면 에러가 발생한다
                //센드타임아웃은 상관이 없다.
                //client.ReceiveTimeout = 15000;
                socket.SendTimeout = 5000;
                //client.Connect(ip, port);

                var ep = new IPEndPoint(IPAddress.Parse(serverip), port);
                socket.Connect(ep);

                if (socket.Connected)
                {
                    var message = $"고객이름::{XCLIENT}";
                    Send1024(message, "#s");

                    EnableListen = true;

                    new Thread(ListenServerMessage).Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void Close()
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    var closemessage = $"연결중단::{XCLIENT}";
                    //sendMessage(closemessage, "s");
                }

                Thread.Sleep(1000);

                if (socket != null && socket.Connected)
                {
                    socket.Close();
                    Console.WriteLine("socket.Close();");
                }
                else
                {
                    Console.WriteLine("이미 끊어진 소켓!");
                }

                //socket = null;
            }
            catch (Exception e)
            {
                Console.WriteLine("에러!! socket.Close()");
                Console.WriteLine($"{e.ToString()}");
            }
        }
        void ListenServerMessage()
        {
            bool ShowMessage = true;

            byte[] rdata = new byte[CHUNK1024];

            while (EnableListen && socket.Connected)
            {
                int dataLen = 0;
                if (!Receive1024(ref rdata, ref dataLen))
                    continue;

                var hcode = Encoding.UTF8.GetString(rdata, 0, 2);
                var message = Encoding.UTF8.GetString(rdata, 6, dataLen);
                Mess m = new Mess(hcode, message);

                switch(m.hcode)
                {
                    case "#s":
                    case "#m":
                    case "#c":
                        if (ShowMessage)
                            Console.WriteLine($"[{m.hcode}] {m.message}");
                        break;
                }

                if (m.hcode == "#s")
                {
                    
                }
                else if (m.hcode == "#m")
                    main.socket_gotMessage(m);

                else if (m.hcode == "#c")
                    main.socket_gotMessage(m);

                else if (m.hcode == "#i")
                {
                    int imgSize;
                    if (int.TryParse(m.content, out imgSize))
                    {
                        //recvImageData(rdata, imgSize, m.title);
                        Send1024($"그림받음:: (매크로) {imgSize}", "#m");
                    }
                }
            }

            //Close();
            Console.WriteLine("ListenDho, 소켓 끊음.");
        }
        bool Send1024(string message, string hcode)
        {
            if (socket == null || !socket.Connected)
                return false;

            byte[] data = Encoding.UTF8.GetBytes(message);
            int dataLen = Math.Min(CHUNK1024, data.Length);

            byte[] hdata = buildHeader6(hcode, dataLen);
            byte[] sdata = buildChunk1024(hdata, data, dataLen);

            socket.Send(sdata, 0, CHUNK1024, SocketFlags.None);

            return true;
        }
        bool Receive1024(ref byte[] rdata, ref int dataLen)
        {
            dataLen = 0;

            try
            {
                int readlen = socket.Receive(rdata, 0, CHUNK1024, SocketFlags.None);

                if (readlen < CHUNK1024)
                {
                    var rem = CHUNK1024 - readlen;
                    var len = socket.Receive(rdata, readlen, rem, SocketFlags.None);
                    readlen += len;
                }
                if (readlen < CHUNK1024)//한번더
                {
                    var rem = CHUNK1024 - readlen;
                    var len = socket.Receive(rdata, readlen, rem, SocketFlags.None);
                    readlen += len;
                }

                if (readlen < CHUNK1024)//결국 실패
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"socket.Receive(Receive1024) 에러: {e}");

                return false;
            }

            var lenx = Encoding.UTF8.GetString(rdata, 2, 4);
            int datalen;
            if (int.TryParse(lenx.Trim(), out datalen))
            {
                dataLen = datalen;
                return true;
            }

            return false;
        }
        byte[] buildHeader6(string hcode, int dataLen)
        {
            byte[] hdata = new byte[6];

            var header = Encoding.UTF8.GetBytes($"{hcode}{dataLen:d04}");
            Array.Copy(header, 0, hdata, 0, header.Length);//header.Length<=6

            return hdata;
        }
        byte[] buildChunk1024(byte[] hdata, byte[] data, int dataLen)
        {
            byte[] sdata = new byte[CHUNK1024];

            Array.Copy(hdata, 0, sdata, 0, 6);
            Array.Copy(data, 0, sdata, 6, dataLen);
            return sdata;
        }
        internal void Test(string cate, string command)
        {
            var msg = $"통신연습 // {cate}::{command}";

            Send1024(command, "#m");
        }
    }
}
