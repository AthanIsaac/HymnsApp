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
    public partial class EditAddStudent : ContentPage
    {
        readonly HymnsAttendance2 Attendance;
        readonly string Name;
        readonly bool Add;
        readonly string Grade;
        readonly string id;
        public EditAddStudent(HymnsAttendance2 attendance, string id, string name, string grade, bool add)
        {
            InitializeComponent();
            Name = name == null ? "" : name.ToLower();
            Add = add;
            Grade = grade;
            this.id = id;
            if (!add)
            {
                string[] info = Attendance.GetStudent(id);
                //name, phone, grade, parentName, parentPhone, birthday, photo, later 
                PhoneEntry.Text = info[1];
            }
            NameEntry.Text = name;
            GradeEntry.Text = grade.ToString();
            Attendance = attendance;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string name = NameEntry.Text.Trim().ToLower();
            if (!Add)
            {
                // oldname, oldgrade, newname, newphone, newgrade
                //string studentId, string newClassName, string newStudentName, string newStudentPhone, 
                // string newGrade, string newParentName, string newParentPhone, DateTime newBirthday
                Attendance.EditStudent(id, "", name, PhoneEntry.Text, GradeEntry.Text, "", "", DateTime.Now);
                //Attendance.WriteStudents();
                Navigation.PopAsync();
                return;
            }
            // submit
            if (Attendance.StudentsOfGrade(Grade).Select(a => a.Value).Contains(name))
            {
                DisplayAlert("Error", "This student already exists in this grade", "ok");
            }
            else
            {
                // string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday /*photo*/);
                Attendance.AddStudent(name.Trim(), PhoneEntry.Text, GradeEntry.Text, "", "", DateTime.Now);
                
                Navigation.PopAsync();
            }

        }
    }
}