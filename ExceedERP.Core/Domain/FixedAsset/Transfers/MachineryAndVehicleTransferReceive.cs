using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Transfers
{

    public class MachineryAndVehicleTransferReceive : Operations
    {
        public int MachineryAndVehicleTransferReceiveId { get; set; }
        [Display(Name = "Transfer No.")]
        public int MachineryAndVehicleTransferId { get; set; }
        public virtual MachineryAndVehicleTransfer Transfer { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
        public virtual ICollection<DamagedPartReceive> DamagedPartReceives { get; set; }
        public virtual ICollection<MissedPartReceive> MissedPartReceives { get; set; }
        public virtual ICollection<AccessoryReceive> AccessoryReceives { get; set; }
        public virtual ICollection<MachineryAndVehicleTransferReceiveValidation> MachineryAndVehicleTransferReceiveValidations { get; set; }
    }
    public class DamagedPartReceive : TrackUserSettingOperation
    {
        public int DamagedPartReceiveId { get; set; }
        public int MachineryAndVehicleTransferReceiveId { get; set; }
        public virtual MachineryAndVehicleTransferReceive TransferReceive { get; set; }
        [DisplayName("Damaged Part")]
        public string DamagePart { get; set; }
    }
    public class MissedPartReceive : TrackUserSettingOperation
    {
        public int MissedPartReceiveId { get; set; }
        public int MachineryAndVehicleTransferReceiveId { get; set; }
        public virtual MachineryAndVehicleTransferReceive TransferReceive { get; set; }
        [DisplayName("Missed Part")]
        public string MissingPart { get; set; }
    }
    public class AccessoryReceive : TrackUserSettingOperation
    {
        public int AccessoryReceiveId { get; set; }
        public int MachineryAndVehicleTransferReceiveId { get; set; }
        public virtual MachineryAndVehicleTransferReceive TransferReceive { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("QTY")]
        public int Quantity { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
    }
    public class MachineryAndVehicleTransferReceiveValidation : VoucherValidation
    {
        public int MachineryAndVehicleTransferReceiveValidationId { get; set; }
        public int MachineryAndVehicleTransferReceiveId { get; set; }
        public virtual MachineryAndVehicleTransferReceive TransferReceive { get; set; }
    }
}
