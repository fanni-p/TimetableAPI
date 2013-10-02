using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Timetable.Data;
using Timetable.Models;

namespace Timetable.Services.Controllers
{
    public class BaseApiController : ApiController
    {
        protected static Random rand = new Random();

        protected T PerformOperationAndHandleExceptions<T>(Func<T> operation)
        {
            try
            {
                return operation();
            }
            catch (Exception ex)
            {
                var errResponse = this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
                throw new HttpResponseException(errResponse);
            }
        }

        protected User GetUserByAccessToken(string accessToken, TimetableContext context)
        {
            var user = context.Users.FirstOrDefault(usr => usr.AccessToken == accessToken);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid user credentials");
            }

            return user;
        }
    }
}