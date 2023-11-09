using api_access;
using api_access.models;
using giga_pans_ui.Controls;
using giga_pans_ui.Tabs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
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
using static giga_pans_ui.W_Select;

namespace giga_pans_ui
{
    /// <summary>
    /// Logika interakcji dla klasy W_ResearchManagement.xaml
    /// </summary>
    
    public class ResearchEventArgs : EventArgs
    {
        public Research Research { get; }

        public Author__Research Author__Research { get; }

        public ResearchEventArgs(Research research, Author__Research author__Research = null)
        {
            Research = research;
            Author__Research = author__Research;
        }
    }
    public partial class W_ResearchManagement : Window
    {
        public event EventHandler<ResearchEventArgs> ResearchAdded;
        public event EventHandler<ResearchEventArgs> ResearchDeleted;

        public Author__Research research = new Author__Research();
        public Author author = new Author();
        public ObservableCollection<Author__Research> AuthorList = new ObservableCollection<Author__Research>();

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

        public W_ResearchManagement(Author author = null, Author__Research research = null)
        {
            InitializeComponent();
            this.author = author;
            autorsListView.ItemsSource = AuthorList;
            if (research != null)
            {
                this.research = research;
                editMode = true;
                AddResearch.Content = "Edit Research";
                deleteResearch.Visibility = Visibility.Visible;
                GetResearchAuthors();
            }
            else
            {
                this.research = new Author__Research();
                this.research.Research = new Research();

                if (author != null)
                {
                    Author__Research author_research = new Author__Research
                    {
                        Author = author,
                        author_id = author.id
                    };
                    AuthorList.Add(author_research);
                }

            }
            this.DataContext = this.research;
        }

        private async void GetResearchAuthors()
        {
            try
            {
                Research autors = await AccessAPI.GetResearchAuthors(research.Research.id);

                AuthorList.Clear();

                foreach (var author in autors.Author__Research)
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
            if (sender is Button button && button.DataContext is Author__Research selectedAuthor)
            {
                if (author != null && selectedAuthor.Author.id == author.id && !editMode) return;
                AuthorList.Remove(selectedAuthor);
            }
        }

        public async void AddResearchToDb(Research research, ObservableCollection<Author__Research> authorsResearch)
        {
            try
            {
                Research createResearch = await AccessAPI.CreateResearch(research);


                if (createResearch != null)
                {
                    research.id = createResearch.id;
                    //Add main autor

                    Author__Research new_ar = null;

                    if (author != null)
                    {
                        Author__Research author_research = new Author__Research
                        {
                            author_id = author.id,
                            research_id = createResearch.id
                        };
                        Author__Research createMainAuthor = await AccessAPI.createAuthorResearch(author_research);

                        new_ar = new Author__Research
                        {
                            Research = createResearch,
                            id = createMainAuthor.id,
                            author_id = author.id,
                            research_id = createResearch.id
                        };
                    }


                    //Add other autors
                    foreach (var listAuthor in authorsResearch)
                    {
                        if (author != null && author.id == listAuthor.author_id) continue;

                        listAuthor.research_id = createResearch.id;
                        listAuthor.Research = null;
                        listAuthor.Author = null;
                        Author__Research createdAuthorResearch = await AccessAPI.createAuthorResearch(listAuthor);
                    }

                    ResearchAdded?.Invoke(this, new ResearchEventArgs(research, new_ar));

                    MessageBox.Show("Research added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception )
            {
                MessageBox.Show("Failed to add research.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void EditResearchInDb(Author__Research research, ObservableCollection<Author__Research> authorsResearch)
        {
            try
            {
                Research editedResearch = await AccessAPI.UpdateResearch(research.Research);

                if (editedResearch != null)
                {
                    await AccessAPI.DeleteResearchAuthors(research.Research.id);


                    foreach (var listAuthor in authorsResearch)
                    {
                        listAuthor.research_id = research.Research.id;
                        listAuthor.Research = null;
                        listAuthor.Author = null;

                        Author__Research createdAuthorResearch = await AccessAPI.createAuthorResearch(listAuthor);
                    }

                    MessageBox.Show("Research modified successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Console.WriteLine(authorsResearch.Count());
                    if (author != null)
                    {
                        bool authorExists = authorsResearch.Any(ab => ab.author_id == author.id);

                        if (!authorExists)
                        {
                            ResearchDeleted?.Invoke(this, new ResearchEventArgs(null, research));
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Failed to modify research.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void DeleteResearch(Author__Research research)
        {
            try
            {
                await AccessAPI.DeleteResearch(research.Research.id);

                if (author != null)
                {
                    ResearchDeleted?.Invoke(this, new ResearchEventArgs(null, research));
                }
                else
                {
                    ResearchDeleted?.Invoke(this, new ResearchEventArgs(research.Research));
                }
                MessageBox.Show("Research deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Failed to delete research.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void AddResearch_Click(object sender, RoutedEventArgs e)
        {
            if (title.Text == string.Empty || dateStartPicker.Box.Text == string.Empty || dateEndPicker.Box.Text == string.Empty || function.Text == string.Empty || financing.Text == string.Empty)
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            if (AuthorList.Count() <= 0)
            {
                MessageBox.Show("Please add the author of the conference", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!editMode)
            {
                AddResearchToDb(research.Research, AuthorList);
            }
            else
            {
                EditResearchInDb(research, AuthorList);
            }
            Close();
        }

        private void DeletedResearch_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this research? Deleting the research will remove all associated authors.", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteResearch(research);
                Close();
            }
        }

        private void LoadAuthorsAsync(object sender, RoutedEventArgs e)
        {
            List<int> withoutAuthors = new List<int>();

            foreach (Author__Research authorResearch in AuthorList)
            {
                withoutAuthors.Add(authorResearch.Author.id);
            }

            W_Select w_Select = new W_Select(ItemType.Authors, withoutAuthors);

            w_Select.ItemSelected += (s, args) =>
            {
                if (args.SelectedItem is Author selectedAuthor)
                {
                    Author__Research newAr = new Author__Research
                    {
                        Author = selectedAuthor,
                        author_id = selectedAuthor.id,
                    };
                    AuthorList.Add(newAr);
                }

            };

            w_Select.Owner = Application.Current.MainWindow;

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_Select);
        }
    }
}
