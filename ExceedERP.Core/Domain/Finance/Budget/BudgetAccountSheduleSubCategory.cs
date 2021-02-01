using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
