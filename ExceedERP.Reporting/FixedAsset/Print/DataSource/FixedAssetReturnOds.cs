
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.Print.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.Print.DataSource
{
    [DataObject]
    public class FixedAssetReturnOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetReturnHeader GetReturn(int id)
        {
            var assetReturn = db.FixedAssetReturns.Find(id);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            FixedAssetReturnHeader headerDs = new FixedAssetReturnHeader();
            if (assetReturn != null)
            {
                headerDs.Reference = assetReturn.Reference;
                headerDs.ReturnNo = InventoryPrintHelper.GetId(assetReturn.FixedAssetReturnId);
                headerDs.StoreCode = assetReturn.StoreCode+"-"+InventoryPrintHelper.StoreName(assetReturn.StoreCode);
                headerDs.ReturnDate = assetReturn.ReturnDate.ToShortDateString();
               
              
                headerDs.StoreKeeperName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetReturn.CreatedBy));
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetReturn.CreatedBy));
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetReturn.OnlineApprovedBy));
                headerDs.BranchOrProject = InventoryPrintHelper.BranchName(assetReturn.BranchId);
                headerDs.Department = InventoryPrintHelper.WorkUnitName(assetReturn.OrgStructureId);
                headerDs.Datecreated = assetReturn.DateCreated.GetValueOrDefault().ToShortDateString();
                headerDs.ReturnedBy = EmployeeHelper.GetEmployeeEnglishName(assetReturn.EmployeeId);


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
        public List<FixedAssetReturnItemVm> ReturnItems(int id)
        {
            List<FixedAssetReturnItemVm> list = new List<FixedAssetReturnItemVm>();

            var assetReturnItems = db.FixedAssetReturnItems.Where(x => x.FixedAssetReturnId == id).ToList();
            foreach (var item in assetReturnItems)
            {
                var asset = db.AssetRegistrations.Find(item.AssetRegistrationId);
                if (asset != null)
                {
                    var issueitem = new FixedAssetReturnItemVm
                    {
                        Category = asset.FixedAssetCategory.CategoryName,
                        SubCategory = asset.FixedAssetSubCategory.SubCategoryName,
                        TagNo = asset.Tag,
                        SerialNo = asset.SerialNo,
                        EngineNo = asset.EngineNo,
                        ChassisNo = asset.ChassisNo,
                        Manufacturer = asset.Manufacturer,
                        Condition =asset.Condition.Name,
                        Unit = asset.Unit,
                      
                        Model = asset.ModelNo,
                        Name = asset.Name,
                        GrnNo = asset.GRN,
                        Size = asset.Size,
                        PlateNo = asset.PlateNo,
                        UnitPrice = item.UnitPrice,
                        Total = item.Total,
                        Quantity = item.Quantity

                    };

                    list.Add(issueitem);
                }
            }
           return list;
        }
    }
}