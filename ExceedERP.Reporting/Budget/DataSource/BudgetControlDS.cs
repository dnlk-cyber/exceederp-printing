using ExceedERP.Core.Reporting.ViewModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Budget.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Budget.DataSource
{
     [DataObject]
        public class BudgetControlDS: List<BudgetControlViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();


      //  [DataObjectMethod(DataObjectMethodType.Select)]
     //   public List<BudgetControlViewModel> GetUsageList(string BudgetCategory, string Year, string WorkUnit)
      //  {

       //     return 
      //  }
     
     }
}
