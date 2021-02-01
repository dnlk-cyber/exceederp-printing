using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    [DataObject]
    public class ItemTransferDs
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ItemTransferHeader GetItemtrasferHeader(string id)
        {
            int did = int.Parse(id);
            Core.Domain.Inventory.ItemTransfer itemTransfer = db.ItemTransfers.Find(did);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            
            ItemTransferHeader headerDs = new ItemTransferHeader();
            if (itemTransfer != null)
            {
                headerDs.TransferNumber = InventoryPrintHelper.GetId(itemTransfer.ItemTransferID);
                headerDs.TransferDate = itemTransfer.TransferDate.ToShortDateString();
                headerDs.Requestor = EmployeeHelper.GetEmployeeEnglishName(itemTransfer.EmployeeId);
                headerDs.ToStore  = InventoryPrintHelper.StoreName(itemTransfer.ToStore);
                headerDs.FromStore = InventoryPrintHelper.StoreName(itemTransfer.FromStore);
                headerDs.ToProject = InventoryPrintHelper.BranchName(InventoryHelper.GetStoreBranch(itemTransfer.ToStore));
                headerDs.FromProject = InventoryPrintHelper.BranchName(InventoryHelper.GetStoreBranch(itemTransfer.FromStore));
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(itemTransfer.CreatedBy));
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(itemTransfer.OnlineApprovedBy));
                if (itemTransfer.OnlineApprovedTime != null)
                    headerDs.ApprovedDate = (DateTime) itemTransfer.OnlineApprovedTime;
                if (itemTransfer.DateCreated != null) headerDs.DateCreated = (DateTime) itemTransfer.DateCreated;

                if(itemTransfer.DriverType == Driver.Internal)
                {
                    if (!String.IsNullOrEmpty(itemTransfer.DeliveredBy))
                        headerDs.Deliverer=EmployeeHelper.GetEmployeeEnglishName(int.Parse(itemTransfer.DeliveredBy));
                }
                else
                {
                    headerDs.Deliverer = itemTransfer.SupplierName;
                }
                int requestNo = itemTransfer.StoreRequisitionID;
                headerDs.RequestionNo = InventoryPrintHelper.GetId(requestNo);
                headerDs.DocumentRef = itemTransfer.DocumentRef;
                headerDs.PlateNo = itemTransfer.PlateNo;
                headerDs.SentDate = itemTransfer.TransferDate.ToShortDateString();
                
            }

            headerDs.NoOfCopy = InventoryHelper.GetDocumentCopy(PrintCopyAttributes.STORE_TRANSFER_SEND);
            headerDs.CopyMembers = InventoryHelper.GetDocumentCopyList(PrintCopyAttributes.STORE_TRANSFER_SEND);

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
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
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                              a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ItemTransferViewModel> TransferItems(string id)
        {
            int did = int.Parse(id);
            List<ItemTransferViewModel> list = new List<ItemTransferViewModel>();

            var itemTransferItems = db.ItemTransferItems.Where(x => x.ItemTransferID == did).ToList();
            foreach (var itemTransferItem in itemTransferItems)
            {
                var receivedList = db.ItemTransferItemReceive
                    .Where(x => x.ItemTransferReceive.ItemTransferID == itemTransferItem.ItemTransferID)
                    .Where(x => x.ItemTransferReceive.IsOnlineApproved)
                    .Where(x => x.ItemTransferReceive.IsVoid == false)
                    .Where(x => x.ItemCode == itemTransferItem.ItemCode)
                    .ToList();

                var entity = new ItemTransferViewModel
                {
                    Id = id,
                    Reference = itemTransferItem.ItemTransfer.DocumentRef,
                    InventoryItem = itemTransferItem.InventoryItem.ItemCode+" "+itemTransferItem.InventoryItem.Name,
                    Unit = itemTransferItem.InventoryItem.UoMName,
                    RequestedQuantity = itemTransferItem.RequestedQuantity,
                    ShippedQuantity = itemTransferItem.ShippedQuantity,
                    UPrice = itemTransferItem.UPrice,
                    TPrice = itemTransferItem.TPrice,
                    ReceivedQuantity = receivedList.Sum(x => x.ReceivedQuantity)

                };

                list.Add(entity);
            }
            return list;
        }
    }
}