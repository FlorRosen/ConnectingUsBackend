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
using Newtonsoft.Json;

namespace ConnectingUsWebApp.Repositories
{
    public class ChatsRepository
    {
        private SqlConnection connection;
        private SqlCommand command;

        public ChatsRepository()
        {
            var constr = ConfigurationManager.ConnectionStrings["AzureConnection"].ToString();
            connection = new SqlConnection(constr);
        }

        public List<Chat> GetChats(int? idUserOffertor,int? idUserRequester)
        {
            List<Chat> chats = new List<Chat>();

            try
            {
                String query = "select * from chats " +
                    "where id_user_requester = @id_user_requester or id_user_offertor = @id_user_offertor " +
                    "order by last_message_date desc";
                command = new SqlCommand(query, connection);
                if (idUserOffertor != null)
                {
                    command.Parameters.AddWithValue("@id_user_offertor", idUserOffertor);
                    command.Parameters.AddWithValue("@id_user_requester", Convert.DBNull);
                }
                else
                {
                    command.Parameters.AddWithValue("@id_user_requester", idUserRequester);
                    command.Parameters.AddWithValue("@id_user_offertor", Convert.DBNull);
                }
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
            }
            finally
            {
                connection.Close();
            }
            return chats;
        }

        public Chat GetChat(int idChat)
        {
            Chat chat = new Chat();
            try
            {

                String query = "select * from chats " +
                    "where id_chat = @id_chat";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_chat", idChat);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chat = MapChatFromDB(reader);
                    }
                }

                connection.Close();
            }
            finally
            {
                connection.Close();
            }
            return chat;
        }



        public Chat GetChatByUser(GetChatViewModel getChatViewModel)
        {
            Chat chat = new Chat();
            try
            {

                String query = "select * from chats " +
                    "where id_service = @id_service and id_user_requester = @id_user_requester and active = 1";
                command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id_service", getChatViewModel.IdService);
                command.Parameters.AddWithValue("@id_user_requester", getChatViewModel.IdUser);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chat = MapChatFromDB(reader);
                    }
                }

                connection.Close();
            }
            finally
            {
                connection.Close();
            }
            return chat;
        }

        //Create new chat
        public Chat AddChat(Chat chat)
        {
            
            var result = false;
            try
            {
                using (SqlCommand command_addChat = new SqlCommand())
                {
                    connection.Open();

                    command_addChat.Connection = connection;

                    command_addChat.CommandText = "INSERT INTO chats (id_service, id_user_requester,id_user_offertor,last_message_date) " +
                     "OUTPUT INSERTED.id_chat  VALUES (@id_service, @id_user_requester,@id_user_offertor,@last_message_date)";

                    command_addChat.Parameters.AddWithValue("@id_service", chat.Service.Id);
                    command_addChat.Parameters.AddWithValue("@id_user_offertor", chat.UserOffertorId);
                    command_addChat.Parameters.AddWithValue("@id_user_requester", chat.UserRequesterId);
                    command_addChat.Parameters.AddWithValue("@last_message_date", chat.LastMessageDate);

                    SqlParameter param = new SqlParameter("@id_chat", SqlDbType.Int, 4)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command_addChat.Parameters.Add(param);

                    int chatId = (int)command_addChat.ExecuteScalar();
                    
                    connection.Close();
                    result = true;
                    chat = GetChat(chatId);


                }
            }
            finally
            {
                connection.Close();
            }
            return chat;
        }

        public Message AddMessage(Message message)
        {
            MessagesRepository messagesRepository = new MessagesRepository();               
            return messagesRepository.AddMessage(message);
        }


        //DELETE chat and messages
        public bool DeleteChat(int idChat)
        {
            
            var result = false;

            using (SqlCommand command_deleteChat = new SqlCommand())
            {
                connection.Open();

                command_deleteChat.Connection = connection;

                //delete messages
                command_deleteChat.CommandText = "delete from messages where id_chat = @id_chat";
                command_deleteChat.Parameters.AddWithValue("@id_chat", idChat);
                command_deleteChat.ExecuteNonQuery();
                //delete chat
                command_deleteChat.CommandText = "delete from chats where id_chat = @id_chat";
                command_deleteChat.Parameters.AddWithValue("@id_chat",idChat);
                command_deleteChat.ExecuteNonQuery();


                connection.Close();
                result = true;
            }

            return result;
        }


        //Close the chat and insert the qualification
        public bool UpdateChat(Chat chat) //preguntar si puede pasarme solo el id del chat y no todo el objeto
        {
            
            var result = false;
            try
            {
                using (SqlCommand command_UpdateChat = new SqlCommand())
                {
                    connection.Open();

                    command_UpdateChat.Connection = connection;

                    command_UpdateChat.CommandText = "UPDATE chats SET active = 0 " +
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
                    //if the qualifications is canceled|
                    if (chat.Qualification != null)
                    {
                        QualificationsRepository qualificationsRepository = new QualificationsRepository();
                        NotificationsRepository notifRepo = new NotificationsRepository();

                        result = qualificationsRepository.AddQualification(chat);
                        notifRepo.AddNotification(2, chat.UserRequesterId, chat.UserOffertorId, chat.Id);
                        
                    }
                }
            }
            finally
            {
                connection.Close();
            }
            return result;
        }
        //Private Methods
        private Chat MapChatFromDB(SqlDataReader reader)
        {

            UsersRepository usersRepository = new UsersRepository();
            ServicesRepository servicesRepository = new ServicesRepository();
            MessagesRepository messagesRepository = new MessagesRepository();

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ"

            };

            Chat chat = new Chat
            {
                Id = Int32.Parse(reader["id_chat"].ToString()),
                UserRequesterId = Int32.Parse(reader["id_user_requester"].ToString()),
                UserOffertorId = Int32.Parse(reader["id_user_offertor"].ToString()),
                Active = Boolean.Parse(reader["active"].ToString()),
                Service = servicesRepository.GetServices(Int32.Parse(reader["id_service"].ToString()),null,null).First<Service>(),
                Messages = messagesRepository.GetMessages(Int32.Parse(reader["id_chat"].ToString())),
                UserRequesterNickname = usersRepository.GetUser(Int32.Parse(reader["id_user_requester"].ToString())).Account.Nickname,
                UserOffertorNickname = usersRepository.GetUser(Int32.Parse(reader["id_user_offertor"].ToString())).Account.Nickname,
                LastMessageDate = JsonConvert.SerializeObject(reader["last_message_date"], settings).Replace("\"", ""),
            };

            return chat;
        }
    }
}