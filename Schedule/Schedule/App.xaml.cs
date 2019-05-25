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
        public static List<Day> myTimetable;

        public int MyProperty { get; set; }

        public App()
        {
            SchedulesLoad();

            InitializeComponent();

            //параметры пользователя
            if (!Current.Properties.ContainsKey("isTeacher")) //проверка на авторизованность
            {
                MainPage = new Login();

                //Значит пользователь еще не входил и нужно загрузить файл с графиком, 
                //чтобы в дальнейшем сделать в Properties переменную timetable
                TimetableLoad();
            }
            else
            {
                MainPage = new MasterDetailPage1();
                //проверка графика, для студента нужен, сохраненный при авторизации "timetable" в Properties,
                //а для преподавателя еще и весь файл timetable.json
                if (!Current.Properties.ContainsKey("groupIdName")) 
                {
                    TimetableLoad();
                    string table = "";
                    if (App.Current.Properties.TryGetValue("timetable", out object tableFrom))
                    {
                        table = (string)tableFrom;
                        myTimetable = JsonConvert.DeserializeObject<List<Day>>(table);
                    }

                    DateTime now = DateTime.Now;
                    int day = now.Day;
                    int month = now.Month;
                    foreach (var item in myTimetable)
                    {
                        if (item.ThisDay == day && item.ThisMonth == month)
                        {
                            if (item.ThisWeek % 2 == 0)
                            {
                                App.Current.Properties["numOfWeek"] = "2";
                            }
                            else
                                App.Current.Properties["numOfWeek"] = "1";
                            break;
                        }
                    }
                }
                else
                {
                    string table = "";
                    if (App.Current.Properties.TryGetValue("timetable", out object tableFrom))
                    {
                        table = (string)tableFrom;
                        myTimetable = JsonConvert.DeserializeObject<List<Day>>(table);
                    }

                    DateTime now = DateTime.Now;
                    int day = now.Day;
                    int month = now.Month;
                    foreach (var item in myTimetable)
                    {
                        if (item.ThisDay == day && item.ThisMonth == month)
                        {
                            if (item.ThisWeek % 2 == 0)
                            {
                                App.Current.Properties["numOfWeek"] = "2";
                            }
                            else
                                App.Current.Properties["numOfWeek"] = "1";
                            break;
                        }
                    }
                }
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

        public void TimetableLoad() //загрузка файла с графиком
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(DayMonday)).Assembly;
            timetable = new List<Specialty>();
            Stream stream2 = assembly.GetManifestResourceStream("Schedule.timetable.json");
            using (var reader = new System.IO.StreamReader(stream2))
            {
                string json = reader.ReadToEnd();
                timetable = JsonConvert.DeserializeObject<List<Specialty>>(json);
            }
        }

        public void SchedulesLoad() //загрузка расписаний каждого факультета
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
        }

    }
}
