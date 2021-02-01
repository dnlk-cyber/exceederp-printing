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
    public class GoodSalesDonateOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public GoodSalesOrDonateHeader GetGoodSalesDonate(int id)
        {
            var receive = _db.GoodSellingAndLendings.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            
            GoodSalesOrDonateHeader headerDs = new GoodSalesOrDonateHeader();
            if (receive != null)
            {
               
                headerDs.Id = InventoryPrintHelper.GetId(receive.GoodSellingAndLendingID);
                headerDs.Type = receive.SellOrLend.ToString();
                headerDs.SaleDate = receive.Date.ToShortDateString();
                headerDs.OrganizationName = receive.Buyer;
                headerDs.OrganizationAddress = receive.BuyerAddress;
                headerDs.Reference = receive.Remark;
                headerDs.StoreName = InventoryPrintHelper.StoreName(receive.StoreCode);
                headerDs.Approver = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(receive.OnlineApprovedBy));
                headerDs.ApprovedDate = receive.OnlineApprovedTime.GetValueOrDefault().Date.ToShortDateString();
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(receive.CreatedBy));
            }

            headerDs.NoOfCopy = InventoryHelper.GetDocumentCopy(PrintCopyAttributes.GOOD_SALES_DONATION);
            headerDs.CopyMembers = InventoryHelper.GetDocumentCopyList(PrintCopyAttributes.GOOD_SALES_DONATION);

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
        public List<GoodSellingAndLendingItem> GoodSalesOrDonateItem(int id)
        {
            List<GoodSellingAndLendingItem> goodReceiveItems = _db.GoodSellingAndLendingItems.Where(x => x.GoodSellingAndLendingID == id).ToList();
            return goodReceiveItems;
        }

        
    }
}
