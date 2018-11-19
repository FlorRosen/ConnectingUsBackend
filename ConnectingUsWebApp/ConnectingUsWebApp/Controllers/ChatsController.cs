using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Repositories;
using ConnectingUsWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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
        //if message is created correctly, it sends the email via smtp
        [Route("api/chats/messages")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] Message message)
        {
            var ok = chatsRepo.AddMessage(message);

            /*if (ok) {

                ChatsRepository chatsRepository = new ChatsRepository();
                UsersRepository usersRepository = new UsersRepository();

                EmailViewModel email = new EmailViewModel();
                email.SubjectText = "New Message";
                email.BodyText = "send you a message for service: ";
                email.ServiceTitle = chatsRepository.GetChat(message.IdChat).Service.Title;
                email.UserSenderMail = usersRepository.GetUser(message.UserSenderId).Account.Mail;
                email.UserReceiverMail = usersRepository.GetUser(message.UserReceiverId).Account.Mail;

                try
                {
                    EmailService.SendEmailViaWebApi(email);
                } 
                catch(SmtpFailedRecipientException ex)
                {
                    throw new SmtpFailedRecipientException("Failed while sending the email");
                }
            }*/

            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail add message to chat");
        }

        //close the chat
        [HttpPut]
        public IHttpActionResult Put([FromBody] Chat chat)
        {
            var ok = chatsRepo.UpdateChat(chat);
            /*if (ok)
            {
                UsersRepository usersRepository = new UsersRepository();

                EmailViewModel email = new EmailViewModel();
                email.SubjectText = "New qualification";
                email.BodyText = "has qualified you for service: ";
                email.ServiceTitle = chat.Service.Title;
                email.UserSenderMail = usersRepository.GetUser(chat.UserRequesterId).Account.Mail;
                email.UserReceiverMail = usersRepository.GetUser(chat.UserOfertorId).Account.Mail;
                try
                {
                    EmailService.SendEmailViaWebApi(email);
                }
                catch (SmtpFailedRecipientException ex)
                {
                    throw new SmtpFailedRecipientException("Failed while sending the email");
                } 
        } */
            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail to update chat");
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
