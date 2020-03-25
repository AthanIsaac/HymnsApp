using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradeAttendance : ContentPage
    {

        readonly HymnsAttendance2 Attendance;
        readonly string ClassName;
        readonly List<ViewCell> Backup;
        TableSection NamesTable;
        List<KeyValuePair<string, string>> InGrade;
        public GradeAttendance(HymnsAttendance2 attendance, string className)
        {
            InitializeComponent();
            Attendance = attendance;
            ClassName = className;
            Backup = new List<ViewCell>();
            InitializeStudents();
        }

        private void InitializeStudents()
        {
            NamesTable = new TableSection();
            Backup.Clear();
            InGrade = Attendance.StudentsOfGrade(ClassName);
            
            if (InGrade.Count == 0)
            {
                NotFoundStack.IsVisible = true;
            }

            int count = 0;
            string filter = StudentSearch.Text == null ? "" : StudentSearch.Text.Trim().ToLower();
            for (int i = 0; i < InGrade.Count; i++)
            {
                InfoGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(60)
                });

                ViewCell cell = new ViewCell() { Height = 70 };
                
                Grid grid = new Grid() { BackgroundColor = Color.FromHex("#EEEEEE") };
                
               

                grid.Children.Add(new Label()
                {
                    Text = InGrade[i].Value,
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Black,
                    FontSize = 16
                }, 0, i);
                grid.Children.Add(new ImageButton
                {
                    Source = "editsmall.png",
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, i);
                CheckBox cb = new CheckBox();
                grid.Children.Add(cb, 2, i);

                //id
                Label id = new Label() { Text = InGrade[i].Key, IsVisible = false };
                grid.Children.Add(id, 3, i);

                cell.Tapped += TableCell_Tapped;

                //sl.Children.Add(imageButton);
                //sl.Children.Add(l);
                //sl.Children.Add(cb);

                cell.View = grid;

                Backup.Add(cell);

                if (Attendance.AttendedToday(InGrade[i].Key, DatePicker.Date))
                {
                    cb.IsChecked = true;
                }
                else
                {
                    cb.IsChecked = false;
                }

                if (InGrade[i].Value.ToLower().Contains(filter))
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
            CheckBox x = (c.View as Grid).Children[2] as CheckBox;
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
                Grid s = c.View as Grid;
                
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
                Grid sl = c.View as Grid;
                CheckBox cb = sl.Children[2] as CheckBox;
                Label l = sl.Children[3] as Label;

                if (cb.IsChecked)
                {
                    selected.Add(l.Text);
                }
            }
            
            var answer = await DisplayAlert("Submit", "Please verify there are " + selected.Count + " students", "continue", "cancel");
            if (answer)
            {
                Attendance.TakeAttendance(selected, DatePicker.Date);
                await Navigation.PopAsync();
            }
        }
        

        private void AddStudent_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditAddStudent(Attendance, "", StudentSearch.Text, ClassName, true));
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            // this is meant to display the person that was added
            // this also gets called when we come from the main page 
            // List<string> InGrade = Attendance.StudentsOfGrade(Grade);
            // wierd behavior check here!
            if (Backup.Count != InGrade.Count)
            {
                NotFoundStack.IsVisible = false;
                for (int i = 0; i < InGrade.Count; i++)
                {
                    var b = Backup[i].View as Grid;
                    if ((b.Children[1] as Label).Text != InGrade[i].Key) 
                    {
                        ViewCell cell = new ViewCell() { Height = 70 };
                        Grid sl = new Grid();
                        sl.Children.Add(new Label() { Text = InGrade[i].Key, IsVisible = false });
                        CheckBox cb = new CheckBox();
                        Label l = new Label()
                        {
                            Text = InGrade[i].Value,
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
                        Backup.Insert(i, cell);
                        return;
                    }
                }
            }
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            // this is a heavy method so change if slow
            InitializeStudents();
        }

    }
}