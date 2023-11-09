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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static giga_pans_ui.W_Select;

namespace giga_pans_ui.Tabs.Employees.Tabs
{
    /// <summary>
    /// Logika interakcji dla klasy C_Employee_Articles.xaml
    /// </summary>
    public partial class C_Employee_Articles : UserControl
    {


        private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            var col1 = 0.40;

            var col2 = 0.20;

            var col3 = 0.20;

            var col4 = 0.20;




            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
        }

        private C_Employees_Add_Tab parentWindow;
        public ObservableCollection<Author__Article> authorArticles = new ObservableCollection<Author__Article>();
        Author author = null;

        public C_Employee_Articles(C_Employees_Add_Tab parentWindow, ObservableCollection<Author__Article> authorArticles, Author author)
        {
            InitializeComponent();
            this.authorArticles = authorArticles;
            this.author = author;
            this.parentWindow = parentWindow;
            articlesListView.ItemsSource = authorArticles;

        }



        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            W_ArticleManagement w_ArticleManagement = new W_ArticleManagement(author);
            w_ArticleManagement.Owner = Application.Current.MainWindow;

            w_ArticleManagement.ArticleAdded += (s, args) =>
            {

                authorArticles.Add(args.Author__Article);
            };

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_ArticleManagement);
        }


        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            Author__Article selectedArticle = (Author__Article)articlesListView.SelectedItem;

            if (selectedArticle != null)
            {


                W_ArticleManagement w_ArticleManagement = new W_ArticleManagement(author, selectedArticle);
                w_ArticleManagement.Owner = Application.Current.MainWindow;

                w_ArticleManagement.ArticleDeleted += (s, args) =>
                {
                    authorArticles.Remove(args.Author__Article);
                };

                WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

                windowLocker.ShowModal(w_ArticleManagement);

            }
        }

        

        private void AddExistingButtonClick(object sender, RoutedEventArgs e)
        {

            List<int> withoutBooks = new List<int>();

            foreach (Author__Article article in authorArticles)
            {
                withoutBooks.Add(article.Article.id);
            }

            W_Select w_Select = new W_Select(ItemType.Articles, withoutBooks);

            w_Select.ItemSelected += async (s, args) =>
            {
                if (args.SelectedItem is Article selected)
                {
                    try
                    {
                        Author__Article new_ab = new Author__Article
                        {
                            author_id = author.id,
                            article_id = selected.id,
                            participation = 100
                        };
                        Author__Article createdAuthor = await AccessAPI.createAuthorArticle(new_ab);
                        new_ab.Article = selected;
                        authorArticles.Add(new_ab);
                        MessageBox.Show("Article added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to add article.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            };

            w_Select.Owner = Application.Current.MainWindow;

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_Select);
        }


        private void articleListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Edit.IsEnabled = (articlesListView.SelectedItem != null);
        }



    }
}
