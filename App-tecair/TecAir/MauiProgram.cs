using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using TecAir.Services;
using TecAir.ViewModels;

namespace TecAir
{
    public static class MauiProgram
    {
        public static DatabaseService DatabaseService { get; private set; }
        public static AuthenticationService AuthenticationService { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar servicios
            builder.Services.AddSingleton<SyncService>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<AutoSyncService>();
            
            // Registrar ViewModels
            builder.Services.AddSingleton<UserViewModel>();
            builder.Services.AddSingleton<FlightViewModel>();
            builder.Services.AddSingleton<ReservationViewModel>();
            builder.Services.AddSingleton<ReservationDetailViewModel>();
            builder.Services.AddSingleton<PromotionViewModel>();

            // Registrar Pages
            // Las páginas que usan SyncService necesitan recibirlo por constructor
            builder.Services.AddSingleton<Views.HomePage>(sp =>
                new Views.HomePage(sp.GetRequiredService<SyncService>()));

            builder.Services.AddSingleton<Views.FlightsSearchPage>(sp =>
                new Views.FlightsSearchPage(sp.GetRequiredService<SyncService>()));

            builder.Services.AddSingleton<Views.ReservationsPage>(sp =>
                new Views.ReservationsPage(sp.GetRequiredService<SyncService>()));

            builder.Services.AddSingleton<Views.PromotionsPage>(sp =>
                new Views.PromotionsPage(sp.GetRequiredService<SyncService>()));

            // Estas no usan SyncService, se registran normal
            builder.Services.AddSingleton<Views.LoginPage>();
            builder.Services.AddSingleton<Views.RegisterPage>();
            builder.Services.AddSingleton<Views.ReservationDetailPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Inicializar base de datos y servicios
            DatabaseService = app.Services.GetRequiredService<DatabaseService>();
            AuthenticationService = app.Services.GetRequiredService<AuthenticationService>();
            var autoSyncService = app.Services.GetRequiredService<AutoSyncService>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DatabaseService.InitializeAsync();

                // Iniciar sincronización automática en segundo plano
                autoSyncService.Start();

                System.Diagnostics.Debug.WriteLine(
                    "[MauiProgram] Inicialización completada - Sincronización automática iniciada");
            });

            return app;
        }
    }
}
