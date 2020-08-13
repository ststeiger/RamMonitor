
namespace RamMonitorPrototype
{
    

    public class FileLoggerProvider
        : Microsoft.Extensions.Logging.ILoggerProvider
    {

        protected System.Collections.Concurrent.ConcurrentDictionary<
            string, Microsoft.Extensions.Logging.ILogger
            > m_loggers;


        public FileLoggerProvider()
        {
            this.m_loggers = new System.Collections.Concurrent.ConcurrentDictionary<
                            string, Microsoft.Extensions.Logging.ILogger
                        >(System.StringComparer.InvariantCultureIgnoreCase);
        } // End Constructor 



        Microsoft.Extensions.Logging.ILogger Microsoft.Extensions.Logging.ILoggerProvider
            .CreateLogger(string categoryName)
        {
            return this.m_loggers.GetOrAdd(categoryName,
                 delegate (string category)
                 {
                     return new FileLogger(this, category);
                 }
             );

        } // End Function CreateLogger 


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

    } // End Class FileLoggerProvider 


}
