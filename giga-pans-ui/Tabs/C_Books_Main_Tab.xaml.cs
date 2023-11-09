using System;
using System.Collections.Generic;
using api_access;
using api_access.models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;

namespace giga_pans_ui.Tabs
{
    /// <summary>
    /// Logika interakcji dla klasy C_Books_Main_Tab.xaml
    /// </summary>
    public partial class C_Books_Main_Tab : UserControl
    {
        private ContentControl contentControl;
        private ObservableCollection<Book> allListElements;

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

        public C_Books_Main_Tab(ContentControl contentControl)
        {
            InitializeComponent();
            this.contentControl = contentControl;
            LoadingScreen(true);
            LoadBooksAsync();
        }

        public void LoadingScreen(bool state)
        {
            if (state)
            {
                bookListView.Visibility = Visibility.Hidden;
                loading_control.Visibility = Visibility.Visible;
            }
            else
            {
                bookListView.Visibility = Visibility.Visible;
                loading_control.Visibility = Visibility.Hidden;
            }
        }

        private async void LoadBooksAsync()
        {
            try
            {
                allListElements = new ObservableCollection<Book>(await AccessAPI.GetBooks());

                bookListView.ItemsSource = allListElements;
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
                    bookListView.ItemsSource = allListElements;
                }
                else
                {
                    List<Book> filteredBook = allListElements
                        .Where(book =>
                        book.id.ToString().Contains(searchText) ||
                        book.title.ToLower().Contains(searchText) ||
                        book.year.ToString().Contains(searchText) ||
                        book.volume.ToString().Contains(searchText) ||
                        book.number.ToString().Contains(searchText) ||
                        book.series.ToLower().Contains(searchText)
                        )
                        .ToList();

                    bookListView.ItemsSource = filteredBook;
                }
        }



        private void AddNewButtonClick(object sender, RoutedEventArgs e)
        {
            W_BookManagement w_BookManagement = new W_BookManagement();
            w_BookManagement.Owner = Application.Current.MainWindow;

            w_BookManagement.BookAdded += (s, args) =>
            {

                allListElements.Add(args.Book);
            };

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_BookManagement);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Book book = (Book)bookListView.SelectedItem;

            if (book != null)
            {

                Author__Book ab = new Author__Book
                {
                    Book = book,
                    book_id = book.id,
                };

                W_BookManagement w_BookManagement = new W_BookManagement(null, ab);
                w_BookManagement.Owner = Application.Current.MainWindow;

                w_BookManagement.BookDeleted += (s, args) =>
                {
                    allListElements.Remove(args.Book);
                };

                WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

                windowLocker.ShowModal(w_BookManagement);

                bookListView.SelectedItem = null;
            }
        }

        private void bookListView_Scroll(object sender, MouseWheelEventArgs e)
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
