using giga_pans_ui.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace giga_pans_ui.Controls
{
    public partial class ExtendableList : UserControl
    {
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register("ItemSource", typeof(IEnumerable), typeof(ExtendableList));

        public IEnumerable<HeaderPropertyMapping> HeaderMappings
        {
            get { return (IEnumerable<HeaderPropertyMapping>)GetValue(HeaderMappingsProperty); }
            set { SetValue(HeaderMappingsProperty, value); }
        }

        public static readonly DependencyProperty HeaderMappingsProperty = DependencyProperty.Register("HeaderMappings", typeof(IEnumerable<HeaderPropertyMapping>), typeof(ExtendableList));

        public string ButtonText { get; set; }

        public ExtendableList()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (HiddenContentContainer.Visibility == Visibility.Collapsed)
            {
                HiddenContentContainer.Visibility = Visibility.Visible;
                Search.Visibility = Visibility.Visible;
            }
            else
            {
                HiddenContentContainer.Visibility = Visibility.Collapsed;
                Search.Visibility = Visibility.Collapsed;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateListViewColumns();
        }

        private void UpdateListViewColumns()
        {
            GridView gridView = new GridView();

            foreach (var header in HeaderMappings)
            {
                var column = new GridViewColumn
                {
                    Header = header.Header,
                    DisplayMemberBinding = new System.Windows.Data.Binding(header.PropertyName)
                };
                gridView.Columns.Add(column);
            }

            ListViewControl.View = gridView;
        }
    }
}