using System;
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
    public class ChatsRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        private void CreateConnection()
        {
            string constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }


        public List<Chat> GetChats(int idUser)
        {
            List<Chat> chats = new List<Chat>();

            CreateConnection();
            String query = "select * from chats " +
                "where id_user_requester = @id_user or id_user_ofertor = @id_user";
            command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id_user", idUser);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Chat chat = MapChatFromDB(reader);
                    chats.Add(chat);
                }
            }
            connection.Close();
            return chats;
        }

      

        //Create new chat
        public bool AddChat(Chat chat)
        {
            CreateConnection();
            var result = false;

            using (SqlCommand command_addChat = new SqlCommand())
            {
                connection.Open();

                command_addChat.Connection = connection;

                command_addChat.CommandText = "INSERT INTO chats (id_service, id_user_requester,id_user_ofertor) " +
                 "VALUES (@id_service, @id_user_requester,@id_user_ofertor)";

                command_addChat.Parameters.AddWithValue("@id_service", chat.Service.Id);
                command_addChat.Parameters.AddWithValue("@id_user_ofertor", chat.UserOfertorId);
                command_addChat.Parameters.AddWithValue("@id_user_requester", chat.UserRequesterId);

                command_addChat.ExecuteNonQuery();
                connection.Close();
                result = true;

            }
            return result;
        }

        public bool AddMessage(Message message)
        {
            MessagesRepository messagesRepository = new MessagesRepository();               
            return messagesRepository.AddMessage(message);
        }


        //DELETE chat and messages
        public bool DeleteChat(Chat chat)
        {
            CreateConnection();
            var result = false;

            using (SqlCommand command_deleteChat = new SqlCommand())
            {
                connection.Open();

                command_deleteChat.Connection = connection;
                //delete chat
                command_deleteChat.CommandText = "delete from chats where id_chat = @id_chat";
                command_deleteChat.Parameters.AddWithValue("@id_chat", chat.Id);
                command_deleteChat.ExecuteNonQuery();

                //delete messages
                command_deleteChat.CommandText = "delete from messages where id_chat = @id_chat";
                command_deleteChat.Parameters.AddWithValue("@id_chat", chat.Id);
                command_deleteChat.ExecuteNonQuery();

                connection.Close();
                result = true;
            }

            return result;
        }


        //Close the chat
        public bool UpdateChat(Chat chat)
        {
            CreateConnection();
            var result = false;
            using (SqlCommand command_UpdateChat = new SqlCommand())
            {
                connection.Open();

                command_UpdateChat.Connection = connection;

                command_UpdateChat.CommandText = "UPDATE chats SET active = @active " +
                 "WHERE  id_chat = @id_chat";

                command_UpdateChat.Parameters.AddWithValue("@id_chat", chat.Id);
                
                //The active column is a BIT in the database
                if (chat.Active)
                {
                    command_UpdateChat.Parameters.AddWithValue("@active", 1);
                }
                else
                {
                    command_UpdateChat.Parameters.AddWithValue("@active", 0);
                }

                command_UpdateChat.ExecuteNonQuery();
                connection.Close();
                result = true;

            }
            return result;
        }
        //Private Methods
        private Chat MapChatFromDB(SqlDataReader reader)
        {

            UsersRepository usersRepository = new UsersRepository();
            ServicesRepository servicesRepository = new ServicesRepository();
            MessagesRepository messagesRepository = new MessagesRepository();

            Chat chat = new Chat
            {
                Id = Int32.Parse(reader["id_chat"].ToString()),
                UserRequesterId = Int32.Parse(reader["id_user_requester"].ToString()),
                UserOfertorId = Int32.Parse(reader["id_user_ofertor"].ToString()),
                Active = Boolean.Parse(reader["active"].ToString()),
                Service = servicesRepository.GetServices(Int32.Parse(reader["id_service"].ToString()),null,null).First<Service>(),
                Messages = messagesRepository.GetMessages(Int32.Parse(reader["id_chat"].ToString()))
            };

            return chat;
        }
    }
}