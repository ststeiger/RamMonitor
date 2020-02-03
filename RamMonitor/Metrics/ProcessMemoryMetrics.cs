
namespace RamMonitor
{


    public class ProcessMemoryMetrics
    {
        // The amount of physical memory, in bytes, allocated for the associated process.
        public long WorkingSet;
        // The amount of physical memory, in bytes, allocated for the associated process.
        public long WorkingSet64;

        // The maximum amount of physical memory that the associated process has required all at once, in bytes.
        public long PeakWorkingSet;
        // The maximum amount of physical memory that the associated process has required all at once, in bytes.
        public long PeakWorkingSet64;

        // Gets the amount of the virtual memory, in bytes, allocated for the associated process.
        public long VirtualMemorySize;
        // Gets the amount of the virtual memory, in bytes, allocated for the associated process.
        public long VirtualMemorySize64;

        // The maximum amount of virtual memory, in bytes, that the associated process has requested.
        public long PeakVirtualMemorySize;
        // The maximum amount of virtual memory, in bytes, that the associated process has requested.
        public long PeakVirtualMemorySize64;

        // The amount of memory, in bytes, allocated for the associated process 
        // that cannot be shared with other processes.
        public long PrivateMemorySize;
        // The amount of memory, in bytes, allocated for the associated process 
        // that cannot be shared with other processes.
        public long PrivateMemorySize64;

        // The amount of memory, in bytes, allocated by the associated process 
        // that can be written to the virtual memory paging file.
        public long PagedMemorySize;
        // The amount of memory, in bytes, allocated by the associated process 
        // that can be written to the virtual memory paging file.
        public long PagedMemorySize64;

        // The maximum amount of memory, in bytes, allocated by the associated process 
        // that could be written to the virtual memory paging file.
        public long PeakPagedMemorySize;
        // The maximum amount of memory, in bytes, allocated by the associated process 
        // that could be written to the virtual memory paging file.
        public long PeakPagedMemorySize64;

        // The amount of memory, in bytes, the system has allocated for the associated process
        // that can be written to the virtual memory paging file.
        public long PagedSystemMemorySize;
        // The amount of memory, in bytes, the system has allocated for the associated process
        // that can be written to the virtual memory paging file.
        public long PagedSystemMemorySize64;


        // The amount of nonpaged system memory, in bytes, allocated for the associated process 
        // that cannot be written to the virtual memory paging file.
        public long NonpagedSystemMemorySize;
        // The amount of nonpaged system memory, in bytes, allocated for the associated process 
        // that cannot be written to the virtual memory paging file.
        public long NonpagedSystemMemorySize64;



        static readonly string[] SizeSuffixes =
              { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };


        static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (System.Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }

        static string GetSize(long amount)
        {
            return "  ( " + SizeSuffix(amount) + " )";
        }


        public void WriteMemory(System.IO.TextWriter output)
        {
            output.Write("WorkingSet:\t\t\t");
            output.Write(this.WorkingSet.ToString("N0"));
            output.WriteLine(GetSize(this.WorkingSet));

            output.Write("WorkingSet64:\t\t\t");
            output.Write(this.WorkingSet64.ToString("N0"));
            output.WriteLine(GetSize(this.WorkingSet64));

            output.Write("PeakWorkingSet:\t\t\t");
            output.Write(this.PeakWorkingSet.ToString("N0"));
            output.WriteLine(GetSize(this.PeakWorkingSet));
            output.Write("PeakWorkingSet64:\t\t");
            output.Write(this.PeakWorkingSet64.ToString("N0"));
            output.WriteLine(GetSize(this.PeakWorkingSet64));

            output.Write("VirtualMemorySize:\t\t");
            output.Write(this.VirtualMemorySize.ToString("N0"));
            output.WriteLine(GetSize(this.VirtualMemorySize));
            output.Write("VirtualMemorySize64:\t\t");
            output.Write(this.VirtualMemorySize64.ToString("N0"));
            output.WriteLine(GetSize(this.VirtualMemorySize64));

            output.Write("PeakVirtualMemorySize:\t\t");
            output.Write(this.PeakVirtualMemorySize.ToString("N0"));
            output.WriteLine(GetSize(this.PeakVirtualMemorySize));
            output.Write("PeakVirtualMemorySize64:\t");
            output.Write(this.PeakVirtualMemorySize64.ToString("N0"));
            output.WriteLine(GetSize(this.PeakVirtualMemorySize64));

            output.Write("PrivateMemorySize:\t\t");
            output.Write(this.PrivateMemorySize.ToString("N0"));
            output.WriteLine(GetSize(this.PrivateMemorySize));
            output.Write("PrivateMemorySize64:\t\t");
            output.Write(this.PrivateMemorySize64.ToString("N0"));
            output.WriteLine(GetSize(this.PrivateMemorySize64));

            output.Write("PagedMemorySize:\t\t");
            output.Write(this.PagedMemorySize.ToString("N0"));
            output.WriteLine(GetSize(this.PagedMemorySize));
            output.Write("PagedMemorySize64:\t\t");
            output.Write(this.PagedMemorySize64.ToString("N0"));
            output.WriteLine(GetSize(this.PagedMemorySize64));

            output.Write("PeakPagedMemorySize:\t\t");
            output.Write(this.PeakPagedMemorySize.ToString("N0"));
            output.WriteLine(GetSize(this.PeakPagedMemorySize));
            output.Write("PeakPagedMemorySize64:\t\t");
            output.Write(this.PeakPagedMemorySize64.ToString("N0"));
            output.WriteLine(GetSize(this.PeakPagedMemorySize64));

            output.Write("PagedSystemMemorySize:\t\t");
            output.Write(this.PagedSystemMemorySize.ToString("N0"));
            output.WriteLine(GetSize(this.PagedSystemMemorySize));
            output.Write("PagedSystemMemorySize64:\t");
            output.Write(this.PagedSystemMemorySize64.ToString("N0"));
            output.WriteLine(GetSize(this.PagedSystemMemorySize64));

            output.Write("NonpagedSystemMemorySize:\t");
            output.Write(this.NonpagedSystemMemorySize.ToString("N0"));
            output.WriteLine(GetSize(this.PagedSystemMemorySize64));
            output.Write("NonpagedSystemMemorySize64:\t");
            output.Write(this.NonpagedSystemMemorySize64.ToString("N0"));
            output.WriteLine(GetSize(this.PagedSystemMemorySize64));
        } // End Sub WriteMemory 


    } // End Class ProcessMemoryMetrics 


}
