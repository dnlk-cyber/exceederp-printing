using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Kendo.Mvc;
using System.Data.Entity.Validation;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRPlacementAdmin,
                    HrmConstants.HRPlacementUser)]
    public class EmployeeToBePromotedController : BaseController
    {

        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            
            PopulateViewData();
           
            return View();
        }
        private void PopulateViewData()
        {
            //GetRank();
            //GetStep();
            var ranks = db.Ranks.AsQueryable();
            ViewData["RankList"] = CacheData.GetRankCaches().Select(
               i => new
               {
                   RankName = i.Name,
                   RankId = i.Id,
               }).AsQueryable(); ;
            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = CacheData.GetStepCaches().Select(
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
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
               i => new
               {
                   Description = i.Name,
                   LookupId = i.Id,
               }).AsQueryable();
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
        }
        public ActionResult RefreshSalaryChangeDates()
        {
            DateTime today = DateTime.Now;
            string currentUser = User.Identity.Name;

            // Get current placements that need adjustment
            var placements = db.Placements
                .Where(p => p.isCurrent &&
                           p.AssignmentType == AssignmentType.SCALE_PROMOTION &&
                           p.SalarChangeDate.HasValue &&
                           p.NextSalaryChangeDate.HasValue)
                .ToList();

            foreach (var placement in placements)
            {
                DateTime assignmentDate = placement.AssignmentDate;
                DateTime salaryChangeDate = placement.SalarChangeDate.Value;
                DateTime nextSalaryChangeDate = placement.NextSalaryChangeDate.Value;

                // Only process if dates need adjustment
                if (salaryChangeDate.Year != assignmentDate.Year ||
                    nextSalaryChangeDate <= today)
                {
                    // Calculate proper 2-year cycle dates
                    //DateTime newSalaryChangeDate = new DateTime(assignmentDate.Year, salaryChangeDate.Month, salaryChangeDate.Day);
                    DateTime newSalaryChangeDate = salaryChangeDate;
                    DateTime newNextSalaryChangeDate = newSalaryChangeDate.AddYears(2);

                    // Ensure we don't set dates in the past
                    while (newNextSalaryChangeDate <= today)
                    {
                        newSalaryChangeDate = newSalaryChangeDate.AddYears(2);
                        newNextSalaryChangeDate = newNextSalaryChangeDate.AddYears(2);
                    }

                    // Update placement
                    placement.SalarChangeDate = newSalaryChangeDate;
                    placement.NextSalaryChangeDate = newNextSalaryChangeDate;
                    placement.SalarChangeDateAmharic = HrHelper.GetECDate(newSalaryChangeDate);
                    placement.NextSalaryChangeDateAmharic = HrHelper.GetECDate(newNextSalaryChangeDate);
                    placement.ModifiedBy = currentUser;
                    placement.LastModified = today;
                }
            }

            // Save all changes at once
            db.SaveChanges();
            return RedirectToAction("Index");
        }

      
        private IList<int> CheckToBePromotedEmployeesOld()
        {
            var dateLimit = DateTime.Now.AddYears(-2);

            // Get employees with current placements
            var currentPlacements = db.Placements
                .Where(p => p.isCurrent)
                .Select(p => new
                {
                    p.EmployeeId,
                    p.StepId,
                    p.RankId
                })
                .ToList();

            var eligibleEmployeeIds = new List<int>();
            foreach (var cp in currentPlacements)
            {
                var hasChanged = db.Placements
                    .Where(p => p.EmployeeId == cp.EmployeeId && p.AssignmentDate >= dateLimit)
                    .Any(p => p.StepId != cp.StepId); // Add RankId != cp.RankId if needed

                if (!hasChanged)
                {
                    eligibleEmployeeIds.Add(cp.EmployeeId);
                }
            }

            return eligibleEmployeeIds;
        }
        //private IList<int> CheckToBePromotedEmployees()
        //{
        //    var dateLimit = DateTime.Now.AddYears(-2);
        //    var eligibleEmployeeIds = new List<int>();

        //    // Get current placements (one per employee)
        //    var currentPlacements = db.Placements
        //        .Where(p => p.isCurrent)
        //        .Select(p => new
        //        {
        //            p.EmployeeId,
        //            p.StepId,
        //            p.RankId,
        //            p.AssignmentDate
        //        })
        //        .ToList();

        //    foreach (var current in currentPlacements)
        //    {
        //        // Get all placements for this employee ordered by date descending (newest first)
        //        var employeePlacements = db.Placements
        //            .Where(p => p.EmployeeId == current.EmployeeId&&p.AssignmentDate<= current.AssignmentDate)
        //            .OrderByDescending(p => p.AssignmentDate)
        //            .ToList();

        //        // Start from the current assignment date
        //        DateTime earliestSameStepDate = current.AssignmentDate;

        //        // Walk backwards while StepId is the same
        //        foreach (var placement in employeePlacements)
        //        {
        //            if (placement.StepId == current.StepId && placement.AssignmentDate < earliestSameStepDate)
        //            {
        //                earliestSameStepDate = placement.AssignmentDate;
        //            }
        //            else if (placement.AssignmentDate < earliestSameStepDate)
        //            {
        //                // We hit a different step; stop
        //                break;
        //            }
        //        }

        //        // If earliest date is 2+ years ago, eligible
        //        if (earliestSameStepDate.Date <= dateLimit.Date)
        //        {
        //            eligibleEmployeeIds.Add(current.EmployeeId);
        //        }
        //    }

        //    return eligibleEmployeeIds;
        //}

        //private IList<int> CheckToBePromotedEmployees()
        //{
        //    List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
        //    var dateLimit = DateTime.Now.AddYears(-2).Date;

        //    // Get current placements in user's branches
        //    var currentPlacements = db.Placements
        //        .Where(p => p.isCurrent && myBranches.Contains(p.BranchId))
        //        .Select(p => new { p.EmployeeId, p.StepId, p.AssignmentDate })
        //        .ToList();  // Materialize the query to avoid the AsNoTracking issue

        //    var eligibleEmployeeIds = new List<int>();

        //    foreach (var current in currentPlacements)
        //    {
        //        var curentAssignmet = current.AssignmentDate.AddYears(-2);
        //        // Get all placements for this employee within the 2-year period
        //        var placementsInPeriod = db.Placements
        //            .Where(p => p.EmployeeId == current.EmployeeId &&
        //                   p.AssignmentDate <= current.AssignmentDate &&
        //                   p.AssignmentDate >= curentAssignmet)
        //            .OrderBy(p => p.AssignmentDate)
        //            .ToList();

        //        // Check conditions
        //        if (placementsInPeriod.All(p => p.StepId == current.StepId) &&
        //            placementsInPeriod.First().AssignmentDate <= dateLimit)
        //        {
        //            eligibleEmployeeIds.Add(current.EmployeeId);
        //        }
        //    }

        //    return eligibleEmployeeIds.Distinct().ToList();
        //}

        private IList<int> CheckToBePromotedEmployees()
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var dateLimit = DateTime.Now.AddYears(-2).Date;

            // Get all current placements in user's branches
            var currentPlacements = db.Placements
                .Where(p => p.isCurrent && myBranches.Contains(p.BranchId))
                .Select(p => new { p.EmployeeId, p.StepId, p.AssignmentDate })
                .ToList();

            var eligibleEmployeeIds = new List<int>();

            foreach (var current in currentPlacements)
            {
                // Case 1: Current placement is already ≥2 years old
                if (current.AssignmentDate <= dateLimit)
                {
                    eligibleEmployeeIds.Add(current.EmployeeId);
                    continue;
                }

                // Case 2: Check cumulative duration in same StepId
                // Get all placements with same StepId, ordered by date
                var sameStepPlacements = db.Placements
                    .Where(p => p.EmployeeId == current.EmployeeId 
                                &&p.StepId == current.StepId
                                &&p.AssignmentDate < current.AssignmentDate)
                    .OrderByDescending(p => p.AssignmentDate)
                    .ToList();

                // Calculate total duration in this step
                DateTime? previousDate = null;
                TimeSpan totalDuration = TimeSpan.Zero;

                foreach (var placement in sameStepPlacements)
                {
                    if (previousDate == null)
                    {
                        // For current placement, count from AssignmentDate to now
                        totalDuration += DateTime.Now - placement.AssignmentDate;
                    }
                    else
                    {
                        // For previous placements, count the time until next placement
                        totalDuration += previousDate.Value - placement.AssignmentDate;
                    }
                    previousDate = placement.AssignmentDate;

                    // Stop if we've already accumulated 2 years
                    if (totalDuration.TotalDays >= 365 * 2)
                        break;
                }

                if (totalDuration.TotalDays >= 365 * 2)
                {
                    eligibleEmployeeIds.Add(current.EmployeeId);
                }
            }

            return eligibleEmployeeIds.Distinct().ToList();
        }
        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
        public ActionResult PlacementToBePromoteds_Read([DataSourceRequest]DataSourceRequest request,string stepId,string gradeId)
       {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
           
            DateTime currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            // Base query with common conditions
            var eligiblePlacements = db.Placements.Where(p => p.isCurrent
                                                         && myBranches.Contains(p.BranchId)
                                                         && p.NextSalaryChangeDate.HasValue)
                                                 .Where(p =>// Case 1: Next salary change is in current month/year
                                                     (p.NextSalaryChangeDate.Value.Month == currentMonth &&
                                                      p.NextSalaryChangeDate.Value.Year == currentYear) ||
                                                     // Case 2: Next salary change is any past date
                                                     p.NextSalaryChangeDate.Value <= currentDate);

            // Add business unit filter if needed
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                eligiblePlacements = eligiblePlacements.Where(p => myBusinessUnits.Contains(p.BusinessUnitId));
            }

            // Optional step filter
            if (!string.IsNullOrEmpty(stepId) && int.TryParse(stepId, out int parsedStepId))
            {
                eligiblePlacements = eligiblePlacements.Where(x => x.StepId == parsedStepId);
            }

            // Optional grade/rank filter
            if (!string.IsNullOrEmpty(gradeId) && int.TryParse(gradeId, out int parsedGradeId))
            {
                eligiblePlacements = eligiblePlacements.Where(x => x.RankId == parsedGradeId); // Assuming RankId is GradeId
            }


            DataSourceResult result = eligiblePlacements.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeCode = placement.Employee.EmployeeCode,
                EmployeeId = placement.EmployeeId,
                HeadId = placement.HeadId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                AssignmentDate = placement.AssignmentDate,
                SalarChangeDate = placement.SalarChangeDate,
                SalarChangeDateAmharic = placement.SalarChangeDateAmharic,
                OrgStructureId = placement.OrgStructureId,
                ParentOrgStructureId = placement.ParentOrgStructureId,
                LookupId = placement.LookupId,
                MappingId = placement.MappingId,
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
                EmployementType = placement.EmployementType,
                IsPend = placement.IsPend,
                BusinessUnitId = placement.BusinessUnitId,
                IsApprove = placement.IsApprove,
                IsToBePromoted = true,
                //IsToBePromoted = empIds.Contains(placement.EmployeeId),
                NextSalaryChangeDateAmharic = placement.NextSalaryChangeDateAmharic,
                NextSalaryChangeDate = placement.NextSalaryChangeDate,
                HireDate = placement.Employee.HiredDateAmharic,
                HireDateGorg = placement.Employee.HiredDate,
                //IsToBePromoted = CheckToBePromoted(placement.EmployeeId),
                //FullName = GetFullName(placement.EmployeeId),
                //HireDate = HrHelper.GetHireDate(placement.EmployeeId),
                //Age = EmployeeHelper.GetAge(placement.EmployeeId)
            });

            return Json(result);
        }
        [HttpPost]
        public JsonResult GetPromoteEmployee(int fromRank, int toRank, int fromStep, int toStep)
        {
            try
            {
                // Validate input parameters
                if (fromStep == toStep && fromRank == toRank)
                    return Json("'From' and 'To' steps/ranks cannot be identical.", JsonRequestBehavior.AllowGet);

                var step1 = db.Steps.AsNoTracking().FirstOrDefault(s => s.StepId == fromStep);
                var step2 = db.Steps.AsNoTracking().FirstOrDefault(s => s.StepId == toStep);
                var grade1 = db.Ranks.AsNoTracking().FirstOrDefault(r => r.RankId == fromRank);
                var grade2 = db.Ranks.AsNoTracking().FirstOrDefault(r => r.RankId == toRank);

                if (step1 == null || step2 == null || grade1 == null || grade2 == null)
                    return Json("Invalid step or rank selection.", JsonRequestBehavior.AllowGet);

                if (step2.StepSequence < step1.StepSequence)
                    return Json("Target step must be equal or higher sequence.", JsonRequestBehavior.AllowGet);

                if (grade2.MaximumSalary < grade1.MaximumSalary)
                    return Json("Target rank must have equal or higher maximum salary.", JsonRequestBehavior.AllowGet);

                // Current date
                var now = DateTime.Now;
                var currentMonth = now.Month;
                var currentYear = now.Year;

                // Get eligible employees
                var myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

                var eligibleEmployees = db.Placements
                    .Where(p => p.isCurrent &&
                               p.RankId == fromRank &&
                               p.StepId == fromStep &&
                               myBranches.Contains(p.BranchId) &&
                               p.NextSalaryChangeDate.HasValue)
                    .Where(p => (p.NextSalaryChangeDate.Value.Month == currentMonth &&
                                p.NextSalaryChangeDate.Value.Year == currentYear) ||
                               p.NextSalaryChangeDate.Value <= now)
                    .ToList();

                if (!eligibleEmployees.Any())
                    return Json("No employees eligible for promotion.", JsonRequestBehavior.AllowGet);

                // Verify target salary
                var targetSalary = db.SalariesJobTitles
                    .AsNoTracking()
                    .FirstOrDefault(s => s.StepId == toStep && s.RankId == toRank);

                if (targetSalary == null || targetSalary.Salary <= 0)
                    return Json("Target salary not configured.", JsonRequestBehavior.AllowGet);

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var placement in eligibleEmployees)
                        {
                            var originalSalaryDate = placement.SalarChangeDate ?? now;
                            var originalNextDate = placement.NextSalaryChangeDate ?? originalSalaryDate.AddYears(2);

                            DateTime newSalaryDate = originalNextDate;
                            DateTime newNextDate = originalNextDate;

                            // Case 1: If next salary date is in current month and year)
                            if ((originalNextDate.Month == now.Month&& originalNextDate.Year==now.Year)
                                ||(originalNextDate > now && originalNextDate <= now.AddMonths(1)))
                            {
                                // Keep same day in current year
                                newSalaryDate = originalNextDate;
                                newNextDate = newSalaryDate.AddYears(2);
                            }
                           
                            // Case 2: Past due dates (next date is in past)
                            else if (originalNextDate <= now)
                            {
                                // Find next future cycle
                                newSalaryDate = originalNextDate;
                                while ((newSalaryDate <= now || newSalaryDate <= now.AddMonths(1))
                                    && newSalaryDate.AddYears(2)<= now.AddMonths(1))
                                {
                                    newSalaryDate = newSalaryDate.AddYears(2);

                                }
                                newNextDate = newSalaryDate.AddYears(2);

                            }
                            //Future dates beyond 1 month (2024-06-15/2026-06-15)
                            else
                            {
                                // work as usual
                                newSalaryDate = originalNextDate;
                                newNextDate = newSalaryDate.AddYears(2);

                            }

                            // Archive current placement
                            PlacementAfterSystem_Create(placement, now);

                            // Create new placement with all required fields
                            var newPlacement = new Placement
                            {
                                EmployeeId = placement.EmployeeId,
                                HeadId = placement.HeadId,
                                ImmediateSupervisor = placement.ImmediateSupervisor,
                                IsHead = placement.IsHead,
                                //AssignmentDate = now,
                                //AssignmentDateAmharic = GetEthiopicDate(now),
                                AssignmentDate = newSalaryDate,
                                AssignmentDateAmharic = HrHelper.GetECDate(newSalaryDate),
                                SalarChangeDate = newSalaryDate,
                                SalarChangeDateAmharic = HrHelper.GetECDate(newSalaryDate),
                                NextSalaryChangeDate = newNextDate,
                                NextSalaryChangeDateAmharic = HrHelper.GetECDate(newNextDate),
                                MappingId = placement.MappingId,
                                RankId = toRank,
                                StepId = toStep,
                                Salary = targetSalary.Salary,
                                AssignmentType = AssignmentType.SCALE_PROMOTION,
                                LookupId = placement.LookupId,
                                OrgStructureId = placement.OrgStructureId,
                                ParentOrgStructureId = placement.ParentOrgStructureId,
                                ReportsTo = placement.ReportsTo,
                                JobTitleName = placement.JobTitleName,
                                Department = placement.Department,
                                ReportsToMappingId = placement.ReportsToMappingId,
                                ResponsibilityLevel = placement.ResponsibilityLevel,
                                BranchId = placement.BranchId,
                                BusinessUnitId = placement.BusinessUnitId,
                                Remark = $"Promoted from {step1.StepName} to {step2.StepName}",
                                DateCreated = now,
                                LastModified = now,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = User.Identity.Name,
                                isCurrent = true,
                                IsApprove = false,
                                IsPend = false,
                                IsTemporary = false,
                                Index = placement.Index,
                                EmployementType = placement.EmployementType,
                                EmployeeCode = placement.EmployeeCode,
                                OrgBranchName = placement.OrgBranchName,
                                PreparedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name)
                            };

                            db.Placements.Add(newPlacement);
                        }

                        db.SaveChanges();
                        transaction.Commit();

                        return Json($"Successfully promoted {eligibleEmployees.Count} employees.", JsonRequestBehavior.AllowGet);
                    }
                    catch (DbEntityValidationException ex)
                    {
                        transaction.Rollback();
                        var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                        return Json($"Validation errors: {string.Join("; ", errorMessages)}", JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Json($"Promotion failed: {ex.Message}", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json($"System error: {ex.Message}", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPromoteEmployeeOriginal(int fromRank, int toRank, int fromStep, int toStep)
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

            if (step2.StepSequence < step1.StepSequence)
                return Json("Please select a step with a greater than or equal to sequence.", JsonRequestBehavior.AllowGet);

            if (grade2.MaximumSalary < grade1.MaximumSalary)
                return Json("The selected rank has a higher maximum salary than the current rank.", JsonRequestBehavior.AllowGet);

            //var employeeIds = CheckToBePromotedEmployees();


            // Filter once from the DB with distinct EmployeeId using GroupBy
            ///

            //var employeePlacements = db.Placements
            //                        .Where(x => employeeIds.Contains(x.EmployeeId)
            //                                    &&x.RankId==fromRank 
            //                                    &&x.StepId==fromStep && x.isCurrent)
            //                                    .ToList();
            var  nextSalaryChangedDate = DateTime.Now;
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<Placement> employeePlacements = db.Placements
                .Where(x => x.isCurrent && myBranches.Contains(x.BranchId)
                        && x.RankId == fromRank
                        &&x.StepId==fromStep
                        &&( (x.NextSalaryChangeDate.HasValue
                        && x.NextSalaryChangeDate.Value.Month == nextSalaryChangedDate.Month
                        && x.NextSalaryChangeDate.Value.Year == nextSalaryChangedDate.Year)
                        || x.NextSalaryChangeDate.Value <= nextSalaryChangedDate));

            if (!employeePlacements.Any())
                return Json("No employees are placed at the selected rank and step.", JsonRequestBehavior.AllowGet);

            var salary = db.SalariesJobTitles
                .FirstOrDefault(x => x.StepId == toStep && x.RankId == toRank);

            if (salary == null || salary.Salary <= 0)
                return Json("Please update the salary settings first. The salary must be greater than zero.", JsonRequestBehavior.AllowGet);

            //// Cache commonly used values
            //var inputsList = CacheData.GetEmployeeCaches()
            //             .Select(i => new
            //             {
            //                 FullNameAmharic = i.AmharicName,
            //                 FullNameEnglish = i.Name,        // Typo fixed: "FullNameaEnglish" -> "FullNameEnglish"
            //                EmployeeId = i.Id,
            //                 HeadId = i.Id                    // If this is not a mistake, it's fine; otherwise see note below.
            //            }).ToList();



            var defaultBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            var preparedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);


            int updatedCount = 0;
            foreach (var placement in employeePlacements)
            {
                var now = DateTime.Now;
                var oneMonthAhead = now.AddMonths(1);

                // Skip null placements for safety
                if (placement == null)
                    continue;

                // Base salary change logic
                var salaryChangeDate = placement.SalarChangeDate ?? placement.AssignmentDate;
                if (placement.NextSalaryChangeDate.HasValue && placement.NextSalaryChangeDate.Value.Date <= oneMonthAhead.Date)
                {
                    salaryChangeDate = placement.NextSalaryChangeDate.Value;
                }

                // Next salary change after 2 years
                var nextSalaryChange = salaryChangeDate.AddYears(2);

                // If both salaryChangeDate and nextSalaryChange are in past, calculate the upcoming ones
                if (salaryChangeDate.Date <= oneMonthAhead.Date && nextSalaryChange.Date <= oneMonthAhead.Date)
                {
                    while (nextSalaryChange <= oneMonthAhead.Date)
                    {
                        salaryChangeDate = nextSalaryChange;
                        nextSalaryChange = salaryChangeDate.AddYears(2);
                    }
                }

                // Ethiopic conversions - isolated method (to avoid repetitive code)
                //string GetEthiopicDate(DateTime dt) => HrHelper.GetECDate(dt);

                string amharicNow = HrHelper.GetECDate(now);
                string amharicSalaryChange = HrHelper.GetECDate(salaryChangeDate);
                string amharicNextPromotion = HrHelper.GetECDate(nextSalaryChange);

                // Archive current placement
                PlacementAfterSystem_Create(placement, now);

                // Create new placement (clone relevant info + modify)
                var newPlacement = new Placement
                {
                    EmployeeId = placement.EmployeeId,
                    HeadId = placement.HeadId,
                    ImmediateSupervisor = placement.ImmediateSupervisor,
                    IsHead = placement.IsHead,
                    AssignmentDate = now,
                    AssignmentDateAmharic = amharicNow,
                    SalarChangeDate = salaryChangeDate,
                    SalarChangeDateAmharic = amharicSalaryChange,
                    NextSalaryChangeDate = nextSalaryChange,
                    NextSalaryChangeDateAmharic = amharicNextPromotion,
                    MappingId = 0,
                    RankId = toRank,
                    StepId = toStep,
                    Salary = salary?.Salary ?? 0,
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
                    Remark = "Promoted by Scale",
                    DateCreated = now,
                    LastModified = placement.LastModified,
                    CreatedBy = User.Identity?.Name ?? "System",
                    ModifiedBy = placement.ModifiedBy,
                    isCurrent = false,
                    IsApprove = false,
                    IsPend = false,
                    IsTemporary = false,
                    Index = placement.Index,
                    EmployementType = placement.EmployementType,
                    EmployeeCode = placement.Employee.EmployeeCode,
                    OrgBranchName = defaultBranch,
                    PreparedByEmpId = preparedByEmpId
                };

                db.Placements.Add(newPlacement);

                // Update person record only once
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
                if (person != null)
                {
                    person.IsActive = true;
                    person.BranchId = newPlacement.BranchId;
                    person.Status = newPlacement.EmployementType;
                    person.OrgBranchName = newPlacement.BranchId.ToString(); // Optional: map actual name
                    db.Entry(person).State = EntityState.Modified;
                }

                updatedCount++;
            }

            db.SaveChanges(); // Save once after loop

            return Json($"Salary has been promoted for {updatedCount} employee(s). Approve them in the main page.", JsonRequestBehavior.AllowGet);
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
        public void PlacementAfterSystem_Create(Placement placement, DateTime dateNewAssignmet)
        {
            if (placement != null)
            {
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
                place.FromAm = HrHelper.GetECDate(placement.AssignmentDate);
                place.ToAm = HrHelper.GetECDate(dateNewAssignmet);
                db.PlacementAfterSystems.Add(place);
                db.SaveChanges();
            }
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
                FromDateAm = HrHelper.GetECDate(now),
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
        private string GetTitleName(int id)
        {
            try
            {
                using (var db = new ExceedDbContext())
                {
                    return db.Lookups
                             .Where(x => x.LookupId == id)
                             .Select(x => x.Description)
                             .FirstOrDefault() ?? "";
                }
            }
            catch
            {
                return "";
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}