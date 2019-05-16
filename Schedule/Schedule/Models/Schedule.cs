using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Schedule.Models
{
    public class Faculty
    {
        public string FacultyName { get; set; }
        public List<Group> Groups { get; set; }
    }

    public class Group
    {
        public string GroupName { get; set; }
        public string GroupId { get; set; }
        public List<Couple> Couples { get; set; }
    }

    public class Couple
    {
        public string SubgroupName { get; set; }
        public string SubgroupId { get; set; }
        public string Week { get; set; }
        public string Day { get; set; }
        private string coupleNum;
        public string СoupleNum
        {
            set
            {
                if (value == "1")
                {
                    TimeBegin = "9:00";
                    TimeEnd = "10:30";
                }
                else if (value == "2")
                {
                    TimeBegin = "10:40";
                    TimeEnd = "12:10";
                }
                else if (value == "3")
                {
                    TimeBegin = "12:50";
                    TimeEnd = "14:20";
                }
                else if (value == "4")
                {
                    TimeBegin = "14:30";
                    TimeEnd = "16:00";
                }
                else if (value == "5")
                {
                    TimeBegin = "16:10";
                    TimeEnd = "17:40";
                }
                coupleNum = value;

            }
            get { return coupleNum; }
        }
        public string CoupleName { get; set; }
        public string CoupleTeacher { get; set; }
        public string CoupleAud { get; set; }
        public string TimeBegin { get; set; }
        public string TimeEnd { get; set; }
    }

    public class TeacherCouple
    {
        public string CoupleName { get; set; }
        public string CoupleTeacher { get; set; }
        public string CoupleAud { get; set; }
        public string TimeBegin { get; set; }
        public string TimeEnd { get; set; }
        public string CoupleNum { get; set; }

        public TeacherCouple(Couple couple, string groupId)
        {
            CoupleName = couple.CoupleName;
            CoupleTeacher = groupId;
            CoupleAud = couple.CoupleAud;
            TimeBegin = couple.TimeBegin;
            TimeEnd = couple.TimeEnd;
            CoupleNum = couple.СoupleNum;
        }
    }

}
