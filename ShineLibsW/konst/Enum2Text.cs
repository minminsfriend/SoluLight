using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Android.Views;

namespace shine.libs.konst
{
    public static class TextEnum
    {
        public static void SaveValTexts(string fileList, List<string> eTexts, List<int> eVals)
        {
            string content = "";
            content += string.Format("# keyvals count\t{0}", eTexts.Count);
            content += "\n";

            for (int i = 0; i < eTexts.Count; i++)
            {
                content += string.Format("{0:d03}\t{1}\t{2}", i, eVals[i], eTexts[i]);

                if (i < eTexts.Count - 1)
                    content += "\n";
            }

            byte[] data = Encoding.UTF8.GetBytes(content);

            FileStream fw = new FileStream(fileList, FileMode.Create, FileAccess.Write);
            fw.Write(data, 0, data.Length);
            fw.Close();
        }
        public static void GetEnumVals(Type xnumType, ref List<string> eTexts, ref List<int> eVals)
        {
            string[] numTexts = Enum.GetNames(xnumType);
            int[] numVals = (int[])Enum.GetValues(xnumType);

            for (int i = 0; i < numTexts.Length; i++)
            {
                eTexts.Add(numTexts[i]);
                eVals.Add(numVals[i]);
            }
        }
        public static void ReadValTexts(string fileList, ref string[] numTexts, ref int[] numVals)
        {
            string[] lines = File.ReadAllLines(fileList, Encoding.UTF8);
            string[] ss;
            int count = 0;
            int n = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length == 0)
                    continue;

                ss = Regex.Split(line, "\t");

                if (i == 0)
                {
                    count = int.Parse(ss[1]);
                    numTexts = new string[count];
                    numVals = new int[count];

                    continue;
                }
                if (ss.Length != 3)
                    continue;

                n = int.Parse(ss[0]);
                numVals[n] = int.Parse(ss[1]);
                numTexts[n] = ss[2];
            }
        }
    }
}
