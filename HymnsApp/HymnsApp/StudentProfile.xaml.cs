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


    public partial class StudentProfilexaml : ContentPage
    {
        readonly HymnsAttendance Attendance;
        readonly string Id;
        readonly string ClassName;

        public StudentProfilexaml(HymnsAttendance attendance, string id, string className)
        {
            Attendance = attendance;
            Id = id;
            ClassName = className;
            string[] studentInfo = Attendance.GetStudentInfo(Id);
            // studentname, studentphone, grade, parentname, parentphone, birthday
            ToolbarItem item = new ToolbarItem();
            item.Text = studentInfo[0];

            InitializeComponent();

            

            Image profilePicture = new Image
            {
                Source = ImageSource.FromStream(() =>
                {
                    var stream = Attendance.GetStudentPhoto(id);
                    return stream;
                })
            };

            if (profilePicture.Source == null)
            {
                profilePicture = new Image { Source = "blankprofile.png", HeightRequest = 500, WidthRequest = 500 };
            }

            storeInfo.Children.Add(profilePicture);
            Label lname = new Label { Text = "Student's Name:", TextColor = Color.SteelBlue, FontSize = 23 };
            var ename = new Entry { Text = studentInfo[0], IsReadOnly = true };
            storeInfo.Children.Add(lname);
            storeInfo.Children.Add(ename);

            Label lstudentPhone = new Label { Text = "Student's Phone Number:", TextColor = Color.SteelBlue, FontSize = 23 };
            var estudentPhone = new Entry { Text = studentInfo[1], IsReadOnly = true };
            storeInfo.Children.Add(lstudentPhone);
            storeInfo.Children.Add(estudentPhone);

            Label lgrade = new Label { Text = "Grade:", TextColor = Color.SteelBlue, FontSize = 23 };
            var egrade = new Entry { Text = studentInfo[2], IsReadOnly = true };
            storeInfo.Children.Add(lgrade);
            storeInfo.Children.Add(egrade);

            Label lparentName = new Label { Text = "Parent Name:", TextColor = Color.SteelBlue, FontSize = 23 };
            var eParentName = new Entry { Text = studentInfo[3], IsReadOnly = true };
            storeInfo.Children.Add(lparentName);
            storeInfo.Children.Add(eParentName);

            Label lParentPhone = new Label { Text = "Parent's Phone Number:", TextColor = Color.SteelBlue, FontSize = 23 };
            var eparentPhone = new Entry { Text = studentInfo[4], IsReadOnly = true };
            storeInfo.Children.Add(lParentPhone);
            storeInfo.Children.Add(eparentPhone);

            Label lbirthday = new Label { Text = "Student's Birthday Number:", TextColor = Color.SteelBlue, FontSize = 23 };
            //for the edit
            var ebirthday = new Entry { Text = studentInfo[5], IsReadOnly = true };
            storeInfo.Children.Add(lbirthday);
            storeInfo.Children.Add(ebirthday);

        }
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string[] studentInfo = Attendance.GetStudentInfo(Id);
            //EditAddStudent(HymnsAttendance2 attendance, string id, string name, string grade, bool add)
            Navigation.PushAsync(new EditAddStudent(Attendance, Id, studentInfo[0], ClassName, false));
        }
       
    }
}