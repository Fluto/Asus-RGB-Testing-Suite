using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AuraDeviceTestSuite.Helper;

namespace AuraDeviceTestSuite
{
    class Program
    {

        private static void Main(string[] args)
        {
            Directory.CreateDirectory(FolderLoc);

            try
            {
                MainMenu();
            }
            catch (Exception e)
            {
                PrintError(e.ToString());
            }
        }

        private static void MainMenu()
        {
            PrintLine($@"
*****************
{DateTime.Now.ToString(CultureInfo.InvariantCulture)}
*****************
Sup, welcome to the Aura test suite.
This program will run some tests, check your devices 
and print what they are.

More importantly, this is to provide feedback to see which 
configuration works with your device setup. This will help the 
development of the Aurora RGB project.

All prints will be logged will be stored in 
{Directory.GetCurrentDirectory()}\{Helper.FolderLoc}

Options:
1 - Print driver info
2 - Print device info
3 - RGB testing

0 - Exit
");

            string input;
            do
            {
                ClearLine();
            } while (!IsValidEnum(typeof(MainInput), input = Console.ReadLine()));
            Console.Clear();
            var mainInput = (MainInput)int.Parse(input);

            switch (mainInput)
            {
                case MainInput.PrintDriverInfo:
                    PrintDriverInfo();
                    break;
                case MainInput.PrintDeviceInfo:
                    PrintDeviceInfo();
                    break;
                case MainInput.RgbTesting:
                    GoToSdkSelectionMenu();
                    break;
                case MainInput.Exit:
                    Exit();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            GoToMainMenu();
        }

        private static void SdkSelectionMenu()
        {
            PrintLine($@"
Before testing, please select a method. 
These two methods use different aspects of the AuraSDK

Options:
1 - Method 1 (Aura Development Mode)
2 - Method 2 (AuraSDK Mode)

0 - Back
");

            string input;
            do
            {
                ClearLine();
            } while (!IsValidEnum(typeof(RgbTestingMenuInput), input = Console.ReadLine()));
            Console.Clear();

            var mainInput = (RgbTestingMenuInput)int.Parse(input);

            switch (mainInput)
            {
                case RgbTestingMenuInput.AuraDev:
                    RgbTestingMenu(AuraMethod.AuraDev);
                    return;
                case RgbTestingMenuInput.AuraSdk:
                    RgbTestingMenu(AuraMethod.AuraSdk);
                    return;
                case RgbTestingMenuInput.Back:
                    GoToMainMenu();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void RgbTestingMenu(AuraMethod method)
        {
            string input;
            PrintLine($@"
What effect would you like to run?

Options:
1 - All colors RED
2 - All colors BLUE
3 - All colors GREEN
4 - Rainbow across the device
5 - Random Colors per key

0 - Back
");
            do
            {
                ClearLine();
            } while (!IsValidEnum(typeof(RgbEffectOptionInput), input = Console.ReadLine()));
            Console.Clear();

            var effect = (RgbEffectOptionInput)int.Parse(input);
            switch (effect)
            {
                case RgbEffectOptionInput.AllRed:
                    SetAllColors(method, Color.Red);
                    GoToRgbTestingMenu(method);
                    return;
                case RgbEffectOptionInput.AllBlue:
                    SetAllColors(method, Color.Blue);
                    GoToRgbTestingMenu(method);
                    return;
                case RgbEffectOptionInput.AllGreen:
                    SetAllColors(method, Color.Green);
                    GoToRgbTestingMenu(method);
                    return;
                case RgbEffectOptionInput.Back:
                    GoToSdkSelectionMenu();
                    break;
                case RgbEffectOptionInput.RainbowEffect:
                case RgbEffectOptionInput.RandomEffect:
                    // play this effect later
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            PrintLine($@"
Would you like to do this synchronously or asynchronously?


Options:
1 - Synchronous (Light up the colours in order of device)
2 - Asynchronous (Light up the colours Asap)
"); 
            do
            {
                ClearLine();
            } while (!IsValidEnum(typeof(SynchronousOptionInput), input = Console.ReadLine()));
            Console.Clear();

            var syncOption = (SynchronousOptionInput)int.Parse(input);

            PrintLine($@"
What Update per second would you like this device to update at?

0 - Unlimited
1 -  1 UPS
2 -  2 UPS
3 -  5 UPS
4 - 10 UPS
5 - 20 UPS
6 - 30 UPS
7 - 60 UPS
");
            do
            {
                ClearLine();
            } while (!IsValidEnum(typeof(UpsOptionInput), input = Console.ReadLine()));
            
            var ups = (UpsOptionInput)int.Parse(input);

            switch (effect)
            {
                case RgbEffectOptionInput.RainbowEffect:
                    PlayRainbowEffect(method, syncOption, ups);
                    GoToRgbTestingMenu(method);
                    return;
                case RgbEffectOptionInput.RandomEffect:
                    PlayRandomEffect(method, syncOption, ups);
                    GoToRgbTestingMenu(method);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static void PrintDriverInfo()
        {
            AuraDriver.Print();
            ContinueLine();
        }

        private static void PrintDeviceInfo()
        {
            AuraDev.PrintDeviceInfo();
            ContinueLine();
        }

        private static void Exit()
        {
            Environment.Exit(0);
        }

        private static void GoToMainMenu()
        {
            Console.Clear();
            MainMenu();
        }

        private static void GoToSdkSelectionMenu()
        {
            Console.Clear();
            SdkSelectionMenu();
        }
        private static void GoToRgbTestingMenu(AuraMethod method)
        {
            Console.Clear();
            RgbTestingMenu(method);
        }

        private static void SetAllColors(AuraMethod method, Color color)
        {
            Console.Clear();
            PrintLine("Setting all colors...");
            if (method == AuraMethod.AuraDev)
                AuraDev.ChangeAllDevicesColor(color);
            if (method == AuraMethod.AuraSdk)
                AuraSync.ChangeAllDevicesColor(color);

            ContinueLine();
        }

        private static void PlayRainbowEffect(AuraMethod method, SynchronousOptionInput syncOption, UpsOptionInput upsOption)
        {
            Console.Clear();
            PrintLine("Setting all colors...");
            if (method == AuraMethod.AuraDev && syncOption == SynchronousOptionInput.Synchronous)
                AuraDev.PlayRainbowEffectSync(GetUps(upsOption));
            if (method == AuraMethod.AuraDev && syncOption == SynchronousOptionInput.Asynchronous)
                AuraDev.PlayRainbowEffectAsync(GetUps(upsOption));
            if (method == AuraMethod.AuraSdk && syncOption == SynchronousOptionInput.Synchronous)
                AuraSync.PlayRainbowEffectSync(GetUps(upsOption));
            if (method == AuraMethod.AuraSdk && syncOption == SynchronousOptionInput.Asynchronous)
                AuraSync.PlayRainbowEffectAsync(GetUps(upsOption));
        }

        private static void PlayRandomEffect(AuraMethod method, SynchronousOptionInput syncOption, UpsOptionInput upsOption)
        {
            Console.Clear();
            PrintLine("Setting all colors...");
            if (method == AuraMethod.AuraDev && syncOption == SynchronousOptionInput.Synchronous)
                AuraDev.PlayRandomEffectSync(GetUps(upsOption));
            if (method == AuraMethod.AuraDev && syncOption == SynchronousOptionInput.Asynchronous)
                AuraDev.PlayRandomEffectAsync(GetUps(upsOption));
            if (method == AuraMethod.AuraSdk && syncOption == SynchronousOptionInput.Synchronous)
                AuraDev.PlayRandomEffectSync(GetUps(upsOption));
            if (method == AuraMethod.AuraSdk && syncOption == SynchronousOptionInput.Asynchronous)
                AuraDev.PlayRandomEffectAsync(GetUps(upsOption));

        }

        private static int GetUps(UpsOptionInput option)
        {
            switch (option)
            {
                case UpsOptionInput.Unlimited:
                    return 0;
                case UpsOptionInput.Ups1:
                    return 1000 / 1;
                case UpsOptionInput.Ups2:
                    return 1000 / 2;
                case UpsOptionInput.Ups5:
                    return 1000 / 5;
                case UpsOptionInput.Ups10:
                    return 1000 / 10;
                case UpsOptionInput.Ups20:
                    return 1000 / 20;
                case UpsOptionInput.Ups30:
                    return 1000 / 30;
                case UpsOptionInput.Ups60:
                    return 1000 / 60;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }
        
        private enum MainInput: int
        {
            PrintDriverInfo = 1,
            PrintDeviceInfo = 2,
            RgbTesting = 3,
            Exit = 0
        }

        private enum RgbTestingMenuInput : int
        {
            AuraDev = 1,
            AuraSdk = 2,
            Back = 0
        }

        private enum SynchronousOptionInput : int
        {
            Synchronous = 1,
            Asynchronous = 2,
        }

        private enum UpsOptionInput : int
        {
            Unlimited,
            Ups1,
            Ups2,
            Ups5,
            Ups10,
            Ups20,
            Ups30,
            Ups60,
        }

        private enum RgbEffectOptionInput : int
        {
            AllRed = 1,
            AllBlue = 2,
            AllGreen = 3,
            RainbowEffect = 4,
            RandomEffect = 5,

            Back = 0
        }


        private enum AuraMethod : int
        {
            AuraDev,
            AuraSdk
        }
    }
}
