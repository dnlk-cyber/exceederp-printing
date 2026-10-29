using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class StaffDebtorBalance: TrackUserSettingOperation
    {
        [Key]
        public int StaffDebtorBalanceId { get; set; }

        public int EmployeeId { get; set; }

        public int TerminationId { get; set; }

        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Debit")]
        [DataType(DataType.Currency)]
        public decimal? Debit { get; set; }

        [Display(Name = "Credit")]
        [DataType(DataType.Currency)]
        public decimal? Credit { get; set; }

        [Display(Name = "Balance")]
        [DataType(DataType.Currency)]
        public decimal? Balance { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

     
    }
}
