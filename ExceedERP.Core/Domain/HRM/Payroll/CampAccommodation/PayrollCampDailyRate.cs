using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.SalaryStructure;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollCampDailyRate : TrackUserOperation
    {
        public int PayrollCampDailyRateId { get; set; }
        public int RankId { get; set; }
        public virtual Rank Rank { get; set; }
        public int BranchId { get; set; }
        public decimal Rate { get; set; }
        public string Remark { get; set; }       
       
    }
}