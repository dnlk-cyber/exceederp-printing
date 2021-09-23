using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class OperatorLicense : TrackUserSettingOperation
    {

        public int OperatorLicenseId { get; set; }
        public Guid OperatorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
