using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.HRM.Payroll.BackPays;

namespace ExceedERP.Core.Domain.HRM.Payroll.MissingPayrollBackPays
{
  
    public class MissingPayrollBackPay : TrackUserOperation
    {
        public int MissingPayrollBackPayId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        [Display(Name ="Branch/project")]
        public int BranchId { get; set; }
        [Display(Name = "Business Unit")]
        public int BusinessUnitId { get; set; }
        public string Remark { get; set; }
        public bool IsClosed { get; set; }
        public bool IsOnlineTransferred { get; set; }
        public virtual ICollection<MissingPayrollBackPaymentLine> MissingPayrollBackPaymentLines { get; set; }

    }
    public class MissingPayrollBackPaymentLine : TrackUserOperation
    {
        public int MissingPayrollBackPaymentLineId { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee PersonEmployee { get; set; }
        public string Reason { get; set; }
        public int MissingPayrollBackPayId { get; set; }
        public virtual MissingPayrollBackPay MissingPayrollBackPay { get; set; }
        public virtual ICollection<MissingPayrollBackPaymentDetail> MissingPayrollBackPaymentDetails { get; set; }
        public virtual ICollection<MissingPayrollBackPaymentDetailPeriod> MissingPayrollBackPaymentDetailPeriods { get; set; }
    }
    public class MissingPayrollBackPaymentDetailPeriod : TrackUserSettingOperation
    {
        public int MissingPayrollBackPaymentDetailPeriodId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public string Description { get; set; }
        public int MissingPayrollBackPaymentLineId { get; set; }
        public virtual MissingPayrollBackPaymentLine MissingPayrollBackPaymentLine { get; set; }
    }

    public class MissingPayrollBackPaymentDetail : TrackUserSettingOperation
    {
        public int MissingPayrollBackPaymentDetailId { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        public string Description { get; set; }
        public DetailType Type { get; set; }
        public decimal Amount { get; set; }
        public int MissingPayrollBackPaymentLineId { get; set; }
        public virtual MissingPayrollBackPaymentLine MissingPayrollBackPaymentLine { get; set; }
    }

    public class MissingPayrollBackPaymentDistribution : VoucherDistribution
    {
        public int MissingPayrollBackPaymentDistributionId { get; set; }
        public int MissingPayrollBackPayId { get; set; }
        public virtual MissingPayrollBackPay MissingPayrollBackPay { get; set; }
    }
}
