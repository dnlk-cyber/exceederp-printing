
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ExceedERP.Core.Domain.Global
{
    public class GlobalNotification
    {
        public int GlobalNotificationId { get; set; }
        public string IpAddress { get; set; }
        public string ResponsibleRoles { get; set; }
        public string ResponsibleUserName { get; set; }
         [DataType(DataType.DateTime)]
        public DateTime AlarmGeneratedTime { get; set; }
         [DataType(DataType.DateTime)]
        public DateTime DeadLine { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
  
}
