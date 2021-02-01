using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class RegionNation : TrackUserOperation
    {
        [Key]
        public int RegionNationId { get; set; }
        [Display(Name = "HR_Company_RegionNation_Region", ResourceType = typeof(Localization.Resources))]
        public Region Region { get; set; }
        [Display(Name = "HR_Company_RegionNation_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}