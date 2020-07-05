using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

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

        public async Task SendEmail(string subject, string body, List<string> recipients)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                await DisplayAlert("Ошибка", "Не поддерживается на вашем устройстве. {" + fbsEx.Message + "} \n Может у вас просто не установлено ни одно почтвое приложение?", "OK");
            }
            catch (Exception)
            {
                await DisplayAlert("Ошибка", "По неведомым для нас причинам произошла ошибка... \n Вы можете написать нам на почту sschedule@inbox.ru. ", "OK");
            }
        }
    }
}