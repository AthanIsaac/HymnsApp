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
        public CurriculumPage()
        {
            InitializeComponent();
        }
    }

    public class Curriculum
    {
        public string Class;
        //hymn, link
        public List<KeyValuePair<string, string>> Hymns;
        public Curriculum(string theclass, List<KeyValuePair<string, string>> hymns)
        {
            Class = theclass;
            Hymns = hymns;

        }
    }
}