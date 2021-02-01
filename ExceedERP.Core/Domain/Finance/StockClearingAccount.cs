using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance
{
    public class StockClearingAccount : TrackUserOperation
    {
        public int StockClearingAccountId { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Enable ?")]
        public bool Enable { get; set; }
    }
}
