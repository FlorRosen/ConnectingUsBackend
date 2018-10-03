using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ConnectingUsWebApp.Models;
using System.Data.SqlClient;
using System.Text;

namespace ConnectingUsWebApp.Controllers
{
    public class UsersController : ApiController
    {
        List<User> users = new List<User>();

        public IEnumerable<User> GetAllUsers()
        {

            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AzureConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                String query = "select * from dbo.users";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User();
                            user.Id = Int32.Parse(reader["id_user"].ToString());
                            user.FirstName = reader["first_name"].ToString();
                            user.LastName = reader["last_name"].ToString();

                            users.Add(user);
                          
                        }
                    }
                }
            }

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
