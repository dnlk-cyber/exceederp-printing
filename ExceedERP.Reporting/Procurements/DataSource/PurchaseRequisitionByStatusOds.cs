using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.Print.DataSource;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class PurchaseRequisitionByStatusOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NoOfPurchasedRequisitionViewModel> PurchaseRequisitionByStatus(DateTime from, DateTime to, string branchid, string purchaser, string status, string businessUnitId)
        {
            List<NoOfPurchasedRequisitionViewModel> list = new List<NoOfPurchasedRequisitionViewModel>();
            var branch = db.Branch.ToList();
            
            //var purchase = db.PurchaseRequisitions
            //    .Where(x=>x.IsVoid==false&&x.IsOnlineApproved)
            //    .Where(x => x.RequestDate >= from && x.RequestDate <= to).ToList();

            var tenders = db.Tenders
                .Where(x => x.FloatDate >= from && x.FloatDate <= to)
                .Where(x => x.IsVoid == false && x.IsOnlineApproved)
                .AsQueryable();
            var proformaPurchases = db.ProformaPurchases
                .Where(x => x.FloatDate >= from && x.FloatDate <= to)
                .Where(x => x.IsVoid == false && x.IsOnlineApproved)
                .AsQueryable();
            var purchaseOrders = db.PurchaseOrders
                .Where(x => x.OrderDate >= from && x.OrderDate <= to)
                .Where(x => x.IsManual).Where(x => x.IsVoid == false && x.IsOnlineApproved).AsQueryable();

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                int did = int.Parse(branchid);
                tenders = tenders.Where(x => x.BranchId == did).AsQueryable();
                proformaPurchases = proformaPurchases.Where(x => x.BranchId == did).AsQueryable();
                purchaseOrders = purchaseOrders.Where(x => x.BranchId == did).AsQueryable();
            }

            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                tenders = tenders.Where(x => x.BusinessUnitId == bdid).AsQueryable();
                proformaPurchases = proformaPurchases.Where(x => x.BusinessUnitId == bdid).AsQueryable();
                purchaseOrders = purchaseOrders.Where(x => x.BusinessUnitId == bdid).AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                //var prassignments = db.PurchaseRequisitionAssignments
                //    .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                //    .ToList();

                //purchase = purchase
                //    .Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                //    .ToList();

                tenders = tenders.Where(x => x.ProcurementOfficer.ToString() == purchaser).AsQueryable();
                proformaPurchases = proformaPurchases.Where(x => x.ProcurementOfficer.ToString() == purchaser).AsQueryable();
                purchaseOrders = purchaseOrders.Where(x => x.ProcurementOfficer.ToString() == purchaser).AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(status))
            {
                switch (status)
                {
                    case "In Progress":
                        tenders = tenders.Where(x =>
                                x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Pending
                                && x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled
                                && x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Awarded)
                            .AsQueryable();
                        purchaseOrders = purchaseOrders.Where(x =>
                                x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Pending
                                && x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled
                                && x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Awarded)
                            .AsQueryable();
                        proformaPurchases = proformaPurchases.Where(x =>
                                x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Pending
                                && x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled
                                && x.PurchaseStatus != Core.Domain.Procurement.Settings.PurchaseStatus.Awarded)
                            .AsQueryable();
                        break;
                    case "Pending":

                        tenders = tenders.Where(x =>
                            x.PurchaseStatus == Core.Domain.Procurement.Settings.PurchaseStatus.Pending).AsQueryable();
                        purchaseOrders = purchaseOrders.Where(x =>
                            x.PurchaseStatus == Core.Domain.Procurement.Settings.PurchaseStatus.Pending).AsQueryable();
                        proformaPurchases = proformaPurchases.Where(x =>
                            x.PurchaseStatus == Core.Domain.Procurement.Settings.PurchaseStatus.Pending).AsQueryable();
                        break;
                    case "Cancelled":

                        tenders = tenders.Where(x =>
                            x.PurchaseStatus == Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled).AsQueryable();
                        purchaseOrders = purchaseOrders.Where(x =>
                            x.PurchaseStatus == Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled).AsQueryable();
                        proformaPurchases = proformaPurchases.Where(x =>
                            x.PurchaseStatus == Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled).AsQueryable();
                        break;
                }
            }

            #region tender

            foreach (var tender in tenders)
            {
                var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();
                foreach (var lot in lots)
                {
                    var lotPurchases= db.LotPurchaseRequisitions.Where(x => x.LotID == lot.LotID).ToList();

                    foreach (var lotPurchase in lotPurchases)
                    {
                        var purchaseReq = db.PurchaseRequisitions.Find(lotPurchase.PurchaseRequisitionID);
                        if (purchaseReq != null)
                        {
                            var noOfApprovedRequisition = new NoOfPurchasedRequisitionViewModel
                            {
                                Branch = branch.FirstOrDefault(x => x.BranchId == purchaseReq.BranchId)?.Name,
                                ItemType = purchaseReq.ItemType,
                                ReferenceNo = purchaseReq.ReferenceNo,
                                RequisitionNo = purchaseReq.PurchaseRequisitionID,
                                GenerateDate = purchaseReq.RequestDate,
                                ApprovedDate = purchaseReq.SecondOnlineApprovedTime?.ToShortDateString() ?? "",
                                Workunit = InventoryPrintHelper.WorkUnitName(purchaseReq.OrgStructureId),
                            };

                            list.Add(noOfApprovedRequisition);
                        }
                    }
                }

            }

            #endregion

            #region proforma

            foreach (var proformaPurchase in proformaPurchases)
            {
                var proformaPurchaseRequisitions = db.ProformaPurchaseRequisitions.Where(x => x.ProformaPurchaseID == proformaPurchase.PurchaseRequisitionID).ToList();

                foreach (var purchaseRequisition in proformaPurchaseRequisitions)
                {
                    var purchaseReq = db.PurchaseRequisitions.Find(purchaseRequisition.PurchaseRequisitionID);
                    if (purchaseReq != null)
                    {
                        var noOfApprovedRequisition = new NoOfPurchasedRequisitionViewModel
                        {
                            Branch = branch.FirstOrDefault(x => x.BranchId == purchaseReq.BranchId)?.Name,
                            ItemType = purchaseReq.ItemType,
                            ReferenceNo = purchaseReq.ReferenceNo,
                            RequisitionNo = purchaseReq.PurchaseRequisitionID,
                            GenerateDate = purchaseReq.RequestDate,
                            ApprovedDate = purchaseReq.SecondOnlineApprovedTime?.ToShortDateString() ?? "",
                            Workunit = InventoryPrintHelper.WorkUnitName(purchaseReq.OrgStructureId),
                        };

                        list.Add(noOfApprovedRequisition);
                    }
                }

            }

            #endregion

            #region po

            foreach (var purchaseOrder in purchaseOrders)
            {
                var purchaseReq = db.PurchaseRequisitions.Find(purchaseOrder.PurchaseRequisitionID);
                if (purchaseReq != null)
                {
                    var noOfApprovedRequisition = new NoOfPurchasedRequisitionViewModel
                    {
                        Branch = branch.FirstOrDefault(x => x.BranchId == purchaseReq.BranchId)?.Name,
                        ItemType = purchaseReq.ItemType,
                        ReferenceNo = purchaseReq.ReferenceNo,
                        RequisitionNo = purchaseReq.PurchaseRequisitionID,
                        GenerateDate = purchaseReq.RequestDate,
                        ApprovedDate = purchaseReq.SecondOnlineApprovedTime?.ToShortDateString() ?? "",
                        Workunit = InventoryPrintHelper.WorkUnitName(purchaseReq.OrgStructureId),
                    };

                    list.Add(noOfApprovedRequisition);
                }

            }

            #endregion

            return list;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> PurchaseStatus()
        {

            List<string> list = new List<string> {"In Progress", "Pending", "Cancelled"};
            return list;
        }

    }
}