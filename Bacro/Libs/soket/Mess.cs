using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Bacro
{
    public class Mess
    {
        public string hcode;
        public string message;
        public string title;
        public string content;
        public Mess(string hcode, string message)
        {
            this.hcode = hcode;
            this.message = message;

            if (message != null && message.Length > 0)
            {
                var ss = Regex.Split(message, "::");

                if (ss.Length > 1)
                {
                    title = ss[0];
                    content = ss[1];
                }
            }
        }
    }
}
