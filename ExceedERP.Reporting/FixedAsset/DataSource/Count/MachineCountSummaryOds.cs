using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.Inventory.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.DataSource.Count
{
    [DataObject]
    public class MachineCountSummaryOds : List<ItemVm>
    {
        ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetCountSheetHeader GetMachineryCountSummaryHeader(string fiscalYear, string project, string workunit, string employee)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            FixedAssetCountSheetHeader headerDs = new FixedAssetCountSheetHeader();

            if (!String.IsNullOrEmpty(fiscalYear))
            {
                int id = int.Parse(fiscalYear);
                var glFiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == id);
                if (glFiscalYear != null) headerDs.FiscalYear = glFiscalYear.Name;
            }

            if (!String.IsNullOrEmpty(workunit))
            {
                int id = int.Parse(workunit);
                var branchCostCenter = _db.BranchCostCenter.FirstOrDefault(x => x.OrgStructureId == id);
                if (branchCostCenter != null)
                    headerDs.WorkUnit = branchCostCenter.MainCompanyStructure.Name;
            }

            if (!String.IsNullOrEmpty(project))
            {
                int id = int.Parse(project);
                var branch = _db.Branch.FirstOrDefault(x => x.BranchId == id);
                if (branch != null)
                    headerDs.ProjectOrBranch = branch.Name;
            }

            if (!String.IsNullOrEmpty(employee))
            {
                int id = int.Parse(employee);
                var emp = _db.Person_Employee.Find(id);
                if (emp != null)
                    headerDs.ResponsibleUser = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
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
        public List<MachineryCountSummarySheetVm> GetMachineryCountSummary(string category, string subcategory, string project, string workunit, string employee)
        {
          
            List<MachineryCountSummarySheetVm> countSummary = new List<MachineryCountSummarySheetVm>();
            if (!String.IsNullOrEmpty(category))
            {
                var assetRegistrations = _db.AssetRegistrations
                    .Where(x => x.FixedAssetCategory.CategoryCode == category)
                    .Select(x => new { x.FixedAssetSubCategory, x.FixedAssetSubCategoryID, x.FixedAssetCategory, x.EmployeeId, x.BranchId, x.StoreCode, x.BarCodeNo, x.Tag, x.Manufacturer, x.Name, x.ChassisNo, x.MotorNo, x.PlateNo, x.SIV, x.ModelNo, x.SerialNo, x.Condition, x.Unit, x.Quanity, x.GRN, x.Remark }).AsQueryable();
                if (!String.IsNullOrEmpty(subcategory))
                {
                    assetRegistrations = assetRegistrations
                        .Where(x => x.FixedAssetSubCategoryID.ToString() == subcategory).AsQueryable();
                }
                if (!String.IsNullOrEmpty(project))
                {
                    int did = int.Parse(project);
                    assetRegistrations = assetRegistrations.Where(x => x.BranchId == did).AsQueryable();

                }

                if (!String.IsNullOrEmpty(employee))
                {
                    assetRegistrations = assetRegistrations.Where(x => x.EmployeeId == employee).AsQueryable();

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
                    bool canAdd = true;
                    //if (!String.IsNullOrEmpty(project))
                    //{
                    //    int id = int.Parse(project);
                    //    var projectExist = asset.LocationTrackings.Where(x => x.IsCurrent).Where(x=>x.BranchId==id).ToList();
                    //    if (projectExist.Any())
                    //    {
                    //        if (!String.IsNullOrEmpty(employee))
                    //        {
                    //            int empid = int.Parse(employee);
                    //            var employeeExist =asset.LocationTrackings.Where(x => x.IsCurrent).Where(x => x.EmployeeId == empid).ToList();
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
                    #endregion

                    var item = new MachineryCountSummarySheetVm
                    {
                        TagNo = asset.Tag,
                        Description = asset.Name,
                        Manufacturer = asset.Manufacturer,
                        Model = asset.ModelNo,
                        SerialNo = asset.SerialNo,
                        ChasisNo = asset.ChassisNo,
                        MotorNo = asset.MotorNo,
                        PlateNo = asset.PlateNo,
                        Unit = "",
                        RrtNo = asset.GRN,
                        Status = asset.Condition.Name,
                        Remark = "",
                    };

                    countSummary.Add(item);
                }
            }
            return countSummary;
        }

    }
}
