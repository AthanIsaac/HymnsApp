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
    public partial class TeacherProfile : ContentPage
    {
        readonly HymnsAttendance Attendance;
        readonly string Id;
        readonly string ClassName;

        public TeacherProfile(HymnsAttendance attendance, string id, string className)
        {
            Attendance = attendance;
            Id = id;
            ClassName = className;
            string[] teacherInfo = Attendance.GetTeacherInfo(Id);
            // name, phone, birthday
            ToolbarItem item = new ToolbarItem();
            item.Text = teacherInfo[0];

            InitializeComponent();


            var stream = Attendance.GetTeacherPhoto(id);
            Image profilePicture = new Image
            {

                Source = ImageSource.FromStream(() =>
                {

                    return stream;
                })
            };

            if (stream == null)
            {
                profilePicture = new Image { Source = "blankprofile.png", HeightRequest = 500, WidthRequest = 500 };
            }

            storeInfo.Children.Add(profilePicture);
            Label lname = new Label { Text = "Teacher's Name:", TextColor = Color.SteelBlue, FontSize = 23 };
            var ename = new Entry { Text = teacherInfo[0], IsReadOnly = true };
            storeInfo.Children.Add(lname);
            storeInfo.Children.Add(ename);

            Label lPhone = new Label { Text = "Teacher's Phone Number:", TextColor = Color.SteelBlue, FontSize = 23 };
            string num = teacherInfo[1];
            string parsed = num.Length == 0 ? "" : "(" + num.Substring(0, 3) + ")-" + num.Substring(3, 3) + "-" + num.Substring(6);
            var ePhone = new Entry { Text = parsed, IsReadOnly = true };
            storeInfo.Children.Add(lPhone);
            storeInfo.Children.Add(ePhone);

            StackLayout slbirthday = new StackLayout() {Orientation = StackOrientation.Horizontal, Spacing = 10 };
            int slash = teacherInfo[2].IndexOf("/");

            Label lbirthday = new Label { Text = "Teacher's Birthday:", TextColor = Color.SteelBlue, FontSize = 23 };
            var ebirthdayMonth = new Entry { Text = teacherInfo[2].Substring(0, slash), IsReadOnly = true };
            Label slashText = new Label() { Text = "/" };
            var ebirthdayDay = new Entry { Text = teacherInfo[2].Substring(slash + 1), IsReadOnly = true };

            slbirthday.Children.Add(ebirthdayMonth);
            slbirthday.Children.Add(slashText);
            slbirthday.Children.Add(ebirthdayDay);

            storeInfo.Children.Add(lbirthday);
            storeInfo.Children.Add(slbirthday);
            

            Label lclassName = new Label { Text = "Teacher's Class:", TextColor = Color.SteelBlue, FontSize = 23 };
            //for the edit
            var eclassName = new Entry { Text = parseName(ClassName), IsReadOnly = true };
            storeInfo.Children.Add(lclassName);
            storeInfo.Children.Add(eclassName);
        }

        private string parseName(string c) 
        {
            if (c.Contains("kindergarten"))
            {
                c = "Kindergarten";
                return c;
            }

            if (c.Contains("highSchool"))
            {
                c = "HighSchool";
                return c;
            }
            if (c.Contains("Grade"))
            {
                int index = c.IndexOf("Grade");
                c = c.Substring(0, index) + " " + c.Substring(index);

                if (c.Contains("&"))
                {
                    int ampersand = c.IndexOf("&");
                    c = c.Substring(0, ampersand) + " & " + c.Substring(ampersand + 1);
                }
                return c;
            }

            else
            {
                //really inefficent, find better way
                for (int j = 0; j < c.Length; j++)
                {

                    if (char.IsUpper(c[j]))
                    {
                        c = c.Substring(0, j) + " " + c.Substring(j);
                        j++;
                    }


                }

                c = c.Replace("m", "M");
                return c;
            }
        
    }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string[] teacherInfo = Attendance.GetTeacherInfo(Id);
            //EditAddStudent(HymnsAttendance2 attendance, string id, string name, string grade, bool add)
            Navigation.PushAsync(new EditAddTeacher(Attendance, Id, teacherInfo[0], ClassName, false));
        }
    }
}