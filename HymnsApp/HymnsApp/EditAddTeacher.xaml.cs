using Plugin.Media;
using Plugin.Media.Abstractions;
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
    public partial class EditAddTeacher : ContentPage
    {
        readonly HymnsAttendance Attendance;
        readonly bool Add;
        readonly string ClassName;
        readonly string id;
        public EditAddTeacher(HymnsAttendance attendance, string id, string name, string className, bool add)
        {
            ToolbarItem item = new ToolbarItem();
            if (!add)
            {
                item.Text = "Edit Teacher";

            }
            else
            {
                item.Text = "Add Teacher";


            }
            // "this" refers to a Page object
            this.ToolbarItems.Add(item);
            InitializeComponent();
            name = name == null ? "" : name.ToLower();
            Add = add;
            ClassName = className;
            Attendance = attendance;
            NameEntry.Text = name;
            this.id = id;
            if (!add)
            {
                string[] info = Attendance.GetTeacherInfo(id);
                //name, phone, grade, parentName, parentPhone, birthday, photo, later 
                string num = info[1];
                TeacherPhoneEntry.Text = num.Length == 0 ? "" : "(" + num.Substring(0, 3) + ")-" + num.Substring(3, 3) + "-" + num.Substring(6);               
                //MM/dd
                BirthdayEntry.Text = info[2];



            }


        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string name = Capitalize(NameEntry.Text.Trim());
            if (!Add)
            {
                String[] birthday = BirthdayEntry.Text.Split('/');
                //string studentId, string newClassName, string newStudentName, string newStudentPhone, 
                // string newGrade, string newParentName, string newParentPhone, DateTime newBirthday
                Attendance.EditTeacher(id, ClassName, name, TeacherPhoneEntry.Text, new DateTime(2020, Int32.Parse(birthday[0]), Int32.Parse(birthday[1])));

                Navigation.PopAsync();
                return;
            }
            // submit
            /*
            if (Attendance.TeacherOfGrade(ClassName).Select(a => a.Value).Contains(name))
            {
                DisplayAlert("Error", "This student already exists in this grade", "ok");
            }*/

            else
            {
                String[] birthday = BirthdayEntry.Text.Split('/');
                // string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday /*photo*/);
                Attendance.AddTeacher(name, TeacherPhoneEntry.Text, new DateTime(2020, Int32.Parse(birthday[0]), Int32.Parse(birthday[1])));

                Navigation.PopAsync();
            }

        }

        private async void PictureButton_OnClicked(object sender, EventArgs e)
        {

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No Camera", "No Camera Availible", "OK");
                return;
            }
            var a = new StoreCameraMediaOptions() { };
            var photo = await CrossMedia.Current.TakePhotoAsync(a);

            if (photo != null)
                Picture.Source = ImageSource.FromStream(() => { return photo.GetStream(); });

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { });

            if (file == null)
                return;

            Label path = new Label { Text = file.AlbumPath };

            Picture.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });

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

        private void BirthdayEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BirthdayEntry.Text.Length == 2)
            {
                BirthdayEntry.Text += "/";
            }
        }
    }
}
