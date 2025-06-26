using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
   public class ExternalPaymentRequestJob:TrackUserSettingOperation
    {
        public int ExternalPaymentRequestJobId { get; set; }
        public int ExternalMaintenancePaymenntrequestId { get; set; }
        public string ServiceCenterJobId { get; set; }
        public string Desctiption { get; set; }
        public Guid? FleetId { get; set; }
        public string EquipmentName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EnteranceDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExitDate { get; set; }
    }
}
