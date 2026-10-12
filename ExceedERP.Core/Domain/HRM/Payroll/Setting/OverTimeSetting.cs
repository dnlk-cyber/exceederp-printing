using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Payroll
{
    public class OverTimeSetting : TrackUserSettingOperation
    {
        [Key]
        public int OverTimeSettingId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        // If Branch is null, then this is a global setting. Otherwise, it's for a specific Branch.
        public int? BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "Not allowed for head")]
        public bool NotAllowedForHead { get; set; }
       
        public string Remark { get; set; }

    }
}
