using ConnectingUsWebApp.Enums;
using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Models.ViewModels;
using ConnectingUsWebApp.Repositories;
using ConnectingUsWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace ConnectingUsWebApp.Controllers
{
    [Authorize]
    public class ChatsController : ApiController
    {
        private static readonly ChatsRepository chatsRepo = new ChatsRepository();
        private static readonly NotificationsRepository notifRepo = new NotificationsRepository();

        //Get chats when idUser is the provider of a service
        [HttpGet]
        [Route("api/chats/offertor")]
        public IEnumerable<Chat> GetChatsOffertor(int idUser)
        {
            List<Chat> chats = chatsRepo.GetChats(idUser,null);
            return chats;
        }

        //Get chats when idUser is the requester of a service
        [HttpGet]
        [Route("api/chats/requester")]
        public IEnumerable<Chat> GetChatsRequester(int idUser)
        {
            List<Chat> chats = chatsRepo.GetChats(null,idUser);
            return chats;
        }


        //Create a new chat
        [HttpPost]
        public Chat Post([FromBody] Chat chat)
        {
            return chatsRepo.AddChat(chat);
        }

        
        //Create a new message
        //if message is created correctly, it sends the email via smtp
        [Route("api/chats/messages")]
        [HttpPost]
        public Message Post([FromBody] Message message)
        {
            var ok = chatsRepo.AddMessage(message);

            if (ok != null){
                //TODO: Change notification type
                notifRepo.AddNotification(1, message.UserSenderId, message.UserReceiverId, message.IdChat);
            }

            return ok;
        }

        //Close the chat
        [HttpPut]
        public IHttpActionResult Put([FromBody] Chat chat)
        {
            var ok = chatsRepo.UpdateChat(chat);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail to update chat");
        }

        //get chat by id
        [Route("api/chats/chat")]
        [HttpPost]
        public int? GetChatByUser(GetChatViewModel getChatViewModel)
        {
            int? result = null;
            Chat chat = chatsRepo.GetChatByUser(getChatViewModel); ;
            if (chat.Id != 0)
            {
                result = chat.Id;
            }
            return result;
        }

        //get chat by id
        [Route("api/chats/chat")]
        [HttpGet]
        public Chat GetChat(int idChat)
        {
            Chat chat = chatsRepo.GetChat(idChat);
            return chat;
        }
    }
}
