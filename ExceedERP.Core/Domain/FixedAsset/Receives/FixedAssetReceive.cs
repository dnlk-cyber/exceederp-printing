using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{

    public class FixedAssetReceive : Operations
    {
        public int FixedAssetReceiveId { get; set; }
        [Required]
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Display(Name = "Receive Type")]
        public int ReceiveTypeId { get; set; }
        public virtual ReceiveType ReceiveType { get; set; }
        [Display(Name = "Receive Type Ref.")]
        public string ReceiveRef { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }//for po and tender
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }//for manual
        [Display(Name = "Supplier Invoice No.")]
        public string SupplierInvoiceNo { get; set; }
        [Display(Name = "Receive Reference")] // manual reference
        public string Reference { get; set; }
        [Display(Name = "Lot")]
        public string TenderLot { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public bool IsManual { get; set; }

        public virtual ICollection<FixedAssetInspection> Inspections { get; set; }
        public virtual ICollection<FixedAssetReceiveItem> FixedAssetReceiveItem { get; set; }
        public virtual ICollection<FixedAssetReceiveValidation> FixedAssetReceiveValidation { get; set; }
        public virtual ICollection<FixedAssetReceiveDistribution> FixedAssetReceiveDistribution { get; set; }
    }
}

