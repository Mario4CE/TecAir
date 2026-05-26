namespace TecAir.Views;

public partial class PromotionsPage : ContentPage
{
	public PromotionsPage()
	{
		InitializeComponent();
		LoadPromotions();
	}

	private async void LoadPromotions()
	{
		// TODO: Cargar promociones activas desde ViewModel
	}

	private async void OnBookPromoClicked(object sender, EventArgs e)
	{
		await DisplayAlert("Promoción", "Reservando con precio especial...", "OK");
		// TODO: Navegar a página de reserva con precio promocional
	}
}
