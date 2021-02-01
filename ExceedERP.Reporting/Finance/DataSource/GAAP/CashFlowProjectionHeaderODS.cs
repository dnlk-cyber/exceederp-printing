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
            var year = _db.GLFiscalYears.Where(x=>x.GlFiscalYearId.ToString() == fiscalYear).ToList();



            CashFlowProjectionViewModel per = new CashFlowProjectionViewModel();
            if (year.Any())
            {
                try
                {
                    per.Month1 = year[0].Name;
                    per.Month2 = year[1].Name;
                    per.Month3 = year[2].Name;
                    per.Month4 = year[3].Name;
                    per.Month5 = year[4].Name;
                    per.Month6 = year[5].Name;
                    per.Month7 = year[6].Name;
                    per.Month8 = year[7].Name;
                    per.Month9 = year[8].Name;
                    per.Month10 = year[9].Name;
                    per.Month11 = year[10].Name;
                    per.Month12 = year[11].Name;

                    cashFlowList.Add(per);
                }
                catch(Exception e)
                {
                    
                }
            }
                
            return cashFlowList;
        }
    }
}

