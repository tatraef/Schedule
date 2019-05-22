using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Models
{
    public class Specialty
    {
        public string SpecialtyName { get; set; }
        public List<Course> Courses { get; set; }
    }

    public class Course
    {
        public string CourseNumber { get; set; }
        public List<Day> Days { get; set; }
    }

    public class Day
    {
        public int ThisDate { get; set; }
        public int ThisWeek { get; set; }
        public int ThisMonth { get; set; }
        public string Content { get; set; }
    }
}
