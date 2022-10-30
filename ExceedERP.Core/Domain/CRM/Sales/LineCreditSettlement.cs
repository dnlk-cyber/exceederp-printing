using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class LineCreditSettlement : Operations
    {
        public int LineCreditSettlementId { get; set; }
        public int CreditSettlementId { get; set; }
        public int InvoiceId { get; set; }

        public string CurrentAmmount { get; set; }

        public decimal SettelmentAmount { get; set; }

        [Display(Name = "Balance Due")]
        public decimal RemainingAmount { get; set; }

        public decimal NetBalance { get; set; }

        [Display(Name = "Witholding type")]
        public int WithholdingType { get; set; }

        public decimal WithholdingAmount { get; set; }

        [ForeignKey("CreditSettlementId")]
        public virtual CreditSettlement CreditSettlement { get; set; }
    }
}
