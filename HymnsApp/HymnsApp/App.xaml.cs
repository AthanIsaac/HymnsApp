using System;
using System.IO;
using Xamarin.Forms;

namespace HymnsApp
{
    public partial class App : Application
    {
        readonly HymnsAttendance2 Attendance;
        public App()
        {
            InitializeComponent();

            Attendance = new HymnsAttendance2();
            MainPage = new NavigationPage(new MainPage(Attendance)) { BarBackgroundColor = Color.FromHex("#4682B4"), BarTextColor = Color.White };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
