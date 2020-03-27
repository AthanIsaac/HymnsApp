using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StudentInfo : ContentPage
    {
        readonly HymnsAttendance Attendance;
        readonly string ClassName;
        private IList<KeyValuePair<string, string>> students;
        private IList<KeyValuePair<string, string>> teachers; 

        public StudentInfo(HymnsAttendance attendance, string className)
        {
            Attendance = attendance;
            ClassName = className;
            students = Attendance.StudentsOfGrade(ClassName);
            teachers = Attendance.TeachersOfGrade(ClassName);
            InitializeComponent();
            // InitGrid();
        }
        private void InitGrid()
        {
            /* photo? name | phone | attendance days | editbutton */
            
            int total = (students.Count + teachers.Count);
            while ( total > InfoGrid.RowDefinitions.Count)
            {
                InfoGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(60)
                    
                }) ; 
            }

          

            for (int i = 0; i < students.Count; i++)
            {

                CircleImage profilePicture = new CircleImage
                {
                    Source = ImageSource.FromStream(() =>
                    {
                        var stream = Attendance.GetStudentPhoto(students[i].Key); 
                        return stream;
                    })
                };
                if (profilePicture == null)
                {
                    profilePicture = new CircleImage { Source = "blankprofile.png", HeightRequest = 500, WidthRequest = 500 };
                }

                profilePicture.HorizontalOptions = LayoutOptions.Center;
                profilePicture.VerticalOptions = LayoutOptions.Center;

                InfoGrid.Children.Add(profilePicture, 0, i);

                InfoGrid.Children.Add(new Label()
                {
                    
                    Text = students[i].Value.Trim(),
                    TextColor = Color.Black,
                     HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 15,
                    //Style = Resources["detailTablet"] as Style
                }, 1, i); ;

                InfoGrid.Children.Add(new Label()
                {
                    Text = Attendance.GetDatesForYear(students[i].Key).ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Color.Black,
                    FontSize = 15
                    ,
                    //Style = Resources["detailTablet"] as Style
                }, 2, i); 

               
                Button b = new Button()
                {
                    Text = ">",
                    BackgroundColor = Color.White,
                    FontSize = 15,
                    WidthRequest = 15,
                    CommandParameter = new Label()
                    {
                        Text = students[i].Key,
                        IsVisible = false
                    },
                     HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };

                b.Clicked += Edit_Clicked;

                InfoGrid.Children.Add(b, 3, i);
            }

            int index = 0;
            for (int i = students.Count; i < total; i++)
            {

                CircleImage profilePicture = new CircleImage
                {
                    Source = ImageSource.FromStream(() =>
                    {
                        var stream = Attendance.GetTeacherPhoto(students[index].Key); 
                        return stream;
                    })
                };
                if (profilePicture == null)
                {
                    profilePicture = new CircleImage { Source = "blankprofile.png", HeightRequest = 500, WidthRequest = 500 };
                }

                profilePicture.HorizontalOptions = LayoutOptions.Center;
                profilePicture.VerticalOptions = LayoutOptions.Center;

                InfoGrid.Children.Add(profilePicture, 0, i);

                InfoGrid.Children.Add(new Label()
                {

                    Text = teachers[index].Value.Trim(),
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 15,
                    //Style = Resources["detailTablet"] as Style
                }, 1, i);   


                InfoGrid.Children.Add(new Label()
                {
                    //Text = Attendance.GetDatesForYear(teachers[i].Key).ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 15,

                    //Style = Resources["detailTablet"] as Style
                }, 2, i);


                Button b = new Button()
                {
                    Text = ">",
                    BackgroundColor = Color.White,
                    FontSize = 15,
                    WidthRequest = 15,
                    CommandParameter = new Label()
                    {
                        Text = students[total - i].Key,
                        IsVisible = false
                    },
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };

                b.Clicked += Edit_Clicked;

                InfoGrid.Children.Add(b, 3, i);
                index++;
            }
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            Button b = sender as Button;
            Label sl = b.CommandParameter as Label;
            string id = sl.Text; 
            
            Navigation.PushAsync(new StudentProfilexaml(Attendance, sl.Text, ClassName));
            
        }

        protected override void OnAppearing()
        {
            InitGrid();
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            //bug: HymnsAttendance attendance, string id, string name, string className, bool add
            Navigation.PushAsync(new EditAddStudent(Attendance, "", "",  ClassName, true));
        }
    }
}