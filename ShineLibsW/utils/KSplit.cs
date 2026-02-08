using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace shine.libs.utils
{
    public static class KSplit
    {
        public static string[] getLines(string file)
        {
            byte[] data = File.ReadAllBytes(file);
            string content = Encoding.UTF8.GetString(data, 0, data.Length);

            return Regex.Split(content, "\n");
        }
        public static bool SplitLine(string line, ref string[] ss, int nSplit)
        {
            line = line.Trim();

            if (line.Length == 0)
                return false;
            if (line.Substring(0, 1).Equals("#"))
                return false;

            ss = Regex.Split(line, " .. ");
            if (ss.Length != nSplit)
                ss = Regex.Split(line, "\t");

            if (nSplit == -1)
                return true;
            else
                return ss.Length == nSplit;
        }
    }
}