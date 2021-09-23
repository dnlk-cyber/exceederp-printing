using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollCostAllocationSetting : TrackUserOperation
    {
        public int PayrollCostAllocationSettingId { get; set; }
        public string CostCenter { get; set; }
        public string ProductAccount { get; set; }
        public decimal Percentage { get; set; }
        public string Remark { get; set; }

    }
}