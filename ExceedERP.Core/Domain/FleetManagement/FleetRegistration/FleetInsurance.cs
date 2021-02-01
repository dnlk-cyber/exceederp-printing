
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{

  
    public partial class FleetInsurance : Operations
    {
        [ScaffoldColumn(false)]
        public Guid FleetInsuranceID { get; set; }
        [Required]
        [DisplayName("  Equipment ")]
        public Guid FleetsID { get; set; }

        public enum InsuranceTypeenum { TypeOne, TypeTwo, TypeThree, Comprehensive }

        [Required]
        [DisplayName("Insurance Company ")]
        public string InsuranceCompany { get; set; }
        [DisplayName("Premium Cost")]
        [DataType(DataType.Currency)]
        [Required]
        public decimal PremiumCost { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName(" Due Date ")]
        public Nullable<DateTime> DueDate { get; set; }
        [DisplayName("Effective Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ExpireDate { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [DisplayName("Phone No")]
        public string PhoneNo { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string City { get; set; }

        public string Attachment { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [ForeignKey("FleetsID")]
        public virtual Fleets Fleet { get; set; }
      [Display(Name = "Insurance_Insurance_Type", ResourceType = typeof(Localization.Resources))]

        public int InsuranceTypeId { get; set; }
     [Display(Name = "Insurance_Insurance_Catagory", ResourceType = typeof(Localization.Resources))]

        public int InsuranceCategoryId { get; set; }



        [NotMapped]
        public string PlateNo { get; set; }


        [NotMapped]
        public string SerialNo { get; set; }


        [NotMapped]
        public string ChasisNo { get; set; }

    }
}
