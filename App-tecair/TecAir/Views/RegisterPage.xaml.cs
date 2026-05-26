using TecAir.Models;

namespace TecAir.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();

        Shell.SetBackButtonBehavior(this, new BackButtonBehavior
        {
            IsEnabled = true,
            IsVisible = true
        });
    }

    private void OnIsStudentCheckBoxChanged(object sender, CheckedChangedEventArgs e)
    {
        // Mostrar/ocultar campos de estudiante según el checkbox
        StudentDetailsFrame.IsVisible = e.Value;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {
            // Validar campos requeridos
            if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
            {
                await DisplayAlert("Validación", "El nombre completo es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailEntry.Text))
            {
                await DisplayAlert("Validación", "El email es requerido", "OK");
                return;
            }

            if (!EmailEntry.Text.Contains("@"))
            {
                await DisplayAlert("Validación", "Ingresa un email válido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneEntry.Text))
            {
                await DisplayAlert("Validación", "El teléfono es requerido", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text) || PasswordEntry.Text.Length < 6)
            {
                await DisplayAlert("Validación", "La contraseña debe tener al menos 6 caracteres", "OK");
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Validación", "Las contraseñas no coinciden", "OK");
                return;
            }

            if (IsStudentCheckBox.IsChecked)
            {
                if (string.IsNullOrWhiteSpace(UniversityEntry.Text))
                {
                    await DisplayAlert("Validación", "La universidad es requerida", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(StudentIDEntry.Text))
                {
                    await DisplayAlert("Validación", "El carnet estudiantil es requerido", "OK");
                    return;
                }
            }

            // Crear nuevo usuario
            var newUser = new User
            {
                FullName = FullNameEntry.Text.Trim(),
                Email = EmailEntry.Text.Trim(),
                Password = PasswordEntry.Text,  // ← Guardar contraseña
                Phone = PhoneEntry.Text.Trim(),
                IsStudent = IsStudentCheckBox.IsChecked,
                University = IsStudentCheckBox.IsChecked ? UniversityEntry.Text.Trim() : null,
                StudentID = IsStudentCheckBox.IsChecked ? StudentIDEntry.Text.Trim() : null,
                LoyaltyMiles = 0,
                Role = 0, // Customer
                CreatedAt = DateTime.Now
            };

            // Intentar registrar
            var (success, message, user) = await MauiProgram.AuthenticationService.RegisterAsync(newUser);

            if (success)
            {
                await DisplayAlert("Éxito", "Usuario registrado exitosamente", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Error", message, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
        }
    }

    private async void OnLoginTapped(object sender, TappedEventArgs e)
    {
        // Navegar de vuelta a login
        await Shell.Current.GoToAsync("..");
    }
}
