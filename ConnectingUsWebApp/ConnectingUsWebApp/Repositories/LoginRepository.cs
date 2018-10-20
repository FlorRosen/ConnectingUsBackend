using System;
using System.Configuration;
using System.Data.SqlClient;
using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Models.ViewModels;

namespace ConnectingUsWebApp.Repositories
{
    public class LoginRepository
    {
        SqlConnection connection;
        SqlCommand command;

        public LoginRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }


        public User LoginUser(LoginViewModel login){
            var user = new User();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "SELECT * FROM users us INNER JOIN accounts ac ON us.id_user = ac.id_user " +
                "WHERE UPPER(ac.mail) = @mail and ac.password = @password"
            };
            command.Parameters.AddWithValue("@mail", login.Mail.ToUpper());
            command.Parameters.AddWithValue("@password", login.Password);

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    user = UsersRepository.MapUserFromDB(reader);
                }
            }

            connection.Close();
            return user;
        }

        public Account GetAccount(int userId){
            var account = new Account();

            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "SELECT * FROM accounts WHERE id_user = @id_user"
            };
            command.Parameters.AddWithValue("@id_user", userId);

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    account = MapAccountFromDB(reader);
                }
            }

            connection.Close();
            return account;
        }

        public Account MapAccountFromDB(SqlDataReader reader){

            Account account = new Account
            {
                Id = Int32.Parse(reader["id_user"].ToString()),
                Nickname = reader["nickname"].ToString()
            };

            return account;
        }
        
    }
}
