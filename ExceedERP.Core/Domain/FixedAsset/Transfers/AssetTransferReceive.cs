using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Core.Domain.FixedAsset.Transfers
{
   
    public class AssetTransferReceive : Operations
    {
        public int AssetTransferReceiveId { get; set; }
        public int AssetTransferId { get; set; }
        public virtual AssetTransfer Transfer { get; set; }

        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
        
        public virtual ICollection<AssetTransferReceiveItem> AssetTransferReceiveItems { get; set; }
        public virtual ICollection<AssetTransferReceiveValidation> AssetTransferReceiveValidations { get; set; }
    }
}
