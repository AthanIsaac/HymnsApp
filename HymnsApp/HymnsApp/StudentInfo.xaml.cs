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
                InfoGrid.Children.Add(new Image 
                { Source = "blankProfile.png",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                }, 0, i);

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
               
                InfoGrid.Children.Add(new Image { Source = "blankProfile.png", HeightRequest = 10 }, 0, i);

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
            
            //if teacher
            //if(teachers.Contains(id, info[0]))

            //if student
            Navigation.PushAsync(new StudentProfilexaml(Attendance, sl.Text, ClassName));
            /*
            Button b = sender as Button;
            Label sl = b.CommandParameter as Label;
            
            string id = sl.Text;
            

            string [] studentInfo = Attendance.GetStudent(id);
            //EditAddStudent(HymnsAttendance2 attendance, string id, string name, string grade, bool add)
            Navigation.PushAsync(new EditAddStudent(Attendance, id, studentInfo[0], ClassName, false)); */
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