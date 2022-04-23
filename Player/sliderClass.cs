using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class sliderClass
    {
        public static double oldValue = 0;
        public static bool retValueChanged = false;


        public static void refresh()
        {
            oldValue = 0;
            retValueChanged = false;
        }


        public static string timeForLabel(TimeSpan mediaPos)//вернет время для лейбла
        {
            double d = Math.Round(mediaPos.TotalSeconds, 1);
            TimeSpan ts = TimeSpan.FromSeconds(d);
            string str = ts.ToString();

            int lInd = str.LastIndexOf(".");
            if (lInd != -1)
                str = str.Remove(lInd, str.Length - lInd);
            return str;
        }

        public static double sliderValueCalculate(double posTotalSec, double durationTotalSec, double sliderValueMax)
        {
            double sliderValue = posTotalSec / durationTotalSec * sliderValueMax;
            oldValue = sliderValue;

            return sliderValue;
        }

        public static TimeSpan mediaPosCalculate(double durationTotalSec, double sliderValue, double sliderValueMax)
        {
            double factor = (sliderValue / sliderValueMax);
            double totalSeconds = durationTotalSec * factor;

            int DurationHours = (int)((totalSeconds / 3600));
            totalSeconds -= (DurationHours * 3600);
            int DurationMin = (int)((totalSeconds / 60));
            totalSeconds -= (DurationMin * 60);
            int DurationSec = (int)(totalSeconds);

            oldValue = sliderValue;

            return new TimeSpan(DurationHours, DurationMin, DurationSec);
        }
    }
}
