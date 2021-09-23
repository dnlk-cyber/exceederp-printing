using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Budget
{
    public class BudgetAccountSheduleLine : TrackUserSettingOperation
    {

        public int BudgetAccountSheduleLineId { get; set; }
        public int BudgetAccountSheduleSubCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Remark { get; set; }
        public virtual BudgetAccountSheduleSubCategory BudgetAccountSheduleSubCategory { get; set; }
    }
}
