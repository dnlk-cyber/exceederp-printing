using ExceedERP.Core.Domain.CRM;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Web.Areas.CRM.Models
{
    public class LineItemViewModel 
    {
    
        public int LineItemID { get; set; }

        public string References { get; set; }
        public string VoucherDefinitionId { get; set; }
        public string Voucher { get; set; }

        [DisplayName("Store")]
        [Required]
        public int StoreID { get; set; }

        public string Class { get; set; }
        [DisplayName("Reserved Balance (Quote + not shipped Sales)")]

        public decimal? StockBalanceReserved { get; set; }
        [NotMapped]
        [Display(Name ="Net Balance (Actual - Reserved)")]
        public decimal ? ActualRemainingBalance { get; set; }
        [DisplayName("Stock Balance")]
        public decimal? StockBalance { get; set; }
        [DisplayName("Price Tag")]
        public string PriceTag { get; set; }
        //[DisplayName("Price Tag Detail")]
        //public string PriceTagDetail { get; set; }
        [DisplayName("Product")]
        [Required]
        public int ThingID { get; set; }


        public string Variety { get; set; }
        [Required]
        [DisplayName("Unit Amount")]
        [ExclusiveRange(0, ErrorMessage ="Unit Amount (Price) should be greater than 0")]
        public decimal UnitAmount { get; set; }
        [Required]
        [ExclusiveRange(0, ErrorMessage = "Quantity should be greater than 0")]
        public decimal Quantity { get; set; }
        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }
        [DisplayName("Tax Type")]
        [Required]
        public string Tax { get; set; }
        [DisplayName("Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Required]
        [DisplayName(" Sub Total ")]
        public decimal CalculatedCost { get; set; }
     
        public decimal Vat { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
 
        [Display(Name = "Withholding Type")]
        public string WithholdingType { get; set; }
        [Display(Name = "Withholding Amount")]
        public decimal WithholdingAmount { get; set; }
        [Display(Name = "Withhold Tax(VAT)")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }
        public decimal Discount { get; set; }
        [Display(Name ="UoM")]
        public string UOM { get; set; }
        public List<BindingModel> AdditionalCostItem { get; set; }

        // unit convertion
        [Display(Name ="Other UoM")]
        public string OtherUoM { get; set; }
        [Display(Name ="Quantity")]
        public decimal? QuantityInOtherUoM { get; set; }
        [Display(Name ="Multiplication Factor")]
        public decimal? MultiplcationFactor { get; set; }
        public decimal? Result { get; set; }

        /// <summary>
        /// maintenance store requisition number
        /// </summary>
        public string StoreRequisitionNumber { get; set; }

        /// <summary>
        /// store issue (shipment number)
        /// </summary>
        public string ShipmentNumber { get; set; }
    }

}