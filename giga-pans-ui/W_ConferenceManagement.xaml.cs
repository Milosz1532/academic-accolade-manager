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
using static giga_pans_ui.W_Select;

namespace giga_pans_ui
{
    /// <summary>
    /// Logika interakcji dla klasy W_ConferenceManagement.xaml
    /// </summary>
    public class ConferenceEventArgs : EventArgs
    {
        public Conference Conference { get; }

        public Author__Conference Author__Conference { get; }

        public ConferenceEventArgs(Conference conference, Author__Conference author__Conference = null)
        {
            Conference = conference;
            Author__Conference = author__Conference;
        }
    }

    public partial class W_ConferenceManagement : Window
    {
        public event EventHandler<ConferenceEventArgs> ConferenceAdded;
        public event EventHandler<ConferenceEventArgs> ConferenceDeleted;

        public Author__Conference conference = new Author__Conference();
        public Author author = new Author();
        public ObservableCollection<Author__Conference> AuthorList = new ObservableCollection<Author__Conference>();

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

        public W_ConferenceManagement(Author author = null, Author__Conference conference = null)
        {
            InitializeComponent();
            this.author = author;
            autorsListView.ItemsSource = AuthorList;

            ParticipationBox.Items.Add("Yes");
            ParticipationBox.Items.Add("No");
            if (conference != null)
            {
                this.conference = conference;
                editMode = true;
                addConference.Content = "Edit Conference";
                deleteConference.Visibility = Visibility.Visible;
                GetConferenceAuthors();
                ParticipationBox.SelectedItem = (conference.Conference.active_participation == 1) ? "Yes" : "No";
            }
            else
            {
                this.conference = new Author__Conference();
                this.conference.Conference = new Conference();

                if (author != null)
                {
                    Author__Conference author_conference = new Author__Conference
                    {
                        Author = author,
                        author_id = author.id,
                        participation = 100,
                    };
                    AuthorList.Add(author_conference);
                }
            }

            this.DataContext = this.conference;
        }

        private async void GetConferenceAuthors()
        {
            try
            {
                Conference autors = await AccessAPI.GetConferenceAuthors(conference.Conference.id);

                AuthorList.Clear();

                foreach (var author in autors.Author__Conference)
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
            if (sender is Button button && button.DataContext is Author__Conference selectedAuthor)
            {
                if (author != null && selectedAuthor.Author.id == author.id && !editMode) return;
                AuthorList.Remove(selectedAuthor);
            }
        }

        public async void AddConferenceToDb(Conference conference, ObservableCollection<Author__Conference> authorsConference)
        {
            try
            {
                Conference createConference = await AccessAPI.CreateConference(conference);


                if (createConference != null)
                {

                    //Add main autor
                    conference.id = createConference.id;
                    Author__Conference new_ac = null;

                    if (author != null)
                    {
                        Author__Conference author_conference = new Author__Conference
                        {
                            author_id = author.id,
                            conference_id = createConference.id,
                            participation = 100,
                        };
                        Author__Conference createMainAuthor = await AccessAPI.createAuthorConference(author_conference);

                        new_ac = new Author__Conference
                        {
                            Conference = createConference,
                            id = createMainAuthor.id,
                            author_id = author.id,
                            conference_id = createConference.id,
                            participation = 100
                        };
                    }
                    //Add other autors
                    foreach (var listAuthor in authorsConference)
                    {
                        if (author != null && author.id == listAuthor.author_id) continue;

                        listAuthor.conference_id = createConference.id;
                        listAuthor.participation = 100;
                        listAuthor.Conference = null;
                        listAuthor.Author = null;
                        Author__Conference createdAuthorConference = await AccessAPI.createAuthorConference(listAuthor);
                    }

                    ConferenceAdded?.Invoke(this, new ConferenceEventArgs(conference, new_ac));



                    MessageBox.Show("Conference added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Failed to add conference.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void EditConferenceInDb(Author__Conference conference, ObservableCollection<Author__Conference> authorsConference)
        {
            try
            {
                Conference editedConference = await AccessAPI.UpdateConference(conference.Conference);

                if (editedConference != null)
                {
                    await AccessAPI.DeleteConferenceAuthors(conference.Conference.id);


                    foreach (var listAuthor in authorsConference)
                    {
                        listAuthor.conference_id = conference.Conference.id;
                        listAuthor.participation = 100;
                        listAuthor.Conference = null;
                        listAuthor.Author = null;

                        Author__Conference createdAuthorConference = await AccessAPI.createAuthorConference(listAuthor);
                    }

                    MessageBox.Show("Conference modified successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (author != null)
                    {
                        bool authorExists = authorsConference.Any(ab => ab.author_id == author.id);

                        if (!authorExists)
                        {
                            ConferenceDeleted?.Invoke(this, new ConferenceEventArgs(null, conference));
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Failed to modify conference.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void DeleteConference(Author__Conference conference)
        {
            try
            {
                await AccessAPI.DeleteConference(conference.Conference.id);

                if (author != null)
                {
                    ConferenceDeleted?.Invoke(this, new ConferenceEventArgs(null, conference));
                }
                else
                {
                    ConferenceDeleted?.Invoke(this, new ConferenceEventArgs(conference.Conference));
                }
                MessageBox.Show("Conference deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Failed to delete conference.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddConference_Click(object sender, RoutedEventArgs e)
        {

            if (title.Text == string.Empty || address.Text == string.Empty || datePicker.Box.Text == string.Empty || organization.Text == string.Empty || note.Text == string.Empty)
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            if (ParticipationBox.SelectedItem == null)
            {
                MessageBox.Show("Active participation filled must be filled.");
                return;
            }

            if (author == null && AuthorList.Count() <= 0)
            {
                MessageBox.Show("Please add the author of the conference", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Byte active_participation;
            if ((string)ParticipationBox.SelectedItem == "Yes")
                active_participation = 1;
            else
                active_participation = 0;

            conference.Conference.active_participation = active_participation;

            if (!editMode)
            {
                AddConferenceToDb(conference.Conference, AuthorList);
            }
            else
            {
                EditConferenceInDb(conference, AuthorList);
            }

            Close();
        }

        private void DeletedConference_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this conference? Deleting the conference will remove all associated authors.", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DeleteConference(conference);
                Close();
            }
        }

        private void LoadAuthorsAsync(object sender, RoutedEventArgs e)
        {
            List<int> withoutAuthors = new List<int>();

            foreach (Author__Conference authorConference in AuthorList)
            {
                withoutAuthors.Add(authorConference.Author.id);
            }

            W_Select w_Select = new W_Select(ItemType.Authors, withoutAuthors);

            w_Select.ItemSelected += (s, args) =>
            {
                if (args.SelectedItem is Author selectedAuthor)
                {
                    Author__Conference newAc = new Author__Conference
                    {
                        Author = selectedAuthor,
                        author_id = selectedAuthor.id,
                    };
                    AuthorList.Add(newAc);
                }

            };

            w_Select.Owner = Application.Current.MainWindow;

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_Select);
        }
    }
}
