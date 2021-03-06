﻿using System;
using System.Collections.Generic;
using System.IO;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradeAttendance : ContentPage
    {
        readonly HymnsAttendance Attendance;
        readonly string ClassName;

        readonly List<ViewCell> Backup;
        readonly List<ViewCell> TeacherBackup;
        
        TableSection NamesTable;
        TableSection TeachersNamesTable;
        
        IList<KeyValuePair<string, string>> InGrade;
        IList<KeyValuePair<string, string>> TeachersInGrade;

        public GradeAttendance(HymnsAttendance attendance, string className)
        {
            InitializeComponent();
            DatePicker.MaximumDate = DateTime.Now;
            Attendance = attendance;
            ClassName = className;
            Backup = new List<ViewCell>();
            TeacherBackup = new List<ViewCell>();
            NamesTable = new TableSection();
            TeachersNamesTable = new TableSection();

            InGrade = Attendance.StudentsOfGrade(ClassName);
            InitializeTeachers();
            InitializeStudents();

        }

        private void InitializeStudents()
        {
            NamesTable.Clear();
            Backup.Clear();

            if (InGrade.Count == 0 && TeachersInGrade.Count == 0)
            {
                NotFoundStack.IsVisible = true;
            }
            else
            {
                NotFoundStack.IsVisible = false;
            }

            int count = 0;
            string filter = StudentSearch.Text == null ? "" : StudentSearch.Text.Trim().ToLower();
            foreach (var s in InGrade)
            {
                ViewCell cell = new ViewCell() { Height = 70 };

                StackLayout sl = new StackLayout() { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.FromHex("#FFFFFF") };
                sl.Padding = new Thickness(10, 0, 10, 0);
                sl.Children.Add(new Label() { Text = s.Key, IsVisible = false });

                CircleImage ci = GetPhoto(s, true);

                sl.Children.Add(ci);

                CheckBox cb = new CheckBox() { HorizontalOptions = LayoutOptions.EndAndExpand };

                Label l = new Label()
                {
                    Text = s.Value,
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Black,
                    FontSize = 16,
                    Padding = new Thickness(30, 0, 0, 0)
                };
                cell.Tapped += TableCell_Tapped;

                sl.Children.Add(l);
                sl.Children.Add(cb);

                cell.View = sl;

                Backup.Add(cell);

                if (Attendance.AttendedToday(s.Key, DatePicker.Date))
                {
                    cb.IsChecked = true;
                }
                else
                {
                    cb.IsChecked = false;
                }

                if (s.Value.ToLower().Contains(filter))
                {
                    // make visible
                    NamesTable.Add(cell);
                    count++;

                }
            }
            //NamesTableRoot.Clear();
            NamesTableRoot.Add(NamesTable);


        }

        private CircleImage GetPhoto(KeyValuePair<string, string> s, bool isStudent)
        {
            Stream studentstream;
            if (isStudent)
            {
               studentstream = Attendance.GetStudentPhoto(s.Key);
            }
            else
            {
                studentstream = Attendance.GetTeacherPhoto(s.Key);
            }
            
            CircleImage profilePicture = new CircleImage
            {
                Source = ImageSource.FromStream(() =>
                {
                    return studentstream;
                }), 
                HeightRequest = 50,
                WidthRequest = 50
            };

            if (studentstream == null)
            {
                profilePicture = new CircleImage { Source = "blankprofile.png", HeightRequest = 50, WidthRequest = 50 };

            }
            return profilePicture;
        }

        private void InitializeTeachers()
        {
            TeachersNamesTable.Clear();
            TeacherBackup.Clear();
            TeachersInGrade = Attendance.TeachersOfGrade(ClassName);

            /*
            if (TeachersInGrade.Count == 0)
            {
                NotFoundStack.IsVisible = true;
            }*/

            int count = 0;
            string filter = StudentSearch.Text == null ? "" : StudentSearch.Text.Trim().ToLower();
            foreach (var t in TeachersInGrade)
            {
                ViewCell cell = new ViewCell() { Height = 70, };

                StackLayout sl = new StackLayout() { Orientation = StackOrientation.Horizontal, BackgroundColor = Color.FromHex("#FFFFFF") };
                sl.Padding = new Thickness(0, 0, 10, 0);
                sl.Children.Add(new Label() { Text = t.Key, IsVisible = false });

                sl.Children.Add(GetPhoto(t, false));

                CheckBox cb = new CheckBox() { HorizontalOptions = LayoutOptions.EndAndExpand };

                Label l = new Label()
                {
                    Text = t.Value,
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Black,
                    FontSize = 16,
                    Padding = new Thickness(30, 0, 0, 0)
                };
                cell.Tapped += TableCell_Tapped;

                sl.Children.Add(l);
                sl.Children.Add(cb);

                cell.View = sl;

                TeacherBackup.Add(cell);

                if (Attendance.TeacherAttendedToday(t.Key, DatePicker.Date))
                {
                    cb.IsChecked = true;
                }
                else
                {
                    cb.IsChecked = false;
                }

                if (t.Value.ToLower().Contains(filter))
                {
                    TeachersNamesTable.Add(cell);
                    count++;
                }
            }
            NamesTableRoot.Clear();
            TeachersNamesTable.Add(new ViewCell() { View = new Label { BackgroundColor = Color.LightGray } });
            NamesTableRoot.Add(TeachersNamesTable);
        }
        private void TableCell_Tapped(object sender, EventArgs e)
        {
            ViewCell c = sender as ViewCell;
            CheckBox x = (c.View as StackLayout).Children[3] as CheckBox;
            x.IsChecked = !x.IsChecked;
        }

        private void StudentSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if ((e.OldTextValue??"").Length > (e.NewTextValue??"").Length)
            {
                InGrade = Attendance.StudentsOfGrade(ClassName);
                InitializeTeachers();
                InitializeStudents();
            }
            else
            {
                NamesTable = new TableSection();
                List<ViewCell> visible = new List<ViewCell>();
                string filter = StudentSearch.Text.Trim().ToLower();
                int count = 0;
                foreach (ViewCell c in Backup)
                {
                    StackLayout s = c.View as StackLayout;
                    Label l = s.Children[2] as Label;
                    if (l.Text.ToLower().Contains(filter))
                    {
                        // make visible
                        s.Children[1] = GetPhoto(new KeyValuePair<string, string>((s.Children[0] as Label).Text, l.Text), true);
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
        }

        private async void SubmitAttendance_Clicked(object sender, EventArgs e)
        {
            List<string> selected = new List<string>();
            List<string> teachersSelected = new List<string>();

            foreach (ViewCell c in Backup)
            {
                StackLayout sl = c.View as StackLayout;
                CheckBox cb = sl.Children[3] as CheckBox;
                Label l = sl.Children[0] as Label;

                if (cb.IsChecked && Backup.Contains(c))
                {
                    selected.Add(l.Text);
                }
            }

            foreach (ViewCell c in TeacherBackup)
            {
                StackLayout sl = c.View as StackLayout;
                CheckBox cb = sl.Children[3] as CheckBox;
                Label l = sl.Children[0] as Label;

                if (cb.IsChecked && TeacherBackup.Contains(c))
                {
                    teachersSelected.Add(l.Text);
                }
            }

            var answer = await DisplayAlert("Submit", "Please verify there are " + selected.Count + " students and " + teachersSelected.Count + " Teachers.", "continue", "cancel");
            if (answer)
            {
                Attendance.TakeAttendance(selected, DatePicker.Date);
                Attendance.TakeTeacherAttendance(teachersSelected, DatePicker.Date);
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
            //for students
            if (Backup.Count != InGrade.Count)
            {
                NotFoundStack.IsVisible = false;
                for (int i = 0; i < InGrade.Count; i++)
                {
                    string q = "";
                    if (Backup.Count != i)
                    {
                        var b = Backup[i].View as StackLayout;
                        q = (b.Children[0] as Label).Text;
                    }
                    if (q != InGrade[i].Key)
                    {
                        ViewCell cell = new ViewCell() { Height = 70 };
                        StackLayout sl = new StackLayout() { Orientation = StackOrientation.Horizontal };
                        sl.Children.Add(new Label() { Text = InGrade[i].Key, IsVisible = false });
                        CheckBox cb = new CheckBox() { HorizontalOptions = LayoutOptions.EndAndExpand };
                        Label l = new Label()
                        {
                            Text = InGrade[i].Value,
                            VerticalOptions = LayoutOptions.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            TextColor = Color.Black,
                            FontSize = 16
                        };
                        cell.Tapped += TableCell_Tapped;
                        sl.Children.Add(GetPhoto(InGrade[i], true));
                        sl.Children.Add(l);
                        sl.Children.Add(cb);
                        cell.View = sl;

                        NamesTable.Add(cell);
                        Backup.Insert(i, cell);

                    }
                }
            }

            //for teachers
            if (TeacherBackup.Count != TeachersInGrade.Count)
            {
                NotFoundStack.IsVisible = false;
                for (int i = 0; i < TeachersInGrade.Count; i++)
                {
                    string q = "";
                    if (TeacherBackup.Count != i)
                    {
                        var b = TeacherBackup[i].View as StackLayout;
                        q = (b.Children[0] as Label).Text;
                    }
                    if (q != TeachersInGrade[i].Key)
                    {
                        ViewCell cell = new ViewCell() { Height = 70 };
                        StackLayout sl = new StackLayout() { Orientation = StackOrientation.Horizontal };
                        sl.Children.Add(new Label() { Text = InGrade[i].Key, IsVisible = false });
                        CheckBox cb = new CheckBox() { HorizontalOptions = LayoutOptions.EndAndExpand };
                        Label l = new Label()
                        {
                            Text = InGrade[i].Value,
                            VerticalOptions = LayoutOptions.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            TextColor = Color.Black,
                            FontSize = 16
                        };
                        cell.Tapped += TableCell_Tapped;
                        sl.Children.Add(GetPhoto(TeachersInGrade[i], false));
                        sl.Children.Add(l);
                        sl.Children.Add(cb);
                        cell.View = sl;

                        TeachersNamesTable.Add(cell);
                        TeacherBackup.Insert(i, cell);
                        return;
                    }
                }
            }
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            InGrade = Attendance.StudentsOfGrade(ClassName);
            InitializeTeachers();
            InitializeStudents();
        }

    }
}