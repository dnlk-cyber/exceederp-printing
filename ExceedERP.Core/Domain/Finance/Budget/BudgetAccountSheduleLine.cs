using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
