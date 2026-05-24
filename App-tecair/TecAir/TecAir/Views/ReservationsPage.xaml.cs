namespace TecAir.Views;

public partial class ReservationsPage : ContentPage
{
	public ReservationsPage()
	{
		InitializeComponent();
		LoadReservations();
	}

	private async void LoadReservations()
	{
		// TODO: Cargar reservaciones del usuario actual desde ViewModel
	}

	private async void OnCheckInClicked(object sender, EventArgs e)
	{
		await DisplayAlert("Check-in", "Check-in realizado exitosamente", "OK");
		// TODO: Ejecutar check-in
	}

	private async void OnCancelClicked(object sender, EventArgs e)
	{
		var confirmed = await DisplayAlert("Confirmar", "¿Deseas cancelar esta reservación?", "Sí", "No");
		if (confirmed)
		{
			// TODO: Cancelar reservación
			await DisplayAlert("Cancelada", "Tu reservación ha sido cancelada", "OK");
		}
	}
}
