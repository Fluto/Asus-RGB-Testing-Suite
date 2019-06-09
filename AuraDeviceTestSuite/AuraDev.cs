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
    public static class AuraDev
    {
        private static readonly AuraDevelopement AuraDevelopment = new AuraDevelopement();
        private static readonly Stopwatch StopWatch = new Stopwatch();

        public static void PrintDeviceInfo()
        {
            AuraDevelopment.AURARequireToken(0);

            PrintLine("DEVICES");
            PrintLine("Retrieving...");
            var allDevices = AuraDevelopment.GetAllDevices();
            foreach (IAuraDevice auraDevice in allDevices)
            {
                var foundType = IsValidEnum(typeof(AsusDeviceType), (int) auraDevice.Type);
                PrintLine($@"-------
Name:                   {auraDevice.Name}
Hardware:               {(foundType ? ((AsusDeviceType)auraDevice.Type).ToString() : "UNKNOWN" )}
Model:                  {auraDevice.Manufacture}
Manufacture:            {auraDevice.Model}
Light Count:            {auraDevice.LightCount}
Light Count Variable:   {auraDevice.LightCountVariable}
Default Light Count:    {auraDevice.DefaultLightCount}
Width:                  {auraDevice.Width}
Height:                 {auraDevice.Height}
");

                if (auraDevice.Lights.Count > 0)
                {
                    PrintLine("Lights");
                    for (var i = 0; i < auraDevice.Lights.Count; i++)
                    {
                        IAuraRgbLight light = auraDevice.Lights[i];
                        PrintLine($"  {i:D3}:{light.Name}");
                    }
                }
                if (auraDevice.Effects.Count > 0)
                {
                    PrintLine("Effects");
                    foreach (IAuraEffect auraDeviceEffect in auraDevice.Effects)
                    {
                        PrintLine($"  {auraDeviceEffect.Name}");
                    }
                }
                if (auraDevice.StandbyEffects.Count > 0)
                {
                    PrintLine("Standby Effects");
                    foreach (IAuraEffect auraDeviceEffect in auraDevice.StandbyEffects)
                    {
                        PrintLine($"  {auraDeviceEffect.Name}");
                    }
                }

            }
            PrintLine("-------");
        }

        public static void ChangeAllDevicesColor(Color color)
        {
            PrintLine("Starting SDK...");
            AuraDevelopment.AURARequireToken(0);
            var allDevices = AuraDevelopment.GetAllDevices();
            Console.Clear();
            foreach (IAuraDevice allDevice in allDevices)
            {
                Print($"Setting device {allDevice.Name}...");
                allDevice.SetMode(0);
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
        }

        public static void PlayRainbowEffectSync(int ups)
        {
            PrintLine("Starting SDK...");
            AuraDevelopment.AURARequireToken(0);
            var allDevices = AuraDevelopment.GetAllDevices();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.Clear();
            PrintLine("Playing rainbow effect. Press ENTER to cancel");
            foreach (IAuraDevice auraDevice in allDevices)
            {
                auraDevice.SetMode(0);
            }
            
            while ((!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Enter))
            {
                if (stopwatch.ElapsedMilliseconds < ups)
                    continue;

                stopwatch.Restart();
                var color = GetRainbowOnTime();
                    foreach (IAuraDevice device in allDevices)
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
        }


        public static void PlayRandomEffectSync(int ups)
        {
            PrintLine("Starting SDK...");
            AuraDevelopment.AURARequireToken(0);
            var random = new Random();
            var allDevices = AuraDevelopment.GetAllDevices();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.Clear();

            PrintLine("Playing random effect. Press ENTER to cancel");
            foreach (IAuraDevice auraDevice in allDevices)
            {
                auraDevice.SetMode(0);
            }

            while ((!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Enter))
            {
                if (stopwatch.ElapsedMilliseconds < ups)
                    continue;

                stopwatch.Restart();
                foreach (IAuraDevice device in allDevices)
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
        }


        public static bool RunAsync = false;
        public static void PlayRainbowEffectAsync(int ups)
        {
            PrintLine("Starting SDK...");
            AuraDevelopment.AURARequireToken(0);
            var allDevices = AuraDevelopment.GetAllDevices();
            RunAsync = true;
            var tasks = new List<Task>();
            foreach (IAuraDevice device in allDevices)
            {
                device.SetMode(0);
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
            ContinueLine();
        }
        public static void PlayRandomEffectAsync(int ups)
        {
            PrintLine("Starting SDK...");
            AuraDevelopment.AURARequireToken(0);
            var allDevices = AuraDevelopment.GetAllDevices();
            var random = new Random();
            RunAsync = true;
            var tasks = new List<Task>();
            foreach (IAuraDevice device in allDevices)
            {
                device.SetMode(0);
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
            ContinueLine();
        }
    }
}
