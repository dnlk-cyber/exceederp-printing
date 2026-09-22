using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement
{
    public class FleetAvailabilityTreshold:Operations
    {
        public int FleetAvailabilityTresholdId { get; set; }
        public string CategoryId { get; set; }
        public string TypeId { get; set; }
        public string NameId { get; set; }
        public string FleetId { get; set; }
        public int GLFiscalYearId { get; set; }
        public decimal AvailabilityTreshold { get; set; }
    }

    public class FleetAvailabilityTresholdValidation:TrackUserSettingOperation
    {
        public int FleetAvailabilityTresholdValidationId { get; set; }
        public int FleetAvailabilityTresholdId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}




