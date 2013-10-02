using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Timetable.Data;
using Timetable.Models;
using Timetable.Services.AuthenticationHeaders;
using Timetable.Services.Models;

namespace Timetable.Services.Controllers
{
    public class LessonController : BaseApiController
    {
        [HttpGet]
        public LessonModel GetLessonsById(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, dbContext);
                var lesson = user.Lessons.FirstOrDefault(l => l.Id == id);

                var models = LessonModel.Parse(lesson);

                return models;
            });
        }

        [HttpGet]
        [ActionName("byDay")]
        public IEnumerable<LessonModel> GetLessonsPerDay(string currentDay,
             [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, dbContext);
                var lessons = user.Lessons;

                var models = LessonModel.ParseModels(lessons);

                var selectedLessons = models.Where(m => m.Day == currentDay).OrderBy(o => o.StartTime);
                return selectedLessons;
            });
        } 

        [HttpGet]
        public IEnumerable<LessonModel> GetLessonsPerWeek(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, dbContext);
                var lessons = user.Lessons;

                var models = LessonModel.ParseModels(lessons);

                var selectedLessons = models.Where(l => l.Day != "Saturday" && l.Day != "Sunday")
                                            .OrderBy(l => l.DayNumber)    
                                            .ThenBy(l => l.StartTime);
                return selectedLessons;
            });
        }

        [HttpGet]
        [ActionName("bySubject")]
        public IEnumerable<LessonModel> GetLessonsBySubject(string subject,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var lessons = this.GetLessonsPerWeek(accessToken);

            if (lessons != null)
            {
                var models = lessons.Where(l => l.Subject == subject).OrderBy(l => l.DayNumber);
                return models;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateLesson(LessonModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                using (dbContext)
                {
                    var user = this.GetUserByAccessToken(accessToken, dbContext);
                    var subject = model.Subject;
                    var existingSubject = user.Subjects.FirstOrDefault(s => s.Name == subject);
                    var lesson = new Lesson
                    {
                        Owner = user,
                        Type = model.Type,
                        Room = model.Room,
                        Day = model.Day,
                        DayNumber = LessonModel.CheckDay(model.Day),
                        Start = model.StartTime,
                        End = model.EndTime,
                        Note = model.Note
                    };

                    user.Lessons.Add(lesson);
                    dbContext.SaveChanges();

                    if (existingSubject == null)
                    {
                        var newSubject = new Subject { Name = subject, Owner = user };
                        user.Subjects.Add(newSubject);
                        dbContext.SaveChanges();
                        lesson.Subject = newSubject;
                        newSubject.Lessons.Add(lesson);
                    }
                    else
                    {
                        lesson.Subject = existingSubject;
                        existingSubject.Lessons.Add(lesson);
                    }


                    dbContext.SaveChanges();

                    var responseModel = new ResponseModel 
                    {
                        Id = lesson.Id 
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, responseModel);
                    return response;
                }
            });

            return responseMsg;
        }

        [HttpPost]
        public HttpResponseMessage EditLesson(LessonModel model, int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                using (dbContext)
                {
                    var user = this.GetUserByAccessToken(accessToken, dbContext);
                    var subject = model.Subject;
                    var existingSubject = user.Subjects.FirstOrDefault(s => s.Name == subject);

                    var lesson = user.Lessons.FirstOrDefault(l => l.Id == id);
                    lesson.Type = model.Type;
                    lesson.Room = model.Room;
                    lesson.Day = model.Day;
                    lesson.DayNumber = LessonModel.CheckDay(model.Day);
                    lesson.Start = model.StartTime;
                    lesson.End = model.EndTime;
                    lesson.Note = model.Note;

                    if (existingSubject == null)
                    {
                        var newSubject = new Subject { Name = subject, Owner = user };
                        user.Subjects.Add(newSubject);
                        dbContext.SaveChanges();
                        lesson.Subject = newSubject;
                        newSubject.Lessons.Add(lesson);
                    }
                    else
                    {
                        lesson.Subject = existingSubject;
                        existingSubject.Lessons.Add(lesson);
                    }

                    dbContext.SaveChanges();

                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
            });

            return responseMsg;
        }

        [HttpDelete]
        public HttpResponseMessage DeleteLesson(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                using (dbContext)
                {
                    var user = this.GetUserByAccessToken(accessToken, dbContext);
                    var existingLesson = user.Lessons.FirstOrDefault(l => l.Id == id);
                    user.Lessons.Remove(existingLesson);
                    dbContext.SaveChanges();

                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
            });

             return responseMsg;
        }
    }
}
