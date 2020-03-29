using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurriculumPage : ContentPage
    {
        HymnsAttendance Attendance;
        public CurriculumPage(HymnsAttendance attendance)
        {
            InitializeComponent();
            Attendance = attendance;
            //PrintCurriculum();
        }

        

        //public void PrintCurriculum()
        //{
        //    var cur = Attendance.GetCurriculum();

        //    for (int i = 0; i < cur.Length; i++)
        //    {
        //        Curriculum.Children.Add(new Label { Text = "next class" });
        //        for (int j = 0; j < cur[i].Length; j++)
        //        {
        //            Curriculum.Children.Add(new Label { Text = cur[i][j] });
        //        }
        //    }
        //}
    }
}