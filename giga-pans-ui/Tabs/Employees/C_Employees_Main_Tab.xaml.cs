using api_access;
using api_access.models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace giga_pans_ui.Tabs.Employees
{
    /// <summary>
    /// Logika interakcji dla klasy C_Employees_Main_Tab.xaml
    /// </summary>
    ///

    public partial class C_Employees_Main_Tab : UserControl
    {
        private ContentControl contentControl;
        private (List<Author>, bool) allListElements;


        private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; 
            var col1 = 0.10;

            var col2 = 0.15;
            
            var col3 = 0.15;
            
            var col4 = 0.15;
            
            var col5 = 0.20;
            
            var col6 = 0.25;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
            gView.Columns[4].Width = workingWidth * col5;
            gView.Columns[5].Width = workingWidth * col6;
        }

        public C_Employees_Main_Tab(ContentControl contentControl)
        {
            InitializeComponent();
            this.contentControl = contentControl;
            LoadingScreen(true);
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
                allListElements = await AccessAPI.GetAuthors();
                bool isError = false;

                if (!allListElements.Item2)
                {
                    authorListView.ItemsSource = allListElements.Item1;
                }
                else
                {
                    MessageBox.Show("Failed to load data from the API server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                LoadingScreen(false);

                //GridView gridView = authorListView.View as GridView;

                //double columnWidth = authorListView.ActualWidth / 7;

                //foreach (GridViewColumn column in gridView.Columns)
                //{
                //    column.Width = columnWidth;
                //}
            }
            catch
            {
            }
        }

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Employees_Add_Tab(contentControl);
        }



        private void authorListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contentControl.Content = new C_Employees_Add_Tab(contentControl, (Author)authorListView.SelectedItem);
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
                authorListView.ItemsSource = allListElements.Item1;
            }
            else
            {
                List<Author> filteredAuthor = allListElements.Item1
                    .Where(author =>
                    author.first_Name.ToLower().Contains(searchText) ||
                    author.last_Name.ToLower().Contains(searchText) ||
                    author.phone_number.ToString().Contains(searchText) ||
                    author.email.ToString().Contains(searchText) ||
                    author.Department.name.ToLower().Contains(searchText)
                    )
                    .ToList();

                authorListView.ItemsSource = filteredAuthor;
            }
        }

        private void authorListView_Scroll(object sender, MouseWheelEventArgs e)
        {
            double newVerticalOffset = scrollView.VerticalOffset - e.Delta;

            if(newVerticalOffset < 0)
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