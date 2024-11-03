using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollEmployeeCampDay : TrackUserOperation
    {
        public int PayrollEmployeeCampDayId { get; set; }    
        
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        public int GLPeriodId { get; set; }
        public int? BranchId { get; set; }
        public decimal Days { get; set; }
        public string Remark { get; set; }       
       
    }
}