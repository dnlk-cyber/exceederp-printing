using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Budget.GAAP
{

    public class ProfitLossStatementCategory : TrackUserOperation
    {
        public int ProfitLossStatementCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public Enums.ProfitLostType ProfitLostType { get; set; }
        public string Remark { get; set; }
    }

}
