using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HymnsApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GradeTabbedPage : TabbedPage
    {

        public GradeTabbedPage(HymnsAttendance attendance, string grade)
        {
            InitializeComponent();
            Children.Add(new GradeAttendance(attendance, grade) { Title = "Attendance"});
            Children.Add(new StudentInfo(attendance, grade) { Title = "Student Info"});

        }
    }
}