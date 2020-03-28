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
        }

        private void InitGrid()
        {
            var students = Attendance.StudentsOfGrade(ClassName);

            InfoStack.Children.Clear();

            for (int i = 0; i < students.Count; i++)
            {
                var stream = Attendance.GetStudentPhoto(students[i].Key);

                CircleImage profilePicture = new CircleImage
                {
                    Source = ImageSource.FromStream(() =>
                    {
                        return stream;
                    }),
                    HeightRequest = 50,
                    WidthRequest = 50
                };

                if (stream == null)
                {
                    profilePicture = new CircleImage { Source = "blankprofile.png", HeightRequest = 50, WidthRequest = 50 };
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
                    RowDefinitions = new RowDefinitionCollection()
                    {
                        new RowDefinition() { Height = new GridLength(75, GridUnitType.Absolute) }
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
                InfoStack.Children.Add(new BoxView
                {
                    Color = Color.LightGray,
                    BackgroundColor = Color.LightGray,
                    HeightRequest = 0.5,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center
                });
            }
        }

        private void InfoSwipeItem_Clicked(object sender, EventArgs e)
        {
            SwipeItem s = sender as SwipeItem;
            Label label = s.CommandParameter as Label;
            Navigation.PushAsync(new StudentProfilexaml(Attendance, label.Text.Split(';')[0], ClassName));
        }

        private void SwipeItem_Clicked(object sender, EventArgs e)
        {
            SwipeItem s = sender as SwipeItem;
            Label label = s.CommandParameter as Label;
            string[] idname = label.Text.Split(';');
            Navigation.PushAsync(new EditAddStudent(Attendance, idname[0], idname[1], ClassName, false));
        }

        protected override void OnAppearing()
        {
            InitGrid();
        }
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            //bug: HymnsAttendance attendance, string id, string name, string className, bool add
            Navigation.PushAsync(new EditAddStudent(Attendance, "", "", ClassName, true));
        }
    }
}