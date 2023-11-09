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
using static giga_pans_ui.W_Select;
using System.Collections.ObjectModel;

namespace giga_pans_ui.Tabs.Employees.Tabs
{
    /// <summary>
    /// Logika interakcji dla klasy C_Employee_Research.xaml
    /// </summary>
    public partial class C_Employee_Research : UserControl
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
        public ObservableCollection<Author__Research> authorResearch = new ObservableCollection<Author__Research>();
        Author author = null;
        public C_Employee_Research(C_Employees_Add_Tab parentWindow, ObservableCollection<Author__Research> authorResearch, Author author)
        {
            InitializeComponent();
            this.authorResearch = authorResearch;
            this.author = author;
            this.parentWindow = parentWindow;
            researchListView.ItemsSource = authorResearch;
        }


        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            W_ResearchManagement w_ResearchManagement = new W_ResearchManagement(author);
            w_ResearchManagement.Owner = Application.Current.MainWindow;

            w_ResearchManagement.ResearchAdded += (s, args) =>
            {
                authorResearch.Add(args.Author__Research);
            };

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_ResearchManagement);
        }


        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            Author__Research selectedConference = (Author__Research)researchListView.SelectedItem;
            if (selectedConference != null)
            {

                W_ResearchManagement W_ResearchManagement = new W_ResearchManagement(author, selectedConference);
                W_ResearchManagement.Owner = Application.Current.MainWindow;

                W_ResearchManagement.ResearchDeleted += (s, args) =>
                {
                    authorResearch.Remove(args.Author__Research);
                };

                WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

                windowLocker.ShowModal(W_ResearchManagement);

            }
        }



        private void AddExistingButtonClick(object sender, RoutedEventArgs e)
        {

            List<int> withoutResearch = new List<int>();

            foreach (Author__Research research in authorResearch)
            {
                withoutResearch.Add(research.Research.id);
            }

            W_Select w_Select = new W_Select(ItemType.Research, withoutResearch);

            w_Select.ItemSelected += async (s, args) =>
            {
                if (args.SelectedItem is Research selected)
                {
                    try
                    {
                        Author__Research new_ab = new Author__Research
                        {
                            author_id = author.id,
                            research_id = selected.id,
                        };
                        Author__Research createdAuthor = await AccessAPI.createAuthorResearch(new_ab);
                        new_ab.Research = selected;
                        authorResearch.Add(new_ab);
                        MessageBox.Show("Research added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to add research.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            };

            w_Select.Owner = Application.Current.MainWindow;

            WindowLocker windowLocker = new WindowLocker(Application.Current.MainWindow);

            windowLocker.ShowModal(w_Select);
        }


        private void Listview_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            Button_Edit.IsEnabled = (researchListView.SelectedItem != null);
        }
    }
}
