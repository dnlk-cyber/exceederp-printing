using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
namespace ExceedERP.Core.Domain.Budget
{
        public  class BudgetCurrency : TrackUserOperation
    {

            public int BudgetCurrencyId { get; set;}
            [Required]
            public string Name { get; set;}
            public string Description { get; set;}


 
    }
}
