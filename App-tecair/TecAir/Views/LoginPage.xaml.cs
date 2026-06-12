namespace TecAir.Views;

using System.Net.Http;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

		Shell.SetBackButtonBehavior(this, new BackButtonBehavior
		{
			IsEnabled = false,
			IsVisible = false
		});


	}

	private async void OnLoginClicked(object sender, EventArgs e)
	{
		var email = EmailEntry.Text;

		if (string.IsNullOrWhiteSpace(email))
		{
			await DisplayAlert("Error", "Por favor ingresa tu email", "OK");
			return;
		}

		// Validar credenciales con el servicio de autenticación (solo email)
		var (success, message, user) = await MauiProgram.AuthenticationService.LoginAsync(email);

		if (success)
		{
			await Shell.Current.GoToAsync(nameof(HomePage));
		}
		else
		{
			await DisplayAlert("Error de autenticación", message, "OK");
		}
	}

	private async void OnRegisterTapped(object sender, TappedEventArgs e)
	{
		// Navegar a la página de registro
		await Shell.Current.GoToAsync(nameof(RegisterPage));
	}

    private async void OnTestSQLiteClicked(object sender, EventArgs e)
    {
        try
        {
            var usuarios = await MauiProgram.DatabaseService.GetAllUsersAsync();

            string mensaje = $"Cantidad: {usuarios.Count}\n\n";

            foreach (var u in usuarios)
            {
                mensaje += $"{u.Email}\n";
            }

            await DisplayAlert(
                "SQLite",
                mensaje,
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Error",
                ex.ToString(),
                "OK");
        }
    }
}
