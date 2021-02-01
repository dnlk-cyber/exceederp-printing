using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Inventory.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class ProcurementNeedOds : List<ProcurementNeedViewModel>
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProcurementNeedViewModel> GetProcurementNeed(string project,string workunit, string fiscalyear)
        {
            List<ProcurementNeedViewModel> list = new List<ProcurementNeedViewModel> { };
            if (!String.IsNullOrEmpty(fiscalyear))
            {
                int per = int.Parse(fiscalyear);

                List<AnnualNeedAssesmentItem> reportData =
                    _db.AnnualNeedAssesmentItems
                        .Where(x => x.AnnualNeedAssesment.GlFiscalYearId == per)
                        .Where(x => x.AnnualNeedAssesment.IsOnlineApproved && x.AnnualNeedAssesment.isVoid == false)
                        .ToList();


                if (!String.IsNullOrEmpty(project))
                {
                    int prj = int.Parse(project);
                    reportData = reportData.Where(x => x.AnnualNeedAssesment.BranchId == prj).ToList();
                }

                if (!String.IsNullOrEmpty(workunit))
                {
                    int orgid = int.Parse(workunit);
                    reportData = reportData.Where(x => x.AnnualNeedAssesment.OrgStructureId == orgid).ToList();
                }

                foreach (var item in reportData)
                {
                    var needViewModel = new ProcurementNeedViewModel
                    {
                        Id = item.AnnualNeedAssesmentID.ToString(),
                        ItemCategoryCode = item.CatCode + "-" + item.CategoryName,
                        ItemCode = item.ItmCode + "-" + item.ItemName,
                        Function = item.Function,
                        PerformanceRequirement = item.PerformanceRequirement,
                        PhysicalCharacteristics = item.PhysicalCharacteristics,
                        Quantity = item.Quantity,
                        MinimumQualityRequired = item.MinimumQualityRequired,
                        BudgetSource = item.BudgetSource.ToString(),
                        ProcurementCategory = item.ProcurementCategory.ToString(),
                        EstimatedBudget = item.EstimatedBudget,
                        EstimatedTimeTheItemIsNeeded = item.EstimatedTimeTheItemIsNeeded,
                        Unit = item.UoM,
                        July = item.Period1,
                        Aug = item.Period2,
                        Sep = item.Period3,
                        Oct = item.Period4,
                        Nov = item.Period5,
                        Dec = item.Period6,
                        Jan = item.Period7,
                        Feb = item.Period8,
                        Mar = item.Period9,
                        Apr = item.Period10,
                        May = item.Period11,
                        June = item.Period12
                    };

                    list.Add(needViewModel);
                }
            }



            return list;
        }
    }
}