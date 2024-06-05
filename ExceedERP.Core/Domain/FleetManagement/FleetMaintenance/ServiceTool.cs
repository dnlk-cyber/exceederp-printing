using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class ServiceTool : Operations
    {

        public Guid ServiceToolID { get; set; }
        [DisplayName("Service")]
        [Required]
        public Guid ServiceID { get; set; }
        public Guid MaintenanceToolID { get; set; }
        public string ServiceName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Date { get; set; }
        [DisplayName("Status")]
        public ApprovalStatuses ApprovalStatus { get; set; }


        public virtual MaintenanceTool MaintenanceTools { get; set; }
    }
}
