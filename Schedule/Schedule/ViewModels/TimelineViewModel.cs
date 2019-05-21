using Schedule.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.ViewModels
{
    class TimelineViewModel
    {
        public List<TimelineItem> Items { get; set; }

        public TimelineViewModel()
        {
            Items = new List<TimelineItem>();
            Items = GetItems();
        }

        public List<TimelineItem> GetItems()
        {
            List<TimelineItem> lines = new List<TimelineItem>();

            for (int i = 0; i < 7; i++)
            {
                lines.Add(MakeDays(DateTime.Now.AddDays(i)));
            }

            return lines;
        }

        public TimelineItem MakeDays(DateTime NeedDate)
        {
            List<Couple> couples = new List<Couple>();

            string dt = NeedDate.DayOfWeek.ToString().ToLower();

            string numOfWeek = "";
            if (App.Current.Properties.TryGetValue("numOfWeek", out object num))
            {
                numOfWeek = (string)num;
            }

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
                                        couples.Add(c);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            TimelineItem nowaday = new TimelineItem
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
