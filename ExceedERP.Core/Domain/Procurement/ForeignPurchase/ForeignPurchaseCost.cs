using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.ForeignPurchase
{
    public class ForeignPurchaseCost : Operations
    {
        public int ForeignPurchaseCostId { get; set; }
        public int ForeignPurchaseId { get; set; }
        public virtual ForeignPurchase ForeignPurchase { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Payee")]
        public string Payee { get; set; }
        [Display(Name = "Cost Name")]
        public string CostType { get; set; }
        [Display(Name = "Functional Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Currency")]
        public string Currency { get; set; }
        [Display(Name = "Rate")]
        public decimal ExchangeRate { get; set; }
        [Display(Name = "Amount in Forex")]
        public decimal ForexAmount { get; set; }
        [Display(Name = "Is Paid?")]
        public bool Paid { get; set; }
        public virtual ICollection<ForeignPurchaseCostValidation> ForeignPurchaseCostValidations { get; set; }
    }
}