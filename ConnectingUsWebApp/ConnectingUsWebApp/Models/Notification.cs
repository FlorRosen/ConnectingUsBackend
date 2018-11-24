using System;
namespace ConnectingUsWebApp.Models
{
    public class Notification
    {
       
        public int Id { get; set; }
        public User UserSender { get; set; }
        public int IdUserNotify { get; set; }
        public int IdType { get; set; }
        public int IdChat { get; set; }
        public bool IsRead { get; set; }

    }
}
