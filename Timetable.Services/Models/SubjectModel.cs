using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using Timetable.Models;

namespace Timetable.Services.Models
{
    [DataContract]
    public class SubjectModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "teacher")]
        public string Teacher { get; set; }

        [DataMember(Name = "color")]
        public string Color { get; set; }

        public static SubjectModel Parse(Subject subject)
        {
            var models = new SubjectModel()
            {
                Id = subject.Id,
                Name = subject.Name,
                Teacher = subject.Teacher,
                Color = subject.Color
            };
            return models;
        }

        public static IEnumerable<SubjectModel> ParseModels(ICollection<Subject> subjects)
        {
            var models = (from s in subjects
                          select new SubjectModel()
                          {
                              Id = s.Id,
                              Name = s.Name,
                              Teacher = s.Teacher,
                              Color = s.Color
                          });

            return models;
        }
    }
}