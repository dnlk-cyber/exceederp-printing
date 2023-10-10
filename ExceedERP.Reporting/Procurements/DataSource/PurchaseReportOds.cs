using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class PurchaseReportOds
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PurchaseReportViewModel> GetPurchaseReport(DateTime from, DateTime to, string purchaser,string branchid,string purchaseType,string status, string businessUnitId)
        {
            List<PurchaseReportViewModel> list = new List<PurchaseReportViewModel>();
            string purchasetype = "All";

            var purchaseRequisions = _db.PurchaseRequisitions
                .Where(x=>x.IsSecondOnlineApproved&&x.IsVoid==false)
                .Where(x => x.RequestDate >= from && x.RequestDate <= to)
                .Include(x => x.PurchaseRequisitionAssignments).AsQueryable();

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                purchaseRequisions = purchaseRequisions
                    .Where(x => x.BranchId.ToString() == branchid).AsQueryable();
            }
            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                purchaseRequisions = purchaseRequisions.Where(x => x.BusinessUnitId == bdid).AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                //purchaseRequisions = purchaseRequisions
                //    .Where(x => x.PurchaseRequisitionAssignments.Any(s=>s.ProcurementOfficer.ToString()==purchaser)).AsQueryable();

                var prassignments = _db.PurchaseRequisitionAssignments
                    .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                    .ToList();

                purchaseRequisions = purchaseRequisions.Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                    .AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(purchaseType))
            {
                purchasetype = purchaseType;
            }

            foreach (var purchaseRequision in purchaseRequisions)
            {
                var assignments =
                    purchaseRequision.PurchaseRequisitionAssignments.Where(s =>
                        s.IsOnlineApproved && s.IsVoid == false);

                foreach (var assignment in assignments)
                {

                    if ((assignment.ProcurementMethod == ProcurementMethod.DirectPurchase)&&(purchasetype == "Direct Purchase" || purchasetype == "All"))
                    {
                        #region direct purchase

                        DateTime date = DateTime.Today;

                        var poPurchaseRequisition = _db.PoPurchaseRequisitions
                            .Where(x => x.PurchaseOrder.IsManual)
                            .FirstOrDefault(x => x.PurchaseRequisitionID == assignment.PurchaseRequisitionID);

                        if (poPurchaseRequisition != null)
                        {
                            foreach (var item in assignment.PurchaseRequisitionReassignmentItems)
                            {
                                var purchase = new PurchaseReportViewModel();

                                var poItem = poPurchaseRequisition.PurchaseOrder.PurchaseOrderItem
                                    .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                                

                                if (poPurchaseRequisition.PurchaseOrder != null && poItem != null)
                                {
                                    if (poItem.ItemType == PurchaseItemType.Stock)
                                    {
                                        #region check stock status

                                        var goodReceive = _db.GoodReceives
                                            .Where(x=>x.IsOnlineApproved&&x.IsVoid==false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder).FirstOrDefault(x => x.ReceiveRef == poItem.PurchaseOrderID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;

                                            var receiveItem = _db.GoodReceiveItems
                                                .Where(x => x.GoodReceiveId == goodReceive.GoodReceiveId)
                                                .FirstOrDefault(x => x.ItemCode == poItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }
                                            else if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else if (poItem.PurchaseOrder.PurchaseStatus == ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }
                                            else 
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }
                                    else if (poItem.ItemType == PurchaseItemType.FixedAsset)
                                    {
                                        #region check stock status

                                        var goodReceive = _db.FixedAssetReceives
                                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder).FirstOrDefault(x => x.ReceiveRef == poItem.PurchaseOrderID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;

                                            var receiveItem = _db.FixedAssetReceiveItems
                                                .Where(x => x.FixedAssetReceiveId == goodReceive.FixedAssetReceiveId)
                                                .Where(x=>x.Name==poItem.Name)
                                                .FirstOrDefault(x => x.FixedAssetSubCategoryID.ToString() == poItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }
                                            else if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else if (poItem.PurchaseOrder.PurchaseStatus == ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }
                                    else if (poItem.ItemType == PurchaseItemType.Service)
                                    {
                                        #region check stock status

                                        var goodReceive = _db.ServiceReceives
                                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder).FirstOrDefault(x => x.ReceiveRef == poItem.PurchaseOrderID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;

                                            var receiveItem = _db.ServiceReceiveItems
                                                .Where(x => x.ServiceReceiveId == goodReceive.ServiceReceiveId)
                                                .FirstOrDefault(x => x.ServiceItemCode.ToString() == poItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }
                                            else if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else if (poItem.PurchaseOrder.PurchaseStatus == ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }
                                    purchase.ItemCategory = poItem.CatCode + "-" + poItem.CategoryName;
                                    purchase.ItemName = poItem.ItmCode+"-"+poItem.ItemName;
                                    purchase.RefNo = purchaseRequision.PurchaseRequisitionID.ToString();
                                    purchase.Date = assignment.OnlineApprovedTime;
                                    purchase.PurchaseMethod = "Purchase Order";
                                    purchase.DurationForComplate =(Math.Round((date - (DateTime)assignment.OnlineApprovedTime).TotalDays, 2)).ToString();
                                    purchase.BudgetToComplete = poItem.Total;
                                    list.Add(purchase);
                                }

                            }
                        }

                        #endregion
                    }

                    else if ((assignment.ProcurementMethod == ProcurementMethod.Proforma)&&(purchasetype == "Proforma" || purchasetype == "All"))
                    {
                        #region proforma purchase

                        DateTime date = DateTime.Today;

                        var poPurchaseRequisition = _db.PoPurchaseRequisitions
                            .Where(x => x.PurchaseOrder.IsManual==false)
                            .FirstOrDefault(x => x.PurchaseRequisitionID == assignment.PurchaseRequisitionID);

                        if (poPurchaseRequisition != null)
                        {
                            foreach (var item in assignment.PurchaseRequisitionReassignmentItems)
                            {
                                var purchase = new PurchaseReportViewModel();

                                var poItem = poPurchaseRequisition.PurchaseOrder.PurchaseOrderItem
                                    .FirstOrDefault(x => x.ItmCode == item.ItmCode);

                                if (poPurchaseRequisition.PurchaseOrder != null && poItem!=null)
                                {
                                    if (poItem.ItemType == PurchaseItemType.Stock)
                                    {
                                        #region check stock status

                                        var goodReceive = _db.GoodReceives
                                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder).FirstOrDefault(x => x.ReceiveRef == poItem.PurchaseOrderID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;

                                            var receiveItem = _db.GoodReceiveItems
                                                .Where(x => x.GoodReceiveId == goodReceive.GoodReceiveId)
                                                .FirstOrDefault(x => x.ItemCode == poItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }
                                            else if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else if (poItem.PurchaseOrder.PurchaseStatus == ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }

                                    else if (poItem.ItemType == PurchaseItemType.FixedAsset)
                                    {
                                        #region check fixed status

                                        var goodReceive = _db.FixedAssetReceives
                                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder).FirstOrDefault(x => x.ReceiveRef == poItem.PurchaseOrderID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;

                                            var receiveItem = _db.FixedAssetReceiveItems
                                                .Where(x => x.FixedAssetReceiveId == goodReceive.FixedAssetReceiveId)
                                                .Where(x=>x.Name==poItem.Name)
                                                .FirstOrDefault(x => x.FixedAssetSubCategoryID.ToString() == poItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }
                                            else if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else if (poItem.PurchaseOrder.PurchaseStatus == ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }

                                    else if (poItem.ItemType == PurchaseItemType.Service)
                                    {
                                        #region check fixed status

                                        var goodReceive = _db.ServiceReceives
                                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder).FirstOrDefault(x => x.ReceiveRef == poItem.PurchaseOrderID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;

                                            var receiveItem = _db.ServiceReceiveItems
                                                .Where(x => x.ServiceReceiveId == goodReceive.ServiceReceiveId)
                                                .FirstOrDefault(x => x.ServiceItemCode == poItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }
                                            else if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            if (poItem.PurchaseOrder.IsTransferredToPayment)
                                            {
                                                purchase.CurrentStatus = "Sent for Payment";
                                            }
                                            else if (poItem.PurchaseOrder.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else if (poItem.PurchaseOrder.PurchaseStatus == ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }

                                    purchase.ItemCategory = poItem.CatCode + "-" + poItem.CategoryName ;
                                    purchase.ItemName =  poItem.ItmCode + "-" + poItem.ItemName;
                                    purchase.RefNo = purchaseRequision.PurchaseRequisitionID.ToString();
                                    purchase.Date = assignment.OnlineApprovedTime;
                                    purchase.PurchaseMethod = "Proforma";
                                    purchase.DurationForComplate = (Math.Round((date - (DateTime)assignment.OnlineApprovedTime).TotalDays, 2)).ToString();
                                    purchase.BudgetToComplete = poItem.Total;
                                    list.Add(purchase);
                                }

                            }
                        }

                        #endregion

                    }
                    else if ((assignment.ProcurementMethod == ProcurementMethod.Tender) && (purchasetype == "Tender" || purchasetype == "All"))
                    {
                        var lotpurchase = _db.LotPurchaseRequisitions.FirstOrDefault(x => x.PurchaseRequisitionID == assignment.PurchaseRequisitionID);
                        if (lotpurchase != null)
                        {
                            foreach (var item in assignment.PurchaseRequisitionReassignmentItems)
                            {
                                var purchase = new PurchaseReportViewModel();
                                var lot = _db.Lots.FirstOrDefault(x => x.LotID == lotpurchase.LotID);
                                var awardIds = _db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID)
                                    .Select(x => x.BidAwardedSupplierID).ToList();

                                var awardItem = _db.BidAwardedSupplierItems
                                    .Where(x => awardIds.Contains(x.BidAwardedSupplierID))
                                    .FirstOrDefault(x => x.ItmCode == item.ItmCode);


                                if (awardItem != null)
                                {
                                    DateTime date = DateTime.Today;
                                    if (awardItem.ItemType == PurchaseItemType.Stock)
                                    {
                                        #region check stock status

                                        var goodReceive = _db.GoodReceives
                                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.Tender).FirstOrDefault(x => x.ReceiveRef == awardItem.BidAwardedSupplierID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;
                                            var receiveItem = _db.GoodReceiveItems
                                                .Where(x => x.GoodReceiveId == goodReceive.GoodReceiveId)
                                                .FirstOrDefault(x => x.ItemCode == awardItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }

                                            var contract = _db.ContractManagements.FirstOrDefault(x =>
                                                x.ProcurementRefNo == awardItem.BidAwardedSupplierID);

                                            if (contract != null)
                                            {

                                                if (contract.IsTransferredToPayment)
                                                {
                                                    purchase.CurrentStatus = "Sent for Payment";
                                                }
                                            }

                                            else if (lot.Tender.PurchaseStatus== ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }

                                            else if (lot.Tender.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            var contract = _db.ContractManagements.FirstOrDefault(x =>
                                                x.ProcurementRefNo == awardItem.BidAwardedSupplierID);

                                            if (contract != null)
                                            {

                                                if (contract.IsTransferredToPayment)
                                                {
                                                    purchase.CurrentStatus = "Sent for Payment";
                                                }
                                            }
                                            
                                            else if (lot.Tender.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }

                                    else if (awardItem.ItemType == PurchaseItemType.FixedAsset)
                                    {
                                        #region check fixed status

                                        var goodReceive = _db.FixedAssetReceives
                                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.Tender).FirstOrDefault(x => x.ReceiveRef == awardItem.BidAwardedSupplierID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;
                                            var receiveItem = _db.FixedAssetReceiveItems
                                                .Where(x => x.FixedAssetReceiveId == goodReceive.FixedAssetReceiveId)
                                                .Where(x=>x.Name==awardItem.Name)
                                                .FirstOrDefault(x => x.FixedAssetSubCategoryID.ToString() == awardItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }

                                            var contract = _db.ContractManagements.FirstOrDefault(x =>
                                                x.ProcurementRefNo == awardItem.BidAwardedSupplierID);

                                            if (contract != null)
                                            {

                                                if (contract.IsTransferredToPayment)
                                                {
                                                    purchase.CurrentStatus = "Sent for Payment";
                                                }
                                            }

                                            else if (lot.Tender.PurchaseStatus == ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }

                                            else if (lot.Tender.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            var contract = _db.ContractManagements.FirstOrDefault(x =>
                                                x.ProcurementRefNo == awardItem.BidAwardedSupplierID);

                                            if (contract != null)
                                            {

                                                if (contract.IsTransferredToPayment)
                                                {
                                                    purchase.CurrentStatus = "Sent for Payment";
                                                }
                                            }

                                            else if (lot.Tender.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }

                                    else if (awardItem.ItemType == PurchaseItemType.Service)
                                    {
                                        #region check service status

                                        var goodReceive = _db.ServiceReceives
                                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.Tender).FirstOrDefault(x => x.ReceiveRef == awardItem.BidAwardedSupplierID.ToString());
                                        if (goodReceive != null)
                                        {
                                            date = goodReceive.ReceiveDate;
                                            var receiveItem = _db.ServiceReceiveItems
                                                .Where(x => x.ServiceReceiveId == goodReceive.ServiceReceiveId)
                                                .FirstOrDefault(x => x.ServiceItemCode== awardItem.ItmCode);
                                            if (receiveItem != null)
                                            {
                                                purchase.CurrentStatus = "Received";
                                            }

                                            var contract = _db.ContractManagements.FirstOrDefault(x =>
                                                x.ProcurementRefNo == awardItem.BidAwardedSupplierID);

                                            if (contract != null)
                                            {

                                                if (contract.IsTransferredToPayment)
                                                {
                                                    purchase.CurrentStatus = "Sent for Payment";
                                                }
                                            }

                                            else if (lot.Tender.PurchaseStatus == ExceedERP.Core.Domain.Procurement.Settings.PurchaseStatus.Cancelled)
                                            {
                                                purchase.CurrentStatus = "Cancelled";
                                            }

                                            else if (lot.Tender.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }
                                        else
                                        {
                                            var contract = _db.ContractManagements.FirstOrDefault(x =>
                                                x.ProcurementRefNo == awardItem.BidAwardedSupplierID);

                                            if (contract != null)
                                            {

                                                if (contract.IsTransferredToPayment)
                                                {
                                                    purchase.CurrentStatus = "Sent for Payment";
                                                }
                                            }

                                            else if (lot.Tender.IsOnlineApproved)
                                            {
                                                purchase.CurrentStatus = "Approved";
                                            }
                                            else
                                            {
                                                purchase.CurrentStatus = "New";
                                            }
                                        }

                                        #endregion
                                    }

                                    purchase.ItemCategory =awardItem.CatCode + "-" + awardItem.CategoryName ;
                                    purchase.ItemName = awardItem.ItmCode +"-" + awardItem.ItemName;
                                    purchase.RefNo = purchaseRequision.PurchaseRequisitionID.ToString();
                                    purchase.Date = assignment.OnlineApprovedTime;
                                    purchase.PurchaseMethod = "Tender";
                                    purchase.DurationForComplate =(Math.Round((date - (DateTime)assignment.OnlineApprovedTime)
                                            .TotalDays, 2))
                                        .ToString();
                                    purchase.BudgetToComplete = awardItem.TPrice;
                                    list.Add(purchase);
                                }

                               
                            }
                        }
                    }

                }
            }

            if (!String.IsNullOrWhiteSpace(status))
            {
                list = list.Where(x=>x.CurrentStatus== status).ToList();
            }

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> PurchaseStatus()
        {

            List<string> list = new List<string> { "Received", "Sent for Payment", "Approved", "Cancelled", "New" };
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> PurchaseTypes()
        {

            List<string> list = new List<string> { "Direct Purchase", "Proforma", "Tender" };
            return list;
        }
    }
}
