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
    public class ReputationsRepository
    {
        SqlConnection connection;
        SqlCommand command;

        public ReputationsRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }


        //Get the average qualifications
        public Reputation GetReputation(int idUser)
        {
            var reputation = new Reputation();
            try
            {

                String query = "select (convert(decimal(9, 2), avg(cast(punctuation as float)))) as reputation, count(punctuation) as votes from qualifications where id_user_offertor = @id_user " +
                "GROUP BY id_user_offertor";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_user", idUser);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reputation = MapReputationsFromDB(reader);
                    }
                }
                connection.Close();

            }

            finally
            {
                connection.Close();
            }
            return reputation;
        }

        public static Reputation MapReputationsFromDB(SqlDataReader reader)
        {


            Reputation reputation = new Reputation
            {
               // UserId = Int32.Parse(reader["id_user"].ToString()),
                Average = Decimal.Parse(reader["reputation"].ToString()),
                Votes = Int32.Parse(reader["votes"].ToString()),

            };

            return reputation;
        }

    }
}
