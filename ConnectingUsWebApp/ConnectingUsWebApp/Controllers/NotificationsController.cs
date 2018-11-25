using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using ConnectingUsWebApp.Models;
using ConnectingUsWebApp.Repositories;

namespace ConnectingUsWebApp.Controllers
{
    public class NotificationsController : ApiController
    {
        private static readonly NotificationsRepository notifRepo = new NotificationsRepository();

        //Set notification as read
        [HttpPut]
        public IHttpActionResult Put([FromBody] Notification notif)
        {
            var ok = notifRepo.UpdateNotification(notif.Id);
           
            return ok ? (IHttpActionResult)Ok() : Content(HttpStatusCode.BadRequest, "Fail to update chat");
        }

        [HttpGet]
        public IEnumerable<Notification> GetNotificationsForUser(int idUser)
        {
            List<Notification> notifications = notifRepo.GetNotificationsForUser(idUser);
            return notifications;
        }
    }
}
