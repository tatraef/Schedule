using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Schedule.Models;
using System.Net.Http;
using Plugin.Connectivity;
using Schedule.ViewModels;

namespace Schedule.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesirePage : ContentPage
    {
        public DesirePage()
        {
            InitializeComponent();
        }

        private DesireViewModel bind;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            bind = new DesireViewModel();
            BindingContext = bind;
        }

        private void MondayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox) sender;
                bind.Monday[checkBox.TabIndex] = 1;
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Monday[checkBox.TabIndex] = 0;
            }
        }

        private void TuesdayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Tuesday[checkBox.TabIndex] = 1;
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Tuesday[checkBox.TabIndex] = 0;
            }
        }

        private void WednesdayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Wednesday[checkBox.TabIndex] = 1;
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Wednesday[checkBox.TabIndex] = 0;
            }
        }

        private void ThursdayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Thursday[checkBox.TabIndex] = 1;
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Thursday[checkBox.TabIndex] = 0;
            }
        }

        private void FridayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Friday[checkBox.TabIndex] = 1;
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Friday[checkBox.TabIndex] = 0;
            }
        }

        private void SaturdayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Saturday[checkBox.TabIndex] = 1;
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                bind.Saturday[checkBox.TabIndex] = 0;
            }
        }

        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            if (indicator.IsVisible)
                return;

            if (string.IsNullOrEmpty(CodeEntry.Text))
            {
                await DisplayAlert("Ошибка", "Введите проверочный код!", "OK");
            }
            else
            {
                await SendDesire();
            }
        }

        public async Task SendDesire()
        {
            Desire desire = new Desire
            {
                Понедельник = new String(intArrayToChar(bind.Monday)),
                Вторник = new String(intArrayToChar(bind.Tuesday)),
                Среда = new String(intArrayToChar(bind.Wednesday)),
                Четверг = new String(intArrayToChar(bind.Thursday)),
                Пятница = new String(intArrayToChar(bind.Friday)),
                Суббота = new String(intArrayToChar(bind.Saturday)),
                Message = MessageEditor.Text
            };

            if (App.Current.Properties.TryGetValue("teacherName", out object tname))
            {
                var json = JsonConvert.SerializeObject(desire);
                var faculty = App.Current.Properties["facultyName"];

                var urlContent = "updateDesire=some&name=" + tname + "&faculty=" + faculty + "&code=" + CodeEntry.Text.ToString() + "&json=" + json;

                HttpContent content = new StringContent(urlContent, Encoding.UTF8, "application/x-www-form-urlencoded");
                
                string result = "";
                ShowActivityIndicator();

                if (CrossConnectivity.Current.IsConnected == true)
                {
                    try
                    {
                        HttpClient client = new HttpClient
                        {
                            BaseAddress = new Uri(App.url)
                        };
                        var response = await client.PostAsync(client.BaseAddress, content);
                        response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                        result = await response.Content.ReadAsStringAsync();
                        if (result == "SAVED")
                        {
                            App.Current.Properties["desire"] = json;
                            await DisplayAlert("Готово", "Пожелания успешно сохранены!", "OK");
                        }
                        else if(result == "NOT")
                        {
                            await DisplayAlert("Ошибка", "Неверный проверочный код", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", "Не удалось отправить данные", "OK");
                        }
                    }
                    catch (Exception)
                    {
                        await DisplayAlert("Ошибка", "Не удалось отправить данные", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Нет интернет-соединения", "OK");
                }
                HideActivityIndicator();
            }
        }

        private char[] intArrayToChar(int[] arr)
        {
            char[] some = new char[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                some[i] = Convert.ToChar(arr[i].ToString());
            }

            return some;
        }

        void ShowActivityIndicator()
        {
            indicator.IsVisible = true;
        }

        void HideActivityIndicator()
        {
            indicator.IsVisible = false;
        }
    }
}