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

        void CreateConnection()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }


        public User LoginUser(LoginViewModel login){
            var user = new User();

            CreateConnection();
            command = new SqlCommand
            {
                Connection = connection,
                CommandText = "SELECT * FROM users us INNER JOIN accounts ac ON us.id_user = ac.id_user WHERE ac.mail = @mail"
            };
            command.Parameters.AddWithValue("@mail", login.Mail);

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    user = UsersRepository.MapUserFromDB(reader);
                }
            }
            return user;
        }
        
    }
}
