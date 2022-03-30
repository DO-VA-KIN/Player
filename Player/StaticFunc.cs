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



        public static string createFilterStringSFD(List<string> extensions)
        {
            //создает строку фильтра SaveFileDialog/OpenFileDialog , переберая стринговый лист допустимых расширений

            string filter = "";

            filter += "Valid Files (";
            foreach (string item in extensions)
                filter += "*" + item + " ";
            filter += ")|";
            foreach (string item in extensions)
                filter += "*" + item + ";";
            filter = filter.Remove(filter.Length - 1);

            return filter;
        }
        public static string createFilterSFD(Dictionary<string, List<string>> extensionsDict)
        {
            //создает фильтр SaveFileDialog/OpenFileDialog , 
            //переберая словарь(Ключ - произвольное имя, Значение - лист стринг с расширениями) допустимых расширений; пример вызова ниже

            //List<string> musicExtension = new List<string>() { ".wav", ".mp3" };
            //List<string> videoExtension = new List<string>() { ".mp4" };
            //Dictionary<string, List<string>> extensionsDict = new Dictionary<string, List<string>>();
            //extensionsDict.Add("music files ", musicExtension);
            //extensionsDict.Add("video files ", videoExtension);
            //sfd.Filter = StaticFunc.createFilterSFD(extensionsDict);

            string filter = "";
            foreach (KeyValuePair<string, List<string>> pair in extensionsDict)
            {
                filter += pair.Key + " (";
                foreach (string item in pair.Value)
                    filter += "*" + item + " ";
                filter += ")|";
                foreach (string item in pair.Value)
                    filter += "*" + item + ";";
                filter = filter.Remove(filter.Length - 1);

                filter += "|";
            }
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
