using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public class InspectionApproval : TrackUserSettingOperation
    {
       public int InspectionApprovalId { get; set; }
       public string InspectionId { get; set; }
       public string Remark { get; set; }
       public string Status { get; set; }
    }
}
