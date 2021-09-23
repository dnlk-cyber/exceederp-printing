using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Budget
{
    public class BudgetLineSheduleAccounts : TrackUserSettingOperation
    {
        public int BudgetLineSheduleAccountsId { get; set; }
        public int BudgetAccountSheduleLineId { get; set; }
        [Required]
        public string FromAccountCode { get; set; }
          [Required]
        public string ToAccountCode { get; set; }
        public string Remark { get; set; }
        public virtual BudgetAccountSheduleLine BudgetAccountSheduleLine { get; set; }
    }
}
