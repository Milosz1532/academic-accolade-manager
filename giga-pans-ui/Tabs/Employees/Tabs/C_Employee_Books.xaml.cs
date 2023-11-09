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
using System.Security.Cryptography;
using static giga_pans_ui.W_Select;

namespace giga_pans_ui.Tabs.Employees.Tabs
{
    public partial class C_Employee_Books : UserControl
    {

        private C_Employees_Add_Tab parentWindow;
        public ObservableCollection<Author__Book> authorBooks = new ObservableCollection<Author__Book>();
        Author author = null;


        private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            var col1 = 0.50;

            var col2 = 0.10;

            var col3 = 0.20;

            var col4 = 0.20;



            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
        }

        public C_Employee_Books(C_Employees_Add_Tab parentWindow, ObservableCollection<Author__Book> authorBooks, Author author)
        {
            InitializeComponent();
            this.authorBooks = authorBooks;
            this.author = author;
            this.parentWindow = parentWindow;
            bookListView.ItemsSource = authorBooks;
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            W_BookManagement w_BookManagement = new W_BookManagement(author);
            w_BookManagement.Owner = Application.Current.MainWindow;

            w_BookManagement.BookAdded += (s, args) =>
            {
                
                authorBooks.Add(args.Author__Book);
            };

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_BookManagement);
        }


        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            Author__Book selectedBook = (Author__Book)bookListView.SelectedItem;

            if (selectedBook != null)
            {


                W_BookManagement w_BookManagement = new W_BookManagement(author, selectedBook);
                w_BookManagement.Owner = Application.Current.MainWindow;

                w_BookManagement.BookDeleted += (s, args) =>
                {
                    authorBooks.Remove(args.Author__Book);
                };

                WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

                windowLocker.ShowModal(w_BookManagement);

            }
        }

        private void bookListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Edit.IsEnabled = (bookListView.SelectedItem != null);


        }

        private void AddExistingButtonClick(object sender, RoutedEventArgs e)
        {

            List<int> withoutBooks = new List<int>();

            foreach (Author__Book book in authorBooks)
            {
                withoutBooks.Add(book.Book.id);
            }

            W_Select w_Select = new W_Select(ItemType.Books, withoutBooks);

            w_Select.ItemSelected += async (s, args) =>
            {
                if (args.SelectedItem is Book selected)
                {
                    try
                    {
                        Author__Book new_ab = new Author__Book
                        {
                            author_id = author.id,
                            book_id = selected.id,
                            participation = 100
                        };
                        Author__Book createdAuthorBook = await AccessAPI.createAuthorBook(new_ab);
                        new_ab.Book = selected;
                        authorBooks.Add(new_ab);
                        MessageBox.Show("Book added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to add book.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            };

            w_Select.Owner = Application.Current.MainWindow;

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_Select);
        }
    }
}
