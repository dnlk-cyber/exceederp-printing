using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class Tax : TrackUserOperation
    {
        public int TaxID { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Description { get; set; }

        [DisplayName("Rate")]
        [Required]
        public decimal Amount { get; set; }


        public string Remark { get; set; }
    }


}
