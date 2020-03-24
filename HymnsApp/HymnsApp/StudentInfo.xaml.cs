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
                StackLayout sl = new StackLayout();
                sl.Children.Add(new Label() { Text = students[i].Key, IsVisible = false });

                Label l = new Label()
                {
                    Text = students[i].Value,
                    Style = Resources["detailTablet"] as Style
                };
                sl.Children.Add(l);
                InfoGrid.Children.Add(l, 0, i);

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
                ImageButton b2 = new ImageButton()
                {
                    Source = "editsmall.png",
                    CommandParameter = sl,
                    Style = Resources["buttonTablet"] as Style
                };
                Button b = new Button()
                {
                    Text = ">",
                    BackgroundColor = Color.FromHex("#EEEEEE"),
                    CommandParameter = sl,
                    FontSize = 20,
                    WidthRequest = 15
                };

                b.Clicked += Edit_Clicked;
                InfoGrid.Children.Add(b, 3, i);
            }
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            Button b = sender as Button;
            StackLayout sl = b.CommandParameter as StackLayout;
            string id = (sl.Children[0] as Label).Text;
            string name = (sl.Children[1] as Label).Text;

            //EditAddStudent(HymnsAttendance2 attendance, string id, string name, string grade, bool add)
            Navigation.PushAsync(new EditAddStudent(Attendance, id, name, ClassName, false));
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