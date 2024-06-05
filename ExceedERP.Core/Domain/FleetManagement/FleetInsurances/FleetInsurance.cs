
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{

  
    public partial class FleetInsurance : Operations
    {
        [Key]
        public int FleetInsuranceId { get; set; }
       
        public Guid FleetsID { get; set; }
        public enum InsuranceTypeenum { TypeOne, TypeTwo, TypeThree, Comprehensive }
        [Required]
        [DisplayName("Company ")]
        public int InsuranceCompanyId { get; set; }
        [DisplayName("Premium Cost")]
        [Required]
        public decimal PremiumCost { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName(" Due Date ")]
        public DateTime? DueDate { get; set; }
        [DisplayName("Effective Date")]   
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpireDate { get; set; } 
        [NotMapped]
        public bool HasExpired { get; set; }
        public string Attachment { get; set; }
        public string Description { get; set; }
       
        [Display(Name = "Policy No")]
        public string PolicyNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HazardOfExpiryDate { get; set; }
       
        public string ThirdParty { get; set; }
        public FleetInsuranceTypeEnum InsuranceType { get; set; }
        [Display(Name ="Third Party")]
        public decimal ThirdPartyInsurance { get; set; }    
        [Display(Name ="Comprehensive")]
        public decimal ComprehensiveInsurance { get; set; }

    }
    public enum FleetInsuranceTypeEnum
    {
        [Display(Name ="Third party and comprehensive")]
        Third_Party_and_Comprehensive,
        [Display(Name ="political Violence")]
        Political_Violence
    }
}
 