using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class LineItem : Operations
    {
        public int LineItemID { get; set; }

        public string References { get; set; }
        public string VoucherDefinitionID { get; set; }
        public string Voucher { get; set; }

        [DisplayName("Store")]
        //[Required]
        public int StoreID { get; set; }

        public string Class { get; set; }
        [DisplayName("Reserved Stock Balance")]

        public decimal? StockBalanceReserved { get; set; }

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
        public decimal UnitAmount { get; set; }
        [Required]
        [ExclusiveRange(0, ErrorMessage = "Quantity Should be greater than 0")]
        public decimal Quantity { get; set; }
        [DisplayName("Total Amount")]
        public decimal TotalAmount { get; set; }
        [DisplayName("Tax Type")]
        //[Required]
        public string Tax { get; set; }
        [DisplayName("Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Required]
        [DisplayName(" Calculated Cost ")]
        public decimal CalculatedCost { get; set; }

        [Display(Name = "Withholding Type")]
        public string WithholdingType { get; set; }
        [Display(Name = "Withholding Amount")]
        public decimal WithholdingAmount { get; set; }
        [Display(Name = "Withhold Tax(VAT)")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }

        public decimal Vat { get; set; }
        public decimal Discount { get; set; }

        public string Status { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        public decimal PartialQuantity { get; set; }

        /// <summary>
        /// maintenance store requisition number
        /// </summary>
        public string StoreRequisitionNumber { get; set; }

        /// <summary>
        /// store issue (shipment number)
        /// </summary>
        public string ShipmentNumber { get; set; }

        [ForeignKey("ThingID")]
        public virtual Product Product { get; set; }
        [ForeignKey("StoreID")]
        public virtual SalesStore SalesStore { get; set; }
        public virtual ICollection<AdditionalCostLog> AdditionalCostLog { get; set; }

    }


}
