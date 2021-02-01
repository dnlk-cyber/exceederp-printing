using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.DataSource.Count
{
    [DataObject]
    public class FixedAssetCountSheetOds : List<FixedAssetCountSheetHeader>
    {
        ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetCountSheetHeader GetFixedAssetCountSheetHeader(string fiscalyear, string project,string store, string workUnit, string employee)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            FixedAssetCountSheetHeader headerDs = new FixedAssetCountSheetHeader();

            if (!String.IsNullOrEmpty(fiscalyear))
            {
                int id = int.Parse(fiscalyear);
                var glFiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == id);
                if (glFiscalYear != null) headerDs.FiscalYear = glFiscalYear.Name;
            }

            //if (!String.IsNullOrEmpty(workUnit))
            //{
            //    int id = int.Parse(workUnit);
            //    var branchCostCenter = _db.BranchCostCenter.FirstOrDefault(x => x.OrgStructureId == id);
            //    if (branchCostCenter != null)
            //        headerDs.WorkUnit = branchCostCenter.MainCompanyStructure.Name;
            //}

            if (!String.IsNullOrEmpty(project))
            {
                int id = int.Parse(project);
                var branch = _db.Branch.FirstOrDefault(x => x.BranchId == id);
                if (branch != null)
                    headerDs.ProjectOrBranch = branch.Name;
            }

            if (!String.IsNullOrEmpty(store))
            {

                var storeKeeperAssignment = _db.StoreKeeperAssignments.FirstOrDefault(x => x.StoreCode == store);
                if (storeKeeperAssignment != null)
                {
                    int empid = UserHelper.GetUserEmployeeId(UserHelper.GetUserName(storeKeeperAssignment.StoreKeeper));
                    var emp = _db.Person_Employee.Find(empid);
                    if (emp != null)
                    {
                        headerDs.ResponsibleUser = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
                        headerDs.JobTitle = EmployeeHelper.GetEmployeeJobTitle(emp.EmployeeId);
                        headerDs.WorkUnit = EmployeeHelper.GetEmployeeWorkUnit(emp.EmployeeId);
                    }
                }

            }

            if (!String.IsNullOrEmpty(employee))
            {
                int id = int.Parse(employee);
                var emp = _db.Person_Employee.Find(id);
                if (emp != null)
                {
                    headerDs.ResponsibleUser = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
                    headerDs.JobTitle = EmployeeHelper.GetEmployeeJobTitle(emp.EmployeeId);
                    headerDs.WorkUnit= EmployeeHelper.GetEmployeeWorkUnit(emp.EmployeeId);
                }
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
        public List<FixedAssetItemVm> GetFixedAssetCountItem(string category, string subcategory, string project, string workUnit, string employee, string store)
        {
            List<FixedAssetItemVm> assetCountItem = new List<FixedAssetItemVm>();
            var assetRegistrations = _db.AssetRegistrations
                .Select(x=> new {x.FixedAssetSubCategory,x.FixedAssetCategoryID,x.FixedAssetSubCategoryID,x.FixedAssetCategory,x.EmployeeId,x.BranchId,x.StoreCode,x.BarCodeNo,x.Tag,x.Location,x.Name,x.Size,x.GRNDate,x.ModelNo,x.SerialNo,x.Condition,x.Unit,x.Quanity,x.GRN,x.Remark,x.SIV,x.PlateNo})
                .AsQueryable();

            if (!String.IsNullOrEmpty(category))
            {
                assetRegistrations = assetRegistrations.Where(x => x.FixedAssetCategory.CategoryCode == category)
                    .AsQueryable();
                if (!String.IsNullOrEmpty(subcategory))
                {

                    assetRegistrations = assetRegistrations.Where(x => x.FixedAssetSubCategoryID.ToString() == subcategory)
                        .AsQueryable();
                }

                if (!String.IsNullOrEmpty(employee))
                {
                    assetRegistrations = assetRegistrations.Where(x => x.EmployeeId == employee).AsQueryable();

                }
                if (!String.IsNullOrEmpty(store))
                {
                    assetRegistrations = assetRegistrations.Where(x => x.StoreCode == store).AsQueryable();

                }

                if (!String.IsNullOrEmpty(project))
                {
                    int did = int.Parse(project);
                    assetRegistrations = assetRegistrations.Where(x => x.BranchId == did).AsQueryable();

                }

                if (!String.IsNullOrEmpty(workUnit))
                {
                    int did = int.Parse(workUnit);
                    List<string> employeeIds = EmployeeHelper.GetEmployeesByWorkUnit(did).Select(x => x.EmployeeId.ToString()).ToList();
                    assetRegistrations = assetRegistrations.Where(x => employeeIds.Contains(x.EmployeeId)).AsQueryable();

                }


                var assets = assetRegistrations.ToList();
                foreach (var asset in assets)
                {
                    //#region  filter
                    //bool canAdd = true;
                    //if (!String.IsNullOrEmpty(project))
                    //{
                    //    int id = int.Parse(project);
                    //    var projectExist = asset.LocationTrackings.Where(x => x.IsCurrent).Where(x => x.BranchId == id).ToList();
                    //    if (!projectExist.Any())
                    //    {
                    //        canAdd = false;
                    //    }
                    //}
                    //if (!String.IsNullOrEmpty(employee))
                    //{
                    //    int empid = int.Parse(employee);
                    //    var employeeExist = _db.UserCardItems.Where(x=>x.Active)
                    //        .Where(x => x.EmployeeId == empid)
                    //        .FirstOrDefault(x=>x.AssetRegistrationId==asset.AssetRegistrationId);
                    //    if (employeeExist!=null)
                    //    {
                    //        canAdd = false;
                    //    }
                    //}
                    //#endregion
                    var item = new FixedAssetItemVm
                    {
                        SubCategory = asset.FixedAssetSubCategory.SubCategoryName,
                        BarCode = asset.BarCodeNo,
                        Tag = asset.Tag,
                        Location = asset.Location,
                        Description = asset.Name,
                        Size = asset.Size,
                        GrnDate = asset.GRNDate,
                        Model = asset.ModelNo,
                        SerialNo = asset.SerialNo,
                        Condition = asset.Condition.Name,
                        Unit = asset.Unit,
                        Quantity = asset.Quanity,
                        RrtNo = asset.GRN,
                        SIV = asset.SIV,
                        PlateNo = asset.PlateNo,
                        Remark = asset.Remark,
                    };

                    assetCountItem.Add(item);
                }
            }
           
            
            return assetCountItem;
        }

    }
}
