using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using ConnectingUsWebApp.Enums;
using ConnectingUsWebApp.Models;

namespace ConnectingUsWebApp.Repositories
{
    public class NotificationsRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        public NotificationsRepository()
        {
            string constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        public List<Notification> GetNotificationsForUser (int idUser){
            List<Notification> notifications = new List<Notification>();

            try
            {
                String query = "select * " +
                    "from notifications where id_user_notify = @id_user_notify and isRead = 0 ";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_user_notify", idUser);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Notification notif = MapNotificationFromBackend(reader);
                        notifications.Add(notif);
                    }
                }
                connection.Close();
            }
            finally
            {
                connection.Close();
            }
            return notifications;
        }

        public bool AddNotification(int type, int idUserSender, int idUserReceiver, int idChat){
            var result = false;

            try
            {
                using (SqlCommand command_addNotification = new SqlCommand())
                {
                    connection.Open();

                    command_addNotification.Connection = connection;

                    command_addNotification.CommandText = "INSERT INTO notifications (id_user_sender, id_user_notify, id_type, id_chat, isRead) " +
                     " VALUES (@id_user_sender, @id_user_notify, @id_type, @id_chat, @isRead)";

                    command_addNotification.Parameters.AddWithValue("@id_user_sender", idUserSender);
                    command_addNotification.Parameters.AddWithValue("@id_user_notify", idUserReceiver);
                    //TODO: Cambiar el proximo
                    command_addNotification.Parameters.AddWithValue("@id_type", type);
                    command_addNotification.Parameters.AddWithValue("@id_chat", idChat);
                    command_addNotification.Parameters.AddWithValue("@isRead", 0);
                   
                    command_addNotification.ExecuteNonQuery();
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

        public bool UpdateNotification(int idNotif)
        {
            var result = false;

            try
            {
                using (SqlCommand command_UpdateNotif = new SqlCommand())
                {
                    connection.Open();

                    command_UpdateNotif.Connection = connection;

                    command_UpdateNotif.CommandText = "UPDATE notifications SET isRead = @isRead " +
                     "WHERE id_notification = @id_notification";
                    command_UpdateNotif.Parameters.AddWithValue("@id_notification", idNotif);
                    command_UpdateNotif.Parameters.AddWithValue("@isRead", 1);

                    command_UpdateNotif.ExecuteNonQuery();
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
        private Notification MapNotificationFromBackend(SqlDataReader reader)
        {
            UsersRepository usersRepository = new UsersRepository();

            Notification notif = new Notification
            {
                Id = Int32.Parse(reader["id_notification"].ToString()),
                UserSender = usersRepository.GetUser(Int32.Parse(reader["id_user_sender"].ToString())),
                IdUserNotify = Int32.Parse(reader["id_user_notify"].ToString()),
                IdType = Int32.Parse(reader["id_type"].ToString()),
                IdChat = Int32.Parse(reader["id_chat"].ToString())
            };

            return notif;
        }
    }
}
