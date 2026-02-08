using System;
using System.Net.Sockets;
using shine.libs.utils;

namespace shine.libs.socket
{
    static class __LOG__
    {
        // socket send
        public static string CLAS;

        public static void SendMessage(bool doWrite, string msgToSend)
        {
            if (doWrite)
                xLog.d(CLAS, string.Format(
                                    "<message to send> {0}", msgToSend));
        }
        public static void NotConnected(bool doWrite)
        {
            if (doWrite)
                xLog.d(CLAS, string.Format("Not Connected!"));
        }
        // socket receive
        public static void fromServer(bool doWrite, string text)
        {
            if (doWrite)
                xLog.d(CLAS, string.Format("<from server>: {0}", text));
        }
        public static void HeaderMessage(bool doWrite, string headerMessage)
        {
            if (doWrite)
                xLog.d(CLAS, string.Format(
                    "<header message> == {0}", headerMessage));
        }
        public static void ErrorHeader(bool doWrite, int READ_LEN, int BSIZE)
        {
            if (doWrite)
                xLog.d(CLAS, string.Format(
                    "<head error>: ReadLen/Buffer == {0}/{1}", READ_LEN, BSIZE));
        }
      
        public static void Katch(Socket socketRdp, Exception e)
        {
            //throw new NotImplementedException();
        }
    }
}