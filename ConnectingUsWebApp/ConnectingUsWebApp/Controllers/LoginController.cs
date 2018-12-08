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
        [Route("userlogged")]
        public IHttpActionResult userlogged()
        {
            User user = null;
            var identity = Thread.CurrentPrincipal.Identity;
            if (identity.IsAuthenticated)
            {
                UsersRepository usersRepo = new UsersRepository();
                user = usersRepo.GetUserByEmail(identity.Name);
            }
            return user == null ? NotFound() : (IHttpActionResult)Ok(user);
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
