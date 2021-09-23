using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.InfoDesk
{
    public class TicketType_HelpDesk : TrackUserSettingOperation
    {

        public int TicketType_HelpDeskId { get; set; }
        //  public TickeType Type { get; set; }

        [Required]
        [DisplayName("Ticket Type")]
        public String Type { get; set; }

        public string Description { get; set; }
    }
}
