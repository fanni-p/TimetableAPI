using System;
using System.Linq;
using System.Web.Http;

namespace Timetable.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "UserLoginApi",
                routeTemplate: "api/auth/token",
                defaults: new { controller = "user", action = "token" });

            config.Routes.MapHttpRoute(
                name: "UserApi",
                routeTemplate: "api/user/{action}",
                defaults: new { controller = "user" });

            config.Routes.MapHttpRoute(
                name: "HomeworkBySubjectApi",
                routeTemplate: "api/homework/bySubject/{subject}",
                defaults: new { controller = "homework", action = "bySubject" }
            );

            config.Routes.MapHttpRoute(
                 name: "LessonByDayApi",
                 routeTemplate: "api/lesson/byDay/{currentDay}",
                 defaults: new { controller = "lesson", action = "byDay" }
             );

            config.Routes.MapHttpRoute(
                name: "LessonBySubjectApi",
                routeTemplate: "api/lesson/bySubject/{subject}",
                defaults: new { controller = "lesson", action = "bySubject" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
