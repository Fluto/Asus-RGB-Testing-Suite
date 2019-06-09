

using System;
using System.Drawing;
using System.Linq;
using System.IO;

namespace AuraDeviceTestSuite
{
    static class Helper
    {

        public static string FolderLoc = @"logs\";
        public static string MainLogFileName = @"log_";
        public static int RainbowSpeed = 5000;
        public static int InputTimeout = 500;

        private static StreamWriter mainLogFile;
        public static StreamWriter MainLogFile => mainLogFile ?? (mainLogFile = new StreamWriter(File.Open(FolderLoc + MainLogFileName + DateTime.Now.ToFileTime() + ".txt", FileMode.Append)));

        public static bool IsValidEnum(Type enumType, string value)
        {
            if (!int.TryParse(value, out var input))
                return false;

            return IsValidEnum(enumType, input);
        }

        public static bool IsValidEnum(Type enumType, int value)
        {
            return ((int[])Enum.GetValues(enumType)).Contains(value);
        }

        public static void ClearLine(int lines = 1)
        {
            for (int i = 1; i <= lines; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
        }

        public static void PrintLine(string line)
        {
            Console.WriteLine(line);
            MainLogFile.WriteLine(line);
        }

        public static void Print(string text)
        {
            Console.Write(text);
            MainLogFile.Write(text);
        }

        public static void PrintError(string line)
        {
            PrintLine("*****ERROR_START*****");
            PrintLine(line);
            PrintLine("*****ERROR_END*****");
        }

        public static void ContinueLine()
        {
            PrintLine("Done! Press Enter to continue");
            Console.ReadLine();
        }

        public static Color GetRainbowOnTime()
        {
            return ColorFromHSV(((GetTime() % RainbowSpeed) / (float)RainbowSpeed) * 360, 1, 1);
        }

        public static long GetTime()
        {
            return (long) (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        
        //https://stackoverflow.com/a/1626232
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }

    /// <summary>
    /// Devices specified in the AsusSDK documentation
    /// </summary>
    public enum AsusDeviceType : uint
    {
        All = 0x00000000,
        Motherboard = 0x00010000,
        MotherboardLedStrip = 0x00011000,
        AllInOnePc = 0x00012000,
        Vga = 0x00020000,
        Display = 0x00030000,
        Headset = 0x00040000,
        Microphone = 0x00050000,
        ExternalHdd = 0x00060000,
        ExternalBdDrive = 0x00061000,
        Dram = 0x00070000,
        Keyboard = 0x00080000,
        NotebookKeyboard = 0x00081000,
        NotebookKeyboard4ZoneType = 0x00081001,
        Mouse = 0x00090000,
        Chassis = 0x000B0000,
        Projector = 0x000C0000,
    }
}
