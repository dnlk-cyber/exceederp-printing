using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class LicenseType : TrackUserSettingOperation
    {

        public int LicenseTypeId { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
