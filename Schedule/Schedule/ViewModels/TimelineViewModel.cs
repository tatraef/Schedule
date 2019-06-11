using Schedule.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace Schedule.ViewModels
{
    class TimelineViewModel
    {
        public List<TimelineItemForStudent> ItemsForStudents { get; set; }
        public List<TimelineItemForTeacher> ItemsForTeacher { get; set; }

        public TimelineViewModel()
        {
            #region Определение номера недели
            List<Day> myTimetable = new List<Day>();
            string table = "";
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
            #endregion

            if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
            {
                if ((bool)isTeacher)
                {
                    ItemsForTeacher = new List<TimelineItemForTeacher>();
                    ItemsForTeacher = GetDaysForTeacher();
                }
                else
                {
                    ItemsForStudents = new List<TimelineItemForStudent>();
                    ItemsForStudents = GetDaysForStudent();
                }
            }
        }
        //перегрузка для выбора определенной даты
        public TimelineViewModel(DateTime selectedDate)
        {
            #region Определение номера недели
            List<Day> myTimetable = new List<Day>();
            string table = "";
            if (App.Current.Properties.TryGetValue("timetable", out object tableFrom))
            {
                table = (string)tableFrom;
                myTimetable = JsonConvert.DeserializeObject<List<Day>>(table);
            }

            int day = selectedDate.Day;
            int month = selectedDate.Month;
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
            #endregion

            if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
            {
                if ((bool)isTeacher)
                {
                    ItemsForTeacher = new List<TimelineItemForTeacher>
                    {
                        MakeDayForTeacher(selectedDate, true)
                    };
                }
                else
                {
                    ItemsForStudents = new List<TimelineItemForStudent>
                    {
                        MakeDayForStudent(selectedDate, true)
                    };
                }
            }
        }

        public List<TimelineItemForStudent> GetDaysForStudent()
        {
            List<TimelineItemForStudent> lines = new List<TimelineItemForStudent>();

            for (int i = 0; i < 7; i++)
            {
                lines.Add(MakeDayForStudent(DateTime.Now.AddDays(i), false));
            }

            return lines;
        }

        public List<TimelineItemForTeacher> GetDaysForTeacher()
        {
            List<TimelineItemForTeacher> lines = new List<TimelineItemForTeacher>();

            for (int i = 0; i < 7; i++)
            {
                lines.Add(MakeDayForTeacher(DateTime.Now.AddDays(i), false));
            }

            return lines;
        }

        //isItForOne - для определения откуда пришел запрос, если с загрузки определенного дня, 
        //то не нужно проверять на проход через недели
        public TimelineItemForTeacher MakeDayForTeacher(DateTime NeedDate, bool isItForOne) 
        {
            DateTime now = DateTime.Now;
            Dictionary<byte, TeacherCouple> teacherCouples = new Dictionary<byte, TeacherCouple>();
            List<TeacherCouple> teacherCoupleList = new List<TeacherCouple>();

            string dt = NeedDate.DayOfWeek.ToString().ToLower();

            string numOfWeek = "";
            if (App.Current.Properties.TryGetValue("numOfWeek", out object num))
            {
                numOfWeek = (string)num;
            }

            //Если прошел переход через неделю, то есть открыл страницу в четверг,
            //а загружаются пары для Вторника уже новой недели, первая неделя сменилась второй
            if (!isItForOne && NeedDate.DayOfWeek < now.DayOfWeek)
            {
                if (numOfWeek == "1")
                    numOfWeek = "2";
                else
                    numOfWeek = "1";
            }

            //проверяется имя преподавателя
            if (App.Current.Properties.TryGetValue("teacherName", out object AppTeacherName))
            {
                string thisTeacher = (string)AppTeacherName;
                foreach (var f in App.facultiesJSON)
                {
                    foreach (var g in f.Groups)
                    {
                        foreach (var c in g.Couples)
                        {
                            //Contains, так как в паре английского может быть несколь преподавателей
                            if (c.CoupleTeacher.Contains(thisTeacher))
                            {
                                if (c.Week == numOfWeek)
                                {
                                    if (c.Day == dt)
                                    {
                                        //проверка, чтобы не показывать уже завершенные пары
                                        if (NeedDate.Day == now.Day)
                                        {
                                            string[] s = c.TimeEnd.Split(':');
                                            int h = Convert.ToInt32(s[0]);
                                            int m = Convert.ToInt32(s[1]);
                                            TimeSpan t = new TimeSpan(h, m, 0);
                                            if (t < now.TimeOfDay)
                                            {
                                                continue;
                                            }
                                        }
                                        #region Проверка в графике, учится ли данная группа, если да, то пара добавляется в коллекцию
                                            int indexOfDigit = 0;
                                            for (int i = 0; i < g.GroupName.Length; i++)//вырезаем код специальности, чтобы по нему искать
                                            {
                                                if (Char.IsDigit(g.GroupName[i]))
                                                {
                                                    indexOfDigit = i;
                                                    break;
                                                }
                                            }

                                            string code = g.GroupName.Substring(indexOfDigit, 8);
                                            string course = g.GroupId[0].ToString();

                                            foreach (var item in App.timetable)
                                            {
                                                if (item.SpecialtyName.Contains(code))
                                                {
                                                    foreach (var courses in item.Courses)
                                                    {
                                                        if (courses.CourseNumber == course)
                                                        {
                                                            int day = NeedDate.Day;
                                                            int month = NeedDate.Month;
                                                            for (int j = 0; j < courses.Days.Count; j++)
                                                            {
                                                                if (courses.Days[j].ThisDay == day && courses.Days[j].ThisMonth == month)
                                                                {
                                                                    if (courses.Days[j].Content == null && j + 6 < courses.Days.Count && courses.Days[j + 6].Content != "Э" && courses.Days[j].ThisWeek - 24 != 9) //change 24
                                                                    {
                                                                        byte coupleNum = Convert.ToByte(c.CoupleNum);

                                                                        if (!teacherCouples.ContainsKey(coupleNum))
                                                                        {
                                                                            teacherCouples.Add(coupleNum, new TeacherCouple(c, g.GroupId + (c.SubgroupId != null ? "(" + c.SubgroupId + ")" : "")));
                                                                        }
                                                                        else
                                                                        {
                                                                            teacherCouples[coupleNum].CoupleTeacher += ", " + g.GroupId + (c.SubgroupId != null ? "(" + c.SubgroupId + ")" : "");
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        #endregion  
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //сортировка пар, так как могут находится не в правильном порядке
            if (teacherCouples != null)
            {
                SortedDictionary<byte, TeacherCouple> sortedTeacherCouples = new SortedDictionary<byte, TeacherCouple>(teacherCouples);

                LoadRaitingForTeacher(sortedTeacherCouples, NeedDate);

                teacherCoupleList = sortedTeacherCouples.Values.ToList();
            }

            TimelineItemForTeacher nowaday = new TimelineItemForTeacher
            {
                ThisDate = NeedDate
            };

            if (teacherCoupleList.Count == 0)
            {
                TeacherCouple some = new TeacherCouple
                {
                    CoupleName = "Ничего интересного..."
                };
                teacherCoupleList.Add(some);
            }

            nowaday.AddRange(teacherCoupleList);
            return nowaday;

            
        }

        public TimelineItemForStudent MakeDayForStudent(DateTime NeedDate, bool isItForOne)
        {
            List<Couple> couples = new List<Couple>();

            WhatTodayForStudent(couples, NeedDate);
            
            //Если в коллекции ничего нет, значит обычный день
            if (couples.Count == 0)
            {
                DateTime now = DateTime.Now;

                string dt = NeedDate.DayOfWeek.ToString().ToLower();

                string numOfWeek = "";
                if (App.Current.Properties.TryGetValue("numOfWeek", out object num))
                {
                    numOfWeek = (string)num;
                }
                //Если прошел переход через неделю, то есть открыл страницу в четверг,
                //а загружаются пары для Вторника уже новой недели, первая неделя сменилась второй
                if (!isItForOne && NeedDate.DayOfWeek < now.DayOfWeek)
                {
                    if (numOfWeek == "1")
                        numOfWeek = "2";
                    else
                        numOfWeek = "1";
                }

                //проверяется факультет
                if (App.Current.Properties.TryGetValue("facultyName", out object FacultyName))
                {
                    string facultyName = (string)FacultyName;
                    //проверяются номер группы, имя группы и подгруппа
                    string groupId = "";
                    if (App.Current.Properties.TryGetValue("groupId", out object GroupId))
                    { groupId = (string)GroupId; }
                    string groupName = "";
                    if (App.Current.Properties.TryGetValue("groupName", out object GroupName))
                    { groupName = (string)GroupName; }
                    string subgroup = null;
                    if (App.Current.Properties.TryGetValue("subgroup", out object Subgroup))
                    { subgroup = (string)Subgroup; }

                    foreach (var f in App.facultiesJSON)
                    {
                        if (f.FacultyName == facultyName)
                        {
                            foreach (var g in f.Groups)
                            {
                                if (g.GroupId == groupId && g.GroupName == groupName)
                                {
                                    foreach (var c in g.Couples)
                                    {
                                        if (c.Week == numOfWeek && c.SubgroupName == subgroup && c.Day == dt)
                                        {
                                            //проверка, чтобы не показывать уже завершенные пары
                                            if (NeedDate.Day == now.Day)
                                            {
                                                string[] s = c.TimeEnd.Split(':');
                                                int h = Convert.ToInt32(s[0]);
                                                int m = Convert.ToInt32(s[1]);
                                                TimeSpan t = new TimeSpan(h, m, 0);
                                                if (t < now.TimeOfDay)
                                                {
                                                    continue;
                                                }
                                            }
                                            couples.Add(c);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (couples.Count == 0)
                {
                    NothingInteresting(couples, "");
                }
            }

            TimelineItemForStudent nowaday = new TimelineItemForStudent
            {
                ThisDate = NeedDate
            };

            nowaday.AddRange(couples);
            return nowaday;
        }

        public void WhatTodayForStudent(List<Couple> couples, DateTime NeedDate)
        {
            //Что сегодня? Обычный/Выходной/Практика/Экзамен/Рейтинг?
            List<Day> myTimetable;
            string table = "";

            int firstWeekOfSecondSemestr = 0;
            if (App.Current.Properties.TryGetValue("timetable", out object tableFrom))
            {
                table = (string)tableFrom;
                myTimetable = JsonConvert.DeserializeObject<List<Day>>(table);

                int day = NeedDate.Day;
                int month = NeedDate.Month;
                for (int i = 0; i < myTimetable.Count; i++)
                {
                    if (myTimetable[i].ThisDay == day && myTimetable[i].ThisMonth == month)
                    {
                        if (myTimetable[i].Content != null || myTimetable[i].Content != "Э") //пока не проверяю на экзамены
                        {
                            switch (myTimetable[i].Content)
                            {
                                case "*": NothingInteresting(couples, ""); break;
                                case "Д": NothingInteresting(couples, "Д"); break;
                                case "Э": NothingInteresting(couples, "Э"); break;
                                case "Г": NothingInteresting(couples, ""); break;
                                case "У": NothingInteresting(couples, "П"); break;
                                case "К": NothingInteresting(couples, ""); break;
                                case "П": NothingInteresting(couples, "П"); break;
                                case "Н": NothingInteresting(couples, "Н"); break;
                                default:
                                    break;
                            }
                        }

                        if (couples.Count > 0)
                        {
                            break;
                        }

                        //если через шесть дней сессия или если сейчас девятая неделя семестра, то это рейтинг
                        //Что если за шесть дней до сесии были выходные, тогда определение не правильное? Нужно доработать. change
                        if (i + 6 < myTimetable.Count && (myTimetable[i + 6].Content == "Э" || myTimetable[i].ThisWeek - firstWeekOfSecondSemestr == 9))
                        {
                            LoadRaitingForStudent(couples, NeedDate);
                            if (couples.Count == 0)
                            {
                                NothingInteresting(couples, "");
                            }
                            break;
                        }
                        
                    }
                    //Определение первой недели второго семестра для определения первого рейтинга второго семестра, 
                    //ведь он начинаяется на девятой неделе после каникул
                    if (myTimetable[i].Content == "K")
                    {
                        firstWeekOfSecondSemestr = myTimetable[i].ThisWeek;
                    }
                }
            }
        }

        public void NothingInteresting(List<Couple> couples, string s)
        {
            Couple some = new Couple();
            if (s == "П")
                some.CoupleName = "Практика...";
            else if (s == "К")
                some.CoupleName = "Каникулы";
            else if (s == "Д")
                some.CoupleName = "Дипломная работа на носу...";
            else if (s == "Н")
                some.CoupleName = "Научно-исследовательская работа...";
            else if (s == "Э")
                some.CoupleName = "Экзамены да зачеты...";
            else
                some.CoupleName = "Ничего интересного...";

            couples.Add(some);
        }

        public void LoadRaitingForStudent(List<Couple> couples, DateTime NeedDate)
        {
            Couple someCouple;
            //проверяется факультет
            if (App.Current.Properties.TryGetValue("facultyName", out object FacultyName))
            {
                DateTime now = DateTime.Now;
                string dt = NeedDate.DayOfWeek.ToString().ToLower();

                string facultyName = (string)FacultyName;
                //проверяются номер группы, имя группы и подгруппа
                string groupId = "";
                if (App.Current.Properties.TryGetValue("groupId", out object GroupId))
                { groupId = (string)GroupId; }
                string groupName = "";
                if (App.Current.Properties.TryGetValue("groupName", out object GroupName))
                { groupName = (string)GroupName; }
                string subgroup = null;
                if (App.Current.Properties.TryGetValue("subgroup", out object Subgroup))
                { subgroup = (string)Subgroup; }

                foreach (var f in App.facultiesJSONRaiting)
                {
                    if (f.FacultyName == facultyName)
                    {
                        foreach (var g in f.Groups)
                        {
                            if (g.GroupId == groupId && g.GroupName == groupName)
                            {
                                for (int i = 0; i < g.Couples.Count; i++)
                                {
                                    if (g.Couples[i].Day == dt && g.Couples[i].Week == "1" && g.Couples[i].SubgroupName == subgroup)
                                    {
                                        //проверка, чтобы не показывать уже завершенные пары
                                        if (NeedDate.Day == now.Day)
                                        {
                                            string[] s = g.Couples[i].TimeEnd.Split(':');
                                            int h = Convert.ToInt32(s[0]);
                                            int m = Convert.ToInt32(s[1]);
                                            TimeSpan t = new TimeSpan(h, m, 0);
                                            if (t < now.TimeOfDay)
                                            {
                                                continue;
                                            }
                                        }
                                        someCouple = new Couple(g.Couples[i]);
                                        //проверка на два рейтинга за пару
                                        if (i + 1 < g.Couples.Count && g.Couples[i + 1].Week == "2" && g.Couples[i+1].SubgroupName == subgroup && g.Couples[i + 1].CoupleName != someCouple.CoupleName)
                                        {
                                            someCouple.CoupleName += ", " + g.Couples[i + 1].CoupleName;
                                        }
                                        else if (i + 2 < g.Couples.Count && g.Couples[i + 2].Week == "2" && g.Couples[i + 2].CoupleName != someCouple.CoupleName)
                                        {
                                            someCouple.CoupleName += ", " + g.Couples[i + 2].CoupleName;
                                        }

                                        couples.Add(someCouple);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadRaitingForTeacher(SortedDictionary<byte, TeacherCouple> sortedTeacherCouples, DateTime NeedDate)
        {
            DateTime now = DateTime.Now;
            Dictionary<byte, TeacherCouple> teacherCouplesRaiting = new Dictionary<byte, TeacherCouple>();
            List<TeacherCouple> teacherCoupleListRaiting = new List<TeacherCouple>();

            string dt = NeedDate.DayOfWeek.ToString().ToLower();

            //проверяется имя преподавателя
            if (App.Current.Properties.TryGetValue("teacherName", out object AppTeacherName))
            {
                string thisTeacher = (string)AppTeacherName;
                foreach (var f in App.facultiesJSONRaiting)
                {
                    foreach (var g in f.Groups)
                    {
                        for (int j = 0; j < g.Couples.Count; j++)
                        {
                            //Contains, так как в паре английского может быть несколь преподавателей
                            if (g.Couples[j].CoupleTeacher.Contains(thisTeacher))
                            {
                                if (g.Couples[j].Week == "1")
                                {
                                    if (g.Couples[j].Day == dt)
                                    {
                                        //проверка, чтобы не показывать уже завершенные пары
                                        if (NeedDate.Day == now.Day)
                                        {
                                            string[] s = g.Couples[j].TimeEnd.Split(':');
                                            int h = Convert.ToInt32(s[0]);
                                            int m = Convert.ToInt32(s[1]);
                                            TimeSpan t = new TimeSpan(h, m, 0);
                                            if (t < now.TimeOfDay)
                                            {
                                                continue;
                                            }
                                        }
                                        #region Проверка в графике, учится ли данная группа, если да, то пара добавляется в коллекцию
                                        int indexOfDigit = 0;
                                        for (int i = 0; i < g.GroupName.Length; i++)//вырезаем код специальности, чтобы по нему искать
                                        {
                                            if (Char.IsDigit(g.GroupName[i]))
                                            {
                                                indexOfDigit = i;
                                                break;
                                            }
                                        }

                                        string code = g.GroupName.Substring(indexOfDigit, 8);
                                        string course = g.GroupId[0].ToString();

                                        foreach (var item in App.timetable)
                                        {
                                            if (item.SpecialtyName.Contains(code))
                                            {
                                                foreach (var courses in item.Courses)
                                                {
                                                    if (courses.CourseNumber == course)
                                                    {
                                                        int day = NeedDate.Day;
                                                        int month = NeedDate.Month;
                                                        for (int i = 0; i < courses.Days.Count; i++)
                                                        {
                                                            if (courses.Days[i].ThisDay == day && courses.Days[i].ThisMonth == month)
                                                            {
                                                                if (i + 6 < courses.Days.Count && (courses.Days[i + 6].Content == "Э" || courses.Days[i].ThisWeek - 24 == 9)) //change 24
                                                                {
                                                                    if (courses.Days[i].Content == null)
                                                                    {
                                                                        byte coupleNum = Convert.ToByte(g.Couples[j].CoupleNum);

                                                                        if (!teacherCouplesRaiting.ContainsKey(coupleNum))
                                                                        {
                                                                            teacherCouplesRaiting.Add(coupleNum, new TeacherCouple(g.Couples[j],
                                                                                g.GroupId + (g.Couples[j].SubgroupId != null ? "(" + g.Couples[j].SubgroupId + ")" : "")));
                                                                        }
                                                                        else
                                                                        {
                                                                            teacherCouplesRaiting[coupleNum].CoupleTeacher += ", " + g.GroupId + (g.Couples[j].SubgroupId != null ? "(" + g.Couples[j].SubgroupId + ")" : "");
                                                                        }

                                                                        //проверка на два рейтинга за пару
                                                                        if (j + 1 < g.Couples.Count && g.Couples[j + 1].SubgroupId == g.Couples[j].SubgroupId && g.Couples[j + 1].CoupleName != teacherCouplesRaiting[coupleNum].CoupleName)
                                                                        {
                                                                            teacherCouplesRaiting[coupleNum].CoupleName += ", " + g.Couples[j + 1].CoupleName;
                                                                        }
                                                                        else if (j + 2 < g.Couples.Count && g.Couples[j + 2].SubgroupId != null && g.Couples[j + 2].CoupleName != teacherCouplesRaiting[coupleNum].CoupleName)
                                                                        {
                                                                            //рейтинг уже может быть записан после первой группы, поэтому проверка
                                                                            if (!teacherCouplesRaiting[coupleNum].CoupleName.Contains(g.Couples[j + 2].CoupleName))
                                                                            {
                                                                                teacherCouplesRaiting[coupleNum].CoupleName += ", " + g.Couples[j + 2].CoupleName;
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        #endregion  
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (teacherCouplesRaiting.Count > 0)
            {
                //Соединение рейтингов с парами
                if (teacherCouplesRaiting != null)
                {
                    foreach (var item in sortedTeacherCouples)
                    {
                        if (!teacherCouplesRaiting.ContainsKey(item.Key))
                        {
                            teacherCouplesRaiting.Add(item.Key, item.Value);
                        }
                    }

                    SortedDictionary<byte, TeacherCouple> sortedTeacherCouplesRaiting = new SortedDictionary<byte, TeacherCouple>(teacherCouplesRaiting);
                    sortedTeacherCouples.Clear();
                    foreach (KeyValuePair<byte, TeacherCouple> item in sortedTeacherCouplesRaiting)
                    {
                        sortedTeacherCouples.Add(item.Key, item.Value);
                    }
                }
            }
        }

        public void LoadExams(List<Couple> couples)
        {

        }

        
    }
}
