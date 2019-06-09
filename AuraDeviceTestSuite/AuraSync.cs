using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuraServiceLib;
using static AuraDeviceTestSuite.Helper;

namespace AuraDeviceTestSuite
{
    public static class AuraSync
    {
        private static readonly IAuraSdk2 MyAuraSdk = (IAuraSdk2)(new AuraSdk());
        private static readonly Stopwatch StopWatch = new Stopwatch();

        public static void ChangeAllDevicesColor(Color color)
        {
            PrintLine("Starting SDK...");
            MyAuraSdk.SwitchMode();
            var allDevices = MyAuraSdk.Enumerate(0);
            Console.Clear();
            foreach (IAuraSyncDevice allDevice in allDevices)
            {
                Print($"Setting device {allDevice.Name}...");
                StopWatch.Restart();
                foreach (IAuraRgbLight light in allDevice.Lights)
                {
                    light.Red = color.R;
                    light.Green = color.G;
                    light.Blue = color.B;
                }

                allDevice.Apply();
                StopWatch.Stop();
                PrintLine($"Done! took {StopWatch.ElapsedMilliseconds}ms");
            }
            PrintLine("Stopping SDK...");
            MyAuraSdk.ReleaseControl(0);
        }

        public static void PlayRainbowEffectSync(int ups)
        {
            PrintLine("Starting SDK...");
            MyAuraSdk.SwitchMode();
            var allDevices = MyAuraSdk.Enumerate(0);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.Clear();
            PrintLine("Playing rainbow effect. Press ENTER to cancel");
            
            while ((!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Enter))
            {
                if (stopwatch.ElapsedMilliseconds < ups)
                    continue;

                stopwatch.Restart();
                var color = GetRainbowOnTime();
                foreach (IAuraSyncDevice device in allDevices)
                {
                    foreach (IAuraRgbLight light in device.Lights)
                    {
                        light.Red = color.R;
                        light.Green = color.G;
                        light.Blue = color.B;
                    }

                    device.Apply();
                }
            }
            PrintLine("Stopping SDK...");
            MyAuraSdk.ReleaseControl(0);
        }


        public static void PlayRandomEffectSync(int ups)
        {
            PrintLine("Starting SDK...");
            MyAuraSdk.SwitchMode();
            var allDevices = MyAuraSdk.Enumerate(0);
            var random = new Random();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.Clear();

            PrintLine("Playing random effect. Press ENTER to cancel");

            while ((!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Enter))
            {
                if (stopwatch.ElapsedMilliseconds < ups)
                    continue;

                stopwatch.Restart();
                foreach (IAuraSyncDevice device in allDevices)
                {
                    foreach (IAuraRgbLight light in device.Lights)
                    {
                        var color = ColorFromHSV(random.Next(0, 360), 1, 1);
                        light.Red = color.R;
                        light.Green = color.G;
                        light.Blue = color.B;
                    }

                    device.Apply();
                }
            }
            PrintLine("Stopping SDK...");
            MyAuraSdk.ReleaseControl(0);
        }


        public static bool RunAsync = false;
        public static void PlayRainbowEffectAsync(int ups)
        {
            PrintLine("Starting SDK...");
            MyAuraSdk.SwitchMode();
            var allDevices = MyAuraSdk.Enumerate(0);
            RunAsync = true;
            var tasks = new List<Task>();
            foreach (IAuraSyncDevice device in allDevices)
            {
                var task = Task.Run(() => {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    while (RunAsync)
                    {
                        if (stopwatch.ElapsedMilliseconds < ups)
                            continue;

                        stopwatch.Restart();
                        var color = GetRainbowOnTime();
                        foreach (IAuraRgbLight light in device.Lights)
                        {
                            light.Red = color.R;
                            light.Green = color.G;
                            light.Blue = color.B;
                        }

                        device.Apply();
                    }
                });
                tasks.Add(task);
            }

            Console.Clear();
            PrintLine("Playing rainbow effect. Press ENTER to cancel");
            do ; while ((!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Enter));

            RunAsync = false;
            PrintLine("Stopping threads...");
            do; while (!tasks.All(task => task.IsCompleted));
            PrintLine("Stopping SDK...");
            MyAuraSdk.ReleaseControl(0);
            ContinueLine();
        }
        public static void PlayRandomEffectAsync(int ups)
        {
            PrintLine("Starting SDK...");
            MyAuraSdk.SwitchMode();
            var allDevices = MyAuraSdk.Enumerate(0);
            var random = new Random();
            RunAsync = true;
            var tasks = new List<Task>();
            foreach (IAuraSyncDevice device in allDevices)
            {
                var task = Task.Run(() => {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    while (RunAsync)
                    {
                        if (stopwatch.ElapsedMilliseconds < ups)
                            continue;

                        stopwatch.Restart();
                        foreach (IAuraRgbLight light in device.Lights)
                        {
                            var color = ColorFromHSV(random.Next(0, 360), 1, 1);
                            light.Red = color.R;
                            light.Green = color.G;
                            light.Blue = color.B;
                        }

                        device.Apply();
                    }
                });
                tasks.Add(task);
            }

            Console.Clear();
            PrintLine("Playing rainbow effect. Press ENTER to cancel");
            do; while ((!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Enter));

            RunAsync = false;
            PrintLine("Stopping threads...");
            do; while (!tasks.All(task => task.IsCompleted));
            PrintLine("Stopping SDK...");
            MyAuraSdk.ReleaseControl(0);
            ContinueLine();
        }
    }
}
