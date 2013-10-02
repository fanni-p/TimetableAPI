using System;

namespace Timetable.Models
{
    public class Homework
    {
        public int Id { get; set; }

        public virtual User Owner { get; set; }

        public virtual Subject Subject { get; set; }

        public DateTime SubmitDate { get; set; }

        public bool IsDone { get; set; }
    }
}
