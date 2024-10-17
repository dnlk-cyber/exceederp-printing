using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System.Data.Entity;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{
   
    [DataObject]
    public class AssetStatsByBranchAndBizUnitOds
    {
        ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetCountSheetHeader GetFixedAssetHeader()
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            FixedAssetCountSheetHeader headerDs = new FixedAssetCountSheetHeader();

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
        public List<AssetStatsByBranchAndBizUnitVM> GetAssetStatsByBranchAndBizUnit()
        {
            // Step 1: Retrieve all necessary data with a single query
            var assetData = _db.AssetRegistrations
                .Include(x => x.FixedAssetCategory)
                .Include(x => x.FixedAssetSubCategory)
                .ToList();

            // Step 2: Retrieve business units and branches separately
            var bizUnits = _db.BusinessUnits.ToDictionary(b => b.BusinessUnitId, b => b.Name);
            var branches = _db.Branch.ToDictionary(br => br.BranchId, br => br.Name);

            // Step 3: Process the data in-memory
            var groupedByCategory = assetData
                .GroupBy(x => new
                {
                    x.FixedAssetCategoryID,
                    CategoryName = x.FixedAssetCategory.CategoryName
                })
                .ToList();

            var list = new List<AssetStatsByBranchAndBizUnitVM>();

            foreach (var categoryGroup in groupedByCategory)
            {
                var groupedBySubCategory = categoryGroup
                    .GroupBy(x => new
                    {
                        x.FixedAssetCategoryID,
                        x.FixedAssetSubCategoryID,
                        SubCategoryName = x.FixedAssetSubCategory.SubCategoryName
                    })
                    .ToList();

                foreach (var subCategoryGroup in groupedBySubCategory)
                {
                    var detailedRecords = subCategoryGroup
                        .GroupBy(x => new
                        {
                            x.BusinessUnitId,
                            x.BranchId,
                            x.FixedAssetCategoryID,
                            x.FixedAssetSubCategoryID
                        })
                        .Select(g => new AssetStatsByBranchAndBizUnitVM
                        {
                            BizUnit = bizUnits.ContainsKey(g.Key.BusinessUnitId) ? bizUnits[g.Key.BusinessUnitId] : "Unknown",
                            Branch = branches.ContainsKey(g.Key.BranchId) ? branches[g.Key.BranchId] : "Unknown",
                            Category = categoryGroup.Key.CategoryName,
                            SubCategory = subCategoryGroup.Key.SubCategoryName,
                            Qty = g.Sum(x => x.Quanity)
                        })
                        .ToList();

                    list.AddRange(detailedRecords);

                    // Add total for each subcategory
                    var subCategoryTotalQty = detailedRecords.Sum(x => x.Qty);
                    list.Add(new AssetStatsByBranchAndBizUnitVM
                    {
                        BizUnit = "Total",
                        Branch = "",
                        Category = categoryGroup.Key.CategoryName,
                        SubCategory = subCategoryGroup.Key.SubCategoryName + " Total",
                        Qty = subCategoryTotalQty,
                        Highlight=true
                    });
                }

                // Add total for each category
                var categoryTotalQty = categoryGroup.Sum(x => x.Quanity);
                list.Add(new AssetStatsByBranchAndBizUnitVM
                {
                    BizUnit = "Total",
                    Branch = "",
                    Category = categoryGroup.Key.CategoryName,
                    SubCategory = "Category Total",
                    Qty = categoryTotalQty,
                    Highlight = true
                });
            }

            return list;



        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AssetStatsByBranchAndBizUnitVM> GetAssetStatsByBranchAndBizUnit1()
        {

            List<AssetStatsByBranchAndBizUnitVM> list = new List<AssetStatsByBranchAndBizUnitVM>();
            var bizUnits = _db.BusinessUnits.ToList();
            var branchs = _db.Branch.ToList();
            var cats = _db.FixedAssetCategories.ToList();
            var subCats = _db.FixedAssetSubCategories.ToList();

            foreach (var b in bizUnits)
            {
                foreach (var br in branchs)
                {
                    foreach (var c in cats)
                    {
                        foreach (var s in subCats)
                        {
                            var count = _db.AssetRegistrations
                                .Where(x => x.BusinessUnitId == b.BusinessUnitId)
                                .Where(x => x.BranchId == br.BranchId)
                                .Where(x => x.FixedAssetCategoryID == c.FixedAssetCategoryID)
                                .Where(x => x.FixedAssetSubCategoryID == s.FixedAssetSubCategoryID).AsQueryable();

                            if (count.Any())
                            {
                                var record = new AssetStatsByBranchAndBizUnitVM
                                {
                                    BizUnit = b.Name,
                                    Branch = br.Name,
                                    Category = c.CategoryName,
                                    SubCategory = s.SubCategoryName,
                                    Qty = count.Sum(x => x.Quanity)
                                };
                                list.Add(record);
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}
