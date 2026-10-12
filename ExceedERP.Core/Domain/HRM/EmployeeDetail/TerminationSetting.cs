using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class TerminationSetting: TrackUserOperation
    {
        [Key]
        public int TerminationSettingId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_ResponsibilityLevel", ResourceType = typeof(Localization.Resources))]
        [Index("IX_ResponsibilityLevel", IsUnique = true)]
        public ResponsibilityLevel? ResponsibilityLevel { get; set; }
        public int NoticePeriodDays { get; set; }    // 60, 45, etc.
        public bool IsActive { get; set; }
    }
}
