using System;

namespace Timetable.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        public virtual User Owner { get; set; }

        public virtual Subject Subject { get; set; }

        public string Day { get; set; }

        public int? DayNumber { get; set; }

        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }

        public LessonType Type { get; set; }

        public string Room { get; set; }

        public string Note { get; set; }
    }
}
