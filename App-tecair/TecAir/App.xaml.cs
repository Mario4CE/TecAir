using Microsoft.Extensions.DependencyInjection;

namespace TecAir
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            MainPage = new AppShell();
            return new Window(MainPage);
        }
    }
}