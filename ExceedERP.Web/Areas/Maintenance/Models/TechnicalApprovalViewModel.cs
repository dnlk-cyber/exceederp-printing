using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.Maintenance.Models
{
    public class TechnicalApprovalViewModel : TrackUserOperation
    {

        
        public Guid JobOrderID { get; set; }
        [DisplayName("Plate Number")]
        public Guid FleetsID { get; set; }
        [DisplayName("Job Order Number ")]

        public Nullable<int> JobOrderNum { get; set; }
        [Required]
        [DisplayName("Maintenance Center ")]
        public string Type { get; set; }
        [Required]
        public string MaintenanceCenter { get; set; }
        [Display(Name ="Maintenance Center")]
        public Guid? MaintenanceCenterId { get; set; } //Added to create a r/n ship
        [Required]
        [DisplayName("Project Name")]
        public string ProjectName { get; set; }
        public string PercentCompleted { get; set; }
        public string KMReading { get; set; }
        public string GPSInfo { get; set; }
        [DisplayName("Date Issued  ")]
        public Nullable<DateTime> DateIssued { get; set; }
        [DisplayName("Date Completed   ")]
        public Nullable<DateTime> DateCompleted { get; set; }
        public string InspectedBy { get; set; }
        public ApprovalStatuses ApproveStatus { get; set; }
    
       [Required]
        [DisplayName(" Reason For Job ")]
        [DataType(DataType.MultilineText)]
        public string InspectorComments { get; set; }
        public bool IsFromWorkShop { get; set; }
        public bool IsApproved { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
