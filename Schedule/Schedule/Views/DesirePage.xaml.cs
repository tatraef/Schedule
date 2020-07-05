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

namespace Schedule.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesirePage : ContentPage
    {
        public DesirePage()
        {
            InitializeComponent();
        }

        private char[] Monday = new char[] { '1', '1', '1', '1', '1' };
        private char[] Tuesday = new char[] { '1', '1', '1', '1', '1' };
        private char[] Wednesday = new char[] { '1', '1', '1', '1', '1' };
        private char[] Thursday = new char[] { '1', '1', '1', '1', '1' };
        private char[] Friday = new char[] { '1', '1', '1', '1', '1' };
        private char[] Saturday = new char[] { '1', '1', '1', '1', '1' };

        private void MondayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox) sender;
                Monday[checkBox.TabIndex] = '1';
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                Monday[checkBox.TabIndex] = '0';
            }
        }

        private void TuesdayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                Tuesday[checkBox.TabIndex] = '1';
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                Tuesday[checkBox.TabIndex] = '0';
            }
        }

        private void WednesdayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                Wednesday[checkBox.TabIndex] = '1';
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                Wednesday[checkBox.TabIndex] = '0';
            }
        }

        private void ThursdayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                Thursday[checkBox.TabIndex] = '1';
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                Thursday[checkBox.TabIndex] = '0';
            }
        }

        private void FridayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                Friday[checkBox.TabIndex] = '1';
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                Friday[checkBox.TabIndex] = '0';
            }
        }

        private void SaturdayCheckBox_Changed(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                CheckBox checkBox = (CheckBox)sender;
                Saturday[checkBox.TabIndex] = '1';
            }
            else
            {
                CheckBox checkBox = (CheckBox)sender;
                Saturday[checkBox.TabIndex] = '0';
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
                Monday = new String(Monday),
                Tuesday = new String(Tuesday),
                Wednesday = new String(Wednesday),
                Thursday = new String(Thursday),
                Friday = new String(Friday),
                Saturday = new String(Saturday),
                Message = MessageEditor.Text
            };

            if (App.Current.Properties.TryGetValue("teacherName", out object tname))
            {
                var name = ((string)tname).Split(' ')[0];
                var json = JsonConvert.SerializeObject(desire);

                var urlContent = "updateDesire=some&name=" + name + "&code=" + CodeEntry.Text.ToString() + "&json=" + json;

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