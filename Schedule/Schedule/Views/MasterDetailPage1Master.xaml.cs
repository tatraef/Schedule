using Newtonsoft.Json;
using Schedule.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Schedule.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1Master : ContentPage
    {
        public ListView ListView;

        public MasterDetailPage1Master()
        {
            InitializeComponent();

            BindingContext = new MasterDetailPage1MasterViewModel();
            ListView = MenuItemsListView;
        }

        protected override void OnAppearing()
        {
            ChangeUserName();
            base.OnAppearing();
        }

        class MasterDetailPage1MasterViewModel
        {
            public ObservableCollection<MasterDetailPage1MenuItem> MenuItems { get; set; }
            
            public MasterDetailPage1MasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterDetailPage1MenuItem>(new[]
                {
                    new MasterDetailPage1MenuItem { Id = 0, Title = "Таймлайн", Icon = "clock.png", TargetType = typeof(TimelinePage)},
                    new MasterDetailPage1MenuItem { Id = 1, Title = "Расписание", Icon = "calendar.png", TargetType = typeof(ScheduleTabbedPage)},
                    new MasterDetailPage1MenuItem { Id = 2, Title = "Обратная связь", Icon = "mail.png", TargetType = typeof(ContactUsPage)},
                });
            }
        }

        //Изменяет имя пользователя, прописанное в меню
        public void ChangeUserName()
        {
            if (App.Current.Properties.TryGetValue("groupIdName", out object name))
            {
                userName.Text = (string)name;
            }
            else if (App.Current.Properties.TryGetValue("teacherName", out object tname))
            {
                userName.Text = (string)tname;
            }
        }

        private void ExitClicked(object sender, System.EventArgs e)
        {
            App.Current.Properties.Clear();

            App.Current.MainPage = new Login(); 
        }
    }
}