using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.CRM.Models
{
    public class AssignTicketViewModel
    {
        [DisplayName("Id")]
        public int AssignTicketId { get; set; }
        [Required]
        public string TicketId { get; set; }
        [DisplayName("Ticket Name")]
        public string TicketName { get; set; }
        [Required]
        [DisplayName("Ticket Defintion")]
        public string TicketDefintion { get; set; }
        [Required]
        [DisplayName("Assined To")]
        public string AssinedTo { get; set; }

        public bool IsActive { get; set; }
        [DisplayName("Status Type")]
        public string StatusType { get; set; }

        [DisplayName("Assined Date")]
        [DataType(DataType.Date)]
      //  [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AssignedDate { get; set; }
        public bool IsClosed { get; set; }
        public bool IsTransfer { get; set; }
        [DisplayName("Transfer To")]
        public string TransferTo { get; set; }
          [NotMapped]
        [DisplayName("Status Color")]
        public string StatusColor { get; set; }
          [NotMapped]
        [DisplayName("Priority Color")]
        public string PriorityColor { get; set; }
         [NotMapped]
        [DisplayName("Customer")]
        public int CustomerId { get; set; }
          [NotMapped]
        public FinalizeType Status { get; set; }
        [NotMapped]
        public string Priority { get; set; }
          [NotMapped]
        [DisplayName("Ticket Group")]
        public string TicketGroup { get; set; }
        [NotMapped]
        [DisplayName(" Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DueDate { get; set; }

        [DisplayName(" Closed Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ClosedDate { get; set; }

        public string Age { get; set; }
    }

}