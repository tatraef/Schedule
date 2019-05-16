using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Schedule.ViewModels
{
    class SelectUser : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand pickerUserType_ChangedCommand { protected set; get; }

        public SelectUser()
        {
            pickerUserType_ChangedCommand = new Command(pickerUserType_Changed);
        }



        Label headerGroup;
        Picker pickerGroup;

        void pickerUserType_Changed(object sender)
        {
            headerGroup = new Label
            {
                Text = "Выберите группу",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };

            pickerGroup = new Picker
            {
                Title = "Группа",
            };
            pickerGroup.Items.Add("C#");
            pickerGroup.Items.Add("JavaScript");
            pickerGroup.Items.Add("Java");
            pickerGroup.Items.Add("PHP");

            /*pickerGroup. += pickerUserType_SelectedIndexChanged;

            SelectUserStackLoyaout.Children.Add(headerGroup);
            SelectUserStackLoyaout.Children.Add(pickerGroup);*/
        }
    }
}
