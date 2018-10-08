using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Web;  
using System.Data.SqlClient;  
using System.Configuration;  
using System.Data;
using ConnectingUsWebApp.Models;
using System.Web.Http;

namespace ConnectingUsWebApp.Repositories
{
    public class UsersRepository
    {

        private SqlConnection connection;
        private SqlCommand command;

        private void CreateConnection()
        {
            string constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }


        //Public Methods
        public List<User> GetUsers()
        {
            List<User> users = new List<User>();

            CreateConnection();
            String query = "select * from users";
            command = new SqlCommand(query, connection);
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    User user = MapUserFromDB(reader);
                    users.Add(user);
                }
            }
            connection.Close();
            return users;
        }

        public User GetUser(int id)
        {
            User user = new User();

            CreateConnection();
            String query = "select * from users where id_user = " + id;
            command = new SqlCommand(query, connection);
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    user = MapUserFromDB(reader);
                }
            }

            connection.Close();
            return user;
        }

        public bool AddUser(User user)
        {
            CreateConnection();
            var result = false;
            var dateAndTime = DateTime.Now;
            var date = dateAndTime.Date.ToString("d");

            using (SqlCommand command_addUser = new SqlCommand())
            {
                connection.Open();

                command_addUser.Connection = connection;

                command_addUser.CommandText = "INSERT INTO users (id_country, first_name, last_name, birth_date, create_date, gender, phone_number, phone_type, id_city_residence) " +
                 "OUTPUT INSERTED.id_user VALUES (@id_country, @first_name, @last_name, @birth_date, @create_date, @gender, @phone_number, @phone_type, @id_city_residence)";

                command_addUser.Parameters.AddWithValue("@id_country", user.CountryOfResidence.Id);
                command_addUser.Parameters.AddWithValue("@first_name", user.FirstName);
                command_addUser.Parameters.AddWithValue("@last_name", user.LastName);
                command_addUser.Parameters.AddWithValue("@birth_date", user.DateOfBirth);
                command_addUser.Parameters.AddWithValue("@create_date", date);
                command_addUser.Parameters.AddWithValue("@gender", user.Gender);
                command_addUser.Parameters.AddWithValue("@phone_number", user.PhoneNumber);
                command_addUser.Parameters.AddWithValue("@phone_type", user.PhoneType);
                command_addUser.Parameters.AddWithValue("@id_city_residence", user.CityOfResidence.Id);

                SqlParameter param = new SqlParameter("@id_user", SqlDbType.Int, 4)
                {
                    Direction = ParameterDirection.Output
                };
                command_addUser.Parameters.Add(param);

                int userId = (int) command_addUser.ExecuteScalar();

                using (SqlCommand command_addAccountForUser = new SqlCommand()){
                    command_addAccountForUser.Connection = connection;

                    command_addAccountForUser.CommandText = "INSERT INTO accounts (id_user, mail, nickname, password) VALUES (@id_user, @mail, @nickname, @password)";

                    command_addAccountForUser.Parameters.AddWithValue("@id_user", userId);
                    command_addAccountForUser.Parameters.AddWithValue("@mail", user.Account.Mail);
                    command_addAccountForUser.Parameters.AddWithValue("@nickname", user.Account.Nickname);
                    command_addAccountForUser.Parameters.AddWithValue("@password", user.Account.Password);

                    command_addAccountForUser.ExecuteNonQuery();
                }
            }

            connection.Close();
            result = true;
            return result;

        }


        //Private Methods
        private User MapUserFromDB(SqlDataReader reader)
        {
            User user = new User
            {
                Id = Int32.Parse(reader["id_user"].ToString()),
                FirstName = reader["first_name"].ToString(),
                LastName = reader["last_name"].ToString(),
                Gender = reader["gender"].ToString(),
                PhoneNumber = reader["phone_number"].ToString(),
                CreateDate = Convert.ToDateTime(reader["create_date"].ToString()),
                DateOfBirth = Convert.ToDateTime(reader["birth_date"].ToString())
            };

            return user;
        }
    }
}
