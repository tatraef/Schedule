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
                    ItemsForTeacher = GetItemsForTeacher();
                }
                else
                {
                    ItemsForStudents = new List<TimelineItemForStudent>();
                    ItemsForStudents = GetItemsForStudent();
                }
            }
        }

        public List<TimelineItemForStudent> GetItemsForStudent()
        {
            List<TimelineItemForStudent> lines = new List<TimelineItemForStudent>();

            for (int i = 0; i < 7; i++)
            {
                lines.Add(MakeDaysForStudent(DateTime.Now.AddDays(i)));
            }

            return lines;
        }

        public List<TimelineItemForTeacher> GetItemsForTeacher()
        {
            List<TimelineItemForTeacher> lines = new List<TimelineItemForTeacher>();

            for (int i = 0; i < 7; i++)
            {
                lines.Add(MakeDaysForTeacher(DateTime.Now.AddDays(i)));
            }

            return lines;
        }

        public TimelineItemForTeacher MakeDaysForTeacher(DateTime NeedDate)
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
            if (NeedDate.DayOfWeek < now.DayOfWeek)
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
            }

            TimelineItemForTeacher nowaday = new TimelineItemForTeacher
            {
                ThisDate = NeedDate
            };


            if (teacherCouples.Count == 0)
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

        public TimelineItemForStudent MakeDaysForStudent(DateTime NeedDate)
        {
            DateTime now = DateTime.Now;
            List<Couple> couples = new List<Couple>();

            string dt = NeedDate.DayOfWeek.ToString().ToLower();

            string numOfWeek = "";
            if (App.Current.Properties.TryGetValue("numOfWeek", out object num))
            {
                numOfWeek = (string)num;
            }
            //Если прошел переход через неделю, то есть открыл страницу в четверг,
            //а загружаются пары для Вторника уже новой недели, первая неделя сменилась второй
            if (NeedDate.DayOfWeek < now.DayOfWeek)
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

            TimelineItemForStudent nowaday = new TimelineItemForStudent
            {
                ThisDate = NeedDate
            };

            if (couples.Count == 0)
            {
                Couple some = new Couple
                {
                    CoupleName = "Ничего интересного..."
                };
                couples.Add(some);
            }

            nowaday.AddRange(couples);
            return nowaday;


        }
    }
}
