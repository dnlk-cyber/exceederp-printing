using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeAssociation:TrackUserOperation 
    {
        [Key]
        public int AssociationId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_EmployeeAssociation_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_EmployeeAssociation_Name", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public string Name { get; set; }
        [Display(Name = "HR_EmployeeDetail_EmployeeAssociation_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}