using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement.Proforma
{
    public class WinningSupplier : TrackUserOperation
    {
        public int WinningSupplierID { get; set; }
        public int ProformaPurchaseID { get; set; }
        public virtual ProformaPurchase ProformaPurchase { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Award Date")]
        public DateTime AwardDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Delivery Date")]
        public DateTime? DeliveryDate { get; set; }
        [Display(Name = "GRN No.")]
        public string GRNNo { get; set; }
        [Display(Name = "Award Given ?")]
        public bool AwardGiven { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<WinningSupplierItem> WinningItems { get; set; }
    }

    public class WinningSupplierItem : TrackUserOperation
    {
        public int WinningSupplierItemId { get; set; }
        public int WinningSupplierID { get; set; }
        public virtual WinningSupplier WinningSupplier { get; set; }
       
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
        [Display(Name = "Purchased Quantity")]
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
        public int ProformaItemID { get; set; }//proforma item
    }
}
