using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class TicketStatusLog_HelpDesk : TrackUserOperation
    {
        public int TicketStatusLog_HelpDeskId { get; set; }
        public int TicketId { get; set; }
        public string User { get; set; }
        public string Remark { get; set; }
    }
}
