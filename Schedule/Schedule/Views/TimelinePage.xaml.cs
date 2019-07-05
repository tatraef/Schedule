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

            CheckUpdatesAsync();

            NumberOfItems = 7;

            InitializeComponent();
            TimelineViewModel bind = new TimelineViewModel(NumberOfItems);
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

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            DateTime now = DateTime.Now;
            if (now.Month < 9)
            {
                if (e.NewDate.Year == now.Year && e.NewDate.Month < 9)
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

        private void PickerToChangeNumberOfItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            NumberOfItems = Convert.ToByte(pickerToChangeNumberOfItems.Items[pickerToChangeNumberOfItems.SelectedIndex]);

            TimelineViewModel bind = new TimelineViewModel(NumberOfItems);
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

            forChangeNumberOfItems.IsVisible = false;
        }

        public async void CheckUpdatesAsync() //проверка обновлений расписания
        {
            if (CrossConnectivity.Current.IsConnected == true)
            {
                string url = "http://localhost/schedule/getAnswer.php";

                try
                {
                    HttpContent content = new StringContent("check", Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpClient client = new HttpClient
                    {
                        BaseAddress = new Uri(url)
                    };
                    var response = await client.PostAsync(client.BaseAddress, content);
                    response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                    var res = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Получилось", res, "ОK");
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
    }

    
}