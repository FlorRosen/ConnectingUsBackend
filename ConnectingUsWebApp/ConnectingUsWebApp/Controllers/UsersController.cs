using System.Collections.Generic;
using System.Net;
using ConnectingUsWebApp.Models;
using System.Web.Http;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    public class UsersController : ApiController
    {
        static readonly UsersRepository usersRepo = new UsersRepository();


        [HttpGet]
        public IHttpActionResult GetUser(int id)
        {
            var user = usersRepo.GetUser(id);

            return user == null ? NotFound() : (IHttpActionResult)Ok(user);
        }

        //POST api/users
        [HttpPost]
        //public IHttpActionResult Post([FromBody] User user)
        public User Post([FromBody] User user)
        {
           
           // user = usersRepo.AddUser(user);

            return usersRepo.AddUser(user);
            // return user != null ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "The email or nickname already exist");
        }

        //POST api/users
        [HttpPut]
        public IHttpActionResult Put([FromBody] User user)
        {
           
            user = usersRepo.EditUser(user);
  
            return user != null ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Couln't modify the user data");
        }
    }
}
