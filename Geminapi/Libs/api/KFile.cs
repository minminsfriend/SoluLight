using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geminapi
{
    internal class KFile
    {
        public static List<string> GetFiles(string dir)
        {
            //string dir = @"d:\Works\ApiGemini\texts\01 설정집\pages_설정 요약";

            DirectoryInfo dInfo = new DirectoryInfo(dir);

            // Name 속성을 기준으로 이름순 정렬
            var fileList = dInfo.GetFiles()
                                .OrderBy(f => f.Name)
                                .ToList();

            List<string> files = new List<string>();

            foreach (var file in fileList)
                files.Add(file.Name);

            return files;
        }
        public static List<string> GetListByLastFile(string fileName)
        {
            string d2num = fileName.Substring(fileName.Length - 6, 2);
            string realNude = fileName.Substring(0, fileName.Length - 6);
            var ext = Path.GetExtension(fileName);
            var filesList = Enumerable.Range(0, 8).Select(i => $"{realNude}{i:00}{ext}").ToList();

            return filesList;
        }
    }
}
