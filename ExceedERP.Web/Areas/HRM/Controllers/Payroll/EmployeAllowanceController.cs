using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = HR_Admin + "," + HR_Benefit_User + "," + HR_Benefit_Admin)]
    public class EmployeAllowanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        private const string HR_Admin = "HR_Admin";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var alloanceList = db.AllowanceDefinations.AsQueryable();
            ViewData["AllowanceList"] = alloanceList.OrderBy(o => o.AllowanceName);

            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive).Where(x => x.IsActive);

            return View();
        }


        public ActionResult AllowancePerEmployees_Read([DataSourceRequest] DataSourceRequest request)
        {
            db.Database.CommandTimeout = 7200; // added

            string user = User.Identity.Name;
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(user);
            List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(user);

            var placments = db.Placements.Where(x => x.isCurrent).
                                         Where(x => mybraches.Contains(x.BranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                placments = placments.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
            List<int> employees = placments.Select(x => x.EmployeeId).Distinct().ToList();

            // 1. Fetch raw records from the database first (without unsupported array projections)
            var rawData = db.AllowancePerEmployees
                .AsNoTracking()
                .Where(x => employees.Contains(x.EmployeeId))
                .ToList();

            // 2. Project the results in-memory to safely initialize arrays/collections
            var result = rawData.Select(allowancePerEmployee => new AllowancePerEmployee
            {
                AllowanceId = allowancePerEmployee.AllowanceId,
                EmployeeId = allowancePerEmployee.EmployeeId,
                AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
                AllowanceDefinationIds = new int[] { allowancePerEmployee.AllowanceDefinationId }, // 👈 Safe in-memory assignment
                Remark = allowancePerEmployee.Remark,
                DateCreated = allowancePerEmployee.DateCreated,
                LastModified = allowancePerEmployee.LastModified,
                CreatedBy = allowancePerEmployee.CreatedBy,
                ModifiedBy = allowancePerEmployee.ModifiedBy,
                EmployeeCode = allowancePerEmployee.EmployeeCode?? allowancePerEmployee.Employee.EmployeeCode,
                OrgBranchName = allowancePerEmployee.OrgBranchName,
                From = allowancePerEmployee.From,
                To = allowancePerEmployee.To,
                Pend = allowancePerEmployee.Pend
            }).AsQueryable().ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void PerapreEmployeeAllowance()
        {
            var mappings = db.AllowanceMappings;
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name).Where(x => x.IsActive);
            var placment = db.Placements.Where(x => x.isCurrent).ToList();

            foreach (var l in list)
            {
                db = new ExceedDbContext();

                foreach (var x in mappings)
                {



                    var empJobtitle = placment.FirstOrDefault(y => y.EmployeeId == l.PersonId);
                    if (empJobtitle != null)
                    {
                        if (x.LookupId == empJobtitle.LookupId)
                        {
                            var checkExistance = db.AllowancePerEmployees.FirstOrDefault(g => g.EmployeeId == l.PersonId && x.AllowanceDefinationId == g.AllowanceDefinationId);
                            if (checkExistance == null)
                            {
                                var entity = new AllowancePerEmployee
                                {
                                    EmployeeId = l.PersonId,
                                    AllowanceDefinationId = x.AllowanceDefinationId,
                                    Remark = null,
                                    DateCreated = DateTime.Now,
                                    LastModified = null,
                                    CreatedBy = User.Identity.Name,
                                    ModifiedBy = null,
                                    EmployeeCode = l.EmployeeCode,
                                    From = null,
                                    To = null,
                                    Pend = false,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                                };

                                db.AllowancePerEmployees.Add(entity);
                                db.SaveChanges();

                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"add allowance  {entity.AllowanceDefination.AllowanceName} to employee {entity.EmployeeCode} ", Modules.Payroll);
                                #endregion

                            }
                            else
                            {

                                if (checkExistance.To != null && checkExistance.To.Value.Date <= DateTime.Now.Date)
                                {
                                    if (checkExistance.Pend)
                                    {
                                        checkExistance.Pend = false;
                                        checkExistance.From = null;
                                        checkExistance.To = null;
                                        db.AllowancePerEmployees.Add(checkExistance);
                                        //db.Entry(checkExistance).State = EntityState.Modified;
                                        db.SaveChanges();
                                        #region operation log
                                        UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"unpend  {checkExistance.AllowanceDefination.AllowanceName} to employee {checkExistance.EmployeeCode} ", Modules.Payroll);
                                        #endregion
                                    }


                                }


                            }

                        }

                    }

                }
            }
            GetDelegeatedEmployee(mappings);
        }

        private void GetDelegeatedEmployee(DbSet<AllowanceMapping> mappings)
        {
            var assignedJobTitles = mappings.Select(x => x.LookupId).Distinct().ToList();
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && !assignedJobTitles.Contains(x.LookupId)).ToList();


            var delegatedEmployees = from l in lookups
                                     join m in db.Placements
                                     on l.LookupId equals m.LookupId
                                     join e in db.Person_Employee
                                     on m.EmployeeId equals e.EmployeeId
                                     join p in db.AllowancePerEmployees
                                     on e.EmployeeId equals p.EmployeeId
                                     join a in db.AllowanceDefinations
                on p.AllowanceDefinationId equals a.AllowanceDefinationId


                                     select (new
                                     {
                                         a.AllowanceDefinationId,
                                         p.EmployeeId,
                                         p.Pend,
                                         p.To,
                                         l.LookupId
                                     });
            var employees = delegatedEmployees.ToList();

            foreach (var e in employees)
            {
                var checkExistance =
                    db.AllowancePerEmployees.FirstOrDefault(x => x.AllowanceDefinationId == e.AllowanceDefinationId &&
                                                        e.EmployeeId == x.EmployeeId);
                if (checkExistance != null)
                {
                    if (e.To != null && e.To.Value.Date <= DateTime.Now.Date)
                    {
                        if (!e.Pend)
                        {
                            checkExistance.Pend = true;
                            db.AllowancePerEmployees.Attach(checkExistance);
                            db.Entry(checkExistance).State = EntityState.Modified;
                            db.SaveChanges();

                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"pend delegated allowance  {checkExistance.AllowanceDefination.AllowanceName} of employee {checkExistance.EmployeeCode} ", Modules.Payroll);
                            #endregion
                        }


                    }
                }
            }

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowancePerEmployees_Create([DataSourceRequest]DataSourceRequest request, AllowancePerEmployee allowancePerEmployee)
        {
            ModelState.Clear();

            // Safely extract definition IDs, ignoring any zeros or uninitialized values
            var definitionIds = (allowancePerEmployee.AllowanceDefinationIds != null && allowancePerEmployee.AllowanceDefinationIds.Length > 0)
                ? allowancePerEmployee.AllowanceDefinationIds.Where(id => id > 0).ToArray()
                : (allowancePerEmployee.AllowanceDefinationId > 0 ? new[] { allowancePerEmployee.AllowanceDefinationId } : new int[0]);

            if (!definitionIds.Any())
            {
                ModelState.AddModelError("", "Please select at least one allowance definition.");
                return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
            }

            // --- Date Validation for HARDSHIP and DESERT ---
            var selectedDefinitions = db.AllowanceDefinations
                .Where(x => definitionIds.Contains(x.AllowanceDefinationId))
                .ToList();

            bool requiresDates = selectedDefinitions.Any(x =>
                x.AllowanceType == AllowanceType.HARDSHIP ||
                x.AllowanceType == AllowanceType.DESERT);

            if (requiresDates)
            {
                if (!allowancePerEmployee.From.HasValue || allowancePerEmployee.From == DateTime.MinValue)
                {
                    ModelState.AddModelError("From", "The 'From' date is required for Hardship and Desert allowances.");
                }

                if (!allowancePerEmployee.To.HasValue || allowancePerEmployee.To == DateTime.MinValue)
                {
                    ModelState.AddModelError("To", "The 'To' date is required for Hardship and Desert allowances.");
                }

                if (allowancePerEmployee.From.HasValue && allowancePerEmployee.To.HasValue && allowancePerEmployee.From > allowancePerEmployee.To)
                {
                    ModelState.AddModelError("To", "The 'To' date must be later than or equal to the 'From' date.");
                }

                if (!ModelState.IsValid)
                {
                    return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
                }
            }
            // -----------------------------------------------
            var results = new List<AllowancePerEmployee>();

            foreach (var defId in definitionIds)
            {
                var exist = db.AllowancePerEmployees.Where(x => x.EmployeeId == allowancePerEmployee.EmployeeId && x.AllowanceDefinationId == defId);

                if (exist.Any())
                {
                    continue; // Skip existing duplicates gracefully during batch inserts
                }

                var allowancetype = db.AllowanceDefinations.Find(defId);

                if (allowancetype != null)
                {
                    var allowanceSettings = db.AllowanceSettings
                        .Where(x => x.AllowanceType == allowancetype.AllowanceType)
                        .ToList();

                    var placement = db.Placements
                        .FirstOrDefault(x => x.EmployeeId == allowancePerEmployee.EmployeeId && x.isCurrent);

                    if (allowanceSettings.Any())
                    {
                        // 1. Head-only restriction
                        if (allowanceSettings.Any(x => x.IsOnlyForHead))
                        {
                            if (placement != null && !placement.IsHead)
                            {
                                continue; // Skip if restricted to head positions only
                            }
                        }

                        // 2. Job title restriction
                        if (placement != null)
                        {
                            bool isAllowedJobTitle = allowanceSettings.Any(x => x.LookupId == placement.LookupId);

                            if (!isAllowedJobTitle)
                            {
                                continue; // Skip if job title doesn't match allowance settings
                            }
                        }
                    }
                }

                var firstOrDefault = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == allowancePerEmployee.EmployeeId);
                if (firstOrDefault != null)
                {
                    var entity = new AllowancePerEmployee
                    {
                        EmployeeId = allowancePerEmployee.EmployeeId,
                        AllowanceDefinationId = defId,
                        Remark = allowancePerEmployee.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = allowancePerEmployee.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = allowancePerEmployee.ModifiedBy,
                        EmployeeCode = firstOrDefault.EmployeeCode,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        From = allowancePerEmployee.From,
                        To = allowancePerEmployee.To,
                        Pend = allowancePerEmployee.Pend,
                    };

                    db.AllowancePerEmployees.Add(entity);
                    db.SaveChanges();

                    var resultRow = new AllowancePerEmployee
                    {
                        AllowanceId = entity.AllowanceId,
                        EmployeeId = entity.EmployeeId,
                        AllowanceDefinationId = entity.AllowanceDefinationId,
                        Remark = entity.Remark,
                        DateCreated = entity.DateCreated,
                        LastModified = entity.LastModified,
                        CreatedBy = entity.CreatedBy,
                        ModifiedBy = entity.ModifiedBy,
                        EmployeeCode = entity.EmployeeCode,
                        OrgBranchName = entity.OrgBranchName,
                        From = entity.From,
                        To = entity.To,
                        Pend = entity.Pend
                    };

                    results.Add(resultRow);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"create allowance {entity.AllowanceId} of employee {entity.EmployeeCode}", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowancePerEmployees_Update([DataSourceRequest]DataSourceRequest request, AllowancePerEmployee allowancePerEmployee)
        {
            ModelState.Clear();

            // Capture all selected definition IDs from the multi-select popup
            ModelState.Clear();

            // Safely extract definition IDs, ignoring any zeros or uninitialized values
            var definitionIds = (allowancePerEmployee.AllowanceDefinationIds != null && allowancePerEmployee.AllowanceDefinationIds.Length > 0)
                ? allowancePerEmployee.AllowanceDefinationIds.Where(id => id > 0).ToArray()
                : (allowancePerEmployee.AllowanceDefinationId > 0 ? new[] { allowancePerEmployee.AllowanceDefinationId } : new int[0]);

            if (!definitionIds.Any())
            {
                ModelState.AddModelError("", "Please select at least one allowance definition.");
                return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
            }

            // --- Date Validation for HARDSHIP and DESERT ---
            var selectedDefinitions = db.AllowanceDefinations
                .Where(x => definitionIds.Contains(x.AllowanceDefinationId))
                .ToList();

            bool requiresDates = selectedDefinitions.Any(x =>
                x.AllowanceType == AllowanceType.HARDSHIP ||
                x.AllowanceType == AllowanceType.DESERT);

            if (requiresDates)
            {
                if (!allowancePerEmployee.From.HasValue || allowancePerEmployee.From == DateTime.MinValue)
                {
                    ModelState.AddModelError("From", "The 'From' date is required for Hardship and Desert allowances.");
                }

                if (!allowancePerEmployee.To.HasValue || allowancePerEmployee.To == DateTime.MinValue)
                {
                    ModelState.AddModelError("To", "The 'To' date is required for Hardship and Desert allowances.");
                }

                if (allowancePerEmployee.From.HasValue && allowancePerEmployee.To.HasValue && allowancePerEmployee.From > allowancePerEmployee.To)
                {
                    ModelState.AddModelError("To", "The 'To' date must be later than or equal to the 'From' date.");
                }

                if (!ModelState.IsValid)
                {
                    return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
                }
            }
            // -----------------------------------------------

            if (ModelState.IsValid)
            {
                var employee = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == allowancePerEmployee.EmployeeId);
                string empCode = employee?.EmployeeCode;

                // 1. Update the primary/current record with the first selected definition
                var primaryDefId = definitionIds.FirstOrDefault();
                var entity = db.AllowancePerEmployees.Find(allowancePerEmployee.AllowanceId);

                if (entity != null)
                {
                    entity.EmployeeId = allowancePerEmployee.EmployeeId;
                    entity.AllowanceDefinationId = primaryDefId != 0 ? primaryDefId : allowancePerEmployee.AllowanceDefinationId;
                    entity.Remark = allowancePerEmployee.Remark;
                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;
                    entity.EmployeeCode = empCode;
                    entity.From = allowancePerEmployee.From;
                    entity.To = allowancePerEmployee.To;
                    entity.Pend = allowancePerEmployee.Pend;

                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // 2. If multiple definitions were checked during update, create records for any additional ones
                foreach (var defId in definitionIds.Skip(1))
                {
                    var exists = db.AllowancePerEmployees.Any(x => x.EmployeeId == allowancePerEmployee.EmployeeId && x.AllowanceDefinationId == defId);
                    if (!exists)
                    {
                        var extraEntity = new AllowancePerEmployee
                        {
                            EmployeeId = allowancePerEmployee.EmployeeId,
                            AllowanceDefinationId = defId,
                            Remark = allowancePerEmployee.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = User.Identity.Name,
                            EmployeeCode = empCode,
                            OrgBranchName = allowancePerEmployee.OrgBranchName,
                            From = allowancePerEmployee.From,
                            To = allowancePerEmployee.To,
                            Pend = allowancePerEmployee.Pend,
                        };
                        db.AllowancePerEmployees.Add(extraEntity);
                        db.SaveChanges();
                    }
                }

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"updated allowance definitions for employee {empCode}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        }
        
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult AllowancePerEmployees_Create([DataSourceRequest]DataSourceRequest request, AllowancePerEmployee allowancePerEmployee)
        //{
        //    ModelState.Clear();
        //    var exist = db.AllowancePerEmployees.Where(x => x.EmployeeId == allowancePerEmployee.EmployeeId &&x.AllowanceDefinationId == allowancePerEmployee.AllowanceDefinationId);

        //    if (exist.Any())
        //    {
        //        ModelState.AddModelError("", "This allowance already exists for the selected employee.");
        //        return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        //    }

        //    var allowancetype = db.AllowanceDefinations.Find(allowancePerEmployee.AllowanceDefinationId);

        //    if (allowancetype != null)
        //    {
        //        var allowanceSettings = db.AllowanceSettings
        //            .Where(x => x.AllowanceType == allowancetype.AllowanceType)
        //            .ToList();

        //        var placement = db.Placements
        //            .FirstOrDefault(x => x.EmployeeId == allowancePerEmployee.EmployeeId && x.isCurrent);

        //        if (allowanceSettings.Any())
        //        {
        //            // 1. Head-only restriction
        //            if (allowanceSettings.Any(x => x.IsOnlyForHead))
        //            {
        //                if (placement != null && !placement.IsHead)
        //                {
        //                    ModelState.AddModelError("", "This allowance type is restricted to employees in head positions only.");
        //                    return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        //                }
        //            }

        //            // 2. Job title restriction
        //            if (placement != null)
        //            {
        //                bool isAllowedJobTitle = allowanceSettings.Any(x => x.LookupId == placement.LookupId);

        //                if (!isAllowedJobTitle)
        //                {
        //                    var jobTitle = db.Lookups.Find(placement.LookupId);

        //                    if (jobTitle != null)
        //                    {
        //                        ModelState.AddModelError("", $"This allowance type is only permitted for employees with the job title: {jobTitle.Description}.");
        //                    }

        //                    return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        //                }
        //            }
        //        }
        //    }



        //    if (ModelState.IsValid)
        //    {
        //        var entity = new AllowancePerEmployee
        //        {
        //            EmployeeId = allowancePerEmployee.EmployeeId,
        //            AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
        //            Remark = allowancePerEmployee.Remark,
        //            DateCreated = DateTime.Now,
        //            LastModified = allowancePerEmployee.LastModified,
        //            CreatedBy = User.Identity.Name,
        //            ModifiedBy = allowancePerEmployee.ModifiedBy,
        //            EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == allowancePerEmployee.EmployeeId).FirstOrDefault().EmployeeCode,
        //            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
        //            From = allowancePerEmployee.From,
        //            To = allowancePerEmployee.To,
        //            Pend = allowancePerEmployee.Pend,
        //        };

        //        db.AllowancePerEmployees.Add(entity);
        //        db.SaveChanges();
        //        allowancePerEmployee.AllowanceId = entity.AllowanceId;

        //        #region operation log
        //        UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"create allowance  {entity.AllowanceId} of employee {entity.EmployeeCode} ", Modules.Payroll);
        //        #endregion
        //    }

        //    return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        //}
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult AllowancePerEmployees_Update([DataSourceRequest]DataSourceRequest request, AllowancePerEmployee allowancePerEmployee)
        //{
        //    ModelState.Clear();

        //    var exist = db.AllowancePerEmployees.Where(x => x.EmployeeId == allowancePerEmployee.EmployeeId)
        //                                        .Where(x => x.AllowanceDefinationId == allowancePerEmployee.AllowanceDefinationId)
        //                                        .Where(x => x.AllowanceId != allowancePerEmployee.AllowanceId);
        //    if (exist.Any())
        //    {
        //        ModelState.AddModelError("Exisit", "The allowance already exists for this employee");
        //        return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));

        //    }
        //    var allowancetype = db.AllowanceDefinations.Find(allowancePerEmployee.AllowanceDefinationId);

        //    if (allowancetype != null)
        //    {

        //        var allowanceSettings = db.AllowanceSettings.Where(x => x.AllowanceType == allowancetype.AllowanceType).ToList();
        //        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == allowancePerEmployee.EmployeeId && x.isCurrent && !x.IsHead);
        //        if (allowanceSettings.Any())
        //        {
        //            if (allowanceSettings.Where(x => x.IsOnlyForHead).Any() && placement != null && !placement.IsHead)
        //            {

        //                ModelState.AddModelError("", "This Allowance Type is only allowed for head only ");
        //                return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));

        //            }
        //            if (placement != null && !allowanceSettings.Where(x => x.LookupId == placement.LookupId).Any())
        //            {
        //                if (allowanceSettings.Where(x => x.LookupId == placement.LookupId).Any())
        //                {
        //                    var jobTitle = db.Lookups.Find(placement.LookupId);
        //                    if (jobTitle != null)
        //                    {
        //                        ModelState.AddModelError("", "This Allowance Type is only allowed for   " + jobTitle.Description + "jobtitle only"); ;
        //                        return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));

        //                    }

        //                }

        //            }
        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var entity = new AllowancePerEmployee
        //        {
        //            EmployeeId = allowancePerEmployee.EmployeeId,
        //            AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
        //            AllowanceId = allowancePerEmployee.AllowanceId,
        //            Remark = allowancePerEmployee.Remark,
        //            DateCreated = allowancePerEmployee.DateCreated,
        //            LastModified = DateTime.Now,
        //            CreatedBy = allowancePerEmployee.CreatedBy,
        //            ModifiedBy = User.Identity.Name,
        //            EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == allowancePerEmployee.EmployeeId).FirstOrDefault().EmployeeCode,
        //            OrgBranchName = allowancePerEmployee.OrgBranchName,
        //            From = allowancePerEmployee.From,
        //            To = allowancePerEmployee.To,
        //            Pend = allowancePerEmployee.Pend,

        //        };

        //        db.AllowancePerEmployees.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        #region operation log
        //        UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"updated allowance  {entity.AllowanceId} of employee {entity.EmployeeCode} ", Modules.Payroll);
        //        #endregion
        //    }

        //    return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AllowancePerEmployee> allowanceesPerEmployees)
        {
            var results = new List<AllowancePerEmployee>();
            var allowancePerEmployees = allowanceesPerEmployees as IList<AllowancePerEmployee> ?? allowanceesPerEmployees.ToList();

            if (ModelState.IsValid)
            {
                foreach (var allowancePerEmployee in allowancePerEmployees)
                {
                    // Determine which definition IDs to loop through (supports multi-select array or single fallback)
                    var defIds = (allowancePerEmployee.AllowanceDefinationIds != null && allowancePerEmployee.AllowanceDefinationIds.Length > 0)
                        ? allowancePerEmployee.AllowanceDefinationIds
                        : new[] { allowancePerEmployee.AllowanceDefinationId };

                    foreach (var defId in defIds)
                    {
                        // Check if this allowance definition already exists for this specific employee
                        var exist = db.AllowancePerEmployees
                            .Any(x => x.EmployeeId == allowancePerEmployee.EmployeeId &&
                                      x.AllowanceDefinationId == defId);

                        if (exist)
                        {
                            continue; // Skip duplicates silently or handle per your workflow
                        }

                        var firstOrDefault = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == allowancePerEmployee.EmployeeId);
                        if (firstOrDefault != null)
                        {
                            var entity = new AllowancePerEmployee
                            {
                                EmployeeId = allowancePerEmployee.EmployeeId,
                                AllowanceDefinationId = defId,
                                Remark = allowancePerEmployee.Remark,
                                DateCreated = DateTime.Now,
                                LastModified = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = User.Identity.Name,
                                EmployeeCode = firstOrDefault.EmployeeCode,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                From = allowancePerEmployee.From,
                                To = allowancePerEmployee.To,
                                Pend = allowancePerEmployee.Pend,
                            };

                            db.AllowancePerEmployees.Add(entity);
                            db.SaveChanges();

                            var resultRow = new AllowancePerEmployee
                            {
                                AllowanceId = entity.AllowanceId,
                                EmployeeId = entity.EmployeeId,
                                AllowanceDefinationId = entity.AllowanceDefinationId,
                                Remark = entity.Remark,
                                DateCreated = entity.DateCreated,
                                LastModified = entity.LastModified,
                                CreatedBy = entity.CreatedBy,
                                ModifiedBy = entity.ModifiedBy,
                                EmployeeCode = entity.EmployeeCode,
                                OrgBranchName = entity.OrgBranchName,
                                From = entity.From,
                                To = entity.To,
                                Pend = entity.Pend
                            };

                            results.Add(resultRow);

                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance per employee", "Success", User.Identity.Name, $"created allowance {entity.AllowanceId} for employee {entity.EmployeeCode}", Modules.Payroll);
                            #endregion
                        }
                    }
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeAllowance_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AllowancePerEmployee> allowanceesPerEmployees)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                foreach (var al in allowanceesPerEmployees)
                {
                    var checkexistance = db.PayrollProcessViewModel.FirstOrDefault(x => x.EmployeeId == al.EmployeeId);

                    if (checkexistance != null)
                    {
                        //var per = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == checkexistance.EmployeeId);
                        //if (per != null)
                        //{
                        //    message = message + " " + per.EmployeeCode;
                        //}

                        al.Remark = "Can't delete this row this is already proccessed";

                        db.Entry(al).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {

                        var allowance = db.AllowancePerEmployees.Find(al.AllowanceId);

                        if (allowance != null)
                        {
                            db.Entry(allowance).State = EntityState.Deleted;
                            db.SaveChanges();
                        }

                        //var entity = new AllowancePerEmployee
                        //{
                        //    EmployeeId = allowancePerEmployee.EmployeeId,
                        //    AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
                        //    AllowanceId = allowancePerEmployee.AllowanceId,
                        //    Remark = allowancePerEmployee.Remark,
                        //    DateCreated = allowancePerEmployee.DateCreated,
                        //    LastModified = allowancePerEmployee.LastModified,
                        //    CreatedBy = allowancePerEmployee.CreatedBy,
                        //    ModifiedBy = allowancePerEmployee.ModifiedBy,
                        //    EmployeeCode = allowancePerEmployee.EmployeeCode,
                        //    OrgBranchName = allowancePerEmployee.OrgBranchName,
                        //    From = allowancePerEmployee.From,
                        //    To = allowancePerEmployee.To,
                        //    Pend = allowancePerEmployee.Pend,
                        //};
                        #region delete log

                        string EmployeeCode = "";
                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == al.EmployeeId);
                        if (person != null)
                        {
                            EmployeeCode = person.EmployeeCode;
                        }

                        UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee allowance information of employee " + EmployeeCode, Modules.Payroll);

                        #endregion
                    }
                }
                //db.AllowancePerEmployees.Attach(entity);
                //db.AllowancePerEmployees.Remove(entity);
                //db.SaveChanges();
            }

            //if (!string.IsNullOrEmpty(message))
            //{
            //    string Message = "Can't delete employees " + message + "b/s their allowance are already processed";
            //    return new JavaScriptResult() { Script = "alert('" + Message + "');" };
            //   // ModelState.AddModelError("", "Can't delete employees " + message + "b/s their allowance are already processed");
            //}

            return Json(new[] { allowanceesPerEmployees }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowancePerEmployees_Destroy([DataSourceRequest]DataSourceRequest request, AllowancePerEmployee allowancePerEmployee)
        {
            ModelState.Clear();
            //var checkexistance = db.PayrollProcessViewModel.FirstOrDefault(x => x.EmployeeId == allowancePerEmployee.EmployeeId);

            //if (checkexistance != null)
            //{
            //    ModelState.AddModelError("", "You can not delete this allowance is already processed!!");
            //}

            if (ModelState.IsValid)
            {

                var entity = new AllowancePerEmployee
                {
                    EmployeeId = allowancePerEmployee.EmployeeId,
                    AllowanceDefinationId = allowancePerEmployee.AllowanceDefinationId,
                    AllowanceId = allowancePerEmployee.AllowanceId,
                    Remark = allowancePerEmployee.Remark,
                    DateCreated = allowancePerEmployee.DateCreated,
                    LastModified = allowancePerEmployee.LastModified,
                    CreatedBy = allowancePerEmployee.CreatedBy,
                    ModifiedBy = allowancePerEmployee.ModifiedBy,
                    EmployeeCode = allowancePerEmployee.EmployeeCode,
                    OrgBranchName = allowancePerEmployee.OrgBranchName,
                    From = allowancePerEmployee.From,
                    To = allowancePerEmployee.To,
                    Pend = allowancePerEmployee.Pend,
                };
                #region delete log

                string EmployeeCode = "";
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == allowancePerEmployee.EmployeeId);
                if (person != null)
                {
                    EmployeeCode = person.EmployeeCode;
                }

                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee allowance information of employee " + EmployeeCode, Modules.Payroll);

                #endregion

                db.AllowancePerEmployees.Attach(entity);
                db.AllowancePerEmployees.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { allowancePerEmployee }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Employee Allowance");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Employee Allowance");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
