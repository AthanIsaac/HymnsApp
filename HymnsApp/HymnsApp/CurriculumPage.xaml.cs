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
            PrintCurriculum();

        }



        public void PrintCurriculum()
        {

            var cur = Attendance.GetCurriculum();

            for (int i = 0; i < cur.Length; i++)
            {
                Accordion.CustomControls.Accordion newAccordion = new Accordion.CustomControls.Accordion() { Title = "Nextclass" };
                StackLayout sl = new StackLayout();
                for (int j = 0; j < cur[i].Length; j++)
                {
                    sl.Children.Add(new Label { Text = cur[i][j] });
                    
                }
            }
        }
    }
}