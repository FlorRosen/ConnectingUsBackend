﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using ConnectingUsWebApp.Models;
using System.Web.Http;
using ConnectingUsWebApp.Models.ViewModels;

namespace ConnectingUsWebApp.Repositories
{
    public class QualificationsRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        public QualificationsRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        //Get all qualifications
        public List<Qualification> GetAllQualifications(int? idUser)
        {
            List<Qualification> qualifications = new List<Qualification>();

            try
            {


                String query = "select * from qualifications " +
                    " ((@id_user IS NULL) OR (@id_user IS NOT NULL AND id_user_ofertor = @id_user)) order by punctuation_date desc";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_user", idUser ?? Convert.DBNull);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Qualification qualification = MapQualificationsFromDB(reader);
                        qualifications.Add(qualification);
                    }
                }
                connection.Close();
            }
            finally
            {
                connection.Close();
            }
            return qualifications;
        }

        //Get the average qualifications
        public decimal GetAvgQualifications(int idUser)
        {
            decimal qualification = 0;
            try
            {

                String query = "select (convert(decimal(9, 2), avg(cast(punctuation as float)))) from qualifications where id_user_ofertor = @id_user " +
                "GROUP BY id_user_ofertor";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_user", idUser);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        qualification = reader.GetDecimal(0);
                    }
                }
                connection.Close();

            }
       
            finally
            {
                connection.Close();
            }
            return qualification;
            }

        //Add message to the chat. Sends the notification by mail
        public bool AddQualification(Chat chat)
        {

            var result = false;
            try
            {
                using (SqlCommand command_addQualification = new SqlCommand())
                {
                    connection.Open();

                    command_addQualification.Connection = connection;

                    command_addQualification.CommandText = "INSERT INTO qualifications (id_chat, id_user_requester,id_user_ofertor,punctuation,punctuation_date) " +
                     "VALUES (@id_chat, @id_user_requester,@id_user_ofertor,@punctuation,getdate())";

                    command_addQualification.Parameters.AddWithValue("@id_chat", chat.Id);
                    command_addQualification.Parameters.AddWithValue("@id_user_ofertor", chat.UserOfertorId);
                    command_addQualification.Parameters.AddWithValue("@id_user_requester", chat.UserRequesterId);
                    command_addQualification.Parameters.AddWithValue("@punctuation", chat.Qualification.QualificationNumber);


                    command_addQualification.ExecuteNonQuery();
                    connection.Close();
                    result = true;

                }
            }
            finally
            {
                connection.Close();
            }
            return result;
        }



        //Private Methods
        private Qualification MapQualificationsFromDB(SqlDataReader reader)
        {

            Qualification qualification = new Qualification
            {
                IdChat = Int32.Parse(reader["id_chat"].ToString()),
                UserRequesterId = Int32.Parse(reader["id_user_requester"].ToString()),
                UserOfertorId = Int32.Parse(reader["id_user_ofertor"].ToString()),
                QualificationNumber = Int32.Parse(reader["punctuation"].ToString()),
                QualificationDate = Convert.ToDateTime(reader["punctuation"].ToString()),

            };

            return qualification;
        }


    }
}