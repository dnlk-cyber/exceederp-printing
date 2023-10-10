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
        public FixedAssetCountSheetHeader GetFixedAssetCountSheetHeader(int id, string fiscalyear, string project,string store, string workUnit, string employee)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            FixedAssetCountSheetHeader headerDs = new FixedAssetCountSheetHeader();

            if (!String.IsNullOrEmpty(fiscalyear))
            {
                int fid = int.Parse(fiscalyear);
                var glFiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == fid);
                if (glFiscalYear != null) headerDs.FiscalYear = glFiscalYear.Name;
            }

            //if (!String.IsNullOrEmpty(workUnit))
            //{
            //    int id = int.Parse(workUnit);
            //    var branchCostCenter = _db.BranchCostCenter.FirstOrDefault(x => x.OrgStructureId == id);
            //    if (branchCostCenter != null)
            //        headerDs.WorkUnit = branchCostCenter.MainCompanyStructure.Name;
            //}

            //if (!String.IsNullOrEmpty(project))
            //{
            //    int pid = int.Parse(project);
            //    var branch = _db.Branch.FirstOrDefault(x => x.BranchId == pid);
            //    if (branch != null)
            //        headerDs.ProjectOrBranch = branch.Name;
            //}

            //if (!String.IsNullOrEmpty(store))
            //{

            //    var storeKeeperAssignment = _db.StoreKeeperAssignments.FirstOrDefault(x => x.StoreCode == store);
            //    if (storeKeeperAssignment != null)
            //    {
            //        int empid = UserHelper.GetUserEmployeeId(UserHelper.GetUserName(storeKeeperAssignment.StoreKeeper));
            //        var emp = _db.Person_Employee.Find(empid);
            //        if (emp != null)
            //        {
            //            headerDs.ResponsibleUser = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
            //            headerDs.JobTitle = EmployeeHelper.GetEmployeeJobTitle(emp.EmployeeId);
            //            headerDs.WorkUnit = EmployeeHelper.GetEmployeeWorkUnit(emp.EmployeeId);
            //        }
            //    }

            //}

            //if (!String.IsNullOrEmpty(employee))
            //{
            //    int fid = int.Parse(employee);
            //    var emp = _db.Person_Employee.Find(id);
            //    if (emp != null)
            //    {
            //        headerDs.ResponsibleUser = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
            //        headerDs.JobTitle = EmployeeHelper.GetEmployeeJobTitle(emp.EmployeeId);
            //        headerDs.WorkUnit= EmployeeHelper.GetEmployeeWorkUnit(emp.EmployeeId);
            //    }
            //}

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
        public List<FixedAssetItemVm> GetFixedAssetCountItem(int id,string category, string subcategory, string project, string workUnit, string employee, string store)
        {
            List<FixedAssetItemVm> assetCountItem = new List<FixedAssetItemVm>();
            var countItems = _db.FixedAssetCountItems.Where(x => x.FixedAssetCountId == id).ToList();
                foreach (var a in countItems)
                {

                    var asset = _db.AssetRegistrations.Find(a.AssetRegistrationId);
                    
                    var item = new FixedAssetItemVm
                    {
                        SubCategory = _db.FixedAssetSubCategories.Find(a.FixedAssetSubCategoryId)?.SubCategoryName,
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
            
           
            
            return assetCountItem;
        }

    }
}
