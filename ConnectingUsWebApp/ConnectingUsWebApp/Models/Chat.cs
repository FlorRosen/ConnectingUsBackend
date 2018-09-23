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
        public User UserRequester { get; set; }
        public User UserOfertor { get; set; }
        public bool Active { get; set; }

    }
}