using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Global
{
    
    public class GlobalExportLog
    {
        public int GlobalExportLogId { get; set; }
        [DisplayName("IP Address")]
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public DateTime TimeAccessed { get; set; }
        public string Type { get; set; }

        public string Status { get; set; }
        public string Description { get; set; }
    }
}
