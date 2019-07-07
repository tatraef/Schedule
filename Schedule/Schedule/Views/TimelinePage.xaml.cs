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
            ReloadPage();

            if (!App.updateWasLoaded)
            {
                CheckUpdatesAsync();
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

        public async void CheckUpdatesAsync() //проверка обновлений расписания
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

                        if (App.Current.Properties.TryGetValue("updateMain", out object UpdateMain))
                        {
                            string updateMain = (string)UpdateMain;
                            HttpContent content = new StringContent("updateChecking&name=" + facultyName + "&update_main=" + updateMain, Encoding.UTF8, "application/x-www-form-urlencoded");
                            HttpClient client = new HttpClient
                            {
                                BaseAddress = new Uri(url)
                            };
                            var response = await client.PostAsync(client.BaseAddress, content);
                            response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                            string res = await response.Content.ReadAsStringAsync();
                            if (res == "YES")
                            {
                                updateChecking.IsVisible = false;
                                availableUpdate.IsVisible = true;
                            }
                            else
                            {
                                updateText.Text = "Нет доступных обновлений";
                                updateIndicator.IsVisible = false;
                                await Task.Delay(4000);
                                updateChecking.IsVisible = false;

                                App.updateWasLoaded = true;
                            }
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

                        HttpContent content = new StringContent("getScheduleMain&name=" + facultyName, Encoding.UTF8, "application/x-www-form-urlencoded");
                        HttpClient client = new HttpClient
                        {
                            BaseAddress = new Uri(url)
                        };
                        var response = await client.PostAsync(client.BaseAddress, content);
                        response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                        string res = await response.Content.ReadAsStringAsync();
                        List<string> some = JsonConvert.DeserializeObject<List<string>>(res);
                        App.Current.Properties["scheduleMain"] = some[0];
                        App.Current.Properties["updateMain"] = some[1];
                        App.facultiesJSON.Clear();
                        App.facultiesJSON.Add(JsonConvert.DeserializeObject<Faculty>(some[0]));

                        ReloadPage();

                        updateText.Text = "Обновления загружены";
                        updateIndicator.IsVisible = false;
                        await Task.Delay(4000);
                        updateChecking.IsVisible = false;

                        App.updateWasLoaded = true;
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
    }

    
}