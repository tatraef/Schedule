using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Models
{
    public class TimelineItemForStudent : List<Couple>
    {
        public DateTime ThisDate { get; set; }
        public List<Couple> Couples => this;
    }

    public class TimelineItemForTeacher : List<TeacherCouple>
    {
        public DateTime ThisDate { get; set; }
        public List<TeacherCouple> Couples => this;
    }
}
