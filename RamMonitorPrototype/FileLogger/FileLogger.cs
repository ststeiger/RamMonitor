
// using Microsoft.Extensions.Logging;

namespace RamMonitorPrototype
{


    public class FileLogger
        : Microsoft.Extensions.Logging.ILogger
    {

        protected const int NUM_INDENT_SPACES = 4;

        protected object m_lock;

        protected Microsoft.Extensions.Logging.LogLevel m_logLevel;
        protected Microsoft.Extensions.Logging.ILoggerProvider m_provider;
        protected int m_indentLevel;
        protected System.IO.TextWriter m_textWriter;

        protected System.Collections.Generic.LinkedList<object> m_scopes;


        public FileLogger(Microsoft.Extensions.Logging.ILoggerProvider provider, string categoryName)
        {
            this.m_lock = new object();
            
            this.m_logLevel = Microsoft.Extensions.Logging.LogLevel.Trace;
            this.m_provider = provider;
            this.m_indentLevel = 0;
            this.m_textWriter = System.Console.Out;
            this.m_scopes = new System.Collections.Generic.LinkedList<object>();
        } // End Constructor 


        //public FileLogger()
        //    :this(null, null)
        //{ }


        public void WriteIndent()
        {
            this.m_textWriter.Write(new string(' ', this.m_indentLevel * NUM_INDENT_SPACES));
        } // End Sub WriteIndent 


        System.IDisposable Microsoft.Extensions.Logging.ILogger.BeginScope<TState>(TState state)
        {
            FileLoggerScope<TState> scope = null;

            scope = new FileLoggerScope<TState>(this, state);
            this.m_scopes.AddFirst(scope);

            this.m_indentLevel++;
            WriteIndent();
            this.m_textWriter.Write("BeginScope<TState>: ");
            this.m_textWriter.WriteLine(state);
            this.m_indentLevel++;

            // this.m_provider.ScopeProvider.Push(state);
            // throw new System.NotImplementedException();

            return scope;
        } // End Function BeginScope 


        public void EndScope<TState>(TState scopeName)
        {
            // FooLoggerScope<TState> scope = (FooLoggerScope<TState>)this.m_scopes.First.Value;
            this.m_indentLevel--;

            WriteIndent();
            this.m_textWriter.Write("EndScope ");
            // this.m_textWriter.WriteLine(scope.ScopeName);
            this.m_textWriter.WriteLine(scopeName);

            this.m_indentLevel--;
            this.m_scopes.RemoveFirst();
        } // End Sub EndScope 


        bool Microsoft.Extensions.Logging.ILogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            // return this.m_provider.IsEnabled(logLevel);
            return logLevel >= this.m_logLevel;
        } // End Function IsEnabled 


        void Microsoft.Extensions.Logging.ILogger.Log<TState>(
              Microsoft.Extensions.Logging.LogLevel logLevel
            , Microsoft.Extensions.Logging.EventId eventId
            , TState state
            , System.Exception exception
            , System.Func<TState, System.Exception, string> formatter)
        {

            lock (this.m_lock)
            {


                WriteIndent();
                this.m_textWriter.Write("Log<TState>: ");
                this.m_textWriter.WriteLine(state);

                System.Exception currentException = exception;

                while (currentException != null)
                {
                    WriteIndent();
                    this.m_textWriter.Write("Log<TState>.Message: ");
                    this.m_textWriter.WriteLine(exception.Message);
                    WriteIndent();
                    this.m_textWriter.Write("Log<TState>.StackTrace: ");
                    this.m_textWriter.WriteLine(exception.StackTrace);

                    currentException = currentException.InnerException;
                } // Whend 

            } // End Lock 

        } // End Sub Log 


    } // End Class FileLogger 


} // End Namespace RamMonitorPrototype 
