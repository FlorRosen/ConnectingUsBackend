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
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AzureConnection"].ConnectionString;
        List<User> users = new List<User>();

        public IEnumerable<User> GetUsers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                connection.Open();
                String query = "select * from users";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = mapUserFromDB(reader);

                            users.Add(user);

                        }
                    }
                }
            }
            return users;
        }

        public IHttpActionResult GetUser(int id)
        {
            var user = new User();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String query = "select * from users where id_user = " + id;

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = mapUserFromDB(reader);
                        }
                    }
                }
            }

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        private User mapUserFromDB(SqlDataReader reader)
        {
            User user = new User();
            user.Id = Int32.Parse(reader["id_user"].ToString());
            user.FirstName = reader["first_name"].ToString();
            user.LastName = reader["last_name"].ToString();
            user.Gender = reader["gender"].ToString();
            user.PhoneNumber = reader["phone_number"].ToString();
            user.CreateDate = Convert.ToDateTime(reader["create_date"].ToString());
            user.DateOfBirth = Convert.ToDateTime(reader["birth_date"].ToString());

            return user;
        }
    }
}
