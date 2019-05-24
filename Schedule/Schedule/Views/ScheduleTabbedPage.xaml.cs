using Schedule.ViewModels;
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
    public partial class ScheduleTabbedPage : TabbedPage
    {
        public ScheduleTabbedPage ()
        {
            ToolbarItem first = new ToolbarItem
            {
                Text = "Первая неделя",
                Order = ToolbarItemOrder.Secondary,
            };
            first.Clicked += (object sender, System.EventArgs e) =>
            {
                App.Current.Properties["numOfWeek"] = "1";
                this.Children.Clear();
                this.Children.Add(new DayMonday());
                this.Children.Add(new DayTuesday());
                this.Children.Add(new DayWednesday());
                this.Children.Add(new DayThursday());
                this.Children.Add(new DayFriday());
                this.Children.Add(new DaySaturday());
            };
            ToolbarItem second = new ToolbarItem
            {
                Text = "Вторая неделя",
                Order = ToolbarItemOrder.Secondary
            };
            second.Clicked += (object sender, System.EventArgs e) =>
            {
                App.Current.Properties["numOfWeek"] = "2";
                this.Children.Clear();
                this.Children.Add(new DayMonday());
                this.Children.Add(new DayTuesday());
                this.Children.Add(new DayWednesday());
                this.Children.Add(new DayThursday());
                this.Children.Add(new DayFriday());
                this.Children.Add(new DaySaturday());
            };
            ToolbarItems.Add(first);
            ToolbarItems.Add(second);
            InitializeComponent();
        }
    }
}