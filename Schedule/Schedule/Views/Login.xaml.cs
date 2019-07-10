using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
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
        }

        //переменные хранения выбранных значений
        string selectedFaculty = "";
        string selectedGroupId = "";
        string selectedGroupName = "";
        string selectedSubgroup = "";
        string selectedTeacher = "";
        bool isTeacher = false;

        //Загрузка факультетов с сервера, для отображения в списке факультетов
        public async Task<List<String>> LoadFacultiesAsync()
        {
            HttpContent content = new StringContent("getFaculties", Encoding.UTF8, "application/x-www-form-urlencoded");
            string res = await LoadDataFromServer(content);
            return JsonConvert.DeserializeObject<List<String>>(res);
        }

        //Загрузка групп, для отображения в списке групп
        public async Task<List<String>> LoadGroupsAsync()
        {
            HttpContent content = new StringContent("getScheduleMain&name="+selectedFaculty, Encoding.UTF8, "application/x-www-form-urlencoded");
            string res = await LoadDataFromServer(content);
            //сохранение полученного расписания и даты
            List<string> some = JsonConvert.DeserializeObject<List<string>>(res);
            App.Current.Properties["scheduleMain"] = some[0];
            App.Current.Properties["updateMain"] = some[1];
            App.facultiesMain.Clear();
            App.facultiesMain.Add(JsonConvert.DeserializeObject<Faculty>(some[0]));

            List<string> groups = new List<string>();

            foreach (var f in App.facultiesMain)
            {
                if (f.FacultyName == selectedFaculty)
                {
                    foreach (var item in f.Groups)
                    {
                        groups.Add(item.GroupId + " | " + item.GroupName);
                    }
                }
            }
            return groups;
        }

        //Загрузка преподавателей, для отображения в списке преподавателей
        public async Task<List<String>> LoadTeachersAsync()
        {
            HttpContent content = new StringContent("getScheduleMain&name=" + selectedFaculty, Encoding.UTF8, "application/x-www-form-urlencoded");
            string res = await LoadDataFromServer(content);
            List<string> some = JsonConvert.DeserializeObject<List<string>>(res);
            App.facultiesMain.Clear();
            App.facultiesMain.Add(JsonConvert.DeserializeObject<Faculty>(some[0]));

            List<string> teachers = new List<string>();

            foreach (var f in App.facultiesMain)
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

            App.facultiesMain.Clear(); //отчистка, т.к. далее будут загружаться все факультеты разом

            teachers.Sort();
            return teachers;
        }

        //Загрузка подгрупп, для отображения в списке групп
        List<string> subgroups = new List<string>();
        void LoadSubgroups()
        {
            subgroups.Clear();
            foreach (var f in App.facultiesMain)
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

        
        Label headerForPicker;
        Picker picker;

        //изменение поля с выбором типа пользователя
        async void PickerUserType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Отчистка полей, если они, вдруг, заполнены
            selectFacultyStackLoyaout.Children.Clear();
            selectGroupStackLoyaout.Children.Clear();
            selectSubgroupStackLoyaout.Children.Clear();

            List<string> faculties = new List<string>();

            //загрузка факультетов с сервера
            ShowActivityIndicator(); //показать анимацию загрузки
            //Ожидается выполнение асинхронного метода, так как данные уже нужны
            faculties = await LoadFacultiesAsync();
            HideActivityIndicator(); //скрыть анимацию загрузки

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

            selectFacultyStackLoyaout.Children.Add(headerForPicker);
            selectFacultyStackLoyaout.Children.Add(picker);

        }

        //изменение поля с выбором факультета
        async void PickerFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            selectedFaculty = pic.SelectedItem.ToString(); //сохранение факультета

            //Если выбран студент
            if (!isTeacher) 
            {
                List<string> groups = new List<string>();

                ShowActivityIndicator(); //показать анимацию загрузки
                //Ожидается выполнение асинхронного метода, так как данные уже нужны
                groups = await LoadGroupsAsync();
                HideActivityIndicator(); //скрыть анимацию загрузки

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
            //Если выбран преподаватель
            else
            {
                List<string> teachers = new List<string>();

                ShowActivityIndicator(); //показать анимацию загрузки
                //Ожидается выполнение асинхронного метода, так как данные уже нужны
                teachers = await LoadTeachersAsync();
                HideActivityIndicator(); //скрыть анимацию загрузки

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

            selectGroupStackLoyaout.Children.Clear();
            selectSubgroupStackLoyaout.Children.Clear();

            selectGroupStackLoyaout.Children.Add(headerForPicker);
            selectGroupStackLoyaout.Children.Add(picker);

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

                selectSubgroupStackLoyaout.Children.Clear();
                selectSubgroupStackLoyaout.Children.Add(headerForPicker);
                selectSubgroupStackLoyaout.Children.Add(picker);
            }
            else
            {
                selectSubgroupStackLoyaout.Children.Clear();
                //Если нет подгруппы показываем кнопку сохранения
                saveButton.Clicked += OnSavebuttonClick;
                stackLoyaoutForSavebutton.Children.Clear();
                stackLoyaoutForSavebutton.Children.Add(saveButton);
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
            stackLoyaoutForSavebutton.Children.Clear();
            stackLoyaoutForSavebutton.Children.Add(saveButton);
        }

        //изменение поля с выбором подгруппы
        void PickerSubgroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker pic = (Picker)sender;
            selectedSubgroup = pic.SelectedItem.ToString();
            saveButton.Clicked += OnSavebuttonClick;
            stackLoyaoutForSavebutton.Children.Clear();
            stackLoyaoutForSavebutton.Children.Add(saveButton);
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
                        foreach (var f in App.facultiesMain)
                        {
                            if (f.FacultyName == selectedFaculty)
                            {
                                foreach (var g in f.Groups)
                                {
                                    if (g.GroupId == selectedGroupId && g.GroupName == selectedGroupName)
                                    {
                                        App.Current.Properties.Add("groupIdName", selectedGroupId + " | " + selectedGroupName);
                                        break;
                                    }
                                }
                            }
                        }

                        if (selectedSubgroup != "")
                        {
                            App.Current.Properties.Add("subgroup", selectedSubgroup);
                        }

                        App.Current.Properties.Add("isTeacher", false);

                        //Сохранение данных для дальнейшего получения графика
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
                        App.Current.Properties.Add("code", code);
                        string course = selectedGroupId[0].ToString();
                        App.Current.Properties.Add("course", course);
                    }
                    else
                    {
                        App.Current.Properties.Add("isTeacher", true);
                        App.Current.Properties.Add("teacherName", selectedTeacher);

                        //******************************here****************************//
                        //сохранение графика первой попавшейся группы, для определения номера недели для Преподавателя
                        foreach (var item in App.timetable)
                        {
                            foreach (var courses in item.Courses)
                            {
                                string json = JsonConvert.SerializeObject(courses.Days);
                                App.Current.Properties.Add("myTimetable", json);
                                break;
                            }
                            break;
                        }
                    }
                }
                App.Current.Properties.Add("numOfWeek", "1");

                App.justLogged = true;
                App.Current.MainPage = new MasterDetailPage1();
            }
        }

        //Загрузка данных с сервера
        public async Task<String> LoadDataFromServer(HttpContent content)
        {
            string result = "";

            if (CrossConnectivity.Current.IsConnected == true)
            {
                string url = "http://192.168.0.113/schedule/getAnswer.php";

                try
                {
                    HttpClient client = new HttpClient
                    {
                        BaseAddress = new Uri(url)
                    };
                    var response = await client.PostAsync(client.BaseAddress, content);
                    response.EnsureSuccessStatusCode(); // выброс исключения, если произошла ошибка

                    result = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", "Не удалось получить данные, ошибка: " + ex.Message, "ОK");
                }
            }
            else
            {
                await DisplayAlert("Внимание", "Нет интернет-соединения, невозможно загрузить данные.", "Понятно");
            }

            return result;
        }

        void ShowActivityIndicator()
        {
            facultyIndicator.IsVisible = true;
            loadingLabel.IsVisible = true;
        }

        void HideActivityIndicator()
        {
            facultyIndicator.IsVisible = false;
            loadingLabel.IsVisible = false;
        }
    }
}