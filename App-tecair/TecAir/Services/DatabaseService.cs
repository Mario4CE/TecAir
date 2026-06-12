using SQLite;
using System.Diagnostics;
using TecAir.Models;

namespace TecAir.Services
{
    /// <summary>
    /// Servicio de base de datos local (SQLite) para operaciones sin conexión
    /// </summary>
    public class DatabaseService
    {
        private SQLiteAsyncConnection _connection;
        private static readonly string DbPath = Path.Combine(FileSystem.AppDataDirectory, "tecair.db");
        private bool _isInitialized = false;
        private readonly object _connectionLock = new object();

        public DatabaseService()
        {
        }

        /// <summary>
        /// Asegura que la conexión esté disponible y sincronizada
        /// </summary>
        private async Task EnsureConnectionAsync()
        {
            // Verificación rápida sin lock
            if (_connection != null && _isInitialized)
                return;

            // Lock para evitar race conditions
            lock (_connectionLock)
            {
                // Verificación después de adquirir el lock
                if (_connection != null && _isInitialized)
                    return;

                try
                {
                    if (_connection == null)
                    {
                        var dbConnection = new SQLiteAsyncConnection(DbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
                        _connection = dbConnection;
                    }

                    if (!_isInitialized)
                    {
                        // Crear tablas si no existen
                        _connection.CreateTableAsync<User>().Wait();
                        _connection.CreateTableAsync<Airport>().Wait();
                        _connection.CreateTableAsync<Aircraft>().Wait();
                        _connection.CreateTableAsync<Route>().Wait();
                        _connection.CreateTableAsync<Flight>().Wait();
                        _connection.CreateTableAsync<Reservation>().Wait();
                        _connection.CreateTableAsync<Luggage>().Wait();
                        _connection.CreateTableAsync<Promotion>().Wait();
                        _connection.CreateTableAsync<BoardingPass>().Wait();

                        _isInitialized = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Inicializa la conexión a la base de datos y crea las tablas
        /// </summary>
        public async Task InitializeAsync()
        {
            await EnsureConnectionAsync();
            await SeedDataAsync();
        }

        /// <summary>
        /// Carga datos iniciales si la base de datos está vacía
        /// Nota: Con la conexión al API, los datos se sincronizan automáticamente
        /// </summary>
        private async Task SeedDataAsync()
        {
            // Sin datos de demostración. Los datos se cargan desde el API mediante sincronización.
            await Task.CompletedTask;
        }

        // ==================== USUARIOS ====================
        public async Task<User> CreateUserAsync(User user)
        {
            await EnsureConnectionAsync();
            await _connection.InsertAsync(user);
            return user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            await EnsureConnectionAsync();
            return await _connection.Table<User>().ToListAsync();
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            await EnsureConnectionAsync();
            return await _connection.UpdateAsync(user);
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            await EnsureConnectionAsync();
            return await _connection.DeleteAsync<User>(id);
        }

        // ==================== AEROPUERTOS ====================
        public async Task<Airport> GetAirportByIdAsync(int id)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Airport>().Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Airport> GetAirportByCodeAsync(string code)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Airport>().Where(a => a.Code == code).FirstOrDefaultAsync();
        }

        public async Task<List<Airport>> GetAllAirportsAsync()
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Airport>().ToListAsync();
        }

        // ==================== VUELOS ====================
        public async Task<Flight> GetFlightByIdAsync(int id)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Flight>().Where(f => f.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Flight>> GetFlightsByRouteAsync(int routeId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Flight>().Where(f => f.RouteId == routeId).ToListAsync();
        }

        /// <summary>
        /// Busca vuelos por origen y destino directamente (sin usar tabla Routes)
        /// Útil cuando RouteId del API no coincide con Route.Id local
        /// </summary>
        public async Task<List<Flight>> GetFlightsByOriginDestinationAsync(string origin, string destination)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Flight>()
                .Where(f => f.Origin == origin && f.Destination == destination)
                .ToListAsync();
        }

        public async Task<List<Flight>> GetAllFlightsAsync()
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Flight>().ToListAsync();
        }

        public async Task<int> UpdateFlightAsync(Flight flight)
        {
            await EnsureConnectionAsync();
            try
            {
                var result = await _connection.UpdateAsync(flight);
                Debug.WriteLine($"Vuelo {flight.Id} actualizado. Asientos disponibles: {flight.AvailableSeats}, Resultado: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar vuelo {flight.Id}: {ex.Message}");
                throw;
            }
        }

        // ==================== RUTAS ====================
        public async Task<Route> GetRouteByIdAsync(int id)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Route>().Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Route>> GetAllRoutesAsync()
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Route>().ToListAsync();
        }

        public async Task<Route> GetRouteByAirportsAsync(int originId, int destinationId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Route>()
                .Where(r => r.OriginAirportId == originId && r.DestinationAirportId == destinationId)
                .FirstOrDefaultAsync();
        }

        // ==================== RESERVACIONES ====================
        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            await EnsureConnectionAsync();
            await _connection.InsertAsync(reservation);
            return reservation;
        }

        public async Task<Reservation> GetReservationByIdAsync(int id)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Reservation>().Where(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Reservation>> GetReservationsByUserAsync(int userId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Reservation>().Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsByFlightAsync(int flightId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Reservation>().Where(r => r.FlightId == flightId).ToListAsync();
        }

        public async Task<List<string>> GetReservedSeatsForFlightAsync(int flightId)
        {
            await EnsureConnectionAsync();
            // Obtener todos los asientos reservados (status != 4 Cancelada, != 5 NoShow)
            var reservations = await _connection.Table<Reservation>()
                .Where(r => r.FlightId == flightId && r.Status != 4 && r.Status != 5)
                .ToListAsync();

            return reservations.Select(r => r.SeatNumber).ToList();
        }

        public async Task<int> UpdateReservationAsync(Reservation reservation)
        {
            await EnsureConnectionAsync();
            try
            {
                // Asegurar que el registro existe antes de actualizar
                var existing = await _connection.Table<Reservation>()
                    .Where(r => r.Id == reservation.Id)
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    Debug.WriteLine($"Reservación con ID {reservation.Id} no encontrada");
                    return 0;
                }

                var result = await _connection.UpdateAsync(reservation);
                Debug.WriteLine($"Reservación {reservation.Id} actualizada. Status: {reservation.Status}, Resultado: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar reservación {reservation.Id}: {ex.Message}");
                throw;
            }
        }

        public async Task<int> DeleteReservationAsync(int id)
        {
            await EnsureConnectionAsync();
            return await _connection.DeleteAsync<Reservation>(id);
        }

        // ==================== MALETAS ====================
        public async Task<Luggage> CreateLuggageAsync(Luggage luggage)
        {
            await EnsureConnectionAsync();
            await _connection.InsertAsync(luggage);
            return luggage;
        }

        public async Task<List<Luggage>> GetLuggageByReservationAsync(int reservationId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Luggage>().Where(l => l.ReservationId == reservationId).ToListAsync();
        }

        public async Task<int> DeleteLuggageAsync(int id)
        {
            await EnsureConnectionAsync();
            return await _connection.DeleteAsync<Luggage>(id);
        }

        // ==================== PROMOCIONES ====================
        public async Task<List<Promotion>> GetActivePromotionsAsync()
        {
            await EnsureConnectionAsync();
            var now = DateTime.Now;
            return await _connection.Table<Promotion>()
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .ToListAsync();
        }

        public async Task<List<Promotion>> GetAllPromotionsAsync()
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Promotion>().ToListAsync();
        }

        // ==================== PASES DE ABORDAR ====================
        public async Task<BoardingPass> CreateBoardingPassAsync(BoardingPass boardingPass)
        {
            await EnsureConnectionAsync();
            await _connection.InsertAsync(boardingPass);
            return boardingPass;
        }

        public async Task<BoardingPass> GetBoardingPassByReservationAsync(int reservationId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<BoardingPass>()
                .Where(bp => bp.ReservationId == reservationId)
                .FirstOrDefaultAsync();
        }

        // Busca un vuelo por su ID del API
        public async Task<Flight> GetFlightByApiIdAsync(int apiId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Flight>()
                .Where(f => f.ApiId == apiId)
                .FirstOrDefaultAsync();
        }

        // Busca una promoción por su ID del API
        public async Task<Promotion> GetPromotionByApiIdAsync(int apiId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Promotion>()
                .Where(p => p.ApiId == apiId)
                .FirstOrDefaultAsync();
        }

        // Busca un aeropuerto por su ID del API
        public async Task<Airport> GetAirportByApiIdAsync(int apiId)
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Airport>()
                .Where(a => a.ApiId == apiId)
                .FirstOrDefaultAsync();
        }

        // Devuelve usuarios que aún no han sido sincronizados
        public async Task<List<User>> GetUnsyncedUsersAsync()
        {
            await EnsureConnectionAsync();
            return await _connection.Table<User>()
                .Where(u => !u.IsSynced)
                .ToListAsync();
        }

        // Devuelve reservaciones que aún no han sido sincronizadas
        public async Task<List<Reservation>> GetUnsyncedReservationsAsync()
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Reservation>()
                .Where(r => !r.IsSynced)
                .ToListAsync();
        }

        // Crea un aeropuerto en SQLite
        public async Task<Airport> CreateAirportAsync(Airport airport)
        {
            await EnsureConnectionAsync();
            await _connection.InsertAsync(airport);
            return airport;
        }

        // Crea una promoción en SQLite
        public async Task<Promotion> CreatePromotionAsync(Promotion promotion)
        {
            await EnsureConnectionAsync();
            await _connection.InsertAsync(promotion);
            return promotion;
        }

        // Crea un vuelo en SQLite
        public async Task<Flight> CreateFlightAsync(Flight flight)
        {
            await EnsureConnectionAsync();
            await _connection.InsertAsync(flight);
            return flight;
        }

    }
}
