using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectingUsWebApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public User Sender { get; set; }
        public User Receriver { get; set; }

    }
}