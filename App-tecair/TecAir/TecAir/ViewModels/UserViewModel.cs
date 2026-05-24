using System.Collections.ObjectModel;
using TecAir.Models;
using TecAir.Services;

namespace TecAir.ViewModels
{
    /// <summary>
    /// ViewModel para gestión de usuarios
    /// </summary>
    public class UserViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private User _currentUser;
        private ObservableCollection<User> _users;

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public UserViewModel()
        {
            Title = "Gestión de Usuarios";
            _databaseService = MauiProgram.DatabaseService;
            Users = new ObservableCollection<User>();
        }

        public async Task InitializeAsync()
        {
            await LoadUsersAsync();
        }

        public async Task LoadUsersAsync()
        {
            try
            {
                IsBusy = true;
                var users = await _databaseService.GetAllUsersAsync();
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar usuarios: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                IsBusy = true;
                var createdUser = await _databaseService.CreateUserAsync(user);
                Users.Add(createdUser);
                CurrentUser = createdUser;
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al crear usuario: {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                IsBusy = true;
                await _databaseService.UpdateUserAsync(user);
                CurrentUser = user;
                await LoadUsersAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al actualizar usuario: {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                IsBusy = true;
                await _databaseService.DeleteUserAsync(userId);
                await LoadUsersAsync();
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al eliminar usuario: {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
