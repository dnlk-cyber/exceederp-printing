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
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    [DataObject]
    public class ItemTransferReceiveDs
    {
        ExceedDbContext dbContext = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ItemTransferReceiveHeader GetItemTransferReceiveHeader(string id)
        {
            int did = int.Parse(id);
            var transferReceive = dbContext.ItemTransferReceive.Find(did);
            var organization = dbContext.Companies.FirstOrDefault(x => x.Category == "0");
            ItemTransferReceiveHeader headerDs = new ItemTransferReceiveHeader();
            if (transferReceive != null)
            {
                headerDs.ReceiveNumber = InventoryPrintHelper.GetId(transferReceive.ItemTransferReceiveId);
                headerDs.TransferDate = transferReceive.ReceiveDate.ToShortDateString();
                headerDs.ToStore  = InventoryPrintHelper.StoreName(transferReceive.ToStore);
                headerDs.FromStore = InventoryPrintHelper.StoreName(transferReceive.FromStore);
                headerDs.ToProject = InventoryPrintHelper.BranchName(InventoryHelper.GetStoreBranch(transferReceive.ToStore));
                headerDs.FromProject = InventoryPrintHelper.BranchName(InventoryHelper.GetStoreBranch(transferReceive.FromStore));
                headerDs.ReceivedDate = transferReceive.ReceiveDate.ToShortDateString();
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(transferReceive.CreatedBy));
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(transferReceive.OnlineApprovedBy));
                if (transferReceive.OnlineApprovedTime != null)
                    headerDs.ApprovedDate = (DateTime)transferReceive.OnlineApprovedTime;
                if (transferReceive.DateCreated != null) headerDs.DateCreated = (DateTime)transferReceive.DateCreated;
                headerDs.Deliverer = transferReceive.DeliveredBy;
                headerDs.TransferNumber = transferReceive.ItemTransferID.ToString();
                headerDs.DocumentRef = transferReceive.DocumentRef;
                headerDs.PlateNo = transferReceive.PlateNo;
               
               
            }

            headerDs.NoOfCopy = InventoryHelper.GetDocumentCopy(PrintCopyAttributes.STORE_TRANSFER_RECEIVE);
            headerDs.CopyMembers = InventoryHelper.GetDocumentCopyList(PrintCopyAttributes.STORE_TRANSFER_RECEIVE);

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    dbContext.Addresses.Where(
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
                    dbContext.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                              a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ItemTransferItemReceive> TransferReceiveItems(string id)
        {
            int did = int.Parse(id);
            List<ItemTransferItemReceive> itemTransferItems = dbContext.ItemTransferItemReceive.Where(x => x.ItemTransferReceiveId == did).ToList();
            return itemTransferItems;
        }
    }
}