using TecAir.Models;
using TecAir.ViewModels;

namespace TecAir.Views;

public partial class ReservationsPage : ContentPage
{
	private ReservationViewModel _viewModel;

	public ReservationsPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		// Inicializar ViewModel si no está ya establecido
		if (BindingContext == null)
		{
			_viewModel = new ReservationViewModel();
			BindingContext = _viewModel;

			// Obtener usuario actual desde AuthenticationService
			var currentUser = MauiProgram.AuthenticationService.CurrentUser;
			
			if (currentUser != null)
			{
				await _viewModel.InitializeAsync(currentUser);
			}
			else
			{
				// Si no hay usuario autenticado, mostrar mensaje
				await DisplayAlert("Error", "Usuario no autenticado", "OK");
				await Shell.Current.GoToAsync("..");
			}
		}
	}

	private async void OnCheckInClicked(object sender, EventArgs e)
	{
		try
		{
			var button = sender as Button;
			if (button?.CommandParameter is Reservation reservation)
			{
				var confirmed = await DisplayAlert(
					"Confirmar Check-in",
					$"¿Realizar check-in para el vuelo #{reservation.FlightId}?",
					"Sí",
					"No"
				);

				if (confirmed)
				{
					_viewModel ??= (ReservationViewModel)BindingContext;
					await _viewModel.CheckInAsync(reservation.Id);
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
			if (button?.CommandParameter is Reservation reservation)
			{
				var confirmed = await DisplayAlert(
					"Confirmar cancelación",
					$"¿Cancelar la reservación del vuelo #{reservation.FlightId}?",
					"Sí",
					"No"
				);

				if (confirmed)
				{
					_viewModel ??= (ReservationViewModel)BindingContext;
					await _viewModel.CancelReservationAsync(reservation.Id);
					await DisplayAlert("Cancelada", "Tu reservación ha sido cancelada", "OK");
				}
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Error al cancelar: {ex.Message}", "OK");
		}
	}
}
