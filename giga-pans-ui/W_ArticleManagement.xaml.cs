using api_access;
using api_access.models;
using giga_pans_ui.Tabs;
using giga_pans_ui.Tabs.Employees;
using giga_pans_ui.Tabs.Employees.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static giga_pans_ui.W_Select;

namespace giga_pans_ui
{
    /// <summary>
    /// Logika interakcji dla klasy W_ArticleManagement.xaml
    /// </summary>
    /// 
    public class ArticleEventArgs : EventArgs
    {
        public Article Article { get; }

        public Author__Article Author__Article { get; }

        public ArticleEventArgs(Article article, Author__Article author__Article = null)
        {
            Article = article;
            Author__Article = author__Article;
        }
    }
    public partial class W_ArticleManagement : Window
    {

        public event EventHandler<ArticleEventArgs> ArticleAdded;
        public event EventHandler<ArticleEventArgs> ArticleDeleted;

        public Author__Article article = new Author__Article();
        public Author author = new Author();
        public ObservableCollection<Author__Article> AuthorList = new ObservableCollection<Author__Article>();

        public bool editMode = false;


        private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            var col1 = 0.05;

            var col2 = 0.40;

            var col3 = 0.40;

            var col4 = 0.15;



            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
        }


        public W_ArticleManagement(Author author = null, Author__Article article = null)
        {
            InitializeComponent();
            this.author = author;
            autorsListView.ItemsSource = AuthorList;

            if (article != null)
            {
                this.article = article;
                editMode = true;
                AddArticle.Content = "Edit article";
                deleteArticle.Visibility = Visibility.Visible;
                GetArticleAuthors();
            }
            else
            {
                this.article = new Author__Article();
                this.article.Article = new Article();

                if (author != null)
                {
                    Author__Article author_article = new Author__Article
                    {
                        Author = author,
                        author_id = author.id,
                        participation = 100,
                    };
                    AuthorList.Add(author_article);
                }

            }
            this.DataContext = this.article;
        }

        private async void GetArticleAuthors()
        {
            try
            {
                Article autors = await AccessAPI.GetArticleAuthors(article.Article.id);

                AuthorList.Clear();

                foreach (var author in autors.Author__Article)
                {
                    AuthorList.Add(author);
                }

            }
            catch
            {
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Delete_Author_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Author__Article selectedAuthor)
            {
                if (author != null && selectedAuthor.Author.id == author.id && !editMode) return;
                AuthorList.Remove(selectedAuthor);

            }
        }

        public async void AddArticleToDb(Article article, ObservableCollection<Author__Article> authorsArticle)
        {
            try
            {
                Article createArticle = await AccessAPI.CreateArticle(article);

                if (createArticle != null)
                {

                    //Add main autor

                    Author__Article new_aa = null;


                    article.id = createArticle.id;

                    if (author != null)
                    {
                        Author__Article author_article = new Author__Article
                        {
                            author_id = author.id,
                            article_id = createArticle.id,
                            participation = 100,
                        };
                        Author__Article createMainAuthor = await AccessAPI.createAuthorArticle(author_article);

                        new_aa = new Author__Article
                        {
                            Article = createArticle,
                            id = createMainAuthor.id,
                            author_id = author.id,
                            article_id = createArticle.id,
                            participation = 100
                        };
                    }
                    //Add other autors
                    foreach (var listAuthor in authorsArticle)
                    {
                        if (author != null && author.id == listAuthor.author_id) continue;

                        listAuthor.article_id = createArticle.id;
                        listAuthor.participation = 100;
                        listAuthor.Article = null;
                        listAuthor.Author = null;
                        Author__Article createdAuthorArticle = await AccessAPI.createAuthorArticle(listAuthor);
                    }

                    ArticleAdded?.Invoke(this, new ArticleEventArgs(article, new_aa));
                    MessageBox.Show("Article added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Failed to add article.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void EditArticleInDb(Author__Article article, ObservableCollection<Author__Article> authorsArticle)
        {
            try
            {
                Article editedArticle = await AccessAPI.UpdateArticle(article.Article);

                if (editedArticle != null)
                {
                    await AccessAPI.DeleteArticleAuthors(article.Article.id);


                    foreach (var listAuthor in authorsArticle)
                    {
                        listAuthor.article_id = article.Article.id;
                        listAuthor.participation = 100;
                        listAuthor.Article = null;
                        listAuthor.Author = null;

                        Author__Article createdAuthorArticle = await AccessAPI.createAuthorArticle(listAuthor);
                    }

                    MessageBox.Show("Article modified successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Console.WriteLine(authorsArticle.Count());
                    if (author != null)
                    {
                        bool authorExists = authorsArticle.Any(ab => ab.author_id == author.id);

                        if (!authorExists)
                        {
                            ArticleDeleted?.Invoke(this, new ArticleEventArgs(null, article));
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Failed to modify article.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void DeleteArticle(Author__Article article)
        {
            try
            {
                await AccessAPI.DeleteArticle(article.Article.id);

                if (author != null)
                {
                    ArticleDeleted?.Invoke(this, new ArticleEventArgs(null, article));
                }
                else
                {
                    ArticleDeleted?.Invoke(this, new ArticleEventArgs(article.Article));
                }
                MessageBox.Show("Article deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Failed to delete article.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void AddArticle_Click(object sender, RoutedEventArgs e)
        {
            if (title.Text == string.Empty || publish_year.Text == string.Empty || volume.Text == string.Empty ||
                pages.Text == string.Empty || page_start.Text == string.Empty || page_end.Text == string.Empty ||
                month.Text == string.Empty || issn.Text == string.Empty || eissn.Text == string.Empty ||
                note.Text == string.Empty || name.Text == string.Empty)
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            if (publish_year.Text.Any(Char.IsLetter) || pages.Text.Any(Char.IsLetter) || page_start.Text.Any(Char.IsLetter) || page_end.Text.Any(Char.IsLetter) || month.Text.Any(Char.IsLetter))
            {
                MessageBox.Show("Fields: Year, Month, Number of pages, Page start, Page end must be numbers.");
                return;
            }

            if (author == null && AuthorList.Count() <= 0)
            {
                MessageBox.Show("Please add the author of the article", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!editMode)
            {
                AddArticleToDb(article.Article, AuthorList);
            }
            else
            {
                EditArticleInDb(article, AuthorList);

            }
            Close();

        }

        private void DeletedArticle_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this article? Deleting the article will remove all associated authors.", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteArticle(article);
                Close();
            }
        }

        private void LoadAuthorsAsync(object sender, RoutedEventArgs e)
        {
            List<int> withoutAuthors = new List<int>();

            foreach (Author__Article authorArticle in AuthorList)
            {
                withoutAuthors.Add(authorArticle.Author.id);
            }

            W_Select w_Select = new W_Select(ItemType.Authors, withoutAuthors);

            w_Select.ItemSelected += (s, args) =>
            {
                if (args.SelectedItem is Author selectedAuthor)
                {
                    Author__Article newAa = new Author__Article
                    {
                        Author = selectedAuthor,
                        author_id = selectedAuthor.id,
                    };
                    AuthorList.Add(newAa);
                }

            };

            w_Select.Owner = Application.Current.MainWindow;

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_Select);
        }
    }
}
