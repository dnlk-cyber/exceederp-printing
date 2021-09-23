using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Budget
{
    public class BudgetAccountSheduleSubCategory : TrackUserSettingOperation
    {

        public int BudgetAccountSheduleSubCategoryId { get; set; }
        public int BudgetAccountSheduleId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Remark { get; set; }
        public virtual BudgetAccountShedule BudgetAccountShedule { get; set; }
    }
}
