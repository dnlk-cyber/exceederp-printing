using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class MonthlyPurchaseByCategoryAndAmountOds
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MonthlyPurchaseByCategoryAndAmountReportViewModel> GetPurchases(DateTime from,DateTime to,string type, string purchaser, string branchid)
        {
            List< MonthlyPurchaseByCategoryAndAmountReportViewModel > monthlyPurchaseList = new List<MonthlyPurchaseByCategoryAndAmountReportViewModel>();

            var manualPurchaseOrders = _db.PurchaseOrders
                .Where(x => x.IsManual)
                .Where(x => x.OrderDate >= from && x.OrderDate <= to)
                .Where(x => x.IsVoid == false && x.IsOnlineApproved).AsQueryable();

            var purchaseOrders = _db.PurchaseOrders
                .Where(x => x.IsManual == false)
                .Where(x => x.OrderDate >= from && x.OrderDate <= to)
                .Where(x => x.IsVoid == false && x.IsOnlineApproved).AsQueryable();

            var tenders = _db.Tenders
                .Where(x => x.FloatDate >= from && x.FloatDate <= to)
                .Where(x => x.IsVoid == false && x.IsOnlineApproved).AsQueryable();

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                int did = int.Parse(branchid);
                manualPurchaseOrders = manualPurchaseOrders
                    .Where(x => x.BranchId == did).AsQueryable();

                purchaseOrders = purchaseOrders
                    .Where(x => x.BranchId == did).AsQueryable();

                tenders = tenders
                    .Where(x => x.BranchId == did).AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                int empid = int.Parse(purchaser);

                manualPurchaseOrders = manualPurchaseOrders
                    .Where(x => x.ProcurementOfficer == empid).AsQueryable();

                purchaseOrders = purchaseOrders
                    .Where(x => x.ProcurementOfficer == empid).AsQueryable();

                tenders = tenders
                    .Where(x => x.ProcurementOfficer == empid).AsQueryable();
            }

            string purchasetype = !String.IsNullOrWhiteSpace(type) ? type : "All";
            if (purchasetype == "All" || purchasetype == "Direct Purchase")
            {
                #region manual po

                foreach (var purchaseOrder in manualPurchaseOrders)
                {
                    var purchase = new MonthlyPurchaseByCategoryAndAmountReportViewModel
                    {
                        No=purchaseOrder.PurchaseOrderID,
                        PurchaseType = "PO-Direct Purchase",
                        Price = purchaseOrder.PurchaseOrderItem.Sum(x => x.Total)
                    };
                    monthlyPurchaseList.Add(purchase);
                }

                #endregion
            }
            if (purchasetype == "All" || purchasetype == "Proforma")
            {
                #region auto po

                foreach (var purchaseOrder in purchaseOrders)
                {
                    var purchase = new MonthlyPurchaseByCategoryAndAmountReportViewModel
                    {
                        No = purchaseOrder.PurchaseOrderID,
                        PurchaseType = "PO-Proforma Purchase",
                        Price = purchaseOrder.PurchaseOrderItem.Sum(x => x.Total)
                    };

                    monthlyPurchaseList.Add(purchase);
                }

                #endregion
            }
            if (purchasetype == "All" || purchasetype == "Tender")
            {
                #region tender

                foreach (var tender in tenders)
                {
                    var lots = tender.Lots.ToList();

                    foreach (var lot in lots)
                    {
                        var awards = lot.BidAwardedSupplier.ToList();

                        foreach (var award in awards)
                        {
                            var purchase = new MonthlyPurchaseByCategoryAndAmountReportViewModel
                            {
                                No = tender.TenderID,
                                PurchaseType = "Tender-Award",
                                Price = award.BidWinningItems.Sum(x => x.FinalUPrice)
                            };

                            monthlyPurchaseList.Add(purchase);
                        }


                    }

                }

                #endregion
            }

            return monthlyPurchaseList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> PurchaseType()
        {

            List<string> list = new List<string> { "Direct Purchase", "Proforma", "Tender" };
            return list;
        }
    }
}
