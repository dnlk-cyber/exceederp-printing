using ExceedERP.Core.Domain.Budget;
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
    public class BudgetParameterODS
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetParameterViewModel> GetBudgetCategory()
        {
            List<BudgetParameterViewModel> list = new List<BudgetParameterViewModel>();

            list = db.BudgetCategorys.Select(x => new BudgetParameterViewModel
            {
                Display = x.Name,
                Value = x.BudgetCategoryID.ToString()
            }).ToList();

            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetParameterViewModel> GetYear()
        {
            List<BudgetParameterViewModel> list = new List<BudgetParameterViewModel>();

            list = db.GLFiscalYears.OrderByDescending(x=>x.DateFrom).Select(x => new BudgetParameterViewModel
            {
                Display = x.Name,
                Value = x.GlFiscalYearId.ToString()
            }).ToList();

            return list;
        }

       
        
    }
}
