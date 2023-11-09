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
    /// Logika interakcji dla klasy C_Employee_Conferences.xaml
    /// </summary>
    public partial class C_Employee_Conferences : UserControl
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
        public ObservableCollection<Author__Conference> authorConferences = new ObservableCollection<Author__Conference>();
        Author author = null;

        public C_Employee_Conferences(C_Employees_Add_Tab parentWindow, ObservableCollection<Author__Conference> authorConferences, Author author)
        {
            InitializeComponent();
            this.authorConferences = authorConferences;
            this.author = author;
            this.parentWindow = parentWindow;
            conferenceListView.ItemsSource = authorConferences;

        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            W_ConferenceManagement w_ConferenceManagement = new W_ConferenceManagement(author);
            w_ConferenceManagement.Owner = Application.Current.MainWindow;

            w_ConferenceManagement.ConferenceAdded += (s, args) =>
            {
                authorConferences.Add(args.Author__Conference);
            };

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_ConferenceManagement);
        }


        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            Author__Conference selectedConference = (Author__Conference)conferenceListView.SelectedItem;

            if (selectedConference != null)
            {

                W_ConferenceManagement w_ConferenceManagement = new W_ConferenceManagement(author, selectedConference);
                w_ConferenceManagement.Owner = Application.Current.MainWindow;

                w_ConferenceManagement.ConferenceDeleted += (s, args) =>
                {
                    authorConferences.Remove(args.Author__Conference);
                };

                WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

                windowLocker.ShowModal(w_ConferenceManagement);

            }
        }



        private void AddExistingButtonClick(object sender, RoutedEventArgs e)
        {

            List<int> withoutConferences= new List<int>();

            foreach (Author__Conference conference in authorConferences)
            {
                withoutConferences.Add(conference.Conference.id);
            }

            W_Select w_Select = new W_Select(ItemType.Conferences, withoutConferences);

            w_Select.ItemSelected += async (s, args) =>
            {
                if (args.SelectedItem is Conference selected)
                {
                    try
                    {
                        Author__Conference new_ab = new Author__Conference
                        {
                            author_id = author.id,
                            conference_id = selected.id,
                            participation = 100
                        };
                        Author__Conference createdAuthor = await AccessAPI.createAuthorConference(new_ab);
                        new_ab.Conference = selected;
                        authorConferences.Add(new_ab);
                        MessageBox.Show("Conference added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to add conference.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            };

            w_Select.Owner = Application.Current.MainWindow;

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_Select);
        }


        private void Listview_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Edit.IsEnabled = (conferenceListView.SelectedItem != null);
        }
    }
}
