using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
   
    [DataObject]
    public class PurchaseSummaryReportByPurchaseMethodObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PurchaseSummaryReportByPurchaseMethodViewModel> GetPurchasesByDept(DateTime from,DateTime to, string purchaser, string businessUnitId)
        {
            List<PurchaseSummaryReportByPurchaseMethodViewModel> list = new List<PurchaseSummaryReportByPurchaseMethodViewModel>();
            //var proformaRequisitions = db.ProformaPurchaseRequisitions.ToList();
         
            //var purchaseRequisitions = db.PurchaseRequisitions.ToList();

            var tenders = db.Tenders.AsQueryable();
            var purchaseOrders = db.PurchaseOrders.Where(x=>x.IsManual==true).AsQueryable();
            var proformaPurchase = db.ProformaPurchases.AsQueryable();

            if (!String.IsNullOrWhiteSpace(purchaser))
            {

                var prassignments = db.PurchaseRequisitionAssignments
                    .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                    .ToList();

                //purchaseRequisitions = purchaseRequisitions.Where(x => prassignments.Contains(x.PurchaseRequisitionID)).ToList();

                tenders = tenders.Where(x => x.ProcurementOfficer.ToString()==purchaser).AsQueryable();
                proformaPurchase = proformaPurchase.Where(x => x.ProcurementOfficer.ToString() == purchaser).AsQueryable();
                purchaseOrders = purchaseOrders.Where(x => x.ProcurementOfficer.ToString() == purchaser).AsQueryable();
            }

            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                tenders = tenders.Where(x => x.BusinessUnitId == bdid).AsQueryable();
                proformaPurchase = proformaPurchase.Where(x => x.BusinessUnitId == bdid).AsQueryable();
                purchaseOrders = purchaseOrders.Where(x => x.BusinessUnitId == bdid).AsQueryable();
            }

            var purchaseSupport = new PurchaseSummaryReportByPurchaseMethodViewModel();
            #region Number of Purchase Requisition in aMonth

            decimal open1 = 0;
            decimal selected1 = 0;
            decimal proforma1 = 0;
            decimal direct1 = 0;

            #region tender

            foreach (var tender in tenders)
            {
                var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

                foreach (var lot in lots)
                {
                    var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();

                    foreach (var award in awards)
                    {
                        if (tender.TenderType == TenderType.Open)
                        {
                            var items = db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID).ToList();
                            if (items.Any())
                                open1 = open1 +items.Sum(x => x.TPrice);

                        }
                        else if (tender.TenderType == TenderType.Selected)
                        {
                            var items = db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID).ToList();
                            if (items.Any())
                                selected1 = selected1 + items.Sum(x => x.TPrice);

                        }
                    }

                }
            }

            #endregion
            #region proforma

            foreach (var proforma in proformaPurchase)
            {
                var awards = db.WinningSuppliers.Where(x => x.ProformaPurchaseID == proforma.ProformaPurchaseID).ToList();

                foreach (var award in awards)
                {
                    var items = db.WinningSupplierItems.Where(x => x.WinningSupplierID == award.WinningSupplierID).ToList();
                    if (items.Any())
                        proforma1 = proforma1 + items.Sum(x => x.TPrice);
                }
            }

            #endregion
            #region direct

            foreach (var po in purchaseOrders)
            {
                var items = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == po.PurchaseOrderID).ToList();
                if(items.Any())
                direct1 = direct1 + items.Sum(x => x.Total);
            }

            #endregion
            purchaseSupport.Descrption = "Number of Purchase Requisition";
            purchaseSupport.OpenTenders = open1;
            purchaseSupport.SelectedTenders = selected1;
            purchaseSupport.Proforma = proforma1;
            purchaseSupport.DirectPurchase = direct1;

            list.Add(purchaseSupport);
            #endregion

            //#region Number of Opened Technical Tender Requisition

            //decimal open2 = 0;
            //decimal selected2 = 0;
            //decimal proforma2 = 0;
            //decimal direct2 = 0;
            //#region tender

            //var technicalOpen = tenders.Where(x => x.IsTechnicalOpened).ToList();

            //foreach (var tender in technicalOpen)
            //{
            //    var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

            //    foreach (var lot in lots)
            //    {
            //        var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();

            //        foreach (var award in awards)
            //        {
            //            if (tender.TenderType == TenderType.Open)
            //            {
            //                var items = db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID).ToList();
            //                if (items.Any())
            //                    open2 = open2 + items.Sum(x => x.TPrice);

            //            }
            //            else if (tender.TenderType == TenderType.Selected)
            //            {
            //                var items = db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID).ToList();
            //                if (items.Any())
            //                    selected2 = selected2 + items.Sum(x => x.TPrice);

            //            }
            //        }

            //    }
            //}

            //#endregion

            //purchaseSupport =new PurchaseSummaryReportByPurchaseMethodViewModel
            //    {
            //        Descrption = "Number of Opened Technical Tender Requisition",
            //        OpenTenders = open2,
            //        SelectedTenders = selected2,
            //        Proforma = proforma2,
            //        DirectPurchase = proforma2
            //    };

            //#endregion
            //purchaseSupportList.Add(purchaseSupport);
            //#region Number of Opened Financial Tender Requisition

            //decimal open3 = 0;
            //decimal selected3 = 0;
            //decimal proforma3 = 0;
            //decimal direct3 = 0;
            //#region tender

            //var financialOpen = tenders.Where(x => x.IsFinancialOpened).ToList();

            //foreach (var tender in financialOpen)
            //{
            //    var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

            //    foreach (var lot in lots)
            //    {
            //        var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();

            //        foreach (var award in awards)
            //        {
            //            if (tender.TenderType == TenderType.Open)
            //            {
            //                var items = db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID).ToList();
            //                if (items.Any())
            //                    open3 = open3 + items.Sum(x => x.TPrice);

            //            }
            //            else if (tender.TenderType == TenderType.Selected)
            //            {
            //                var items = db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID).ToList();
            //                if (items.Any())
            //                    selected3 = selected3 + items.Sum(x => x.TPrice);

            //            }
            //        }

            //    }
            //}

            //#endregion

            //purchaseSupport = new PurchaseSummaryReportByPurchaseMethodViewModel
            //    {
            //        Descrption = "Number of Opened Financial Tender Requisition",
            //        OpenTenders = open3,
            //        SelectedTenders = selected3,
            //        Proforma = proforma3,
            //        DirectPurchase = direct3
            //    };

            //#endregion
            //purchaseSupportList.Add(purchaseSupport);
            //#region Not Opend Financial and Technical Tenders

            //decimal open4 = 0;
            //decimal selected4 = 0;
            //decimal proforma4 = 0;
            //decimal direct4 = 0;
            //#region tender

            //var financialAndTechNotOpen = tenders.Where(x => x.IsFinancialOpened==false && x.IsTechnicalOpened==false).ToList();

            //foreach (var tender in financialAndTechNotOpen)
            //{
            //    var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

            //    foreach (var lot in lots)
            //    {
            //        var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();

            //        foreach (var award in awards)
            //        {
            //            if (tender.TenderType == TenderType.Open)
            //            {
            //                var items = db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID).ToList();
            //                if (items.Any())
            //                    open4 = open4 + items.Sum(x => x.TPrice);

            //            }
            //            else if (tender.TenderType == TenderType.Selected)
            //            {
            //                var items = db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID).ToList();
            //                if (items.Any())
            //                    selected4 = selected4 + items.Sum(x => x.TPrice);

            //            }
            //        }

            //    }
            //}

            //#endregion
            //purchaseSupport = new PurchaseSummaryReportByPurchaseMethodViewModel
            //    {
            //        Descrption = "Not Opend Financial and Technical Tenders",
            //        OpenTenders = open4,
            //        SelectedTenders = selected4,
            //        Proforma = proforma4,
            //        DirectPurchase = direct4
            //    };

            //#endregion
            //purchaseSupportList.Add(purchaseSupport);
            //#region Number of Complained Tenders

            //purchaseSupport = new PurchaseSummaryReportByPurchaseMethodViewModel();
            //purchaseSupport.Descrption = "Number of Complained Tenders";
            //purchaseSupport.OpenTenders = 0;
            //purchaseSupport.SelectedTenders = 0;
            //purchaseSupport.Proforma = 0;
            //purchaseSupport.DirectPurchase = 0;
            //#endregion
            //purchaseSupportList.Add(purchaseSupport);
            //#region Number of Cancled Tenders

            //purchaseSupport = new PurchaseSummaryReportByPurchaseMethodViewModel();
            //purchaseSupport.Descrption = "Number of Cancled Tenders";
            //purchaseSupport.OpenTenders = 0;
            //purchaseSupport.SelectedTenders = 0;
            //purchaseSupport.Proforma = 0;
            //purchaseSupport.DirectPurchase = 0;
            //#endregion
            //purchaseSupportList.Add(purchaseSupport);
            //#region Number of Contract Purchase

            //purchaseSupport = new PurchaseSummaryReportByPurchaseMethodViewModel();
            //purchaseSupport.Descrption = "Number of Contract Purchase";
            //purchaseSupport.OpenTenders = 0;
            //purchaseSupport.SelectedTenders = 0;
            //purchaseSupport.Proforma = 0;
            //purchaseSupport.DirectPurchase = 0;
            //#endregion
            //purchaseSupportList.Add(purchaseSupport);
            //#region Number of Purchased Requisition

            //purchaseSupport = new PurchaseSummaryReportByPurchaseMethodViewModel();
            //purchaseSupport.Descrption = "Number of Purchased Requisition";
            //purchaseSupport.OpenTenders = 0;
            //purchaseSupport.SelectedTenders = 0;
            //purchaseSupport.Proforma = 0;
            //purchaseSupport.DirectPurchase = 0;
            //#endregion

            // purchaseSupportList.Add(purchaseSupport);

            return list;
        }

    }
}
