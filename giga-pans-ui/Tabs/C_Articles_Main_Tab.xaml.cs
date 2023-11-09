using api_access.models;
using api_access;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;

namespace giga_pans_ui.Tabs
{
    /// <summary>
    /// Logika interakcji dla klasy C_Articles_Main_Tab.xaml
    /// </summary>
    public partial class C_Articles_Main_Tab : UserControl
    {
        private ContentControl contentControl;
        private ObservableCollection<Article> allListElements;

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

        public C_Articles_Main_Tab(ContentControl contentControl)
        {
            InitializeComponent();
            this.contentControl = contentControl;
            LoadingScreen(true);
            LoadArticlesAsync();
        }

        public void LoadingScreen(bool state)
        {
            if (state)
            {
                articleListView.Visibility = Visibility.Hidden;
                loading_control.Visibility = Visibility.Visible;
            }
            else
            {
                articleListView.Visibility = Visibility.Visible;
                loading_control.Visibility = Visibility.Hidden;
            }
        }

        private async void LoadArticlesAsync()
        {
            try
            {
                allListElements = new ObservableCollection<Article>(await AccessAPI.GetArticles());

                articleListView.ItemsSource = allListElements;
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
                    articleListView.ItemsSource = allListElements;
                }
                else
                {
                    List<Article> filteredArticle = allListElements
                        .Where(article =>
                        article.id.ToString().Contains(searchText) ||
                        article.title.ToLower().Contains(searchText) ||
                        article.name.ToLower().Contains(searchText) ||
                        article.year.ToString().Contains(searchText) ||
                        article.volume.ToString().Contains(searchText) ||
                        article.number.ToString().Contains(searchText)
                        )
                        .ToList();

                    articleListView.ItemsSource = filteredArticle;
                }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            W_ArticleManagement w_ArticleManagement = new W_ArticleManagement();
            w_ArticleManagement.Owner = Application.Current.MainWindow;

            w_ArticleManagement.ArticleAdded += (s, args) =>
            {

                allListElements.Add(args.Article);
            };

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_ArticleManagement);
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Article article = (Article)articleListView.SelectedItem;

            if (article != null)
            {

                Author__Article aa = new Author__Article
                {
                    Article = article,
                    article_id = article.id,
                };

                W_ArticleManagement w_ArticleManagement = new W_ArticleManagement(null, aa);
                w_ArticleManagement.Owner = Application.Current.MainWindow;

                w_ArticleManagement.ArticleDeleted += (s, args) =>
                {
                    allListElements.Remove(args.Article);
                };

                WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

                windowLocker.ShowModal(w_ArticleManagement);

                articleListView.SelectedItem = null;
            }
        }

        private void articleListView_Scroll(object sender, MouseWheelEventArgs e)
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
