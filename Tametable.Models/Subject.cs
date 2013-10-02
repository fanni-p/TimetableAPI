using System;
using System.Collections.Generic;

namespace Timetable.Models
{
    public class Subject
    {
        public Subject()
        {
            this.Lessons = new HashSet<Lesson>();
            this.Homeworks = new HashSet<Homework>();
        }

        public int Id { get; set; }

        public virtual User Owner { get; set; }

        public string Name { get; set; }

        public string Teacher { get; set; }

        public string Color { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }

        public virtual ICollection<Homework> Homeworks { get; set; }
    }
}
