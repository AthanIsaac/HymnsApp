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
        readonly HymnsAttendance2 Attendance;
        readonly string Id;
        readonly string ClassName;

        public StudentProfilexaml(HymnsAttendance2 attendance, string id, string className)
        {
            Attendance = attendance;
            Id = id;
            ClassName = className;
            string[] studentInfo = Attendance.GetStudent(Id);
            // studentname, studentphone, grade, parentname, parentphone, birthday

            InitializeComponent();

            Image profilePicture = new Image { Source = "editsmall.png", HeightRequest = 200 };
            storeInfo.Children.Add(profilePicture);
            Label lname = new Label {Text ="Name"};
            var ename = new Entry { Text = studentInfo[0], IsReadOnly = true };
            storeInfo.Children.Add(lname);
            storeInfo.Children.Add(ename);

            Label lstudentPhone = new Label { Text = "Student Phone" };
            var estudentPhone = new Entry { Text = studentInfo[1], IsReadOnly = true };
            storeInfo.Children.Add(lstudentPhone);
            storeInfo.Children.Add(estudentPhone);

            Label lgrade= new Label { Text = "Grade" };
            var egrade = new Entry { Text = studentInfo[2], IsReadOnly = true };
            storeInfo.Children.Add(lgrade);
            storeInfo.Children.Add(egrade);

            Label lparentName = new Label { Text = "Parent Name" };
            var eParentName = new Entry { Text = studentInfo[3], IsReadOnly = true };
            storeInfo.Children.Add(lparentName);
            storeInfo.Children.Add(eParentName);

            Label lParentPhone = new Label { Text = "Student Phone" };
            var eparentPhone = new Entry { Text = studentInfo[4], IsReadOnly = true };
            storeInfo.Children.Add(lParentPhone);
            storeInfo.Children.Add(eparentPhone);

            Label lbirthday = new Label { Text = "birthday" };
            //for the edit
            var ebirthday = new Entry { Text = studentInfo[5], IsReadOnly = true};
            storeInfo.Children.Add(lbirthday);
            storeInfo.Children.Add(ebirthday);

        }
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string[] studentInfo = Attendance.GetStudent(Id);
            //EditAddStudent(HymnsAttendance2 attendance, string id, string name, string grade, bool add)
            Navigation.PushAsync(new EditAddStudent(Attendance, Id, studentInfo[0], ClassName, false));
        }
    }
}