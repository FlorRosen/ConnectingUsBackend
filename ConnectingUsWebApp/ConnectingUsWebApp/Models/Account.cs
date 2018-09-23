using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectingUsWebApp.Models
{
    public class Account
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
    }
}