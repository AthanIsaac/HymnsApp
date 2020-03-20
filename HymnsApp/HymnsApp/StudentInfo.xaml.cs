using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StudentInfo : ContentPage
    {
        readonly HymnsAttendance Attendance;
        readonly int Grade;
        // static readonly int NUM_FIELDS = 4;
        public StudentInfo(HymnsAttendance attendance, int grade)
        {
            Attendance = attendance;
            Grade = grade;
            InitializeComponent();
            // InitGrid();
        }
        private void InitGrid()
        {
            /* photo? name | phone | attendance days | editbutton */
            List<string> students = Attendance.StudentsOfGrade(Grade);
            students.Sort();
            while (students.Count > InfoGrid.RowDefinitions.Count)
            {
                InfoGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(60)
                });
            }

            for (int i = 0; i < students.Count; i++)
            {
                Label l = new Label()
                {
                    Text = Capitalize(students[i]),
                    Style = Resources["detailTablet"] as Style
                };
                InfoGrid.Children.Add(l, 0, i);

                string num = Attendance.GetNumber(students[i], Grade);

                InfoGrid.Children.Add(new Label()
                {
                    Text = num.Length == 0 ? "" : "(" + num.Substring(0, 3) + ")-" + num.Substring(3, 3) + "-" + num.Substring(6),
                    Style = Resources["detailTablet"] as Style
                }, 1, i);
                InfoGrid.Children.Add(new Label()
                {
                    Text = Attendance.GetDatesForYear(students[i], Grade),
                    Style = Resources["detailTablet"] as Style
                }, 2, i);
                ImageButton b2 = new ImageButton()
                {
                    Source = "editsmall.png",
                    CommandParameter = l,
                    Style = Resources["buttonTablet"] as Style
                };
                Button b = new Button()
                {
                    Text = ">",
                    BackgroundColor = Color.FromHex("#EEEEEE"),
                    CommandParameter = l,
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
            Label l = b.CommandParameter as Label;
            Navigation.PushAsync(new EditAddStudent(Attendance, l.Text, Grade, false));
        }

        protected override void OnAppearing()
        {
            InitGrid();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditAddStudent(Attendance, "", Grade, true));
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