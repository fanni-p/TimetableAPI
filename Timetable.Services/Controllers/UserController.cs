using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Timetable.Data;
using Timetable.Models;
using Timetable.Services.AuthenticationHeaders;
using Timetable.Services.Models;

namespace Timetable.Services.Controllers
{
    public class UserController : BaseApiController
    {
        private const int TokenLength = 50;
        private const string TokenChars = "qwertyuiopasdfghjklmnbvcxzQWERTYUIOPLKJHGFDSAZXCVBNM";
        private const int MinUsernameLength = 3;
        private const int MaxUsernameLength = 30;
        private const int AuthenticationCodeLength = 40;
        private const string ValidUsernameChars = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.@";

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage RegisterUser(UserModel model)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                this.ValidateUser(model);

                var context = new TimetableContext();
                var dbUser = GetUserByUsername(model, context);
                if (dbUser != null)
                {
                    throw new InvalidOperationException("This user already exists in the database");
                }

                dbUser = new User()
                {
                    Username = model.Username,
                    AuthenticationCode = model.AuthCode
                };

                context.Users.Add(dbUser);
                context.SaveChanges();

                var responseModel = new RegisterUserResponseModel()
                {
                    Id = dbUser.Id,
                    Username = dbUser.Username,
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, responseModel);
                return response;
            });
        }

        [HttpPost]
        [ActionName("token")]
        public HttpResponseMessage LoginUser(UserModel model)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                if (model == null)
                {
                    throw new FormatException("invalid username and/or password");
                }

                this.ValidateAuthCode(model.AuthCode);
                this.ValidateUsername(model.Username);

                var context = new TimetableContext();
                var username = model.Username.ToLower();
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    throw new InvalidOperationException("Invalid username or password");
                }

                if (user.AccessToken == null)
                {
                    user.AccessToken = this.GenerateAccessToken(user.Id);
                    context.SaveChanges();
                }

                var responseModel = new LoginResponseModel()
                {
                    Id = user.Id,
                    Username = user.Username,
                    AccessToken = user.AccessToken
                };

                var response = this.Request.CreateResponse(HttpStatusCode.OK, responseModel);
                return response;
            });
        }

        [HttpPut]
        [ActionName("logout")]
        public HttpResponseMessage LogoutUser
            ([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string accessToken)
        {
            return this.PerformOperationAndHandleExceptions(() =>
            {
                var context = new TimetableContext();
                var user = this.GetUserByAccessToken(accessToken, context);
                user.AccessToken = null;
                context.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.NoContent);
                return response;
            });
        }

        private User GetUserByUsername(UserModel model, TimetableContext context)
        {
            var usernameToLower = model.Username.ToLower();
            var dbUser = context.Users.FirstOrDefault(u => u.Username == usernameToLower);
            return dbUser;
        }

        private string GenerateAccessToken(int userId)
        {
            StringBuilder tokenBuilder = new StringBuilder(TokenLength);
            tokenBuilder.Append(userId);
            while (tokenBuilder.Length < TokenLength)
            {
                var index = rand.Next(TokenChars.Length);
                var ch = TokenChars[index];
                tokenBuilder.Append(ch);
            }
            return tokenBuilder.ToString();
        }

        private void ValidateUser(UserModel userModel)
        {
            if (userModel == null)
            {
                throw new FormatException("Username and/or password are invalid");
            }
            this.ValidateUsername(userModel.Username);
            this.ValidateAuthCode(userModel.AuthCode);
        }

        private void ValidateAuthCode(string authCode)
        {
            if (string.IsNullOrEmpty(authCode) || authCode.Length != AuthenticationCodeLength)
            {
                throw new FormatException("Password is invalid");
            }
        }

        private void ValidateUsername(string username)
        {
            if (username.Length < MinUsernameLength || MaxUsernameLength < username.Length)
            {
                throw new FormatException(
                    string.Format("Username must be between {0} and {1} characters",
                        MinUsernameLength,
                        MaxUsernameLength));
            }
            if (username.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new FormatException("Username contains invalid characters");
            }
        }
    }
}
