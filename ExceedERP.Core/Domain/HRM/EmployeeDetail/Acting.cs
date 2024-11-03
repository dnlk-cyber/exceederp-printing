using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>    Acting . </summary>
    ///
    /// <remarks>   Kibrom Abera, 10/28/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class Acting : TrackUserOperation
    {
        [Key]
        public int ActingId { get; set; }
        [Required]
        [Display(Name = "Actor Employee")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }

        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_Attendance_Attendance_FromDateTime", ResourceType = typeof(Localization.Resources))]
        public DateTime? FromDateTime { get; set; }
        [Display(Name = "HR_Attendance_Attendance_FromDateTime", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string FromDateTimeAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        //[Required]
        [Display(Name = "HR_Attendance_Attendance_ToDateTime", ResourceType = typeof(Localization.Resources))]
        public DateTime? ToDateTime { get; set; }
        [Display(Name = "HR_Attendance_Attendance_ToDateTime", ResourceType = typeof(Localization.Resources))]
        public string ToDateTimeAm { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public decimal Salary { get; set; }
        public bool IsOpen { get; set; }
        public int ActingAllowanceId { get; set; }
        public IList<ActingAllowance> ActingAllowances { get; set; }


    }
    public class ActingAllowance : TrackUserOperation
    {

        public int ActingAllowanceId { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_AllowancePerEmployee_AllowanceDefnId", ResourceType = typeof(Localization.Resources))]
        public int AllowanceDefinationId { get; set; }
        public virtual AllowanceDefination AllowanceDefination{ get; set;}
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public int ActingId { get; set; }
        public virtual Acting  Acting { get; set; }

    }

}