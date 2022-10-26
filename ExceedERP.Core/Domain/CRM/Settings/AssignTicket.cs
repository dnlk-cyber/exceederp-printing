using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class AssignTicket : Operations
    {
        public int AssignTicketId { get; set; }
        public int CustomerTicketId { get; set; }
        public string TicketDefintion { get; set; }
        public string AssinedTo {get; set;}
        public bool IsTransfer { get; set; }
        public bool IsActive { get; set; }
        public bool IsClosed { get; set; }
         [DisplayName("Transfer To")]
        public string TransferTo { get; set; }
        [DisplayName("Assined Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AssignedDate { get; set; }
        [DisplayName("Closed Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ClosedDate { get; set; }
        public string Age { get; set; }

        //[ForeignKey("CustomerTicketId")]
        //public virtual CustomerTicket Ticket { get; set; }
    }
}
