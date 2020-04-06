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

            string[] classes = new string[cur.Length];

            for (int i = 0; i < cur.Length; i++) 
            {
                classes[i] = parseName(cur[i][0]);
            }

            for (int i = 0; i < cur.Length; i++)
            {
                
                StackLayout sl = new StackLayout();
                for (int j = 1; j < cur[i].Length; j++)
                {
                    sl.Children.Add(new Label { Text = (j) + ". " + cur[i][j] });

                }
                Accordion.CustomControls.Accordion newAccordion = new Accordion.CustomControls.Accordion()
                { Title = classes[i] , AccordionContentView = sl, IndicatorView = new Label (){ Text = "V   ", FontSize = 23} };

                curGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                curGrid.Children.Add(newAccordion, 0, i);
            }
        }

        private string parseName(string c)
        {
            if (c.Contains("kindergarten"))
            {
                c = "Kindergarten";
                return c;
            }

            if (c.Contains("highSchool"))
            {
                c = "HighSchool";
                return c;
            }
            if (c.Contains("Grade"))
            {
                int index = c.IndexOf("Grade");
                c = c.Substring(0, index) + " " + c.Substring(index);

                if (c.Contains("&"))
                {
                    int ampersand = c.IndexOf("&");
                    c = c.Substring(0, ampersand) + " & " + c.Substring(ampersand + 1);
                }
                return c;
            }

            else
            {
                //really inefficent, find better way
                for (int j = 0; j < c.Length; j++)
                {

                    if (char.IsUpper(c[j]))
                    {
                        c = c.Substring(0, j) + " " + c.Substring(j);
                        j++;
                    }


                }

                c = c.Replace("m", "M");
                c = c.Trim();
                return c;
            }

        }
    }
}