
using System;
using System.ComponentModel;


namespace ExceedERP.Core.Domain.Global
{
    public class GlobalDeleteLog
    {
        public int GlobalDeleteLogId { get; set; }
        [DisplayName("IP Address")]
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public DateTime TimeAccessed { get; set; }
        public string Type { get; set; }

        public string Status { get; set; }
        public string Description { get; set; }
    }
  
}
