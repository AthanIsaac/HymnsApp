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
        readonly HymnsAttendance Attendance;
        readonly string Name;
        readonly bool Add;
        readonly int Grade;
        public EditAddStudent(HymnsAttendance attendance, string name, int grade, bool add)
        {
            InitializeComponent();
            Name = name == null ? "" : name.ToLower();
            Add = add;
            Grade = grade;
            if (!add)
            {
                PhoneEntry.Text = attendance.GetNumber(Name, grade);
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
                Attendance.EditStudent(Name, Grade, name, PhoneEntry.Text, int.Parse(GradeEntry.Text));
                //Attendance.WriteStudents();
                Navigation.PopAsync();
                return;
            }
            // submit
            if (Attendance.StudentsOfGrade(int.Parse(GradeEntry.Text)).Contains(name))
            {
                DisplayAlert("Error", "This student already exists in this grade", "ok");
            }
            else
            {
                Attendance.AddSudent(name.ToLower().Trim(), PhoneEntry.Text, int.Parse(GradeEntry.Text));
                //Attendance.WriteStudents();
                Navigation.PopAsync();
            }

        }
    }
}