using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Core.Domain.FixedAsset.Transfers
{
   
    public class AssetTransfer : Operations
    {
        public int AssetTransferId { get; set; }
        [Display(Name = "Request Document No.")]
        public string RequestNumber { get; set; }
        public TransferType Type { get; set; }

        [Display(Name = "Transfer Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransferDate { get; set; }
       
        [Display(Name = "Transfer Description")]
        public string TransferDesc { get; set; }
        [Display(Name = "Custodian Before")]
        public int? CustodianBeforeTransfer { get; set; }//empid
        [Display(Name = "Transfer To")]
        public int? TransferTo { get; set; }//empid
        [Display(Name = "From Store")]
        public string FromStoreCode { get; set; }
        [Display(Name = "To Store")]
        public string ToStoreCode { get; set; }
        [Display(Name = "From Branch")]
        public int? FromBranchId { get; set; }
        [Display(Name = "To Branch")]
        public int? ToBranchId { get; set; }
       
        [Display(Name = "Vehicle Plate No.")]
        public string PlateNo { get; set; }
       
        [Display(Name = "Deliverer Type")]
        public AssetTransferDelivererType DriverType { get; set; }
        [Display(Name = "Deliverer Name")]
        public string DeliveredBy { get; set; }//internal
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }//external
        [Display(Name = "Recipient Type")]
        public AssetTransferRecipientType RecipientType { get; set; }
        [Display(Name = "Recipient Employee")]
        public string RecipientEmployee { get; set; }//internal
        [Display(Name = "Recipient Supplier")]
        public string RecipientSupplier { get; set; }//external
        public virtual ICollection<AssetTransferItem> AssetTransferItems { get; set; }
        public virtual ICollection<AssetTransferValidation> AssetTransferValidations { get; set; }
    }
}
