using api_access.models;
using api_access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using giga_pans_ui.Controls;
using System.Collections.ObjectModel;

namespace giga_pans_ui.Tabs
{
    /// <summary>
    /// Logika interakcji dla klasy C_Conferences_Main_Tab.xaml
    /// </summary>
    public partial class C_Conferences_Main_Tab : UserControl
    {
        private ContentControl contentControl;
        private ObservableCollection<Conference> allListElements;


        private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            var col1 = 0.05;

            var col2 = 0.60;

            var col3 = 0.25;



            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
        }

        public C_Conferences_Main_Tab(ContentControl contentControl)
        {
            InitializeComponent();
            this.contentControl = contentControl;
            LoadingScreen(true);
            LoadConferencesAsync();
        }

        public void LoadingScreen(bool state)
        {
            if (state)
            {
                conferencesListView.Visibility = Visibility.Hidden;
                loading_control.Visibility = Visibility.Visible;
            }
            else
            {
                conferencesListView.Visibility = Visibility.Visible;
                loading_control.Visibility = Visibility.Hidden;
            }
        }

        private async void LoadConferencesAsync()
        {
            try
            {
                allListElements = new ObservableCollection<Conference>(await AccessAPI.GetConferences());

                conferencesListView.ItemsSource = allListElements;
                LoadingScreen(false);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data from the API server: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadingScreen(false);
            }
        }

        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = searchBar.SearchBarText;
            if (textBox.Text == "Search...")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = FindResource("SearchBarItemsColor") as SolidColorBrush;
            }
        }

        private void SearchBarText_KeyUp(object sender, KeyEventArgs e)
        {
                string searchText = searchBar.SearchBarText.Text.ToLower();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    conferencesListView.ItemsSource = allListElements;
                }
                else
                {
                    List<Conference> filteredConferences = allListElements
                        .Where(conference =>
                        conference.id.ToString().Contains(searchText) ||
                        conference.title.ToLower().Contains(searchText) ||
                        conference.date.ToString().Contains(searchText)
                        )
                        .ToList();

                    conferencesListView.ItemsSource = filteredConferences;
                }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            W_ConferenceManagement w_ConferenceManagement = new W_ConferenceManagement();
            w_ConferenceManagement.Owner = Application.Current.MainWindow;

            w_ConferenceManagement.ConferenceAdded += (s, args) =>
            {

                allListElements.Add(args.Conference);
            };

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_ConferenceManagement);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Conference conference = (Conference)conferencesListView.SelectedItem;

            if (conference != null)
            {

                Author__Conference ac = new Author__Conference
                {
                    Conference = conference,
                    conference_id = conference.id,
                };

                W_ConferenceManagement w_ConferenceManagement = new W_ConferenceManagement(null, ac);
                w_ConferenceManagement.Owner = Application.Current.MainWindow;

                w_ConferenceManagement.ConferenceDeleted += (s, args) =>
                {
                    allListElements.Remove(args.Conference);
                };

                WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

                windowLocker.ShowModal(w_ConferenceManagement);

                conferencesListView.SelectedItem = null;
            }
        }

        private void conferencesListView_Scroll(object sender, MouseWheelEventArgs e)
        {
            double newVerticalOffset = scrollView.VerticalOffset - e.Delta;

            if (newVerticalOffset < 0)
            {
                newVerticalOffset = 0;
            }
            else if (newVerticalOffset > scrollView.ScrollableHeight)
            {
                newVerticalOffset = scrollView.ScrollableHeight;
            }

            scrollView.ScrollToVerticalOffset(newVerticalOffset);
            e.Handled = true;
        }
    }
}
