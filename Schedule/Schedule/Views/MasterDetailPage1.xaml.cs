using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Schedule.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetailPage1 : MasterDetailPage
    {
        public MasterDetailPage1()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;

            Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(MasterDetailPage1Detail))) { BarBackgroundColor = Color.FromRgb(38, 38, 38), BarTextColor = Color.FromRgb(235, 179, 13) };
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterDetailPage1MenuItem;
            if (item == null)
                return;
             
            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page) { BarBackgroundColor = Color.FromRgb(38, 38, 38), BarTextColor = Color.FromRgb(235, 179, 13) };
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}