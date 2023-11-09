using giga_pans_ui.Tabs.Dashboard;
using giga_pans_ui.Tabs.Employees;
using giga_pans_ui.Tabs.Publications;
using System;
using System.Windows;
using System.Windows.Input;
using giga_pans_ui.Tabs;
using api_access.models;
using api_access;
using System.Collections.Generic;

namespace giga_pans_ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TopPanel.MouseDown += delegate (object sender, MouseButtonEventArgs e) { if (e.ChangedButton == MouseButton.Left) DragMove(); };

            DashboardButton.Checked += DashboardButton_Checked;
            EmployeeButton.Checked += EmployeeButton_Checked;
            BooksButton.Checked += BooksButton_Checked;
            ArticlesButton.Checked += ArticlesButton_Checked;
            ConferencesButton.Checked += ConferencesButton_Checked;
            ResearchButton.Checked += ResearchButton_Checked;

            DashboardButton.IsChecked = true;
        }

        private void DashboardButton_Checked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Dashboard_Main_Tab();
        }

        private void EmployeeButton_Checked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Employees_Main_Tab(contentControl);
        }

        private void BooksButton_Checked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Books_Main_Tab(contentControl);
        }

        private void ArticlesButton_Checked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Articles_Main_Tab(contentControl);
        }

        private void ConferencesButton_Checked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Conferences_Main_Tab(contentControl);
        }

        private void ResearchButton_Checked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Research_Main_Tab(contentControl);
        }

        private void Button_CloseWindow(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_MaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new Thickness(7);
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }
        }

        private void Button_MinimizeWindow(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void logoutButton(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
    }
}