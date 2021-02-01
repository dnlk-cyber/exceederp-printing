
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public partial class JobOrderDetail : OperationWithSecondValidation
    {
         [ScaffoldColumn(false)]
       public Guid JobOrderDetailID { get; set; }
        public Guid JobOrderID { get; set; }
        public Guid ServiceID { get; set; }
        [DisplayName("Cause")]
        public Guid JobCausesId { get; set; }
         [DisplayName("Service Name")]
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public ApprovalStatuses ApprovalStatus { get; set; }
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName("End Date")]
              [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public Nullable<DateTime> ToDate { get; set; }

 

        public bool IsApproved { get; set; }

        [NotMapped]
        public string Cause { get; set; }
        [NotMapped]
        public string Section { get; set; }
        [NotMapped]
        public decimal Efficency { get; set; }
        public decimal Odometer { get; set; }
        public virtual Service Service { get; set; }
        public virtual JobCauses JobCauses { get; set; }
      
    }
}
