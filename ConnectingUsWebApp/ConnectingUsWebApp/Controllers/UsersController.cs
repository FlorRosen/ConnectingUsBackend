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

        //Public Methods
        public IEnumerable<User> GetUsers()
        {
            List<User> users = usersRepo.GetUsers();
            return users;
        }

        [HttpGet]
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

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "The email or nickname already exist");
        }

        //POST api/users
        [HttpPut]
        public IHttpActionResult Put([FromBody] User user)
        {
            var ok = usersRepo.EditUser(user);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Couln't modify the user data");
        }
    }
}
