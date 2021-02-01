using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.Ratio.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Reporting.Finance.Ratio.DataSource
{

    [DataObject]
    public class RatioAnalysisDataSource
    {
        public ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetQuarterType()
        {

            var budgetAccountShedules = EnumExtension.EnumToSelectList(typeof(BudgetEnums.BudgetQuarter));

            return budgetAccountShedules;
        }


     
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RatioAnalysisViewModel> GetRatioAnalysis(string fiscalYear, string periodfrom, string periodto)
        {
            List<RatioAnalysisViewModel> RatioList = new List<RatioAnalysisViewModel>();
            BudgetRatioHelper _BudgetRatioHelper = new BudgetRatioHelper();
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            int FiscalYear = _FiscalYearHelper.GetCurrentFYearId();
            int periodFrom =0;
            int periodTo = 0;
         

            if (!string.IsNullOrWhiteSpace(fiscalYear))
            {
                FiscalYear = int.Parse(fiscalYear);

            }
            if (!string.IsNullOrWhiteSpace(periodfrom))
            {
                periodFrom = int.Parse(periodfrom);

            }

            if (!string.IsNullOrWhiteSpace(periodto))
            {
                periodTo = int.Parse(periodto);
            }

            if (!string.IsNullOrWhiteSpace(fiscalYear))
            {
                RatioList.AddRange(_BudgetRatioHelper.ActivityRatios(FiscalYear, periodFrom, periodTo).Select(s => new RatioAnalysisViewModel
                {
                    Actual = s.Actual,
                    Budget = s.Budget,
                    RatioName = s.Name,
                    Type = s.Type,
                  
                }));
                RatioList.AddRange(_BudgetRatioHelper.CapitalStructureRatios(FiscalYear, periodFrom, periodTo).Select(s => new RatioAnalysisViewModel
                {
                    Actual = s.Actual,
                    Budget = s.Budget,
                    RatioName = s.Name,
                    Type = s.Type,
                  
                }));
                RatioList.AddRange(_BudgetRatioHelper.LiquidityRatios(FiscalYear, periodFrom, periodTo).Select(s => new RatioAnalysisViewModel
                {
                    Actual = s.Actual,
                    Budget = s.Budget,
                    RatioName = s.Name,
                    Type = s.Type
                    
                }));
                RatioList.AddRange(_BudgetRatioHelper.ProfitabilityRatios(FiscalYear, periodFrom, periodTo).Select(s => new RatioAnalysisViewModel
                {
                    Actual = s.Actual,
                    Budget = s.Budget,
                    RatioName = s.Name,
                    Type = s.Type,
                    
                }));


            }
            if(!RatioList.Any())
            {
                RatioList.Add(new RatioAnalysisViewModel
                {
                    Actual =decimal.Zero,
                    Budget = decimal.Zero,
                    RatioName = string.Empty,
                    Type = string.Empty,
                   
                });

            }
            return RatioList;
        }








    }
}