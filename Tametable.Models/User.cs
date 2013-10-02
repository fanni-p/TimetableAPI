using System;
using System.Collections.Generic;

namespace Timetable.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string AuthenticationCode { get; set; }

        public string AccessToken { get; set; }

        public virtual ICollection<Homework> Homeworks { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
