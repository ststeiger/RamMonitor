
namespace RamMonitor.Logging
{

    public class FileLogger
        : TrivialLogger
    {

        public string FilePath;



        public FileLogger(string path)
            : base()
        {
            // ”D:\IDGLog.txt”
            this.FilePath = path;
            this.LogLevel = LogLevel_t.Everything;
        }


        public FileLogger()
            : this(null)
        { }


        public override void Log(LogLevel_t logLevel, string message, System.Exception exception)
        {
            if (!LogLevel.HasFlag(logLevel))
                return;

            lock (lockObj)
            {
                using (System.IO.Stream strm = System.IO.File.Open(this.FilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read))
                {
                    using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(strm, System.Text.Encoding.UTF8))
                    {
                        // 2009-06-05 08:15:23 [INFO] User credentials entered, Acc={123765987}

                        streamWriter.WriteLine(System.Environment.NewLine);
                        streamWriter.Write(this.Time);
                        streamWriter.Write(" ");
                        string ll = "[" + System.Convert.ToString(logLevel, System.Globalization.CultureInfo.InvariantCulture) + "]";
                        ll = ll.PadRight(15, ' ');

                        streamWriter.Write(ll);
                        streamWriter.Write(" ");

                        if (message == null)
                            streamWriter.WriteLine("Error: ");
                        else
                            streamWriter.WriteLine(message);

                        System.Exception thisError = exception;
                        while (thisError != null)
                        {
                            streamWriter.Write(new string(' ', 40));
                            streamWriter.Write("Type: ");
                            streamWriter.WriteLine(thisError.GetType().Name);

                            streamWriter.Write(new string(' ', 40));
                            streamWriter.Write("Source: ");
                            streamWriter.WriteLine(thisError.Source);

                            streamWriter.Write(new string(' ', 40));
                            streamWriter.WriteLine(thisError.Message);


                            streamWriter.Write(new string(' ', 40));
                            streamWriter.WriteLine("StackTrace: ");
                            streamWriter.Write(new string(' ', 40));
                            streamWriter.WriteLine(exception.StackTrace);

                            thisError = thisError.InnerException;

                            if (thisError != null)
                            {
                                streamWriter.Write(new string(' ', 40));
                                streamWriter.WriteLine("Inner Exception:");
                            } // End if (thisError != null) 

                        } // Whend 

                        streamWriter.Close();
                    } // End Using streamWriter 

                } // End Using strm 

            } // End Lock 

        } // End Sub Log 

    } // End Class FileLogger 


}
