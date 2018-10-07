using System;
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
        static UsersRepository usersRepo = new UsersRepository();

        //Public Methods
        public IEnumerable<User> GetUsers()
        {
            List<User> users = usersRepo.GetUsers();
            return users;
        }

        public IHttpActionResult GetUser(int id)
        {
            var user = usersRepo.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        //POST api/users
       //[Route("~/api/Users/AddUser")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] User user)
        {
            var ok = usersRepo.AddUser(user);

            if (ok)
            {
                return NotFound();
            }

            return Ok(ok );
        }



    }
}
