using TecAir.Services;
using TecAir.ViewModels;

namespace TecAir.Views;

public partial class PromotionsPage : ContentPage
{
    private readonly SyncService _syncService;
    private PromotionViewModel _viewModel;

    public PromotionsPage(SyncService syncService)
    {
        _syncService = syncService;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Sincroniza promociones antes de mostrarlas
        if (_syncService.HayConexion())
            await _syncService.SincronizarAsync();

        // Inicializa el ViewModel con las promociones actualizadas
        _viewModel = new PromotionViewModel();
        BindingContext = _viewModel;
        await _viewModel.InitializeAsync();
    }

    private async void OnBookPromoClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Promoción", "Reservando con precio especial...", "OK");
        // TODO: Navegar a página de reserva con precio promocional
    }
}