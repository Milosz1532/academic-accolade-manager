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

namespace giga_pans_ui.Tabs.Employees.Tabs
{
    /// <summary>
    /// Logika interakcji dla klasy C_Employee_Summary.xaml
    /// </summary>
    public partial class C_Employee_Summary : UserControl
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

        public C_Employee_Summary(ObservableCollection<Author__Book> authorBooks, ObservableCollection<Author__Article> authorArticles, ObservableCollection<Author__Conference> authorConferences, ObservableCollection<Author__Research> authorResearch)
        {
            InitializeComponent();
            bookListView.ItemsSource = authorBooks;
            articlesListView.ItemsSource = authorArticles;
            conferenceListView.ItemsSource = authorConferences;
            researchListView.ItemsSource = authorResearch;

        }
    }
}
