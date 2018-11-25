using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectingUsWebApp.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public Service Service { get; set; }
        public int UserRequesterId { get; set; }
        public string UserRequesterNickname { get; set; }
        public int UserOffertorId { get; set; }
        public string UserOffertorNickname { get; set; }
        public bool Active { get; set; }
        public List<Message> Messages { get; set; }
        public Qualification Qualification { get; set; }
        public String LastMessageDate { get; set; }

    }
}