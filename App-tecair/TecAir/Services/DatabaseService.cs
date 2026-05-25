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

        public DatabaseService()
        {
        }

        /// <summary>
        /// Asegura que la conexión esté disponible
        /// </summary>
        private async Task EnsureConnectionAsync()
        {
            if (_connection != null && _isInitialized)
                return;

            try
            {
                _connection = new SQLiteAsyncConnection(DbPath);
                
                // Crear tablas si no existen
                await _connection.CreateTableAsync<User>();
                await _connection.CreateTableAsync<Airport>();
                await _connection.CreateTableAsync<Aircraft>();
                await _connection.CreateTableAsync<Route>();
                await _connection.CreateTableAsync<Flight>();
                await _connection.CreateTableAsync<Reservation>();
                await _connection.CreateTableAsync<Luggage>();
                await _connection.CreateTableAsync<Promotion>();
                await _connection.CreateTableAsync<BoardingPass>();

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
                throw;
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
        /// </summary>
        private async Task SeedDataAsync()
        {
            // Verificar si ya hay datos
            var userCount = await _connection.Table<User>().CountAsync();
            if (userCount > 0)
                return;

            // Crear usuario por defecto para demostración
            var defaultUser = new User
            {
                FullName = "Usuario Demo",
                Email = "demorera@estudiantec.cr",
                Password = "1234",  // ← Contraseña demo
                Phone = "+506 8765-4321",
                IsStudent = true,
                University = "Instituto Tecnológico de Costa Rica",
                StudentID = "2020001234",
                LoyaltyMiles = 5250,
                CreatedAt = DateTime.Now,
                Role = 0 // Customer
            };
            await _connection.InsertAsync(defaultUser);

            // Crear aeropuertos
            var airports = new List<Airport>
            {
                new() { Code = "SJO", Name = "Juan Manuel Gálvez International", City = "San José", Country = "Costa Rica" },
                new() { Code = "LAX", Name = "Los Angeles International", City = "Los Angeles", Country = "United States" },
                new() { Code = "MIA", Name = "Miami International", City = "Miami", Country = "United States" },
                new() { Code = "MAD", Name = "Adolfo Suárez Madrid", City = "Madrid", Country = "Spain" },
                new() { Code = "CCS", Name = "Simón Bolívar International", City = "Caracas", Country = "Venezuela" }
            };
            await _connection.InsertAllAsync(airports);

            // Crear aeronaves
            var aircrafts = new List<Aircraft>
            {
                new() { Registration = "N12345", Model = "Boeing 737", Capacity = 180, Manufacturer = "Boeing" },
                new() { Registration = "N67890", Model = "Airbus A320", Capacity = 195, Manufacturer = "Airbus" },
                new() { Registration = "N11111", Model = "Boeing 757", Capacity = 239, Manufacturer = "Boeing" }
            };
            await _connection.InsertAllAsync(aircrafts);

            // Crear rutas
            var routes = new List<Route>
            {
                new() { OriginAirportId = 1, DestinationAirportId = 2, BasePrice = 450, Duration = 360 },
                new() { OriginAirportId = 1, DestinationAirportId = 3, BasePrice = 420, Duration = 300 },
                new() { OriginAirportId = 2, DestinationAirportId = 4, BasePrice = 650, Duration = 480 },
                new() { OriginAirportId = 1, DestinationAirportId = 5, BasePrice = 380, Duration = 240 }
            };
            await _connection.InsertAllAsync(routes);

            // Crear vuelos
            var flights = new List<Flight>
            {
                new() 
                { 
                    FlightNumber = "TC-001", 
                    RouteId = 1, 
                    AircraftId = 1, 
                    DepartureTime = DateTime.Now.AddDays(1).Date.AddHours(8),
                    ArrivalTime = DateTime.Now.AddDays(1).Date.AddHours(14),
                    Status = 0,
                    AvailableSeats = 180
                },
                new() 
                { 
                    FlightNumber = "TC-002", 
                    RouteId = 2, 
                    AircraftId = 2, 
                    DepartureTime = DateTime.Now.AddDays(2).Date.AddHours(10),
                    ArrivalTime = DateTime.Now.AddDays(2).Date.AddHours(14),
                    Status = 0,
                    AvailableSeats = 195
                },
                new() 
                { 
                    FlightNumber = "TC-003", 
                    RouteId = 3, 
                    AircraftId = 3, 
                    DepartureTime = DateTime.Now.AddDays(3).Date.AddHours(15),
                    ArrivalTime = DateTime.Now.AddDays(4).Date.AddHours(6),
                    Status = 0,
                    AvailableSeats = 239
                }
            };
            await _connection.InsertAllAsync(flights);

            // Crear promociones
            var promotions = new List<Promotion>
            {
                new() 
                { 
                    OriginAirportId = 1,
                    DestinationAirportId = 2,
                    PromotionalPrice = 350,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(30),
                    Description = "Promoción especial para estudiantes",
                    IsActive = true
                },
                new() 
                { 
                    OriginAirportId = 1,
                    DestinationAirportId = 5,
                    PromotionalPrice = 280,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(15),
                    Description = "Vuelo económico a Venezuela",
                    IsActive = true
                }
            };
            await _connection.InsertAllAsync(promotions);

            // Crear reservaciones de demostración
            var reservations = new List<Reservation>
            {
                new()
                {
                    UserId = 1,
                    FlightId = 1,
                    SeatNumber = "12A",
                    ReservationDate = DateTime.Now,
                    Status = 0, // Pending
                    TotalPrice = 450
                },
                new()
                {
                    UserId = 1,
                    FlightId = 2,
                    SeatNumber = "5B",
                    ReservationDate = DateTime.Now.AddDays(-2),
                    Status = 1, // Confirmed
                    TotalPrice = 420
                }
            };
            await _connection.InsertAllAsync(reservations);
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

        public async Task<List<Flight>> GetAllFlightsAsync()
        {
            await EnsureConnectionAsync();
            return await _connection.Table<Flight>().ToListAsync();
        }

        public async Task<int> UpdateFlightAsync(Flight flight)
        {
            await EnsureConnectionAsync();
            return await _connection.UpdateAsync(flight);
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

        public async Task<int> UpdateReservationAsync(Reservation reservation)
        {
            await EnsureConnectionAsync();
            return await _connection.UpdateAsync(reservation);
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
    }
}
