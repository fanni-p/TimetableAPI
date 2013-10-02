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
    public class SubjectController : BaseApiController
    {
        [HttpGet]
        public SubjectModel GetSubjectById(int id,
             [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, dbContext);
                var subject = user.Subjects.FirstOrDefault(l => l.Id == id);

                var models = SubjectModel.Parse(subject);

                return models;
            });
        }

        [HttpGet]
        public IEnumerable<SubjectModel> GetAllSubjects(
             [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, dbContext);
                var subjects = user.Subjects;

                var models = SubjectModel.ParseModels(subjects);

                return models.OrderBy(s => s.Name);
            });
        }

        [HttpPost]
        public HttpResponseMessage CreateSubject(SubjectModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                using (dbContext)
                {
                    var user = this.GetUserByAccessToken(accessToken, dbContext);

                    var newSubject = new Subject
                    {
                        Owner = user,
                        Name = model.Name,
                        Color = model.Color,
                        Teacher = model.Teacher
                    };

                    user.Subjects.Add(newSubject);
                    dbContext.SaveChanges();

                    var responseModel = new ResponseModel()
                    {
                        Id = newSubject.Id
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, responseModel);
                    return response;
                }
            });

            return responseMsg;
        }

        [HttpPost]
        public HttpResponseMessage EditSubject(SubjectModel model, int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                using (dbContext)
                {
                    var user = this.GetUserByAccessToken(accessToken, dbContext);
                    var subject = user.Subjects.FirstOrDefault(s => s.Id == id);

                    subject.Name = model.Name;
                    subject.Teacher = model.Teacher;
                    subject.Color = model.Color;

                    dbContext.SaveChanges();

                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
            });

            return responseMsg;
        }

        [HttpDelete]
        public HttpResponseMessage DeleteSubject(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(
                () =>
                {
                    var dbContext = new TimetableContext();
                    using (dbContext)
                    {
                        var user = this.GetUserByAccessToken(accessToken, dbContext);
                        var existingSubject = user.Subjects.FirstOrDefault(l => l.Id == id);

                        var homeworksForSubject = user.Homeworks.Where(h => h.Subject.Id == existingSubject.Id);

                        foreach (var homework in homeworksForSubject)
                        {
                            user.Homeworks.Remove(homework);
                        }

                        var lessonsForSubject = user.Lessons.Where(h => h.Subject.Id == existingSubject.Id);

                        foreach (var lesson in lessonsForSubject)
                        {
                            user.Lessons.Remove(lesson);
                        }

                        user.Subjects.Remove(existingSubject);
                        dbContext.SaveChanges();

                        var response = this.Request.CreateResponse(HttpStatusCode.OK);
                        return response;
                    }
                });

            return responseMsg;
        }
    }
}
