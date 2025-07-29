using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class Canibalization : DepartmentNewOperationWithSecondValidation
    {
        public int CanibalizationId { get; set; }
        public string ServiceCenterJobId { get; set; }
        public string ProvidingMachinModel { get; set; }
        public string ProvidingMachinePlateNumber { get; set; }
        public string ProvidingMachineEngineModel { get; set; }
        public string ProvidingMachineEngineNumber { get; set; }
        public string RequestingMachinModel { get; set; }
        public string RequestingMachinePlateNumber { get; set; }
        public string RequestingMachineEngineModel { get; set; }
        public string RequestingMachineEngineNumber { get; set; }
        [NotMapped]
        public bool IsECWC { get; set; }

    }

    public class CanibalizedParts:TrackUserSettingOperation
    {
        public int CanibalizedPartsId { get; set; }
        public int CanibalizationId { get; set; }
        public string Description { get; set; }
        public int VehiclePartId { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
    }

}