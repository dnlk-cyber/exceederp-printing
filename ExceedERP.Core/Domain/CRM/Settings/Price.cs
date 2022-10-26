using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class Price : TrackUserOperation
    {
        public int PriceID { get; set; }

        public string Thing { get; set; }
    
        public string Variety { get; set; }

        [Required]
        [Display(Name ="Reference Type")]
        public string ReferenceType { get; set; }

        public string Reference { get; set; }
        public string Class { get; set; }
        public decimal Value { get; set; }

        public bool IsDefault { get; set; }

        public int Priority { get; set; }

        public string Remark { get; set; }


    }

}
