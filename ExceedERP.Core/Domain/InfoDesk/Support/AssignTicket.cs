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
        [Display(Name ="Assigned Employee")]
        public int AssinedTo {get; set;}
        public TicketPriority TicketPriority { get; set; }
        public bool IsClosed { get; set; }
        [DisplayName("Assigned Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AssignedDate { get; set; }
        [DisplayName("Closed Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ClosedDate { get; set; }
        [DisplayName("Age In Days")]
        public string Age { get; set; }
        [DisplayName("Remark")]
        public string Remarked { get; set; }
        // uses to check the status of the assignment, if it is assigned we hide the buttons based on the value of this field
        public bool IsAssigned { get; set; }
        //uses for to check if the problem is solved and send to assigner 
        public bool IsSendForClosing { get; set; }
        //uses for to sure the problem is solved 
        public bool IsSolved { get; set; }
        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }
        public string Solution { get; set; }
        // uses to know the assined problem of the user
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        //[ForeignKey("CustomerTicketId")]
        //public virtual CustomerTicket Ticket { get; set; }
        public int? MaterialItemsId { get; set; }
    }
}
