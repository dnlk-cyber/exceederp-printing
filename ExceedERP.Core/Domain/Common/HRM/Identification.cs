using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Identification:TrackUserOperation 
    {
        public int IdentificationId { get; set; }
          
        public string ReferenceType { get; set; }
        public string Reference { get; set; }
        [Display(Name = "Common_Identification_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "Common_Identification_IdNumber", ResourceType = typeof(Localization.Resources))]
        public string IdNumber { get; set; }
        [Display(Name = "Common_Identification_Type", ResourceType = typeof(Localization.Resources))]
        public IdentificationType  Type { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_Identification_IssueDate", ResourceType = typeof(Localization.Resources))]
        public DateTime? IssueDate { get; set; }
        [Display(Name = "Common_Identification_IssueDate", ResourceType = typeof(Localization.Resources))]
        public string IssueDateIdAm{ get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_Identification_ExpiryDate", ResourceType = typeof(Localization.Resources))]
        public DateTime? ExpiryDate { get; set; }
        [Display(Name = "Common_Identification_ExpiryDate", ResourceType = typeof(Localization.Resources))]
        public string ExpiryDateAm { get; set; }
        [Display(Name = "Common_Identification_Issuer", ResourceType = typeof(Localization.Resources))]
        public string Issuer { get; set; }
        [Display(Name = "Common_Identification_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}