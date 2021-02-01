using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
