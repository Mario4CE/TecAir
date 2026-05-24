using Microsoft.Extensions.Logging;
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
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<AuthenticationService>();
            
            // Registrar ViewModels
            builder.Services.AddSingleton<UserViewModel>();
            builder.Services.AddSingleton<FlightViewModel>();
            builder.Services.AddSingleton<ReservationViewModel>();
            builder.Services.AddSingleton<PromotionViewModel>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            
            // Inicializar base de datos
            DatabaseService = app.Services.GetRequiredService<DatabaseService>();
            AuthenticationService = app.Services.GetRequiredService<AuthenticationService>();
            
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DatabaseService.InitializeAsync();
            });

            return app;
        }
    }
}
