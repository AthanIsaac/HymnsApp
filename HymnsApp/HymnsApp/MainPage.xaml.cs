using System;
using System.Collections.Generic;
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
            string[] weeklyBirthdays = Attendance.WeeklyBirthdays();
            for (int i = 0; i < weeklyBirthdays.Length; i++) {
                birthdays.Children.Add(new Label { Text = weeklyBirthdays[i]});
            }
            
            Classes.ItemsSource = classesToInterface(HymnsAttendance.OrderedClasses);
        }

        //key = dbClasses, Value = string parsed
        public string[] classesToInterface(string[] dbClasses) {
            string[] visualClasses = new string[dbClasses.Length];

            if (dbClasses == null) 
            {
                return null;
            }


            for (int i = 0; i < dbClasses.Length; i++)
            {
                string c = dbClasses[i];

                if (c.Contains("kindergarten"))
                {
                    c = "Kindergarten";

                }

                if (c.Contains("highSchool"))
                    c = "HighSchool";

                if (c.Contains("Grade"))
                {
                    int index = c.IndexOf("Grade");
                    c = c.Substring(0, index) + " " + c.Substring(index);

                    if (c.Contains("&"))
                    {
                        int ampersand = c.IndexOf("&");
                        c = c.Substring(0, ampersand) + " & " +c.Substring(ampersand + 1);
                    }

                }

                else {
                    //really inefficent, find better way
                    for (int j = 0; j < c.Length; j++) {
                        
                        if (char.IsUpper(c[j])) 
                        {
                            c = c.Substring(0, j) + " " + c.Substring(j);
                            j++;
                        }

                        
                    }

                    c= c.Replace("m", "M");
                }
                visualClasses[i] = c;
            }            

            
           return visualClasses;
        }
       


        private void NextButton_Clicked(object sender, EventArgs e)
        {
            // no item selected
            if (ShowError)
            {
                if (Classes.SelectedIndex == -1)
                {
                    DisplayAlert("Error", "Please Select A Class From The Menu.", "ok");
                }
                else
                {
                    Navigation.PushAsync(new GradeTabbedPage(Attendance, HymnsAttendance.OrderedClasses[Classes.SelectedIndex]));

                }
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            ShowError = false;
            Classes.SelectedIndex = -1;
            ShowError = true;
        }

        private void Curriculum_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CurriculumPage(Attendance));
        }
    }
}
