 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class CustomerTicket : TrackUserOperation
    {
        [Key]
        public int CustomerTicketID { get; set; }
        [Required]
        [DisplayName("Customer")]
        public string CustomerID { get; set; }

        public string Description { get; set; }

        public TicketPriorityType Priority { get; set; }
            [DisplayName("Ticket Group")]
        public string TicketGroup { get; set; }
              [DisplayName(" Escalation Level")]
        public string EscalationLevel { get; set; }

        public CustomerTicketSourceType Source { get; set; }
           [DisplayName(" Created On ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedOn { get; set; }
          [DisplayName(" Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DueDate { get; set; }

        public string Remark { get; set; }


    }
    

   
    public class CustomerTicketView
    {
        [Key]
        public int CustomerTicketID { get; set; }
        public string CustomerID { get; set; }

        public string Description { get; set; }

        public TicketPriorityType Priority { get; set; }

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

    public class TicketActivityDefinition : TrackUserOperation
    {
        public int TicketActivityDefinitionID { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public int Index { get; set; }


        public bool CanFinalize { get; set; }
       
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

    public enum TicketPriorityType
    {
        Low,
        Medium,
        Urgent,
        TopUrgent
    }

    public enum CustomerTicketSourceType
    {
        Agent,
        Customer
    }
}
