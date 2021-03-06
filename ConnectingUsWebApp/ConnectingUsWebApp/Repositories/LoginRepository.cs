﻿using System;
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
                CommandText = "SELECT * FROM users us INNER JOIN accounts ac ON us.id_user = ac.id_user WHERE UPPER(ac.mail) = @mail " +
                "AND ac.password = (CONVERT(varchar(256),HASHBYTES('SHA2_256', @password),2))"
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
        //Private Methods
        private Account MapAccountFromDB(SqlDataReader reader){

            Account account = new Account
            {
                Id = Int32.Parse(reader["id_user"].ToString()),
                Mail = reader["mail"].ToString(),
                Nickname = reader["nickname"].ToString(),
               // Password = reader["password"].ToString()
            };

            return account;
        }
        
    }
}
