using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using Timetable.Models;

namespace Timetable.Services.Models
{
    [DataContract]
    public class HomeworkModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name="subject")]
        public string Subject { get; set; }

        [DataMember(Name="submitDate")]
        public DateTime SubmitDate { get; set; }

        [DataMember(Name="isDone")]
        public bool IsDone { get; set; }

        public static HomeworkModel Parse(Homework homework)
        {
            var models =  new HomeworkModel
                          {
                              Id = homework.Id,
                              Subject = homework.Subject.Name,
                              SubmitDate = homework.SubmitDate,
                              IsDone = homework.IsDone
                          };

            return models;
        }

        public static IEnumerable<HomeworkModel> ParseModels(IEnumerable<Homework> homeworks)
        {
            var models = (from hw in homeworks.Where(h => h.IsDone == false)
                          select new HomeworkModel
                          {
                              Id = hw.Id,
                              Subject = hw.Subject.Name,
                              SubmitDate = hw.SubmitDate,
                              IsDone = hw.IsDone
                          });

            return models;
        }
    }
}