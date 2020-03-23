using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

namespace HymnsApp.Droid
{
    [Activity(Label = "Hymns App", Icon = "@drawable/HymnsLogoCrop", Theme = "@style/splashscreen", MainLauncher = true, NoHistory = true)]
    class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            ActivityIndicator activityIndicator = new ActivityIndicator { IsRunning = true };
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }
    }
}