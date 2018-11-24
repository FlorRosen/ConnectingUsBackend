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
        SqlConnection connection;
        SqlCommand command;

        public UsersRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        //Public Methods
        public List<User> GetUsers()
        {
            List<User> users = new List<User>();

            var query = "select * from users";
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
            var user = new User();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from users where id_user = @id_user"
            };
            command.Parameters.AddWithValue("@id_user", id);
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


        //Validates that the nickname or the mail are in the DB
        public bool ValidateUserExistance(Account account)
        {
            var existence = false;

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from accounts where UPPER(mail) = @mail or UPPER(nickname) = @nickname"
            };
            command.Parameters.AddWithValue("@mail", account.Mail.ToUpper());
            command.Parameters.AddWithValue("@nickname", account.Nickname.ToUpper());

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    existence = true;
                }
            }

            connection.Close();
            return existence;
        }

        public User AddUser(User user)
        {
            var result = true;
            if (ValidateUserExistance(user.Account)) {
                result = false;
                user = null;
            }
            else
            {
                try {
                    using (SqlCommand command_addUser = new SqlCommand())
                    {
                        var date = DateTime.Now;
                        connection.Open();

                        command_addUser.Connection = connection;

                        command_addUser.CommandText = "INSERT INTO users (id_country, id_birth_country, first_name, last_name, birth_date, create_date, gender, phone_number, phone_type, id_city_residence, phone_area_code) " +
                         "OUTPUT INSERTED.id_user VALUES (@id_country, @id_birth_country, @first_name, @last_name, @birth_date, @create_date, @gender, @phone_number, @phone_type, @id_city_residence, @phone_area_code)";

                        command_addUser.Parameters.AddWithValue("@id_country", user.CountryOfResidence.Id);
                        command_addUser.Parameters.AddWithValue("@id_birth_country", user.CountryOfBirth.Id);
                        command_addUser.Parameters.AddWithValue("@first_name", user.FirstName);
                        command_addUser.Parameters.AddWithValue("@last_name", user.LastName);
                        command_addUser.Parameters.AddWithValue("@birth_date", user.DateOfBirth);
                        command_addUser.Parameters.AddWithValue("@create_date", date);
                        command_addUser.Parameters.AddWithValue("@gender", user.Gender);
                        command_addUser.Parameters.AddWithValue("@phone_number", user.PhoneNumber ?? Convert.DBNull);
                        command_addUser.Parameters.AddWithValue("@phone_type", user.PhoneType ?? Convert.DBNull);
                        command_addUser.Parameters.AddWithValue("@phone_area_code", user.PhoneAreaCode ?? Convert.DBNull);
                        command_addUser.Parameters.AddWithValue("@id_city_residence", user.CityOfResidence.Id);

                        SqlParameter param = new SqlParameter("@id_user", SqlDbType.Int, 4)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command_addUser.Parameters.Add(param);

                        int userId = (int)command_addUser.ExecuteScalar();
                        user.Id = userId;

                        using (SqlCommand command_addAccountForUser = new SqlCommand()) {
                            command_addAccountForUser.Connection = connection;

                            command_addAccountForUser.CommandText = "INSERT INTO accounts (id_user, mail, nickname, password) " +
                               "VALUES (@id_user, @mail, @nickname,(CONVERT(varchar(256),HASHBYTES('SHA2_256', @password),2)))";

                            command_addAccountForUser.Parameters.AddWithValue("@id_user", userId);
                            command_addAccountForUser.Parameters.AddWithValue("@mail", user.Account.Mail);
                            command_addAccountForUser.Parameters.AddWithValue("@nickname", user.Account.Nickname);
                            command_addAccountForUser.Parameters.AddWithValue("@password", user.Account.Password);

                            command_addAccountForUser.ExecuteNonQuery();
                        }

                    }
                    connection.Close();

                }
                finally {
                    connection.Close();
                }
            }
            return user;

        }

        public User EditUser(User user)
        {

            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    connection.Open();

                    command.Connection = connection;

                    command.CommandText = "UPDATE users SET id_country = @id_country, id_birth_country = @id_birth_country, first_name = @first_name, last_name = @last_name, birth_date = @birth_date, gender = @gender, phone_number = @phone_number, phone_type = @phone_type, id_city_residence = @id_city_residence, phone_area_code = @phone_area_code " +
                     "WHERE id_user = @id_user";

                    command.Parameters.AddWithValue("@id_country", user.CountryOfResidence.Id);
                    command.Parameters.AddWithValue("@id_birth_country", user.CountryOfBirth.Id);
                    command.Parameters.AddWithValue("@first_name", user.FirstName);
                    command.Parameters.AddWithValue("@last_name", user.LastName);
                    command.Parameters.AddWithValue("@birth_date", user.DateOfBirth);
                    command.Parameters.AddWithValue("@gender", user.Gender);
                    command.Parameters.AddWithValue("@phone_number", user.PhoneNumber ?? Convert.DBNull);
                    command.Parameters.AddWithValue("@phone_type", user.PhoneType ?? Convert.DBNull);
                    command.Parameters.AddWithValue("@phone_area_code", user.PhoneAreaCode ?? Convert.DBNull);
                    command.Parameters.AddWithValue("@id_city_residence", user.CityOfResidence.Id);
                    command.Parameters.AddWithValue("@id_user", user.Id);

                    command.ExecuteNonQuery();

                    using (SqlCommand command_Account = new SqlCommand())
                    {
                        command_Account.Connection = connection;

                        command_Account.CommandText = "UPDATE accounts SET password = (CONVERT(varchar(256),HASHBYTES('SHA2_256', @password),2)) WHERE id_user = @id_user";
                        command_Account.Parameters.AddWithValue("@id_user", user.Id);
                        command_Account.Parameters.AddWithValue("@mail", user.Account.Mail);
                        command_Account.Parameters.AddWithValue("@nickname", user.Account.Nickname);
                        command_Account.Parameters.AddWithValue("@password", user.Account.Password);

                        command_Account.ExecuteNonQuery();

                        connection.Close();
                    }
                }
            }
            finally
            {
                connection.Close();
            }
            

            return user;
        }

        public User GetUserReputation(int id)
        {
            var user = new User();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "select * from users where id_user = @id_user"
            };
            command.Parameters.AddWithValue("@id_user", id);
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    
                    user.Id = MapUserFromDB(reader).Id;
                    user.FirstName = MapUserFromDB(reader).FirstName;
                    user.LastName = MapUserFromDB(reader).LastName;
                    user.Gender = MapUserFromDB(reader).Gender;
                    user.Reputation = MapUserFromDB(reader).Reputation;
                    user.CityOfResidence = MapUserFromDB(reader).CityOfResidence;
                    user.CityOfResidence.Id = null;
 
                    user.CountryOfResidence = MapUserFromDB(reader).CountryOfResidence;
                    user.CountryOfResidence.Id = null;
                    user.Account = MapUserFromDB(reader).Account;
                    user.Account.Mail = null;
                    user.Account.Password = null;
                    user.Account.Id = null;

                }
            }

            connection.Close();
            return user;
        }


        public static User MapUserFromDB(SqlDataReader reader)
        {
            CountriesRepository countriesRepo = new CountriesRepository();
            CitiesRepository citiesRepo = new CitiesRepository();
            LoginRepository loginRepo = new LoginRepository();
            ReputationsRepository reputationRepo = new ReputationsRepository();

            User user = new User
            {
                Id = Int32.Parse(reader["id_user"].ToString()),
                FirstName = reader["first_name"].ToString(),
                LastName = reader["last_name"].ToString(),
                Gender = reader["gender"].ToString(),
                PhoneNumber = reader["phone_number"].ToString(),
                PhoneType = reader["phone_type"].ToString(),
                PhoneAreaCode = reader["phone_area_code"].ToString(),
                CreateDate = Convert.ToDateTime(reader["create_date"].ToString()),
                DateOfBirth = Convert.ToDateTime(reader["birth_date"].ToString()),
                CountryOfResidence = countriesRepo.GetCountry(Int32.Parse(reader["id_country"].ToString())),
                CountryOfBirth = countriesRepo.GetCountry(Int32.Parse(reader["id_birth_country"].ToString())),
                CityOfResidence = citiesRepo.GetCity(Int32.Parse(reader["id_city_residence"].ToString()), Int32.Parse(reader["id_country"].ToString())),
                Account = loginRepo.GetAccount(Int32.Parse(reader["id_user"].ToString())),
                Reputation = reputationRepo.GetReputation(Int32.Parse(reader["id_user"].ToString())),

            };

            return user;
        }

    }
}
