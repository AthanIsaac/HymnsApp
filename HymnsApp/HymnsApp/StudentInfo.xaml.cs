using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StudentInfo : ContentPage
    {
        readonly HymnsAttendance2 Attendance;
        readonly string ClassName;
        
        public StudentInfo(HymnsAttendance2 attendance, string className)
        {
            Attendance = attendance;
            ClassName = className;
            InitializeComponent();
            // InitGrid();
        }
        private void InitGrid()
        {
            /* photo? name | phone | attendance days | editbutton */
            var students = Attendance.StudentsOfGrade(ClassName);

            while (students.Count > InfoGrid.RowDefinitions.Count)
            {
                InfoGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(60)
                });
            }


            for (int i = 0; i < students.Count; i++)
            {
               InfoGrid.Children.Add(new Label()
                {
                    Text = students[i].Value,
                    Style = Resources["detailTablet"] as Style
                }, 0, i);

                string num = Attendance.GetStudentPhone(students[i].Key);

                InfoGrid.Children.Add(new Label()
                {
                    Text = num.Length == 0 ? "" : "(" + num.Substring(0, 3) + ")-" + num.Substring(3, 3) + "-" + num.Substring(6),
                    Style = Resources["detailTablet"] as Style
                }, 1, i);

              

                InfoGrid.Children.Add(new Label()
                {
                    Text = Attendance.GetDatesForYear(students[i].Key).ToString(),
                    Style = Resources["detailTablet"] as Style
                }, 2, i);


                /*
                ImageButton b2 = new ImageButton()
                {
                    Source = "editsmall.png",
                    //CommandParameter = sl,
                    Style = Resources["buttonTablet"] as Style
                };
                */

               
               
                Button b = new Button()
                {
                    Text = ">",
                    BackgroundColor = Color.FromHex("#EEEEEE"),
                    FontSize = 20,
                    WidthRequest = 15,
                    CommandParameter = new Label()
                    {
                        Text = students[i].Key,
                        IsVisible = false
                    }
                };

                b.Clicked += Edit_Clicked;

                InfoGrid.Children.Add(b, 3, i);
            }
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            Button b = sender as Button;
            Label sl = b.CommandParameter as Label;
            
            string id = sl.Text;
            

            string [] studentInfo = Attendance.GetStudent(id);
            //EditAddStudent(HymnsAttendance2 attendance, string id, string name, string grade, bool add)
            Navigation.PushAsync(new EditAddStudent(Attendance, id, studentInfo[0], ClassName, false));
        }

        protected override void OnAppearing()
        {
            InitGrid();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditAddStudent(Attendance, "", "",  ClassName, true));
        }
    }
}