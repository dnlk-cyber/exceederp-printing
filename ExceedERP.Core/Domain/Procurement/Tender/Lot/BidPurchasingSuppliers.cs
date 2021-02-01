using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Tender.Lot
{
    public class BidPurchasingSuppliers : TrackUserOperation
    {
        public int BidPurchasingSuppliersID { get; set; }
        public int LotID { get; set; }
        public virtual Lot Lot { get; set; }
        [Display(Name = "Supplier ID")]//supplier database
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier")]//manual supplier
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]//manual supplier
        public string SupplierName { get; set; }
        public string Representative { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        public string Telephone { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
       
        [Display(Name = "Receipt No.")]
        public string ReceiptNo { get; set; }

        [Display(Name = "Receipt Not Required?")]
        public bool ReceiptNotRequired { get; set; }
        public string Remark { get; set; }
    }
}
