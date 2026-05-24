using System.Collections.ObjectModel;
using TecAir.Models;
using TecAir.Services;

namespace TecAir.ViewModels
{
    /// <summary>
    /// ViewModel para gestión de reservaciones
    /// </summary>
    public class ReservationViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Reservation> _reservations;
        private Reservation _selectedReservation;
        private User _currentUser;

        public ObservableCollection<Reservation> Reservations
        {
            get => _reservations;
            set => SetProperty(ref _reservations, value);
        }

        public Reservation SelectedReservation
        {
            get => _selectedReservation;
            set => SetProperty(ref _selectedReservation, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public ReservationViewModel()
        {
            Title = "Mis Reservaciones";
            _databaseService = MauiProgram.DatabaseService;
            Reservations = new ObservableCollection<Reservation>();
        }

        public async Task InitializeAsync(User user)
        {
            CurrentUser = user;
            await LoadUserReservationsAsync(user.Id);
        }

        public async Task LoadUserReservationsAsync(int userId)
        {
            try
            {
                IsBusy = true;
                var reservations = await _databaseService.GetReservationsByUserAsync(userId);
                Reservations.Clear();
                foreach (var reservation in reservations)
                {
                    Reservations.Add(reservation);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar reservaciones: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> CreateReservationAsync(Reservation reservation)
        {
            try
            {
                IsBusy = true;
                var createdReservation = await _databaseService.CreateReservationAsync(reservation);
                Reservations.Add(createdReservation);
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al crear reservación: {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            try
            {
                IsBusy = true;
                var reservation = await _databaseService.GetReservationByIdAsync(reservationId);
                if (reservation != null)
                {
                    reservation.Status = 4; // Cancelled
                    await _databaseService.UpdateReservationAsync(reservation);
                    await LoadUserReservationsAsync(CurrentUser.Id);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cancelar: {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> CheckInAsync(int reservationId)
        {
            try
            {
                IsBusy = true;
                var reservation = await _databaseService.GetReservationByIdAsync(reservationId);
                if (reservation != null)
                {
                    reservation.Status = 2; // CheckedIn
                    await _databaseService.UpdateReservationAsync(reservation);
                    await LoadUserReservationsAsync(CurrentUser.Id);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error en check-in: {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
