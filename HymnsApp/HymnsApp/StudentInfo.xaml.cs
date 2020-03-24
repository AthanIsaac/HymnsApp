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
        readonly string Grade;
        // static readonly int NUM_FIELDS = 4;
        public StudentInfo(HymnsAttendance2 attendance, string grade)
        {
            Attendance = attendance;
            Grade = grade;
            InitializeComponent();
            // InitGrid();
        }
        private void InitGrid()
        {
            /* photo? name | phone | attendance days | editbutton */
            var students = Attendance.StudentsOfGrade(Grade);
           // students.Sort();
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
                    Text = Capitalize(students[i].Value),
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
                    Text = Attendance.GetDatesForYear(students[i].Value).ToString(),
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
                    Text = "EDIT",
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
            Navigation.PushAsync(new EditAddStudent(Attendance, id, name, Grade, false));
        }

        protected override void OnAppearing()
        {
            InitGrid();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditAddStudent(Attendance, "", "",  Grade, true));
        }

        // Credit: Nardin
        private string Capitalize(string name)
        {
            string[] s = name.Split(' ');
            s[0] = char.ToUpper(s[0][0]).ToString() + s[0].Substring(1);
            name = s[0];
            if (s.Length != 1)
            {

                s[1] = char.ToUpper(s[1][0]).ToString() + s[1].Substring(1);
                name += ' ' + s[1];
            }
            return name;
        }
    }
}