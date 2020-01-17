
// using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Logging;


namespace RamMonitorPrototype
{


    public class TheService
        : Microsoft.Extensions.Hosting.IHostedService,
            System.IDisposable
    {

        protected bool m_run;
        protected System.IServiceProvider m_sp;
        protected Microsoft.Extensions.Logging.ILogger<TheService> m_logger;


        public TheService(
             System.IServiceProvider sp,
             Microsoft.Extensions.Logging.ILogger<TheService> logger
         )
        {
            this.m_sp = sp;
            this.m_logger = logger;
        }

        async System.Threading.Tasks.Task Microsoft.Extensions.Hosting.IHostedService
            .StartAsync(System.Threading.CancellationToken cancellationToken)
        {
            

            this.m_run = true;
            using (this.m_logger.BeginScope("Checking mail"))
            {
                // Scope is "Checking mail"
                this.m_logger.LogInformation("Opening SMTP connection");

                using (this.m_logger.BeginScope("Downloading messages"))
                {
                    // Scope is "Checking mail" -> "Downloading messages"
                    this.m_logger.LogError("Connection interrupted");
                } // End Scope Opening SMTP connection 

            } // End Scope Checking mail


            while (this.m_run)
            {
                this.m_logger.LogInformation("foobar", "foo", 123);
                System.Console.Write("Heartbeat: ");
                System.Console.WriteLine(System.DateTime.Now.ToString("dddd, dd.MM.yyyy HH:mm:ss.fff"));
                await System.Threading.Tasks.Task.Delay(2000);
            }

        }


        async System.Threading.Tasks.Task Microsoft.Extensions.Hosting.IHostedService
            .StopAsync(System.Threading.CancellationToken cancellationToken)
        {
            this.m_run = false;
            System.Console.Write("Stopping...");
            await System.Threading.Tasks.Task.Delay(1000);
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
                    this.m_logger.LogCritical("Disposing");
                }
                
                // TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer weiter unten überschreiben.
                // TODO: große Felder auf Null setzen.

                disposedValue = true;
            }
        }

        // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
        // ~TheService() {
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


}