using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Kendo.Mvc;
using ExceedERP.Helpers.HR;
using System.Web;
using System.IO;
using System.Runtime.Caching;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.HRM.Allowance;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRPlacementAdmin,HrmConstants.HRPlacementUser)]
    public class EmployeePlacmentWithOJController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
          
            PopulateViewData();
            return View();
        }
        public ActionResult PlacementWithSwap()
        {
            PopulateViewData();
            return View();
        }
        public ActionResult SalaryIncreement()
        {
          
            PopulateViewData();
            return View();
        }
        public void ClearStepCache()
        {
            const string cacheKey = CacheKeys.Steps;
            MemoryCache.Default.Remove(cacheKey);
        }

        private void PopulateViewData()
        {
            
            ViewData["RankList"] = CacheData.GetRankCaches().Select(
               i => new
               {
                   RankName = i.Name,
                   RankId = i.Id,
               }).AsQueryable(); ;
            ViewData["StepList"] =  CacheData.GetStepCaches().Select(
               i => new
               {
                   StepName = i.Name,
                   StepId = i.Id,

               }).AsQueryable();
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
               i => new
               {
                   FullNameAmharic = i.AmharicName,
                   FullNameaEnglish = i.Name,
                   EmployeeId = i.Id,
                   HeadId = i.Id
               }).AsQueryable();
            ViewData["jobTitles"] = CacheData.GetJobTitlesCaches().Select(
               i => new
               {
                   Description = i.Name,
                   LookupId = i.Id,
               }).AsQueryable();
            ViewData["AllJobTitles"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                                         || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                         || x.LookupType == LookUpTypes.Jobtitle_External_Experience).Select(
                i => new
                {
                    Description = i.Description,
                    LookupId = i.LookupId,
                }).ToList();
            ViewData["branches"] = CacheData.GetBranchCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BranchId = i.Id,
               }).AsQueryable(); ;
         
            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
               }).AsQueryable(); ; ;
            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable(); 
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["isManual"] = db.GeneralJobtitleSettings.FirstOrDefault()?.IsManual;
            ViewData["employeeType"] = db.EmployeeTypes
              .Select(x => new { x.EmployeeTypeId, x.Name });

            ViewData["placementChecker"] = db.PlacementWorkflowSettings.FirstOrDefault()?.RequireChecker??false;

            ViewData["autoEmployeeReg"] = db.Configurations.Any(x => x.Reference == "BiometricEmployeeAutoRegistration" && x.Attribute == "BiometricEmployeeAutoRegistration");
            ViewData["autoEmployeeExempt"] = db.Configurations.Any(x => x.Reference == "BiometricEmployeeAutoExempt" && x.Attribute == "BiometricEmployeeAutoExempt");
        }

        [HttpPost]
        public JsonResult Refresh()
        {
            var allPlacements = db.Placements.Where(x => x.isCurrent).ToList();

            // Preload all needed salaries once (optional optimization)
            var rankStepPairs = allPlacements
                .Select(p => new { p.RankId, p.StepId })
                .Distinct()
                .ToList();

            var salaryLookup = db.SalariesJobTitles
                .Where(s => rankStepPairs.Any(r => r.RankId == s.RankId && r.StepId == s.StepId))
                .ToList(); 
            
            // load once
            foreach (var p in allPlacements)
            {
                var salary = salaryLookup
                    .FirstOrDefault(s => s.RankId == p.RankId && s.StepId == p.StepId);

                if (salary == null) continue;
                if (salary.Salary <= 0) continue;
                if (p.Salary == salary.Salary) continue;

                p.Salary = salary.Salary;
                p.ModifiedBy = User.Identity.Name;
                p.LastModified = DateTime.Now;

                db.Entry(p).State = EntityState.Modified;
            }

            db.SaveChanges(); // Save ONCE
            return Json(new
            {
                Message = "Salary has successfully been updated.",
                Status = "Okay"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEmployeeSalaryExisted()
        {

            var allPlacements = db.Placements.Where(x => x.isCurrent).ToList();

            var salaryLookup = db.SalariesJobTitles.ToList(); // load once

            foreach (var p in allPlacements)
            {
                var salary = salaryLookup
                    .FirstOrDefault(s => s.RankId == p.RankId && s.StepId == p.StepId);

                if (salary == null) continue;
                if (salary.Salary <= 0) continue;
                if (p.Salary == salary.Salary) continue;

                p.Salary = salary.Salary;
                p.ModifiedBy = User.Identity.Name;
                p.LastModified = DateTime.Now;

                db.Entry(p).State = EntityState.Modified;
            }

            db.SaveChanges(); // Save ONCE
            return Json(new
            {
                Message = "Salary has successfully been updated.",
                Status = "Okay"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEmployeeSalary(DateTime? newAssignmentDate = null)
        {
            var allPlacements = db.Placements.Where(x => x.isCurrent).ToList();

            if (!allPlacements.Any())
            {
                return Json(new
                {
                    Message = "No current placements found to process.",
                    Status = "Warning"
                }, JsonRequestBehavior.AllowGet);
            }

            var salaryLookup = db.SalariesJobTitles.ToList(); // Load lookup once
            int updatedCount = 0;

            foreach (var p in allPlacements)
            {
                var salary = salaryLookup
                    .FirstOrDefault(s => s.RankId == p.RankId && s.StepId == p.StepId);

                if (salary == null || salary.Salary <= 0)
                    continue;

                bool isModified = false;

                // 1. Check & Update Salary
                if (p.Salary != salary.Salary)
                {
                    p.Salary = salary.Salary;
                    isModified = true;
                }

                // 2. Optional: Update Assignment Date if provided by user
                if (p.Salary != salary.Salary && newAssignmentDate.HasValue && p.AssignmentDate != newAssignmentDate.Value)
                {
                    p.AssignmentDate = newAssignmentDate.Value;
                    p.AssignmentDateAmharic = HrHelper.GetECDate( newAssignmentDate.Value);
                    isModified = true;
                }

                // Apply audit properties only if changes took place
                if (isModified)
                {
                    p.ModifiedBy = User.Identity.Name;
                    p.LastModified = DateTime.Now;

                    db.Entry(p).State = EntityState.Modified;
                    updatedCount++;
                }
            }

            db.SaveChanges(); // Batch Save ONCE

            return Json(new
            {
                Message = $"Operation completed successfully. {updatedCount} placement(s) updated.",
                Status = "Okay"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEmployeeProbationStatus()
        {
            List<int> myBranchIds = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            var today = DateTime.Today;

            // Load once
            var probationSettings = db.ProbationsSettings.ToList();
            var defaultSetting = probationSettings.FirstOrDefault(x => x.ResponsibilityLevel == null)
                                  ?? probationSettings.FirstOrDefault();

            var placements = db.Placements
                .Where(x => x.isCurrent)
                .Where(b => myBranchIds.Contains(b.BranchId))
                .ToList();

            var placementLookup = placements
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            // Employees in probation only
            var employees = db.Person_Employee
                .Where(x =>
                    x.IsActive &&
                    x.Status == Core.Domain.Common.EmployeeStatus.PROBATION)
                .ToList();

            // Most recent probation evaluation per employee, used to decide Fit/UnFit
            var latestPerformance = db.ProbationPerformances
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.ProbationPerformanceId).FirstOrDefault());

            int promotedCount = 0;
            int terminatedCount = 0;
            int autoPromotedNoEvaluationCount = 0;
            int mismatchFixedCount = 0;

            foreach (var emp in employees)
            {
                // Current placement
                placementLookup.TryGetValue(emp.EmployeeId, out var placement);
                if (placement == null)
                    continue;

                // Data mismatch: the current placement no longer says Probation, so the employee's
                // Status is a stale/mistaken value - sync it directly, no day/evaluation check needed.
                if (placement.EmployementType != Core.Domain.Common.EmployeeStatus.PROBATION)
                {
                    emp.Status = placement.EmployementType;
                    emp.IsProbationEnd = true;
                    emp.ProhabitionState = 0;
                    emp.LastModified = DateTime.Now;
                    emp.ModifiedBy = User.Identity.Name;
                    db.Entry(emp).State = EntityState.Modified;

                    mismatchFixedCount++;
                    continue;
                }

                // Responsibility level specific probation setting (falls back to the default row)
                var setting = probationSettings.FirstOrDefault(x =>
                    x.ResponsibilityLevel == placement.ResponsibilityLevel) ?? defaultSetting;

                int probationDays = setting != null && setting.ProbationDays > 0 ? setting.ProbationDays : 60;
                decimal minimumFitValue = setting?.MinimumFitValue ?? 0;

                // Total employed days
                int totalDays = (today - emp.HiredDate.Date).Days;

                if (totalDays < probationDays)
                    continue; // still within the probation period

                // Probation period has ended. If no evaluation was recorded, default to Fit instead
                // of leaving the employee stuck on probation indefinitely.
                latestPerformance.TryGetValue(emp.EmployeeId, out var performance);
                bool isFit = performance == null || performance.Result >= minimumFitValue;

                if (isFit)
                {
                    if (performance == null)
                        autoPromotedNoEvaluationCount++;

                    // Fit - confirm as permanent
                    emp.Status = Core.Domain.Common.EmployeeStatus.PERMANENT;
                    emp.IsProbationEnd = true;
                    emp.ProhabitionState = 0; // clears the grid's stale "in probation" (yellow) background
                    emp.LastModified = DateTime.Now;
                    emp.ModifiedBy = User.Identity.Name;
                    db.Entry(emp).State = EntityState.Modified;

                    placement.EmployementType = EmployeeStatus.PERMANENT;
                    placement.IsApprove = true;
                    placement.isCurrent = true;
                    placement.LastModified = DateTime.Now;
                    placement.ModifiedBy = User.Identity.Name;
                    db.Entry(placement).State = EntityState.Modified;

                    promotedCount++;
                }
                else
                {
                    // UnFit - take them out of the probation tab and record the termination
                    emp.IsActive = false;
                    emp.IsPlaced = false;
                    emp.LastModified = DateTime.Now;
                    emp.ModifiedBy = User.Identity.Name;
                    db.Entry(emp).State = EntityState.Modified;

                    placement.IsApprove = false;
                    placement.isCurrent = false;
                    placement.LastModified = DateTime.Now;
                    placement.ModifiedBy = User.Identity.Name;
                    db.Entry(placement).State = EntityState.Modified;

                    db.Terminations.Add(new ExceedERP.Core.Domain.HRM.EmployeeDetail.Termination
                    {
                        EmployeeId = emp.EmployeeId,
                        LookupId = placement.LookupId,
                        Reason = "probation unfit",
                        ApplicationDate = DateTime.Now,
                        TerminationDate = DateTime.Now,
                        Remark = "Unfit",
                        TerminationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                        ApplicationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name,
                        IsOnlineApproved = true,
                        IsDirect = true,
                        IsForciblyTerminated = true,
                        IsDepartmentSendForApproval = true,
                        IsApproved = true,
                        EmployeeCode = emp.EmployeeCode,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                    });

                    terminatedCount++;
                }
            }

            db.SaveChanges();

            return Json(new
            {
                Message = $"{promotedCount} employee(s) confirmed permanent ({autoPromotedNoEvaluationCount} without an evaluation on file), {terminatedCount} unfit employee(s) removed from probation, {mismatchFixedCount} mismatched placement(s) corrected.",
                Status = "Okay"
            }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult UpdateEmployeeProbationStatus()
        //{

        //    List<int> myBranchIds = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

        //    var today = DateTime.Today;

        //    // Load once
        //    var probationSettings = db.ProbationsSettings.ToList();

        //    var placements = db.Placements
        //        .Where(x => x.isCurrent)
        //        .Where(b=>myBranchIds.Contains(b.BranchId))
        //        .ToList();

        //    var placementLookup = placements
        //        .GroupBy(x => x.EmployeeId)
        //        .ToDictionary(x => x.Key, x => x.FirstOrDefault());

        //    // Employees in probation only
        //    var employees = db.Person_Employee
        //        .Where(x =>
        //            x.IsActive &&
        //            x.Status == Core.Domain.Common.EmployeeStatus.PROBATION)
        //        .ToList();

        //    int updatedCount = 0;

        //    foreach (var emp in employees)
        //    {
        //        // Current placement
        //        placementLookup.TryGetValue(emp.EmployeeId, out var placement);

        //        // Default probation days
        //        int probationDays = probationSettings
        //            .FirstOrDefault()?.ProbationDays ?? 60;

        //        // Responsibility level specific probation
        //        if (placement != null)
        //        {
        //            var setting = probationSettings.FirstOrDefault(x =>
        //                x.ResponsibilityLevel == placement.ResponsibilityLevel);

        //            if (setting != null && setting.ProbationDays > 0)
        //            {
        //                probationDays = setting.ProbationDays;
        //            }
        //        }

        //        // Total employed days
        //        int totalDays = (today - emp.HiredDate.Date).Days;

        //        bool probationCompleted = totalDays >= probationDays;

        //        // Update status
        //        if (probationCompleted)
        //        {
        //            emp.Status = Core.Domain.Common.EmployeeStatus.PERMANENT;
        //            emp.LastModified = DateTime.Now;
        //            emp.ModifiedBy = User.Identity.Name;

        //            db.Entry(emp).State = EntityState.Modified;

        //            updatedCount++;
        //        }
        //    }

        //    db.SaveChanges();

        //    return Json(new
        //    {
        //        Message = updatedCount + " employee(s) probation status updated successfully.",
        //        Status = "Okay"
        //    }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetEmployees([DataSourceRequest] DataSourceRequest request)
        {
            db = new ExceedDbContext();
            var employees = inputs.GetEmployeesForPlacment(User.Identity.Name);

            return Json(employees.ToDataSourceResult(request));
        }
        public ActionResult Employee_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = inputs.GetEmployeesForPlacment(User.Identity.Name);
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.PersonId.ToString() == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStructure()
        {
            db = new ExceedDbContext();
            var orgs = db.MainCompanyStructures.ToList();


            return Json(orgs.Select(p => new { OrgStructureId = p.OrgStructureId, Name = p.Name }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult UpdateParentStructure()
        {

            var placements = db.Placements;
            foreach(var placement in placements)
            {
                var OrgStructure  = db.MainCompanyStructures.Find(placement.OrgStructureId);
                if (OrgStructure !=null)
                {
                    placement.ParentOrgStructureId = OrgStructure.Parent;
                    db.Entry(placement).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
           
            return Json("Successfully Updated!", JsonRequestBehavior.AllowGet);
            
        }
        public JsonResult UpdateEmployeeAge()
        {
            try
            {
                // Load current placements
                var currentPlacements = db.Placements
                    .Where(x => x.isCurrent)
                    .ToList();

                // Load employees referenced by those placements
                var employeeIds = currentPlacements
                    .Select(p => p.EmployeeId)
                    .Distinct()
                    .ToList();

                var employees = db.Person_Employee
                    .Where(e => employeeIds.Contains(e.EmployeeId))
                    .ToDictionary(e => e.EmployeeId);

                if (!currentPlacements.Any())
                {
                    return Json(new
                    {
                        success = true,
                        message = "No current placements found."
                    }, JsonRequestBehavior.AllowGet);
                }

                foreach (var placement in currentPlacements)
                {
                    if (employees.TryGetValue(placement.EmployeeId, out var person))
                    {
                        if (person.DateOfBirth != null)
                        {
                            var today = DateTime.Today;
                            var age = today.Year - person.DateOfBirth.Year;

                            if (string.IsNullOrEmpty(person.DateOfBirthAm))
                                person.DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                                    person.DateOfBirth.Day,
                                    person.DateOfBirth.Month,
                                    person.DateOfBirth.Year
                                );
                            if (string.IsNullOrEmpty(placement.AssignmentDateAmharic))
                                placement.AssignmentDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(
                                    placement.AssignmentDate.Day,
                                    placement.AssignmentDate.Month,
                                    placement.AssignmentDate.Year
                                );

                            if (person.DateOfBirth.Date > today.AddYears(-age))
                                age--;

                            //  Update only the Age property, not the whole entity
                            placement.Age = age;
                            db.Entry(placement).Property(x => x.Age).IsModified = true;

                            person.Age = age;
                            db.Entry(person).Property(x => x.Age).IsModified = true;
                        }
                    }
                }

                // Save all changes in a single transaction
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Employee ages updated successfully."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // 🔍 Capture detailed validation error info
                var errorDetails = new System.Text.StringBuilder();

                foreach (var eve in ex.EntityValidationErrors)
                {
                    errorDetails.AppendLine($"Entity: {eve.Entry.Entity.GetType().Name}, State: {eve.Entry.State}");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errorDetails.AppendLine($"- Property: {ve.PropertyName}, Error: {ve.ErrorMessage}");
                    }
                    errorDetails.AppendLine();
                }

                return Json(new
                {
                    success = false,
                    message = "Validation failed for one or more entities.",
                    details = errorDetails.ToString()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Catch-all for other unexpected issues
                return Json(new
                {
                    success = false,
                    message = "Unexpected error occurred.",
                    details = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetBoss(int empId)
        {
            var boss = "";
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if (placement != null&& placement.IsHead)
            {
                var bossPlace = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.ParentOrgStructureId && x.isCurrent && x.IsHead);
                if (bossPlace != null)
                {
                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossPlace.EmployeeId);
                    if (profile != null)
                    {
                        boss = profile.FullNameaEnglish;
                    }
                }

            }
            else if(placement != null && !placement.IsHead)
            {
                var bossPlace = db.Placements.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId &&  x.isCurrent && x.IsHead);
                if (bossPlace != null)
                {
                    var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == bossPlace.EmployeeId);
                    if (profile != null)
                    {
                        boss = profile.FullNameaEnglish;
                    }
                }
            }

            return boss;
        }
        public ActionResult UpdateBoss()
        {
            var placements = db.Placements
                .Where(x => x.isCurrent && x.IsHead)
                //.Where(x => x.HeadId == 0)
                .ToList();
            foreach (var place in placements)
            {
                var subordinates = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(place.EmployeeId).Where(x => x != place.EmployeeId);
                foreach (var empId in subordinates)
                {
                    var employee = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
                    if (employee != null)
                    {
                        employee.HeadId = place.EmployeeId;
                        db.Entry(employee).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return View("Success");
        }
        public ActionResult UpdateZeroSalary()
        {

            var placements = db.Placements.Where(x => x.isCurrent).Where(x => x.Salary == 0).ToList();

            foreach (var placement in placements)
            {
                
                var salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == placement.RankId && s.StepId == placement.StepId);

                if (salary != null)
                {
                    placement.Salary = salary.Salary;
                    db.Entry(placement).State = EntityState.Modified;
                    db.SaveChanges();
                }


            }
            return View("Success");
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo =
                            db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
        private IQueryable<Placement> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<Placement> person_employee)
        {
            IQueryable<Placement> person_employees = new Placement[] { }.AsQueryable(); ;
            if (filters.Any())
            {
                string searchedValue = "";

                dd: if (filters.Any())
                {

                    foreach (var filter in filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "JobTitleName")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (person_employees.Any())
                                {
                                    var person =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.JobTitleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                value.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person;
                                }
                                else
                                {
                                    var person =
                                           person_employee.ToList().Where(
                                               j =>
                                                   j.JobTitleName.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                   value.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person_employees.Union(person).AsQueryable();
                                }

                            }

                        }
                        else if (filter is CompositeFilterDescriptor)
                        {

                            filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
                            goto dd;
                            //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

                        }

                    }

                }
            }
            return person_employees.AsQueryable();
        }
        public string GetFullName(int empId)
        {
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (employee != null)
            {
                return employee.FullNameaEnglish;
            }

            return "";
        }
        private bool CheckToBePromoted(int employeeId)
        {
            var placements = db.Placements
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.AssignmentDate)
                .Select(p => new { p.StepId, p.AssignmentDate })
                .ToList();

            if (!placements.Any())
                return false;

            var latestStepId = placements.First().StepId;
            var dateLimit = DateTime.Now.AddYears(-2);

            // Find the most recent placement where the StepId was different (i.e., a promotion)
            var lastStepChange = placements
                .Skip(1) // skip the current
                .FirstOrDefault(p => p.StepId != latestStepId);

            if (lastStepChange == null)
            {
                // No step change ever — they’ve always had the current step
                return placements.Last().AssignmentDate <= dateLimit;
            }

            // They had a step change — check how long ago it was
            return lastStepChange.AssignmentDate <= dateLimit;
        }
        // it cakes 
        private IList<int> CheckToBePromotedEmployees()
        {
            var dateLimit = DateTime.Now.AddYears(-2);

            // Get employee IDs with current placements
            var currentEmployeeIds = db.Placements
                .Where(p => p.isCurrent)
                .Select(p => p.EmployeeId)
                .Distinct()
                .ToList();

            var eligibleEmployeeIds = new List<int>();

            foreach (var employeeId in currentEmployeeIds)
            {
                var placements = db.Placements
                    .Where(p => p.EmployeeId == employeeId)
                    .OrderByDescending(p => p.AssignmentDate)
                    .Select(p => new { p.StepId, p.AssignmentDate })
                    .ToList();

                if (!placements.Any())
                    continue;

                var latestStepId = placements.First().StepId;

                // Find the most recent placement with a different StepId (i.e., the last time the step changed)
                var lastStepChange = placements
                    .Skip(1) // skip the current
                    .FirstOrDefault(p => p.StepId != latestStepId);

                bool isEligible;

                if (lastStepChange == null)
                {
                    // No prior step change — use earliest available placement
                    isEligible = placements.Last().AssignmentDate <= dateLimit;
                }
                else
                {
                    isEligible = lastStepChange.AssignmentDate <= dateLimit;
                }

                if (isEligible)
                {
                    eligibleEmployeeIds.Add(employeeId);
                }
            }

            return eligibleEmployeeIds;
        }
        private JobTitleMapping GetJobtitleCode(int id)
        {
            if (id != 0)
            {
                db = new ExceedDbContext();
                var title = db.JobTitleMapping.Where(x => x.JobTitleMappingId == id).FirstOrDefault();
                return title;
            }
            return new JobTitleMapping();

        }

        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
        public ActionResult PlacementToBePromoteds_Read([DataSourceRequest]DataSourceRequest request,string stepId,string gradeId)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var empIds = CheckToBePromotedEmployees();

            // Start with base query
            IQueryable<Placement> placements = db.Placements
                .Where(x => x.isCurrent)
                .Where(x => myBranches.Contains(x.BranchId))
                .Where(x => empIds.Contains(x.EmployeeId));

            // Optional business unit filter
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                placements = placements.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }

            // Optional step filter
            if (!string.IsNullOrEmpty(stepId) && int.TryParse(stepId, out int parsedStepId))
            {
                placements = placements.Where(x => x.StepId == parsedStepId);
            }

            // Optional grade/rank filter
            if (!string.IsNullOrEmpty(gradeId) && int.TryParse(gradeId, out int parsedGradeId))
            {
                placements = placements.Where(x => x.RankId == parsedGradeId); // Assuming RankId is GradeId
            }
            DataSourceResult result = placements.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                HeadId = placement.HeadId,
                EmployeeTypeId = placement.EmployeeTypeId?? placement.Employee?.EmployeeTypeId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                AssignmentDate = placement.AssignmentDate,
                SalarChangeDate = placement.SalarChangeDate,
                SalarChangeDateAmharic = placement.SalarChangeDateAmharic,
                OrgStructureId = placement.OrgStructureId,
                ParentOrgStructureId = placement.ParentOrgStructureId,
                LookupId = placement.LookupId,
                MappingId = placement.MappingId,
                JobTitleName = GetTitleName(placement.LookupId),
                ReportsToMappingId = placement.ReportsToMappingId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                BranchId = placement.BranchId,
                Salary = placement.Salary,
                Department = placement.Department,
                ReportsTo = placement.ReportsTo,
                AssignmentType = placement.AssignmentType,
                ResponsibilityLevel = placement.ResponsibilityLevel,
                Remark = placement.Remark,
                DateCreated = placement.DateCreated,
                LastModified = placement.LastModified,
                CreatedBy = placement.CreatedBy,
                ModifiedBy = placement.ModifiedBy,
                IsEditable = placement.IsEditable,
                isCurrent = placement.isCurrent,
                AssignmentDateAmharic = placement.AssignmentDateAmharic ,
                //AssignmentDateAmharic = placement.AssignmentDateAmharic + " (" + placement.AssignmentDate.ToString("MMMM dd, yyyy") + ")",
                EmployeeCode = placement.EmployeeCode,
                EmployementType = placement.EmployementType,
                IsPend = placement.IsPend,
                BusinessUnitId = placement.BusinessUnitId,
                IsApprove = placement.IsApprove,
                IsAttendanceExempt = placement.IsAttendanceExempt,
                IsToBePromoted = CheckToBePromoted(placement.EmployeeId),
                FullName = GetFullName(placement.EmployeeId),
                HireDate = HrHelper.GetHireDate(placement.EmployeeId),
                Age = EmployeeHelper.GetAge(placement.EmployeeId)
            });

            return Json(result);
        }


        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
        public ActionResult Placements_Read_Prev([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<Placement> placements = db.Placements.Where(x => !x.IsTemporary)
                                                            .Where(x => x.isCurrent || !x.IsApprove)
                                                            .Where(x => mybraches.Contains(x.BranchId));
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                placements = placements.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
         
            DataSourceResult result = placements.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                EmployeeTypeId = placement.EmployeeTypeId ?? placement.Employee?.EmployeeTypeId,
                HeadId = placement.HeadId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                IsAttendanceExempt = placement.IsAttendanceExempt,
                AssignmentDate = placement.AssignmentDate,
                SalarChangeDate = placement.SalarChangeDate,
                SalarChangeDateAmharic = placement.SalarChangeDateAmharic,
                OrgStructureId = placement.OrgStructureId,
                ParentOrgStructureId = placement.ParentOrgStructureId,
                LookupId = placement.LookupId,
                MappingId = placement.MappingId,
                JobTitleName = GetTitleName(placement.LookupId),
                ReportsToMappingId = placement.ReportsToMappingId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                BranchId = placement.BranchId,
                Salary = placement.Salary,
                Department = placement.Department,
                ReportsTo = placement.ReportsTo,
                AssignmentType = placement.AssignmentType,
                ResponsibilityLevel = placement.ResponsibilityLevel,
                Remark = placement.Remark,
                DateCreated = placement.DateCreated,
                LastModified = placement.LastModified,
                CreatedBy = placement.CreatedBy,
                ModifiedBy = placement.ModifiedBy,
                isCurrent = placement.isCurrent,
                IsEditable = placement.IsEditable,
                AssignmentDateAmharic = placement.AssignmentDateAmharic + " (" + placement.AssignmentDate.ToString("MMMM dd, yyyy") + ")",
                //AssignmentDateAmharic = placement.AssignmentDateAmharic+"("+placement.AssignmentDate.ToString("dd.MM.yyyy")+")",
                EmployeeCode = placement.EmployeeCode,
                EmployementType = placement.EmployementType,
                IsPend = placement.IsPend,
                BusinessUnitId = placement.BusinessUnitId,
                IsApprove = placement.IsApprove,
                IsToBePromoted = CheckToBePromoted(placement.EmployeeId),
                FullName = GetFullName(placement.EmployeeId),
                HireDate = HrHelper.GetHireDate(placement.EmployeeId),
                Age = EmployeeHelper.GetAge(placement.EmployeeId)
            });

            return Json(result);
        }
        public ActionResult Placements_Read([DataSourceRequest] DataSourceRequest request)
        {
            bool useEthDate = true; // default

            try
            {
                useEthDate = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            }
            catch (Exception ex)
            {
                // Optional: Log the error if needed
                // logger.LogError(ex, "Failed to read DatePreferenceSettings");
                useEthDate = true;
            }
            string username = User.Identity.Name;
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(username);
            List<int> myBusinessUnits = GlobalHelper.IsMultipleBusinessUnit()
                ? UserInformationHelper.GetUserBusinessUnits(username)
                : new List<int>();

            var placements = db.Placements
                .Where(x => !x.IsTemporary &&(x.isCurrent || !x.IsApprove) &&
                            myBranches.Contains(x.BranchId));

            if (myBusinessUnits.Any())
            {
                placements = placements.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
            var result = placements
                .Select(x => new
                {
                    x.PlacementId,
                    x.EmployeeId,
                    EmployeeCode=x.Employee.EmployeeCode,
                    x.HeadId,
                    x.ImmediateSupervisor,
                    x.IsHead,
                    x.AssignmentDate,
                    x.SalarChangeDate,
                    x.SalarChangeDateAmharic,
                    x.OrgStructureId,
                    x.ParentOrgStructureId,
                    x.LookupId,
                    x.MappingId,
                    x.JobTitleName,
                    //JobTitleName = x.lo.Description, // Assuming Lookup is navigational property
                    x.ReportsToMappingId,
                    x.RankId,
                    x.StepId,
                    x.BranchId,
                    x.Salary,
                    x.Department,
                    x.ReportsTo,
                    x.AssignmentType,
                    x.ResponsibilityLevel,
                    x.Remark,
                    x.DateCreated,
                    x.LastModified,
                    x.CreatedBy,
                    x.ModifiedBy,
                    x.isCurrent,
                    x.IsEditable,
                    x.AssignmentDateAmharic,
                    x.EmployementType,
                    x.IsPend,
                    x.BusinessUnitId,
                    x.IsApprove,
                    x.Age,
                    x.StepName,
                    x.IsChecked,
                    x.CheckedBy,
                    x.CheckedDate,
                    x.HaveFixedAsset,
                    x.IsAttendanceExempt,
                    x.ApprovedByEmpId,
                    x.NextSalaryChangeDateAmharic,
                    x.NextSalaryChangeDate,
                    HireDate= x.Employee.HiredDateAmharic,
                    EmployeeTypeId = x.Employee.EmployeeTypeId,
                    HireDateGorg = x.Employee.HiredDate,
                    Gender = x.Employee.Gender.ToString().ToLower(),
                    CheckCEO = x.HeadId==null&&x.IsHead,
                   
                })
                .ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Create([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            if (orgStructure != null)
            {
                placement.ParentOrgStructureId = orgStructure.Parent;
            }
            #region validation
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(placement.AssignmentDateAmharic))
                {
                    ModelState.AddModelError("", "placement Assignment Date is required");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
                placement.AssignmentDate = HrHelper.ParseEthiopicDate(placement.AssignmentDateAmharic);
            }
            else
            {
                placement.AssignmentDateAmharic = HrHelper.GetECDate(placement.AssignmentDate);
            }
            var palcements = db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.AssignmentDate).ToList();
            var currentpalcement = palcements.FirstOrDefault(x => x.isCurrent&&x.IsApprove);
            var checkNotApproved = palcements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && !x.IsApprove);
            if (checkNotApproved != null)
            {
                ModelState.AddModelError("", "This employee already have not approved placement");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }
            var checkAssignment = palcements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (checkAssignment != null && placement.AssignmentType == AssignmentType.NEW)
            {
                ModelState.AddModelError("", "You can not assign this employee as a new assignment!!");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }
            var checkexistance =
                    palcements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId 
                              && x.OrgStructureId == placement.OrgStructureId 
                              && x.BusinessUnitId == placement.BusinessUnitId 
                              && x.ParentOrgStructureId == placement.ParentOrgStructureId 
                              && x.LookupId == placement.LookupId
                              && x.BranchId == placement.BranchId 
                              &&x.RankId == placement.RankId 
                              && x.StepId == placement.StepId 
                              &&x.Salary == placement.Salary 
                              &&x.AssignmentDate == placement.AssignmentDate 
                              && x.AssignmentType == placement.AssignmentType);

            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This employee is already assigened ");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }
            //if (placement.AssignmentDate.Date > DateTime.Now.Date)
            //{
            //    ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");
            //    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            //}

            var workunitHead = palcements.FirstOrDefault(x => x.BranchId == placement.BranchId 
                                                    && x.OrgStructureId == placement.OrgStructureId 
                                                    && x.isCurrent
                                                    && x.IsHead 
                                                    && x.EmployeeId !=placement.EmployeeId
                                                    && x.PlacementId !=placement.PlacementId);
            if (placement.IsHead && workunitHead != null)
            {
                ModelState.AddModelError("", "This Work unit already have Head");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }

            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (emp != null)
            {
                if (placement.AssignmentDate.Date < emp.HiredDate.Date)
                {
                    ModelState.AddModelError("", "assignment date can't less than hire date");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
                
            }


            var empPlace = palcements.Where(x => x.AssignmentType == AssignmentType.PROMOTION).ToList().OrderByDescending(x => x.AssignmentDate);
            if (empPlace.Any())
            {
                var placeFirstOrDefault = empPlace.FirstOrDefault();
                if (placeFirstOrDefault != null)
                {
                    //var diff = DateTime.Today.Subtract(placeFirstOrDefault.AssignmentDate).Days;
                    //if (diff < 365 && placement.AssignmentType == AssignmentType.YEARLY_PROMOTION)
                    //{
                    //    ModelState.AddModelError("", "you were promoted before, you can not promoted two times a year ");
                    //    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                    //}
                }
            }

            var yearProSetting = db.Configurations.FirstOrDefault(c => c.Category == "Promotion" && c.Reference == "YEARLY_PROMOTION_PER_YEAR");
            if (yearProSetting != null)
            {
                var yearPros = palcements.Where(x => x.AssignmentType == AssignmentType.YEARLY_PROMOTION).OrderByDescending(x => x.AssignmentDate).ToList();
                if (yearPros.Any())
                {
                    var placeFirstOrDefault = empPlace.FirstOrDefault();
                    if (placeFirstOrDefault != null)
                    {
                        var days = Convert.ToInt32(yearProSetting.CurrentValue) * 365;
                        var diff = DateTime.Today.Subtract(placeFirstOrDefault.AssignmentDate).Days;

                        if (diff < days && placement.AssignmentType == AssignmentType.YEARLY_PROMOTION)
                        {
                            ModelState.AddModelError("", "you were promoted yearly promotion before, you can not promoted two times a year ");
                            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
            }

            #endregion

            if (placement.EmployementType == EmployeeStatus.CONTRACT)
            {
                var contractLast = db.Contract.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (contractLast != null && contractLast.To.Date <= placement.AssignmentDate.Date)
                {
                    ModelState.AddModelError("", "First Extend Contract date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
            }
            if ( placement.EmployementType == EmployeeStatus.FREELANCER)
            {
                var freelancerLast = db.Freelancers.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (freelancerLast != null && freelancerLast.To <= placement.AssignmentDate)
                {
                    ModelState.AddModelError("", "First Extend Freelancer date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
            }
            var manPower = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == placement.BranchId
                                                                &&x.BusinessUnitId==placement.BusinessUnitId 
                                                                && x.OrgStructureId == placement.OrgStructureId
                                                                && x.LookupId == placement.LookupId);
            if (manPower != null && manPower.PlanType == JobTitlePlan.BOUNDED)
            {
                var allPlacementOnThisJob = db.Placements.Where(x => x.BranchId == manPower.BranchId
                                                                        &&x.BusinessUnitId==manPower.BusinessUnitId 
                                                                        && x.OrgStructureId == manPower.OrgStructureId 
                                                                        && x.LookupId == manPower.LookupId 
                                                                        && x.EmployeeId != placement.EmployeeId 
                                                                        && x.isCurrent).ToList();
                if (allPlacementOnThisJob.Any())
                {
                    int totalPlaced = allPlacementOnThisJob.Count();
                    if (totalPlaced >= manPower.Quntity)
                    {
                        ModelState.AddModelError("", "This Jobtitle is set to Bounded so you can't assign more");
                        return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            if(placement.EmployementType == EmployeeStatus.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employement status Type");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }
            if (placement.AssignmentType == AssignmentType.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employee Assignment Type");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }
            if (placement.ResponsibilityLevel == ResponsibilityLevel.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employee Responsibility Level");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }


            bool isJobTitleIncluded =
                 db.SalarySettings
                   .Select(x => x.IsJobtitleIncludedInSalary)
                   .FirstOrDefault();

            var salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == placement.RankId &&
                                                                    (
                                                                        s.StepId == placement.StepId ||
                                                                        (isJobTitleIncluded && s.LookupId == placement.LookupId)
                                                                    )
                                                                );

            // If placement salary is not set
            if (placement.Salary <= 0 || salary == null)
            {
                if (salary == null)
                {
                    ModelState.AddModelError("", "The salary setting for the specified rank and step or JobTitle is not configured. Please update the salary settings.");
                }
                else
                {
                    ModelState.AddModelError("", "Salary cannot be negative or zero.");
                }
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }
            var currentsalary = currentpalcement?.Salary ?? 0;
            // If a valid salary is available, update fields
            if (salary != null && salary.Salary > 0 && currentsalary < salary.Salary)
            {
                DateTime today = DateTime.Now;

                // Determine initial salary change date
                DateTime salaryChangeDate = currentpalcement?.SalarChangeDate
                                          ?? placement?.SalarChangeDate
                                          ?? placement.AssignmentDate;

                // If the placement type is a PROMOTION, prefer the promotion's change date or today
                if (placement.AssignmentType == AssignmentType.PROMOTION)
                {
                    salaryChangeDate = today;
                }

                // Calculate the next salary change (2 years after the last change)
                DateTime nextSalaryChange = salaryChangeDate.AddYears(2);

                // If salaryChangeDate and nextSalaryChange are both in the past, update them from NextSalaryChangeDate
                if (salaryChangeDate.Date <= today.Date && nextSalaryChange.Date <= today.Date)
                {


                    DateTime initialDate = nextSalaryChange;

                    DateTime latestSalaryChange = initialDate;

                    DateTime oneMonthAhead = today.AddMonths(1);

                    while (latestSalaryChange.AddYears(2) <= oneMonthAhead.Date)
                    {
                        latestSalaryChange = latestSalaryChange.AddYears(2);
                    }

                    salaryChangeDate = latestSalaryChange;
                    nextSalaryChange = salaryChangeDate.AddYears(2);
                    
                }

                // Apply updates to the placement
                placement.SalarChangeDate = salaryChangeDate;
                placement.NextSalaryChangeDate = nextSalaryChange;

                // Set corresponding Amharic dates using Ethiopic calendar conversion
                placement.SalarChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(
                    salaryChangeDate.Day, salaryChangeDate.Month, salaryChangeDate.Year);

                placement.NextSalaryChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(
                    nextSalaryChange.Day, nextSalaryChange.Month, nextSalaryChange.Year);

                // Update the salary
                placement.Salary = salary.Salary;
            }
            else if(placement.SalarChangeDate.HasValue||!string.IsNullOrEmpty(placement.SalarChangeDateAmharic))
            {
                if (datePreference)
                {
                    if (!string.IsNullOrEmpty(placement.SalarChangeDateAmharic))
                    {
                        var splitDate = placement.SalarChangeDateAmharic.Split('/');
                        placement.SalarChangeDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                        placement.NextSalaryChangeDate = placement.SalarChangeDate.Value.AddYears(2);
                        placement.NextSalaryChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placement.NextSalaryChangeDate.Value.Day, placement.NextSalaryChangeDate.Value.Month, placement.NextSalaryChangeDate.Value.Year);

                    }



                }
                else if(placement.SalarChangeDate.HasValue)
                {

                    placement.SalarChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placement.SalarChangeDate.Value.Day, placement.SalarChangeDate.Value.Month, placement.SalarChangeDate.Value.Year);
                    placement.NextSalaryChangeDate = placement.SalarChangeDate.Value.AddYears(2);
                    placement.NextSalaryChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placement.NextSalaryChangeDate.Value.Day, placement.NextSalaryChangeDate.Value.Month, placement.NextSalaryChangeDate.Value.Year);


                }
               
            }
            else 
            {
                placement.SalarChangeDate = currentpalcement?.SalarChangeDate;

                placement.NextSalaryChangeDate = currentpalcement?.NextSalaryChangeDate;
                placement.SalarChangeDateAmharic = currentpalcement?.SalarChangeDateAmharic;
                placement.NextSalaryChangeDateAmharic = currentpalcement?.NextSalaryChangeDateAmharic;
            }

            if (placement.EmployeeId == placement.HeadId)
            {
                ModelState.AddModelError("", "Please select a valid head —> you cannot assign yourself as your own head.!");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.HeadId == null || placement.HeadId == 0)
            {
                placement.HeadId = EmployeeHelper.GetSupervisorEmployeeId(
                    placement.EmployeeId,
                    placement.OrgStructureId,
                    placement.BranchId,
                    placement.BusinessUnitId
                );

                if (!placement.CheckCEO&&(placement.HeadId == null || placement.HeadId == 0))
                {
                    ModelState.AddModelError("", "Please select your corresponding head. It was not found!");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
               
            }
            var placementHead = db.Placements
                                .FirstOrDefault(x => x.isCurrent 
                                                    && x.EmployeeId == placement.HeadId 
                                                    && x.HeadId == placement.EmployeeId);

            if (placementHead != null)
            {
                ModelState.AddModelError("", "Invalid selection: You cannot be assigned as the head of your own head. Please select a valid corresponding head.");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }


            if (ModelState.IsValid)
            {
                int i = 0;

                var currentPlacement = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.isCurrent);

                if (currentPlacement != null)
                {
                   
                    PlacementAfterSystem_Create(currentPlacement, placement.AssignmentDate);
                }
                placement.PreparedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                //placement.Salary = HrHelper.GetSalaryFromSalarySetting(placement.StepId, placement.RankId, placement.LookupId);
                placement.MappingId = 0;
                placement.ReportsToMappingId = 0;
                placement.JobTitleName = GetTitleName(placement.LookupId);
                placement.DateCreated = DateTime.Now;
                placement.CreatedBy = User.Identity.Name;
                placement.isCurrent = false; // this will be true after approval
                placement.IsApprove = false;
                placement.IsPend = false;
                placement.IsTemporary = false;
                placement.Index = i;
                placement.EmployeeCode = placement.Employee?.EmployeeCode??placement.EmployeeCode;
                placement.EmployeeTypeId = placement.EmployeeTypeId
                                           ?? placement.Employee?.EmployeeTypeId;
                placement.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                var rank = db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                var step = db.Steps.FirstOrDefault(x => x.StepId == placement.StepId);
                var entity = new Placement
                {
                    EmployeeId = placement.EmployeeId,
                    HeadId = placement.HeadId,
                    ImmediateSupervisor = placement.ImmediateSupervisor,
                    IsHead = placement.IsHead,
                    CheckCEO = placement.CheckCEO,
                    AssignmentDate = placement.AssignmentDate,
                    MappingId = placement.MappingId,
                    RankId = placement.RankId,
                    StepId = placement.StepId,
                    LookupId = placement.LookupId,
                    OrgStructureId = placement.OrgStructureId,
                    ParentOrgStructureId = placement.ParentOrgStructureId,
                    EmployeeTypeId = placement.EmployeeTypeId,
                    Salary = placement.Salary,
                    SalarChangeDate = placement.SalarChangeDate,
                    SalarChangeDateAmharic = placement.SalarChangeDateAmharic,
                    NextSalaryChangeDate = placement.NextSalaryChangeDate,
                    NextSalaryChangeDateAmharic = placement.NextSalaryChangeDateAmharic,
                    ReportsTo = placement.ReportsTo,
                    JobTitleName = placement.JobTitleName,
                    Department = placement.ReportsTo,
                    ReportsToMappingId = placement.ReportsToMappingId,
                    AssignmentType = placement.AssignmentType,
                    ResponsibilityLevel = placement.ResponsibilityLevel,
                    BranchId = placement.BranchId,
                    BusinessUnitId = placement.BusinessUnitId,
                    Remark = placement.Remark,
                    DateCreated = placement.DateCreated,
                    LastModified = placement.LastModified,
                    CreatedBy = placement.CreatedBy,
                    ModifiedBy = placement.ModifiedBy,
                    isCurrent = placement.isCurrent, // this will be true after approval
                    IsApprove = placement.IsApprove,
                    IsPend = placement.IsPend,
                    Index = placement.Index,
                    RankName = rank?.RankName,
                    StepName = step?.StepName,
                    IsEditable = placement.IsEditable,
                    IsTemporary = placement.IsTemporary,
                    IsToBePromoted = placement.IsToBePromoted,
                    ApprovedByEmpId = placement.ApprovedByEmpId,
                    HaveFixedAsset = placement.HaveFixedAsset,
                    Age = EmployeeHelper.GetAge(placement.EmployeeId),
                    EmployementType = placement.EmployementType,
                    AssignmentDateAmharic = placement.AssignmentDateAmharic,
                    EmployeeCode = placement.EmployeeCode,
                    OrgBranchName = placement.OrgBranchName,
                    PreparedByEmpId = placement.PreparedByEmpId

                };
                //var entity = GetMappedObject(placement);
                db.Placements.Add(entity);
                if (emp != null)
                {
                    emp.IsActive = true;
                    emp.BranchId = entity.BranchId;
                    emp.Status = entity.EmployementType;
                    emp.EmployeeTypeId = entity.EmployeeTypeId;
                    emp.OrgBranchName = entity.BranchId.ToString();
                    db.Entry(emp).State = EntityState.Modified;
                    
                }

                //check configurtion and  mapp allownace 
                var allowanceSetting = db.AllowanceGenerationSettings.FirstOrDefault();
                if (allowanceSetting != null && allowanceSetting.AllowGenerationForNewEmployee)
                {
                    AddEmployeeAllowance(currentPlacement, placement);

                }
                db.SaveChanges();
                placement.PlacementId = entity.PlacementId;
                if (entity.IsHead)
                {
                    var subordinateEmployeeIds = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(entity.EmployeeId);
                    foreach (var subEmployeeId in subordinateEmployeeIds)
                    {
                        var changeHead = db.Placements.FirstOrDefault(x => x.EmployeeId == subEmployeeId && x.isCurrent);
                        if (changeHead != null)
                        {
                            changeHead.HeadId = entity.EmployeeId;
                            db.Entry(changeHead).State = EntityState.Modified;

                        }

                    }
                    db.SaveChanges();
                }
                else
                {
                    //if he is head of any employee they haveto assign the correct head
                    var placedEmployees = db.Placements.Where(x => x.HeadId == entity.EmployeeId && x.isCurrent).ToList();
                    foreach (var placedEmployee in placedEmployees)
                    {

                        var headId = EmployeeHelper.GetSupervisor(placedEmployee.EmployeeId);
                        placedEmployee.HeadId = headId;
                        db.Entry(placedEmployee).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                //Add To EployeeCared
                UserInformationHelper.UpdateDefalutBranchAndBusinessunit(placement.EmployeeId, placement.BranchId, placement.BusinessUnitId);
                #region operation log

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);

                if (person != null)
                {
                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "Employee Placement",
                        "Success",
                        User.Identity.Name,
                        $"Placement Created for Employee [{person.EmployeeCode}] by [{User.Identity.Name}]. " +
                        $"Branch: {placement.BranchId}, " +
                        $"JobTitle: {placement.LookupId}, " +
                        $"Rank: {placement.RankId}, " +
                        $"Salary: {placement.Salary:N2}",
                        Modules.HRM);
                }

                #endregion


            }

            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }      

        [AuthorizeRoles(HrmConstants.HRPlacementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Update([DataSourceRequest]DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            if (placement == null)
            {
                ModelState.AddModelError("", "Placement data is missing.");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }

            var branchId = placement.BranchId;
            var rankId = placement.RankId;
            var jobTitleId = placement.LookupId;
            var orgStructure = db.MainCompanyStructures.Find(placement.OrgStructureId);
            if (orgStructure != null)
            {
                placement.ParentOrgStructureId = orgStructure.Parent;
            }
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(placement.AssignmentDateAmharic))
                {
                    ModelState.AddModelError("", "placement Assignment Date is required");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
                }
                placement.AssignmentDate = HrHelper.ParseEthiopicDate(placement.AssignmentDateAmharic);



            }
            else
            {
                placement.AssignmentDateAmharic = HrHelper.GetECDate(placement.AssignmentDate);
            }

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
            if (employee != null)
            {
                if (placement.AssignmentDate.Date < employee.HiredDate.Date)
                {
                    ModelState.AddModelError("", "assignment date can't less than hire date");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            //if (placement.AssignmentDate.Date > DateTime.Now.Date)
            //{

            //    ModelState.AddModelError("", "Assigment date should be less or equal to the current date!!");
            //    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            //}
            var pp = db.Placements.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId
                                                    && x.isCurrent
                                                    && x.PlacementId != placement.PlacementId);
            if (pp != null && placement.isCurrent)
            {
                ModelState.AddModelError("", "this employee have other current placement ");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }

            var workunitHead = db.Placements.FirstOrDefault(x => x.PlacementId != placement.PlacementId 
                                    && x.BranchId == placement.BranchId 
                                    && x.EmployeeId != placement.EmployeeId 
                                    && x.OrgStructureId == placement.OrgStructureId 
                                    && x.IsHead && x.isCurrent);
            if (placement.IsHead 
                && workunitHead != null)
            {
                ModelState.AddModelError("", "This Workunit already have Head");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }

          
            if ( placement.EmployementType == EmployeeStatus.CONTRACT)
            {
                var contractLast = db.Contract.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (contractLast != null && contractLast.To.Date <= placement.AssignmentDate.Date)
                {
                    ModelState.AddModelError("", "First Extend Contract date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            if ( placement.EmployementType == EmployeeStatus.FREELANCER)
            {
                var freelancerLast = db.Freelancers.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.To).ToList().FirstOrDefault();
                if (freelancerLast != null && freelancerLast.To.Date <= placement.AssignmentDate.Date)
                {
                    ModelState.AddModelError("", "First Extend Freelancer date for this employee ");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            var oldPlacement = db.Placements.Find(placement.PlacementId);
            var changes = new List<string>();

            if (oldPlacement.BranchId != placement.BranchId)
                changes.Add($"Branch: {oldPlacement.BranchId} → {placement.BranchId}");

            if (oldPlacement.LookupId != placement.LookupId)
                changes.Add($"JobTitle: {oldPlacement.LookupId} → {placement.LookupId}");

            if (oldPlacement.RankId != placement.RankId)
                changes.Add($"Rank: {oldPlacement.RankId} → {placement.RankId}");

            if (oldPlacement.StepId != placement.StepId)
                changes.Add($"Step: {oldPlacement.StepId} → {placement.StepId}");

            if (oldPlacement.Salary != placement.Salary)
                changes.Add($"Salary: {oldPlacement.Salary:N2} → {placement.Salary:N2}");

            if (oldPlacement.HeadId != placement.HeadId)
                changes.Add($"Head: {oldPlacement.HeadId} → {placement.HeadId}");
            // Check allowance regeneration configuration
            var allowanceSetting = db.AllowanceGenerationSettings.FirstOrDefault();

            if (allowanceSetting != null
                && allowanceSetting.AllowRegenerationForUpdatedEmployee
                && oldPlacement != null
                && (
                       oldPlacement.BranchId != branchId
                       || oldPlacement.RankId != rankId ||
                       oldPlacement.LookupId != jobTitleId
                   ))
            {
                AddEmployeeAllowance(oldPlacement, placement);
            }


            if (!(oldPlacement.OrgStructureId ==placement.OrgStructureId
                && oldPlacement.BusinessUnitId==placement.BusinessUnitId
                && oldPlacement.BranchId==placement.BranchId
                && oldPlacement.LookupId == placement.LookupId))
            {
                
                var manPower = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == placement.BranchId
                                                  && x.OrgStructureId == placement.OrgStructureId
                                                   && x.BusinessUnitId == placement.BusinessUnitId
                                                   && x.BranchId == placement.BranchId
                                                    && x.OrgStructureId == placement.OrgStructureId
                                                    && x.LookupId == placement.LookupId);
                if (manPower != null && manPower.PlanType == JobTitlePlan.BOUNDED)
                {
                    var allPlacementOnThisJob = db.Placements.Where(x => x.BranchId == manPower.BranchId
                                            && x.OrgStructureId == manPower.OrgStructureId
                                            && x.LookupId == manPower.LookupId
                                            && x.isCurrent
                                            //&& x.PlacementId != placement.PlacementId
                                            ).ToList();
                    if (allPlacementOnThisJob.Any())
                    {
                        int totalPlaced = allPlacementOnThisJob.Count();
                        if (totalPlaced >= manPower.Quntity)
                        {
                            ModelState.AddModelError("", "This Jobtitle is set to Bounded so you can't assign more");
                            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                        }
                    }
                }

            }
            if (placement.EmployementType == EmployeeStatus.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employement status Type");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.AssignmentType == AssignmentType.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employee Assignment Type");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            if (placement.ResponsibilityLevel == ResponsibilityLevel.NOTSELECTED)
            {
                ModelState.AddModelError("", "Please Select Employee Responsibility Level");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
           
            // Retrieve the original salary from the database using placement's primary key
            var originalSalary = db.Placements
                .Where(p => p.PlacementId == placement.PlacementId)
                .Select(p => p.Salary)
                .FirstOrDefault();
            DateTime salaryChangeDate = placement?.SalarChangeDate ?? placement.AssignmentDate;
            bool isJobTitleIncluded =
                db.SalarySettings
                  .Select(x => x.IsJobtitleIncludedInSalary)
                  .FirstOrDefault();

            var salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == placement.RankId &&
                                                                    (
                                                                        s.StepId == placement.StepId ||
                                                                        (isJobTitleIncluded && s.LookupId == placement.LookupId)
                                                                    )
                                                                );


            if (placement.Salary <= 0|| salary==null)
            {
                if (salary == null)
                {
                    ModelState.AddModelError("","The salary setting for the specified rank and step or JobTitle is not configured. Please update the salary settings.");
                }
                else
                {
                    ModelState.AddModelError("", "Salary cannot be negative or zero.");
                }
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }

            if (salary.Salary > originalSalary) // Detect manual salary modification
            {
                DateTime today = DateTime.Now;
                // If the placement type is a PROMOTION, prefer the promotion's change date or today
                if (placement.AssignmentType == AssignmentType.PROMOTION)
                {
                    salaryChangeDate =  today;
                }

                // Calculate the next salary change (2 years after the last change)
                DateTime nextSalaryChange = salaryChangeDate.AddYears(2);

                // If salaryChangeDate and nextSalaryChange are both in the past, update them from NextSalaryChangeDate
                if (salaryChangeDate.Date <= today.Date && nextSalaryChange.Date <= today.Date)
                {
                    if (placement.NextSalaryChangeDate.HasValue)
                    {
                        DateTime initialDate = placement.NextSalaryChangeDate.Value;
                        DateTime latestSalaryChange = initialDate;

                        while (latestSalaryChange.AddYears(2) <= today.Date)
                        {
                            latestSalaryChange = latestSalaryChange.AddYears(2);
                        }

                        salaryChangeDate = latestSalaryChange;
                        nextSalaryChange = salaryChangeDate.AddYears(2);
                    }


                }

                // Apply updates to the placement
                placement.SalarChangeDate = salaryChangeDate;
                placement.NextSalaryChangeDate = nextSalaryChange;

                // Set corresponding Amharic dates using Ethiopic calendar conversion
                placement.SalarChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(
                    salaryChangeDate.Day, salaryChangeDate.Month, salaryChangeDate.Year);

                placement.NextSalaryChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(
                    nextSalaryChange.Day, nextSalaryChange.Month, nextSalaryChange.Year);



            }
            else if (placement.SalarChangeDate.HasValue || !string.IsNullOrEmpty(placement.SalarChangeDateAmharic))
            {
                if (datePreference)
                {
                    if (!string.IsNullOrEmpty(placement.SalarChangeDateAmharic))
                    {
                        var splitDate = placement.SalarChangeDateAmharic.Split('/');
                        placement.SalarChangeDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                        placement.NextSalaryChangeDate = placement.SalarChangeDate.Value.AddYears(2);
                        placement.NextSalaryChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placement.NextSalaryChangeDate.Value.Day, placement.NextSalaryChangeDate.Value.Month, placement.NextSalaryChangeDate.Value.Year);

                    }



                }
                else if (placement.SalarChangeDate.HasValue)
                {

                    placement.SalarChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placement.SalarChangeDate.Value.Day, placement.SalarChangeDate.Value.Month, placement.SalarChangeDate.Value.Year);
                    placement.NextSalaryChangeDate = placement.SalarChangeDate.Value.AddYears(2);
                    placement.NextSalaryChangeDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(placement.NextSalaryChangeDate.Value.Day, placement.NextSalaryChangeDate.Value.Month, placement.NextSalaryChangeDate.Value.Year);


                }

            }
           
            // Update the salary to include demotion
            placement.Salary = salary.Salary;

            if (placement.HeadId == null||placement.HeadId ==0)
            {
                // placement.HeadId = EmployeeHelper.GetSupervisor(placement.EmployeeId);
                placement.HeadId = EmployeeHelper.GetSupervisorEmployeeId(placement.EmployeeId,placement.OrgStructureId, placement.BranchId,placement.BusinessUnitId);
                if (!placement.CheckCEO&&(placement.HeadId == 0||placement.HeadId ==null))
                {
                    ModelState.AddModelError("", "Please Select Your Corresponding  Head the system doesn't found  !");
                    return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

                }
            }
            var placementHead = db.Placements
                              .FirstOrDefault(x => x.isCurrent
                                                  && x.EmployeeId == placement.HeadId
                                                  && x.HeadId == placement.EmployeeId);
            if (placementHead != null)
            {
                ModelState.AddModelError("", "Invalid selection: You cannot be assigned as the head of your own head. Please select a valid corresponding head.");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
            }
            if (placement.EmployeeId == placement.HeadId)
            {
                ModelState.AddModelError("", "Please select a valid head —> you cannot assign yourself as your own head.!");
                return Json(new[] { placement }.ToDataSourceResult(request, ModelState));

            }
            placement.EmployeeTypeId = placement.EmployeeTypeId ?? employee?.EmployeeTypeId ?? null;

            if (ModelState.IsValid)
            {
                placement.PreparedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                placement.JobTitleName = GetTitleName(placement.LookupId);
                placement.LastModified = DateTime.Now;
                placement.ModifiedBy = User.Identity.Name;
                placement.EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == placement.EmployeeId)?.EmployeeCode;
                placement.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                var entity = GetMappedObject(placement);
                if (oldPlacement != null)
                {

                    // Map values manually (or with AutoMapper)
                    db.Entry(oldPlacement).CurrentValues.SetValues(placement);
                    db.SaveChanges();
                    if (employee != null)
                    {
                        employee.OrgBranchName = placement.BranchId.ToString();
                        employee.EmployeeTypeId = placement.EmployeeTypeId?? employee.EmployeeTypeId;
                        employee.ModifiedBy = User.Identity.Name;
                        employee.LastModified = DateTime.Now;
                        employee.BranchId = placement.BranchId;
                        employee.Status = placement.EmployementType;

                        db.Entry(employee).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    placement.PlacementId = entity.PlacementId;
                    if (entity.IsHead)
                    {
                        var subordinateEmployeeIds = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(oldPlacement.EmployeeId).Where(x => x != entity.EmployeeId);
                        foreach (var subEmployeeId in subordinateEmployeeIds)
                        {
                            var changeHead = db.Placements
                                               .FirstOrDefault(x => x.EmployeeId == subEmployeeId
                                                                 && x.isCurrent);

                            if (changeHead != null)
                            {
                                changeHead.HeadId = entity.EmployeeId;
                                db.Entry(changeHead).State = EntityState.Modified;

                            }

                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        // if employees were under him
                        var placedEmployees = db.Placements.Where(x => x.HeadId == oldPlacement.EmployeeId && x.isCurrent).ToList();
                        foreach (var placedEmployee in placedEmployees)
                        {

                            var headId = EmployeeHelper.GetSupervisor(placedEmployee.EmployeeId);
                            placedEmployee.HeadId = headId;
                            db.Entry(placedEmployee).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                    }
                    //PerapreEmployeeAllowance(entity.EmployeeId, placement.LookupId);
                    UserInformationHelper.UpdateDefalutBranchAndBusinessunit(placement.EmployeeId, placement.BranchId, placement.BusinessUnitId);
                    
                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                        string changeDetails = changes.Any()
                            ? string.Join(", ", changes)
                            : "No significant changes";

                        UserHelper.OperationLog(
                            Request.UserHostAddress,
                            "Employee Placement",
                            "Success",
                            User.Identity.Name,
                            $"Placement of Employee [{person.EmployeeCode}] updated by [{User.Identity.Name}]. Changes: {changeDetails}",
                            Modules.HRM);
                    }
                   
                }

                #endregion
                

            }

            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Placements_Destroy([DataSourceRequest] DataSourceRequest request, Placement placement)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                try
                {
                    var place = db.Placements.Find(placement.PlacementId);
                    if (place != null)
                    {
                        db.Placements.Remove(place);
                        db.SaveChanges();

                        UserHelper.DeleteLog(
                                         Request.UserHostAddress,
                                         "Employee Placement",
                                         "Success",
                                         User.Identity.Name,
                                         $"Placement deleted for Employee [{placement.EmployeeCode}] and placement Id: {placement.PlacementId} by [{User.Identity.Name}]. " +
                                         $"BranchId: {placement.BranchId}, " +
                                         $"JobTitleId: {placement.LookupId}, " +
                                         $"RankId: {placement.RankId}, " +
                                         $"Salary: {placement.Salary:N2}"
                                     );
                        // Re-activate the last placement
                        var checkAssignment = db.Placements
                            .Where(x => x.EmployeeId == placement.EmployeeId)
                            .OrderByDescending(x => x.PlacementId)
                            .FirstOrDefault();

                        if (checkAssignment != null)
                        {
                            checkAssignment.isCurrent = true;
                            db.Entry(checkAssignment).State = EntityState.Modified;
                            db.SaveChanges();

                            var branch = checkAssignment.BranchId.ToString();
                            var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);

                            if (employee != null)
                            {
                                employee.IsActive = true;
                                employee.Status = checkAssignment.EmployementType;
                                employee.OrgBranchName = branch;
                                employee.ModifiedBy = User.Identity.Name;
                                employee.LastModified = DateTime.Now;

                                db.Entry(employee).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Failed", User.Identity.Name, $"Delete failed: {ex.Message}");
                    ModelState.AddModelError("", "Error deleting placement: " + ex.Message);
                }
            }

            return Json(new[] { placement }.ToDataSourceResult(request, ModelState));
        }

        private Placement GetMappedObject(Placement placement)
        {
            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
            var rank = db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
            var step = db.Steps.FirstOrDefault(x => x.StepId == placement.StepId);
            var entity = new Placement
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                EmployeeTypeId = placement.EmployeeTypeId,
                HeadId = placement.HeadId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                AssignmentDate = placement.AssignmentDate,
                MappingId = placement.MappingId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                LookupId = placement.LookupId,
                OrgStructureId = placement.OrgStructureId,
                ParentOrgStructureId = placement.ParentOrgStructureId,
                Salary = placement.Salary,
                SalarChangeDate = placement.SalarChangeDate,
                SalarChangeDateAmharic = placement.SalarChangeDateAmharic,
                NextSalaryChangeDate = placement.NextSalaryChangeDate,
                NextSalaryChangeDateAmharic = placement.NextSalaryChangeDateAmharic,
                ReportsTo = placement.ReportsTo,
                JobTitleName = placement.JobTitleName,
                Department = placement.ReportsTo,
                ReportsToMappingId =placement.ReportsToMappingId,
                AssignmentType = placement.AssignmentType,
                ResponsibilityLevel = placement.ResponsibilityLevel,
                BranchId = placement.BranchId,
                BusinessUnitId = placement.BusinessUnitId,
                Remark = placement.Remark,
                DateCreated = placement.DateCreated,
                LastModified = placement.LastModified,
                CreatedBy = placement.CreatedBy,
                ModifiedBy = placement.ModifiedBy,
                isCurrent = placement.isCurrent, // this will be true after approval
                IsApprove = placement.IsApprove,
                CheckCEO = placement.CheckCEO,
                IsPend = placement.IsPend,
                Index = placement.Index,
                RankName = rank?.RankName,
                StepName = step?.StepName,
                IsEditable = placement.IsEditable,
                IsTemporary = placement.IsTemporary,
                IsToBePromoted = placement.IsToBePromoted,
                ApprovedByEmpId = placement.ApprovedByEmpId,
                HaveFixedAsset = placement.HaveFixedAsset,
                Age = EmployeeHelper.GetAge(placement.EmployeeId),
                EmployementType = placement.EmployementType,
                AssignmentDateAmharic = placement.AssignmentDateAmharic,
                EmployeeCode = placement.EmployeeCode,
                OrgBranchName = placement.OrgBranchName,
                PreparedByEmpId = placement.PreparedByEmpId

            };
            return entity;
        }

        private void AddEmployeeAllowance(Placement oldPlacement, Placement placement)
        {

            var now = DateTime.Now;
            var user = User.Identity.Name;

            //  Get DESERT allowance definition IDs ONCE
            var desertDefIds = db.AllowanceDefinations
                .Where(x => x.AllowanceType == AllowanceType.DESERT)
                .Select(x => x.AllowanceDefinationId)
                .ToList();

            if (!desertDefIds.Any())
                return;

            // Load all relevant mappings ONCE
            var mappings = db.AllowanceMappings
                .Where(x => desertDefIds.Contains(x.AllowanceDefinationId))
                .ToList();

            // Helper function for priority matching
            //AllowanceMapping FindBestMapping(int? lookupId, int? branchId, int? rankId)
            //{
            //    return mappings.FirstOrDefault(x => x.LookupId == lookupId)?? mappings.FirstOrDefault(x => x.LookupId == lookupId)
            //}


            //  Pend OLD allowances (single query)
            //if(oldPlacement != null)
            //{
            //    //previous mappings
            //    var prevMapping = FindBestMapping(oldPlacement.LookupId, oldPlacement.BranchId, oldPlacement.RankId);
            //    if (prevMapping != null)
            //    {
            //        var oldAllowances = db.AllowancePerEmployees
            //            .Where(a => a.EmployeeId == placement.EmployeeId &&
            //                        a.AllowanceDefinationId == prevMapping.AllowanceDefinationId)
            //            .ToList();

            //        foreach (var old in oldAllowances)
            //        {
            //            old.Pend = true;
            //            old.LastModified = now;
            //            old.ModifiedBy = user;
            //            db.Entry(old).State = EntityState.Modified;

            //        }
            //    }

            //}
          
            //// Get current  mappings
            //var currentMapping = FindBestMapping(placement.LookupId, placement.BranchId, placement.RankId);
            //if (currentMapping == null)
            //    return;

            //  Check existing NEW allowance
            //var existing = db.AllowancePerEmployees
            //    .FirstOrDefault(a => a.EmployeeId == placement.EmployeeId &&
            //                         a.AllowanceDefinationId == currentMapping.AllowanceDefinationId);

            //if (existing != null)
            //{
            //    // If already active → pend it=>unpend it
            //    if (existing.Pend)
            //    {
            //        existing.Pend = false;
            //        existing.LastModified = now;
            //        existing.ModifiedBy = user;
            //        db.Entry(existing).State = EntityState.Modified;
                    
            //    }

            //    return;
            //}

            //  Create NEW allowance
            //var newAllowance = new AllowancePerEmployee
            //{
            //    EmployeeId = placement.EmployeeId,
            //    AllowanceDefinationId = currentMapping.AllowanceDefinationId,
            //    Remark = currentMapping.Remark,
            //    DateCreated = now,
            //    LastModified = now,
            //    CreatedBy = user,
            //    ModifiedBy = user,
            //    From = placement.AssignmentDate,
            //    To = null,
            //    Pend = false,
            //    EmployeeCode = placement.EmployeeCode,
            //    OrgBranchName = placement.OrgBranchName
            //};

            //db.AllowancePerEmployees.Add(newAllowance);
            
        }
        public ActionResult EmployeeSwapping(int employeeId1, int employeeId2, DateTime swapDate)
        {
            string swapDateAmh = XAPI.EthiopicDateTime.GetEthiopicDate(swapDate.Day, swapDate.Month, swapDate.Year);

            var placements = db.Placements
                .Where(x => x.isCurrent && (x.EmployeeId == employeeId1 || x.EmployeeId == employeeId2))
                .ToList();

            var employeePlacement1 = placements.FirstOrDefault(x => x.EmployeeId == employeeId1);
            var employeePlacement2 = placements.FirstOrDefault(x => x.EmployeeId == employeeId2);

            if (employeePlacement1 == null)
            {
                return Json(new { Message = "The 1st Employee has no Current placement.", Status = "Not-OK" }, JsonRequestBehavior.AllowGet);
            }

            if (employeePlacement2 == null)
            {
                return Json(new { Message = "The 2nd Employee has no Current placement.", Status = "Not-OK" }, JsonRequestBehavior.AllowGet);
            }

            // --- Swap Salary ---
            var tempSalary = employeePlacement1.Salary;
            var tempStepId = employeePlacement1.StepId;
            var tempAge = employeePlacement1.Age;
            var tempRankId = employeePlacement1.RankId;
            var employee1 = employeePlacement1.Employee;
            var tempAssigmentDate = employeePlacement1.AssignmentDate;
            var tempAssigmentDateAm = employeePlacement1.AssignmentDateAmharic;
            employeePlacement1.Salary = employeePlacement2.Salary;
            employeePlacement1.StepId = employeePlacement2.StepId;
            employeePlacement1.RankId = employeePlacement2.RankId;
            employeePlacement1.Age = employeePlacement2.Age;
            employeePlacement1.Employee = employeePlacement2.Employee;
            employeePlacement2.Salary = tempSalary;
            employeePlacement2.StepId = tempStepId;
            employeePlacement2.RankId = tempRankId;
            employeePlacement2.Age = tempAge;
            employeePlacement2.Employee = employee1;
            
            // --- Swap Placement ---
            employeePlacement1.EmployeeId = employeeId2;
            //employeePlacement1.AssignmentDate = swapDate;
            employeePlacement1.AssignmentDate = employeePlacement2.AssignmentDate;
            //employeePlacement1.AssignmentDateAmharic = swapDateAmh;
            employeePlacement1.AssignmentDateAmharic = employeePlacement2.AssignmentDateAmharic;


            employeePlacement2.EmployeeId = employeeId1;
            //employeePlacement2.AssignmentDate = swapDate;
            employeePlacement2.AssignmentDate = tempAssigmentDate;
            //employeePlacement2.AssignmentDateAmharic = swapDateAmh;
            employeePlacement2.AssignmentDateAmharic = tempAssigmentDateAm;

            db.Entry(employeePlacement1).State = EntityState.Modified;
            db.Entry(employeePlacement2).State = EntityState.Modified;
            db.SaveChanges();
            #region operation log

            var emp1 = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId1);
            var emp2 = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId2);

            if (emp1 != null && emp2 != null)
            {
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Employee Placement Swapping",
                    "Success",
                    User.Identity.Name,
                    $"Placement swap performed by [{User.Identity.Name}]. " +
                    $"Employee [{emp1.EmployeeCode}] exchanged placement with Employee [{emp2.EmployeeCode}]. " +
                    $"Effective Date: {swapDate:dd/MM/yyyy}.",
                    Modules.HRM);
            }

            #endregion
            return Json(new { Message = "You successfully swapped the placement and salary.", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }

        private string GetTitleName(int id)
        {
            if (id <= 0)
                return string.Empty;

            try
            {
                using (var context = new ExceedDbContext())
                {
                    return context.Lookups
                                  .Where(x => x.LookupId == id)
                                  .Select(x => x.Description)
                                  .FirstOrDefault() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Log the exception (recommended)
                // Logger.LogError(ex, "Error while fetching lookup title.");
                return string.Empty;
            }
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        public JsonResult GetRank()
        {
            db = new ExceedDbContext();
            var ranks = db.Ranks.AsQueryable();
            ViewData["RankList"] = ranks;
            return Json(ranks.Select(p => new { RankId = p.RankId, RankName = p.RankName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStep()
        {
            db = new ExceedDbContext();
            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = stepss;
            return Json(stepss.Select(p => new { StepId = p.StepId, StepName = p.StepName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalary()
        {
            db = new ExceedDbContext();
            var salaries = db.Salaries.AsQueryable();
            return Json(salaries.Select(p => new
            {
                SalaryStructureId = p.SalaryStructureId,
                SalaryAmount = p.SalaryAmount
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSalary(int fromRank, int toRank, int fromStep, int toStep, string hireDateAm,DateTime? hiredDateAGorg)
        {

            // 1. Load Steps & Ranks Once
            var fromStep1 = db.Steps.Find(fromStep);
            var toStep2 = db.Steps.Find(toStep);
            var fromGrade1 = db.Ranks.Find(fromRank);
            var toGrade2 = db.Ranks.Find(toRank);
            var fyearId = _fiscalYearHelper.GetCurrentFYearId();
            var fiscalYear = db.GLFiscalYears.Find(fyearId);
            if (fiscalYear == null)
                return Json("Please ensure that the Current Fiscal Year is Set to proceed.", JsonRequestBehavior.AllowGet);

            if (fromStep1 == null || toStep2 == null || fromGrade1 == null || toGrade2 == null)
                return Json("Please select all required steps and ranks.", JsonRequestBehavior.AllowGet);

            if (fromStep1.StepId == toStep2.StepId && fromGrade1.RankId == toGrade2.RankId)
                return Json("‘From’ and ‘To’ steps and grades cannot be the same.", JsonRequestBehavior.AllowGet);

            if (toStep2.StepSequence < fromStep1.StepSequence && toGrade2.MaximumSalary < fromGrade1.MaximumSalary)
                return Json("Please select a step with a greater or equal sequence.", JsonRequestBehavior.AllowGet);

            if (toGrade2.MaximumSalary < fromGrade1.MaximumSalary)
                return Json("The selected rank has a higher maximum salary than the current rank.", JsonRequestBehavior.AllowGet);

            // 2. Load Increment Settings Only Once
            var incrementSettings = db.EmployeeSalaryIncrementSettings.ToList();
            var requiredLevels = new[]
            {
                ResponsibilityLevel.STAFF,
                ResponsibilityLevel.LOW_MANAGMENT,
                ResponsibilityLevel.MEDIUM_MANAGMENT,
                ResponsibilityLevel.TOP_MANAGMENT
            };

            var existingLevels = incrementSettings.Select(x => x.ResponsibilityLevel).ToList();
            var missingLevels = requiredLevels.Except(existingLevels).ToList();

            if (missingLevels.Any())
                return Json(
                    $"Please configure Salary Increment settings for: {string.Join(", ", missingLevels)}.",
                    JsonRequestBehavior.AllowGet
                );

            // 3. Load Employees at the Given Rank/Step (Single Query)
            var employeePlacements = db.Placements
                .Where(x => x.isCurrent && x.StepId == fromStep && x.RankId == fromRank)
                .ToList();

            if (!employeePlacements.Any())
                return Json("No employees are placed at the selected rank and step.", JsonRequestBehavior.AllowGet);

            // 4. Optional Filtering by Hire Date (Ethiopian Calendar)
            if (!string.IsNullOrEmpty(hireDateAm)||hiredDateAGorg.HasValue)
            {
                DateTime hiredDate =(!string.IsNullOrEmpty(hireDateAm))? HrHelper.ParseEthiopicDate(hireDateAm):hiredDateAGorg.Value;

                var hiredEmpIds = new HashSet<int>(
                    db.Person_Employee.Where(x => x.HiredDate <= hiredDate)
                                      .Select(x => x.EmployeeId)
                );

                employeePlacements = employeePlacements
                                        .Where(x => hiredEmpIds.Contains(x.EmployeeId))
                                        .ToList();

                if (!employeePlacements.Any())
                    return Json("No employees eligible based on hire date filter.", JsonRequestBehavior.AllowGet);
            }

            // 5. Load Salary for the Target Rank/Step Only Once
            var salary = db.SalariesJobTitles
                .SingleOrDefault(x => x.StepId == toStep && x.RankId == toRank);

            if (salary == null || salary.Salary <= 0)
                return Json("Please update the salary settings first. Salary cannot be zero.", JsonRequestBehavior.AllowGet);

            // 6. Load All Employee Records in One Query (Performance!)
            var employeeIds = employeePlacements.Select(x => x.EmployeeId).ToList();

            var employees = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToDictionary(x => x.EmployeeId, x => x);

            // 7. Perform Updates
            int updatedCount = 0;
            DateTime today = DateTime.Now;

            foreach (var placement in employeePlacements)
            {
                if (!employees.TryGetValue(placement.EmployeeId, out var person))
                    continue;

                var rule = incrementSettings
                    .FirstOrDefault(x => x.ResponsibilityLevel == placement.ResponsibilityLevel);

                if (rule == null)
                    continue;

                // Accurate eligibility date instead of using *30 days
                DateTime eligibleDate = person.HiredDate.AddMonths(rule.EligibilityServiceMonths);

                if (fiscalYear.DateFrom.Date < eligibleDate.Date)
                    continue;

                // Apply salary update
                placement.SalarChangeDate = today;
                placement.SalarChangeDateAmharic = HrHelper.GetECDate(today);
                placement.StepId = toStep;
                placement.RankId = toRank;
                placement.Salary = salary.Salary;
                db.Entry(placement).State = EntityState.Modified;
                updatedCount++;
            }

            db.SaveChanges();
            #region operation log

            string hireDateFilter = !string.IsNullOrEmpty(hireDateAm)
                ? $"Hire Date Filter (EC): {hireDateAm}"
                : hiredDateAGorg.HasValue
                    ? $"Hire Date Filter (GC): {hiredDateAGorg.Value:dd/MM/yyyy}"
                    : "No Hire Date Filter";

            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Employee Salary Increment",
                "Success",
                User.Identity.Name,
                $"Bulk salary increment executed by [{User.Identity.Name}]. " +
                $"From [{fromGrade1.RankName} - {fromStep1.StepName}] " +
                $"To [{toGrade2.RankName} - {toStep2.StepName}]. " +
                $"Employees Updated: {updatedCount}. " +
                $"{hireDateFilter}",
                Modules.HRM
            );

            #endregion
            return Json($"Salary updated for {updatedCount} employee(s).", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPromoteEmployee(int fromRank, int toRank, int fromStep, int toStep)
        {
            var step1 = db.Steps.Find(fromStep);
            var step2 = db.Steps.Find(toStep);
            var grade1 = db.Ranks.Find(fromRank);
            var grade2 = db.Ranks.Find(toRank);

            if (step1 == null || step2 == null || grade1 == null || grade2 == null)
                return Json("Please select all required steps and ranks.", JsonRequestBehavior.AllowGet);

            if (step1.StepId == step2.StepId && grade1.RankId == grade2.RankId)
                return Json("‘From’ and ‘To’ steps and grades cannot be the same. Please select different values.", JsonRequestBehavior.AllowGet);

            if (step1.StepId == step2.StepId)
                return Json("‘From’ and ‘To’ steps cannot be the same. Please select different values.", JsonRequestBehavior.AllowGet);

            if (step2.StepSequence < step1.StepSequence && grade2.MaximumSalary < grade1.MaximumSalary)
                return Json("Please select a step with a greater than or equal to sequence.", JsonRequestBehavior.AllowGet);

            if (grade2.MaximumSalary < grade1.MaximumSalary)
                return Json("The selected rank has a higher maximum salary than the current rank.", JsonRequestBehavior.AllowGet);

            var employeeIds = CheckToBePromotedEmployees();

            // Filter once from the DB with distinct EmployeeId using GroupBy
            var employeePlacements = db.Placements
                .Where(x => employeeIds.Contains(x.EmployeeId)
                            && x.isCurrent
                            && x.StepId == fromStep
                            && x.RankId == fromRank)
                .GroupBy(x => x.EmployeeId)
                .Select(g => g.FirstOrDefault())
                .ToList();

            if (!employeePlacements.Any())
                return Json("No employees are placed at the selected rank and step.", JsonRequestBehavior.AllowGet);

            var salary = db.SalariesJobTitles
                .FirstOrDefault(x => x.StepId == toStep && x.RankId == toRank);

            if (salary == null || salary.Salary <= 0)
                return Json("Please update the salary settings first. The salary must be greater than zero.", JsonRequestBehavior.AllowGet);

            // Cache commonly used values
            var inputsList = inputs.GetEmployees();
            var employeeCodeMap = inputsList.ToDictionary(x => x.PersonId, x => x.EmployeeCode);

            var defaultBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            var preparedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var now = DateTime.Now;
            var nextPromotionDate = now.AddYears(2);
            string amharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(now.Day, now.Month, now.Year);
            string nextPromotionaDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(nextPromotionDate.Day, nextPromotionDate.Month, nextPromotionDate.Year);

            int updatedCount = 0;
            foreach (var placement in employeePlacements)
            {
                // Deactivate current placement
                PlacementAfterSystem_Create(placement, now);

                var newPlacement = new Placement
                {
                    EmployeeId = placement.EmployeeId,
                    HeadId = placement.HeadId,
                    ImmediateSupervisor = placement.ImmediateSupervisor,
                    IsHead = placement.IsHead,
                    AssignmentDate = now,
                    SalarChangeDate = now,
                    MappingId = 0,
                    RankId = toRank,
                    StepId = toStep,
                    Salary = salary.Salary,
                    AssignmentType = AssignmentType.SCALE_PROMOTION,
                    LookupId = placement.LookupId,
                    OrgStructureId = placement.OrgStructureId,
                    ParentOrgStructureId = placement.ParentOrgStructureId,
                    ReportsTo = placement.ReportsTo,
                    JobTitleName = GetTitleName(placement.LookupId),
                    Department = placement.ReportsTo,
                    ReportsToMappingId = 0,
                    ResponsibilityLevel = placement.ResponsibilityLevel,
                    BranchId = placement.BranchId,
                    BusinessUnitId = placement.BusinessUnitId,
                    Remark = "Promoted",
                    DateCreated = now,
                    LastModified = placement.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = placement.ModifiedBy,
                    isCurrent = false,
                    IsApprove = false,
                    IsPend = false,
                    IsTemporary = false,
                    Index = placement.Index,
                    EmployementType = placement.EmployementType,
                    AssignmentDateAmharic = amharicDate,
                    SalarChangeDateAmharic = amharicDate,
                    NextSalaryChangeDate = nextPromotionDate,
                    NextSalaryChangeDateAmharic = nextPromotionaDateAm,
                    EmployeeCode = employeeCodeMap.ContainsKey(placement.EmployeeId) ? employeeCodeMap[placement.EmployeeId] : null,
                    OrgBranchName = defaultBranch,
                    PreparedByEmpId = preparedByEmpId
                };

                db.Placements.Add(newPlacement);

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
                if (person != null)
                {
                    person.IsActive = true;
                    person.BranchId = newPlacement.BranchId;
                    person.Status = newPlacement.EmployementType;
                    person.OrgBranchName = newPlacement.BranchId.ToString();
                    db.Entry(person).State = EntityState.Modified;
                }

                updatedCount++;
            }

            db.SaveChanges(); // Save once after loop
            #region operation log

            var promotedEmployeeCodes = employeePlacements
                .Select(x => x.EmployeeCode)
                .Where(x => !string.IsNullOrEmpty(x))
                .Take(20);

            string employeeList = string.Join(", ", promotedEmployeeCodes);

            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Employee Scale Promotion",
                "Success",
                User.Identity.Name,
                $"Scale promotion performed by [{User.Identity.Name}]. " +
                $"From [{grade1.RankName}-{step1.StepName}] " +
                $"To [{grade2.RankName}-{step2.StepName}]. " +
                $"Employees Affected: {updatedCount}. " +
                $"Sample Employees: {employeeList}" +
                $"{(updatedCount > 20 ? " ..." : "")}",
                Modules.HRM
            );

            #endregion
            return Json($"Salary has been promoted for {updatedCount} employee(s). Approve them in the main page.", JsonRequestBehavior.AllowGet);
        }
        public void PlacementAfterSystem_Create(Placement placement, DateTime dateNewAssignmet)
        {
            if (placement == null) return;

            //  Check if record already exists
            bool exists = db.PlacementAfterSystems.Any(x =>
                x.PlacementId == placement.PlacementId &&
                x.From == placement.AssignmentDate &&
                x.To == dateNewAssignmet);

            if (exists)
            {
                // Already created → skip
                return;
            }

           
                var place = new PlacementAfterSystem();
                place.PlacementId = placement.PlacementId;
                place.EmployeeId = placement.EmployeeId;
                place.OrgStructureId = placement.OrgStructureId;
                place.LookupId = placement.LookupId;
                place.RankId = placement.RankId;
                place.StepId = placement.StepId;
                place.Salary = placement.Salary;
                place.AssignmentType = placement.AssignmentType;
                place.ResponsibilityLevel = placement.ResponsibilityLevel;
                place.BranchId = placement.BranchId;
                place.IsCurrent = placement.isCurrent;
                place.IsHead = placement.IsHead;
                place.EmployementType = placement.EmployementType;
                place.From = placement.AssignmentDate;
                place.To = dateNewAssignmet;
                place.FromAm = XAPI.EthiopicDateTime.GetEthiopicDate(placement.AssignmentDate.Day, placement.AssignmentDate.Month, placement.AssignmentDate.Year);
                place.ToAm = XAPI.EthiopicDateTime.GetEthiopicDate(dateNewAssignmet.Day, dateNewAssignmet.Month, dateNewAssignmet.Year);
                db.PlacementAfterSystems.Add(place);
                db.SaveChanges();
            
        }
        public ActionResult PlacementAfterSystem_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var placementAfterSystems = db.PlacementAfterSystems
                                          .Where(x => x.EmployeeId == id)
                                          .OrderByDescending(x => x.From); // optional: sorted by latest first

            var result = placementAfterSystems.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                IsHead = placement.IsHead,
                OrgStructureId = placement.OrgStructureId,
                LookupId = placement.LookupId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                BranchId = placement.BranchId,
                Salary = placement.Salary,
                AssignmentType = placement.AssignmentType,
                ResponsibilityLevel = placement.ResponsibilityLevel,
                IsCurrent = placement.IsCurrent,
                EmployementType = placement.EmployementType,
                From = placement.From,
                To = placement.To,
                FromAm = placement.FromAm,
                ToAm = placement.ToAm,
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Placements_AfterSystem_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var placements = db.Placements
                .Where(x => x.EmployeeId == id && !x.isCurrent)
                .OrderBy(x => x.AssignmentDate)
                .ToList(); // fetch once

            var branchIds = placements.Select(x => x.BranchId).Distinct().ToList();
            var orgIds = placements.Select(x => x.OrgStructureId).Distinct().ToList();
            var stepIds = placements.Select(x => x.StepId).Distinct().ToList();
            var rankIds = placements.Select(x => x.RankId).Distinct().ToList();
            var jobTitleIds = placements.Select(x => x.LookupId).Distinct().ToList();

            // Pre-load related data in single queries
            var branches = db.Branch.Where(x => branchIds.Contains(x.BranchId)).ToList();
            var orgs = db.MainCompanyStructures.Where(x => orgIds.Contains(x.OrgStructureId)).ToList();
            var steps = db.Steps.Where(x => stepIds.Contains(x.StepId)).ToList();
            var ranks = db.Ranks.Where(x => rankIds.Contains(x.RankId)).ToList();
            var jobTitles = db.Lookups.Where(x => jobTitleIds.Contains(x.LookupId) &&
                                                  (x.LookupType == LookUpTypes.JOB_TITLE
                                                  ||x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                  ||x.LookupType == LookUpTypes.Jobtitle_External_Experience)).ToList();

            var list = new List<OldPlacementVM>();
            int length = placements.Count;

            for (int i = 0; i < length; i++)
            {
                var p = placements[i];
                var vm = new OldPlacementVM
                {
                    PlaceId = p.PlacementId,
                    LookupId = p.LookupId,
                    AssignmentDate = p.AssignmentDateAmharic,
                    FromDate = p.AssignmentDate,
                    From = p.AssignmentDateAmharic,
                    IsHead = p.IsHead,
                    IsCurrent = p.isCurrent,
                    Salary = p.Salary,
                    AssignmentType = p.AssignmentType
                };

                // Branch
                vm.Branch = branches.FirstOrDefault(b => b.BranchId == p.BranchId)?.Name;

                // Department / Org Structure
                vm.Department = orgs.FirstOrDefault(o => o.OrgStructureId == p.OrgStructureId)?.Name;

                // Job Title
                vm.JobTitle = jobTitles.FirstOrDefault(j => j.LookupId == p.LookupId)?.Description;

                // Step
                vm.Step = steps.FirstOrDefault(s => s.StepId == p.StepId)?.StepName;

                // Grade
                vm.Grade = ranks.FirstOrDefault(r => r.RankId == p.RankId)?.RankName;

                // To / ToDate logic
                if (i == length - 1)
                {
                    // last record
                    if (p.isCurrent)
                    {
                        DateTime now = DateTime.Now;
                        vm.ToDate = now;
                        vm.To = XAPI.EthiopicDateTime.GetEthiopicDate(now.Day, now.Month, now.Year);
                    }
                    else
                    {
                        vm.ToDate = p.AssignmentDate;
                        vm.To = p.AssignmentDateAmharic;
                    }
                }
                else
                {
                    // next placement determines the To date
                    vm.ToDate = placements[i + 1].AssignmentDate;
                    vm.To = placements[i + 1].AssignmentDateAmharic;
                }

                list.Add(vm);
            }

            var result = list.ToDataSourceResult(request);

            return Json(result);
        }
        public void SetEmployeeId(string fullName)
        {
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            if (employeeList.Count() > 0)
            {
                employeeList = employeeList.Where(l => l.IsActive).AsQueryable();
            }
            foreach (var emp in employeeList)
            {
                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdViews.Add(EmployeeIdView);
            }
            var employees = EmployeeIdViews.Where(p => p.FullName == fullName).FirstOrDefault();
            if (employees != null)
            {
                HRMInputs.EmployeeId = employees.PersonId;
            }

        }

        [HttpPost]
        public JsonResult GetSalaryAmount(int rank, int? step, int? jobtitle)
        {
            // Load salary data for the given rank only
            var data = db.SalariesJobTitles
                         .Where(s => s.RankId == rank)
                         .Select(s => new
                         {
                             s.StepId,
                             s.LookupId,
                             s.Salary
                         })
                         .ToList();

            // Read primitive value only (EF-safe)
            bool isJobTitleIncluded =
                db.SalarySettings
                  .Select(x => x.IsJobtitleIncludedInSalary)
                  .FirstOrDefault();

            decimal? amount = null;

            bool hasStep = step.HasValue && step > 0;
            bool hasJobTitle = jobtitle.HasValue && jobtitle > 0;

            // 1 Rank + Step + JobTitle
            if (hasStep && hasJobTitle && isJobTitleIncluded)
            {
                amount = data
                    .Where(x => x.StepId == step && x.LookupId == jobtitle)
                    .Select(x => (decimal?)x.Salary)
                    .FirstOrDefault();
            }

            // 2️ Rank + Step
            if (amount == null && hasStep)
            {
                amount = data
                    .Where(x => x.StepId == step)
                    .Select(x => (decimal?)x.Salary)
                    .FirstOrDefault();
            }

            // 3️ Rank + JobTitle
            if (amount == null && hasJobTitle && isJobTitleIncluded)
            {
                amount = data
                    .Where(x => x.LookupId == jobtitle)
                    .Select(x => (decimal?)x.Salary)
                    .FirstOrDefault();
            }



            return Json(amount ?? 0m, JsonRequestBehavior.AllowGet);
        }


   
        public JsonResult GetJobTitle(int OrgStructureId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgStructureId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    if (!titleList.Contains(lookup))
                    {
                        titleList.Add(lookup);
                    }

                }
            }
            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult SetPlacementAsEditable(int placementId)
        {
            var placement = db.Placements.Find(placementId);

            if (placement == null)
            {
                return Json(new
                {
                    Message = "The placement record does not exist.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }
             if (placement.IsEditable)
            {
                return Json(new
                {
                    Message = "enabled already.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            placement.IsEditable = true;
            placement.ModifiedBy = User.Identity?.Name ?? "System";
            placement.LastModified = DateTime.Now;
            db.Entry(placement).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new
            {
                Message = "Placement has been successfully marked as editable.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CheckPlacement(int id)
        {
            var placement = db.Placements
                .FirstOrDefault(x => x.PlacementId == id);

            if (placement == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Placement not found."
                });
            }

            if (placement.IsChecked)
            {
                return Json(new
                {
                    Success = false,
                    Message = "This placement has already been checked."
                });
            }

            placement.IsChecked = true;
            placement.CheckedBy = User.Identity.Name;
            placement.CheckedDate = DateTime.Now;

            db.SaveChanges();

            return Json(new
            {
                Success = true,
                Message = "Placement checked successfully."
            });
        }

        [HttpPost]
        public ActionResult ApprovePlacement(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Set command timeout
                    db.Database.CommandTimeout = 120;

                    var placement = db.Placements.Find(id);
                    if (placement == null)
                    {
                        return Json(new
                        {
                            Message = "This placement does not exist.",
                            Status = "Not-Ok"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    if (placement.IsApprove)
                    {
                        return Json(new
                        {
                            Message = "This placement is Approved already.",
                            Status = "Not-Ok"
                        }, JsonRequestBehavior.AllowGet);
                    }

                    bool requireChecker = db.PlacementWorkflowSettings
                                        .FirstOrDefault()?.RequireChecker ?? false;

                    if (requireChecker && !placement.IsChecked)
                    {
                        return Json(new
                        {
                            Message = "This placement must be checked before approval.",
                            Status = "Not-Ok"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var employeeId = placement.EmployeeId;
                    var branchId = placement.BranchId;
                    var user = User.Identity.Name;
                    var approvedByEmpId = UserInformationHelper.GetEmployeeId(user);

                    // 1. Update current placement
                    placement.IsApprove = true;
                    placement.isCurrent = true;
                    placement.ApprovedByEmpId = approvedByEmpId;
                    placement.LastModified = DateTime.Now;
                    placement.ModifiedBy = user;
                    db.Entry(placement).State = EntityState.Modified;

                    // 2. Update previous placements
                    var previousPlacements = db.Placements
                        .Where(x => x.isCurrent && x.EmployeeId == employeeId && x.PlacementId != id)
                        .ToList();

                    foreach (var prevPlacement in previousPlacements)
                    {
                        prevPlacement.isCurrent = false;
                        prevPlacement.IsTemporary = false;
                        prevPlacement.LastModified = DateTime.Now;
                        prevPlacement.ModifiedBy = user;
                        prevPlacement.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(user);
                        prevPlacement.ApprovedByEmpId = prevPlacement.EmployeeId;
                        db.Entry(prevPlacement).State = EntityState.Modified;

                    }

                    // 3. Update employee
                    var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == employeeId);
                    if (employee != null)
                    {
                        employee.IsActive = true;
                        employee.IsPlaced = true;
                        employee.OrgBranchName = branchId.ToString();
                        employee.ModifiedBy = user;
                        employee.LastModified = DateTime.Now;
                        employee.BranchId = branchId;
                        db.Entry(employee).State = EntityState.Modified;

                    }

                    // Save all changes so far
                    db.SaveChanges();

                    // 4. OPTIMIZED: Handle head assignments if needed
                    if (placement.IsHead)
                    {
                        // Get subordinates using optimized single query
                        var subordinateEmployeeIds = GetSubordinatesOptimized(placement.EmployeeId, placement.OrgStructureId, placement.BranchId);

                        if (subordinateEmployeeIds.Any())
                        {
                            // Single query to update all head assignments
                            var changeHeads = db.Placements
                                .Where(x => subordinateEmployeeIds.Contains(x.EmployeeId)
                                            && x.HeadId != placement.EmployeeId
                                            && x.isCurrent)
                                .ToList();

                            foreach (var changeHead in changeHeads)
                            {
                                changeHead.HeadId = placement.EmployeeId;
                                db.Entry(changeHead).State = EntityState.Modified;

                            }

                            db.SaveChanges();
                        }
                    }

                    transaction.Commit();
                    if (!db.LeavesEntitle.Any(x => x.EmployeeId == placement.EmployeeId))
                    {
                        GenerateLeaveEntitlement(placement);
                    }
                    // 5. Handle employee card
                    if (placement.AssignmentType == AssignmentType.NEW)
                    {
                        HandlePlacementToEmployeeCard(placement, EmployeeCardType.New_Placement);
                    }
                    else
                    {
                        HandlePlacementToEmployeeCard(placement, EmployeeCardType.Placement_Change);
                    }

                    return Json(new
                    {
                        Message = "You successfully approved this placement.",
                        Status = "OK"
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    var errorMessage = ex.Message;
                    if (ex.InnerException != null)
                        errorMessage += " | Inner: " + ex.InnerException.Message;
                    if (ex.InnerException?.InnerException != null)
                        errorMessage += " | Inner Inner: " + ex.InnerException.InnerException.Message;

                    return Json(new
                    {
                        Message = "An error occurred: " + errorMessage,
                        Status = "Not-Ok"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        private void GenerateLeaveEntitlement(Placement placement)
        {
           
            var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom < DateTime.Now && DateTime.Now<x.DateTo);
            if (currentPeriod == null)
            {
                return;
            }

            // 2. Validate initial Awaj settings
            var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
            if (newAwajInitialYear == null)
            {
                return;
            }

            var initialGlFiscalYear = db.GLFiscalYears
                .FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
            if (initialGlFiscalYear == null)
            {
                return;
            }

            

            // 4. Load configuration values
            var (maxLeaveConfig, maxLeaderConfig, thresholdLeave, forleadersnew) = LoadConfigurationValues();
            if (maxLeaveConfig == null || maxLeaderConfig == null || thresholdLeave == null || forleadersnew == null)
            {
                return;
            }

            int maximumValue = int.Parse(maxLeaveConfig.CurrentValue);
            int LeaveTrusholdValue = int.Parse(thresholdLeave.CurrentValue);
            int leaveTresholdForLeaderdrsNew = int.Parse(forleadersnew.CurrentValue);
            int maximumForLeader = int.Parse(maxLeaderConfig.CurrentValue);
            bool isLeader = placement.ResponsibilityLevel != ResponsibilityLevel.STAFF;
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId&&x.IsActive);
            if (person == null) return;

            var hireDate = person.HiredDate;
            decimal newEntitle = isLeader ? leaveTresholdForLeaderdrsNew : LeaveTrusholdValue;
            decimal newAmount = newEntitle;

            // Pro-rate if hired during the current period
            if (hireDate.Date > currentPeriod.DateFrom.Date && hireDate.Date < currentPeriod.DateTo.Date)
            {
                var totalPeriodDays = (currentPeriod.DateTo.Date - currentPeriod.DateFrom.Date).TotalDays;
                var eligibleDays = (currentPeriod.DateTo.Date - hireDate.Date).TotalDays;
                var proratedFraction = (decimal)(eligibleDays / totalPeriodDays);
                newAmount = Math.Round(newEntitle * proratedFraction, 2);
            }

            // Skip if hired after period end
            if (hireDate.Date > currentPeriod.DateTo.Date)
                return;

            var entitle = new LeaveEntitle
            {
                GlFiscalYearId = currentPeriod.GlFiscalYearId,
                EmployeeId = placement.EmployeeId,
                EmployeeCode = placement.EmployeeCode,
                BranchId = placement.BranchId,
                OrgBranchName = placement.BranchId.ToString(),
                Amount = newEntitle,
                Balance = newAmount,
                IsAdded = true,
                IsExpired = false,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name
            };
            db.LeavesEntitle.Add(entitle);
            db.SaveChanges();
        }
        private (Configuration maxLeave, Configuration maxLeader, Configuration threshold, Configuration forLeadersNew) LoadConfigurationValues()
        {
            var maxLeaveConfig = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            var maxLeaderConfig = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());
            var thresholdLeave = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());
            var forleadersnew = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());

            return (maxLeaveConfig, maxLeaderConfig, thresholdLeave, forleadersnew);
        }
        // Optimized subordinate retrieval method
        private List<int> GetSubordinatesOptimized(int employeeId, int orgStructureId, int branchId)
        {
            var subordinateIds = new List<int>();

            // Get current placements in same org structure and branch
            var sameOrgPlacements = db.Placements.AsNoTracking()
                .Where(x => x.isCurrent && x.OrgStructureId == orgStructureId && x.BranchId == branchId)
                .Select(x => x.EmployeeId)
                .ToList();

            subordinateIds.AddRange(sameOrgPlacements);

            // Get child work units
            var childWorkUnits = db.MainCompanyStructures.AsNoTracking()
                .Where(x => x.IsActive && x.Parent == orgStructureId)
                .Select(x => x.OrgStructureId)
                .ToList();

            // Get heads of child work units in same branch
            var childHeads = db.Placements.AsNoTracking()
                .Where(x => x.isCurrent && x.IsHead && childWorkUnits.Contains(x.OrgStructureId) && x.BranchId == branchId)
                .Select(x => x.EmployeeId)
                .ToList();

            subordinateIds.AddRange(childHeads);

           
            // Remove the current employee from subordinates
            subordinateIds.Remove(employeeId);

            return subordinateIds.Distinct().ToList();
        }


        private JsonResult UpdatePendStatus(int id, bool isPend)
        {
            var placement = db.Placements.FirstOrDefault(x => x.PlacementId == id);

            if (placement == null)
            {
                return Json(new
                {
                    Message = "Employee placement not found.",
                    Status = "FAIL"
                }, JsonRequestBehavior.AllowGet);
            }

            placement.IsPend = isPend;
            db.Entry(placement).State = EntityState.Modified;
            db.SaveChanges();

            string message = isPend
                ? "You successfully pended this employee for payroll processing."
                : "You successfully removed this employee from pend list for payroll processing.";

            return Json(new { Message = message, Status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoCompleteHead(int employeeId, int orgId, int branId, int businUinitId)
        {
            var headId = EmployeeHelper.GetSupervisorEmployeeId(employeeId, orgId, branId, businUinitId);
            var head = db.Person_Employee.Find(headId);

            return Json(new
            {
                headId = headId,
                fullName = head != null
                    ? $"{head.EmployeeCode} - {head.FirstNameEnglish} {head.MiddleNameEnglish} {head.LastNameEnglish}"
                    : ""
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Pend(int id) => UpdatePendStatus(id, true);
        public ActionResult UnPend(int id) => UpdatePendStatus(id, false);

        public ActionResult UpdateNewSalary()
        {
            var placements = db.Placements.Where(x => x.isCurrent).ToList();

            int updatedCount = 0;

            foreach (var place in placements)
            {
                var salarySetting = db.SalariesJobTitles
                    .FirstOrDefault(x => x.RankId == place.RankId
                                      && x.StepId == place.StepId);

                if (salarySetting != null && place.Salary != salarySetting.Salary)
                {
                    place.Salary = salarySetting.Salary;
                    db.Entry(place).State = EntityState.Modified;
                    updatedCount++;
                }
            }

            db.SaveChanges();

            #region operation log

            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Salary Synchronization",
                "Success",
                User.Identity.Name,
                $"Salary synchronization executed by [{User.Identity.Name}]. " +
                $"Updated salary records for [{updatedCount}] employee(s) based on the current Rank and Step salary settings.",
                Modules.HRM
            );

            #endregion

            return RedirectToAction("Index");
        }

        public ActionResult UploadDocument(string employeeId, HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT), HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = employeeId,
                            Description = HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = employeeId
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult UploadPdfToEmployeeArchive(int employeeId, string fileName, string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {


                    string mainPath = Path.Combine(filePath, fileName);

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(filePath))
                    {

                        var entity = new Attachment
                        {

                            Reference = employeeId.ToString(),
                            Description = HRMInputs.EmployeeId.ToString() + "-" + fileName ,
                            FileName = fileName,
                            FileType = "Pdf",
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = filePath,
                            Remark = employeeId.ToString()
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        private void HandlePlacementToEmployeeCard(Placement placement, EmployeeCardType employeeCardType)
        {
            DateTime now = DateTime.Now;
            var entity = new EmployeeCard
            {
                EmployeeId = placement.EmployeeId,
                ToEmployeeId = null,
                OrgStructureId = placement.OrgStructureId,
                LookupId = placement.LookupId,
                BranchId = placement.BranchId,
                BusinessUnitId = placement.BusinessUnitId,
                EachPrimaryId = placement.PlacementId,
                CardType = employeeCardType,
                IsActive = false,
                ActionTaken = placement.AssignmentType.ToString(),
                Salary = placement.Salary,
                Remark = placement.AssignmentType.ToString(),
                FromDate = DateTime.Now,
                FromDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(now.Day, now.Month, now.Year),
                ToDate = null,
                ToDateTimeAm = null,
                CreatedBy = User.Identity.Name,
                DateCreated = now,
                ModifiedBy = null,
                LastModified = null,
                OrgBranchName = placement.OrgBranchName
            };
            db.EmployeeCards.Add(entity);
            db.SaveChanges();
        }

        public async Task<JsonResult> RegisterEmployeeToBiometric(int empId)
        {
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["BiometricBaseUrl"];
                string apiUrl = WebConfigurationManager.AppSettings["EmployeeRegistrationApiUrl"];
                string username = WebConfigurationManager.AppSettings["JwtUsername"];
                string password = WebConfigurationManager.AppSettings["JwtPassword"];

                using (var context = new ExceedDbContext())
                {
                    var emp = context.Person_Employee
                        .FirstOrDefault(x => x.EmployeeId == empId && x.IsActive);

                    if (emp == null)
                        return JsonError("Employee not found.");

                    if (emp.IsBiometricRegistered)
                        return JsonError("Employee already registered.");

                    var placement = context.Placements
                        .Where(p => p.EmployeeId == empId && p.isCurrent)
                        .OrderByDescending(p => p.AssignmentDate)
                        .FirstOrDefault();

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrl);

                        // Login
                        var loginData = new
                        {
                            username,
                            password
                        };

                        var loginContent = new StringContent(
                            JsonConvert.SerializeObject(loginData),
                            Encoding.UTF8,
                            "application/json"
                        );

                        var loginResponse = await client.PostAsync("jwt-api-token-auth/", loginContent);

                        if (!loginResponse.IsSuccessStatusCode)
                            return JsonError("JWT authentication failed.");

                        var loginResult = await loginResponse.Content.ReadAsStringAsync();
                        dynamic tokenObj = JsonConvert.DeserializeObject(loginResult);
                        string token = tokenObj.token;

                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("JWT", token);

                        // SAFE DATA MAPPING
                        var biometricEmployee = new
                        {
                            emp_code = emp.EmployeeCode ?? "",
                            first_name = emp.FirstNameEnglish ?? "",
                            last_name = emp.LastNameEnglish ?? "",
                            nickname = emp.MiddleNameEnglish ?? "",

                            hire_date = emp.HiredDate != null
                                ? emp.HiredDate.ToString("yyyy-MM-dd")
                                : "2000-01-01",

                            birthday = emp.DateOfBirth != null
                                ? emp.DateOfBirth.ToString("yyyy-MM-dd")
                                : null,

                            // 🔥 IMPORTANT FIX
                            //gender = emp.Gender == null
                            //    ? (int?)null
                            //    : (emp.Gender == Gender.MALE ? 1 : 2),


                            // 🚫 DO NOT SEND gender

                            department = 1,         
                            area = new int[] { 2 }, //

                            mobile = emp.PhoneNo ?? "",
                            city = emp.City ?? "",
                            address = $"{emp.SubCity ?? ""} {emp.Wereda ?? ""}".Trim(),

                            national = emp.Nationality ?? "",

                            verify_mode = 0
                        };

                        // Ignore nulls (VERY IMPORTANT)
                        var json = JsonConvert.SerializeObject(
                            biometricEmployee,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(apiUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            return JsonError(error);
                        }
                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(responseContent);

                        emp.IsBiometricRegistered = true;
                        emp.BiometricEmployeeId = result.id;

                        context.SaveChanges();
                        return JsonSuccess("Employee successfully registered to biometric system.");
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }
        private JsonResult JsonSuccess(string message, object data = null)
        {
            return Json(new
            {
                success = true,
                message = message,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult JsonError(string message)
        {
            return Json(new
            {
                success = false,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SalaryRangeReportIndex()
        {
            var model = new SalaryRangeFilterVM
            {
                FromDate = DateTime.Now.AddYears(-5),
                ToDate = DateTime.Now
            };

            return View(model);
        }
        // 2. Report Output Action
        [HttpGet]
        public ActionResult SalaryRangeByProjectReport(DateTime? from, DateTime? to)
        {
            var company = db.Companies.AsNoTracking().FirstOrDefault();
            string compName = !string.IsNullOrEmpty(company?.TradeName) ? company.TradeName : company?.BrandName ?? "MCG CONSTRUCTION P.L.C";

            var query = from p in db.Placements.AsNoTracking().Where(x => x.isCurrent)
                        join emp in db.Person_Employee.AsNoTracking() on p.EmployeeId equals emp.EmployeeId
                        join b in db.Branch.AsNoTracking() on p.BranchId equals b.BranchId
                        select new
                        {
                            p.BranchId,
                            BranchName = b.Name,
                            p.Salary,
                            emp.Gender,
                            p.AssignmentDate
                        };

            if (from.HasValue)
                query = query.Where(x => x.AssignmentDate >= from.Value);
            if (to.HasValue)
                query = query.Where(x => x.AssignmentDate <= to.Value);

            var records = query.ToList();

            var groupedByBranch = records
                .GroupBy(x => new { x.BranchId, x.BranchName })
                .OrderBy(g => g.Key.BranchName)
                .ToList();

            var model = new SalaryRangeByProjectReportVM
            {
                CompanyName = compName,
                FromDate = from,
                ToDate = to
            };

            foreach (var grp in groupedByBranch)
            {
                var row = new ProjectSalaryRangeRowVM
                {
                    BranchId = grp.Key.BranchId,
                    ProjectName = grp.Key.BranchName
                };

                foreach (var item in grp)
                {
                    bool isFemale = item.Gender == Gender.MALE || (item.Gender.ToString().Trim().Equals("Female", StringComparison.OrdinalIgnoreCase));
                    decimal sal = item.Salary;

                    if (sal <= 10000m)
                    {
                        if (isFemale) row.RangeA.Female++; else row.RangeA.Male++;
                    }
                    else if (sal <= 20000m)
                    {
                        if (isFemale) row.RangeB.Female++; else row.RangeB.Male++;
                    }
                    else if (sal <= 30000m)
                    {
                        if (isFemale) row.RangeC.Female++; else row.RangeC.Male++;
                    }
                    else if (sal <= 40000m)
                    {
                        if (isFemale) row.RangeD.Female++; else row.RangeD.Male++;
                    }
                    else if (sal <= 50000m)
                    {
                        if (isFemale) row.RangeE.Female++; else row.RangeE.Male++;
                    }
                    else
                    {
                        if (isFemale) row.RangeF.Female++; else row.RangeF.Male++;
                    }
                }

                model.Rows.Add(row);

                // Accumulate totals
                model.GrandTotal.RangeA.Female += row.RangeA.Female;
                model.GrandTotal.RangeA.Male += row.RangeA.Male;
                model.GrandTotal.RangeB.Female += row.RangeB.Female;
                model.GrandTotal.RangeB.Male += row.RangeB.Male;
                model.GrandTotal.RangeC.Female += row.RangeC.Female;
                model.GrandTotal.RangeC.Male += row.RangeC.Male;
                model.GrandTotal.RangeD.Female += row.RangeD.Female;
                model.GrandTotal.RangeD.Male += row.RangeD.Male;
                model.GrandTotal.RangeE.Female += row.RangeE.Female;
                model.GrandTotal.RangeE.Male += row.RangeE.Male;
                model.GrandTotal.RangeF.Female += row.RangeF.Female;
                model.GrandTotal.RangeF.Male += row.RangeF.Male;
            }

            return View("SalaryRangeByProjectReport", model);
        }

        [HttpGet]
        public ActionResult SalaryRangeByDepartmentReport(DateTime? from, DateTime? to)
        {
            var company = db.Companies.AsNoTracking().FirstOrDefault();
            string compName = !string.IsNullOrEmpty(company?.TradeName) ? company.TradeName : (!string.IsNullOrEmpty(company?.BrandName) ? company.BrandName : company?.BrandName ?? "MCG CONSTRUCTION P.L.C");

            // Query active placements joined with Department / MainCompanyStructures
            var query = from p in db.Placements.AsNoTracking().Where(x => x.isCurrent)
                        join emp in db.Person_Employee.AsNoTracking() on p.EmployeeId equals emp.EmployeeId
                        join dept in db.MainCompanyStructures.AsNoTracking() on p.OrgStructureId equals dept.OrgStructureId
                        select new
                        {
                            p.OrgStructureId,
                            DepartmentName = dept.Name,
                            p.Salary,
                            emp.Gender,
                            p.AssignmentDate
                        };

            if (from.HasValue)
                query = query.Where(x => x.AssignmentDate >= from.Value);
            if (to.HasValue)
                query = query.Where(x => x.AssignmentDate <= to.Value);

            var records = query.ToList();

            var groupedByDept = records
                .GroupBy(x => new { x.OrgStructureId, x.DepartmentName })
                .OrderBy(g => g.Key.DepartmentName)
                .ToList();

            var model = new SalaryRangeByDepartmentReportVM
            {
                CompanyName = compName,
                FromDate = from,
                ToDate = to
            };

            foreach (var grp in groupedByDept)
            {
                var row = new DepartmentSalaryRangeRowVM
                {
                    OrgStructureId = grp.Key.OrgStructureId,
                    DepartmentName = grp.Key.DepartmentName ?? "Unassigned Department"
                };

                foreach (var item in grp)
                {
                    bool isFemale = item.Gender == Gender.FEMALE || (item.Gender.ToString().Trim().Equals("Female", StringComparison.OrdinalIgnoreCase));
                    decimal sal = item.Salary;

                    if (sal <= 10000m)
                    {
                        if (isFemale) row.RangeA.Female++; else row.RangeA.Male++;
                    }
                    else if (sal <= 20000m)
                    {
                        if (isFemale) row.RangeB.Female++; else row.RangeB.Male++;
                    }
                    else if (sal <= 30000m)
                    {
                        if (isFemale) row.RangeC.Female++; else row.RangeC.Male++;
                    }
                    else if (sal <= 40000m)
                    {
                        if (isFemale) row.RangeD.Female++; else row.RangeD.Male++;
                    }
                    else if (sal <= 50000m)
                    {
                        if (isFemale) row.RangeE.Female++; else row.RangeE.Male++;
                    }
                    else
                    {
                        if (isFemale) row.RangeF.Female++; else row.RangeF.Male++;
                    }
                }

                model.Rows.Add(row);

                // Accumulate into Grand Total
                model.GrandTotal.RangeA.Female += row.RangeA.Female;
                model.GrandTotal.RangeA.Male += row.RangeA.Male;
                model.GrandTotal.RangeB.Female += row.RangeB.Female;
                model.GrandTotal.RangeB.Male += row.RangeB.Male;
                model.GrandTotal.RangeC.Female += row.RangeC.Female;
                model.GrandTotal.RangeC.Male += row.RangeC.Male;
                model.GrandTotal.RangeD.Female += row.RangeD.Female;
                model.GrandTotal.RangeD.Male += row.RangeD.Male;
                model.GrandTotal.RangeE.Female += row.RangeE.Female;
                model.GrandTotal.RangeE.Male += row.RangeE.Male;
                model.GrandTotal.RangeF.Female += row.RangeF.Female;
                model.GrandTotal.RangeF.Male += row.RangeF.Male;
            }

            return View("SalaryRangeByDepartmentReport", model);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
    public class BiometricEmployeeDto
    {
        public string emp_code { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string nickname { get; set; }
        public string hire_date { get; set; }           // "yyyy-MM-dd"
        public string birthday { get; set; }           // "yyyy-MM-dd"
        public int? department { get; set; }           // Department ID
        public int? gender { get; set; }               // Nullable gender
        public int[] area { get; set; }                // List of area IDs
        public string mobile { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string national { get; set; }
        public int verify_mode { get; set; } = 0;      // Default 0
    }
    public class SalaryRangeFilterVM
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class SalaryRangeByDepartmentReportVM
    {
        public string CompanyName { get; set; } = "MCG CONSTRUCTION P.L.C";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<DepartmentSalaryRangeRowVM> Rows { get; set; } = new List<DepartmentSalaryRangeRowVM>();
        public DepartmentSalaryRangeRowVM GrandTotal { get; set; } = new DepartmentSalaryRangeRowVM { DepartmentName = "Total" };
    }

    public class DepartmentSalaryRangeRowVM
    {
        public int OrgStructureId { get; set; }
        public string DepartmentName { get; set; }

        // (A) 0 - 10,000
        public GenderCountRange RangeA { get; set; } = new GenderCountRange();

        // (B) 10,001 - 20,000
        public GenderCountRange RangeB { get; set; } = new GenderCountRange();

        // (C) 20,001 - 30,000
        public GenderCountRange RangeC { get; set; } = new GenderCountRange();

        // (D) 30,001 - 40,000
        public GenderCountRange RangeD { get; set; } = new GenderCountRange();

        // (E) 40,001 - 50,000
        public GenderCountRange RangeE { get; set; } = new GenderCountRange();

        // (F) > 50,000
        public GenderCountRange RangeF { get; set; } = new GenderCountRange();

        // Row Totals
        public int TotalFemale => RangeA.Female + RangeB.Female + RangeC.Female + RangeD.Female + RangeE.Female + RangeF.Female;
        public int TotalMale => RangeA.Male + RangeB.Male + RangeC.Male + RangeD.Male + RangeE.Male + RangeF.Male;
        public int RowTotal => RangeA.Total + RangeB.Total + RangeC.Total + RangeD.Total + RangeE.Total + RangeF.Total;
    }
    public class ProjectSalaryRangeRowVM
    {
        public int BranchId { get; set; }
        public string ProjectName { get; set; }

        // --- Salary Brackets ---

        // Bracket (A): 0 - 10,000 ETB
        public GenderCountRange RangeA { get; set; } = new GenderCountRange();

        // Bracket (B): 10,001 - 20,000 ETB
        public GenderCountRange RangeB { get; set; } = new GenderCountRange();

        // Bracket (C): 20,001 - 30,000 ETB
        public GenderCountRange RangeC { get; set; } = new GenderCountRange();

        // Bracket (D): 30,001 - 40,000 ETB
        public GenderCountRange RangeD { get; set; } = new GenderCountRange();

        // Bracket (E): 40,001 - 50,000 ETB
        public GenderCountRange RangeE { get; set; } = new GenderCountRange();

        // Bracket (F): > 50,000 ETB
        public GenderCountRange RangeF { get; set; } = new GenderCountRange();

        // --- Row Aggregates ---

        public int TotalFemale => RangeA.Female + RangeB.Female + RangeC.Female +
                                  RangeD.Female + RangeE.Female + RangeF.Female;

        public int TotalMale => RangeA.Male + RangeB.Male + RangeC.Male +
                                RangeD.Male + RangeE.Male + RangeF.Male;

        public int RowTotal => RangeA.Total + RangeB.Total + RangeC.Total +
                               RangeD.Total + RangeE.Total + RangeF.Total;
    }

    public class GenderCountRange
    {
        public int Female { get; set; }
        public int Male { get; set; }
        public int Total => Female + Male;
    }

    public class SalaryRangeByProjectReportVM
    {
        // Header / Company Metadata
        public string CompanyName { get; set; } 
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Itemized Rows (One per Project/Branch)
        public List<ProjectSalaryRangeRowVM> Rows { get; set; } = new List<ProjectSalaryRangeRowVM>();

        // Summary Grand Total Row
        public ProjectSalaryRangeRowVM GrandTotal { get; set; } = new ProjectSalaryRangeRowVM
        {
            ProjectName = "Total"
        };
    }
}