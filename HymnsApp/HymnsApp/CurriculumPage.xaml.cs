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
                
                StackLayout sl = new StackLayout();
                for (int j = 0; j < cur[i].Length; j++)
                {
                    sl.Children.Add(new Label { Text = j + ". " + cur[i][j] });

                }
                Accordion.CustomControls.Accordion newAccordion = new Accordion.CustomControls.Accordion()
                { Title = "Class" + i ,AccordionContentView = sl, IndicatorView = new Label (){ Text = "V   " ,FontSize = 23} };
               

                curGrid.Children.Add(newAccordion, 0, i);
            }
        }
    }
}