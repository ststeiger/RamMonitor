
using Serilog;


namespace RamMonitorPrototype
{


    class Program
    {


        /*
https://devblogs.microsoft.com/dotnet/introducing-net-5/
https://andrewlock.net/creating-a-rolling-file-logging-provider-for-asp-net-core-2-0/
https://devblogs.microsoft.com/dotnet/collecting-and-analyzing-memory-dumps/
https://devblogs.microsoft.com/dotnet/an-introduction-to-dataframe/
https://devblogs.microsoft.com/dotnet/updates-to-net-core-windows-forms-designer-in-visual-studio-16-5-preview-1/
https://github.com/NickFane/NetCoreWorkerService
https://nblumhardt.com/2016/10/aspnet-core-file-logger/
https://visualstudiomagazine.com/articles/2019/03/22/logging-in-net-core.aspx
https://dotnetcoretutorials.com/2018/04/12/using-the-ilogger-beginscope-in-asp-net-core/
https://thinkrethink.net/2017/03/09/application-shutdown-in-asp-net-core/
https://csharp.christiannagel.com/2018/11/13/iloggertofile/
https://stackify.com/asp-net-core-logging-what-changed/
https://gunnarpeipman.com/aspnet-core-file-logging/
https://www.citusdata.com/blog/2018/01/22/multi-tenant-web-apps-with-dot-net-core-and-postgres/
https://www.mikesdotnetting.com/article/342/managing-authentication-token-expiry-in-webassembly-based-blazor
https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
https://wildermuth.com/2018/04/10/Using-JwtBearer-Authentication-in-an-API-only-ASP-NET-Core-Project
https://jasonwatmore.com/post/2018/08/14/aspnet-core-21-jwt-authentication-tutorial-with-example-api
https://builtwithdot.net/blog/fluent-apis-make-developers-love-using-your-net-libraries



https://medium.com/@nickfane/introduction-to-worker-services-in-net-core-3-0-4bb3fc631225
https://andrewlock.net/introducing-ihostlifetime-and-untangling-the-generic-host-startup-interactions/
https://andrewlock.net/new-in-aspnetcore-3-structured-logging-for-startup-messages/
http://www.dotnetspeak.com/net-core/creating-schedule-driven-windows-service-in-net-core-3-0/


https://devblogs.microsoft.com/dotnet/
https://github.com/RickStrahl
https://github.com/filipw


https://www.strathweb.com/
https://visualstudiomagazine.com
https://dotnetcoretutorials.com
https://www.citusdata.com/blog/
https://developers.redhat.com/blog/category/products/infrastructure/rhel/dot-net/
https://thomaslevesque.com/about/
https://blog.codinghorror.com/
http://james.newtonking.com/
https://wildermuth.com/
https://odetocode.com/blogs/scott
https://nblumhardt.com
https://medium.com/@nickfane
https://weblog.west-wind.com/
https://blog.noser.com/

*/

        // https://github.com/dotnet/corefx/issues/32888
        // https://devblogs.microsoft.com/dotnet/introducing-net-5/
        static void Main(string[] args)
        {
            MainTask.Start(args).Wait();

            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
               .MinimumLevel.Verbose()
               // .MinimumLevel.Is(Serilog.Events.LogEventLevel.Verbose)
               .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Verbose)
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.File("mylog.txt")
               .CreateLogger();


            Log.Information("Starting web host");
            Log.Error("foobar");
            Log.Fatal("fatal");
            Log.Warning("Warning");
            Log.Debug("Debug");
            Log.Verbose("Verbose");


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
