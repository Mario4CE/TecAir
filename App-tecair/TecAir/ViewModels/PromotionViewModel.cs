using System.Collections.ObjectModel;
using TecAir.Models;
using TecAir.Services;

namespace TecAir.ViewModels
{
    /// <summary>
    /// ViewModel para gestión de promociones
    /// </summary>
    public class PromotionViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Promotion> _promotions;

        public ObservableCollection<Promotion> Promotions
        {
            get => _promotions;
            set => SetProperty(ref _promotions, value);
        }

        public PromotionViewModel()
        {
            Title = "Promociones";
            _databaseService = MauiProgram.DatabaseService;
            Promotions = new ObservableCollection<Promotion>();
        }

        public async Task InitializeAsync()
        {
            await LoadPromotionsAsync();
        }

        public async Task LoadPromotionsAsync()
        {
            try
            {
                IsBusy = true;

                var promotions = await _databaseService.GetAllPromotionsAsync();

                System.Diagnostics.Debug.WriteLine(
                    $"Promociones totales encontradas: {promotions.Count}");

                Promotions.Clear();

                foreach (var promotion in promotions)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"ID={promotion.Id} | " +
                        $"ApiId={promotion.ApiId} | " +
                        $"Precio={promotion.PromotionalPrice} | " +
                        $"Inicio={promotion.StartDate} | " +
                        $"Fin={promotion.EndDate} | " +
                        $"Activa={promotion.IsActive}");

                    Promotions.Add(promotion);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
