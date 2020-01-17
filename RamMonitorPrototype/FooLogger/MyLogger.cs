
// using Microsoft.Extensions.Logging;


using System;

namespace RamMonitorPrototype
{

    public class FooLoggerScope<TState>
        : System.IDisposable
    {
        protected FooLogger m_logger;
        protected TState m_scopeName;
        

        public FooLoggerScope(FooLogger logger, TState scopeName)
        {
            this.m_logger = logger;
            this.m_scopeName = scopeName;
        }


        void IDisposable.Dispose()
        {
            this.m_logger.EndScope(this.m_scopeName);
        }
    }



    public class FooLogger
        : Microsoft.Extensions.Logging.ILogger
    {

        protected const int NUM_INDENT_SPACES = 4;

        protected Microsoft.Extensions.Logging.LogLevel m_logLevel;
        protected Microsoft.Extensions.Logging.ILoggerProvider m_provider;
        protected int m_indentLevel;
        protected System.IO.TextWriter m_textWriter;

        


        public FooLogger(Microsoft.Extensions.Logging.ILoggerProvider provider, string categoryName)
        {
            this.m_logLevel = Microsoft.Extensions.Logging.LogLevel.Trace;
            this.m_provider = provider;
            this.m_indentLevel = 0;
            this.m_textWriter = System.Console.Out;
        }

        //public FooLogger()
        //    :this(null, null)
        //{ }

        public void WriteIndent()
        {
            this.m_textWriter.Write(new String(' ', this.m_indentLevel * NUM_INDENT_SPACES));
        }

        
        System.IDisposable Microsoft.Extensions.Logging.ILogger.BeginScope<TState>(TState state)
        {
            this.m_indentLevel++;
            WriteIndent();
            this.m_textWriter.Write("BeginScope<TState>: ");
            this.m_textWriter.WriteLine(state);
            this.m_indentLevel++;

            // this.m_provider.ScopeProvider.Push(state);
            // throw new System.NotImplementedException();
            return new FooLoggerScope<TState>(this, state);
        }


        public void EndScope<TState>(TState scopeName)
        {
            this.m_indentLevel--;

            WriteIndent();
            this.m_textWriter.Write("EndScope ");
            this.m_textWriter.WriteLine(scopeName);

            this.m_indentLevel--;
        }

        bool Microsoft.Extensions.Logging.ILogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            // return this.m_provider.IsEnabled(logLevel);
            return logLevel >= this.m_logLevel;
        }

        void Microsoft.Extensions.Logging.ILogger.Log<TState>(
              Microsoft.Extensions.Logging.LogLevel logLevel
            , Microsoft.Extensions.Logging.EventId eventId
            , TState state
            , System.Exception exception
            , System.Func<TState, System.Exception, string> formatter)
        {
            WriteIndent();
            this.m_textWriter.Write("Log<TState>: ");
            this.m_textWriter.WriteLine(state);

            if (exception != null)
            {
                WriteIndent();
                this.m_textWriter.Write("Log<TState>.Message: ");
                this.m_textWriter.WriteLine(exception.Message);
                WriteIndent();
                this.m_textWriter.Write("Log<TState>.StackTrace: ");
                this.m_textWriter.WriteLine(exception.StackTrace);
            }
            
        }
    }



    public class FooLoggerProvider
        : Microsoft.Extensions.Logging.ILoggerProvider
    {

        protected System.Collections.Concurrent.ConcurrentDictionary<
            string, Microsoft.Extensions.Logging.ILogger
            > m_loggers = new System.Collections.Concurrent.ConcurrentDictionary<
                            string, Microsoft.Extensions.Logging.ILogger
                        >(System.StringComparer.InvariantCultureIgnoreCase);


        Microsoft.Extensions.Logging.ILogger Microsoft.Extensions.Logging.ILoggerProvider
            .CreateLogger(string categoryName)
        {
            return this.m_loggers.GetOrAdd(categoryName,
                 delegate (string category)
                 {
                     return new FooLogger(this, category);
                 }
             );

        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
                }

                // TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer weiter unten überschreiben.
                // TODO: große Felder auf Null setzen.

                disposedValue = true;
            }
        }

        // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
        // ~FoologgerProvider() {
        //   // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
        //   Dispose(false);
        // }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        void System.IDisposable.Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }


    // https://github.com/serilog/serilog
    // https://github.com/serilog/serilog-aspnetcore/blob/dev/src/Serilog.AspNetCore/AspNetCore/RequestLoggingMiddleware.cs
    public class FooLoggerFactory
        : Microsoft.Extensions.Logging.ILoggerFactory
    {

        protected Microsoft.Extensions.Logging.ILoggerProvider m_provider;

        public FooLoggerFactory(Microsoft.Extensions.Logging.ILoggerProvider provider)
        {
            this.m_provider = provider;
        }


        public FooLoggerFactory()
            :this(new FooLoggerProvider())
        { }


        void Microsoft.Extensions.Logging.ILoggerFactory.AddProvider(
            Microsoft.Extensions.Logging.ILoggerProvider provider)
        {
            this.m_provider = provider;

            throw new System.InvalidOperationException("Ignoring added logger provider");
        }

        Microsoft.Extensions.Logging.ILogger Microsoft.Extensions.Logging.ILoggerFactory
            .CreateLogger(string categoryName)
        {
            return this.m_provider.CreateLogger(categoryName);
        }

        void System.IDisposable.Dispose()
        {
            this.m_provider.Dispose();
        }

    }


}
