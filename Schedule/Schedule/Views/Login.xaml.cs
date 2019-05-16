using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Schedule.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Schedule.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login : ContentPage
	{
		public Login ()
		{
			InitializeComponent ();
            loadGroups();
            loadTeachers();
        }

        //переменные хранения выбранных значений
        string selectedGroup = "";
        string selectedGroupName = "";
        string selectedSubgroup = "";
        string selectedTeacher = "";

        //Загрузка групп, для отображения в списке групп
        List<string> groups = new List<string>();
        void loadGroups()
        {
            foreach (KeyValuePair<string, Group> group in App.sched)
            {
                groups.Add(group.Value.groupId);
            }
        }
        //Загрузка преподавателей, для отображения в списке преподавателей
        List<string> teachers = new List<string>();
        void loadTeachers()
        {
            //GetManifestResourceStream используется для доступа к внедренному файлу, 
            //путь определяется через Assembly
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(DayMonday)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Schedule.data.json");

            using (var reader = new System.IO.StreamReader(stream))
            {
                string[] json = reader.ReadToEnd().Split('"'); //разделение файла json на кавычку, чтобы найти поле coupleTeacher
                for (int i = 0; i < json.Length; i++)
                {
                    if (json[i] == "coupleTeacher")
                    {
                        string teacher = json[i + 2];
                        if (teacher.Contains(',')) //Если в строке преподавателя есть запятая, то это английский, то есть три преподавателя
                        {
                            //поэтому их нужно разделить
                            string[] someTeachers = teacher.Split(',');
                            foreach (var item in someTeachers)
                            {
                                if (!teachers.Contains(item.Trim()))
                                {
                                    teachers.Add(item.Trim());
                                }
                            }
                        }
                        else
                        if (!teachers.Contains(teacher))
                        {
                            teachers.Add(teacher);
                        }
                    }
                }
            }

            teachers.Sort();
        }


        Label headerForPicker;
        Picker picker;

        //изменение поля с выбором типа пользователя
        void pickerUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedGroup = ""; //обнуляем переменные хранения значений
            selectedTeacher = "";
            Picker workPicker = (Picker)sender;
            if (workPicker.SelectedIndex == 0) //Если выбран студент
            {
                headerForPicker = new Label
                {
                    Text = "Выберите группу:",
                    TextColor = Color.FromRgb(38, 38, 38),
                    Margin = new Thickness(10, 0, 0, 0),
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };

                picker = new Picker
                {
                    Title = ""

                };
                foreach (var item in groups)
                {
                    picker.Items.Add(item);
                }

                picker.SelectedIndexChanged += pickerGroup_SelectedIndexChanged;


            }
            else if (workPicker.SelectedIndex == 1) //Если выбран преподаватель
            {
                headerForPicker = new Label
                {
                    Text = "Выберите преподавателя:",
                    TextColor = Color.FromRgb(38, 38, 38),
                    Margin = new Thickness(10, 0, 0, 0),
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };

                picker = new Picker
                {
                    Title = ""

                };
                foreach (var item in teachers)
                {
                    picker.Items.Add(item);
                }

                picker.SelectedIndexChanged += pickerTeacher_SelectedIndexChanged;

            }

            SelectGroupStackLoyaout.Children.Clear();
            SelectSubgroupStackLoyaout.Children.Clear();

            SelectGroupStackLoyaout.Children.Add(headerForPicker);
            SelectGroupStackLoyaout.Children.Add(picker);

        }
        //изменение поля с выбором группы
        void pickerGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            selectedGroup = pic.SelectedItem.ToString(); //сохранение группы
            string selectedGroupId = pic.SelectedItem.ToString();

            foreach (KeyValuePair<string, Group> group in App.sched)
            {
                if (group.Value.groupId == selectedGroupId)
                {
                    if (group.Value.secondSubgroup != null)
                    {
                        headerForPicker = new Label
                        {
                            Text = "Выберите подгруппу:",
                            TextColor = Color.FromRgb(38, 38, 38),
                            Margin = new Thickness(10, 0, 0, 0),
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                        };

                        picker = new Picker
                        {
                            Title = "Подгруппа"

                        };

                        picker.Items.Add(group.Value.firstSubgroup.subgroupName);
                        picker.Items.Add(group.Value.secondSubgroup.subgroupName);

                        picker.SelectedIndexChanged += pickerSubgroup_SelectedIndexChanged;

                        SelectSubgroupStackLoyaout.Children.Clear();
                        SelectSubgroupStackLoyaout.Children.Add(headerForPicker);
                        SelectSubgroupStackLoyaout.Children.Add(picker);
                    }
                    else
                    {
                        SelectSubgroupStackLoyaout.Children.Clear();
                        //Если нет подгруппы показываем кнопку сохранения
                        saveButton.Clicked += onSavebuttonClick;
                        StackLoyaoutForSavebutton.Children.Add(saveButton);
                    }

                    break;
                }
            }
        }

        //Кнопка Сохранить
        Button saveButton = new Button
        {
            Text = "Сохранить",
            TextColor = Color.FromRgb(38, 38, 38),
            Margin = new Thickness(10),
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            BackgroundColor = Color.FromRgb(235, 179, 13)
        };

        //изменение поля с выбором преподавателя
        void pickerTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            selectedTeacher = pic.SelectedItem.ToString();
            saveButton.Clicked += onSavebuttonClick;
            StackLoyaoutForSavebutton.Children.Add(saveButton);
        }

        //изменение поля с выбором подгруппы
        void pickerSubgroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            if (pic.SelectedIndex == 0) selectedSubgroup = "firstSubgroup";
            else selectedSubgroup = "secondSubgroup";
            saveButton.Clicked += onSavebuttonClick;
            StackLoyaoutForSavebutton.Children.Add(saveButton);
        }


        //Нажатие на кнопку Сохранить
        void onSavebuttonClick(object sender, EventArgs e)
        {
            if (selectedGroup != "")
            {
                //Сохранение данных в словарь App.Current.Properties
                App.Current.Properties.Add("groupId", selectedGroup);
                //получение наименования группы (специальность) (нужна для вывода в меню)
                foreach (KeyValuePair<string, Group> group in App.sched)
                {
                    if (group.Value.groupId == selectedGroup)
                    {
                        App.Current.Properties.Add("groupName", selectedGroup + " | " + group.Value.groupName);
                    }
                }

                if (selectedSubgroup != "")
                {
                    App.Current.Properties.Add("subgroup", selectedSubgroup);
                }
                else App.Current.Properties.Add("subgroup", "firstSubgroup");

                App.Current.Properties.Add("isTeacher", false);
            }
            else
            {
                App.Current.Properties.Add("isTeacher", true);
                App.Current.Properties.Add("teacherName", selectedTeacher);
            }

            App.Current.Properties.Add("numOfWeek", "1");
            App.Current.MainPage = new MasterDetailPage1();
        }
    }
}