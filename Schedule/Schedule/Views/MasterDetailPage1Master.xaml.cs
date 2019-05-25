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

        class MasterDetailPage1MasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterDetailPage1MenuItem> MenuItems { get; set; }
            
            public MasterDetailPage1MasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterDetailPage1MenuItem>(new[]
                {
                    new MasterDetailPage1MenuItem { Id = 0, Title = "Таймлайн", Icon = "clock.png", TargetType = typeof(TimelinePage)},
                    new MasterDetailPage1MenuItem { Id = 1, Title = "Расписание", Icon = "calendar.png", TargetType = typeof(ScheduleTabbedPage)},
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
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

        private void Exit_Clicked(object sender, SelectedItemChangedEventArgs e)
        {
            App.Current.Properties.Clear();

            App.Current.MainPage = new Login();
            
            /*загрузка графика, необходимо загружать здесь, так как если пользователь выйдет, данного файла не будет
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(DayMonday)).Assembly;
            App.timetable = new List<Specialty>();
            Stream stream2 = assembly.GetManifestResourceStream("Schedule.timetable.json");
            using (var reader = new System.IO.StreamReader(stream2))
            {
                string json = reader.ReadToEnd();
                App.timetable = JsonConvert.DeserializeObject<List<Specialty>>(json);
            }*/
        }
    }
}