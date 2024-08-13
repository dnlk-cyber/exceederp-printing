using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Insurance
{
    public class InsuranceType : TrackUserOperation
    {
        [Key]
        public int InsuranceTypeId { get; set; }
           
        //[Required]
        [Display(Name = "HR_HealthAndInsurance_InsuranceType_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "Insurance Type Group")]
        public InsuranceGroupType InsuranceTypeGroup { get; set; }
        [Display(Name = "Insurance Type")]
        public int LookupId { get; set; }
        public Lookup Lookup { get; set; }

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}