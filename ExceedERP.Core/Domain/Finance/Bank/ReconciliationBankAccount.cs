using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Bank
{
    public class ReconciliationBankAccount : TrackUserOperation
    {
        public int ReconciliationBankAccountId { get; set; }
        [Required]
        [Display(Name = "Bank Account")]
        public string BankAccount { get; set; }
        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; }
        [Required]
        public string Reference { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal ExRate { get; set; }
        public decimal DebitNonFun { get; set; }
        public decimal CreditNonFun { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Reconciled Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReconciledDate { get; set; }
        public bool Reconciled { get; set; }
        public bool Check { get; set; }
       
    }
}
