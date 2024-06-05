using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class MaintenanceType : TrackUserSettingOperation
    {
        public Guid MaintenanceTypeId { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }
    }
}
