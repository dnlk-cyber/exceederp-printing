
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Plan;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Helpers.Inventory;
namespace ExceedERP.Reporting.FixedAsset.DataSource
{

    [DataObject]
    public class FixedAssetSupplyPlanActualOds
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplyPlanActualVm> GetSupplyPlanActual(string fiscalyear, string period, string businessUnitId, string branchId)
        {
            List<SupplyPlanActualVm> list = new List<SupplyPlanActualVm>();

            var planItems = _db.ProcurementPlanItems.Include(x => x.ProcurementPlan).AsQueryable();
            var businessUnits = _db.BusinessUnits.ToList();

            if (!string.IsNullOrEmpty(fiscalyear) && !string.IsNullOrEmpty(period))
            {
                int fyearid = int.Parse(fiscalyear);
                int periodid = int.Parse(period);
                var periodObj = _db.GLPeriods.Find(periodid);
                var fyearObj = _db.GLFiscalYears.Find(fyearid);

                if (!string.IsNullOrEmpty(businessUnitId))
                {
                    int bId = int.Parse(businessUnitId);
                    businessUnits = businessUnits.Where(x => x.BusinessUnitId == bId).ToList();
                    List<int> bIds = businessUnits.Select(x => x.BusinessUnitId).ToList();
                    planItems = planItems.Where(x => bIds.Contains(x.ProcurementPlan.BusinessUnitId));
                }

                planItems = planItems.Where(x => x.ProcurementPlan.GlFiscalYearId == fyearid);
                var planList = planItems.ToList();
                List<string> inventoryCats = _db.ItemCategorys.OrderBy(x => x.Name).Select(x => x.ItemCategoryCode).ToList();
                List<string> fixedCats = _db.FixedAssetCategories.OrderBy(x => x.CategoryName).Select(x => x.CategoryCode).ToList();
                List<string> serviceCats = _db.ServiceCategories.OrderBy(x => x.Name).Select(x => x.ServiceCategoryCode).ToList();

                var inventoryData = _db.GoodReceiveItems
                .Where(x => x.GoodReceive.IsOnlineApproved && !x.GoodReceive.IsVoid && x.GoodReceive.ReceiveDate >= fyearObj.DateFrom && x.GoodReceive.ReceiveDate <= periodObj.DateTo)
                .AsNoTracking()
                .Include(x => x.GoodReceive)
                .ToList();

                var inventoryIssueData = _db.IssueItems
               .Where(x => x.Issue.IsOnlineApproved && !x.Issue.IsVoid && x.Issue.IssueDate >= fyearObj.DateFrom && x.Issue.IssueDate <= periodObj.DateTo)
               .AsNoTracking()
               .Include(x => x.Issue)
               .ToList();

                var fixedAssetData = _db.FixedAssetReceiveItems
                .Where(x => x.FixedAssetReceive.IsOnlineApproved && !x.FixedAssetReceive.IsVoid && x.FixedAssetReceive.ReceiveDate >= fyearObj.DateFrom && x.FixedAssetReceive.ReceiveDate <= periodObj.DateTo)
                .AsNoTracking()
                .Include(x => x.FixedAssetReceive)
                .ToList();

                var fixedAssetIssueData = _db.FixedAssetIssueItems
                .Where(x => x.FixedAssetIssue.IsOnlineApproved && !x.FixedAssetIssue.IsVoid && x.FixedAssetIssue.IssueDate >= fyearObj.DateFrom && x.FixedAssetIssue.IssueDate <= periodObj.DateTo)
                .AsNoTracking()
                .Include(x => x.FixedAssetIssue)
                .ToList();

                var serviceData = _db.ServiceReceiveItems
                .Where(x => x.ServiceReceive.IsOnlineApproved && !x.ServiceReceive.IsVoid && x.ServiceReceive.ReceiveDate >= fyearObj.DateFrom && x.ServiceReceive.ReceiveDate <= periodObj.DateTo)
                .AsNoTracking()
                .Include(x => x.ServiceReceive)
                .ToList();

                var record = new SupplyPlanActualVm();
               

                foreach (var cat in fixedCats)
                {
                    var catObj = _db.FixedAssetCategories.Where(c => c.CategoryCode == cat).FirstOrDefault();
                    #region cat header
                    record = new SupplyPlanActualVm
                    {
                        Category = _db.FixedAssetCategories.FirstOrDefault(x => x.CategoryCode == cat)?.CategoryName,
                        Unit = "",
                        Isheader = true,
                        Items = new List<SupplyPlanActualVm>()
                    };

                    #endregion
                    foreach (var b in businessUnits)
                    {
                        List<string> bizUnitStores = InventoryHelper.GetStoreByBusinessUnit(b.BusinessUnitId);

                        var businessunitPlan = planItems
                            .Where(x => x.ProcurementPlan.BusinessUnitId == b.BusinessUnitId)
                            .Where(x => x.CatCode == cat)
                            .Where(x => x.ItemType == PurchaseItemType.FixedAsset)
                            .ToList();

                        var businessUnitFixedAsset = fixedAssetData
                            .Where(x => x.FixedAssetCategoryID == catObj.FixedAssetCategoryID)
                            .Where(x => bizUnitStores.Contains(x.FixedAssetReceive.StoreCode))
                            .ToList();

                        var businessUnitFixedAssetIssue = fixedAssetIssueData
                            .Where(x => x.FixedAssetCategoryId == catObj.FixedAssetCategoryID)
                            .Where(x => bizUnitStores.Contains(x.FixedAssetIssue.StoreCode))
                            .ToList();


                        GetPlan(periodObj, businessunitPlan, out decimal cq, out decimal cp, out decimal tq, out decimal tp);
                        GetExecutedPPE(fyearObj, periodObj, businessUnitFixedAsset, out decimal ceq, out decimal cep, out decimal teq, out decimal tep);
                        GetIssuedPPE(fyearObj, periodObj, businessUnitFixedAssetIssue, out decimal teiq, out decimal teip);

                        #region business unit 
                        var item = new SupplyPlanActualVm
                        {
                            Category = b.Name,
                            AnnualPlanQty = businessunitPlan.Any() ? businessunitPlan.Sum(x => x.Quantity) : 0,
                            AnnualPlanPrice = businessunitPlan.Any() ? businessunitPlan.Sum(x => x.TotalPrice) : 0,
                            CurrentMonthPlanQty = cq,
                            CurrentMonthPlanPrice = cp,
                            CurrentMonthExcecutedQty = ceq,
                            CurrentMonthExcecutedPrice = cep,
                            CurrentMonthPercentage = cp > 0 ? (cep / cp) * 100 : 0,
                            ToDatePlanQty = tq,
                            ToDatePlanPrice = tp,
                            ToDateExcecutedQty = teq,
                            ToDateExcecutedPrice = tep,
                            ToDatePercentage = tp > 0 ? (tep / tp) * 100 : 0,
                            TodateIssuedQty = teiq,
                            ToDateIssuedPrice = teip,
                            ToDateIssuedPercentage = tp > 0 ? (teip / tp) * 100 : 0,
                        };

                        if (businessunitPlan.Any() || businessUnitFixedAsset.Any())
                        {
                            record.Items.Add(item);
                        }
                        #endregion
                    }
                    // Sum the fields from the Items list and assign to the record
                    record.AnnualPlanQty = record.Items.Sum(x => x.AnnualPlanQty);
                    record.AnnualPlanPrice = record.Items.Sum(x => x.AnnualPlanPrice);
                    record.CurrentMonthPlanQty = record.Items.Sum(x => x.CurrentMonthPlanQty);
                    record.CurrentMonthPlanPrice = record.Items.Sum(x => x.CurrentMonthPlanPrice);
                    record.CurrentMonthExcecutedQty = record.Items.Sum(x => x.CurrentMonthExcecutedQty);
                    record.CurrentMonthExcecutedPrice = record.Items.Sum(x => x.CurrentMonthExcecutedPrice);
                    record.ToDatePlanQty = record.Items.Sum(x => x.ToDatePlanQty);
                    record.ToDatePlanPrice = record.Items.Sum(x => x.ToDatePlanPrice);
                    record.ToDateExcecutedQty = record.Items.Sum(x => x.ToDateExcecutedQty);
                    record.ToDateExcecutedPrice = record.Items.Sum(x => x.ToDateExcecutedPrice);
                    record.TodateIssuedQty = record.Items.Sum(x => x.TodateIssuedQty);
                    record.ToDateIssuedPrice = record.Items.Sum(x => x.ToDateIssuedPrice);

                    // Only add the record if at least one of the summed values is greater than 0
                    if (record.AnnualPlanQty > 0 || record.AnnualPlanPrice > 0 ||
                        record.CurrentMonthPlanQty > 0 || record.CurrentMonthPlanPrice > 0 ||
                        record.CurrentMonthExcecutedQty > 0 || record.CurrentMonthExcecutedPrice > 0 ||
                        record.ToDatePlanQty > 0 || record.ToDatePlanPrice > 0 ||
                        record.ToDateExcecutedQty > 0 || record.ToDateExcecutedPrice > 0 ||
                        record.TodateIssuedQty > 0 || record.ToDateIssuedPrice > 0)
                    {
                        list.Add(record);
                    }
                }

                
            }


            return list;
        }

        private void GetPlan(GLPeriod p, List<ProcurementPlanItem> plan, out decimal cq, out decimal cp, out decimal tq, out decimal tp)
        {

            cq = 0;
            cp = 0;
            tq = 0;
            tp = 0;
            switch (p.Index)
            {
                case 1:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period1;
                        cp = cp + cp * item.UnitPrice;

                        tq = tq + item.Period1;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 2:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period2;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq + item.Period1 + item.Period2;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 3:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period3;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 4:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period4;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 5:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period5;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4 +
                            item.Period5
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 6:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period6;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4 +
                            item.Period5 +
                            item.Period6
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 7:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period7;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4 +
                            item.Period5 +
                            item.Period6 +
                            item.Period7
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 8:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period8;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4 +
                            item.Period5 +
                            item.Period6 +
                            item.Period7 +
                            item.Period8
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 9:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period9;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4 +
                            item.Period5 +
                            item.Period6 +
                            item.Period7 +
                            item.Period8 +
                            item.Period9
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 10:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period10;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4 +
                            item.Period5 +
                            item.Period6 +
                            item.Period7 +
                            item.Period8 +
                            item.Period9 +
                            item.Period10
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 11:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period11;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4 +
                            item.Period5 +
                            item.Period6 +
                            item.Period7 +
                            item.Period8 +
                            item.Period9 +
                            item.Period10 +
                            item.Period11
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
                case 12:
                    foreach (var item in plan)
                    {
                        cq = cq + item.Period12;
                        cp = cp + cq * item.UnitPrice;

                        tq = tq +
                            item.Period1 +
                            item.Period2 +
                            item.Period3 +
                            item.Period4 +
                            item.Period5 +
                            item.Period6 +
                            item.Period7 +
                            item.Period8 +
                            item.Period9 +
                            item.Period10 +
                            item.Period11 +
                            item.Period12
                            ;
                        tp = tp + tq * item.UnitPrice;
                    }
                    break;
            }

        }

        private void GetExecutedPPE(GlFiscalYear f, GLPeriod p, List<FixedAssetReceiveItem> receiveItems, out decimal ceq, out decimal cep, out decimal teq, out decimal tep)
        {

            ceq = 0;
            cep = 0;
            teq = 0;
            tep = 0;
            var currentItems = receiveItems.Where(x => x.FixedAssetReceive.ReceiveDate >= p.DateFrom && x.FixedAssetReceive.ReceiveDate <= p.DateTo).ToList();
            if (currentItems.Any())
            {
                ceq = currentItems.Sum(x => x.Quantity);
                cep = currentItems.Sum(x => x.Total);
            }
            var toDateItems = receiveItems.Where(x => x.FixedAssetReceive.ReceiveDate >= f.DateFrom && x.FixedAssetReceive.ReceiveDate <= p.DateTo).ToList();
            if (toDateItems.Any())
            {
                teq = toDateItems.Sum(x => x.Quantity);
                tep = toDateItems.Sum(x => x.Total);
            }
        }

        private void GetIssuedPPE(GlFiscalYear f, GLPeriod p, List<FixedAssetIssueItem> receiveItems, out decimal teiq, out decimal teip)
        {
            teiq = 0;
            teip = 0;

            var toDateItems = receiveItems.Where(x => x.FixedAssetIssue.IssueDate >= f.DateFrom && x.FixedAssetIssue.IssueDate <= p.DateTo).ToList();
            if (toDateItems.Any())
            {
                teiq = toDateItems.Sum(x => x.IssuedQuantity);
                teip = toDateItems.Sum(x => x.Total);
            }
        }
    }
}
