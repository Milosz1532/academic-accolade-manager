using api_access;
using api_access.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace giga_pans_ui
{
    /// <summary>
    /// Logika interakcji dla klasy W_Select.xaml
    /// </summary>
    public partial class W_Select : Window
    {

        public class SelectEventArgs<T> : EventArgs
        {
            public T SelectedItem { get; }

            public SelectEventArgs(T selectedItem)
            {
                SelectedItem = selectedItem;
            }
        }

        public event EventHandler<SelectEventArgs<object>> ItemSelected;


        public enum ItemType
        {
            Authors,
            Books,
            Articles,
            Conferences,
            Research,
        }

        public void LoadingScreen(bool state)
        {
            if (state)
            {
                selectListView.Visibility = Visibility.Hidden;
                loading_control.Visibility = Visibility.Visible;
            }
            else
            {
                selectListView.Visibility = Visibility.Visible;
                loading_control.Visibility = Visibility.Hidden;
            }
        }

        public W_Select(ItemType itemType, List<int> withoutElements = null)
        {
            InitializeComponent();
            LoadingScreen(true);
            switch (itemType)
            {
                case ItemType.Authors:
                    WindowName.Text = "Select author";

                    Column2.DisplayMemberBinding = new Binding("first_Name");
                    Column3.DisplayMemberBinding = new Binding("last_Name");

                    Loaded += async (sender, e) =>
                    {
                        try
                        {
                            (List<Author> authors, bool isError) = await AccessAPI.GetAuthors();
                            LoadingScreen(false);

                            if (!isError)
                            {
                                var filteredAuthors = withoutElements != null
                                    ? authors.Where(el => !withoutElements.Contains(el.id)).ToList()
                                    : authors;

                                selectListView.ItemsSource = filteredAuthors;
                            }
                            else
                            {
                                MessageBox.Show("Failed to load data from the API server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };
                    break;
                case ItemType.Books:
                    WindowName.Text = "Select book";

                    Column2.DisplayMemberBinding = new Binding("title");
                    Column3.DisplayMemberBinding = new Binding("year");
                    Column2.Header = "Title";
                    Column3.Header = "Year";

                    Loaded += async (sender, e) =>
                    {
                        try
                        {
                            List<Book> list = await AccessAPI.GetBooks();
                            LoadingScreen(false);


                            var filtered = withoutElements != null
                                    ? list.Where(el => !withoutElements.Contains(el.id)).ToList()
                                    : list;

                                selectListView.ItemsSource = filtered;
                            
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };
                    break;
                case ItemType.Articles:
                    WindowName.Text = "Select article";

                    Column2.DisplayMemberBinding = new Binding("title");
                    Column3.DisplayMemberBinding = new Binding("year");
                    Column2.Header = "Title";
                    Column3.Header = "Year";

                    Loaded += async (sender, e) =>
                    {
                        try
                        {
                            List<Article> list = await AccessAPI.GetArticles();
                            LoadingScreen(false);


                            var filtered = withoutElements != null
                                ? list.Where(el => !withoutElements.Contains(el.id)).ToList()
                                : list;

                            selectListView.ItemsSource = filtered;

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };
                    break;
                case ItemType.Conferences:
                    WindowName.Text = "Select conference";

                    Column2.DisplayMemberBinding = new Binding("title");
                    Column3.DisplayMemberBinding = new Binding("date");
                    Column2.Header = "Title";
                    Column3.Header = "Date";

                    Loaded += async (sender, e) =>
                    {
                        try
                        {
                            List<Conference> list = await AccessAPI.GetConferences();
                            LoadingScreen(false);


                            var filtered = withoutElements != null
                                ? list.Where(el => !withoutElements.Contains(el.id)).ToList()
                                : list;

                            selectListView.ItemsSource = filtered;

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };
                    break;
                case ItemType.Research:
                    WindowName.Text = "Select research";

                    Column2.DisplayMemberBinding = new Binding("title");
                    Column3.DisplayMemberBinding = new Binding("date_start");
                    Column2.Header = "Title";
                    Column3.Header = "Start date";

                    Loaded += async (sender, e) =>
                    {
                        try
                        {
                            List<Research> list = await AccessAPI.GetResearches();
                            LoadingScreen(false);


                            var filtered = withoutElements != null
                                ? list.Where(el => !withoutElements.Contains(el.id)).ToList()
                                : list;

                            selectListView.ItemsSource = filtered;

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    };
                    break;
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void selectListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectListView.SelectedItem != null)
            {
                var selectedItem = selectListView.SelectedItem;
                var eventArgs = new SelectEventArgs<object>(selectedItem);

                ItemSelected?.Invoke(this, eventArgs);
                Close();

            }
        }



    }
}
