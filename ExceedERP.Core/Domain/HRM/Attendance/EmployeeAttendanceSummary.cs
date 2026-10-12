using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Attendance
{
    public class EmployeeAttendanceSummary : TrackUserOperation
    {
        [Key]
        public int EmployeeAttendanceSummaryId { get; set; }

        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }

        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int? OrgStructureId { get; set; }

        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int? BranchId { get; set; }
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }

        [Display(Name = "Punch Time")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        [Display(Name = "Check-In Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? CheckInTime { get; set; }

        [Display(Name = "Check-Out Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime? CheckOutTime { get; set; }
        [Display(Name = "Leave(day)")]
        public decimal Leave { get; set; }
        [Display(Name = "Absent(day)")]
        public decimal Absent { get; set; }
        [Display(Name = "Late(day)")]
        public decimal LateComing { get; set; }
        [Display(Name = "Total (hr)")]
        public decimal? TotalHrs { get; set; }
        [Display(Name = "Total (Days)")]
        public decimal? TotalDays { get; set; }
        [Display(Name = "OT(hr)")]
        public decimal OvertimeHours { get; set; }
        [Display(Name = "OT(day)")]
        public decimal OvertimeDays { get; set; }
        [NotMapped]
        [Display(Name = "Day")]
        public string Day { get; set; }

    }
}
