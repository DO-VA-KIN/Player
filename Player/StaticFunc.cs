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
            //возвращает короткое имя без расширения, получая путь
            int end = way.LastIndexOf(".");
            if (end == -1)
                end = way.Length;
            int beg = way.LastIndexOf("\\");

            way = way.Remove(end, way.Length - end);
            way = way.Remove(0, beg + 1);
            return way;
        }


        public static string createFilterStringSFD(List<string> extension)
        {
            //создает строку фильтра SaveFileDialog/OpenFileDialog , переберая массив допустимых расширений; ниже примеры написания в ручную

            //sfd.Filter = "cnt64 files (*.cnt64)|*.cnt64|All files (*.*)|*.*";
            //string filter = "music files (*" + musicExtension[0] +" *" + musicExtension[1] +")|*" + musicExtension[0] +"; *" + musicExtension[1];

            string filter = "valid files (";
            foreach (string item in extension)
                filter += "*" + item + " ";
            filter += ")|";
            foreach (string item in extension)
                filter += "*" + item + "; ";
            filter = filter.Remove(filter.Length - 1);


            return filter;
        }


        public static void checkAndCreateDirectory(string directory)
        {
            // проверяет существование директории и если ее не существует - создает
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
        }


        public static void killProcessByName(string sName, string procType)
        {
            //находит и убивает процесс по имени(title) и типу процесса(например "Notepad")
            Process[] proc = Process.GetProcessesByName(procType);
            foreach (Process process in proc)
            {
                if (StaticFunc.parseWay(process.MainWindowTitle) == sName)
                    process.Kill();
            }
        }
    }
}
