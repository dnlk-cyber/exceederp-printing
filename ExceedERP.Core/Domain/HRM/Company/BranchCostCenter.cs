  using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class BranchCostCenter
    {
        [Key]
        public int BranchCostCenterId { get; set; }
        [Display(Name = "HR_Company_BranchCostCenter_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "HR_Company_BranchCostCenter_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Display(Name = "HR_Company_BranchCostCenter_Values", ResourceType = typeof(Localization.Resources))]
        public string Values { get; set; }
        public virtual  GLChartOfAccountCostCenter GLChartOfAccountCostCenter{get;set;}
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }   
        [NotMapped ]
                public string Name { get; set; }
    }
}