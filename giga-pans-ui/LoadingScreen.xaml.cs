using api_access;
using System.Threading.Tasks;
using System.Windows;

namespace giga_pans_ui
{
    /// <summary>
    /// Logika interakcji dla klasy LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : Window
    {
        public LoadingScreen()
        {
            InitializeComponent();
            CheckBackendConnection();
        }

        private async void CheckBackendConnection()
        {
            //await Task.Delay(3000); // Opóźnienie
            await Task.Delay(000); // Opóźnienie

            bool isConnected = await AccessAPI.CheckApiConnection();

            if (isConnected)
            {
                MainWindow mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Błąd połączenia z serwerem. Sprawdź ustawienia i spróbuj ponownie.");
                Application.Current.Shutdown();
            }
        }
    }
}