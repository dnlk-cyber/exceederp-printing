using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class InsuranceViewModel : Operations
    {
        public string PlateNumber { get; set; }

        [ScaffoldColumn(false)]
        public Guid FleetInsuranceID { get; set; }

        public Guid FleetsID { get; set; }
        public enum InsuranceTypeenum { TypeOne, TypeTwo, TypeThree, Comprehensive }
        public decimal PremiumCost { get; set; }
        public string InsuranceType { get; set; }
        public string InsuranceCompany { get; set; }
 
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DueDate { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ExpireDate { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public string PhoneNo { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string City { get; set; }

        public string Attachment { get; set; }
        public string Description { get; set; }



        [NotMapped]
        public string PlateNo { get; set; }


        [NotMapped]
        public string SerialNo { get; set; }


        [NotMapped]
        public string ChasisNo { get; set; }
         


    }
}