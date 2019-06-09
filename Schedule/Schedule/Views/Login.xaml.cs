using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
            LoadFaculties();
        }

        //переменные хранения выбранных значений
        string selectedFaculty = "";
        string selectedGroupId = "";
        string selectedGroupName = "";
        string selectedSubgroup = "";
        string selectedTeacher = "";
        bool isTeacher = false;

        //Загрузка факультетов, для отображения в списке факультетов
        List<string> faculties = new List<string>();
        void LoadFaculties()
        {
            foreach (var faculty in App.facultiesJSON)
            {
                if (!faculties.Contains(faculty.FacultyName))
                {
                    faculties.Add(faculty.FacultyName);
                }
            }
        }
        //Загрузка групп, для отображения в списке групп
        List<string> groups = new List<string>();
        void LoadGroups()
        {
            groups.Clear();
            foreach (var f in App.facultiesJSON)
            {
                if (f.FacultyName == selectedFaculty)
                {
                    foreach (var item in f.Groups)
                    {
                        groups.Add(item.GroupId + " | " + item.GroupName);
                    }
                }
            }
        }
        //Загрузка подгрупп, для отображения в списке групп
        List<string> subgroups = new List<string>();
        void LoadSubgroups()
        {
            subgroups.Clear();
            foreach (var f in App.facultiesJSON)
            {
                if (f.FacultyName == selectedFaculty)
                {
                    foreach (var g in f.Groups)
                    {
                        if (g.GroupId == selectedGroupId && g.GroupName == selectedGroupName)
                        {
                            foreach (var s in g.Couples)
                            {
                                if (s.SubgroupName == null)
                                    break;
                                if (!subgroups.Contains(s.SubgroupName))
                                {
                                    subgroups.Add(s.SubgroupName);
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }
        //Загрузка преподавателей, для отображения в списке преподавателей
        List<string> teachers = new List<string>();
        void LoadTeachers()
        {
            foreach (var f in App.facultiesJSON)
            {
                if (f.FacultyName == selectedFaculty)
                {
                    foreach (var g in f.Groups)
                    {
                        foreach (var s in g.Couples)
                        {
                            //Если в строке преподавателя есть запятая, то это английский, то есть три преподавателя
                            if (s.CoupleTeacher.Contains(',')) 
                            {
                                //поэтому их нужно разделить
                                string[] someTeachers = s.CoupleTeacher.Split(',');
                                foreach (var item in someTeachers)
                                {
                                    if (!teachers.Contains(item.Trim()))
                                    {
                                        teachers.Add(item.Trim());
                                    }
                                }
                            }
                            else if (!teachers.Contains(s.CoupleTeacher))
                            {
                                teachers.Add(s.CoupleTeacher);
                            }
                        }

                    }
                    break;
                }
            }
            //в файлах рейтинга
            foreach (var f in App.facultiesJSONRaiting)
            {
                if (f.FacultyName == selectedFaculty)
                {
                    foreach (var g in f.Groups)
                    {
                        foreach (var s in g.Couples)
                        {
                            //Если в строке преподавателя есть запятая, то это английский, то есть три преподавателя
                            if (s.CoupleTeacher.Contains(','))
                            {
                                //поэтому их нужно разделить
                                string[] someTeachers = s.CoupleTeacher.Split(',');
                                foreach (var item in someTeachers)
                                {
                                    if (!teachers.Contains(item.Trim()))
                                    {
                                        teachers.Add(item.Trim());
                                    }
                                }
                            }
                            else if (!teachers.Contains(s.CoupleTeacher))
                            {
                                teachers.Add(s.CoupleTeacher);
                            }
                        }

                    }
                    break;
                }
            }

            teachers.Sort();
        }


        Label headerForPicker;
        Picker picker;

        //изменение поля с выбором типа пользователя
        void PickerUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedGroupName = ""; //обнуляем переменные хранения значений
            selectedGroupId = ""; //обнуляем переменные хранения значений
            selectedTeacher = "";
            selectedSubgroup = "";
            Picker workPicker = (Picker)sender;
            if (workPicker.SelectedIndex == 0) //Если выбран студент
            {
                isTeacher = false;
            }
            else isTeacher = true;

            headerForPicker = new Label
            {
                Text = "Выберите факультет:",
                TextColor = Color.FromRgb(38, 38, 38),
                Margin = new Thickness(10, 0, 0, 0),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            picker = new Picker
            {
                Title = ""

            };
            foreach (var item in faculties)
            {
                picker.Items.Add(item);
            }

            picker.SelectedIndexChanged += PickerFaculty_SelectedIndexChanged;

            SelectFacultyStackLoyaout.Children.Clear();
            SelectGroupStackLoyaout.Children.Clear();
            SelectSubgroupStackLoyaout.Children.Clear();

            SelectFacultyStackLoyaout.Children.Add(headerForPicker);
            SelectFacultyStackLoyaout.Children.Add(picker);

        }
        //изменение поля с выбором факультета
        void PickerFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            selectedFaculty = pic.SelectedItem.ToString(); //сохранение группы

            LoadGroups();

            if (!isTeacher) //Если выбран студент
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

                picker.SelectedIndexChanged += PickerGroup_SelectedIndexChanged;


            }
            else //Если выбран преподаватель
            {
                LoadTeachers();

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

                picker.SelectedIndexChanged += PickerTeacher_SelectedIndexChanged;

            }

            SelectGroupStackLoyaout.Children.Clear();
            SelectSubgroupStackLoyaout.Children.Clear();

            SelectGroupStackLoyaout.Children.Add(headerForPicker);
            SelectGroupStackLoyaout.Children.Add(picker);

        }
        //изменение поля с выбором группы
        void PickerGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            string[] gr = pic.SelectedItem.ToString().Split('|');
            selectedGroupId = gr[0].TrimEnd(); //сохранение номера группы
            selectedGroupName = gr[1].TrimStart(); //сохранение имени группы

            LoadSubgroups();

            if (subgroups.Count > 0)
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

                foreach (var item in subgroups)
                {
                    picker.Items.Add(item);
                }

                picker.SelectedIndexChanged += PickerSubgroup_SelectedIndexChanged;

                SelectSubgroupStackLoyaout.Children.Clear();
                SelectSubgroupStackLoyaout.Children.Add(headerForPicker);
                SelectSubgroupStackLoyaout.Children.Add(picker);
            }
            else
            {
                SelectSubgroupStackLoyaout.Children.Clear();
                //Если нет подгруппы показываем кнопку сохранения
                saveButton.Clicked += OnSavebuttonClick;
                StackLoyaoutForSavebutton.Children.Clear();
                StackLoyaoutForSavebutton.Children.Add(saveButton);
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
        void PickerTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            selectedTeacher = pic.SelectedItem.ToString();
            saveButton.Clicked += OnSavebuttonClick;
            StackLoyaoutForSavebutton.Children.Clear();
            StackLoyaoutForSavebutton.Children.Add(saveButton);
        }

        //изменение поля с выбором подгруппы
        void PickerSubgroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            selectedSubgroup = pic.SelectedItem.ToString();
            saveButton.Clicked += OnSavebuttonClick;
            StackLoyaoutForSavebutton.Children.Clear();
            StackLoyaoutForSavebutton.Children.Add(saveButton);
        }


        //Нажатие на кнопку Сохранить
        void OnSavebuttonClick(object sender, EventArgs e)
        {
            //проверка, так как если выбрать факультет, 
            //а потом поменять его на другой и нажать на Сохранить, 
            //программа снова заходит в этот метод и пытается добавить данный ключ
            if (!App.Current.Properties.ContainsKey("facultyName")) 
            {
                if (selectedFaculty != "")
                {
                    App.Current.Properties.Add("facultyName", selectedFaculty);

                    if (selectedGroupId != "")
                    {
                        //Сохранение данных в словарь App.Current.Properties
                        App.Current.Properties.Add("groupId", selectedGroupId);
                        App.Current.Properties.Add("groupName", selectedGroupName);
                        //получение наименования группы (специальность) (нужна для вывода в меню)
                        foreach (var f in App.facultiesJSON)
                        {
                            if (f.FacultyName == selectedFaculty)
                            {
                                foreach (var g in f.Groups)
                                {
                                    if (g.GroupId == selectedGroupId && g.GroupName == selectedGroupName)
                                    {
                                        App.Current.Properties.Add("groupIdName", selectedGroupId + " | " + selectedGroupName);
                                    }
                                }
                            }
                        }

                        if (selectedSubgroup != "")
                        {
                            App.Current.Properties.Add("subgroup", selectedSubgroup);
                        }

                        App.Current.Properties.Add("isTeacher", false);

                        //Сохранение графика
                        int indexOfDigit = 0;
                        for (int i = 0; i < selectedGroupName.Length; i++)//вырезаем код специальности, чтобы по нему искать
                        {
                            if (Char.IsDigit(selectedGroupName[i]))
                            {
                                indexOfDigit = i;
                                break;
                            }
                        }
                        string code = selectedGroupName.Substring(indexOfDigit, 8);
                        string course = selectedGroupId[0].ToString();

                        foreach (var item in App.timetable)
                        {
                            if (item.SpecialtyName.Contains(code))
                            {
                                foreach (var courses in item.Courses)
                                {
                                    if (courses.CourseNumber == course)
                                    {
                                        string json = JsonConvert.SerializeObject(courses.Days);
                                        App.Current.Properties.Add("timetable", json);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        App.Current.Properties.Add("isTeacher", true);
                        App.Current.Properties.Add("teacherName", selectedTeacher);

                        //сохранение графика первой попавшейся группы, для определения номера недели для Преподавателя
                        foreach (var item in App.timetable)
                        {
                            foreach (var courses in item.Courses)
                            {
                                string json = JsonConvert.SerializeObject(courses.Days);
                                App.Current.Properties.Add("timetable", json);
                                break;
                            }
                            break;
                        }
                    }
                }

                //Определение номера недели
                App.Current.Properties.Add("numOfWeek", "1");
                string table = "";
                List<Day> myTimetable = new List<Day>();
                if (App.Current.Properties.TryGetValue("timetable", out object tableFrom))
                {
                    table = (string)tableFrom;
                    myTimetable = JsonConvert.DeserializeObject<List<Day>>(table);
                }

                DateTime now = DateTime.Now;
                int day = now.Day;
                int month = now.Month;
                foreach (var item in myTimetable)
                {
                    if (item.ThisDay == day && item.ThisMonth == month)
                    {
                        if (item.ThisWeek % 2 == 0)
                        {
                            App.Current.Properties["numOfWeek"] = "2";
                        }
                        else
                            App.Current.Properties["numOfWeek"] = "1";
                        break;
                    }
                }
                App.Current.MainPage = new MasterDetailPage1();
            }
        }
    }
}