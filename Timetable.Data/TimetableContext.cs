using System;
using System.Data.Entity;
using Timetable.Models;

namespace Timetable.Data
{
    public class TimetableContext: DbContext
    {
        public TimetableContext()
            :base("TimetableDB")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<Homework> Homeworks { get; set; }

        public DbSet<Subject> Subjects { get; set; }
    }
}
