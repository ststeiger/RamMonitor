
namespace RamMonitorPrototype
{


    class Program
    {


        // https://cheesehead-techblog.blogspot.com/2009/02/five-ways-to-make-notification-pop-up.html
        // https://wiki.debianforum.de/Desktop-Notification_von_Systemservice_mittels_dbus
        // https://gist.github.com/ducin/6152106
        // https://cweiske.de/tagebuch/DBus%20notify-send%20over%20network.htm
        // dotnet dbus list services --bus system | grep NetworkManager org.freedesktop.NetworkManager
        // dotnet dbus list objects --bus system --service org.freedesktop.NetworkManager
        // dotnet dbus codegen --bus system --service org.freedesktop.NetworkManager
        
        // cd ~/gitlab/Projects/NiHaoRS/Tmds.DBus.Tool
        // dotnet run codegen --bus system --service org.freedesktop.NetworkManager
        // dotnet run codegen --bus session --service org.freedesktop.Notifications
        

        static void Main(string[] args)
        {
            Utsname uts = new Utsname();
            uts.Write(System.Console.Out);

            System.Console.WriteLine(OsInfo.OSFullName);
            
            
            System.Console.Write(@"OS Description: ");
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);
            System.Console.WriteLine(System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion());

            // ProcessManager.KillProcessGroup("chrome");
            ProcessMemoryMetrics mem = ProcessManager.GetProcessGroupMemory("chrome");
            // mem.WriteMemory(System.Console.Out);


            // https://forums.freebsd.org/threads/getting-free-and-real-memory-with-c.38754/
            // https://stackoverflow.com/questions/2513505/how-to-get-available-memory-c-g
            // System.GC.GetTotalMemory(true);
            
            GlobalMemoryMetrics metrics = OsInfo.MemoryMetrics;
            System.Console.WriteLine(metrics.Load);
            System.Console.WriteLine(metrics.SwapLoad);


            GlobalMemoryMetrics procMem = new LinuxMemoryMetrics();
            System.Console.WriteLine(procMem);
            procMem.WriteMemory(System.Console.Out);
            

            GlobalMemoryMetrics winMem = new WindowsMemoryMetrics();
            System.Console.WriteLine(winMem);
            winMem.WriteMemory(System.Console.Out);
            


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main(string[] args) 


    } // End Class Program 


} // End Namespace RamMonitorPrototype 
