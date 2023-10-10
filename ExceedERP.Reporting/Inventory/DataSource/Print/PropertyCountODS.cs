using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    [DataObject]
    public class PropertyCountODs
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PropertyCountHeaderViewModel PropertyCountHeader(int id)
        {
            var count = _db.PropertyCounts.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            PropertyCountHeaderViewModel headerDs = new PropertyCountHeaderViewModel();

            if (count != null)
            {
                headerDs.VoucherId = count.PropertyCountId.ToString();
                var store = _db.InventoryStores.Find(count.StoreCode);
                if (store != null)
                    headerDs.Store=count.StoreCode+"-"+store.Name;
                var branch = _db.Branch.Find(count.BranchId);
                if (branch != null) headerDs.Branch = branch.Name;
                headerDs.CountDate = count.CountDate;
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
        public IOrderedEnumerable<PropertyCountLinesViewModel> PropertyCountLines(int id)
        {
            List<PropertyCountLinesViewModel> lines = new List<PropertyCountLinesViewModel>();
            var countedItems = _db.PropertyCountItems.Where(x => x.PropertyCountId == id).ToList();
            foreach (var line in countedItems)
            {
                PropertyCountLinesViewModel record = new PropertyCountLinesViewModel
                {
                    ItemCategoryCode = line.ItemCategory.Name,
                    ItemCode = line.ItemCode ,
                    ItemName =line.InventoryItem.Name,
                    UOM = line.InventoryItem.UoMName,
                    PhysicalCount = line.Count,
                    Price=line.Price,
                    Total=line.Count*line.Price,
                    Status = line.Status
                };

                lines.Add(record);

            }
            return lines.OrderBy(x=>x.ItemCode);
        }
    }
}
