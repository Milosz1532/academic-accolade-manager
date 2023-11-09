using System;
using System.Windows;
using System.Windows.Controls;

namespace giga_pans_ui.Controls
{
    public partial class C_DatePicker : UserControl
    {
        public static readonly DependencyProperty SelectedDateProperty =
            DatePicker.SelectedDateProperty.AddOwner(typeof(C_DatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }


        public C_DatePicker()
        {
            InitializeComponent();
        }

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            C_DatePicker datePicker = (C_DatePicker)d;
            DatePicker innerDatePicker = datePicker.Box;

            innerDatePicker.SelectedDateChanged -= datePicker.InnerDatePicker_SelectedDateChanged;

            if (e.NewValue is DateTime newDate)
            {
                innerDatePicker.SelectedDate = newDate;
                innerDatePicker.Text = newDate.ToShortDateString();
            }
            else
            {
                innerDatePicker.SelectedDate = null;
                innerDatePicker.Text = string.Empty;
            }

            innerDatePicker.SelectedDateChanged += datePicker.InnerDatePicker_SelectedDateChanged;
        }

        private void InnerDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDate = Box.SelectedDate;
        }
    }
}