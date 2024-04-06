using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class EmployeeProbationSetting : TrackUserOperation
    {
        [Key]
        public int EmployeeProbationSettingId { get; set; }
        
        [Display(Name = "Include Weekend")]
        public bool IncludeWeekend { get; set; }
        [Display(Name = "Include Holiday")]
        public bool IncludeHoliday { get; set; }


    }
}