
using Microsoft.Extensions.Logging; // for this.m_logger.LogInformation(txt); 


namespace RamMonitorPrototype.Trash
{

    public class Worker { }


    public class ConsoleTextLogger<T>
        : System.IO.TextWriter
    {

        protected readonly Microsoft.Extensions.Logging.ILogger<T> m_logger;
        protected System.Text.StringBuilder m_sb;


        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }


        public ConsoleTextLogger()
        {
            this.m_sb = new System.Text.StringBuilder();
        }

        public ConsoleTextLogger(Microsoft.Extensions.Logging.ILogger<T> logger)
            : this()
        {
            this.m_logger = logger;
        }


        public override void Write(char value)
        {
            if (value == '\r' || value == '\n')
            {
                if (this.m_sb.Length == 0)
                    return;

                string txt = this.m_sb.ToString();
                System.Console.WriteLine(txt);
                // this.m_logger.LogInformation(txt);
                m_sb.Clear();
            }
            else
                this.m_sb.Append(value);
        }


    }


}
