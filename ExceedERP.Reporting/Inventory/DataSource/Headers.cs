using System;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class InventoryReportHeaderDs : StockCardHeaderVm
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public StockCardHeaderVm GetStockCardHeader(string itemcode)
        {
            var item = db.InventoryItems.Find(itemcode);
            StockCardHeaderVm headerDs = new StockCardHeaderVm();
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (item != null)
            {
                headerDs.ItemName = item.Name;
                headerDs.AccountCode = item.ItemCategory.AccountCode;
                headerDs.Unit = item.UoMName;
                headerDs.PartNo = item.PartNo;
            }

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
        public BinCardHeader GetItemDetailBin(string itemcode, string storeCode)
        {
            var item = db.InventoryItems.Find(itemcode);
            BinCardHeader headerDs = new BinCardHeader();
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            var storeItemAssignment =db.StoreItemAssignments.FirstOrDefault(i => i.StoreCode == storeCode && i.ItemCode == itemcode);
            if (item != null)
            {
                headerDs.ItemName = item.Name;
                headerDs.Model = "Model";
                headerDs.ShelfNo = "Unknown";

                headerDs.PartNo = item.PartNo;
                headerDs.UnitOfIssue = item.UoMName;
                headerDs.InterChargeable = "Unknown";
                if (storeItemAssignment != null)
                {
                    headerDs.MinBalance = storeItemAssignment.MinPoint.ToString();
                    headerDs.Reorder = storeItemAssignment.ReOrderPoint.ToString();
                }

            }
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
        public CountSheetHeader GetInventoryCount(string storeCode)
        {
            CountSheetHeader header = new CountSheetHeader();
           
             
            header.AccountYear = DateTime.Now.Year.ToString();
            header.StoreCode = storeCode;
            return header;

        }
    }
}
