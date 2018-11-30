using System;
using System.Net;
using System.Threading;
using System.Web.Http;
using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Models.ViewModels;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/login")]
    public class LoginController: ApiController
    {
        static readonly LoginRepository loginRepo = new LoginRepository();

        [HttpPost]
        public IHttpActionResult Post([FromBody] LoginViewModel login)
        {
            var user = loginRepo.LoginUser(login);

            return user.Id == 0 ? NotFound() : (IHttpActionResult)Ok(user);
        }

        [HttpGet]
        [Route("echouser")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        }

        [HttpPost]
        [Route("authenticate")]
        public IHttpActionResult Authenticate(LoginViewModel login)
        {
            if (login == null)
                throw new HttpResponseException(HttpStatusCode.BadRequest);
                
            var user = loginRepo.LoginUser(login);

            bool isCredentialValid = (user.Id != 0);
            if (isCredentialValid)
            {
                AuthUser authUser = new AuthUser();
                authUser.Token = TokenGenerator.GenerateTokenJwt(login.Mail);
                authUser.User = user;
                return Ok(authUser);
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
