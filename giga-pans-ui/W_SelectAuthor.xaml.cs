using api_access;
using api_access.models;
using System.Collections.Generic;
using System.Windows;

namespace giga_pans_ui
{
    /// <summary>
    /// Logika interakcji dla klasy W_SelectAuthor.xaml
    /// </summary>
    public partial class W_SelectAuthor : Window
    {
        public W_SelectAuthor()
        {
            InitializeComponent();
            LoadAuthorsAsync();
        }

        public void LoadingScreen(bool state)
        {
            if (state)
            {
                authorListView.Visibility = Visibility.Hidden;
                loading_control.Visibility = Visibility.Visible;
            }
            else
            {
                authorListView.Visibility = Visibility.Visible;
                loading_control.Visibility = Visibility.Hidden;
            }
        }

        private async void LoadAuthorsAsync()
        {
            try
            {
                (List<Author> authors, bool isError) = await AccessAPI.GetAuthors();

                if (!isError)
                {
                    authorListView.ItemsSource = authors;
                }
                else
                {
                    MessageBox.Show("Failed to load data from the API server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //LoadingScreen(false);

                //}
            }
            catch
            {
            }
        }
    }
}