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
        readonly HymnsAttendance Attendance;
        bool ShowError = true;

        public MainPage(HymnsAttendance attendance)
        {
            InitializeComponent();
            Attendance = attendance;
            Grades.ItemsSource = HymnsAttendance.Classes;
        }

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            // no item selected
            if (ShowError)
            {
                if (Grades.SelectedIndex == -1)
                {
                    DisplayAlert("Error", "Please select a grade", "ok");
                }
                else
                {
                    Navigation.PushAsync(new GradeTabbedPage(Attendance, Grades.SelectedIndex));
                }
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            ShowError = false;
            Grades.SelectedIndex = -1;
            ShowError = true;
        }
    }
}
