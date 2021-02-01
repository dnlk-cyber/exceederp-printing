using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class InventoryCountSheetOds : List<ItemVm>
    {
        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public InventoryCountSheetHeader GetInventoryCountSheetHeader(string store)
        {
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            InventoryCountSheetHeader headerDs = new InventoryCountSheetHeader();
            if (!String.IsNullOrEmpty(store))
            {
                var storeObj = db.InventoryStores.Find(store);
                if (storeObj != null)
                {
                    headerDs.StoreCode = storeObj.StoreCode;
                    headerDs.StoreName = storeObj.Name;
                    headerDs.StoreFullName = storeObj.StoreCode+"-"+storeObj.Name;
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
        //private List<StoreItemAssignment> _storeItemAssignments;

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<ItemVm> GetItemsInStore(string store,string category)
        {
            List<ItemVm> itemIncategory = new List<ItemVm>();
            if (!String.IsNullOrEmpty(store))
            {
                var storeItemAssignments = db.StoreItemAssignments.Where(it => it.StoreCode == store).AsQueryable();

                if (!String.IsNullOrEmpty(category))
                {
                    storeItemAssignments =
                        storeItemAssignments.Where(x => x.ItemCategoryCode == category).AsQueryable();
                }

                foreach (var items in storeItemAssignments)
                {
                    var item = new ItemVm
                    {
                        ItemCode = items.InventoryItem.ItemCode.Trim(),
                        Description = items.InventoryItem.Name,
                        Category = items.ItemCategory.Name,
                        Unit = items.InventoryItem.UoMName,
                        Location = items.ItemLocation
                    };

                    itemIncategory.Add(item);
                }
            }
            return itemIncategory.OrderBy(x=>x.ItemCode);
        }

    }
}
