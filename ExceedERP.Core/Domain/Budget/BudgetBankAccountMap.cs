using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetBankAccountMap : TrackUserOperation
    {

        public int BudgetBankAccountMapId { get; set; }
        [DisplayName("Branch")]
        public int BranchId { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsHeadOffice { get; set; }
        public string AccountCode { get; set; }
        public string BankNumber { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
    }
}
