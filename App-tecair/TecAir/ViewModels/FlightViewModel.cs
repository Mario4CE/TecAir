using System.Collections.ObjectModel;
using System.Diagnostics;
using TecAir.Models;
using TecAir.Services;

namespace TecAir.ViewModels
{
    /// <summary>
    /// ViewModel para búsqueda y gestión de vuelos
    /// </summary>
    public class FlightViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<FlightWithRoute> _flights;
        private ObservableCollection<Airport> _airports;
        private Flight _selectedFlight;
        private Airport _selectedOrigin;
        private Airport _selectedDestination;

        public ObservableCollection<FlightWithRoute> Flights
        {
            get => _flights;
            set => SetProperty(ref _flights, value);
        }

        public ObservableCollection<Airport> Airports
        {
            get => _airports;
            set => SetProperty(ref _airports, value);
        }

        public Flight SelectedFlight
        {
            get => _selectedFlight;
            set => SetProperty(ref _selectedFlight, value);
        }

        public Airport SelectedOrigin
        {
            get => _selectedOrigin;
            set => SetProperty(ref _selectedOrigin, value);
        }

        public Airport SelectedDestination
        {
            get => _selectedDestination;
            set => SetProperty(ref _selectedDestination, value);
        }

        public FlightViewModel()
        {
            Title = "Búsqueda de Vuelos";
            _databaseService = MauiProgram.DatabaseService;
            Flights = new ObservableCollection<FlightWithRoute>();
            Airports = new ObservableCollection<Airport>();
        }

        public async Task InitializeAsync()
        {
            await LoadAirportsAsync();
            // No cargar todos los vuelos inicialmente - mostrar lista vacía hasta buscar
            Flights.Clear();
        }

        public async Task LoadAirportsAsync()
        {
            try
            {
                IsBusy = true;
                var airports = await _databaseService.GetAllAirportsAsync();
                Airports.Clear();
                foreach (var airport in airports)
                {
                    Airports.Add(airport);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar aeropuertos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task SearchFlightsAsync()
        {
            try
            {
                IsBusy = true;

                if (SelectedOrigin == null || SelectedDestination == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Selecciona origen y destino", "OK");
                    Flights.Clear();
                    return;
                }

                if (SelectedOrigin.Id == SelectedDestination.Id)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El origen y destino no pueden ser iguales", "OK");
                    Flights.Clear();
                    return;
                }

                // Buscar vuelos usando Origin/Destination sincronizados
                // Airport.Name ya contiene el formato completo "CODE - NombreLargo"
                // Ejemplo: "SJO - Juan Santamaría", "LIR - Guanacaste"
                var origin = SelectedOrigin.Name;
                var destination = SelectedDestination.Name;

                // ================= DEBUG =================
                var allFlights = await _databaseService.GetAllFlightsAsync();

                Debug.WriteLine("======================================");
                Debug.WriteLine("VUELOS GUARDADOS EN SQLITE");
                Debug.WriteLine($"Cantidad total: {allFlights.Count}");
                Debug.WriteLine("======================================");

                foreach (var f in allFlights)
                {
                    Debug.WriteLine(
                        $"Flight={f.FlightNumber} | Origin='{f.Origin}' | Destination='{f.Destination}' | RouteId={f.RouteId}"
                    );
                }

                Debug.WriteLine("======================================");
                Debug.WriteLine(
                    $"BUSCANDO Origin='{origin}' Destination='{destination}'"
                );
                Debug.WriteLine("======================================");
                // =========================================

                var flights = await _databaseService.GetFlightsByOriginDestinationAsync(
                    origin,
                    destination);

                if (flights == null || flights.Count == 0)
                {
                    Flights.Clear();

                    Debug.WriteLine(
                        $"[SearchFlightsAsync] No se encontraron vuelos para {origin} → {destination}"
                    );

                    await Application.Current.MainPage.DisplayAlert(
                        "Información",
                        "No hay vuelos disponibles para esta ruta",
                        "OK");

                    return;
                }

                Flights.Clear();

                foreach (var flight in flights)
                {
                    var flightWithRoute = new FlightWithRoute
                    {
                        Flight = flight,
                        Route = null,
                        OriginAirport = SelectedOrigin,
                        DestinationAirport = SelectedDestination
                    };

                    Flights.Add(flightWithRoute);
                }

                Debug.WriteLine(
                    $"[SearchFlightsAsync] Se encontraron {flights.Count} vuelos para {origin} → {destination}"
                );
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Error en búsqueda: {ex.Message}",
                    "OK");

                Debug.WriteLine($"[SearchFlightsAsync] ERROR: {ex}");

                Flights.Clear();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
