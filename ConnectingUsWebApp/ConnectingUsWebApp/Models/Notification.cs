using System;
namespace ConnectingUsWebApp.Models
{
    public class Notification
    {
       
        public int Id { get; set; }
        public string NicknameUserSender { get; set; }
        public int IdUserNotify { get; set; }
        public int IdType { get; set; }
        public string ServiceTitle { get; set; }
        public bool IsRead { get; set; }

    }
}
