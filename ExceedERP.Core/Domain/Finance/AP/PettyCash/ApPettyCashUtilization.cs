using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.PettyCash
{
    public class ApPettyCashUtilization : TrackUserOperation
    {
        public int ApPettyCashUtilizationId { get; set; }
        public int ApPettyCashId { get; set; }
        public virtual ApPettyCash PettyCash { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "Paid to")]
        public string PaidTo { get; set; }
        [Required]
        [Display(Name = "Purpose of Payment")]
        public string Purpose { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Reference")]
        public string Reference { get; set; }

       
    }
}
