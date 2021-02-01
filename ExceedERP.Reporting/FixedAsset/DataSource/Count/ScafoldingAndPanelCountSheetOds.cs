using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.DataSource.Count
{
    [DataObject]
    public class ScafoldingAndPanelCountSheetOds : List<FixedAssetCountSheetHeader>
    {
        ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetCountSheetHeader GetScafoldingAndPanelCountSheetHeader(string fiscalYear, string project,string store, string workunit, string employee)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            FixedAssetCountSheetHeader headerDs = new FixedAssetCountSheetHeader();

            if (!String.IsNullOrEmpty(fiscalYear))
            {
                int id = int.Parse(fiscalYear);
                var glFiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == id);
                if (glFiscalYear != null) headerDs.FiscalYear = glFiscalYear.Name;
            }

            //if (!String.IsNullOrEmpty(workunit))
            //{
            //    int id = int.Parse(workunit);
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
                    headerDs.WorkUnit = EmployeeHelper.GetEmployeeWorkUnit(emp.EmployeeId);
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
        public List<FixedAssetItemVm> GetScafoldingAndPanelCountItem(string category, string subcategory, string project, string workunit, string employee,string store)
        {
          
            List<FixedAssetItemVm> assetCountItem = new List<FixedAssetItemVm>();
            var assetRegistrations = _db.AssetRegistrations
                .Select(x => new { x.FixedAssetSubCategory, x.FixedAssetSubCategoryID, x.FixedAssetCategory, x.EmployeeId, x.BranchId, x.StoreCode, x.BarCodeNo, x.Tag, x.Manufacturer, x.Name, x.ChassisNo, x.MotorNo, x.PlateNo, x.SIV, x.ModelNo, x.SerialNo, x.Size, x.Unit, x.Quanity, x.GRN, x.Remark })
                .AsQueryable();

            if (!String.IsNullOrEmpty(category))
            {
                assetRegistrations = assetRegistrations.Where(x => x.FixedAssetCategory.CategoryCode == category)
                    .AsQueryable();

            }
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

         
            if (!String.IsNullOrEmpty(workunit))
            {
                int did = int.Parse(workunit);
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
                //    var projectExist = asset.LocationTrackings.Where(x => x.IsCurrent).Where(x=>x.BranchId==id).ToList();
                //    if (projectExist.Any())
                //    {
                //        if (!String.IsNullOrEmpty(employee))
                //        {
                //            int empid = int.Parse(employee);
                //            var employeeExist =asset.LocationTrackings.Where(x => x.IsCurrent).Where(x => x.EmployeeId == empid.ToString()).ToList();
                //            if (!employeeExist.Any())
                //            {
                //                canAdd = false;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        canAdd = false;
                //    }
                //}
                //#endregion

                var item = new FixedAssetItemVm
                {
                    BarCode = asset.BarCodeNo,
                    Description = asset.Name,
                    Model = asset.ModelNo,
                    Size = asset.Size,
                    SerialNo = asset.SerialNo,
                    Unit = asset.Unit,
                    Quantity = asset.Quanity,
                    IssueOrTransfer = asset.SIV,
                    RrtNo = asset.GRN,
                    BranchOrProject = InventoryPrintHelper.BranchName(asset.BranchId),
                    Remark = "",
                };

                assetCountItem.Add(item);
            }
            return assetCountItem;
        }

    }
}
