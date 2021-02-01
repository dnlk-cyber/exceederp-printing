using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lot
{
    public class BidAwardedSupplier : TrackUserOperation
    {
        public int BidAwardedSupplierID { get; set; }
        public int LotID { get; set; }
        public virtual Lot Lot { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [Display(Name = "Award Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AwardDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Delivery Date")]
        public DateTime DeliveryDate { get; set; }
        public string Remark { get; set; }
        public bool IsTransferredToContract { get; set; }
        [Display(Name = "Award Given ?")]
        public bool AwardGiven { get; set; }
        [Display(Name = "Tender Reference")]
        public string TenderReference { get; set; }
        public virtual ICollection<BidAwardedSupplierItem> BidWinningItems { get; set; }
    }
    public class BidAwardedSupplierItem : TrackUserOperation
    {
        public int BidAwardedSupplierItemId { get; set; }
        public int BidAwardedSupplierID { get; set; }
        public virtual BidAwardedSupplier BidAwardedSupplier { get; set; }
        
        [DisplayName("Category Code")]
        public string CatCode { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        
        [DisplayName("Item Code")]
        public string ItmCode { get; set; }
        [DisplayName("Item")]
        public string ItemName { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        public bool IsApproved { get; set; }//item by item
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        public string Name { get; set; }//fixed asset
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quanity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UPrice { get; set; }
        [Display(Name = "Discount (Rebate)")]
        public decimal Discount { get; set; }
        [Display(Name = "Unit Price (After Discount)")]
        public decimal FinalUPrice { get; set; }
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Price")]
        public decimal TPrice { get; set; }
        public int LotItemID { get; set; }//link to lot item
    }
}
