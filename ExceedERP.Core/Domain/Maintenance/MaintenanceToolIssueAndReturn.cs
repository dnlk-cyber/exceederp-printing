using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class MaintenanceToolIssueAndReturn : TrackUserOperation
    {
        [Key]
       [ScaffoldColumn(false)]
        public Guid MaintenanceToolIssueAndReturnId { get; set; }
        [Display(Name ="Tool Name")]
        public Guid MaintenanceToolID { get; set; }
        [Display(Name ="Crew Name")]
        public Guid MechanicCrewID { get; set; }

        [Display(Name ="Quantity")]
        public decimal IssuedQuantity { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime IssueDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReturnDate { get; set; }
        public string Remark { get; set; }

        public virtual MaintenanceTool MaintenanceTool { get; set; }
        public virtual MechanicCrew  MechanicCrew { get; set; }
        public MaintenanceToolStatus Status { get; set; }
    }
    public enum MaintenanceToolStatus
    {
        Issued,
        Returned
    }
}
