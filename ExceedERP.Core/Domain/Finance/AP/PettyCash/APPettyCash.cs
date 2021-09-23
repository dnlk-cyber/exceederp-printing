using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Finance.AP.Travel;

namespace ExceedERP.Core.Domain.Finance.AP.PettyCash
{
    public class ApPettyCash : NewOperations
    {
        public int ApPettyCashId { get; set; }
        [Required]
        [Display(Name = "Cashier")]
        public int ApCashierSetupId { get; set; }
        public virtual ApCashierSetup Cashier { get; set; }
        [Display(Name = "Purpose of Payment")]
        public String PurposeOfPayment { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Current Balance")]
        [NotMapped]
        public decimal CurrentBalance { get; set; }
        [Display(Name = "Reference")]
        public string Reference { get; set; }
        public virtual ICollection<ApPettyCashPayment> PettyCashPayments { get; set; }
        public virtual ICollection<ApPettyCashUtilization> PettyCashUtilizations { get; set; }
        public virtual ICollection<ApPettyCashValidation> PettyCashValidations { get; set; }
        public virtual ICollection<ApPettyCashDistribution> PettyCashDistributions { get; set; }
    }
}
