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
	public partial class DayWednesday : ContentPage
	{
		public DayWednesday()
		{
            InitializeComponent();
            DayViewModel bind = new DayViewModel("wednesday");
            BindingContext = bind;

            //проверяется студент или преподаватель
            if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
                if ((bool)isTeacher)
                {
                    couplesList.ItemsSource = bind.TeacherCoupleList;
                }
                else
                {
                    couplesList.ItemsSource = bind.Couples;
                }

        }
	}
}