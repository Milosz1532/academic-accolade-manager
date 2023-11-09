using api_access;
using api_access.models;
using giga_pans_ui.Tabs.Employees;
using giga_pans_ui.Tabs.Employees.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static giga_pans_ui.W_Select;

namespace giga_pans_ui.Tabs
{
    /// <summary>
    /// Logika interakcji dla klasy W_BookManagement.xaml
    /// </summary>
    /// 

    public class BookEventArgs : EventArgs
    {
        public Book Book { get; }

        public Author__Book Author__Book { get; }

        public BookEventArgs(Book book, Author__Book author__Book = null)
        {
            Book = book;
            Author__Book = author__Book;
        }
    }


    public partial class W_BookManagement : Window
    {


        public event EventHandler<BookEventArgs> BookAdded;
        public event EventHandler<BookEventArgs> BookDeleted;



        public Author__Book book = new Author__Book();
        public Author author = new Author();
        public ObservableCollection<Author__Book> AuthorList = new ObservableCollection<Author__Book>();

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



        public W_BookManagement(Author author = null, Author__Book book = null)
        {

            InitializeComponent();
            this.author = author;
            autorsListView.ItemsSource = AuthorList;

            GetPublishersList();
            if (book != null)
            {
                this.book = book;
                editMode = true;
                AddBook.Content = "Edit Book";
                deleteBook.Visibility = Visibility.Visible;
                GetBookAuthors();
            } else
            {
                this.book = new Author__Book();
                this.book.Book = new Book();

                if (author != null)
                {
                    Author__Book author_book = new Author__Book
                    {
                        Author = author,
                        author_id = author.id,
                        participation = 100,
                    };
                    AuthorList.Add(author_book);
                }

                
            }

            this.DataContext = this.book;

        }

        private async void GetPublishersList()
        {
            try
            {
                List<Publisher> publishers = await AccessAPI.GetPublishers();

                publisherBox.ItemsSource = publishers;
                publisherBox.DisplayMemberPath = "name";
                publisherBox.SelectedValuePath = "id";

                if (editMode)
                {
                    publisherBox.SelectedItem = publishers.Find(x => x.id == book.Book.publisher_id);
                }
            }
            catch
            { }
        }

        private async void GetBookAuthors()
        {
            try
            {
                Book autors = await AccessAPI.GetBookAuthors(book.Book.id);

                AuthorList.Clear();

                foreach (var author in autors.Author__Book)
                {
                    AuthorList.Add(author);
                }



            }
            catch 
            {
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e) // Close Window
        {
            Close();
        }

        private void Delete_Author_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Author__Book selectedAuthor)
            {
                if (author != null && selectedAuthor.Author.id == author.id && !editMode) return;
                AuthorList.Remove(selectedAuthor);

            }
        }

        public async void AddBookToDb(Book book, ObservableCollection<Author__Book> authorsBook)
        {
            try
            {
                Book createBook = await AccessAPI.CreateBook(book);


                if (createBook != null)
                {
                    book.id = createBook.id;
                    //Add main autor

                    Author__Book new_ab = null;

                    if (author != null)
                    {
                        Author__Book author_book = new Author__Book
                        {
                            author_id = author.id,
                            book_id = createBook.id,
                            participation = 100,
                        };
                        Author__Book createMainAuthor = await AccessAPI.createAuthorBook(author_book);

                        new_ab = new Author__Book
                        {
                            Book = createBook,
                            id = createMainAuthor.id,
                            author_id = author.id,
                            book_id = createBook.id,
                            participation = 100
                        };
                    }
                   

                    //Add other autors
                    foreach (var listAuthor in authorsBook)
                    {
                        if (author != null && author.id == listAuthor.author_id) continue;

                        listAuthor.book_id = createBook.id;
                        listAuthor.participation = 100;
                        listAuthor.Book = null;
                        listAuthor.Author = null;
                        Author__Book createdAuthorBook = await AccessAPI.createAuthorBook(listAuthor);
                    }

                    BookAdded?.Invoke(this, new BookEventArgs(book, new_ab));



                    MessageBox.Show("Book added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }



            }
            catch (Exception)
            {
                MessageBox.Show("Failed to add book.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void EditBookInDb(Author__Book book, ObservableCollection<Author__Book> authorsBook)
        {
            try
            {
                Book editedBook = await AccessAPI.UpdateBook(book.Book);

                if (editedBook != null)
                {
                    await AccessAPI.DeleteBookAuthors(book.Book.id);


                    foreach (var listAuthor in authorsBook)
                    {
                        listAuthor.book_id = book.Book.id;
                        listAuthor.participation = 100;
                        listAuthor.Book = null;
                        listAuthor.Author = null;

                        Author__Book createdAuthorBook = await AccessAPI.createAuthorBook(listAuthor);
                    }

                    MessageBox.Show("Book modified successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Console.WriteLine(authorsBook.Count());
                    if (author != null)
                    {
                        bool authorExists = authorsBook.Any(ab => ab.author_id == author.id);

                        if (!authorExists)
                        {
                            BookDeleted?.Invoke(this, new BookEventArgs(null, book));
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Failed to modify book.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void DeleteBook(Author__Book book)
        {
            try
            {
                await AccessAPI.DeleteBook(book.Book.id);

                if (author != null)
                {
                    BookDeleted?.Invoke(this, new BookEventArgs(null, book));
                }
                else {
                    BookDeleted?.Invoke(this, new BookEventArgs(book.Book));
                }
                MessageBox.Show("Book deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Failed to delete book.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }




        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            Publisher selectedPublisher = publisherBox.SelectedItem as Publisher;
            if (title.Text == string.Empty || publish_year.Text == string.Empty || month.Text == string.Empty ||
               pages.Text == string.Empty || address.Text == string.Empty || series.Text == string.Empty ||
               edition.Text == string.Empty || notes.Text == string.Empty || volume.Text == string.Empty ||
               publisherBox.SelectedItem == null || isbn_ebook.Text == string.Empty ||
               isbn_softcover.Text == string.Empty || isbn_hardcover.Text == string.Empty)
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            if (publish_year.Text.Any(Char.IsLetter) || month.Text.Any(Char.IsLetter) || pages.Text.Any(Char.IsLetter))
            {
                MessageBox.Show("Fields: Year, Month, Number of pages must be a number.");
                return;
            }

            if (author == null && AuthorList.Count() <= 0)
            {
                MessageBox.Show("Please add the author of the book", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Publisher publisher = (Publisher)publisherBox.SelectedItem;
            book.Book.publisher_id = publisher.id;

            if (!editMode)
            {
                AddBookToDb(book.Book, AuthorList);
            }
            else
            {
                EditBookInDb(book, AuthorList);
            }
            Close();
        }
        private void DeletedBook_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this book? Deleting the book will remove all associated authors.", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteBook(book);
                Close();
            }
        }


        private void LoadAuthorsAsync(object sender, RoutedEventArgs e)
        {
            List<int> withoutAuthors = new List<int>();

            foreach (Author__Book authorBook in AuthorList)
            {
                withoutAuthors.Add(authorBook.Author.id);
            }

            W_Select w_Select = new W_Select(ItemType.Authors, withoutAuthors);

            w_Select.ItemSelected += (s, args) =>
            {
                if (args.SelectedItem is Author selectedAuthor)
                {
                    Author__Book newAb = new Author__Book
                    {
                        Author = selectedAuthor,
                        author_id = selectedAuthor.id,
                    };
                    AuthorList.Add(newAb);
                }

            };

            w_Select.Owner = Application.Current.MainWindow;

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_Select);
        }


    }
}