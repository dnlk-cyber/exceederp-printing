using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Budget
{
    public enum SheduleCategoryType
    {
        Other,
        PAYABLES,
        RECEIVABLES
    }

    public class BudgetAccountSheduleCategory : TrackUserSettingOperation
    {
        public int BudgetAccountSheduleCategoryId { get; set; }
        public SheduleCategoryType SheduleCategoryType { get; set; }
        [Required]
        public string Name { get; set; }
        public string Remark { get; set; }
    }
}
