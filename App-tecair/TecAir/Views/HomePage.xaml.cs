using TecAir.Services;

namespace TecAir.Views;

public partial class HomePage : ContentPage
{
    private readonly SyncService _syncService;

    // Se inyecta el SyncService por constructor
    public HomePage(SyncService syncService)
    {
        _syncService = syncService;
        InitializeComponent();

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsEnabled = false,
            IsVisible = false
        });

        UpdateUserInfo();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Verificar que el usuario esté autenticado
        if (!MauiProgram.AuthenticationService.IsAuthenticated)
        {
            Shell.Current?.GoToAsync("login", animate: false);
            return;
        }

        UpdateUserInfo();

        // Sincroniza con el API si hay conexión
        // Se hace en segundo plano para no bloquear la pantalla
        if (_syncService.HayConexion())
        {
            var (exito, mensaje) = await _syncService.SincronizarAsync();
            if (!exito)
                await DisplayAlert("Sincronización", mensaje, "OK");
        }
    }

    private void UpdateUserInfo()
    {
        var currentUser = MauiProgram.AuthenticationService.CurrentUser;
        if (currentUser != null)
            UserInfoLabel.Text = $"Usuario: {currentUser.FullName}";
        else
            UserInfoLabel.Text = "Usuario: No autenticado";
    }

    private async void OnSearchFlightsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(FlightsSearchPage));
    }

    private async void OnMyReservationsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ReservationsPage));
    }

    private async void OnPromotionsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PromotionsPage));
    }

    private async void OnMyAccountClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Información", "La página de cuenta aún no existe.", "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert("Confirmar", "¿Deseas cerrar sesión?", "Sí", "No");

        if (confirmed)
        {
            MauiProgram.AuthenticationService.Logout();
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}