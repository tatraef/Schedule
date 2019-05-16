using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            changeUserName();
            base.OnAppearing();
        }

        class MasterDetailPage1MasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterDetailPage1MenuItem> MenuItems { get; set; }
            
            public MasterDetailPage1MasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterDetailPage1MenuItem>(new[]
                {
                    new MasterDetailPage1MenuItem { Id = 0, Title = "Таймлайн"},
                    new MasterDetailPage1MenuItem { Id = 1, Title = "Расписание", TargetType = typeof(ScheduleTabbedPage)},
                    new MasterDetailPage1MenuItem { Id = 2, Title = "Page 3" },
                    new MasterDetailPage1MenuItem { Id = 3, Title = "Page 4" },
                    new MasterDetailPage1MenuItem { Id = 4, Title = "Page 5" },
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
        public void changeUserName()
        {
            object name = "";
            if (App.Current.Properties.TryGetValue("groupIdName", out name))
            {
                userName.Text = (string)name;
            }
            else if (App.Current.Properties.TryGetValue("teacherName", out name))
            {
                userName.Text = (string)name;
            }
        }

        private void exit_Clicked(object sender, SelectedItemChangedEventArgs e)
        {
            App.Current.Properties.Clear();

            App.Current.MainPage = new Login();
        }
    }
}