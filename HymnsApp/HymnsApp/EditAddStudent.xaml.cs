using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Plugin.Media;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using System.IO;

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

            Picture.Source = ImageSource.FromStream(() =>
            {
                var stream = Attendance.GetStudentPhoto(id);
                return stream;
            });
           
            if (add) 
            { 
                SwitchToTeacher.IsVisible = true; 
            }

            //add and edit cases
            name = name == null ? "" : Capitalize(name);
            Add = add;
            ClassName = className;
            Attendance = attendance;
            NameEntry.Text = name;
            this.id = id;

            Classes.ItemsSource = ClassesToInterface(HymnsAttendance.OrderedClasses);

            if (!add)
            {
                string[] info = Attendance.GetStudentInfo(id);
                //name, phone, grade, parentName, parentPhone, birthday, photo, later 
                string num = info[1];
                string parsed = num.Length == 0 ? "" : "(" + num.Substring(0, 3)
                    + ")-" + num.Substring(3, 3) + "-" + num.Substring(6);
                StdPhoneEntry.Text = info[1];
                GradeEntry.Text = info[2];
                
                ParentNameEntry.Text = info[3];
                num = info[4];
                parsed = num.Length == 0 ? "" : "(" + num.Substring(0, 3)
                    + ")-" + num.Substring(3, 3) + "-" + num.Substring(6);
                ParentPhoneEntry.Text = info[4];

                //MM/dd
                int slash = info[5].IndexOf("/");

                BirthdayMonth.Text = info[5].Substring(0, slash);
                BirthdayDay.Text = info[5].Substring(slash + 1);

                Classes.SelectedItem = parseName(ClassName);
            }
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
                visualClasses[i] = c;
            }

            return visualClasses;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (await CheckInputsAsync(NameEntry.Text))
            {

                string name = Capitalize(NameEntry.Text.Trim());

                //String[] birthday = BirthdayEntry.Text.Split('/');
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

                    var teachers = Attendance.TeachersOfGrade(classes).ToArray();
                    foreach (var teacher in teachers)
                    {
                        if ((teacher.Value).Contains(name))
                        {
                            Attendance.EditTeacher(id, ClassName, name, StdPhoneEntry.Text,
                                new DateTime(2020, Int32.Parse(BirthdayMonth.Text), Int32.Parse(BirthdayDay.Text)));
                            await Navigation.PopAsync();
                            return;
                        }
                    }

                    //string studentId, string newClassName, string newStudentName, string newStudentPhone, 
                    // string newGrade, string newParentName, string newParentPhone, DateTime newBirthday
                    Attendance.EditStudent(id, classes, name, StdPhoneEntry.Text, GradeEntry.Text,
                        ParentNameEntry.Text, ParentPhoneEntry.Text, new DateTime(2020, Int32.Parse(BirthdayMonth.Text), Int32.Parse(BirthdayDay.Text)));
                    await Navigation.PopAsync();
                    return;
                }
                // submit
                // string studentName, string studentPhone, string grade, string parentName, string parentPhone, DateTime birthday /*photo*/);
                Attendance.AddStudent(name, StdPhoneEntry.Text, GradeEntry.Text,
                    ParentNameEntry.Text, ParentPhoneEntry.Text, new DateTime(2020, Int32.Parse(BirthdayMonth.Text), Int32.Parse(BirthdayDay.Text)));

                await Navigation.PopAsync();
            }

        }

        public async Task<bool> CheckInputsAsync(string name) {
            if (Attendance.StudentsOfGrade(ClassName).Select(b => b.Value).Contains(name) && Add)
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

            if (!string.IsNullOrEmpty(StdPhoneEntry.Text) && StdPhoneEntry.Text.Length != 10)
            {
                await DisplayAlert("Error", "4Invalid Phone Number.", "ok");
                return false;
            }

            //if (!(int.TryParse(StdPhoneEntry.Text, out int a)))
            //{
            //    await DisplayAlert("Error", "5Invalid Phone Number.", "ok");
            //    return false;
            //}

            if (string.IsNullOrEmpty(ParentNameEntry.Text))
            {
                await DisplayAlert("Error", "6Parent Name is a Required Field", "ok");
                return false;
            }

            if (!ParentNameEntry.Text.Trim().Contains(" "))
            {
                await DisplayAlert("Error", "7First and Last Name are Required. ", "ok");
                return false;
            }

            //if (!string.IsNullOrEmpty(ParentPhoneEntry.Text) && ParentPhoneEntry.Text.Length != 10)
            //{
            //    await DisplayAlert("Error", "8Invalid Phone Number.", "ok");
            //    return false;
            //}

            //if (!(int.TryParse(ParentPhoneEntry.Text, out int a)))
            //{
            //    await DisplayAlert("Error", "9Invalid Phone Number.", "ok");
            //    return false;
            //}

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

       

            if (BirthdayMonth.Text.Length > 2 || BirthdayMonth.Text.Length < 1)
            {
                await DisplayAlert("Error", "11Invalid Student Birthday.", "ok");
                return false;
            }

            if (!int.TryParse(GradeEntry.Text, out int a) || a < 0 || a > 12)
            {
                await DisplayAlert("Error", "12Invalid Grade.", "ok");
                return false;
            }

            return true;

        }

        private async void PictureButton_OnClicked(object sender, EventArgs e) {

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported) {
                await DisplayAlert("No Camera", "No Camera Availible", "OK");
                    return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() {AllowCropping = true, CompressionQuality = 5, CustomPhotoSize = 90 });


            if (file == null)
                return;


            var picture = file.GetStream();

            
       
           //case:Edit
            Attendance.AddStudentPhoto(id, picture);


            //Label path = new Label { Text = file.AlbumPath };
           
            Picture.Source = ImageSource.FromStream(() =>
            {
                var stream = picture;
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

        private void SwitchToTeacherButton_OnClicked(object sender, EventArgs e) {
            //HymnsAttendance attendance, string id, string name, string className, bool add
            Navigation.PushAsync(new EditAddTeacher(Attendance, id, "", ClassName, true));

        }

      
    }
}