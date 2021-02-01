using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Budget
{
    public class BudgetType : TrackUserSettingOperation
    {


        public int BudgetTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
   
    }
}
