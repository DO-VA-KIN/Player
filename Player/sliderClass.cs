using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    class sliderClass
    {
        public static double[] oldValues = new double[2] { 0, 0 };
        private static bool flipFlop = true;


        public static string timeForLabel(string mediaPos)//вернет время для лейбла
        {
            int lInd = mediaPos.LastIndexOf(".");
            if (lInd != -1)
                mediaPos = mediaPos.Remove(lInd, mediaPos.Length - lInd);
            return mediaPos;
        }

        public static double sliderValueCalculate(double posTotalSec, double durationTotalSec, double sliderValueMax)
        {
            double sliderValue = posTotalSec / durationTotalSec * sliderValueMax;

            if (flipFlop)
            { oldValues[0] = sliderValue; flipFlop = false; }
            else { oldValues[1] = sliderValue; flipFlop = true; }

            return sliderValue;
        }

        public static TimeSpan mediaPosCalculate(double durationTotalSec, double sliderValue, double sliderValueMax)
        {
            double totalSeconds = durationTotalSec;
            double factor = (sliderValue / sliderValueMax);
            int DurationHours = (int)((totalSeconds / 3600) * factor);
            totalSeconds -= (DurationHours * 3600);
            int DurationMin = (int)((totalSeconds / 60) * factor);
            totalSeconds -= (DurationMin * 60);
            int DurationSec = (int)(totalSeconds * factor);

            if (flipFlop)
            { oldValues[0] = sliderValue; flipFlop = false; }
            else { oldValues[1] = sliderValue; flipFlop = true; }

            return new TimeSpan(DurationHours, DurationMin, DurationSec);
        }
    }
}
