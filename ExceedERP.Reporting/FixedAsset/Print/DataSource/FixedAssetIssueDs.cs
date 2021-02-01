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
    public class FixedAssetIssueDs
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetIssueHeader GetIssue(int id)
        {
            var issue = db.FixedAssetIssues.Find(id);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            FixedAssetIssueHeader headerDs = new FixedAssetIssueHeader();
            if (issue != null)
            {
                headerDs.Reference = issue.Reference;
                headerDs.IssueNumber = InventoryPrintHelper.GetId(issue.FixedAssetIssueID);
                headerDs.StoreCode = issue.StoreCode+"-"+InventoryPrintHelper.StoreName(issue.StoreCode);
                headerDs.IssueDate = issue.IssueDate.ToShortDateString();
               
                headerDs.Recipient = EmployeeHelper.GetEmployeeEnglishName(issue.RecipientId);
                headerDs.StoreKeeperName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(issue.CreatedBy));
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(issue.CreatedBy));
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(issue.OnlineApprovedBy));
                headerDs.Project = InventoryPrintHelper.BranchName(issue.BranchId);
                headerDs.Department = InventoryPrintHelper.WorkUnitName(issue.OrgStructureId);
                headerDs.Datecreated = issue.DateCreated.GetValueOrDefault().ToShortDateString();
                headerDs.IssueTo = issue.FixedAssetRequisition.OrgStructureId.ToString();
                headerDs.GatePass = issue.GatePass;
                headerDs.StoreRequistionNo = InventoryPrintHelper.GetId(issue.FixedAssetRequisitionId);
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
        public List<FixedAssetIssueItemVm> IssueItems(int id)
        {
            List<FixedAssetIssueItemVm> list = new List<FixedAssetIssueItemVm>();

            var assetIssueItems = db.FixedAssetIssueItems.Where(x => x.FixedAssetIssueID == id).ToList();
            foreach (var item in assetIssueItems)
            {
                var asset = db.AssetRegistrations.Find(item.AssetRegistrationId);
                if (asset != null)
                {
                    var issueitem = new FixedAssetIssueItemVm
                    {
                        Category = asset.FixedAssetCategory.CategoryName,
                        SubCategory = asset.FixedAssetSubCategory.SubCategoryName,
                        TagNo = asset.Tag,
                        SerialNo = asset.SerialNo,
                        EngineNo = asset.EngineNo,
                        ChassisNo = asset.ChassisNo,
                        Manufacturer = asset.Manufacturer,
                        Condition = item.Condition,
                        Unit = item.Unit,
                        UnitPrice = item.UnitPrice,
                        Total = item.Total,
                        Model = item.Model,
                        Name=item.Name,
                        GrnNo = item.GrnNo,
                        Size=item.Size,
                        PlateNo = asset.PlateNo,
                        ApprovedQuantity = item.ApprovedQuantity,
                        IssuedQuantity = item.IssuedQuantity,
                        Remark=item.Remark

                    };

                    list.Add(issueitem);
                }
            }
           return list;
        }
    }
}