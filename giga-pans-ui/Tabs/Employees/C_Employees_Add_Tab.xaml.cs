using api_access;
using api_access.models;
using giga_pans_ui.Tabs.Dashboard;
using giga_pans_ui.Tabs.Employees.Tabs;
using giga_pans_ui.Tabs.Publications;
using System;
using System.Linq; 
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using giga_pans_ui.Controls;

namespace giga_pans_ui.Tabs.Employees
{
    /// <summary>
    /// Logika interakcji dla klasy C_Employees_Add_Tab.xaml
    /// </summary>
    public partial class C_Employees_Add_Tab : UserControl
    {
        private ContentControl contentControl;
        private Author newAuthor = new Author();
        public ObservableCollection<Author__Book> authorBooks = new ObservableCollection<Author__Book>();
        private ObservableCollection<Author__Article> authorArticles = new ObservableCollection<Author__Article>();
        private ObservableCollection<Author__Conference> authorConferences = new ObservableCollection<Author__Conference>();
        private ObservableCollection<Author__Research> authorResearch = new ObservableCollection<Author__Research>();
       
        
        private List<Author__Title> oldAuthor_Titles = new List<Author__Title>();


        private bool editMode = false;

        public C_Employees_Add_Tab(ContentControl contentControl, Author editAuthor = null)
        {
            this.contentControl = contentControl;

            InitializeComponent();

            SummaryButton.Checked += SummaryButton_Checked;
            BooksButton.Checked += BooksButton_Checked;
            ArticlesButton.Checked += ArticlesButton_Checked;
            ConferencesButton.Checked += ConferencesButton_Checked;
            ResearchButton.Checked += ResearchButton_Checked;




            if (editAuthor != null)
            {
                newAuthor = editAuthor;
                editMode = true;

                LeftSectionConfirmBtn.Content = "Edit data";

                ChangeRightSectionEditMode(true);

                GetDepartmentsList(true);
                GetTitlesAndDegreesList(true);

                authorBooks = new ObservableCollection<Author__Book>(editAuthor.Author__Book);
                authorArticles = new ObservableCollection<Author__Article>(editAuthor.Author__Article);
                authorConferences = new ObservableCollection<Author__Conference>(editAuthor.Author__Conference);
                authorResearch = new ObservableCollection<Author__Research>(editAuthor.Author__Research);



                SummaryButton.IsChecked = true;

                if (editAuthor.Author__Title != null)
                {
                    foreach (Author__Title authorTitleElement in editAuthor.Author__Title)
                    {
                        if (authorTitleElement != null)
                        {
                            oldAuthor_Titles.Add(authorTitleElement);
                        }
                    }
                }
            }
            else
            {
                GetDepartmentsList(false);
                GetTitlesAndDegreesList(false);
                LeftSectionCancelBtn.Visibility = Visibility.Visible;
            }
            this.DataContext = newAuthor;
        }

        private async void GetDepartmentsList(bool editing)
        {
            try
            {
                List<Department> departments = await AccessAPI.GetDepartments();


                DepartmentsComboBox.ItemsSource = departments;
                DepartmentsComboBox.DisplayMemberPath = "name";
                DepartmentsComboBox.SelectedValuePath = "id";

                if (editing)
                {
                    DepartmentsComboBox.SelectedItem = departments.Find(x => x.id == newAuthor.department_id);
                }
            }
            catch
            {
            }
        }

        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            string authorTitles = "";
            foreach (CheckBox checkBox in titlesCombobox.Items)
            {
                if(checkBox.IsChecked == true)
                    authorTitles += checkBox.Content + ", ";
            }

            if (authorTitles.Length != 0)
            {
                authorTitles = authorTitles.Trim();
                authorTitles = authorTitles.Substring(0, authorTitles.Length - 1);
            }
            else
                authorTitles = "Select title...";

            titleDisplay.Text = authorTitles;
        }

        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            string authorTitles = "";
            foreach (CheckBox checkBox in titlesCombobox.Items)
            {
                if (checkBox.IsChecked == true)
                    authorTitles += checkBox.Content + ", ";
            }

            if (authorTitles.Length != 0)
            {
                authorTitles = authorTitles.Trim();
                authorTitles = authorTitles.Substring(0, authorTitles.Length - 1);
            }
            else
                authorTitles = "Select title...";

            titleDisplay.Text = authorTitles;
        }

        private async void GetTitlesAndDegreesList(bool editing)
        {
            string authorTitles = "";
            try
            {
                List<TitleAndDegree> results = await AccessAPI.GetTitlesAndDegrees();

                titlesCombobox.ItemsSource = null;

                foreach (var titleAndDegree in results)
                {
                    CheckBox checkBox = new CheckBox
                    {
                        Content = titleAndDegree.name,
                        Tag = titleAndDegree,
                    };

                    checkBox.Checked += Checkbox_Checked;
                    checkBox.Unchecked += Checkbox_Unchecked;

                    titlesCombobox.Items.Add(checkBox);

                    if (editing)
                    {
                        bool isTitleAssigned = newAuthor.Author__Title.Any(title => title.title_id == titleAndDegree.id);

                        checkBox.IsChecked = isTitleAssigned;
                        if(checkBox.IsChecked == true)
                            authorTitles += checkBox.Content + ", ";
                    }
                }

            }
            catch
            {
            }

            if(authorTitles.Length != 0)
            {
                authorTitles = authorTitles.Trim();
                authorTitles = authorTitles.Substring(0, authorTitles.Length - 1);
            }
            else
                authorTitles = "Select title...";

            titleDisplay.Text = authorTitles;
        }


        private void SummaryButton_Checked(object sender, RoutedEventArgs e)
        {
            employeeContentControl.Content = new C_Employee_Summary(authorBooks, authorArticles, authorConferences, authorResearch);
        }

        private void BooksButton_Checked(object sender, RoutedEventArgs e)
        {
            employeeContentControl.Content = new C_Employee_Books(this, authorBooks, newAuthor);
        }

        private void ArticlesButton_Checked(object sender, RoutedEventArgs e)
        {
            employeeContentControl.Content = new C_Employee_Articles(this, authorArticles, newAuthor);
        }


        private void ConferencesButton_Checked(object sender, RoutedEventArgs e)
        {
            employeeContentControl.Content = new C_Employee_Conferences(this, authorConferences, newAuthor);
        }

        private void ResearchButton_Checked(object sender, RoutedEventArgs e)
        {
            employeeContentControl.Content = new C_Employee_Research(this, authorResearch, newAuthor);
        }


        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Employees_Main_Tab(contentControl);

        }

        private void Add_Employee_Cancel_Btn(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new C_Employees_Main_Tab(contentControl);

        }

        public void ChangeRightSectionEditMode(bool state)
        {
            if (state)
            {
                RightSection.Opacity = 1;
                RightSection.IsEnabled = true;
            }else
            {
                RightSection.Opacity = 0.4;
                RightSection.IsEnabled = false;
            }
        }

        private async void Add_Employee_Confirm_Btn(object sender, RoutedEventArgs e)
        {
            if (first_name.Text == string.Empty || last_name.Text == string.Empty || phone_number.Text == string.Empty ||
                email.Text == string.Empty || address.Text == string.Empty || city.Text == string.Empty ||
                birth_date.Box.Text == string.Empty || DepartmentsComboBox.SelectedItem == null || orcid.Text == string.Empty)
            {
                MessageBox.Show("All personal data fields must be filled.");
                return;
            }

            

            if (editMode == true)
            {
                try
                {
                    Department authorDepartment = (Department)DepartmentsComboBox.SelectedItem;
                    newAuthor.department_id = authorDepartment.id;
                    Author updatedAuthor = await AccessAPI.UpdateAuthor(newAuthor);



                    List<Author__Title> newAuthor_Titles = new List<Author__Title>();

                    foreach (CheckBox checkBox in titlesCombobox.Items)
                    {
                        if (checkBox.IsChecked == true)
                        {
                            TitleAndDegree title = (TitleAndDegree)checkBox.Tag;
                            Author__Title author__Title = new Author__Title
                            {
                                author_id = newAuthor.id,
                                title_id = title.id,
                            };
                            newAuthor_Titles.Add(author__Title);
                        }
                    }

                    AccessAPI.UpdateAllAuthorTitles(oldAuthor_Titles, newAuthor_Titles);





                    if (updatedAuthor != null)
                    {
                        MessageBox.Show("Employee data updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update employee data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating employee data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return;
            }

            try
            {
                Department authorDepartment = (Department)DepartmentsComboBox.SelectedItem;
                newAuthor.department_id = authorDepartment.id;
                Author createdAuthor = await AccessAPI.CreateAuthor(newAuthor);
                if (createdAuthor != null)
                {

                    foreach (CheckBox checkBox in titlesCombobox.Items)
                    {
                        if (checkBox.IsChecked == true)
                        {
                            TitleAndDegree title = (TitleAndDegree)checkBox.Tag;
                            Author__Title author__Title = new Author__Title
                            {
                                author_id = createdAuthor.id,
                                title_id = title.id,
                            };
                            AccessAPI.CreateAuthor_Title(author__Title);
                        }
                    }

                    newAuthor = createdAuthor;
                    editMode = true;

                    ChangeRightSectionEditMode(true);


                    


                    MessageBox.Show("Author added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add author.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Failed to add author.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void titlesCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.SelectedItem = null;
        }
    }
}