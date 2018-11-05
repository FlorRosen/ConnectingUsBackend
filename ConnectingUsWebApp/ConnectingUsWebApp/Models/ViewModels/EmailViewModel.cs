using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConnectingUsWebApp.Models
{
    public class EmailViewModel
    {
        public string SubjectText { get; set; }
        public string BodyText { get; set; }
        public string ServiceTitle { get; set; }
        public string UserSenderMail { get; set; }
        public string UserReceiverMail { get; set; }
    }
}