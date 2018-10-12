﻿using System;
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

            return user == null ? NotFound() : (IHttpActionResult)Ok(user);
        }
    }
}
