using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement.Proforma
{
    public class ParticipatingSupplier : TrackUserOperation
    {
        public int ParticipatingSupplierID { get; set; }
        public int ProformaPurchaseID { get; set; }
        public virtual ProformaPurchase ProformaPurchase { get; set; }

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
        public string Name { get; set; }//fixed asset
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UPrice { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Price")]
        public decimal TPrice { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [Display(Name = "Representative Name")]
        public string Representative { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Telephone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{4})[-. ]?([0-9]{6})$", ErrorMessage = "Not a valid Phone number.Should be xxx-xxxx-xxxxxx")]
        public string Telephone { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Submission Date ")]
        public DateTime ProformaSubmitionDate { get; set; }
        [Display(Name = "Submission Time")]
        public string ProformaSubmitionTime { get; set; }
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Is Out of Stock")]
        public bool IsOutOfStock { get; set; }
        public string Remark { get; set; }
        public int ProformaItemID { get; set; }//proforma item
        public virtual ICollection<ParticipatingSupplierItem> ParticipatingSupplierItems { get; set; }

    }

    public class ParticipatingSupplierItem : TrackUserOperation
    {
        public int ParticipatingSupplierItemId { get; set; }
        public int ParticipatingSupplierID { get; set; }
        public virtual ParticipatingSupplier ParticipatingSupplier { get; set; }

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
        public string Name { get; set; }//fixed asset
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public PurchaseItemType ItemType { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Unit Price")]
        public decimal UPrice { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Price")]
        public decimal TPrice { get; set; }
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        [Display(Name = "Is Out of Stock")]
        public bool IsOutOfStock { get; set; }
        public string Remark { get; set; }
        public int ProformaItemID { get; set; }//proforma item

    }
}
