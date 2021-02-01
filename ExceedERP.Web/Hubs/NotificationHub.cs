using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Microsoft.AspNet.SignalR;

namespace ExceedERP.Web.Hubs
{
    public class UserHubModels
    {
        public string UserName { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }

  

    public class NotificationHub:Hub
    {
        private static readonly ConcurrentDictionary<string, UserHubModels> Users =
            new ConcurrentDictionary<string, UserHubModels>(StringComparer.InvariantCultureIgnoreCase);

        private ExceedDbContext context = new ExceedDbContext();

        //Logged Use Call
        //public void GetNotification()
        //{
        //    try
        //    {
        //        string loggedUser = Context.User.Identity.Name;

        //        //Get TotalNotification  
        //        NotifRealTime totalNotif = LoadNotifData(loggedUser);

        //        //Send To  
        //        UserHubModels receiver;
        //        if (Users.TryGetValue(loggedUser, out receiver))
        //        {
        //            var cid = receiver.ConnectionIds.FirstOrDefault();
        //            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        //            context.Clients.Client(cid).broadcaastNotif(totalNotif);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //    }
        //}

        //Specific User Call
        //public void SendNotification(string SentTo)
        //{
        //    try
        //    {
        //        //Get TotalNotification  
        //        NotifRealTime totalNotif = LoadNotifData(SentTo);

        //        //Send To  
        //        UserHubModels receiver;
        //        if (Users.TryGetValue(SentTo, out receiver))
        //        {
        //            var cid = receiver.ConnectionIds.FirstOrDefault();
        //            var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        //            context.Clients.Client(cid).broadcaastNotif(totalNotif);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //    }
        //}

        private NotifRealTime LoadNotifData(string userId)
        {
            NotifRealTime notifRealTime = new NotifRealTime();

           var message = context.Notifications
               .Where(x => x.SentTo == userId)
               .Where(x => x.IsRead == false)
               .ToList();

            notifRealTime.Count = message.Count;
            notifRealTime.List = new List<NotifRealTimeData>();

            //for iteration
            var messageList = message
                .OrderByDescending(x => x.Id)
                .Take(10)
                .ToList();

            foreach (var m in messageList)
            {
                var data = new NotifRealTimeData
                {
                    Message = m.Details,
                    Url = m.DetailsUrl
                };
                notifRealTime.List.Add(data);
            }
            return notifRealTime;
        }

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new UserHubModels
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userName);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            UserHubModels user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        UserHubModels removedUser;
                        Users.TryRemove(userName, out removedUser);
                        Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}