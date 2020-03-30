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
                item.Text = name;

            }
            else
            {
                item.Text = "Add Teacher";


            }
            // "this" refers to a Page object
            this.ToolbarItems.Add(item);
            InitializeComponent();
            name = string.IsNullOrEmpty(name) ? "" : Capitalize(name);
            Add = add;
            ClassName = className;
            Attendance = attendance;
            NameEntry.Text = name;
            this.id = id;

            Classes.ItemsSource = ClassesToInterface(HymnsAttendance.OrderedClasses);

            if (!add)
            {
                string[] info = Attendance.GetTeacherInfo(id);
                //name, phone, grade, parentName, parentPhone, birthday, photo, later 
                string num = info[1];
                string parsed = num.Length == 0 ? "" : "(" + num.Substring(0, 3) + ")-" + num.Substring(3, 3) + "-" + num.Substring(6);
                TeacherPhoneEntry.Text = info[1];
                //MM/dd
                int slash = info[2].IndexOf("/");

                BirthdayMonth.Text = info[2].Substring(0,slash);
                BirthdayDay.Text = info[2].Substring(slash + 1);

                Classes.SelectedItem = parseName(className);

            }


        }

        private string parseName(string c)
        {
            if (c.Contains("kindergarten"))
            {
                c = "Kindergarten";
                return c.Trim();
            }

            if (c.Contains("highSchool"))
            {
                c = "HighSchool";
                return c.Trim();
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
                return c.Trim();
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
                return c.Trim();
            }

        }

        public string[] ClassesToInterface(string[] dbClasses)
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
                visualClasses[i] = c.Trim();
            }

            return visualClasses;
        }

        public async Task<bool> CheckInputsAsync(string name)
        {
            if (Attendance.StudentsOfGrade(ClassName).Select(b => b.Value).Contains(name))
            {
                await DisplayAlert("Error", "1This student already exists in this grade", "ok");
                return false;
            }

            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                await DisplayAlert("Error", "2Student Name is a Required Field", "ok");
                return false;
            }

            if (!NameEntry.Text.Trim().Contains(" "))
            {
                await DisplayAlert("Error", "3First and Last Name are Required. ", "ok");
                return false;
            }

            if (!string.IsNullOrEmpty(TeacherPhoneEntry.Text) && TeacherPhoneEntry.Text.Length != 10)
            {
                await DisplayAlert("Error", "4Invalid Phone Number.", "ok");
                return false;
            }

            if (!(int.TryParse(TeacherPhoneEntry.Text, out int a)))
            {
                await DisplayAlert("Error", "5Invalid Phone Number.", "ok");
                return false;
            }

            if (string.IsNullOrEmpty(BirthdayMonth.Text))
            {
                await DisplayAlert("Error", "10Student Birthday is a Required Field", "ok");
                return false;
            }
            if (string.IsNullOrEmpty(BirthdayDay.Text))
            {
                await DisplayAlert("Error", "10Student Birthday is a Required Field", "ok");
                return false;
            }

            //if (BirthdayMonth.Text.Length != 5 || !int.TryParse(BirthdayEntry.Text.Substring(0, 2), out int a) || BirthdayEntry.Text[2] != '/' || !int.TryParse(BirthdayEntry.Text.Substring(3), out a))
            //{
            //    await DisplayAlert("Error", "11Invalid Student Birthday.", "ok");
            //    return false;
            //}

            if (BirthdayMonth.Text.Length > 2 || BirthdayMonth.Text.Length < 1)
            {
                await DisplayAlert("Error", "11Invalid Student Birthday.", "ok");
                return false;
            }

            return true;

        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (await CheckInputsAsync(NameEntry.Text))
            {
                string name = Capitalize(NameEntry.Text.Trim());
                if (!Add)
                {
                    Classes.SelectedItem = parseName(ClassName);
                    string classes = "";
                    if (Classes.SelectedItem == null || !((Classes.SelectedItem.ToString()).Equals(classes)))
                    {
                        classes = ClassName;
                    }
                    else
                    {
                        classes = Classes.SelectedItem.ToString();
                    }

                    Attendance.EditTeacher(id, classes, name, TeacherPhoneEntry.Text, new DateTime(2020, Int32.Parse(BirthdayMonth.Text), Int32.Parse(BirthdayDay.Text)));
                    await Navigation.PopAsync();
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
                    Attendance.AddTeacher(name, TeacherPhoneEntry.Text, new DateTime(2020, Int32.Parse(BirthdayMonth.Text), Int32.Parse(BirthdayDay.Text)));

                    await Navigation.PopAsync();
                }
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

    }
}
