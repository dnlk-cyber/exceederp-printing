using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{

    public class OrganizationCustomer : TrackUserOperation
    {
        [Key]
        public int OrganizationCustomerID { get; set; }
        [Required]
        public string TradeName { get; set; }
        [Required]
        public string BrandName { get; set; }
        [DisplayName(" TIN Number ")]
        public string TinNo { get; set; }
        [DisplayName(" Location Type ")]
        public CustomerType CustomerType { get; set; }
        [Required]
        public string BusinessType { get; set; }
        [Required]
        public string Category { get; set; }
        public string EsclationLevel { get; set; }
       
        public bool IsActive { get; set; }
        public bool IsCredit { get; set; }

        public bool IsTemporary { get; set; }
        [NotMapped]
        public bool HasActiveTransaction { get; set; }

    }


}
