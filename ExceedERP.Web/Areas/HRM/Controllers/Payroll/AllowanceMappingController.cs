
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = HR_Benefit_User + "," + HR_Benefit_Admin)]
    public class AllowanceMappingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        public ActionResult Index()
        {
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                                ||x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                ||x.LookupType == LookUpTypes.Jobtitle_External_Experience);
            ViewData["jobTitles"] = lookups;
            return View();
        }

        public ActionResult AllowanceMappings_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AllowanceMapping> allowancedefinations = db.AllowanceMappings.Where (x=> x.AllowanceDefinationId ==id);
            DataSourceResult result = allowancedefinations.ToDataSourceResult(request, allowanceDefination => new {
                AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                BranchId = allowanceDefination.BranchId,
                LookupId = allowanceDefination.LookupId,
                RankId = allowanceDefination.RankId,
                Remark = allowanceDefination.Remark,
                DateCreated = allowanceDefination.DateCreated,
                LastModified = allowanceDefination.LastModified,
                CreatedBy = allowanceDefination.CreatedBy,
                ModifiedBy = allowanceDefination.ModifiedBy,
                AllowanceMappingId = allowanceDefination.AllowanceMappingId,            
               
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceMappings_Create([DataSourceRequest] DataSourceRequest request,AllowanceMapping allowanceMapping, int idTwo)
        {
            ModelState.Clear();
            if (!allowanceMapping.BranchId.HasValue &&!allowanceMapping.LookupId.HasValue && !allowanceMapping.RankId.HasValue)
            {
                ModelState.AddModelError("", "Please select at least one option (Branch Rank, Lookup, or both).");
                return Json(new[] { allowanceMapping }.ToDataSourceResult(request, ModelState));
            }

            // 1. Check if mapping already exists
            bool exists = db.AllowanceMappings
                            .Any(x => x.AllowanceDefinationId == idTwo &&
                                      x.LookupId == allowanceMapping.LookupId &&
                                      x.BranchId == allowanceMapping.BranchId &&
                                      x.RankId == allowanceMapping.RankId);

            if (exists)
            {
                ModelState.AddModelError("", "This mapping already exists!");
                return Json(new[] { allowanceMapping }.ToDataSourceResult(request, ModelState));
            }

            allowanceMapping.AllowanceDefinationId = idTwo;

            if (ModelState.IsValid)
            {
                // 2. Build main entity
                var entity = new AllowanceMapping
                {
                    AllowanceDefinationId = allowanceMapping.AllowanceDefinationId,
                    BranchId = allowanceMapping.BranchId,
                    LookupId = allowanceMapping.LookupId,
                    RankId = allowanceMapping.RankId,
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = User.Identity.Name
                };

                db.AllowanceMappings.Add(entity);
                db.SaveChanges(); // Save the mapping to get the ID

                #region Log main mapping
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "payroll allowance",
                    "Success",
                    User.Identity.Name,
                    $"Added allowance {entity.AllowanceDefination?.AllowanceName} with lookup {entity.Lookup?.Description}",
                    Modules.Payroll);
                #endregion

                // 3. If idTwo is valid → Generate AllowancePerEmployee
                if (idTwo != 0)
                {
                    var userBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
                    var branchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                    // Fetch all current applicable employees
                    var employees = (from pe in db.Person_Employee
                                     join p in db.Placements
                                     on pe.EmployeeId equals p.EmployeeId
                                     where p.isCurrent 
                                           && userBranches.Contains(p.BranchId)
                                           && (!allowanceMapping.LookupId.HasValue || p.LookupId == allowanceMapping.LookupId)
                                           && (!allowanceMapping.RankId.HasValue || p.RankId == allowanceMapping.RankId)
                                           && (!allowanceMapping.BranchId.HasValue || p.BranchId == allowanceMapping.BranchId)
                                     select new
                                     {
                                         pe.EmployeeId,
                                         pe.EmployeeCode
                                     }).ToList();

                    // Get all employees who already have this allowance → avoid checking inside loop
                    HashSet<int> existingEmployeeIds = new HashSet<int>( db.AllowancePerEmployees
                        .Where(x => !x.Pend && x.AllowanceDefinationId == idTwo)
                        .Select(x => x.EmployeeId));   // fast lookup (O(1))

                    var newAllowanceRecords = new List<AllowancePerEmployee>();
                    var logs = new List<string>();

                    foreach (var emp in employees)
                    {
                        // skip if allowance already exists
                        if (existingEmployeeIds.Contains(emp.EmployeeId))
                            continue;

                        newAllowanceRecords.Add(new AllowancePerEmployee
                        {
                            EmployeeId = emp.EmployeeId,
                            AllowanceDefinationId = idTwo,
                            Remark = allowanceMapping.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = User.Identity.Name,
                            From = null,
                            To = null,
                            Pend = false,
                            EmployeeCode = emp.EmployeeCode,
                            OrgBranchName = branchName
                        });

                        logs.Add(emp.EmployeeCode);
                    }

                    // Bulk insert once
                    if (newAllowanceRecords.Count > 0)
                    {
                        db.AllowancePerEmployees.AddRange(newAllowanceRecords);
                        db.SaveChanges();
                    }

                    // Logging
                    foreach (var code in logs)
                    {
                        UserHelper.OperationLog(
                            Request.UserHostAddress,
                            "payroll allowance per employee",
                            "Success",
                            User.Identity.Name,
                            $"Added allowance {entity.AllowanceDefination?.AllowanceName} for employee {code}",
                            Modules.Payroll);
                    }
                }

                // Return the created mapping with generated ID
                allowanceMapping.AllowanceMappingId = entity.AllowanceMappingId;
            }

            return Json(new[] { allowanceMapping } .ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceMappings_Update( [DataSourceRequest] DataSourceRequest request, AllowanceMapping allowanceMapping)
        {
            ModelState.Clear();
            if (!allowanceMapping.BranchId.HasValue && !allowanceMapping.LookupId.HasValue && !allowanceMapping.RankId.HasValue)
            {
                ModelState.AddModelError("", "Please select at least one option (Branch Rank, Lookup, or both).");
                return Json(new[] { allowanceMapping }.ToDataSourceResult(request, ModelState));
            }

            bool duplicate = db.AllowanceMappings.Any(x =>
                x.AllowanceDefinationId == allowanceMapping.AllowanceDefinationId &&
                x.BranchId == allowanceMapping.BranchId &&
                x.LookupId == allowanceMapping.LookupId &&
                x.AllowanceMappingId != allowanceMapping.AllowanceMappingId);

            if (duplicate)
            {
                ModelState.AddModelError("", "This mapping already exists!");
                return Json(new[] { allowanceMapping }.ToDataSourceResult(request, ModelState));
            }


            // Update main AllowanceMapping
            var entity = db.AllowanceMappings.FirstOrDefault(x=>x.AllowanceMappingId==allowanceMapping.AllowanceMappingId);

            if (entity == null)
            {
                ModelState.AddModelError("", "Record not found!");

                return Json(new[] { allowanceMapping }.ToDataSourceResult(request, ModelState));

            }

            entity.BranchId = allowanceMapping.BranchId;
            entity.RankId = allowanceMapping.RankId;
            entity.LookupId = allowanceMapping.LookupId;
            entity.Remark = allowanceMapping.Remark;
            entity.LastModified = DateTime.Now;
            entity.ModifiedBy = User.Identity.Name;

            db.SaveChanges();

            // Log
            UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance", "Success",
                User.Identity.Name,
                $"Updated allowance {entity.AllowanceDefination?.AllowanceName} / lookup {entity.Lookup?.Description}",
                Modules.Payroll);


            // If no lookup selected, stop here
            if (allowanceMapping.LookupId == 0)
                return Json(new[] { allowanceMapping }.ToDataSourceResult(request, ModelState));


            // Get user branches only once
            List<int> userBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

          

            var employees = (from pe in db.Person_Employee
                             join p in db.Placements
                                 on pe.EmployeeId equals p.EmployeeId
                             where p.isCurrent
                                   && userBranches.Contains(p.BranchId)
                                   && (!allowanceMapping.LookupId.HasValue || p.LookupId == allowanceMapping.LookupId)
                                   && (!allowanceMapping.RankId.HasValue || p.RankId == allowanceMapping.RankId)
                                   && (!allowanceMapping.BranchId.HasValue || p.BranchId == allowanceMapping.BranchId)
                             select new
                             {
                                 pe.EmployeeId,
                                 pe.EmployeeCode,
                                 p.RankId,
                                 p.BranchId
                             }).ToList();
            // Load existing AllowancePerEmployee for this allowance once
            HashSet<int> existingEmployeeIds = new HashSet<int>( db.AllowancePerEmployees
                .Where(a => a.AllowanceDefinationId == allowanceMapping.AllowanceDefinationId)
                .Select(a => a.EmployeeId));
            string branchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            // Prepare bulk insert list
            List<AllowancePerEmployee> toInsert = new List<AllowancePerEmployee>();

            foreach (var emp in employees)
            {
                //  Skip if already created
                if (existingEmployeeIds.Contains(emp.EmployeeId))
                    continue;

                toInsert.Add(new AllowancePerEmployee
                {
                    EmployeeId = emp.EmployeeId,
                    AllowanceDefinationId = allowanceMapping.AllowanceDefinationId,
                    Remark = allowanceMapping.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = User.Identity.Name,
                    From = null,
                    To = null,
                    Pend = false,
                    EmployeeCode = emp.EmployeeCode,
                    OrgBranchName = branchName
                });
            }


            // Bulk insert only once  
            if (toInsert.Any())
            {
                db.AllowancePerEmployees.AddRange(toInsert);
                db.SaveChanges();

                // Logging
                foreach (var rec in toInsert)
                {
                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "payroll allowance per employee",
                        "Success",
                        User.Identity.Name,
                        $"Assigned allowance {entity.AllowanceDefination?.AllowanceName} to employee {rec.EmployeeCode}",
                        Modules.Payroll
                    );
                }
            }


            return Json(new[] { allowanceMapping }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceMappings_Destroy([DataSourceRequest]DataSourceRequest request, AllowanceMapping allowanceMapping)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
                // Only process employee allowances if Rank or Lookup or both are provided
                if (allowanceMapping.LookupId.HasValue || allowanceMapping.RankId.HasValue)
                {
                    // Build dynamic employee filter
                    var employeeQuery = db.Placements.Where(x => x.isCurrent);

                    if (allowanceMapping.RankId.HasValue)
                        employeeQuery = employeeQuery.Where(x => x.RankId == allowanceMapping.RankId);

                    if (allowanceMapping.LookupId.HasValue)
                        employeeQuery = employeeQuery.Where(x => x.LookupId == allowanceMapping.LookupId);

                    var employeeIds = employeeQuery
                        .Select(x => x.EmployeeId)
                        .Distinct()
                        .ToList();

                    // Allowance records to update
                    var employeeAllowances = db.AllowancePerEmployees
                        .Where(x => x.AllowanceDefinationId == allowanceMapping.AllowanceDefinationId
                                 && employeeIds.Contains(x.EmployeeId))
                        .ToList();

                    // Mark all as pending
                    foreach (var emp in employeeAllowances)
                    {
                        emp.Pend = true;
                        db.Entry(emp).State = EntityState.Modified;
                    }
                }

                // Prepare entity for deletion
                var entity = new AllowanceMapping
                {
                    AllowanceDefinationId = allowanceMapping.AllowanceDefinationId,
                    AllowanceMappingId = allowanceMapping.AllowanceMappingId,
                    Remark = allowanceMapping.Remark,
                    LookupId = allowanceMapping.LookupId,
                    RankId = allowanceMapping.RankId,
                    DateCreated = allowanceMapping.DateCreated,
                    LastModified = allowanceMapping.LastModified,
                    CreatedBy = allowanceMapping.CreatedBy,
                    ModifiedBy = allowanceMapping.ModifiedBy,
                };

                // Log removal
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Delete",
                    "Success",
                    User.Identity.Name,
                    $"Deleted allowance mapping of allowance {allowanceMapping?.AllowanceDefinationId} and {allowanceMapping?.LookupId}",
                    Modules.Payroll
                );

                // Delete mapping record
                db.AllowanceMappings.Attach(entity);
                db.AllowanceMappings.Remove(entity);

                db.SaveChanges();
            }


            return Json(new[] { allowanceMapping }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
