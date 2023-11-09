using api_access;
using giga_pans_ui.Helpers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace giga_pans_ui.Tabs.Publications
{
    public partial class C_Publications_Main_Tab : UserControl
    {
        public ObservableCollection<HeaderPropertyMapping> BookHeadersCollection { get; } = new ObservableCollection<HeaderPropertyMapping>
        {
            new HeaderPropertyMapping { Header = "Book Title", PropertyName = "title" },
            new HeaderPropertyMapping { Header = "Volume Number", PropertyName = "volume" },
            new HeaderPropertyMapping { Header = "Publication Year", PropertyName = "year" }
        };

        public ObservableCollection<HeaderPropertyMapping> ArticleHeadersCollection { get; } = new ObservableCollection<HeaderPropertyMapping>
        {
            new HeaderPropertyMapping { Header = "Article Title", PropertyName = "title" },
            new HeaderPropertyMapping { Header = "Journal", PropertyName = "name" },
            new HeaderPropertyMapping { Header = "Publication Year", PropertyName = "year" }
        };

        public ObservableCollection<HeaderPropertyMapping> ResearchHeadersCollection { get; } = new ObservableCollection<HeaderPropertyMapping>
        {
            new HeaderPropertyMapping { Header = "Research", PropertyName = "title" },
            new HeaderPropertyMapping { Header = "Start Date", PropertyName = "date_start" },
            new HeaderPropertyMapping { Header = "End Date", PropertyName = "date_end" }
        };

        public C_Publications_Main_Tab()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void booklistview_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                booklistview.ItemSource = await AccessAPI.GetBooks();
            }
            catch
            {
                MessageBox.Show("An error occurred while retrieving books from the API.");
            }
        }

        private async void articlelistview_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                articlelistview.ItemSource = await AccessAPI.GetArticles();
            }
            catch
            {
                MessageBox.Show("An error occurred while retrieving books from the API.");
            }
        }

        private async void researchlistview_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                researchlistview.ItemSource = await AccessAPI.GetResearches();
            }
            catch
            {
                MessageBox.Show("An error occurred while retrieving books from the API.");
            }
        }
    }
}