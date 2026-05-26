using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TecAir.Models;
using Microsoft.Maui.Networking;

namespace TecAir.Services
{
    /// <summary>
    /// Servicio de sincronización entre la base de datos local (SQLite)
    /// y el API remoto (PostgreSQL).
    ///
    /// Flujo de sincronización:
    ///   1. Verifica si hay conexión a internet
    ///   2. Si hay conexión, descarga datos del API (vuelos, promociones, etc.)
    ///   3. Sube los datos locales pendientes (reservaciones, usuarios nuevos)
    ///   4. Marca los registros como sincronizados
    /// </summary>
    public class SyncService
    {
        private readonly DatabaseService _databaseService;
        private readonly HttpClient _httpClient;

        // URL base del API — cambiar si el servidor está en otra dirección
        private const string ApiBaseUrl = "http://172.18.34.148:5000/api";
        // Nota: en Android el emulador usa 10.0.2.2 para acceder al localhost
        // Si se prueba en dispositivo físico, usar la IP local del servidor
        // Ejemplo: "http://192.168.1.100:5000/api"

        // Opciones de serialización — el API usa snake_case
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
        };

        public SyncService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        /// <summary>
        /// Verifica si hay conexión a internet disponible
        /// </summary>
        public bool HayConexion()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
        }

        /// <summary>
        /// Ejecuta la sincronización completa.
        /// Descarga datos del API y sube los pendientes locales.
        /// </summary>
        public async Task<(bool exito, string mensaje)> SincronizarAsync()
        {
            // Si no hay conexión, no se puede sincronizar
            if (!HayConexion())
                return (false, "Sin conexión a internet. Los datos se sincronizarán cuando haya conexión.");

            try
            {
                // Descarga datos del API hacia SQLite local
                await DescargarVuelosAsync();
                await DescargarPromocionesAsync();
                await DescargarAeropuertosAsync();

                // Sube datos locales pendientes al API
                await SubirUsuariosPendientesAsync();
                await SubirReservacionesPendientesAsync();

                return (true, "Sincronización completada correctamente.");
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Error de conexión al API: {ex.Message}");
                return (false, "No se pudo conectar al servidor. Intente más tarde.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error durante sincronización: {ex.Message}");
                return (false, $"Error durante sincronización: {ex.Message}");
            }
        }

        /// <summary>
        /// Descarga los vuelos disponibles del API y los guarda en SQLite.
        /// Actualiza los existentes y agrega los nuevos.
        /// </summary>
        private async Task DescargarVuelosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/vuelos");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<VuelosResponse>(json, JsonOptions);
                if (data?.Vuelos == null) return;

                foreach (var vueloApi in data.Vuelos)
                {
                    // Busca si ya existe el vuelo en local
                    var vueloLocal = await _databaseService.GetFlightByApiIdAsync(vueloApi.IdVuelo);

                    if (vueloLocal == null)
                    {
                        // No existe — lo crea
                        await _databaseService.CreateFlightAsync(new Flight
                        {
                            ApiId = vueloApi.IdVuelo,
                            FlightNumber = $"TEC-{vueloApi.IdVuelo:D3}",
                            RouteId = vueloApi.IdRuta,
                            DepartureTime = DateTime.Parse($"{vueloApi.FechaSalida} {vueloApi.HoraSalida}"),
                            ArrivalTime = DateTime.Parse($"{vueloApi.FechaSalida} {vueloApi.HoraSalida}"),
                            Status = MapearEstado(vueloApi.Estado),
                            AvailableSeats = vueloApi.AsientosDisponibles,
                        });
                    }
                    else
                    {
                        // Ya existe — actualiza el estado y asientos
                        vueloLocal.Status = MapearEstado(vueloApi.Estado);
                        vueloLocal.AvailableSeats = vueloApi.AsientosDisponibles;
                        await _databaseService.UpdateFlightAsync(vueloLocal);
                    }
                }

                Debug.WriteLine($"Vuelos sincronizados: {data.Vuelos.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error descargando vuelos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Descarga las promociones activas del API y las guarda en SQLite.
        /// </summary>
        private async Task DescargarPromocionesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/promociones");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<PromocionesResponse>(json, JsonOptions);
                if (data?.Promociones == null) return;

                foreach (var promoApi in data.Promociones)
                {
                    var promoLocal = await _databaseService.GetPromotionByApiIdAsync(promoApi.IdPromocion);

                    if (promoLocal == null)
                    {
                        await _databaseService.CreatePromotionAsync(new Promotion
                        {
                            ApiId = promoApi.IdPromocion,
                            OriginAirportId = promoApi.IdRuta,
                            PromotionalPrice = promoApi.Precio,
                            StartDate = promoApi.FechaInicio?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now,
                            EndDate = promoApi.FechaFin?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.Now.AddMonths(1),
                            ImageUrl = promoApi.Imagen ?? "",
                        });
                    }
                }

                Debug.WriteLine($"Promociones sincronizadas: {data.Promociones.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error descargando promociones: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Descarga los aeropuertos del API y los guarda en SQLite.
        /// </summary>
        private async Task DescargarAeropuertosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/aeropuertos");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<AeropuertosResponse>(json, JsonOptions);
                if (data?.Aeropuertos == null) return;

                foreach (var aeropuertoApi in data.Aeropuertos)
                {
                    var aeropuertoLocal = await _databaseService.GetAirportByApiIdAsync(aeropuertoApi.IdAeropuerto);

                    if (aeropuertoLocal == null)
                    {
                        await _databaseService.CreateAirportAsync(new Airport
                        {
                            ApiId = aeropuertoApi.IdAeropuerto,
                            Name = aeropuertoApi.Nombre,
                            City = aeropuertoApi.Ubicacion ?? "",
                            Country = "",
                            Code = aeropuertoApi.Nombre.Split(' ')[0], // ej. "SJO" de "SJO - Juan Santamaría"
                        });
                    }
                }

                Debug.WriteLine($"Aeropuertos sincronizados: {data.Aeropuertos.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error descargando aeropuertos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sube al API los usuarios registrados localmente que aún no
        /// han sido sincronizados (IsSynced = false).
        /// </summary>
        private async Task SubirUsuariosPendientesAsync()
        {
            try
            {
                var usuariosPendientes = await _databaseService.GetUnsyncedUsersAsync();
                if (usuariosPendientes == null || usuariosPendientes.Count == 0) return;

                foreach (var usuario in usuariosPendientes)
                {
                    // Arma el objeto que espera el API
                    var body = new
                    {
                        nombre1 = usuario.FullName.Split(' ')[0],
                        nombre2 = "",
                        apellido1 = usuario.FullName.Contains(' ')
                            ? usuario.FullName.Split(' ')[1]
                            : "",
                        apellido2 = "",
                        telefono = usuario.Phone,
                        correo = usuario.Email,
                        es_estudiante = usuario.IsStudent,
                        universidad = usuario.University ?? "",
                        carnet = usuario.StudentID ?? "",
                        millas = (int)usuario.LoyaltyMiles,
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(body, JsonOptions),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await _httpClient.PostAsync($"{ApiBaseUrl}/usuarios", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Marca el usuario como sincronizado en SQLite
                        usuario.IsSynced = true;
                        await _databaseService.UpdateUserAsync(usuario);
                        Debug.WriteLine($"Usuario sincronizado: {usuario.Email}");
                    }
                    else
                    {
                        Debug.WriteLine($"Error subiendo usuario {usuario.Email}: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error subiendo usuarios: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sube al API las reservaciones hechas offline que aún no
        /// han sido sincronizadas (IsSynced = false).
        /// </summary>
        private async Task SubirReservacionesPendientesAsync()
        {
            try
            {
                var reservacionesPendientes = await _databaseService.GetUnsyncedReservationsAsync();
                if (reservacionesPendientes == null || reservacionesPendientes.Count == 0) return;

                foreach (var reservacion in reservacionesPendientes)
                {
                    // Necesitamos el ApiId del usuario y del vuelo
                    var usuario = await _databaseService.GetUserByIdAsync(reservacion.UserId);
                    var vuelo = await _databaseService.GetFlightByIdAsync(reservacion.FlightId);

                    // Si alguno no tiene ApiId aún, lo saltamos
                    if (usuario == null || vuelo == null || vuelo.ApiId == 0) continue;

                    var body = new
                    {
                        id_usuario = usuario.ApiId,
                        id_vuelo = vuelo.ApiId,
                        estado = "pendiente_pago",
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(body, JsonOptions),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await _httpClient.PostAsync($"{ApiBaseUrl}/reservaciones", content);

                    if (response.IsSuccessStatusCode)
                    {
                        reservacion.IsSynced = true;
                        await _databaseService.UpdateReservationAsync(reservacion);
                        Debug.WriteLine($"Reservación sincronizada: #{reservacion.Id}");
                    }
                    else
                    {
                        Debug.WriteLine($"Error subiendo reservación #{reservacion.Id}: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error subiendo reservaciones: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Convierte el estado del API (string) al estado local (int)
        /// 0: Scheduled, 1: CheckedIn, 2: Open, 3: Closed, 4: Cancelled
        /// </summary>
        private static int MapearEstado(string estado) => estado switch
        {
            "programado" => 0,
            "abierto" => 2,
            "cerrado" => 3,
            _ => 0,
        };
    }

    // Clases auxiliares para deserializar las respuestas del API

    internal class VuelosResponse
    {
        public List<VueloApiDto> Vuelos { get; set; } = [];
    }

    internal class VueloApiDto
    {
        public int IdVuelo { get; set; }
        public string FechaSalida { get; set; } = "";
        public string HoraSalida { get; set; } = "";
        public string Puerta { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Matricula { get; set; } = "";
        public int IdRuta { get; set; }
        public decimal Precio { get; set; }
        public int AsientosDisponibles { get; set; }
    }

    internal class PromocionesResponse
    {
        public List<PromocionApiDto> Promociones { get; set; } = [];
    }

    internal class PromocionApiDto
    {
        public int IdPromocion { get; set; }
        public decimal Precio { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
        public string? Imagen { get; set; }
        public int IdRuta { get; set; }
    }

    internal class AeropuertosResponse
    {
        public List<AeropuertoApiDto> Aeropuertos { get; set; } = [];
    }

    internal class AeropuertoApiDto
    {
        public int IdAeropuerto { get; set; }
        public string Nombre { get; set; } = "";
        public string Ubicacion { get; set; } = "";
    }
}