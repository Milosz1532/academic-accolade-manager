using api_access;
using api_access.models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace giga_pans_ui.Tabs.Dashboard
{
    /// <summary>
    /// Logika interakcji dla klasy C_Dashboard_Main_Tab.xaml
    /// </summary>
    public partial class C_Dashboard_Main_Tab : UserControl
    {

        public SeriesCollection SeriesCollection { get; set; }

        public SeriesCollection ColumnCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public C_Dashboard_Main_Tab()
        {
            InitializeComponent();
            LoadDashboardDataAsync();



            SeriesCollection = new SeriesCollection();


            ColumnCollection = new SeriesCollection();



        }



        public ObservableCollection<Article> RecentArticles { get; set; }
        public ObservableCollection<Book> RecentBooks { get; set; }
        public ObservableCollection<Conference> RecentConferences { get; set; }
        public ObservableCollection<Research> RecentResearches { get; set; }




        private async void LoadDashboardDataAsync()
        {
            try
            {
                DashboardDataDto dashboardData = await AccessAPI.GetDashboardData();
                BooksCount.Text = dashboardData.BooksCount.ToString();
                EmployeesCount.Text = dashboardData.EmployeesCount.ToString();
                ArticlesCount.Text = dashboardData.ArticleCount.ToString();
                ConferencesCount.Text = dashboardData.ConferencesCount.ToString();

                SeriesCollection.Clear(); 


                foreach (var city in dashboardData.AuthorsCountByCity)
                {
                    SeriesCollection.Add(new PieSeries
                    {
                        Title = $"{city.Key}: {city.Value}", 
                        Values = new ChartValues<ObservableValue> { new ObservableValue(city.Value) },
                        DataLabels = true
                    });
                }

                ColumnCollection.Clear();

                ColumnCollection.Add(new ColumnSeries
                {
                    Values = new ChartValues<int>
            {
                dashboardData.BooksCount,
                dashboardData.ArticleCount,
                dashboardData.ConferencesCount,
                dashboardData.ResearchCount
            }
                });

                Labels = new[] { "Books", "Articles", "Conferences", "Research" };
                Formatter = value => value.ToString("N");



                RecentArticles = new ObservableCollection<Article>(dashboardData.RecentArticles);
                RecentBooks = new ObservableCollection<Book>(dashboardData.RecentBooks);
                RecentConferences = new ObservableCollection<Conference>(dashboardData.RecentConferences);
                RecentResearches = new ObservableCollection<Research>(dashboardData.RecentResearches);



                DataContext = this;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data from the API server: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




    }
}
