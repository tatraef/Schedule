using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Collections.Generic;
using Schedule.Models;
using Schedule.Views;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Schedule
{
    public partial class App : Application
    {
        //переменная, чтобы проверять обновление только один раз
        public static string url = "http://math.nosu.ru/schedule/getAnswer.php";
        //переменная, чтобы проверять обновление только один раз
        public static bool updateWasChecked = false;
        //переменная, чтобы проверять была ли авторизация только что и подгружать расписание
        public static bool justLogged = false; 

        public static List<Faculty> facultiesMain;
        public static List<Faculty> facultiesRait;
        public static List<ExamFaculty> facultiesExam;

        public static List<Specialty> timetable;
        public static List<Day> myTimetable;

        public int MyProperty { get; set; }

        public App()
        {
            facultiesMain = new List<Faculty>();
            facultiesRait = new List<Faculty>();
            facultiesExam = new List<ExamFaculty>();
            timetable = new List<Specialty>();
            myTimetable = new List<Day>();

            InitializeComponent();

            //параметры пользователя
            if (!Current.Properties.ContainsKey("isTeacher")) //проверка на авторизованность
            {
                MainPage = new Login();
            }
            else
            {
                //проверка на существование важных даннах
                if (!Current.Properties.ContainsKey("scheduleMain") ||
                    !Current.Properties.ContainsKey("scheduleRait") ||
                    !Current.Properties.ContainsKey("scheduleExam") ||
                    !Current.Properties.ContainsKey("updateTimetable") ||
                    !Current.Properties.ContainsKey("myTimetable"))
                {
                    justLogged = true;
                    MainPage = new MasterDetailPage1();
                }
                else
                {
                    SchedulesLoad();
                    TimetableLoad();
                    MainPage = new MasterDetailPage1();
                } 
            }
        }

        protected override void OnStart()
        {
            AppCenter.Start("ios=583b87e2-58a6-41be-bd93-4ad2edc67012;android=aab613c6-e8e0-4e29-bced-464ac4737dde;uwp=153f3ec3-d466-4fa3-963b-33d30aeb7d62", typeof(Analytics), typeof(Crashes));        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public void SchedulesLoad() 
        {
            if ((bool)Current.Properties["isTeacher"])
            {
                facultiesMain.AddRange(JsonConvert.DeserializeObject<List<Faculty>>((string)Current.Properties["scheduleMain"]));
                facultiesRait.AddRange(JsonConvert.DeserializeObject<List<Faculty>>((string)Current.Properties["scheduleRait"]));
                facultiesExam.AddRange(JsonConvert.DeserializeObject<List<ExamFaculty>>((string)Current.Properties["scheduleExam"]));
            }
            else
            {
                facultiesMain.Add(JsonConvert.DeserializeObject<Faculty>((string)Current.Properties["scheduleMain"]));
                facultiesRait.Add(JsonConvert.DeserializeObject<Faculty>((string)Current.Properties["scheduleRait"]));
                facultiesExam.Add(JsonConvert.DeserializeObject<ExamFaculty>((string)Current.Properties["scheduleExam"]));
            }
        }

        //загрузка графика и определение номера недели
        public void TimetableLoad()
        {          
            if ((bool)Current.Properties["isTeacher"])
            { 
                //полный график нужен только для преподавателя
                timetable = JsonConvert.DeserializeObject<List<Specialty>>((string)Current.Properties["timetable"]);
            }
            myTimetable = JsonConvert.DeserializeObject<List<Day>>((string)Current.Properties["myTimetable"]);

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
