using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.Payroll.BackPays
{
    public enum DetailType
    {
        NA,
        Addition,
        Deduction
    }

    public class BackPay : TrackUserOperation
    {
        public int BackPayId { get; set; }
        public int GLPeriodId { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        public string Remark { get; set; }
        public bool IsClosed { get; set; }
        public bool IsOnlineTransferred { get; set; }
        public virtual ICollection<BackPaymentLine> BackPaymentLines { get; set; }

    }
    public class BackPaymentLine : TrackUserOperation
    {
        public int BackPaymentLineId { get; set; }
        public int EmployeeId { get; set; }
        public virtual Person_Employee PersonEmployee { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal Pension { get; set; }
        public decimal Net { get; set; }
        public int BackPayId { get; set; }
        public virtual BackPay BackPay { get; set; }
        public virtual ICollection<BackPaymentDetail> BackPaymentDetails { get; set; }
    }


    public class BackPaymentDistribution : VoucherDistribution
    {
        public int BackPaymentDistributionId { get; set; }
        public int BackPayId { get; set; }
        public virtual BackPay BackPay { get; set; }
    }

    public class BackPaymentDetail : TrackUserSettingOperation
    {
        public int BackPaymentDetailId { get; set; }
        public int GLPeriodId { get; set; }
        public string Description { get; set; }
        public DetailType Type { get; set; }
        public decimal Amount { get; set; }
        public int BackPaymentLineId { get; set; }
        public virtual BackPaymentLine BackPaymentLine { get; set; }
    }
}
