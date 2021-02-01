using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Procurement.ForeignPurchase
{
    public class ForeignPurchaseFollowUpSystemStatus
    {
        public int ForeignPurchaseFollowUpSystemStatusId { get; set; }
        public int ForeignPurchaseId { get; set; }
        public virtual ForeignPurchase ForeignPurchase { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
    }
}