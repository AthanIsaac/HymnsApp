using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace HymnsApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        readonly HymnsAttendance2 Attendance;
        bool ShowError = true;

        public MainPage(HymnsAttendance2 attendance)
        {
            InitializeComponent();
            Attendance = attendance;
            Grades.ItemsSource = HymnsAttendance2.OrderedClasses;
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            // no item selected
            if (ShowError)
            {
                if (Grades.SelectedIndex == -1)
                {
                    DisplayAlert("Error", "Please Select A Class From The Menu.", "ok");
                }
                else
                {
                    Navigation.PushAsync(new GradeTabbedPage(Attendance, HymnsAttendance2.OrderedClasses[Grades.SelectedIndex]));
                }
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            ShowError = false;
            Grades.SelectedIndex = -1;
            ShowError = true;
        }

        private void Curriculum_Pressed(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CurriculumPage());
        }

        private void Curriculum_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CurriculumPage());
        }
    }
}
