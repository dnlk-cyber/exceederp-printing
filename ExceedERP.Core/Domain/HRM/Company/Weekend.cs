using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class Weekend : TrackUserOperation
    {
        [Key]
        public int WeekendId { get; set; }
        [Display(Name = "HR_Company_Weekend_IsHalfDay", ResourceType = typeof(Localization.Resources))]
        public bool IsHalfDay { get; set; }
        [Display(Name = "Is Full Day")]
        public bool IsFullDay { get; set; }
        [Display(Name = "HR_Company_Weekend_Name", ResourceType = typeof(Localization.Resources))]
        public WeekendName Name { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


    }

    public class WeekendException : TrackUserOperation
    {
        [Key]
        public int WeekendExceptionId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int? EmployeeId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int? OrgStructureId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int? BranchId { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_Company_Weekend_IsHalfDay", ResourceType = typeof(Localization.Resources))]
        public bool IsHalfDay { get; set; }
        [Display(Name = "Is Full Day")]
        public bool IsFullDay { get; set; }
        public int WeekendId { get; set; }
        public virtual Weekend  GetWeekend { get; set; }

    }
}