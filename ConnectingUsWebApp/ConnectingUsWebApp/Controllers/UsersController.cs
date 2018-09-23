using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ConnectingUsWebApp.Models;

namespace ConnectingUsWebApp.Controllers
{
    public class UsersController : ApiController
    {
        //This should be deleted when the connection to the DB is added
        User[] users = new User[]
        {
            new User { Id = 1, FirstName = "Florencia", LastName = "Rosen"},
            new User { Id = 2, FirstName = "Damian", LastName = "Bers"},
            new User { Id = 3, FirstName = "Facundo", LastName = "Van"},
            new User { Id = 4, FirstName = "Cesar", LastName = "Ler"}
        };

        public IEnumerable<User> GetAllUsers()
        {
            return users;
        }

        public IHttpActionResult GetUser(int id)
        {
            var User = users.FirstOrDefault((p) => p.Id == id);
            if (User == null)
            {
                return NotFound();
            }
            return Ok(User);
        }
    }
}
