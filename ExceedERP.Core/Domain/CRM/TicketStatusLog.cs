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
