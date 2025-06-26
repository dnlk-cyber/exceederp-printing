using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.InfoDesk
{
    public class TicketType_HelpDesk : TrackUserSettingOperation
    {

        public int TicketType_HelpDeskId { get; set; }
        //  public TickeType Type { get; set; }
        [Required]
        [DisplayName("Ticket Type")]
        public string Type { get; set; }
        [DisplayName("Ticket Code")]
        public string TicketCode { get; set; }
        [DisplayName("Due Days")]
        public decimal DueDays { get; set; }
        [DisplayName("Estimated Cost")]
        public decimal? EstimatedCost { get; set; }
        public TicketPriority TicketPriority { get; set; }
        public TicketCategory TicketCategory { get; set; }
        public string Description { get; set; }

    }

    public enum TicketCategory
    {
        Event,
        Problem,
        Issue,
        Incident,
        Change
    }
}
