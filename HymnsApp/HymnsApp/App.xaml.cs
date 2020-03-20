using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{
    public partial class App : Application
    {
        readonly HymnsAttendance Attendance;
        public App()
        {
            InitializeComponent();
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HymnsStudents.txt");

            Attendance = new HymnsAttendance(fileName);
            MainPage = new NavigationPage(new MainPage(Attendance)) { BarBackgroundColor = Color.FromHex("#23395D"), BarTextColor = Color.White };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            Attendance.WriteStudents();
        }

        protected override void OnResume()
        {
        }
    }
}
