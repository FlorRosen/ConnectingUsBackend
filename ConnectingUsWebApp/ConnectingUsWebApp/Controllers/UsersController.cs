﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ConnectingUsWebApp.Models;
using System.Data.SqlClient;
using System.Text;
using System.Web.Http;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    public class UsersController : ApiController
    {
        private static readonly UsersRepository usersRepo = new UsersRepository();

        //Public Methods
        public IEnumerable<User> GetUsers()
        {
            List<User> users = usersRepo.GetUsers();
            return users;
        }

        public IHttpActionResult GetUser(int id)
        {
            var user = usersRepo.GetUser(id);

            return user == null ? NotFound() : (IHttpActionResult)Ok(user);
        }

        //POST api/users
        [HttpPost]
        public IHttpActionResult Post([FromBody] User user)
        {
            var ok = usersRepo.AddUser(user);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "The email already exist");
        }
    }
}
