using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
namespace ExceedERP.Core.Domain.CRM
{
    public class TicketStatusLog : TrackUserOperation
    {
        public int TicketStatusLogId { get; set; }
        public int TicketId { get; set; }
        public string User { get; set; }
        public string Remark { get; set; }
    }
}
