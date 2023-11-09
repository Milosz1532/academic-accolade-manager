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
using System.Collections.ObjectModel;

namespace giga_pans_ui.Tabs
{
    /// <summary>
    /// Logika interakcji dla klasy C_Research_Main_Tab.xaml
    /// </summary>
    public partial class C_Research_Main_Tab : UserControl
    {
        private ContentControl contentControl;
        private ObservableCollection<Research> allListElements;

        private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            var col1 = 0.05;

            var col2 = 0.35;

            var col3 = 0.15;

            var col4 = 0.15;

            var col5 = 0.15;

            var col6 = 0.15;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
            gView.Columns[4].Width = workingWidth * col5;
            gView.Columns[5].Width = workingWidth * col6;
        }


        public C_Research_Main_Tab(ContentControl contentControl)
        {
            InitializeComponent();
            this.contentControl = contentControl;
            LoadingScreen(true);
            LoadResearchAsync();
        }

        public void LoadingScreen(bool state)
        {
            if (state)
            {
                researchListView.Visibility = Visibility.Hidden;
                loading_control.Visibility = Visibility.Visible;
            }
            else
            {
                researchListView.Visibility = Visibility.Visible;
                loading_control.Visibility = Visibility.Hidden;
            }
        }

        private async void LoadResearchAsync()
        {
            try
            {
                allListElements = new ObservableCollection<Research>(await AccessAPI.GetResearches());

                researchListView.ItemsSource = allListElements;
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
                    researchListView.ItemsSource = allListElements;
                }
                else
                {
                    List<Research> filteredResearch = allListElements
                        .Where(research =>
                        research.id.ToString().Contains(searchText) ||
                        research.title.ToLower().Contains(searchText) ||
                        research.date_start.ToString().Contains(searchText) ||
                        research.date_end.ToString().Contains(searchText) ||
                        research.function_in_a_team.ToLower().Contains(searchText) ||
                        research.source_of_funding.ToLower().Contains(searchText)
                        )
                        .ToList();

                    researchListView.ItemsSource = filteredResearch;
                }
        }

        private void AddNewButtonClick(object sender, RoutedEventArgs e)
        {
            W_ResearchManagement w_ResearchManagement = new W_ResearchManagement();
            w_ResearchManagement.Owner = Application.Current.MainWindow;

            w_ResearchManagement.ResearchAdded += (s, args) =>
            {

                allListElements.Add(args.Research);
            };

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_ResearchManagement);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Research research = (Research)researchListView.SelectedItem;

            if (research != null)
            {

                Author__Research ar = new Author__Research
                {
                    Research = research,
                    research_id = research.id,
                };

                W_ResearchManagement w_ResearchManagement = new W_ResearchManagement(null, ar);
                w_ResearchManagement.Owner = Application.Current.MainWindow;

                w_ResearchManagement.ResearchDeleted += (s, args) =>
                {
                    allListElements.Remove(args.Research);
                };

                WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

                windowLocker.ShowModal(w_ResearchManagement);

                researchListView.SelectedItem = null;
            }
        }

        private void researchListView_Scroll(object sender, MouseWheelEventArgs e)
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
