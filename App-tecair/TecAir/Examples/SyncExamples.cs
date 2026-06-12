// GUÍA: Sincronización Manual en Operaciones Críticas
// =====================================================
//
// Este archivo contiene ejemplos de dónde llamar manualmente a SincronizarAsync()
// o SincronizarAhoraAsync() para mantener los datos sincronizados inmediatamente
// después de operaciones importantes.

using TecAir.Services;
using System.Diagnostics;

namespace TecAir.Examples
{
    /// <summary>
    /// Ejemplos de sincronización manual en operaciones críticas
    /// </summary>
    public class SyncExamples
    {
        private readonly AutoSyncService _autoSyncService;
        private readonly SyncService _syncService;

        public SyncExamples(AutoSyncService autoSyncService, SyncService syncService)
        {
            _autoSyncService = autoSyncService;
            _syncService = syncService;
        }

        // ========== EJEMPLO 1: Después de Registrar un Usuario ==========
        public async Task RegisterUserAndSync(string email, string password, string fullName)
        {
            try
            {
                // 1. Realizar el registro del usuario
                // var newUser = await authService.RegisterAsync(email, password, fullName);

                // 2. Sincronizar inmediatamente para asegurar que se actualiza en el servidor
                await _autoSyncService.SincronizarAhoraAsync();

                Debug.WriteLine("[SyncExamples] Usuario registrado y sincronizado");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SyncExamples] Error registrando usuario: {ex.Message}");
            }
        }

        // ========== EJEMPLO 2: Después de Crear una Reservación ==========
        public async Task CreateReservationAndSync(int flightId, int passengerId)
        {
            try
            {
                // 1. Crear la reservación
                // var reservation = await reservationService.CreateAsync(flightId, passengerId);

                // 2. Sincronizar inmediatamente para que el servidor refleje la nueva reservación
                await _autoSyncService.SincronizarAhoraAsync();

                Debug.WriteLine("[SyncExamples] Reservación creada y sincronizada");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SyncExamples] Error creando reservación: {ex.Message}");
            }
        }

        // ========== EJEMPLO 3: Después de Cancelar una Reservación ==========
        public async Task CancelReservationAndSync(int reservationId)
        {
            try
            {
                // 1. Cancelar la reservación
                // await reservationService.CancelAsync(reservationId);

                // 2. Sincronizar inmediatamente
                await _autoSyncService.SincronizarAhoraAsync();

                Debug.WriteLine("[SyncExamples] Reservación cancelada y sincronizada");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SyncExamples] Error cancelando reservación: {ex.Message}");
            }
        }

        // ========== EJEMPLO 4: Después de Actualizar Perfil de Usuario ==========
        public async Task UpdateUserProfileAndSync(int userId, string phone, string university)
        {
            try
            {
                // 1. Actualizar perfil
                // await userService.UpdateProfileAsync(userId, phone, university);

                // 2. Sincronizar inmediatamente para reflejar cambios en el servidor
                await _autoSyncService.SincronizarAhoraAsync();

                Debug.WriteLine("[SyncExamples] Perfil actualizado y sincronizado");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SyncExamples] Error actualizando perfil: {ex.Message}");
            }
        }

        // ========== EJEMPLO 5: Sincronización Manual en Page ==========
        // En cualquier Page (por ejemplo, ReservationDetailPage), inyecta AutoSyncService:

        /*
        public partial class ReservationDetailPage : ContentPage
        {
            private readonly AutoSyncService _autoSyncService;

            public ReservationDetailPage(AutoSyncService autoSyncService)
            {
                InitializeComponent();
                _autoSyncService = autoSyncService;
            }

            private async void OnConfirmReservationClicked(object sender, EventArgs e)
            {
                // 1. Confirmar reservación
                // await ConfirmReservation();

                // 2. Sincronizar inmediatamente
                await _autoSyncService.SincronizarAhoraAsync();

                // 3. Mostrar mensaje al usuario
                await DisplayAlert("Éxito", "Reservación confirmada y sincronizada", "OK");
            }
        }
        */

        // ========== EJEMPLO 6: Diferencia entre AutoSync y SincronizarAhora ==========
        /*
        AUTOSYNCSERVICE (Sincronización Automática)
        - Se ejecuta cada 1 minuto automáticamente
        - No requiere intervención del usuario
        - Ideal para mantener datos actualizados en background
        - Se pausa cuando la app entra en background
        - Se reanuda cuando la app vuelve a foreground

        SINCRONIZAR AHORA (Sincronización Inmediata)
        - Se ejecuta al llamarla explícitamente
        - Ideal después de operaciones críticas
        - Asegura que los datos se sincronicen inmediatamente
        - No espera el intervalo de 1 minuto
        - Útil para dar feedback rápido al usuario
        */

        // ========== EJEMPLO 7: Sincronización con Indicador de Carga ==========
        public async Task SyncWithLoadingIndicator()
        {
            try
            {
                // Mostrar indicador de carga
                // loadingIndicator.IsVisible = true;

                Debug.WriteLine("[SyncExamples] Iniciando sincronización...");

                // Ejecutar sincronización
                await _autoSyncService.SincronizarAhoraAsync();

                Debug.WriteLine("[SyncExamples] Sincronización completada");

                // Ocultar indicador de carga
                // loadingIndicator.IsVisible = false;

                // Mostrar mensaje de éxito
                // await DisplayAlert("Éxito", "Datos sincronizados", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SyncExamples] Error sincronizando: {ex.Message}");
                // await DisplayAlert("Error", "No se pudo sincronizar: " + ex.Message, "OK");
            }
        }

        // ========== RESUMEN: CUÁNDO USAR SINCRONIZACIÓN MANUAL ==========
        /*
        USAR SincronizarAhoraAsync() CUANDO:
        ✓ El usuario acaba de registrarse
        ✓ El usuario crea una reservación
        ✓ El usuario cancela una reservación
        ✓ El usuario actualiza su perfil
        ✓ El usuario compra millas o cupones
        ✓ Operaciones financieras o críticas
        ✓ Cuando necesitas feedback inmediato al usuario

        NO NECESITAS LLAMAR MANUALMENTE CUANDO:
        ✓ Solo estás leyendo datos (la sincronización automática lo hace cada minuto)
        ✓ Los cambios no son críticos
        ✓ Puedes esperar el intervalo de 1 minuto

        UBICACIONES COMUNES:
        - ViewModels: Después de crear/actualizar datos
        - Pages: En event handlers (botones, formularios)
        - Services: Después de operaciones importantes
        */
    }
}
