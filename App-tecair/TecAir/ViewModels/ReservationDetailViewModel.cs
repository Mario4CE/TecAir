using System.Collections.ObjectModel;
using System.Windows.Input;
using TecAir.Models;
using TecAir.Services;

namespace TecAir.ViewModels
{
    /// <summary>
    /// ViewModel para el detalle de una reservación
    /// </summary>
    public class ReservationDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private Flight _selectedFlight;
        private Airport _originAirport;
        private Airport _destinationAirport;
        private Route _route;
        private ObservableCollection<string> _selectedSeats;
        private int _luggageCount = 0;
        private decimal _luggageCost = 0;
        private decimal _baseCost = 0;
        private decimal _totalCost = 0;

        public Flight SelectedFlight
        {
            get => _selectedFlight;
            set => SetProperty(ref _selectedFlight, value);
        }

        public Airport OriginAirport
        {
            get => _originAirport;
            set => SetProperty(ref _originAirport, value);
        }

        public Airport DestinationAirport
        {
            get => _destinationAirport;
            set => SetProperty(ref _destinationAirport, value);
        }

        public ObservableCollection<string> SelectedSeats
        {
            get
            {
                if (_selectedSeats == null)
                {
                    _selectedSeats = new ObservableCollection<string>();
                    _selectedSeats.CollectionChanged += (s, e) => 
                    {
                        UpdateTotalCost();
                        OnPropertyChanged(nameof(SelectedSeatsCount));
                    };
                }
                return _selectedSeats;
            }
            set => SetProperty(ref _selectedSeats, value);
        }

        public string SelectedSeat
        {
            get => SelectedSeats.Count > 0 ? string.Join(", ", SelectedSeats) : null;
        }

        public int SelectedSeatsCount
        {
            get => SelectedSeats.Count;
        }

        public int LuggageCount
        {
            get => _luggageCount;
            set
            {
                SetProperty(ref _luggageCount, value);
                CalculateLuggageCost();
            }
        }

        public decimal LuggageCost
        {
            get => _luggageCost;
            set => SetProperty(ref _luggageCost, value);
        }

        public decimal BaseCost
        {
            get => _baseCost;
            set => SetProperty(ref _baseCost, value);
        }

        public decimal TotalCost
        {
            get => _totalCost;
            set => SetProperty(ref _totalCost, value);
        }

        public ICommand ConfirmReservationCommand { get; }

        public ReservationDetailViewModel()
        {
            _databaseService = MauiProgram.DatabaseService;
            Title = "Detalle de Reservación";
            ConfirmReservationCommand = new Command(async () => await ConfirmReservationAsync());
        }

        /// <summary>
        /// Calcula el costo de maletas según las reglas del proyecto
        /// </summary>
        private void CalculateLuggageCost()
        {
            LuggageCost = 0;

            if (LuggageCount <= 0)
            {
                UpdateTotalCost();
                return;
            }

            // Primera maleta: gratis ($0)
            // Segunda maleta: +$50
            // Tercera en adelante: +$75 cada una

            if (LuggageCount >= 2)
                LuggageCost += 50;

            if (LuggageCount >= 3)
                LuggageCost += (LuggageCount - 2) * 75;

            UpdateTotalCost();
        }

        private void UpdateTotalCost()
        {
            // Calcular costo: (precio base * cantidad de asientos) + costo de maletas
            int seatCount = SelectedSeats?.Count ?? 0;
            decimal seatsCost = seatCount > 0 ? BaseCost * seatCount : BaseCost;
            TotalCost = seatsCost + LuggageCost;
        }

        public async Task LoadFlightDetailsAsync(int flightId)
        {
            try
            {
                IsBusy = true;

                // Obtener vuelo
                SelectedFlight = await _databaseService.GetFlightByIdAsync(flightId);
                if (SelectedFlight == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Vuelo no encontrado", "OK");
                    return;
                }

                // Obtener ruta
                _route = await _databaseService.GetRouteByIdAsync(SelectedFlight.RouteId);

                // Obtener aeropuertos
                OriginAirport = await _databaseService.GetAirportByIdAsync(_route.OriginAirportId);
                DestinationAirport = await _databaseService.GetAirportByIdAsync(_route.DestinationAirportId);

                // Establecer precio base
                BaseCost = _route.BasePrice;
                UpdateTotalCost();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar detalles: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ConfirmReservationAsync()
        {
            try
            {
                if (SelectedSeats.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Validación", "Selecciona al menos un asiento", "OK");
                    return;
                }

                if (SelectedFlight == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Vuelo no seleccionado", "OK");
                    return;
                }

                if (!MauiProgram.AuthenticationService.IsAuthenticated)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Usuario no autenticado", "OK");
                    return;
                }

                IsBusy = true;

                // Validar que los asientos aún están disponibles (en caso de que otro usuario los reserve simultáneamente)
                var reservedSeats = await _databaseService.GetReservedSeatsForFlightAsync(SelectedFlight.Id);
                foreach (var seatNumber in SelectedSeats)
                {
                    if (reservedSeats.Contains(seatNumber))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", 
                            $"El asiento {seatNumber} ya ha sido reservado por otro usuario. Por favor, selecciona otro asiento.", "OK");
                        IsBusy = false;
                        return;
                    }
                }

                var currentUser = MauiProgram.AuthenticationService.CurrentUser;
                decimal costPerSeat = BaseCost; // Cada asiento cuesta el precio base
                int luggagePerSeat = LuggageCount > 0 ? LuggageCount / SelectedSeats.Count : 0;

                // Crear una reservación por cada asiento seleccionado
                foreach (var seatNumber in SelectedSeats)
                {
                    try
                    {
                        var reservation = new Reservation
                        {
                            UserId = currentUser.Id,
                            FlightId = SelectedFlight.Id,
                            ReservationDate = DateTime.Now,
                            Status = 0, // Pending
                            TotalPrice = costPerSeat,
                            SeatNumber = seatNumber,
                            PreCheckIn = false,
                            LuggageCount = luggagePerSeat
                        };

                        var createdReservation = await _databaseService.CreateReservationAsync(reservation);

                        if (createdReservation == null || createdReservation.Id <= 0)
                        {
                            throw new Exception($"No se pudo crear la reservación para el asiento {seatNumber}");
                        }

                        // Si hay maletas, crearlas
                        if (luggagePerSeat > 0)
                        {
                            for (int i = 0; i < luggagePerSeat; i++)
                            {
                                var luggage = new Luggage
                                {
                                    ReservationId = createdReservation.Id,
                                    LuggageNumber = $"LUG-{createdReservation.Id}-{i + 1}",
                                    Weight = 0,
                                    Color = "Negro",
                                    Fee = GetLuggageFeeForNumber(i + 1)
                                };
                                await _databaseService.CreateLuggageAsync(luggage);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", 
                            $"Error al reservar asiento {seatNumber}: {ex.Message}", "OK");
                        IsBusy = false;
                        return;
                    }
                }

                await Application.Current.MainPage.DisplayAlert("Éxito", 
                    $"¡Reservación confirmada!\n\nTotal: ${TotalCost:F2}\nAsientos: {string.Join(", ", SelectedSeats)}", "OK");

                // Navegar de vuelta a reservaciones (/// para navegar a la raíz del TabBar)
                await Shell.Current.GoToAsync("///reservations");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Error al procesar la reservación: {ex.Message}\n\n{ex.InnerException?.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private decimal GetLuggageFeeForNumber(int luggageNumber)
        {
            if (luggageNumber == 1) return 0;
            if (luggageNumber == 2) return 50;
            return 75;
        }
    }
}
