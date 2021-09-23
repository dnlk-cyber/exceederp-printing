using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class LicenseLevel : TrackUserSettingOperation
    {

        public int LicenseLevelId { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
