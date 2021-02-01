using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class AssignTicket_HelpDesk : Operations
    {
        public int AssignTicket_HelpDeskId { get; set; }
        public int CustomerTicketId { get; set; }
       // public int? TicketDefintion { get; set; }
        public int AssinedTo {get; set;}
        //public bool IsTransfer { get; set; }
       // public bool? IsActive { get; set; }
        public bool IsClosed { get; set; }
      //  [DisplayName("Transfer To")]
    //  public string TransferTo { get; set; }
        [DisplayName("Assigned Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AssignedDate { get; set; }
        [DisplayName("Closed Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ClosedDate { get; set; }
        public string Age { get; set; }
        [DisplayName("Remark")]
        public string Remarked { get; set; }
        // uses to check the status of the assignment, if it is assigned we hide the buttons based on the value of this field
        public bool IsAssigned { get; set; }
        //uses for to check if the problem is solved and send to assigner 
        public bool IsSendForClosing { get; set; }
        //uses for to sure the problem is solved 
        public bool IsSolved { get; set; }
        public string Solution { get; set; }
        // uses to know the assined problem of the user
        public string Role { get; set; }
        //[ForeignKey("CustomerTicketId")]
        //public virtual CustomerTicket Ticket { get; set; }
    }
}
