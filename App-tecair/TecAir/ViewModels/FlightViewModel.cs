using System.Collections.ObjectModel;
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

                // Buscar ruta
                var route = await _databaseService.GetRouteByAirportsAsync(SelectedOrigin.Id, SelectedDestination.Id);
                if (route == null)
                {
                    Flights.Clear();
                    await Application.Current.MainPage.DisplayAlert("Información", "No hay vuelos disponibles para esta ruta", "OK");
                    return;
                }

                // Obtener vuelos para la ruta
                var flights = await _databaseService.GetFlightsByRouteAsync(route.Id);
                Flights.Clear();
                
                foreach (var flight in flights)
                {
                    var flightWithRoute = new FlightWithRoute
                    {
                        Flight = flight,
                        Route = route,
                        OriginAirport = SelectedOrigin,
                        DestinationAirport = SelectedDestination
                    };
                    Flights.Add(flightWithRoute);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error en búsqueda: {ex.Message}", "OK");
                Flights.Clear();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
