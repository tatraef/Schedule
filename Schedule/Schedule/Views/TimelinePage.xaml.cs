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
using System.Collections.ObjectModel;
using Schedule.ViewModels;
using Plugin.Connectivity;
using System.Net.Http;

namespace Schedule.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimelinePage : ContentPage
    {
        public byte NumberOfItems { get; set; }

        public TimelinePage()
        {
            #region Добавление элементов в Toolbar
            ToolbarItem changeNumberOfItems = new ToolbarItem
            {
                Icon = "settings.png"
            };
            changeNumberOfItems.Clicked += (object sender, System.EventArgs e) =>
            {
                forSelectDate.IsVisible = false;
                forChangeNumberOfItems.IsVisible = forChangeNumberOfItems.IsVisible ? false : true;
            };
            ToolbarItems.Add(changeNumberOfItems);

            ToolbarItem selectDate = new ToolbarItem
            {
                Icon = "calendar.png"
            };
            selectDate.Clicked += (object sender, System.EventArgs e) =>
            {
                forChangeNumberOfItems.IsVisible = false;
                forSelectDate.IsVisible = forSelectDate.IsVisible ? false : true;
            };
            ToolbarItems.Add(selectDate);
            #endregion

            NumberOfItems = 7;

            InitializeComponent();


            if (App.justLogged)
            {
                LoadScheduleAsync();
            }
            else if (!App.updateWasChecked)
            {
                ReloadPage();
                CheckUpdatesAsync();
            }
            else
            {
                ReloadPage();
            }
        }

        //Выбор определенной даты
        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            DateTime now = DateTime.Now;
            if (now.Month < 9)
            {
                if (e.NewDate.Year == now.Year && e.NewDate.Month < 9)
                {
                    ReloadPage();
                }
                else
                {
                    DisplayAlert("Ошибка", "Можно просматривать только текущий учебный год.", "ОK");
                }
            }
            else
            {
                if ((e.NewDate.Year == now.Year && e.NewDate.Month > 9) || (e.NewDate.Year == now.Year+1 && e.NewDate.Month < 9))
                {
                    TimelineViewModel bind = new TimelineViewModel(e.NewDate);
                    BindingContext = bind;

                    //проверяется студент или преподаватель
                    if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
                        if ((bool)isTeacher)
                        {
                            couplesList.ItemsSource = bind.ItemsForTeacher;
                        }
                        else
                        {
                            couplesList.ItemsSource = bind.ItemsForStudents;
                        }
                }
                else
                {
                    DisplayAlert("Ошибка", "Можно просматривать только текущий учебный год.", "ОK");
                }
            }
        }

        //Изменение количества отображаемых дней
        private void PickerToChangeNumberOfItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            NumberOfItems = Convert.ToByte(pickerToChangeNumberOfItems.Items[pickerToChangeNumberOfItems.SelectedIndex]);

            ReloadPage();

            forChangeNumberOfItems.IsVisible = false;
        }

        //строка для ответа, которую в UpdateButton_Clicked надо будет посылать  на сервер
        string updateString = "";

        //проверка обновлений расписания
        public async void CheckUpdatesAsync() 
        {
            updateChecking.IsVisible = true;

            if (CrossConnectivity.Current.IsConnected == true)
            {
                string url = "http://192.168.0.113/schedule/getAnswer.php";
                try
                {
                    if (App.Current.Properties.TryGetValue("facultyName", out object FacultyName))
                    {
                        string facultyName = (string)FacultyName;

                        if (Application.Current.Properties.ContainsKey("updateMain") &&
                            Application.Current.Properties.ContainsKey("updateRait") &&
                            Application.Current.Properties.ContainsKey("updateExam") &&
                            Application.Current.Properties.ContainsKey("updateTimetable"))
                        {
                            string updateMain = Application.Current.Properties["updateMain"] as string;
                            string updateRait = Application.Current.Properties["updateRait"] as string;
                            string updateExam = Application.Current.Properties["updateExam"] as string;
                            string updateTimetable = Application.Current.Properties["updateTimetable"] as string;

                            HttpContent content = new StringContent("updateChecking&name=" + facultyName + 
                                "&update_main=" + updateMain + 
                                "&update_rait=" + updateRait + 
                                "&update_exam=" + updateExam +
                                "&update_timetable=" + updateTimetable, Encoding.UTF8, "application/x-www-form-urlencoded");
                            HttpClient client = new HttpClient
                            {
                                BaseAddress = new Uri(url)
                            };
                            var response = await client.PostAsync(client.BaseAddress, content);
                            response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                            string res = await response.Content.ReadAsStringAsync();
                            if (res == "NO")
                            {
                                updateText.Text = "Нет доступных обновлений";
                                updateIndicator.IsVisible = false;
                                await Task.Delay(4000);
                                updateChecking.IsVisible = false;

                                App.updateWasChecked = true;  
                            }
                            else
                            {
                                updateString = res;
                                updateChecking.IsVisible = false;
                                availableUpdate.IsVisible = true;
                            }
                        }
                        else
                        {
                            LoadScheduleAsync();
                        }
                        
                    }           
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", "Не удалось получить данные, ошибка: " + ex.Message, "ОK");
                }
            }
            else
            {
                await DisplayAlert("Внимание", "Нет интернет-соединения, невозможно получить обновления раписания.", "Понятно");
            }  
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            availableUpdate.IsVisible = false;
            updateText.Text = "Загрузка обновления...";
            updateChecking.IsVisible = true;
            if (CrossConnectivity.Current.IsConnected == true)
            {
                string url = "http://192.168.0.113/schedule/getAnswer.php";
                try
                {
                    if (App.Current.Properties.TryGetValue("facultyName", out object FacultyName))
                    {
                        string facultyName = (string)FacultyName;

                        HttpContent content = new StringContent("getSchedule&name=" + facultyName + updateString, Encoding.UTF8, "application/x-www-form-urlencoded");
                        HttpClient client = new HttpClient
                        {
                            BaseAddress = new Uri(url)
                        };
                        var response = await client.PostAsync(client.BaseAddress, content);
                        response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                        string res = await response.Content.ReadAsStringAsync();
                        Dictionary<string, string> some = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);
                        if (some.ContainsKey("scheduleMain"))
                        {
                            App.facultiesMain.Clear();
                            App.facultiesMain.Add(JsonConvert.DeserializeObject<Faculty>(some["scheduleMain"]));
                            App.Current.Properties["scheduleMain"] = some["scheduleMain"];
                            App.Current.Properties["updateMain"] = some["updateMain"];
                        }
                        if (some.ContainsKey("scheduleRait"))
                        {
                            App.facultiesRait.Clear();
                            App.facultiesRait.Add(JsonConvert.DeserializeObject<Faculty>(some["scheduleRait"]));
                            App.Current.Properties["scheduleRait"] = some["scheduleRait"];
                            App.Current.Properties["updateRait"] = some["updateRait"];
                        }
                        if (some.ContainsKey("scheduleExam"))
                        {
                            App.facultiesExam.Clear();
                            App.facultiesExam.Add(JsonConvert.DeserializeObject<ExamFaculty>(some["scheduleExam"]));
                            App.Current.Properties["scheduleExam"] = some["scheduleExam"];
                            App.Current.Properties["updateExam"] = some["updateExam"];
                        }
                        if (some.ContainsKey("scheduleTimetable"))
                        {
                            SaveTimetable(some["scheduleTimetable"], some["updateTimetable"]);
                        }

                        ReloadPage();

                        updateText.Text = "Обновления загружены";
                        updateIndicator.IsVisible = false;
                        await Task.Delay(4000);
                        updateChecking.IsVisible = false;

                        App.updateWasChecked = true;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", "Не удалось получить данные, ошибка: " + ex.Message, "ОK");
                }
            }
        }

        private async void LoadScheduleAsync()
        {
            availableUpdate.IsVisible = false;
            updateText.Text = "Загрузка расписания...";
            updateChecking.IsVisible = true;
            if (CrossConnectivity.Current.IsConnected == true)
            {
                string url = "http://192.168.0.113/schedule/getAnswer.php";
                try
                {
                    if (App.Current.Properties.TryGetValue("facultyName", out object FacultyName))
                    {
                        string facultyName = (string)FacultyName;

                        HttpContent content = new StringContent("getScheduleWithoutMain&name=" + facultyName, Encoding.UTF8, "application/x-www-form-urlencoded");
                        HttpClient client = new HttpClient
                        {
                            BaseAddress = new Uri(url)
                        };
                        var response = await client.PostAsync(client.BaseAddress, content);
                        response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                        string res = await response.Content.ReadAsStringAsync();
                        Dictionary<string, string> some = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);

                        App.facultiesRait.Clear();
                        App.facultiesRait.Add(JsonConvert.DeserializeObject<Faculty>(some["scheduleRait"]));
                        App.Current.Properties["scheduleRait"] = some["scheduleRait"];
                        App.Current.Properties["updateRait"] = some["updateRait"];

                        App.facultiesExam.Clear();
                        App.facultiesExam.Add(JsonConvert.DeserializeObject<ExamFaculty>(some["scheduleExam"]));
                        App.Current.Properties["scheduleExam"] = some["scheduleExam"];
                        App.Current.Properties["updateExam"] = some["updateExam"];

                        SaveTimetable(some["scheduleTimetable"], some["updateTimetable"]); 

                        ReloadPage();

                        updateText.Text = "Расписание загружено";
                        updateIndicator.IsVisible = false;
                        await Task.Delay(4000);
                        updateChecking.IsVisible = false;

                        App.updateWasChecked = true;
                        App.justLogged = false;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", "Не удалось получить данные, ошибка: " + ex.Message, "ОK");
                }
            }
        }

        private void ReloadPage()
        {
            TimelineViewModel bind = new TimelineViewModel(NumberOfItems);
            BindingContext = bind;

            //проверяется студент или преподаватель
            if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
            {
                if ((bool)isTeacher)
                {
                    couplesList.ItemsSource = bind.ItemsForTeacher;
                }
                else
                {
                    couplesList.ItemsSource = bind.ItemsForStudents;
                }
            }
        }

        private void SaveTimetable(string scheduleTimetable, string updateTimetable)
        {
            App.Current.Properties["timetable"] = scheduleTimetable;
            App.timetable = JsonConvert.DeserializeObject<List<Specialty>>(scheduleTimetable);
            App.Current.Properties["updateTimetable"] = updateTimetable;

            string code = (string)App.Current.Properties["code"];
            string course = (string)App.Current.Properties["course"];

            foreach (var item in App.timetable)
            {
                if (item.SpecialtyName.Contains(code))
                {
                    foreach (var courses in item.Courses)
                    {
                        if (courses.CourseNumber == course)
                        {
                            App.myTimetable = courses.Days;
                            string json = JsonConvert.SerializeObject(courses.Days);
                            App.Current.Properties.Add("myTimetable", json);
                            break;
                        }
                    }
                }
            }
        }
 
    }   
}