using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Payroll.Setting
{
    public enum BenefitType
    {
        Milk,
        Soft
    }
    public class PayrollEmployeeMilkAndSoft : TrackUserOperation
    {
        public int PayrollEmployeeMilkAndSoftId { get; set; }   
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        public BenefitType BenefitType { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }       
       
    }
}