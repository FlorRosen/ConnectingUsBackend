using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ConnectingUsWebApp.Controllers
{
    public class ChatsController : ApiController
    {
        private static readonly ChatsRepository chatsRepo = new ChatsRepository();


        [HttpGet]
        public IEnumerable<Chat> GetChats(int idUser)
        {
            List<Chat> chats = chatsRepo.GetChats(idUser);
            return chats;
        }

        //Create a new chat
        [HttpPost]
        public IHttpActionResult Post([FromBody] Chat chat)
        {
            var ok = chatsRepo.AddChat(chat);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail to create chat");
        }


        //Create a new message
        [Route("api/chats/messages")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] Message message)
        {
            var ok = chatsRepo.AddMessage(message);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail add message to chat");
        }
        
        //close the chat
        [HttpPut]
        public IHttpActionResult Put([FromBody] Chat chat)
        {
            var ok = chatsRepo.UpdateChat(chat);

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail to update chat");
        }
    }
}
