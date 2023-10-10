using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class NoOfPurchasedRequisitionObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)] 
        public List<NoOfPurchasedRequisitionViewModel> GetNoOfPurchasedRequisition(DateTime from, DateTime to,string type, string branchid, string purchaser, string businessUnitId)
        {
            List<NoOfPurchasedRequisitionViewModel> list = new List<NoOfPurchasedRequisitionViewModel>();
            var branches = db.Branch.ToList();

            string itemtype = !String.IsNullOrWhiteSpace(type)?type:"All";

            if (itemtype == "All" || itemtype == "Stock")
            {
                #region stock

                var stock = db.PurchaseRequisitions
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false && x.ItemType == PurchaseItemType.Stock)
                    .Where(x => x.OnlineApprovedTime >= from && x.OnlineApprovedTime <= to).AsQueryable();

                if (!String.IsNullOrWhiteSpace(purchaser))
                {
                    var prassignments = db.PurchaseRequisitionAssignments
                        .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                        .ToList();
                    stock = stock
                        .Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                        .AsQueryable();
                }

                if (!String.IsNullOrEmpty(branchid))
                {
                    stock = stock
                        .Where(x => x.BranchId.ToString() == branchid).AsQueryable();
                }



                foreach (var branch in branches)
                {
                    int proforma = 0;
                    int openTender = 0;
                    int selectedTender = 0;
                    int directPurchase = 0;

                    var noOfApprovedRequisitions = new NoOfPurchasedRequisitionViewModel
                    {
                        Branch = branch.Name,
                        ItemType = PurchaseItemType.Stock
                    };

                    var branchStockList = stock.Where(x => x.BranchId == branch.BranchId).ToList();
                    if (!branchStockList.Any())
                        continue;
                    foreach (var purchaseRequisition in branchStockList)
                    {

                        var proformas = db.ProformaPurchaseRequisitions
                            .Where(x => x.ProformaPurchase.IsOnlineApproved)
                            .Where(x => x.ProformaPurchase.IsVoid == false)
                            .FirstOrDefault(x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (proformas != null)
                            proforma++;

                        var purcaseOrder = db.PurchaseOrders
                            .Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid == false).FirstOrDefault(x =>
                                x.IsManual && x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (purcaseOrder != null)
                            directPurchase++;

                        var lotPurchaseRequest = db.LotPurchaseRequisitions.FirstOrDefault(x =>
                            x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (lotPurchaseRequest != null)
                        {
                            var lot = db.Lots.FirstOrDefault(x => x.LotID == lotPurchaseRequest.LotID);
                            if (lot != null)
                            {
                                var tenderOpen = db.Tenders.Where(x => x.IsOnlineApproved)
                                    .Where(x => x.IsVoid == false)
                                    .FirstOrDefault(x => x.TenderID == lot.TenderID && x.TenderType == TenderType.Open);
                                if (tenderOpen != null)
                                    openTender++;

                                var tenderSelected = db.Tenders.Where(x => x.IsOnlineApproved)
                                    .Where(x => x.IsVoid == false)
                                    .FirstOrDefault(x =>
                                        x.TenderID == lot.TenderID && x.TenderType == TenderType.Selected);
                                if (tenderSelected != null)
                                    selectedTender++;
                            }
                        }
                    }

                    noOfApprovedRequisitions.OpenTender = openTender;
                    noOfApprovedRequisitions.SelectedTender = selectedTender;
                    noOfApprovedRequisitions.Proforma = proforma;
                    noOfApprovedRequisitions.DirectPurchase = directPurchase;
                    list.Add(noOfApprovedRequisitions);

                }






                #endregion
            }
            if (itemtype == "All" || itemtype == "Fixed Asset")
            {
                #region fixed asset

                var fixedAssetReq = db.PurchaseRequisitions
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false && x.ItemType == PurchaseItemType.FixedAsset)
                    .Where(x => x.OnlineApprovedTime >= from && x.OnlineApprovedTime <= to).AsQueryable();

                if (!String.IsNullOrWhiteSpace(purchaser))
                {
                    var prassignments = db.PurchaseRequisitionAssignments
                        .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                        .ToList();
                    fixedAssetReq = fixedAssetReq
                        .Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                        .AsQueryable();
                }

                if (!String.IsNullOrEmpty(branchid))
                {
                    fixedAssetReq = fixedAssetReq
                        .Where(x => x.BranchId.ToString() == branchid).AsQueryable();
                }




                foreach (var branch in branches)
                {
                    int proforma = 0;
                    int openTender = 0;
                    int selectedTender = 0;
                    int directPurchase = 0;

                    var noOfApprovedRequisitions = new NoOfPurchasedRequisitionViewModel
                    {
                        Branch = branch.Name,
                        ItemType = PurchaseItemType.FixedAsset
                    };

                    var branchStockList = fixedAssetReq.Where(x => x.BranchId == branch.BranchId).ToList();
                    if (!branchStockList.Any())
                        continue;

                    foreach (var purchaseRequisition in branchStockList)
                    {

                        var proformas = db.ProformaPurchaseRequisitions
                            .Where(x => x.ProformaPurchase.IsOnlineApproved)
                            .Where(x => x.ProformaPurchase.IsVoid == false)
                            .FirstOrDefault(x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (proformas != null)
                            proforma++;

                        var purcaseOrder = db.PurchaseOrders
                            .Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid == false).FirstOrDefault(x =>
                                x.IsManual && x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (purcaseOrder != null)
                            directPurchase++;

                        var lotPurchaseRequest = db.LotPurchaseRequisitions.FirstOrDefault(x =>
                            x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (lotPurchaseRequest != null)
                        {
                            var lot = db.Lots.FirstOrDefault(x => x.LotID == lotPurchaseRequest.LotID);
                            if (lot != null)
                            {
                                var tenderOpen = db.Tenders.Where(x => x.IsOnlineApproved)
                                    .Where(x => x.IsVoid == false)
                                    .FirstOrDefault(x => x.TenderID == lot.TenderID && x.TenderType == TenderType.Open);
                                if (tenderOpen != null)
                                    openTender++;

                                var tenderSelected = db.Tenders.Where(x => x.IsOnlineApproved)
                                    .Where(x => x.IsVoid == false)
                                    .FirstOrDefault(x =>
                                        x.TenderID == lot.TenderID && x.TenderType == TenderType.Selected);
                                if (tenderSelected != null)
                                    selectedTender++;
                            }
                        }
                    }

                    noOfApprovedRequisitions.OpenTender = openTender;
                    noOfApprovedRequisitions.SelectedTender = selectedTender;
                    noOfApprovedRequisitions.Proforma = proforma;
                    noOfApprovedRequisitions.DirectPurchase = directPurchase;
                    list.Add(noOfApprovedRequisitions);

                }






                #endregion
            }
            if (itemtype == "All" || itemtype == "Service")
            {

                #region service

                var serviceRequisition = db.PurchaseRequisitions
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false && x.ItemType == PurchaseItemType.Service)
                    .Where(x => x.OnlineApprovedTime >= from && x.OnlineApprovedTime <= to).AsQueryable();

                if (!String.IsNullOrWhiteSpace(purchaser))
                {
                    var prassignments = db.PurchaseRequisitionAssignments
                        .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                        .ToList();
                    serviceRequisition = serviceRequisition
                        .Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                        .AsQueryable();
                }

                if (!String.IsNullOrEmpty(branchid))
                {
                    serviceRequisition = serviceRequisition
                        .Where(x => x.BranchId.ToString() == branchid).AsQueryable();
                }




                foreach (var branch in branches)
                {
                    int proforma = 0;
                    int openTender = 0;
                    int selectedTender = 0;
                    int directPurchase = 0;

                    var noOfApprovedRequisitions = new NoOfPurchasedRequisitionViewModel
                    {
                        Branch = branch.Name,
                        ItemType = PurchaseItemType.Service
                    };

                    var branchStockList = serviceRequisition.Where(x => x.BranchId == branch.BranchId).ToList();
                    if (!branchStockList.Any())
                        continue;

                    foreach (var purchaseRequisition in branchStockList)
                    {

                        var proformas = db.ProformaPurchaseRequisitions
                            .Where(x => x.ProformaPurchase.IsOnlineApproved)
                            .Where(x => x.ProformaPurchase.IsVoid == false)
                            .FirstOrDefault(x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (proformas != null)
                            proforma++;

                        var purcaseOrder = db.PurchaseOrders
                            .Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid == false).FirstOrDefault(x =>
                                x.IsManual && x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (purcaseOrder != null)
                            directPurchase++;

                        var lotPurchaseRequest = db.LotPurchaseRequisitions.FirstOrDefault(x =>
                            x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                        if (lotPurchaseRequest != null)
                        {
                            var lot = db.Lots.FirstOrDefault(x => x.LotID == lotPurchaseRequest.LotID);
                            if (lot != null)
                            {
                                var tenderOpen = db.Tenders.Where(x => x.IsOnlineApproved)
                                    .Where(x => x.IsVoid == false)
                                    .FirstOrDefault(x => x.TenderID == lot.TenderID && x.TenderType == TenderType.Open);
                                if (tenderOpen != null)
                                    openTender++;

                                var tenderSelected = db.Tenders.Where(x => x.IsOnlineApproved)
                                    .Where(x => x.IsVoid == false)
                                    .FirstOrDefault(x =>
                                        x.TenderID == lot.TenderID && x.TenderType == TenderType.Selected);
                                if (tenderSelected != null)
                                    selectedTender++;
                            }
                        }
                    }

                    noOfApprovedRequisitions.OpenTender = openTender;
                    noOfApprovedRequisitions.SelectedTender = selectedTender;
                    noOfApprovedRequisitions.Proforma = proforma;
                    noOfApprovedRequisitions.DirectPurchase = directPurchase;
                    list.Add(noOfApprovedRequisitions);

                }






                #endregion
            }

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NoOfPurchasedRequisitionViewModel> GetNoOfPr(DateTime from, DateTime to,string type, string branchid, string purchaser, string businessUnitId)
        {
            List<NoOfPurchasedRequisitionViewModel> list = new List<NoOfPurchasedRequisitionViewModel>();
            

            #region prs

            var purchaseRequisitions = db.PurchaseRequisitions
                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Where(x => x.OnlineApprovedTime >= from && x.OnlineApprovedTime <= to).AsQueryable();

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                var prassignments = db.PurchaseRequisitionAssignments
                    .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                    .ToList();
                purchaseRequisitions = purchaseRequisitions
                    .Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                    .AsQueryable();
            }

            if (!String.IsNullOrEmpty(branchid))
            {
                purchaseRequisitions = purchaseRequisitions
                    .Where(x => x.BranchId.ToString() == branchid).AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(type))
            {
                switch (type)
                {
                    case "Stock":
                        purchaseRequisitions = purchaseRequisitions
                            .Where(x => x.ItemType == PurchaseItemType.Stock).AsQueryable();
                        break;

                    case "Fixed Asset":
                        purchaseRequisitions = purchaseRequisitions
                            .Where(x => x.ItemType == PurchaseItemType.FixedAsset).AsQueryable();
                        break;
                    case "Service":
                        purchaseRequisitions = purchaseRequisitions
                            .Where(x => x.ItemType == PurchaseItemType.Service).AsQueryable();
                        break;
                }
            }

            foreach (var pr in purchaseRequisitions)
            {
                var noOfApprovedRequisitions = new NoOfPurchasedRequisitionViewModel
                {
                    Workunit = db.MainCompanyStructures.Find(pr.OrgStructureId)?.Name,
                    Branch = db.Branch.Find(pr.BranchId)?.Name,
                    ItemType = pr.ItemType,
                    ReferenceNo = pr.ReferenceNo,
                    RequisitionNo = pr.PurchaseRequisitionID

                };

                list.Add(noOfApprovedRequisitions);
            }


           






            #endregion

           

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> PrType()
        {

            List<string> list = new List<string> { "Stock", "Fixed Asset", "Service" };
            return list;
        }
    }
}
