using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARSetupTransactionType : TrackUserOperation
    {
        public int ARSetupTransactionTypeID { get; set; }
        [Required]
        [Display(Name = "Transaction Name")]
        public string TransactionName { get; set; }
        [Required]
        [Display(Name = "Transaction Class")]
        public string TransactionClass { get; set; }
        public string Description { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "Creation Sign")]
        public string CreationSign { get; set; }
        [Display(Name = "Open Receivable")]
        public bool OpenReceivable { get; set; }
        [Display(Name = "Post to GL")]
        public bool PosttoGL { get; set; }
        public bool IsActive { get; set; }
        [Required]
        [Display(Name = "Receivable Account")]
        public string ReceivableAccount { get; set; }
        [Required]
        [Display(Name = "Revenue Account")]
        public string RevenueAccount { get; set; }
        [Required]
        [Display(Name = "Tax Account")]
        public string TaxAccount { get; set; }
        [Display(Name = "Freight Account")]
        public string FreightAccount { get; set; }
        [Display(Name = "Clearing Account")]
        public string ClearingAccount { get; set; }
        [Display(Name = "Unearned Account")]
        public string UnearnedAccount { get; set; }
    }
}
