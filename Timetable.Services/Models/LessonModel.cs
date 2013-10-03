using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Timetable.Models;

namespace Timetable.Services.Models
{
    [DataContract]
    public class LessonModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "subjectColor")]
        public string SubjectColor { get; set; }

        [DataMember(Name = "day")]
        public string Day { get; set; }

        [DataMember(Name = "dayNumber")]
        public int? DayNumber { get; set; }

        [DataMember(Name = "startTime")]
        public TimeSpan StartTime { get; set; }

        [DataMember(Name = "endTime")]
        public TimeSpan EndTime { get; set; }

        [DataMember(Name = "type")]
        public LessonType Type { get; set; }

        [DataMember(Name = "room")]
        public string Room { get; set; }

        [DataMember(Name = "note")]
        public string Note { get; set; }

        public static int? CheckDay(string day)
        {
            switch (day)
            {
                case "Monday": return 0;
                case "Tuesday": return 1;
                case "Wednesday": return 2;
                case "Thursday": return 3;
                case "Friday": return 4;
                case "Saturday": return 5;
                case "Sunday": return 6;
                default: return null;
            }
        }

        public static LessonModel Parse(Lesson lesson)
        {
            var models = new LessonModel()
            {
                Id = lesson.Id,
                Subject = lesson.Subject.Name,
                SubjectColor = lesson.Subject.Color,
                Room = lesson.Room,
                Day = lesson.Day,
                DayNumber = lesson.DayNumber,
                StartTime = lesson.Start,
                EndTime = lesson.End,
                Type = lesson.Type,
                Note = lesson.Note
            };

            return models;
        }

        public static IEnumerable<LessonModel> ParseModels(ICollection<Lesson> lessons)
        {
            var models = (from l in lessons
                          select new LessonModel()
                          {
                              Id = l.Id,
                              Subject = l.Subject.Name,
                              SubjectColor = l.Subject.Color,
                              Room = l.Room,
                              Day = l.Day,
                              DayNumber = l.DayNumber,
                              StartTime = l.Start,
                              EndTime = l.End,
                              Type = l.Type,
                              Note = l.Note
                          });

            return models;
        }
    }
}