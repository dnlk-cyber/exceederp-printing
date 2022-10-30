using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Finance.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class CreditSettlement : Operations
    {
       public int CreditSettlementId { get; set; }
        public int Customer { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public int RecieveMethod { get; set; }
        [Display(Name="Branch")]
        public int BranchId { get; set; }
        [NotMapped]
        public decimal GrandTotal { get; set; }
        [NotMapped]
        public decimal SettlementAmount { get; set; }
        [NotMapped]
        public decimal Remaining { get; set; }
        [ForeignKey("Customer")]
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
        [ForeignKey("RecieveMethod")]
        public virtual PaymentMethod PaymentMethod { get; set; }

        [Display(Name ="Receipt No.")]
        public string ReceiptNumber { get; set; }
        [Display(Name = "Period")]
        public int CRMPeriodId { get; set; }

        public virtual ICollection<LineCreditSettlement> LineCreditSettlement { get; set; }
    }

    
}
