using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;

namespace TecAir.Services
{
    /// <summary>
    /// Servicio de sincronización automática en segundo plano.
    /// 
    /// Características:
    /// - Ejecuta sincronización cada 1 minuto
    /// - Verifica conectividad antes de sincronizar
    /// - Se pausa cuando la app entra en background
    /// - Se reanuda cuando la app entra en foreground
    /// - Registra errores sin interrumpir la aplicación
    /// </summary>
    public class AutoSyncService : IDisposable
    {
        private readonly SyncService _syncService;
        private Timer _syncTimer;
        private bool _isRunning;
        private bool _isDisposed;

        // Intervalo de sincronización en milisegundos (1 minuto = 60000 ms)
        private const int SyncIntervalMs = 10000;

        // Delay inicial para dar tiempo a que la app se inicialice completamente
        private const int InitialDelayMs = 5000;

        public AutoSyncService(SyncService syncService)
        {
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _isRunning = false;
            _isDisposed = false;
        }

        /// <summary>
        /// Inicia el servicio de sincronización automática.
        /// Se ejecuta una vez inmediatamente y luego cada SyncIntervalMs.
        /// </summary>
        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AutoSyncService));

            if (_isRunning)
            {
                Debug.WriteLine("[AutoSyncService] Ya está ejecutándose.");
                return;
            }

            _isRunning = true;
            Debug.WriteLine("[AutoSyncService] Iniciando servicio de sincronización automática...");

            // Ejecutar sincronización inicial después de un delay
            _syncTimer = new Timer(
                callback: async _ => await ExecuteSyncCycleAsync(),
                state: null,
                dueTime: InitialDelayMs,
                period: SyncIntervalMs
            );
        }

        /// <summary>
        /// Detiene el servicio de sincronización automática.
        /// Se llama automáticamente cuando la app entra en background.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _syncTimer?.Dispose();
            _syncTimer = null;
            Debug.WriteLine("[AutoSyncService] Servicio de sincronización automática detenido.");
        }

        /// <summary>
        /// Reanuda la sincronización automática cuando la app vuelve a foreground.
        /// </summary>
        public void Resume()
        {
            if (_isDisposed || _isRunning)
                return;

            Debug.WriteLine("[AutoSyncService] Reanudando sincronización automática...");
            Start();
        }

        /// <summary>
        /// Ejecuta un ciclo de sincronización verificando conectividad.
        /// </summary>
        private async Task ExecuteSyncCycleAsync()
        {
            try
            {
                // Verificar conectividad
                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    Debug.WriteLine("[AutoSyncService] Sin conexión a Internet. Sincronización omitida.");
                    return;
                }

                Debug.WriteLine("[AutoSyncService] Iniciando ciclo de sincronización...");

                // Ejecutar sincronización en el hilo principal para evitar excepciones de threading
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        var resultado = await _syncService.SincronizarAsync();

                        if (resultado.exito)
                        {
                            Debug.WriteLine($"[AutoSyncService] Sincronización exitosa: {resultado.mensaje}");
                        }
                        else
                        {
                            Debug.WriteLine($"[AutoSyncService] Sincronización con advertencia: {resultado.mensaje}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[AutoSyncService] Error durante sincronización: {ex.Message}");
                        Debug.WriteLine($"[AutoSyncService] StackTrace: {ex.StackTrace}");
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AutoSyncService] Error en ciclo de sincronización: {ex.Message}");
                Debug.WriteLine($"[AutoSyncService] StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Ejecuta una sincronización manual inmediata (para operaciones críticas).
        /// Útil después de registrar usuarios, crear reservaciones, etc.
        /// </summary>
        public async Task SincronizarAhoraAsync()
        {
            try
            {
                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    Debug.WriteLine("[AutoSyncService] Sin conexión a Internet. Sincronización manual omitida.");
                    return;
                }

                Debug.WriteLine("[AutoSyncService] Ejecutando sincronización manual inmediata...");
                var resultado = await _syncService.SincronizarAsync();

                if (resultado.exito)
                {
                    Debug.WriteLine($"[AutoSyncService] Sincronización manual exitosa: {resultado.mensaje}");
                }
                else
                {
                    Debug.WriteLine($"[AutoSyncService] Sincronización manual con advertencia: {resultado.mensaje}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AutoSyncService] Error en sincronización manual: {ex.Message}");
                Debug.WriteLine($"[AutoSyncService] StackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Libera los recursos del servicio.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            Stop();
            _syncTimer?.Dispose();
            _isDisposed = true;
            Debug.WriteLine("[AutoSyncService] Recursos liberados.");
        }
    }
}
