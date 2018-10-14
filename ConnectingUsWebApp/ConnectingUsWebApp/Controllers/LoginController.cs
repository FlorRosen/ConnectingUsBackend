using System;
using System.Net;
using System.Web.Http;
using ConnectingUsWebApp.Models.ViewModels;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    public class LoginController: ApiController
    {
        static readonly LoginRepository loginRepo = new LoginRepository();

        [HttpPost]
        public IHttpActionResult Post([FromBody] LoginViewModel login)
        {
            var user = loginRepo.LoginUser(login);

            return user.Id == 0 ? NotFound() : (IHttpActionResult)Ok(user);
        }

    }
}
