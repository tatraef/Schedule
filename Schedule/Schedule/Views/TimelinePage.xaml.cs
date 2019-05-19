using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using Schedule.Models;

namespace Schedule.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimelinePage : ContentPage
    {
        public static List<Specialty> timetable;

        public TimelinePage()
        {
            InitializeComponent();

            timetable = new List<Specialty>();
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(DayMonday)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Schedule.timetable.json");
            using (var reader = new System.IO.StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                timetable = JsonConvert.DeserializeObject<List<Specialty>>(json);
            }
        }
    }
}