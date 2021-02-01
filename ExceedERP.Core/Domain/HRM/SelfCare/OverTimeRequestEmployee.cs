using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class OverTimeRequestEmployee
    {
        [Key]
        public int OverTimeRequestEmployeeId { get; set; }
        public int OverTimeRequestId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        public virtual OverTimeRequest OverTimeRequest { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequestEmployee_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee PersonEmployee { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}