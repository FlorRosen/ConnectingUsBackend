using System.Collections.Generic;
using System.Net;
using ConnectingUsWebApp.Models;
using System.Web.Http;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    [Authorize]
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
        [AllowAnonymous]
        [HttpPost]
        //public IHttpActionResult Post([FromBody] User user)
        public User Post([FromBody] User user)
        {
            return usersRepo.AddUser(user);
          
        }

        //POST api/users
        [HttpPut]
        public IHttpActionResult Put([FromBody] User user)
        {
           
            user = usersRepo.EditUser(user);
  
            return user != null ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Couln't modify the user data");
        }


        //Public Methods
        [HttpGet]
        [Route("api/users/profile")]
        public IHttpActionResult GetUserReputation(int id)
        {
            var user = usersRepo.GetUserReputation(id);

            return user == null ? NotFound() : (IHttpActionResult)Ok(user);
        }
    }
}
