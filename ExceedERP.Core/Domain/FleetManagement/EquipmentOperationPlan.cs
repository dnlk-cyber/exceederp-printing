using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement
{
    public class EquipmentOperationPlan : Operations
    {
        public int EquipmentOperationPlanId { get; set; }
        public string CategoryId { get; set; }
        public string EquipmentTypeId { get; set; }
        public string EquipmentNameId { get; set; }
        public string FleetId { get; set; }
        public string SerialNumber { get; set; }
        public decimal? OperationHR { get; set; }
        public decimal? IdleHR { get; set; }
        public decimal? DownHR { get; set; }
        public bool IsFuel { get; set; }
        public decimal? FuelConsumption { get; set; }
        public int GLFiscalYearId { get; set; }
        public int? GLPeriodId { get; set; }
        public EquipmentOperationHR EquipmentOperationHR { get; set; }
    }
    public enum EquipmentOperationHR
    {
        Yearly,
        Monthly,
    }
    public class EquipmentOperationPlanValidation : TrackUserSettingOperation
    {
        public int EquipmentOperationPlanValidationId { get; set; }
        public int EquipmentOperationPlanId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
   
}
