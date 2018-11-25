﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectingUsWebApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public String Date { get; set; }
        public int UserSenderId { get; set; }
        public int UserReceiverId { get; set; }
        public String Text { get; set; }
        public int IdChat { get; set; }
        public string UserSenderNickname { get; set; }
        public string UserReceiverNickname { get; set; }




    }
}