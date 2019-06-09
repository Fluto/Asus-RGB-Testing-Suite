using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using static AuraDeviceTestSuite.Helper;

namespace AuraDeviceTestSuite
{
    public static class AuraDriver
    {
        public static void Print()
        {
            PrintLine("======== " + (System.Environment.Is64BitProcess ? "64" : "32") + "-bit ========");
            var rkClsid = Registry.ClassesRoot.OpenSubKey("CLSID");
            for (int i = 0; i < 2; i++)
            {
                if (rkClsid == null)
                {
                    PrintLine("CLSID not found. (!!)");
                }
                else
                {
                    RetrieveAuraInfo(rkClsid);
                }

                if (!System.Environment.Is64BitProcess || i == 1)
                    break;

                PrintLine("======== 32-bit ========");
                rkClsid = Registry.ClassesRoot.OpenSubKey("Wow6432Node\\CLSID");
            }
        }

        public static void RetrieveAuraInfo(RegistryKey rkClsid)
        {
            var rkSdk = rkClsid.OpenSubKey("{05921124-5057-483E-A037-E9497B523590}\\InprocServer32");
            if (rkSdk == null)
            {
                PrintError("Aura SDK not found in registry.");
                return;
            }
            PrintLine("Aura SDK found!");
            string sdk_file = (string)rkSdk.GetValue("");
            PrintLine("Location: " + sdk_file);
            var sdk_file_modified = System.IO.File.GetLastWriteTime(sdk_file);
            PrintLine("Modified: " + sdk_file_modified.ToString());
            PrintLine("Threading Model: " + rkSdk.GetValue("ThreadingModel"));
            PrintLine("");

            var rkHalRoot = rkClsid.OpenSubKey("{9C9E903E-BBC7-4A0E-8326-ED6AC85B9FCC}\\Instance");
            var types = rkHalRoot.GetSubKeyNames();
            foreach (var type in types)
            {
                PrintLine("HAL Type found: " + type);
                var rkHalType = rkHalRoot.OpenSubKey(type + "\\Instance");
                if (rkHalType != null)
                {
                    var hals = rkHalType.GetSubKeyNames();
                    foreach (var clsidHal in hals)
                    {
                        var rkHalEntry = rkHalType.OpenSubKey(clsidHal);
                        PrintLine("--HAL: " + clsidHal);
                        PrintLine("--Name: " + (string)rkHalEntry.GetValue("Name"));

                        // Try to open HAL in CLSID
                        var rkHal = rkClsid.OpenSubKey(clsidHal + "\\InprocServer32");
                        if (rkHal == null)
                        {
                            PrintError("HAL not found in CLSID!!");
                            continue;
                        }

                        string file_location = (string)rkHal.GetValue("");
                        PrintLine("--Location: " + file_location);
                        var date_time = System.IO.File.GetLastWriteTime(file_location);
                        PrintLine("--Modified: " + date_time.ToString());
                        PrintLine("--ThreadingModel: " + (string)rkHal.GetValue("ThreadingModel"));
                        PrintLine("");
                    }
                }
            }
        }
    }
}
