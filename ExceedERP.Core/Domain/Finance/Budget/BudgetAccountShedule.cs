using ExceedERP.Core.Domain.Common;
using System.ComponentModel;

namespace ExceedERP.Core.Domain.Finance.Budget
{

    public class BudgetAccountShedule : TrackUserSettingOperation
    {
        public int BudgetAccountSheduleId { get; set; }
        [DisplayName("Category")]
        public int BudgetAccountSheduleCategoryId { get; set; }
 
         [DisplayName("Rule")]
        public int Index { get; set; }
       public string Remark { get; set; }
    }
}
