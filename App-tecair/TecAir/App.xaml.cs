using Microsoft.Extensions.DependencyInjection;
using TecAir.Services;

namespace TecAir
{
    public partial class App : Application
    {
        private AutoSyncService _autoSyncService;

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            MainPage = new AppShell();
            return new Window(MainPage);
        }

        protected override void OnStart()
        {
            base.OnStart();
            System.Diagnostics.Debug.WriteLine("[App] Aplicación iniciada");
        }

        protected override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine("[App] Aplicación reanudada - Reanudando sincronización automática");

            // Obtener AutoSyncService del contenedor de inyección de dependencias
            _autoSyncService = IPlatformApplication.Current?.Services?.GetService<AutoSyncService>();
            if (_autoSyncService != null)
            {
                _autoSyncService.Resume();
            }
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            System.Diagnostics.Debug.WriteLine("[App] Aplicación pausada - Deteniendo sincronización automática");

            // Obtener AutoSyncService del contenedor de inyección de dependencias
            _autoSyncService = IPlatformApplication.Current?.Services?.GetService<AutoSyncService>();
            if (_autoSyncService != null)
            {
                _autoSyncService.Stop();
            }
        }
    }
}
