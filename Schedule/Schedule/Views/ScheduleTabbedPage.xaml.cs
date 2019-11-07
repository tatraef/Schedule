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
            ToolbarItem weeksItem = new ToolbarItem
            {
                Order = ToolbarItemOrder.Default,
                Text = "1 НЕДЕЛЯ"
            };
            weeksItem.Clicked += (object sender, System.EventArgs e) =>
            {
                if ((string) App.Current.Properties["numOfWeek"] == "1")
                {
                    weeksItem.Text = "2 НЕДЕЛЯ";
                    App.Current.Properties["numOfWeek"] = "2";
                }
                else
                {
                    weeksItem.Text = "1 НЕДЕЛЯ";
                    App.Current.Properties["numOfWeek"] = "1";
                }
                
                this.Children.Clear();
                this.Children.Add(new DayMonday());
                this.Children.Add(new DayTuesday());
                this.Children.Add(new DayWednesday());
                this.Children.Add(new DayThursday());
                this.Children.Add(new DayFriday());
                this.Children.Add(new DaySaturday());
            };
   
            ToolbarItems.Add(weeksItem);
            InitializeComponent();
        }
    }
}