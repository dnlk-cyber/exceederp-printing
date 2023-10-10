using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{
    [DataObject]
    public class FixedAssetReportByCategoryOds : List<FixedAssetReportHeader>
    {
        ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetReportHeader GetFixedAssetReportHeader(string category, string subcategory, string project)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            FixedAssetReportHeader headerDs = new FixedAssetReportHeader();

            if (!String.IsNullOrEmpty(category))
            {
               
                var assetCategory = _db.FixedAssetCategories.FirstOrDefault(x => x.CategoryCode == category);
                if (assetCategory != null) headerDs.Category = assetCategory.CategoryName;
            }

            if (!String.IsNullOrEmpty(subcategory))
            {
                int id = int.Parse(subcategory);
                var assetSubCategory = _db.FixedAssetSubCategories.FirstOrDefault(x => x.FixedAssetSubCategoryID == id);
                if (assetSubCategory != null)
                    headerDs.SubCategory = assetSubCategory.SubCategoryName;
            }

            if (!String.IsNullOrEmpty(project))
            {
                int id = int.Parse(project);
                var branch = _db.Branch.FirstOrDefault(x => x.BranchId == id);
                if (branch != null)
                    headerDs.ProjectOrBranch = branch.Name;
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
        public IEnumerable<FixedAssetItemReport> GetFixedAssetItems(DateTime from, DateTime to, string category, string subcategory, string project, string workunit)
        {
            var assetRegistrations = _db.AssetRegistrations
                .Where(x => x.GRNDate >= from)
                .Where(x => x.GRNDate <= to).AsQueryable();

            List<FixedAssetItemReport> assetCountItem = new List<FixedAssetItemReport>();

            if (!String.IsNullOrEmpty(category))
            {
                assetRegistrations = assetRegistrations
                    .Where(x => x.FixedAssetCategory.CategoryCode == category).AsQueryable();
            }

            if (!String.IsNullOrEmpty(subcategory))
            {
                int id = int.Parse(subcategory);
                assetRegistrations = assetRegistrations.Where(x => x.FixedAssetSubCategoryID == id).AsQueryable();
            }
            if (!String.IsNullOrEmpty(project))
            {
                    
                assetRegistrations = assetRegistrations.Where(x => x.BranchId.ToString() == project).AsQueryable();
            }

          
            if (!String.IsNullOrEmpty(workunit))
            {
                int did = int.Parse(workunit);
                List<string> employeeIds = EmployeeHelper.GetEmployeesByWorkUnit(did).Select(x => x.EmployeeId.ToString()).ToList();
                assetRegistrations = assetRegistrations.Where(x => employeeIds.Contains(x.EmployeeId)).AsQueryable();

            }

            foreach (var asset in assetRegistrations)
                {
                    #region  filter
                    //if (!String.IsNullOrEmpty(project))
                    //{
                    //    int branchid = int.Parse(project);
                    //    var location = _db.LocationTrackings
                    //        .Where(x=>x.BranchId==branchid)
                    //        .Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                    //        .FirstOrDefault(x => x.IsCurrent);
                    //    if (location == null) //jump if not in same project
                    //        continue;

                    //}
                   
                    #endregion
                    var item = new FixedAssetItemReport
                    {
                        Tag = asset.Tag,
                        BarCode = asset.BarCodeNo,
                        Category = asset.FixedAssetCategory.CategoryName,
                        SubCategory = asset.FixedAssetSubCategory.SubCategoryName,
                        Name = asset.Name,
                        BookValue = asset.BookValue,
                        PurchasePrice = asset.PurchasePrice,
                        Size = asset.Size,
                        Location = asset.Location,
                        SivDate = "",
                        SivNo = asset.SIV,
                        GrnNo = asset.GRN,
                        GrnDate = asset.GRNDate.ToString(),
                        SerialNo = asset.SerialNo,
                        ChasisNo = asset.ChassisNo,
                        EngineNo = asset.EngineNo,
                        Model = asset.ModelNo,
                        PlateNo = asset.PlateNo,
                        Condition = asset.Condition.Name,
                        Supplier = asset.Supplier,
                        Manufacturer = asset.Manufacturer,
                        ManufacturingYear = asset.ManufacturDate.ToString(),
                        Quantity = asset.Quanity,
                        TotalPrice = asset.Quanity* asset.PurchasePrice,
                        Remark = "",
                        TotalDepreciationValue = asset.PurchasePrice - asset.BookValue,

                    };
                    var branch = _db.Branch.Find(asset.BranchId);

                    item.Location = $"{branch?.Code}-{branch?.Name}";

                    var usercard = _db.UserCardItems.Where(x=>x.Active).FirstOrDefault(x => x.AssetRegistrationId == asset.AssetRegistrationId);

                    if (usercard != null) item.CurrentUser = EmployeeHelper.GetEmployeeEnglishName(usercard.EmployeeId);

                    //var locationTracking = _db.LocationTrackings
                    //    .Where(x => x.AssetRegistrationId == asset.AssetRegistrationId)
                    //    .FirstOrDefault(x => x.IsCurrent);
                    //if (locationTracking != null)
                    //{
                    //    item.Location = _db.Branch.Find(locationTracking.BranchId)?.Name;
                    //    item.Branch = _db.Branch.Find(locationTracking.BranchId)?.Name;
                    //}


                    assetCountItem.Add(item);
                }
            
            return assetCountItem.OrderBy(x=>x.Tag);
        }

    }
}
