using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;
using ExceedERP.Reporting.Finance.ViewModel.GAAP;

namespace ExceedERP.Reporting.Finance.DataSource.GAAP
{
   
    [DataObject]
    public class CashFlowProjectionHeaderOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CashFlowProjectionViewModel> GetCashFlowProjectionHeaders(string fiscalYear)
        {
            List<CashFlowProjectionViewModel> cashFlowList = new List<CashFlowProjectionViewModel>();

            if (!string.IsNullOrEmpty(fiscalYear))
            {
                int fYearId=int.Parse(fiscalYear);
                var year = _db.GLFiscalYears.Find(fYearId);               
                if (year!=null)
                {
                    var periods=_db.GLPeriods.AsNoTracking().Where(x=>x.GlFiscalYearId==fYearId).Select(x=> new { x.Index,x.Name});
                    CashFlowProjectionViewModel per = new CashFlowProjectionViewModel();
                    per.Month1 = periods.Where(x => x.Index == 1).FirstOrDefault()?.Name;
                    per.Month2 = periods.Where(x => x.Index == 2).FirstOrDefault()?.Name;
                    per.Month3 = periods.Where(x => x.Index == 3).FirstOrDefault()?.Name;
                    per.Month4 = periods.Where(x => x.Index == 4).FirstOrDefault()?.Name;
                    per.Month5 = periods.Where(x => x.Index == 5).FirstOrDefault()?.Name;
                    per.Month6 = periods.Where(x => x.Index == 6).FirstOrDefault()?.Name;
                    per.Month7 = periods.Where(x => x.Index == 7).FirstOrDefault()?.Name;
                    per.Month8 = periods.Where(x => x.Index == 8).FirstOrDefault()?.Name;
                    per.Month9 = periods.Where(x => x.Index == 9).FirstOrDefault()?.Name;
                    per.Month10 = periods.Where(x => x.Index == 10).FirstOrDefault()?.Name;
                    per.Month11 = periods.Where(x => x.Index == 11).FirstOrDefault()?.Name;
                    per.Month12 = periods.Where(x => x.Index == 12).FirstOrDefault()?.Name;

                    cashFlowList.Add(per);
                }
            }
                
            return cashFlowList;
        }
    }
}

