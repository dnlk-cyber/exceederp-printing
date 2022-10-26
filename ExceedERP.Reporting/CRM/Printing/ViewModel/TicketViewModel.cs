using ExceedERP.Core.Domain.CRM;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.Printing.ViewModel
{
    public class TicketViewModel  : Organization
    {

 
        public int CustomerID { get; set; }
         public string CustomerAddress { get; set; }
         public string TicketDefintion { get; set; }
         public string Description { get; set; }
         public string AssignedTo { get; set; }
          public string StatusType { get; set; }
         public string StatusColor { get; set; }
        public string Status  { get; set; }
        public string PriorityColor { get; set; }
           public string Priority { get; set; }
         public string TicketGroup { get; set; }
          public string EscalationLevel { get; set; }
        public bool IsSendForClosing { get; set; }
        public string AssinedTo { get; set; }
        public string Age { get; set; }
        public DateTime CreatedOn { get; set; }
        [DisplayName(" Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        public DateTime DueDate { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ClosedDate { get; set; }
        public string Remark { get; set; }
         public string CreatedBy { get;  set; }
        public DateTime? DateCreated { get;  set; }
        public DateTime? LastModified { get;  set; }
        public int CustomerTicketID { get;  set; }
    }
}
