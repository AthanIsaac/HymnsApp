using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Plugin.Media;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAddStudent : ContentPage
    {
        readonly HymnsAttendance2 Attendance;
        readonly bool Add;
        readonly string ClassName;
        readonly string id;
        public EditAddStudent(HymnsAttendance2 attendance, string id, string name, string className, bool add)
        {
            ToolbarItem item = new ToolbarItem();
            if (!add)
            {
                item.Text = "Edit Student";

            }
            else
            {
                item.Text = "Add Student";


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
                string[] info = Attendance.GetStudent(id);
                //name, phone, grade, parentName, parentPhone, birthday, photo, later 
                StdPhoneEntry.Text = info[1];
                GradeEntry.Text = info[2];
                ParentNameEntry.Text = info[3];
                ParentPhoneEntry.Text = info[4];
                //MM/dd
                BirthdayEntry.Text = info[5];
                //DateTime dt = new DateTime();
                //bool worked = DateTime.TryParse(info[5], out dt);
                //if (worked)
                //{
                //    BirthdayEntry.Date = dt;
                //}

                //photo
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
                Attendance.EditStudent(id, ClassName, name, StdPhoneEntry.Text, GradeEntry.Text, ParentNameEntry.Text, ParentPhoneEntry.Text, new DateTime(2020, Int32.Parse(birthday[0]), Int32.Parse(birthday[1])));

                Navigation.PopAsync();
                return;
            }
            // submit
            if (Attendance.StudentsOfGrade(ClassName).Select(a => a.Value).Contains(name))
            {
                DisplayAlert("Error", "This student already exists in this grade", "ok");
            }
            else
            {
                String[] birthday = BirthdayEntry.Text.Split('/');
                // string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday /*photo*/);
                Attendance.AddStudent(name, StdPhoneEntry.Text, GradeEntry.Text, ParentNameEntry.Text, ParentPhoneEntry.Text, new DateTime(2020, Int32.Parse(birthday[0]), Int32.Parse(birthday[1])));
                
                Navigation.PopAsync();
            }

        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
           
        }

        private async void PictureButton_OnClicked(object sender, EventArgs e) {

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported) {
                await DisplayAlert("No Camera", "No Camera Availible", "OK");
                    return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        SaveToAlbum = true
                    }
                );

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
    }
}