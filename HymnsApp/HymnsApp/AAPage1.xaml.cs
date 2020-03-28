using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AAPage1 : ContentPage
    {
        readonly HymnsAttendance Attendance;
        readonly string ClassName;
        public AAPage1(HymnsAttendance attendance, string className)
        {
            InitializeComponent();
            Attendance = attendance;
            ClassName = className;
            InitGrid();
        }

        private void InitGrid()
        {
            var students = Attendance.StudentsOfGrade(ClassName);

            for (int i = 0; i < students.Count; i++)
            {
                var stream = Attendance.GetStudentPhoto(students[i].Key);

                CircleImage profilePicture = new CircleImage
                {
                    Source = ImageSource.FromStream(() =>
                    {
                        return stream;
                    }),
                    HeightRequest = 200,
                    WidthRequest = 200
                };

                if (stream == null)
                {
                    profilePicture = new CircleImage { Source = "blankprofile.png", HeightRequest = 200, WidthRequest = 200 };
                }

                profilePicture.HorizontalOptions = LayoutOptions.Center;
                profilePicture.VerticalOptions = LayoutOptions.Center;

                Label nameLabel = new Label()
                {
                    Text = students[i].Value,
                    Style = Resources["detailTablet"] as Style
                };
                string birthday = Attendance.GetStudentInfo(students[i].Key)[(int)HymnsAttendance.StudentInfo.BIRTHDAY];

                Label birthdayLabel = new Label()
                {
                    //Text = num.Length == 0 ? "" : "(" + num.Substring(0, 3) + ")-" + num.Substring(3, 3) + "-" + num.Substring(6),
                    Text = birthday,
                    Style = Resources["detailTablet"] as Style
                };

                int days = Attendance.GetDatesForYear(students[i].Key);

                float weeks = DateTime.Now.DayOfYear / 7.0f;
                string percent = ((int)(100 * days / weeks)).ToString() + "%";

                Label attend = new Label()
                {
                    Text = percent,
                    Style = Resources["detailTablet"] as Style
                };

                SwipeItem editSwipeItem = new SwipeItem
                {
                    Text = "EDIT",
                    BackgroundColor = Color.Red,
                    CommandParameter = new Label() {  Text = students[i].Key + ";" + students[i].Value }
                };
                editSwipeItem.Invoked += SwipeItem_Clicked;

                SwipeItem infoSwipeItem = new SwipeItem
                {
                    Text = "INFO",
                    BackgroundColor = Color.Green,
                    CommandParameter = new Label() { Text = students[i].Key + ";" + students[i].Value }
                };
                infoSwipeItem.Invoked += InfoSwipeItem_Clicked;

                Grid grid = new Grid()
                {
                    ColumnDefinitions = new ColumnDefinitionCollection()
                    {
                        new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) },
                        new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) }
                    }, 
                    BackgroundColor = Color.White
                };

                grid.Children.Add(profilePicture, 0, 0);
                grid.Children.Add(nameLabel, 1, 0);
                grid.Children.Add(birthdayLabel, 2, 0);
                grid.Children.Add(attend, 3, 0);

                List<SwipeItem> swipeItems = new List<SwipeItem>() { editSwipeItem, infoSwipeItem };

                SwipeView swipeView = new SwipeView
                {
                    RightItems = new SwipeItems(swipeItems),
                    Content = grid
                };
                InfoStack.Children.Add(swipeView);
            }
        }

        private async void InfoSwipeItem_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Success", "Go to info page", "ok");
        }

        private async void SwipeItem_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Success", "Go to edit page", "ok");
        }
    }
}