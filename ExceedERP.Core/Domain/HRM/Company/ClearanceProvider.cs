using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class ClearanceProvider:TrackUserOperation 
    {
        [Key ]
        public int ClearanceProviderId { get; set; }
        [Display(Name = "HR_Company_ClearanceProvider_WorkUnit", ResourceType = typeof(Localization.Resources))]
        public string WorkUnit { get; set; }
        [Display(Name = "HR_Company_ClearanceProvider_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}