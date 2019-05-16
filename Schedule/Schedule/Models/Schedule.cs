using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Models
{
    public class Group
    {
        public string groupName { get; set; }
        public string groupId { get; set; }
        public Subgroup firstSubgroup { get; set; }
        public Subgroup secondSubgroup { get; set; }
    }

    public class Subgroup
    {
        public string subgroupName { get; set; }
        public Weeks weeks { get; set; }
    }

    public class Weeks
    {
        public Week firstWeek { get; set; }
        public Week secondWeek { get; set; }
    }

    public class Week
    {
        public Day monday { get; set; }
        public Day tuesday { get; set; }
        public Day wednesday { get; set; }
        public Day thursday { get; set; }
        public Day friday { get; set; }
        public Day saturday { get; set; }
    }

    public class Day
    {
        public Couple couple1 = new Couple("9:00", "10:30", "1 пара");
        public Couple couple2 = new Couple("10:40", "12:10", "2 пара");
        public Couple couple3 = new Couple("12:50", "14:20", "3 пара");
        public Couple couple4 = new Couple("14:30", "16:00", "4 пара");
    }

    public class Couple
    {
        public string coupleName { get; set; }
        public string coupleTeacher { get; set; }
        public string coupleAud { get; set; }
        public string timeBegin { get; set; }
        public string timeEnd { get; set; }
        public string numOfCouple { get; set; }

        public Couple (string begin, string end, string num)
        {
            timeBegin = begin;
            timeEnd = end;
            numOfCouple = num;
        }
    }

    public class TeacherCouple
    {
        public string coupleName { get; set; }
        public string coupleTeacher { get; set; }
        public string coupleAud { get; set; }
        public string timeBegin { get; set; }
        public string timeEnd { get; set; }
        public string numOfCouple { get; set; }

        public TeacherCouple(Couple couple, string groupId)
        {
            coupleName = couple.coupleName;
            coupleTeacher = groupId;
            coupleAud = couple.coupleAud;
            timeBegin = couple.timeBegin;
            timeEnd = couple.timeEnd;
            numOfCouple = couple.numOfCouple;
        }
    }
}
