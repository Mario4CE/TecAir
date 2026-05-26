using TecAir.Services;
using TecAir.ViewModels;

namespace TecAir.Views;

public partial class FlightsSearchPage : ContentPage
{
    private FlightViewModel _viewModel;
    private readonly SyncService _syncService;

    public FlightsSearchPage(SyncService syncService)
    {
        _syncService = syncService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Sincroniza vuelos y aeropuertos antes de mostrar la búsqueda
        if (_syncService.HayConexion())
            await _syncService.SincronizarAsync();

        // Inicializar ViewModel si no está ya establecido
        if (BindingContext == null)
        {
            _viewModel = new FlightViewModel();
            BindingContext = _viewModel;
            await _viewModel.InitializeAsync();
        }

        await LoadAirportsInPickers();
    }

    private async Task LoadAirportsInPickers()
    {
        try
        {
            _viewModel ??= (FlightViewModel)BindingContext;

            OriginPicker.Items.Clear();
            DestinationPicker.Items.Clear();

            foreach (var airport in _viewModel.Airports)
            {
                string displayText = $"{airport.Name} ({airport.Code})";
                OriginPicker.Items.Add(displayText);
                DestinationPicker.Items.Add(displayText);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar aeropuertos: {ex.Message}", "OK");
        }
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        try
        {
            if (OriginPicker.SelectedIndex < 0 || DestinationPicker.SelectedIndex < 0)
            {
                await DisplayAlert("Error", "Selecciona origen y destino", "OK");
                return;
            }

            _viewModel ??= (FlightViewModel)BindingContext;
            _viewModel.SelectedOrigin = _viewModel.Airports[OriginPicker.SelectedIndex];
            _viewModel.SelectedDestination = _viewModel.Airports[DestinationPicker.SelectedIndex];

            await _viewModel.SearchFlightsAsync();

            AvailableFlightsLabel.IsVisible = _viewModel.Flights.Count > 0;

            if (_viewModel.Flights.Count == 0)
                await DisplayAlert("Sin resultados", "No hay vuelos disponibles para esta ruta", "OK");
            else
                await DisplayAlert("Éxito", $"Se encontraron {_viewModel.Flights.Count} vuelo(s)", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error en búsqueda: {ex.Message}", "OK");
        }
    }

    private async void OnReserveClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is Button button && button.BindingContext is Models.FlightWithRoute flightWithRoute)
            {
                if (!MauiProgram.AuthenticationService.IsAuthenticated)
                {
                    await DisplayAlert("Error", "Debes iniciar sesión para reservar", "OK");
                    return;
                }

                var reservationPage = new Views.ReservationDetailPage();
                reservationPage.SetFlightId(flightWithRoute.Flight.Id);
                await Navigation.PushAsync(reservationPage);
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener la información del vuelo", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al reservar: {ex.Message}", "OK");
        }
    }
}