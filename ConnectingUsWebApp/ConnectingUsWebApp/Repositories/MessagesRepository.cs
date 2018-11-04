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
    public class MessagesRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        public MessagesRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        //Public Methods

        //Brings messages of a chat order by date 
        public List<Message> GetMessages(int id_chat) 
        {
            List<Message> messages = new List<Message>();

           
            String query = "select * " +
                           "from messages where id_chat = @id_chat order by message_date desc";
            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id_chat", id_chat);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Message message = MapMessagesFromDB(reader);
                    messages.Add(message);
                }
            }
            connection.Close();
            return messages;
        }

        //Add message to the chat. Sends the notification by mail
        public bool AddMessage(Message message)
        {
            
            var result = false;

            using (SqlCommand command_addMeesage = new SqlCommand())
            {
               
                connection.Open();

                command_addMeesage.Connection = connection;

                command_addMeesage.CommandText = "INSERT INTO messages (id_chat, message,message_date,id_sender_user,id_receiver_user) " +
                 "VALUES (@id_chat,@text,getdate(), @id_user_sender,@id_user_receiver)";

                command_addMeesage.Parameters.AddWithValue("@id_chat", message.IdChat);
                command_addMeesage.Parameters.AddWithValue("@text", message.Text);
                command_addMeesage.Parameters.AddWithValue("@id_user_sender", message.UserSenderId);
                command_addMeesage.Parameters.AddWithValue("@id_user_receiver", message.UserReceiverId);
                

                command_addMeesage.ExecuteNonQuery();
                connection.Close();
                result = true;
            }
            return result;
        }


        //Private Methods
        private Message MapMessagesFromDB(SqlDataReader reader)
        {

            ChatsRepository chatsRepository = new ChatsRepository();
            UsersRepository usersRepository = new UsersRepository();

            Message message = new Message
            {
                Text = reader["message"].ToString(),
                UserReceiverId = Int32.Parse(reader["id_receiver_user"].ToString()),
                UserSenderId = Int32.Parse(reader["id_sender_user"].ToString()),
                Date = Convert.ToDateTime(reader["message_date"].ToString()),
                IdChat = Int32.Parse(reader["id_chat"].ToString())
                


    };

            return message;
        }
    }
}