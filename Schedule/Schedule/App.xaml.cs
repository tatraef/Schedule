using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Collections.Generic;
using Schedule.Models;
using Schedule.Views;
using Newtonsoft.Json;
using System.Reflection;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Schedule
{
    public partial class App : Application
    {
        public static List<Faculty> facultiesJSON;
        public static Dictionary<string, Group> sched;

        public static List<Specialty> timetable;

        public int MyProperty { get; set; }

        public App()
        {
            facultiesJSON = new List<Faculty>();
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(DayMonday)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Schedule.math_b.json");
            using (var reader = new System.IO.StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                facultiesJSON.Add(JsonConvert.DeserializeObject<Faculty>(json));
            }
            Stream stream1 = assembly.GetManifestResourceStream("Schedule.math_m.json");
            using (var reader = new System.IO.StreamReader(stream1))
            {
                string json = reader.ReadToEnd();
                facultiesJSON.Add(JsonConvert.DeserializeObject<Faculty>(json));
            }

            timetable = new List<Specialty>();
            Stream stream2 = assembly.GetManifestResourceStream("Schedule.timetable.json");
            using (var reader = new System.IO.StreamReader(stream2))
            {
                string json = reader.ReadToEnd();
                timetable = JsonConvert.DeserializeObject<List<Specialty>>(json);
            }

            InitializeComponent();

            
            //параметры пользователя
            if (!Current.Properties.ContainsKey("isTeacher"))
            {
                MainPage = new Login();
            }
            else
            {
                MainPage = new MasterDetailPage1();
            }
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
