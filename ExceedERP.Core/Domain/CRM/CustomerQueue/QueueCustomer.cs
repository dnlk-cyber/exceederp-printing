using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class QueueCustomer : TrackUserOperation
    {
        [Key]
        public int QueueCustomerId { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(10,MinimumLength =10, ErrorMessage ="TIN Must be 10 digit" )]
        public string TinNumber { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone Must be 10 digit")]
        public string Phone { get; set; }

        [Display(Name = "Investment/Project Type")]
        public int InvestmentTypeId { get; set; }

        public virtual InvestmentType InvestmentType { get; set; }
        [Display(Name = "Investment/Project amount")]
        public decimal InvestmentAmount { get; set; }

        public string Grade { get; set; }
        [NotMapped]
        public bool HasTransactions { get; set; }
    }
}
