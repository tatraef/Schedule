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
    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();
        }

        private async void FeedbackButton_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameEntry.Text) || 
                string.IsNullOrEmpty(SubjectEntry.Text) || 
                string.IsNullOrEmpty(MessageEditor.Text))
            {
                await DisplayAlert("Ошибка", "Все поля обязательны для заполнения!", "OK");
            }
            else
            {
                await SendEmail(SubjectEntry.Text, MessageEditor.Text + "/n/n" + NameEntry.Text, new List<string> { "sschedule@inbox.ru" });
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
                await DisplayAlert("Ошибка", "Не поддерживается на вашем устройстве. {" + fbsEx.Message + "} /n Может у вас просто не установлено ни одно почтвое приложение?", "OK");
            }
            catch (Exception)
            {
                await DisplayAlert("Ошибка", "По неведомым для нас причинам произошла ошибка... /n Вы можете написать нам на почту sschedule@inbox.ru. ", "OK");
            }
        }
    }
}