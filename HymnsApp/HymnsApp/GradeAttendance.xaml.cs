using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradeAttendance : ContentPage
    {

        readonly HymnsAttendance Attendance;
        readonly int Grade;
        readonly List<ViewCell> Backup;
        TableSection NamesTable;
        public GradeAttendance(HymnsAttendance attendance, int grade)
        {
            InitializeComponent();
            Attendance = attendance;
            Grade = grade;
            Backup = new List<ViewCell>();
            InitializeStudents();
        }

        private void InitializeStudents()
        {
            NamesTable = new TableSection();
            Backup.Clear();
            List<string> InGrade = Attendance.StudentsOfGrade(Grade);
            InGrade.Sort();
            if (InGrade.Count == 0)
            {
                NotFoundStack.IsVisible = true;
            }
            int count = 0;
            string filter = StudentSearch.Text == null ? "" : StudentSearch.Text.Trim().ToLower();
            foreach (string s in InGrade)
            {
                ViewCell cell = new ViewCell() { Height = 70 };
                StackLayout sl = new StackLayout() { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.FromHex("#EEEEEE") };
                CheckBox cb = new CheckBox();
                Label l = new Label()
                {
                    Text = Capitalize(s),
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Black,
                    FontSize = 16
                };
                cell.Tapped += TableCell_Tapped;

                sl.Children.Add(cb);
                sl.Children.Add(l);
                cell.View = sl;

                Backup.Add(cell);

                if (Attendance.AttendedToday(s, DatePicker.Date))
                {
                    cb.IsChecked = true;
                }
                else
                {
                    cb.IsChecked = false;
                }

                if (s.Contains(filter))
                {
                    // make visible
                    NamesTable.Add(cell);
                    count++;
                }
            }
            NamesTableRoot.Clear();
            NamesTableRoot.Add(NamesTable);
        }

        private void TableCell_Tapped(object sender, EventArgs e)
        {
            ViewCell c = sender as ViewCell;
            CheckBox x = (c.View as StackLayout).Children[0] as CheckBox;
            x.IsChecked = !x.IsChecked;
        }

        private void StudentSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            NamesTable = new TableSection();
            List<ViewCell> visible = new List<ViewCell>();
            string filter = StudentSearch.Text.Trim().ToLower();
            int count = 0;
            foreach (ViewCell c in Backup)
            {
                StackLayout s = c.View as StackLayout;
                Label l = s.Children[1] as Label;
                if (l.Text.ToLower().Contains(filter))
                {
                    // make visible
                    visible.Add(c);
                    count++;
                }
            }
            foreach (ViewCell v in visible)
            {
                NamesTable.Add(v);
            }

            if (count == 0)
            {
                // there are no students that match this filter
                NotFoundStack.IsVisible = true;
            }
            else
            {
                NotFoundStack.IsVisible = false;
            }
            NamesTableRoot.Clear();
            NamesTableRoot.Add(NamesTable);
            Scroll.ScrollToAsync(0, 0, false);
        }

        private async void SubmitAttendance_Clicked(object sender, EventArgs e)
        {
            List<string> selected = new List<string>();
            foreach (ViewCell c in Backup)
            {
                StackLayout sl = c.View as StackLayout;
                CheckBox cb = sl.Children[0] as CheckBox;
                Label l = sl.Children[1] as Label;

                if (cb.IsChecked)
                {
                    selected.Add(l.Text.ToLower());
                }
            }
            var answer = await DisplayAlert("Submit", "Please verify there are " + selected.Count + " students", "continue", "cancel");
            if (answer)
            {
                Attendance.TakeAttendance(selected, Grade, DatePicker.Date);
                Attendance.WriteStudents();
                await Navigation.PopAsync();
            }
        }

        private void AddStudent_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditAddStudent(Attendance, StudentSearch.Text, Grade, true));
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            // this is meant to display the person that was added
            // this also gets called when we come from the main page 
            List<string> InGrade = Attendance.StudentsOfGrade(Grade);
            // wierd behavior check here!
            if (Backup.Count != InGrade.Count)
            {
                NotFoundStack.IsVisible = false;
                string newStudent = InGrade[InGrade.Count - 1];
                ViewCell cell = new ViewCell() { Height = 70 };
                StackLayout sl = new StackLayout() { Orientation = StackOrientation.Horizontal };
                CheckBox cb = new CheckBox();
                Label l = new Label()
                {
                    Text = newStudent,
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Black,
                    FontSize = 16
                };
                cell.Tapped += TableCell_Tapped;
                sl.Children.Add(cb);
                sl.Children.Add(l);
                cell.View = sl;

                NamesTable.Add(cell);
                Backup.Add(cell);
            }
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            // this is a heavy method so change if slow
            InitializeStudents();
        }

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