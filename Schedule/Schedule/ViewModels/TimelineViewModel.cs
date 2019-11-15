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

        public TimelineViewModel(byte numberOfItems)
        {
            //Определение номера недели
            DateTime now = DateTime.Now;

            if (App.Current.Properties.TryGetValue("isTeacher", out object isTeacher))
            {
                if ((bool)isTeacher)
                {
                    ItemsForTeacher = new List<TimelineItemForTeacher>();
                    ItemsForTeacher = GetDaysForTeacher(numberOfItems);
                }
                else
                {
                    ItemsForStudents = new List<TimelineItemForStudent>();
                    ItemsForStudents = GetDaysForStudent(numberOfItems);
                }
            }
        }

        //перегрузка для выбора определенной даты
        public TimelineViewModel(DateTime selectedDate)
        {
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

        public List<TimelineItemForStudent> GetDaysForStudent(byte numberOfItems)
        {
            List<TimelineItemForStudent> lines = new List<TimelineItemForStudent>();

            for (int i = 0; i < numberOfItems; i++)
            {
                lines.Add(MakeDayForStudent(DateTime.Now.AddDays(i), false));
            }

            return lines;
        }

        public List<TimelineItemForTeacher> GetDaysForTeacher(byte numberOfItems)
        {
            List<TimelineItemForTeacher> lines = new List<TimelineItemForTeacher>();

            for (int i = 0; i < numberOfItems; i++)
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

            DetermineTheNumberOfWeek(NeedDate.Day, NeedDate.Month);

            string numOfWeek = "";
            if (App.Current.Properties.TryGetValue("numOfWeek", out object num))
            {
                numOfWeek = (string)num;
            }

            //проверяется имя преподавателя
            if (App.Current.Properties.TryGetValue("teacherName", out object AppTeacherName))
            {
                string thisTeacher = (string)AppTeacherName;
                foreach (var f in App.facultiesMain)
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

                teacherCoupleList = sortedTeacherCouples.Values.ToList();

                LoadExamsForTeacher(teacherCoupleList, NeedDate);
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

                DetermineTheNumberOfWeek(NeedDate.Day, NeedDate.Month);

                string numOfWeek = "";
                if (App.Current.Properties.TryGetValue("numOfWeek", out object num))
                {
                    numOfWeek = (string)num;
                }

                if (App.Current.Properties.TryGetValue("groupId", out object GroupId))
                {
                    string groupId = (string)GroupId;
                    string groupName = "";
                    if (App.Current.Properties.TryGetValue("groupName", out object GroupName))
                    { groupName = (string)GroupName; }
                    string subgroup = null;
                    if (App.Current.Properties.TryGetValue("subgroup", out object Subgroup))
                    { subgroup = (string)Subgroup; }

                    foreach (var f in App.facultiesMain)
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

            int day = NeedDate.Day;
            int month = NeedDate.Month;
            for (int i = 0; i < App.myTimetable.Count; i++)
            {
                if (App.myTimetable[i].ThisDay == day && App.myTimetable[i].ThisMonth == month)
                {
                    if (App.myTimetable[i].Content != null)
                    {
                        switch (App.myTimetable[i].Content)
                        {
                            case "*": NothingInteresting(couples, ""); break;
                            case "Д": NothingInteresting(couples, "Д"); break;
                            case "Э": LoadExamsForStudent(couples, NeedDate); break;
                            case "Г": NothingInteresting(couples, ""); break;
                            case "У": NothingInteresting(couples, "П"); break;
                            case "К": NothingInteresting(couples, "К"); break;
                            case "П": NothingInteresting(couples, "П"); break;
                            case "Н": NothingInteresting(couples, "Н"); break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (couples.Count > 0)
                    {
                        break;
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
                some.CoupleName = "Каникулы!";
            else if (s == "Д")
                some.CoupleName = "Диплом...";
            else if (s == "Н")
                some.CoupleName = "Научно-исследовательская работа...";
            else if (s == "Э")
                some.CoupleName = "Экзамены да зачеты...";
            else if (s == "Р")
                some.CoupleName = "Рейтинги...";
            else
                some.CoupleName = "Ничего интересного...";

            couples.Add(some);
        }

        public void LoadExamsForStudent(List<Couple> couples, DateTime NeedDate)
        {
            Couple someCouple;

            //Определение, когда было загружено расписание экзаменов
            DateTime lastUpdate = DateTime.Parse((string)App.Current.Properties["updateExam"]);
            TimeSpan difference = NeedDate.Subtract(lastUpdate);
            int days = difference.Days;
            //если более чем 30 дней назад, то значит нового расписания еще нет,
            //показать в таймлайне просто "Экзамены"
            if (Math.Abs(days) > 30)
            {
                NothingInteresting(couples, "Э");
            }
            else
            {
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

                    foreach (var f in App.facultiesExam)
                    {
                        if (f.FacultyName == facultyName)
                        {
                            foreach (var g in f.Groups)
                            {
                                if (groupName.Contains(g.GroupName) && g.Course == groupId[0].ToString())
                                {
                                    for (int i = 0; i < g.Couples.Count; i++)
                                    {
                                        if (Convert.ToDateTime(g.Couples[i].Day).Day == NeedDate.Day)
                                        {
                                            someCouple = new Couple(g.Couples[i]);
                                            bool added = false;
                                            for (int j = 0; j < couples.Count; j++)
                                            {
                                                if (Convert.ToDateTime(couples[j].TimeBegin) > Convert.ToDateTime(someCouple.TimeBegin))
                                                {
                                                    couples.Insert(j, someCouple);
                                                    added = true;
                                                    break;
                                                }
                                            }
                                            if (!added)
                                            {
                                                couples.Add(someCouple);
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    if (couples.Count == 0)
                    {
                        NothingInteresting(couples, "");
                    }
                }
            }
        }

        public void LoadExamsForTeacher(List<TeacherCouple> couples, DateTime NeedDate)
        {
            TeacherCouple someCouple;

            //проверяется имя преподавателя
            if (App.Current.Properties.TryGetValue("teacherName", out object AppTeacherName))
            {
                string thisTeacher = (string)AppTeacherName;
                foreach (var f in App.facultiesExam)
                {
                    foreach (var g in f.Groups)
                    {
                        for (int j = 0; j < g.Couples.Count; j++)
                        {
                            //Contains, так как в паре английского может быть несколько преподавателей
                            if (g.Couples[j].CoupleTeacher.Contains(thisTeacher))
                            {

                                if (Convert.ToDateTime(g.Couples[j].Day).Day == NeedDate.Day &&
                                    Convert.ToDateTime(g.Couples[j].Day).Month == NeedDate.Month)
                                {
                                    someCouple = new TeacherCouple(g.Couples[j], g.GroupName + " (" + g.Course + " курс)");
                                    bool added = false;
                                    for (int q = 0; q < couples.Count; q++)
                                    {
                                        if (Convert.ToDateTime(couples[q].TimeBegin) == Convert.ToDateTime(someCouple.TimeBegin))
                                        {
                                            //чтобы не добавлять одну группу два раза, 
                                            //это возможно т.к. в расписании группа разделена на подгруппы
                                            if (couples[q].CoupleTeacher == someCouple.CoupleTeacher)
                                            {
                                                added = true;
                                                break;
                                            }
                                        }
                                        if (Convert.ToDateTime(couples[q].TimeBegin) > Convert.ToDateTime(someCouple.TimeBegin))
                                        {
                                            couples.Insert(q, someCouple);
                                            added = true;
                                            break;
                                        }
                                    }
                                    if (!added)
                                    {
                                        couples.Add(someCouple);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DetermineTheNumberOfWeek(int selectedDay, int selectedMonth)
        {
            int day = selectedDay;
            int month = selectedMonth;
            foreach (var item in App.myTimetable)
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
        }


    }
}
