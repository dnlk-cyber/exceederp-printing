using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollCampAllowedBranch : TrackUserOperation
    {
        public int PayrollCampAllowedBranchId { get; set; }
		public int BranchId { get; set; }
		public string Remark { get; set; }       
       
    }
}