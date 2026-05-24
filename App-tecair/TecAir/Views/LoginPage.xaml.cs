namespace TecAir.Views;

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
		var password = PasswordEntry.Text;

		if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
		{
			await DisplayAlert("Error", "Por favor completa todos los campos", "OK");
			return;
		}

		// Validar credenciales con el servicio de autenticación
		var (success, message, user) = await MauiProgram.AuthenticationService.LoginAsync(email, password);

		if (success)
		{
            await Shell.Current.GoToAsync(nameof(HomePage));
        }
		else
		{
			await DisplayAlert("Error de autenticación", message, "OK");
		}
	}
}
