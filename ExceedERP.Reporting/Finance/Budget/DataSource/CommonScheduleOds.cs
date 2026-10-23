using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.Budget.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExceedERP.Reporting.Finance.Budget.DataSource
{

    [DataObject]
    public class CommonScheduleOds : IDisposable
    {
        ExceedDbContext Db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CommonScheduleVm> GetAccountSheduleList()
        {

            var budgetAccountShedules = Db.BudgetAccountSheduleCategories.Select(s => new CommonScheduleVm
            {

                AccountDesc = s.Name.ToString(),
                AccountCode = s.BudgetAccountSheduleCategoryId.ToString()

            }).ToList();

            return budgetAccountShedules;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CommonScheduleVm> GetIndexList()
        {

            var budgetAccountShedules = Db.BudgetAccountShedules.DistinctByField(x => x.Index).Select(s => new CommonScheduleVm
            {

                AccountDesc = s.Index.ToString(),
                AccountCode = s.Index.ToString()

            }).ToList();

            return budgetAccountShedules;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetQuarterList()
        {
            List<string> list = new List<string>();
            list.Add("1");
            list.Add("2");
            list.Add("3");
            list.Add("4");
            return list;
        }
        public void Dispose()
        {
            Db?.Dispose();
        }
    }
}
