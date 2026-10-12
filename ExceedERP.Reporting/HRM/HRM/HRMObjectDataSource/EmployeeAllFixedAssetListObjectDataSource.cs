using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeAllFixedAssetListObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeFixedAsset> GetEmployeeFixedAssetList(string employee, string branch, string DepartId)
        {
            int.TryParse(employee, out int employeeId);
            int.TryParse(branch, out int branchId);
            int.TryParse(DepartId, out int orgId);
            if (branchId <= 0)
                return new List<EmployeeFixedAsset>();

            var query =
                from u in db.UserCardItems
                join e in db.Person_Employee on u.EmployeeId equals e.EmployeeId
                join p in db.Placements on e.EmployeeId equals p.EmployeeId
                where e.IsActive && p.isCurrent
                select new { u, e, p };

            if (employeeId > 0)
                query = query.Where(x => x.e.EmployeeId == employeeId);

            if (branchId > 0)
                query = query.Where(x => x.p.BranchId == branchId);

            if (orgId > 0)
                query = query.Where(x => x.p.OrgStructureId == orgId);

            var data = query
                .Select(x => new
                {
                    Employee = x.e,
                    Placement = x.p,
                    UserCard = x.u,
                    Department = db.MainCompanyStructures
                                    .Where(m => m.OrgStructureId == x.p.OrgStructureId)
                                    .Select(m => m.Name)
                                    .FirstOrDefault(),
                    Branch = db.Branch
                                .Where(b => b.BranchId == x.p.BranchId)
                                .Select(b => b.Name)
                                .FirstOrDefault(),
                    JobTitle = db.Lookups
                                .Where(l => l.LookupId == x.p.LookupId && (l.LookupType == LookUpTypes.JOB_TITLE
                                                                            || l.LookupType==LookUpTypes.Jobtitle_External_Experience 
                                                                            || l.LookupType==LookUpTypes.OLD_JOB_TITLE))
                                .Select(l => l.Description)
                                .FirstOrDefault(),
                    Asset = db.AssetRegistrations
                                .Where(a => a.AssetRegistrationId == x.u.AssetRegistrationId)
                                .Select(a => new
                                {
                                    a.Tag,
                                    a.Name,
                                    a.Size,
                                    a.ModelNo,
                                    a.SerialNo,
                                    a.Unit,
                                    a.Quanity,
                                    a.SIV,
                                    a.GRN,
                                    a.GRNDate,
                                    Condition = db.FixedAssetConditions
                                                    .Where(c => c.FixedAssetConditionId == a.FixedAssetConditionId)
                                                    .Select(c => c.Name)
                                                    .FirstOrDefault()
                                }).FirstOrDefault()
                }).AsNoTracking().ToList();

            var result = data
                .GroupBy(x => x.Employee.EmployeeId)
                .Select(g => new EmployeeFixedAsset
                {
                    Id = g.First().Employee.EmployeeCode,
                    FullName = g.First().Employee.EmployeeCode + "-" +
                               g.First().Employee.FirstName + " " +
                               g.First().Employee.MiddleName + " " +
                               g.First().Employee.LastName,
                    Department = g.First().Department,
                    Branch = g.First().Branch,
                    JobTitle = g.First().JobTitle,
                    UserCardItemsList = g.Select(f => new UserCardItemsList
                    {
                        ItemDesc = f.UserCard.Name,
                        Quanity = f.UserCard.Quanity,
                        UnitPrice = f.UserCard.UnitPrice,
                        TagNumber = f.UserCard.Tag,
                        IssueDate = f.UserCard.IssueDate.ToShortDateString(),
                        AssetNo = f.Asset?.Tag,
                        Description = f.Asset?.Name,
                        Size = f.Asset?.Size,
                        Model = f.Asset?.ModelNo,
                        SerialPartNo = f.Asset?.SerialNo,
                        Condition = f.Asset?.Condition,
                        Unit = f.Asset?.Unit,
                        Qty = f.Asset?.Quanity.ToString(),
                        MTITNo = f.Asset?.SIV,
                        GRNNo = f.Asset?.GRN,
                        GRNDate = f.Asset?.GRNDate
                    }).ToList()
                }).ToList();

            return result;
        }
        //public List<EmployeeFixedAsset> GetEmployeeFixedAssetList(string employee, string branch)
        //{
        //    List<EmployeeFixedAsset> list = new List<EmployeeFixedAsset> { };
        //    EmployeeFixedAsset EmployeeFixedAsset = new EmployeeFixedAsset();
        //    UserCardItemsList userCardItemsList = new UserCardItemsList();
        //    var userCardItems = db.UserCardItems;
        //    var employeeIds = new HashSet<int>(userCardItems.Select(x => x.EmployeeId));
        //    var employeeList = db.Person_Employee.Where(x => x.IsActive && employeeIds.Contains(x.EmployeeId));
        //    var placements = db.Placements.Where(x => x.isCurrent && employeeIds.Contains(x.EmployeeId));
        //    if(!string.IsNullOrWhiteSpace(employee)&&int.TryParse(employee,out int employeeId))
        //    {
        //         employeeList = employeeList.Where(x => x.EmployeeId== employeeId);
        //        placements = placements.Where(x => x.EmployeeId == employeeId);
        //    }
        //    if (!string.IsNullOrWhiteSpace(branch)&&int.TryParse(employee,out int branchId))
        //    {
        //         employeeList = employeeList.Where(x => x.BranchId== branchId);
        //        placements = placements.Where(x => x.BranchId == branchId);
        //    }

        //    foreach (var placement  in placements)
        //    {
        //        var Oneemployee = employeeList.Where(x => x.EmployeeCode == employee).FirstOrDefault();
        //        if (Oneemployee == null)
        //            continue;


        //        var usercards = userCardItems.Where(x => x.EmployeeId == Oneemployee.EmployeeId).ToList();

        //        EmployeeFixedAsset.UserCardItemsList = new List<UserCardItemsList>();
        //        EmployeeFixedAsset.FullName = Oneemployee.EmployeeCode+"-"+Oneemployee.FirstName + " " + Oneemployee.MiddleName + " " + Oneemployee.LastName;
        //        EmployeeFixedAsset.Id = Oneemployee.EmployeeCode;


        //        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
        //        if (mainCompany != null)
        //        {
        //            EmployeeFixedAsset.Department = mainCompany.Name;
        //        }
        //        var branchh = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
        //        if (branchh != null)
        //        {
        //            EmployeeFixedAsset.Branch = branchh.Name;
        //        }
        //        var job = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
        //        if (job != null)
        //        {
        //            EmployeeFixedAsset.JobTitle = job.Description;
        //        }


        //            foreach (var f in usercards)
        //            {
        //                userCardItemsList = new UserCardItemsList();
        //                userCardItemsList.ItemDesc = f.Name;
        //                userCardItemsList.Quanity = f.Quanity;
        //                userCardItemsList.UnitPrice = f.UnitPrice;
        //                userCardItemsList.TagNumber = f.Tag;
        //                userCardItemsList.IssueDate = f.IssueDate.ToShortDateString();

        //                var fixedAsset = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == f.AssetRegistrationId);
        //                if (fixedAsset != null)
        //                {
        //                    //userCardItemsList.AccNo = "";
        //                    userCardItemsList.AssetNo = fixedAsset.Tag;
        //                    userCardItemsList.Description = fixedAsset.Name;
        //                    userCardItemsList.Size = fixedAsset.Size;
        //                    userCardItemsList.Model = fixedAsset.ModelNo;
        //                    userCardItemsList.SerialPartNo = fixedAsset.SerialNo;

        //                    var assetCondition = db.FixedAssetConditions.FirstOrDefault(x => x.FixedAssetConditionId == fixedAsset.FixedAssetConditionId);
        //                    if (assetCondition != null)
        //                    {
        //                        userCardItemsList.Condition = assetCondition.Name;
        //                    }
        //                    userCardItemsList.Unit = fixedAsset.Unit;
        //                    userCardItemsList.Qty = fixedAsset.Quanity.ToString();
        //                    userCardItemsList.MTITNo = fixedAsset.SIV;
        //                    // userCardItemsList.MTITDate = fixedAsset.;
        //                    userCardItemsList.GRNNo = fixedAsset.GRN;
        //                    userCardItemsList.GRNDate = fixedAsset.GRNDate;
        //                }

        //                EmployeeFixedAsset.UserCardItemsList.Add(userCardItemsList);

        //            }

        //            list.Add(EmployeeFixedAsset);

        //    }

        //    return list;
        //}

        public void Dispose()
        {
            db.Dispose();
        }
    }
}