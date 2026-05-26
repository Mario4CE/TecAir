using TecAir.Models;
using TecAir.Services;
using TecAir.ViewModels;

namespace TecAir.Views;

public partial class ReservationsPage : ContentPage
{
    private ReservationViewModel _viewModel;
    private readonly SyncService _syncService;

    public ReservationsPage(SyncService syncService)
    {
        _syncService = syncService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var currentUser = MauiProgram.AuthenticationService.CurrentUser;

        if (currentUser == null)
        {
            await DisplayAlert("Error", "Usuario no autenticado", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        // Sincroniza reservaciones antes de mostrarlas
        if (_syncService.HayConexion())
            await _syncService.SincronizarAsync();

        // Siempre reinicializar para obtener datos frescos
        _viewModel = new ReservationViewModel();
        BindingContext = _viewModel;
        await _viewModel.InitializeAsync(currentUser);
    }

    private async void OnCheckInClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            if (button?.CommandParameter is ReservationWithFlight reservationWithFlight)
            {
                var confirmed = await DisplayAlert(
                    "Confirmar Check-in",
                    $"¿Realizar check-in para el vuelo {reservationWithFlight.FlightNumber} ({reservationWithFlight.RouteDisplay})?",
                    "Sí", "No"
                );

                if (confirmed)
                {
                    _viewModel ??= (ReservationViewModel)BindingContext;
                    await _viewModel.CheckInAsync(reservationWithFlight.Id);
                    await DisplayAlert("Éxito", "Check-in realizado exitosamente", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error en check-in: {ex.Message}", "OK");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;
            if (button?.CommandParameter is ReservationWithFlight reservationWithFlight)
            {
                var confirmed = await DisplayAlert(
                    "Confirmar cancelación",
                    $"¿Cancelar la reservación para el vuelo {reservationWithFlight.FlightNumber} ({reservationWithFlight.RouteDisplay})? Se devolverán los asientos disponibles.",
                    "Sí", "No"
                );

                if (confirmed)
                {
                    _viewModel ??= (ReservationViewModel)BindingContext;
                    await _viewModel.CancelReservationAsync(reservationWithFlight.Id);
                    await DisplayAlert("Cancelada", "Tu reservación ha sido cancelada exitosamente", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cancelar: {ex.Message}", "OK");
        }
    }
}