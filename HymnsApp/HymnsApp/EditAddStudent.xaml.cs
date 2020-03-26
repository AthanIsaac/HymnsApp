using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Plugin.Media;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAddStudent : ContentPage
    {
        readonly HymnsAttendance Attendance;
        readonly bool Add;
        readonly string ClassName;
        readonly string id;
        public EditAddStudent(HymnsAttendance attendance, string id, string name, string className, bool add)
        {
            ToolbarItem item = new ToolbarItem();
            if (!add)
            {
                item.Text = name;

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
                string[] info = Attendance.GetStudentInfo(id);
                //name, phone, grade, parentName, parentPhone, birthday, photo, later 
                StdPhoneEntry.Text = info[1];
                GradeEntry.Text = info[2];
                
                ParentNameEntry.Text = info[3];
                string num = info[4];
                ParentPhoneEntry.Text = num.Length == 0 ? "" : "(" + num.Substring(0, 3) + ")-" + num.Substring(3, 3) + "-" + num.Substring(6); 

                //MM/dd
                BirthdayEntry.Text = info[5];
                Classes.ItemsSource = classesToInterface(HymnsAttendance.OrderedClasses);

            }


        }

        public string[] classesToInterface(string[] dbClasses)
        {
            string[] visualClasses = new string[dbClasses.Length];

            if (dbClasses == null)
            {
                return null;
            }


            for (int i = 0; i < dbClasses.Length; i++)
            {
                string c = dbClasses[i];

                if (c.Contains("kindergarten"))
                {
                    c = "Kindergarten";

                }

                if (c.Contains("highSchool"))
                    c = "HighSchool";

                if (c.Contains("Grade"))
                {
                    int index = c.IndexOf("Grade");
                    c = c.Substring(0, index) + " " + c.Substring(index);

                    if (c.Contains("&"))
                    {
                        int ampersand = c.IndexOf("&");
                        c = c.Substring(0, ampersand) + " & " + c.Substring(ampersand + 1);
                    }

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
                }
                visualClasses[i] = c;
            }


            return visualClasses;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            
            string name = Capitalize(NameEntry.Text.Trim());
            CheckInputs(name);
            String[] birthday = BirthdayEntry.Text.Split('/');
            if (!Add)
            {
                string classes = Classes.SelectedItem.ToString();
                
                //string studentId, string newClassName, string newStudentName, string newStudentPhone, 
                // string newGrade, string newParentName, string newParentPhone, DateTime newBirthday
                Attendance.EditStudent(id, classes, name, StdPhoneEntry.Text, GradeEntry.Text, ParentNameEntry.Text, ParentPhoneEntry.Text, new DateTime(2020, Int32.Parse(birthday[0]), Int32.Parse(birthday[1])));

                Navigation.PopAsync();
                return;
            }
            // submit
                // string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday /*photo*/);
                Attendance.AddStudent(name, StdPhoneEntry.Text, GradeEntry.Text, ParentNameEntry.Text, ParentPhoneEntry.Text, new DateTime(2020, Int32.Parse(birthday[0]), Int32.Parse(birthday[1])));

                Navigation.PopAsync();
            

        }

        public void CheckInputs(string name) {
            if (Attendance.StudentsOfGrade(ClassName).Select(a => a.Value).Contains(name))
            {
                DisplayAlert("Error", "This student already exists in this grade", "ok");
            }
            if (string.IsNullOrEmpty(NameEntry.Text.ToString()))
            {

                DisplayAlert("Error", "Student Name is a Required Field", "ok");
            }
            if (string.IsNullOrEmpty(ParentNameEntry.Text.ToString()))
            {

                DisplayAlert("Error", "Parent Name is a Required Field", "ok");
            }
            if (string.IsNullOrEmpty(ParentPhoneEntry.Text.ToString()))
            {

                DisplayAlert("Error", "Parent Phone is a Required Field", "ok");
            }
            if (string.IsNullOrEmpty(BirthdayEntry.Text.ToString()))
            {

                DisplayAlert("Error", "Student Birthday is a Required Field", "ok");
            }


        }

        private async void PictureButton_OnClicked(object sender, EventArgs e) {

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported) {
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
            if (string.IsNullOrEmpty(name))
                return null;
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