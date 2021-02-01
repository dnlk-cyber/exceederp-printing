using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.Print.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.Print.DataSource
{
    [DataObject]
    public class FixedAssetReceiveDs
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetReceiveHeader GetGoodReceive(int id)
        {
            var assetReceive = _db.FixedAssetReceives.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
           
            //var inspection = db.ItemInspections.Where(x => x.GoodReceiveId == id).ToList();
            FixedAssetReceiveHeader headerDs = new FixedAssetReceiveHeader();
            if (assetReceive != null)
            {
                headerDs.GoodReciveNo = InventoryPrintHelper.GetId(assetReceive.FixedAssetReceiveId);
                headerDs.ReceiveDate = assetReceive.ReceiveDate.ToShortDateString();
                headerDs.Supplier = assetReceive.SupplierName;
                headerDs.Reference = assetReceive.Reference;

                if (!string.IsNullOrEmpty(assetReceive.ReceiveRef))
                {
                    int refId = int.Parse(assetReceive.ReceiveRef);
                    headerDs.PurchaseOrderNo = InventoryPrintHelper.GetId(refId);
                }

                headerDs.StoreName = InventoryPrintHelper.StoreName(assetReceive.StoreCode);
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetReceive.CreatedBy));
                headerDs.CreatedDate = assetReceive.DateCreated.ToString();

                headerDs.ApprovedDate = assetReceive.OnlineApprovedTime.GetValueOrDefault().Date.ToShortDateString();
                headerDs.Receiver = InventoryPrintHelper.GetStoreKeeper(assetReceive.StoreCode);
                headerDs.Approver = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetReceive.OnlineApprovedBy)); ;
                headerDs.SupplierInvoice = assetReceive.SupplierInvoiceNo;

                if (!assetReceive.IsManual)
                {
                    #region tender and po reference

                    var type = _db.ReceiveTypes.Find(assetReceive.ReceiveTypeId);
                    if (type != null)
                    {
                        switch (type.Type)
                        {
                            case GoodReceiveType.PurchaseOrder:
                                int pid = int.Parse(assetReceive.ReceiveRef);
                                var purchaseOrder = _db.PurchaseOrders.Find(pid);
                                if (purchaseOrder != null)
                                {
                                    headerDs.PurchaseOrderNo = purchaseOrder.PurchaseOrderID.ToString();
                                }
                                break;
                            case GoodReceiveType.Tender:
                                int awardid = int.Parse(assetReceive.ReceiveRef);
                                var award = _db.BidAwardedSuppliers.Find(awardid);
                                if (award != null)
                                {
                                    var lot = _db.Lots.Find(award.LotID);
                                    if (lot != null)
                                        headerDs.TenderNo = lot.Tender.TenderReference;
                                }
                                break;
                        }
                    }

                    #endregion

                }
            }
            
            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FixedAssetReceiveItem> GoodReceiveItem(int id)
        {
            List<FixedAssetReceiveItem> assetReceiveItems = _db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveId == id).ToList();
            return assetReceiveItems;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InspectorVm> GoodReceiveInspection(int id)
        {
            List<InspectorVm> list = new List<InspectorVm>();
            var inspectors = _db.FixedAssetInspections.Where(x => x.FixedAssetReceiveId == id).ToList();
            foreach (var inspector in inspectors)
            {
                var inspect = new InspectorVm
                {
                    Accepted = inspector.Accepted,
                    Remark = inspector.Remark,
                    InspectionDate = inspector.InspectionDate.Date,
                    Employeename = EmployeeHelper.GetEmployeeEnglishName(inspector.EmployeeId)
                };
                list.Add(inspect);
            }
            return list;
        }

        public class PrNo
        {
            public int PrId { get; set; }
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrNo> PrList(int id)
        {
            List<PrNo> prList = new List<PrNo>();

            var fixedAssetReceive = _db.FixedAssetReceives.Find(id);
            if (fixedAssetReceive != null)
            {
                var type = _db.ReceiveTypes.Find(fixedAssetReceive.ReceiveTypeId);
                if (type != null)
                {
                    switch (type.Type)
                    {
                        case GoodReceiveType.PurchaseOrder:
                            int pid = int.Parse(fixedAssetReceive.ReceiveRef);
                            var purchaseOrder = _db.PurchaseOrders.Find(pid);
                            if (purchaseOrder != null && purchaseOrder.IsManual)
                            {
                                var poPrList = _db.PoPurchaseRequisitions.Where(x => x.PurchaseOrderID == id).ToList();
                                foreach (var poPurchaseRequisition in poPrList)
                                {
                                    var prno = new PrNo { PrId = poPurchaseRequisition.PurchaseRequisitionID };
                                    prList.Add(prno);
                                }

                                var prnoh = new PrNo { PrId = (int)purchaseOrder.PurchaseRequisitionID };
                                prList.Add(prnoh);
                            }
                            else if (purchaseOrder != null && !purchaseOrder.IsManual)
                            {
                                var poPrList = _db.ProformaPurchaseRequisitions.Where(x => x.ProformaPurchaseID == purchaseOrder.ProformaPurchaseID).ToList();
                                foreach (var poPurchaseRequisition in poPrList)
                                {
                                    var prno = new PrNo { PrId = poPurchaseRequisition.PurchaseRequisitionID };
                                    prList.Add(prno);
                                }
                            }
                            break;
                        case GoodReceiveType.Tender:
                            int awardid = int.Parse(fixedAssetReceive.ReceiveRef);
                            var award = _db.BidAwardedSuppliers.Find(awardid);
                            if (award != null)
                            {
                                var lotPrList = _db.LotPurchaseRequisitions.Where(x => x.LotID == award.LotID).ToList();
                                foreach (var poPurchaseRequisition in lotPrList)
                                {
                                    var prno = new PrNo { PrId = poPurchaseRequisition.PurchaseRequisitionID };
                                    prList.Add(prno);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return prList;
        }
    }
}