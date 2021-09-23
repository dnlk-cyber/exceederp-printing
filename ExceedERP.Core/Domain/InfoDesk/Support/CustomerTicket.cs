using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class CustomerTicket_HelpDesk : TrackUserOperation
    {

        public int CustomerTicket_HelpDeskId { get; set; }
        [NotMapped]
        [DisplayName("Customer Adress")]
        public string CustomerAddress { get; set; }
        [DisplayName("Ticket Type")]
        public int? SpecificTicketTypeId { get; set; }     
  
        public string Description { get; set; }

        [DisplayName("Assigned To")]
        public string AssignedTo { get; set; }

        public string Ticket{get;set;}         
       
        [DisplayName("Priority Color")]
        public string PriorityColor { get; set; }
        public InfoDeskFinalizeType Status { get; set; }
       
        public string Priority { get; set; }
        [DisplayName("Group")]
        public string TicketGroup { get; set; }
        [DisplayName(" Due Date")]
    
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        public DateTime? DueDate { get; set; }
        [DisplayName("Closed Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ClosedDate { get; set; }

        [DisplayName("Solution Given")]
        public string SolutionGiven { get; set; }        

        [DisplayName("Employee")]
        public int? EmployeeId { get; set; }
        public string Remark { get; set; }
        [DisplayName("Age In Days")]
        public string AgeOfTicket { get; set; }
        public bool IsConfirmedByTheRequester { get; set; }
        public int Rate { get; set; }
        public bool IsClosed { get; set; }

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
   
        Customer, Employee,
        Unknown
    }

    public enum TicketTiming
    {
        On_Time ,Late
    }
    public enum SourceTicket {

    Direct,In_Direct}
}
