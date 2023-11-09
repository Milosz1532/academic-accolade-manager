using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace giga_pans_ui.Helpers
{
    public class GridViewHeaderConverter : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is IEnumerable<HeaderPropertyMapping>) || !(values[1] is IEnumerable<object>))
                return null;

            var headers = (IEnumerable<HeaderPropertyMapping>)values[0];
            var items = (IEnumerable<object>)values[1];

            var gridViewColumns = new List<GridViewColumn>();

            foreach (var header in headers)
            {
                var binding = new System.Windows.Data.Binding(header.PropertyName);
                var column = new GridViewColumn
                {
                    Header = header.Header,
                    DisplayMemberBinding = binding
                };
                gridViewColumns.Add(column);
            }

            return gridViewColumns;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}