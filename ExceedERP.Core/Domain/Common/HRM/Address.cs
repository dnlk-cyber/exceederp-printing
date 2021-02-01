using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Address:TrackUserOperation 
    {
        public int AddressId { get; set; }
        public string ReferenceType { get; set; }
        public string Reference { get; set; }
        [Display(Name = "Common_Address_Category", ResourceType = typeof(Localization.Resources))]
        public AddressType Category { get; set; }
        [Display(Name = "Common_Address_Value", ResourceType = typeof(Localization.Resources))]
        public string Value { get; set; }
        [Display(Name = "Common_Identification_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}