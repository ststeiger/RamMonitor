
namespace RamMonitorPrototype
{

    // https://stackoverflow.com/questions/33804402/get-performancecounter-by-index
    public static class PerfMapper
    {
        public static System.Collections.Generic.Dictionary<string, int> English;
        

        public static System.Collections.Generic.Dictionary<int, string> Localized;


        public static System.Collections.Generic.Dictionary<string, int> ReverseLocalized;
        public static System.Collections.Generic.Dictionary<int, string> ReverseEnglish;


        public static System.Diagnostics.PerformanceCounter FromEnglish(string category, string name, string instance = null)
        {
            return new System.Diagnostics.PerformanceCounter(Map(category), Map(name), instance);
        }

        public static System.Diagnostics.PerformanceCounter FromIndices(int category, int name, string instance = null)
        {
            return new System.Diagnostics.PerformanceCounter(PdhMap(category), PdhMap(name), instance);
        }

        public static bool HasName(string name)
        {
            if (English == null) LoadNames();
            if (!English.ContainsKey(name)) return false;
            int index = English[name];
            return !Localized.ContainsKey(index);
        }

        public static string Map(string text)
        {
            if (HasName(text)) return Localized[English[text]];
            else return text;
        }

        private static string PdhMap(int index)
        {
            int size = 0;
            uint ret = PdhLookupPerfNameByIndex(null, index, null, ref size);
            if (ret == 0x800007D2)
            {
                System.Text.StringBuilder buffer = new System.Text.StringBuilder(size);
                ret = PdhLookupPerfNameByIndex(null, index, buffer, ref size);
                if (ret == 0) return buffer.ToString();
            }
            throw new System.ComponentModel.Win32Exception((int)ret, "PDH lookup failed");
        }

        public static void LoadNames()
        {
            string[] english;
            string[] local;
            // Retrieve English and localized strings
            using (Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.RegistryKey.OpenBaseKey(
                  Microsoft.Win32.RegistryHive.LocalMachine
                , Microsoft.Win32.RegistryView.Registry64))
            {
                using (Microsoft.Win32.RegistryKey key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\009"))
                {
                    english = (string[])key.GetValue("Counter");
                }
                using (Microsoft.Win32.RegistryKey key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\CurrentLanguage"))
                {
                    local = (string[])key.GetValue("Counter");
                }
            }
            // Create English lookup table
            English = new System.Collections.Generic.Dictionary<string, int>(english.Length / 2, System.StringComparer.InvariantCultureIgnoreCase);
            ReverseEnglish = new System.Collections.Generic.Dictionary<int, string>(english.Length / 2);
            for (int ix = 0; ix < english.Length - 1; ix += 2)
            {
                int index = int.Parse(english[ix]);
                if (!English.ContainsKey(english[ix + 1]))
                {
                    English.Add(english[ix + 1], index);
                    ReverseEnglish.Add(index, english[ix + 1]);
                }
                    
            }
            // Create localized lookup table
            Localized = new System.Collections.Generic.Dictionary<int, string>(local.Length / 2);
            ReverseLocalized = new System.Collections.Generic.Dictionary<string, int>(local.Length / 2, System.StringComparer.InvariantCultureIgnoreCase);
            for (int ix = 0; ix < local.Length - 1; ix += 2)
            {
                int index = int.Parse(local[ix]);
                Localized.Add(index, local[ix + 1]);

                ReverseLocalized[local[ix + 1]] = index;
            }
        }

        [System.Runtime.InteropServices.DllImport("pdh.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern uint PdhLookupPerfNameByIndex(string machine, int index, System.Text.StringBuilder buffer, ref int bufsize);
    }
}
