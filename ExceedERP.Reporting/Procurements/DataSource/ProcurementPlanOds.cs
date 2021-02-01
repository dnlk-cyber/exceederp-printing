using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Inventory.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Core.Domain.Procurement.Plan;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class ProcurementPlanOds : List<ProcurementPlanViewModel>
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProcurementPlanViewModel> GetProcurementNeed(string project, string period)
        {
            List<ProcurementPlanViewModel> procurementNeed = new List<ProcurementPlanViewModel> { };
            try
            {

                List<ProcurementPlanItem> reportData = _db.ProcurementPlanItems.Where(x => x.ProcurementPlan.IsOnlineApproved).ToList();
                if (!String.IsNullOrEmpty(project))
                {
                    int prj = int.Parse(project);
                    reportData = reportData.Where(x => x.ProcurementPlan.BranchId == prj).ToList();
                }
                if (!String.IsNullOrEmpty(period))
                {
                    int per = int.Parse(period);
                    reportData = reportData.Where(x => x.ProcurementPlan.GlFiscalYearId == per).ToList();
                }
               

                foreach (var item in reportData)
                {
                    var planViewModel = new ProcurementPlanViewModel
                    {
                        Id = item.ProcurementPlanId.ToString(),
                        ItemCategoryCode = item.CatCode,
                        ItemCode = item.ItmCode,
                        Quantity = item.Quantity,
                        BudgetSource = item.BudgetSource.ToString(),
                        ProcurementCategory = item.ProcurementCategory.ToString(),
                        ProcurementMethod = item.ProcurementMethod.ToString(),
                        TenderFloatingTime = item.TenderFloatingTime,
                        PurchaseDeliveryTime = item.PurchaseDeliveryTime
                    };

                    procurementNeed.Add(planViewModel);
                }

                return procurementNeed;
            }
            catch (Exception)
            {
                return procurementNeed;
            }
        }
    }
}