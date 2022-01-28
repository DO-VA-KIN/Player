using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    public static class StaticFunc
    {

        public static string parseWay(string way)
        {
            int end = way.LastIndexOf(".");
            if (end == -1)
                end = way.Length;
            int beg = way.LastIndexOf("\\");

            way = way.Remove(end, way.Length - end);
            way = way.Remove(0, beg + 1);
            return way;
        }


        public static void checkAndCreateDirectoryExist(string directory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
        }


        public static void killProcessByName(string sName, string procType)
        {
            Process[] procFile = Process.GetProcessesByName(procType);
            foreach (Process process in procFile)
            {
                if (StaticFunc.parseWay(process.MainWindowTitle) == sName)
                    process.Kill();
            }
        }
    }
}
