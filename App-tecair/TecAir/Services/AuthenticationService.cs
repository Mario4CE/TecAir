using TecAir.Models;

namespace TecAir.Services
{
    /// <summary>
    /// Servicio para gestionar autenticación y sesión del usuario
    /// </summary>
    public class AuthenticationService
    {
        private readonly DatabaseService _databaseService;
        private User _currentUser;

        public User CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public bool IsAuthenticated => _currentUser != null;

        public AuthenticationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Intenta autenticar un usuario con solo email.
        /// La contraseña se almacena localmente en SQLite y no se valida durante login.
        /// </summary>
        public async Task<(bool success, string message, User user)> LoginAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return (false, "El email es requerido", null);
                }

                // Buscar usuario por email
                var user = await _databaseService.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return (false, "Usuario no encontrado", null);
                }

                _currentUser = user;
                return (true, "Autenticación exitosa", user);
            }
            catch (Exception ex)
            {
                return (false, $"Error durante autenticación: {ex.Message}", null);
            }
        }

        /// <summary>
        /// Cierra la sesión actual
        /// </summary>
        public void Logout()
        {
            _currentUser = null;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        public async Task<(bool success, string message, User user)> RegisterAsync(User newUser)
        {
            try
            {
                // Verificar si el email ya existe
                var existingUser = await _databaseService.GetUserByEmailAsync(newUser.Email);
                if (existingUser != null)
                {
                    return (false, "El email ya está registrado", null);
                }

                // Crear usuario
                newUser.CreatedAt = DateTime.Now;
                var createdUser = await _databaseService.CreateUserAsync(newUser);
                _currentUser = createdUser;

                return (true, "Usuario registrado exitosamente", createdUser);
            }
            catch (Exception ex)
            {
                return (false, $"Error durante registro: {ex.Message}", null);
            }
        }
    }
}
