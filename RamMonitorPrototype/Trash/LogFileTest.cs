
using System.Linq;


namespace RamMonitorPrototype.Trash
{


    class LogFileTest
    {

        public static void Test()
        {
            RollFilesByDate();
            RollFiles();
            RollFilesWithoutLinq();
        }

        static void RollFilesByDate()
        {
            int? _maxRetainedFiles = 3;
            string _path = @"D:\username\Documents\Visual Studio 2017\Projects\RamMonitor\RamMonitorPrototype\Log";
            string _fileName = @"";

            if (_maxRetainedFiles > 0)
            {
                System.Collections.Generic.List<System.IO.FileInfo> fileList = new System.IO.DirectoryInfo(_path)
                   .GetFiles(_fileName + "*", System.IO.SearchOption.TopDirectoryOnly)
                   .OrderBy(fi => fi.CreationTime)
                   .ToList();

                foreach (System.IO.FileInfo item in fileList)
                {
                    // item.Delete();
                    System.Console.WriteLine(item.Name);
                }

                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine(System.Environment.NewLine);

                System.IO.FileInfo[] fis = new System.IO.DirectoryInfo(_path).GetFiles(_fileName + "*");

                System.Array.Sort<System.IO.FileInfo>(fis,
                    delegate (System.IO.FileInfo f1, System.IO.FileInfo f2)
                    {
                        return f1.CreationTime.CompareTo(f2.CreationTime);
                    }
                );

                foreach (System.IO.FileInfo item in fis)
                {
                    // item.Delete();
                    System.Console.WriteLine(item.Name);
                }

                System.Console.WriteLine();
            }
        }



        static void RollFiles()
        {
            int? _maxRetainedFiles = 3;
            string _path = @"D:\username\Documents\Visual Studio 2017\Projects\RamMonitor\RamMonitorPrototype\LogLinq";
            string _fileName = @"";

            if (_maxRetainedFiles > 0)
            {
                System.Collections.Generic.IEnumerable<System.IO.FileInfo> files =
                    new System.IO.DirectoryInfo(_path)
                    .GetFiles(_fileName + "*")
                    .OrderByDescending(f => f.Name)
                    .Skip(_maxRetainedFiles.Value);

                foreach (System.IO.FileInfo item in files)
                {
                    // item.Delete();
                    System.Console.WriteLine(item.Name);
                }
            }
        }


        // Delete files if we have too many
        static void RollFilesWithoutLinq()
        {
            int? _maxRetainedFiles = 3;
            string _path = @"D:\username\Documents\Visual Studio 2017\Projects\RamMonitor\RamMonitorPrototype\Log";
            string _fileName = @"";

            if (_maxRetainedFiles > 0)
            {
                System.IO.FileInfo[] fi = new System.IO.DirectoryInfo(_path)
                .GetFiles(_fileName + "*");

                System.Array.Sort<System.IO.FileInfo>(
                    fi,
                    delegate (System.IO.FileInfo f1, System.IO.FileInfo f2)
                    {
                        return f2.Name.CompareTo(f1.Name);
                    }
                );

                System.Console.WriteLine(System.Environment.NewLine);
                System.Console.WriteLine(System.Environment.NewLine);

                _maxRetainedFiles = 0;

                for (int i = _maxRetainedFiles.Value; i < fi.Length; ++i)
                {
                    // fi[i].Delete();
                    System.Console.WriteLine(fi[i].Name);
                }

            }

        }


    }


}
