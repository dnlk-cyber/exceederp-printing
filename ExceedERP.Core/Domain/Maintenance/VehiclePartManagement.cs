using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class VehiclePartManagement: TrackUserSettingOperation
    {

        public int VehiclePartManagementId { get; set; }
        public int VehiclePartId { get; set; }
        public int? VehiclePartDamageTypeId { get; set; }
        public int VehicleReceptionFormId { get; set; }
        public int Quantity { get; set; }
        public string Remark { get; set; }

    }
}
