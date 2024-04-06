using ExceedERP.Core.Domain.Common.HRM;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class CommitteeMember
    {
        public int CommitteeMemberId { get; set; }
        [Display(Name = "EmployeeDetail_Committee_EmployeeId", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "EmployeeDetail_Committee_Role", ResourceType = typeof(Localization.Resources))]
        public string Role { get; set; }
        public int CommitteeId { get; set; }
        public Committee Committee { get; set; }       
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


    }
}
