using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Models
{
    public class TimelineItem : List<Couple>
    {
        public DateTime ThisDate { get; set; }
        public List<Couple> Couples => this;
    }
}
