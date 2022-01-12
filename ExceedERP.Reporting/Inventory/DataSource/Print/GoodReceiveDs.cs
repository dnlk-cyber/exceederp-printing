using System;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.SharedDataSources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    [DataObject]
    public class GoodReceiveDs
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public GoodReceiveHeader GetGoodReceive(int id)
        {
            var goodReceive = _db.GoodReceives.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            //var inspection = _db.ItemInspections.Where(x => x.GoodReceiveId == id).ToList();
            GoodReceiveHeader headerDs = new GoodReceiveHeader();
            if (goodReceive != null)
            {
                headerDs.DocTitle = goodReceive.IsFreshBazaar ? "FRESH BAZZAR MEMO" : "GOODS RECEIVING NOTE";
                headerDs.GoodReciveNo = InventoryPrintHelper.GetId(goodReceive.GoodReceiveId);
                headerDs.ReceiveDate = goodReceive.ReceiveDate.Date.ToShortDateString();
                headerDs.Supplier = goodReceive.SupplierName;
                headerDs.SupplierInvoice = goodReceive.SupplierInvoiceNo;
                headerDs.Reference = goodReceive.IsManual ? goodReceive.Reference : goodReceive.ReceiveRef;
                headerDs.StoreName = InventoryPrintHelper.StoreName(goodReceive.StoreCode);
                headerDs.Approver = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(goodReceive.OnlineApprovedBy));
                headerDs.ApprovedDate = goodReceive.OnlineApprovedTime.GetValueOrDefault().Date.ToShortDateString();
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(goodReceive.CreatedBy));

                if (!goodReceive.IsManual)
                {
                    #region tender and po reference

                    var type = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
                    if (type != null)
                    {
                        switch (type.Type)
                        {
                            case GoodReceiveType.PurchaseOrder:
                                int pid = int.Parse(goodReceive.ReceiveRef);
                                var purchaseOrder = _db.PurchaseOrders.Find(pid);
                                if (purchaseOrder != null)
                                {
                                    headerDs.PurchaseOrderNo = purchaseOrder.PurchaseOrderID.ToString();
                                }
                                break;
                            case GoodReceiveType.Tender:
                                int awardid = int.Parse(goodReceive.ReceiveRef);
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
            headerDs.NoOfCopy = InventoryHelper.GetDocumentCopy(PrintCopyAttributes.GOOD_RECEIVE_NOTE);
            headerDs.CopyMembers = InventoryHelper.GetDocumentCopyList(PrintCopyAttributes.GOOD_RECEIVE_NOTE);


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
        public List<GoodReceiveItem> GoodReceiveItem(int id)
        {
            List<GoodReceiveItem> goodReceiveItems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == id).ToList();
            if (InventoryConfigurationHelper.IsHidePriceOnPrintEnabled())
            {
                foreach(var item in goodReceiveItems)
                {
                    item.UnitPrice = 0;
                    item.TaxAmount = 0;
                    item.SubTotal = 0;
                    item.Total = 0;
                }
            }
            return goodReceiveItems;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InspectorVm> GoodReceiveInspection(int id)
        {
            List<InspectorVm> list=new List<InspectorVm>();
           var inspectors= _db.ItemInspections.Where(x => x.GoodReceiveId == id).ToList();
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
    }
}
