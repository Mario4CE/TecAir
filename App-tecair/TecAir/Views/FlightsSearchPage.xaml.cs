namespace TecAir.Views;

public partial class FlightsSearchPage : ContentPage
{
	public FlightsSearchPage()
	{
		InitializeComponent();
		LoadAirports();
	}

	private void LoadAirports()
	{
		// TODO: Cargar aeropuertos desde ViewModel
		// Por ahora mostramos ejemplos
		OriginPicker.Items.Add("San José (SJO)");
		OriginPicker.Items.Add("Los Angeles (LAX)");
		OriginPicker.Items.Add("Miami (MIA)");

		DestinationPicker.Items.Add("Los Angeles (LAX)");
		DestinationPicker.Items.Add("Miami (MIA)");
		DestinationPicker.Items.Add("Madrid (MAD)");
		DestinationPicker.Items.Add("Caracas (CCS)");
	}

	private async void OnSearchClicked(object sender, EventArgs e)
	{
		if (OriginPicker.SelectedIndex < 0 || DestinationPicker.SelectedIndex < 0)
		{
			await DisplayAlert("Error", "Selecciona origen y destino", "OK");
			return;
		}

		// TODO: Ejecutar búsqueda de vuelos
		await DisplayAlert("Búsqueda", "Buscando vuelos...", "OK");
	}

	private async void OnReserveClicked(object sender, EventArgs e)
	{
		await DisplayAlert("Información", "Proceso de reserva iniciado", "OK");
		// TODO: Navegar a página de reserva
	}
}
