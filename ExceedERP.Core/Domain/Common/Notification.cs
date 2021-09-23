using System;
using System.Collections.Generic;

namespace ExceedERP.Core.Domain.Common
{
    public class UserVM
    {
        public string UserName { get; set; }
        public string FullName { get; set; }        
        public string UserId { get; set; }
    }
    public class NotifModels
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string URL { get; set; }
        public DateTime DeadLine { get; set; }
    }

    public class NotifRealTime
    {
        public int Count { get; set; }
        public List<NotifRealTimeData> List { get; set; }
    }
    public class NotifRealTimeData
    {
        public string Url { get; set; }
        public string Message { get; set; }
    }

    public class Notification
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Details { get; set; }
        public string Title { get; set; }
        public string DetailsUrl { get; set; }
        public string SentTo { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsReminder { get; set; }
        public string Code { get; set; }
        public string NotificationType { get; set; }
        public string IpAddress { get; set; }
        public string ResponsibleRoles { get; set; }
        public string DeadLine { get; set; }
    }
}
