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
        public string CoupleNum { get; set; }
        public string TimeBegin { get; set; }
        public string TimeEnd { get; set; }
        public string CoupleName { get; set; }
        public string CoupleTeacher { get; set; }
        public string CoupleAud { get; set; }
        
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
            CoupleNum = couple.CoupleNum;
        }
        public TeacherCouple()
        {

        }
    }

}
