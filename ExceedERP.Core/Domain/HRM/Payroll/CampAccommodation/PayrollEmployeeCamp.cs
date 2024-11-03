using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public class PayrollEmployeeCamp : TrackUserOperation
    {
        public int PayrollEmployeeCampId { get; set; }   
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        public string Remark { get; set; }       
       
    }
}