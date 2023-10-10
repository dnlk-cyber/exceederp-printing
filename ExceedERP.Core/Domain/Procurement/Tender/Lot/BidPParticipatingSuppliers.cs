using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.PR;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lot
{

    public enum PreQualification
    {
        Pass, Fail
    }
    public class BidPParticipatingSuppliers : TrackUserOperation
    {
        public int BidPParticipatingSuppliersID { get; set; }
        public int LotID { get; set; }
        public virtual Lot Lot { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        [Display(Name = "Representative")]
        public string Representative { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Telephone")]
        //[DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{4})[-. ]?([0-9]{6})$", ErrorMessage = "Not a valid Phone number")]
        public string Telephone { get; set; }
        [Display(Name = "Submission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BidSubmissionDate { get; set; }
        [Display(Name = "Submission Time")]
        public string BidSubmissionTime { get; set; }
        [Display(Name = "Number of Document")]
        public int NumberOfDocument { get; set; }
        [Display(Name = "Pre Qualification")]
        public PreQualification IsQualified { get; set; }
        public string Remark { get; set; }
        public virtual ICollection<BidParticipatingSupplierItem> BidParticipatingSupplierItems { get; set; }
    }

    public class BidParticipatingSupplierItem : TrackUserSettingOperation
    {
        public int BidParticipatingSupplierItemId { get; set; }
        public int BidPParticipatingSuppliersID { get; set; }
        public virtual BidPParticipatingSuppliers BidPParticipatingSupplier { get; set; }

        [Required]
        [DisplayName("Category Code")]
        public string CatCode { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        [Required]
        [DisplayName("Item Code")]
        public string ItmCode { get; set; }
        [DisplayName("Item")]
        public string ItemName { get; set; }
        [Display(Name = "Part No.")]
        public string PartNo { get; set; }
        [Display(Name = "Specification")]
        public string ItemSpecification { get; set; }
        public string Name { get; set; }//fixed asset
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Unit")]
        public string UoM { get; set; }
        public PurchaseItemType ItemType { get; set; }       
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Display(Name = "Rebate Amount")]
        public decimal DiscountAmount { get; set; }
        
        [DisplayName("Tax Type")]
        public int GLTaxRateID { get; set; }
        public float Technical { get; set; }
        public int LotItemID { get; set; }//link to lot item
        [Display(Name = "Is Out of Stock")]
        public bool IsOutOfStock { get; set; }
        public string Remark { get; set; }      

    }
}
