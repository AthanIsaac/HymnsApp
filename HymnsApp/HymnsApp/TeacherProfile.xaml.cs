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
            var ePhone = new Entry { Text = teacherInfo[1], IsReadOnly = true };
            storeInfo.Children.Add(lPhone);
            storeInfo.Children.Add(ePhone);

            Label lbirthday = new Label { Text = "Teacher's Birthday Number:", TextColor = Color.SteelBlue, FontSize = 23 };
            //for the edit
            var ebirthday = new Entry { Text = teacherInfo[2], IsReadOnly = true };
            storeInfo.Children.Add(lbirthday);
            storeInfo.Children.Add(ebirthday);
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string[] teacherInfo = Attendance.GetTeacherInfo(Id);
            //EditAddStudent(HymnsAttendance2 attendance, string id, string name, string grade, bool add)
            Navigation.PushAsync(new EditAddTeacher(Attendance, Id, teacherInfo[0], ClassName, false));
        }
    }
}