namespace TecAir.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsEnabled = false,
            IsVisible = false
        });

        UpdateUserInfo();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Verificar que el usuario esté autenticado
        if (!MauiProgram.AuthenticationService.IsAuthenticated)
        {
            // Si no está autenticado, regresar a login
            Shell.Current?.GoToAsync("login", animate: false);
            return;
        }

        UpdateUserInfo();
    }

    private void UpdateUserInfo()
    {
        var currentUser = MauiProgram.AuthenticationService.CurrentUser;
        if (currentUser != null)
        {
            UserInfoLabel.Text = $"Usuario: {currentUser.FullName}";
        }
        else
        {
            UserInfoLabel.Text = "Usuario: No autenticado";
        }
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
        await DisplayAlert(
            "Información",
            "La página de cuenta aún no existe.",
            "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert(
            "Confirmar",
            "¿Deseas cerrar sesión?",
            "Sí",
            "No");

        if (confirmed)
        {
            // Limpiar la sesión
            MauiProgram.AuthenticationService.Logout();

            // Navegar a login directamente sin AppShell
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}