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
    public class HomeworkController : BaseApiController
    {
        [HttpGet]
        public HomeworkModel GetHomeworkById(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, dbContext);
                var homework = user.Homeworks.FirstOrDefault(l => l.Id == id);

                var models = HomeworkModel.Parse(homework);

                return models;
            });
        }

        [HttpGet]
        public IEnumerable<HomeworkModel> GetAll(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, dbContext);
                var homeworks = user.Homeworks.Where(h => h.SubmitDate >= DateTime.Today).ToList();

                var models = HomeworkModel.ParseModels(homeworks);

                return models.OrderBy(m => m.SubmitDate);
            });
        }
  
        

        [HttpGet]
        [ActionName("bySubject")]
        public IEnumerable<HomeworkModel> GetHomeworkBySubject(string subject,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var homework = this.GetAll(accessToken);

            if (homework != null)
            {
                var models = homework.Where(h => h.Subject == subject).OrderBy(h => h.SubmitDate);
                return models;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public HttpResponseMessage CreateHomework(HomeworkModel model,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                using (dbContext)
                {
                    var user = this.GetUserByAccessToken(accessToken, dbContext);
                    var subject = model.Subject;
                    var existingSubject = dbContext.Subjects.FirstOrDefault(s => s.Name == subject);

                    var homework = new Homework 
                    { 
                        Owner = user,
                        IsDone = model.IsDone,
                        SubmitDate = model.SubmitDate
                    };

                    user.Homeworks.Add(homework);
                    dbContext.SaveChanges();

                    if (existingSubject == null)
                    {
                        var newSubject = new Subject{ Name = subject, Owner = user };
                        user.Subjects.Add(newSubject);
                        dbContext.SaveChanges();
                        homework.Subject = newSubject;
                        newSubject.Homeworks.Add(homework);
                    }
                    else
                    {
                        homework.Subject = existingSubject;
                        existingSubject.Homeworks.Add(homework);
                    }


                    dbContext.SaveChanges();

                    var responseModel = new ResponseModel()
                    {
                        Id = homework.Id
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, responseModel);
                    return response;
                }
            });

            return responseMsg;
        }

        [HttpPost]
        public HttpResponseMessage EditHomework(HomeworkModel model, int id,
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

                    var homework = user.Homeworks.FirstOrDefault(h => h.Id == id);
                    homework.IsDone = model.IsDone;
                    homework.SubmitDate = model.SubmitDate;

                    if (existingSubject == null)
                    {
                        var newSubject = new Subject { Name = subject };
                        user.Subjects.Add(newSubject);
                        dbContext.SaveChanges();
                        homework.Subject = newSubject;
                        newSubject.Homeworks.Add(homework);
                    }
                    else
                    {
                        homework.Subject = existingSubject;
                        existingSubject.Homeworks.Add(homework);
                    }


                    dbContext.SaveChanges();

                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
            });

            return responseMsg;
        }

        [HttpPut]
        public HttpResponseMessage MarkAsDone(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, dbContext);

                var homework = user.Homeworks.FirstOrDefault(h => h.Id == id);

                homework.IsDone = (homework.IsDone) ? false : true;
                dbContext.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.Created);
                return response;
            });
        }

        [HttpDelete]
        public HttpResponseMessage DeleteHomewrok(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var dbContext = new TimetableContext();
                using (dbContext)
                {
                    var user = this.GetUserByAccessToken(accessToken, dbContext);
                    var existingHomework = user.Homeworks.FirstOrDefault(l => l.Id == id);
                    user.Homeworks.Remove(existingHomework);
                    dbContext.SaveChanges();

                    var response = this.Request.CreateResponse(HttpStatusCode.OK);
                    return response;
                }
            });

            return responseMsg;
        }
    }
}
