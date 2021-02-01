using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Hubs
{
    public  class Notify
    {
        public static readonly ExceedDbContext Db = new ExceedDbContext();
        public static void NotifyUser(NotifModels obj)
        {
            NotificationHub objNotifHub = new NotificationHub();
            Notification objNotif = new Notification
            {
                SentTo = obj.UserId,
                Title = obj.Title,
                Details = obj.Message,
                Date = DateTime.Now,
                DetailsUrl = obj.URL,
                DeadLine =obj.DeadLine.ToString(CultureInfo.InvariantCulture)
            };

            Db.Configuration.ProxyCreationEnabled = false;
            Db.Notifications.Add(objNotif);
            Db.SaveChanges();

           //objNotifHub.SendNotification(objNotif.SentTo);

            //var query = (from t in Db.Notifications
            //    select t).ToList();


        }
    }
}