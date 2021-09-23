using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class CustomerTicket : TrackUserOperation
    {
        [Key]
        public int CustomerTicketID { get; set; }
        [Required]
        [DisplayName("Customer")]
        public int CustomerID { get; set; }
        [NotMapped]
        [DisplayName("Customer Address")]
        public string CustomerAddress {get; set;}
             [DisplayName("Ticket Defintion")]
        public string TicketDefintion { get; set; }
        [Required]
        public string Description { get; set; }
        [DisplayName("Assigned To")]
        public string AssignedTo { get; set; }
        [Required]
        [DisplayName("Status")]
        public string StatusType { get; set; }
        [DisplayName("Status Color")]
        public string StatusColor { get; set; }
        [DisplayName("Priority Color")]
        public string PriorityColor { get; set; }
        public FinalizeType Status { get; set; }
        [Required]
        public string Priority { get; set; }
        [DisplayName("Group")]
        public string TicketGroup { get; set; }
        [DisplayName(" Escalation Level")]
        public string EscalationLevel { get; set; }
        public bool IsSendForClosing { get; set; }
        public CustomerTicketSourceType Source { get; set; }
        [DisplayName(" Created On ")]
        [DataType(DataType.Date)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss }")]
        public DateTime CreatedOn { get; set; }
        [DisplayName(" Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        public DateTime DueDate { get; set; }
        [DisplayName("Closed Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ClosedDate { get; set; }
        public string Remark { get; set; }

        [ForeignKey("CustomerID")]
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }

    }



    public class CustomerTicketView
    {
        [Key]
        public int CustomerTicketID { get; set; }
        public int CustomerID { get; set; }
        public string TicketDefintion { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        public string TicketGroup { get; set; }

        public string EscalationLevel { get; set; }

        public CustomerTicketSourceType Source { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DueDate { get; set; }

        public string Remark { get; set; }

    }


    public class TicketAssignment : TrackUserOperation
    {
        public int TicketAssignmentID { get; set; }

        public string TicketID { get; set; }

        public string TicketActivityDefinitionID { get; set; }

        public string AssignedBy { get; set; }
        public string ProcessedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AssignmentDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }
        public string Remark { get; set; }
    }

    public class TicketRole : TrackUserOperation
    {
        public int TicketRoleID { get; set; }
        public string TicketActivityDefinitionID { get; set; }
        public string RoleID { get; set; }

        public string TicketGroup { get; set; }
        public bool NeedsPassKey { get; set; }

        public string Remark;
    }

    //public enum TicketPriorityType
    //{
    //    Low,
    //    Medium,
    //    Urgent,
    //    TopUrgent
    //}

    public enum CustomerTicketSourceType
    {
        Agent,
        Customer
    }
}
