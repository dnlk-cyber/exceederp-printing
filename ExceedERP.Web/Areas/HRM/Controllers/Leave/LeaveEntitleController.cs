using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{

    [AuthorizeRoles(HrmConstants.HRLeaveUser, HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveAdmin)]
    public class LeaveEntitleController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        //HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["PeriodList"] = CacheData.GetFiscalYearCaches().Select(
             i => new
             {
                 Name = i.Name,
                 GlFiscalYearId = i.Id,
             }).AsQueryable();

            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
              i => new
              {
                  FullNameAmharic = i.AmharicName,
                  FullNameaEnglish = i.Name,
                  EmployeeId = i.Id,
              }).AsQueryable();
            ViewData["branch"] = CacheData.GetBranchCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BranchId = i.Id,
               }).AsQueryable(); ;
            
        }

        #region generate Leaveenttle
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin)]
        public JsonResult GetEmployeeLeaveEntitlementNewAwaj(int period, string employeeId)
        {
            try
            {
            // 1. Validate current fiscal year
            var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
            if (currentPeriod == null)
            {
                return Json(new
                {
                    Message = "You are trying to generate entitlement for an undefined fiscal year.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var periodDateTo = currentPeriod.DateTo.Date;

            decimal incrementalAmount = 0;

            var setting = db.LeaveEntitlementSettings
                            .FirstOrDefault(x => x.GlFiscalYearId == period);

            if (setting == null)
            {
                setting = new LeaveEntitlementSetting
                {
                    GlFiscalYearId = period,
                    IsGenerated = true,
                    IsRegeneratingAllowed = false,
                    LeaveIncrementAmount = 1,
                    Remark = "Generated first time"
                };
                incrementalAmount = 1;
                db.LeaveEntitlementSettings.Add(setting);
            }
            else
            {
                if (!setting.IsRegeneratingAllowed)
                {
                    bool entitlementExists = false;

                    if (string.IsNullOrEmpty(employeeId))
                    {
                        // No employeeId provided → block if ANY entitlement exists
                        entitlementExists = db.LeavesEntitle
                                              .Any(x => x.GlFiscalYearId == period);
                    }
                    else
                    {
                        // Specific employee check
                        entitlementExists = db.LeavesEntitle
                                              .Any(x => x.GlFiscalYearId == period &&
                                                       x.EmployeeId.ToString() == employeeId);
                    }

                    if (entitlementExists)
                    {
                        return Json(new
                        {
                            Status = "NOT_OK",
                            Message = "Leave entitlement has already been generated for this fiscal year and regeneration is disabled. Enable 'Is Regenerating Allowed' in Leave Setting if you need to regenerate it."
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                setting.IsGenerated = true;
                setting.Remark = "Regenerated";
                incrementalAmount = setting.LeaveIncrementAmount;

            }

            // 2. Validate initial Awaj settings
            var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
            if (newAwajInitialYear == null)
            {
                return Json(new
                {
                    Message = "Please set the 'New Awaj Start Year' in Leave Setting before generating leave entitlement.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var initialGlFiscalYear = db.GLFiscalYears
                .FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
            if (initialGlFiscalYear == null)
            {
                return Json(new
                {
                    Message = "The fiscal year configured as the 'New Awaj Start Year' in Leave Setting no longer exists. Please review that setting.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 3. Validation leave entitlement
            var validationResult = ValidateLeaveSettings();
            if (validationResult != null)
            {
                return validationResult;
            }

            // 4. Load configuration values
            var (maxLeaveConfig, maxLeaderConfig, thresholdLeave, forleadersnew) = LoadConfigurationValues();
            var missingConfigurations = new List<string>();

            if (maxLeaveConfig == null || string.IsNullOrWhiteSpace(maxLeaveConfig.CurrentValue))
                missingConfigurations.Add(LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());

            if (maxLeaderConfig == null || string.IsNullOrWhiteSpace(maxLeaderConfig.CurrentValue))
                missingConfigurations.Add(LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());

            if (thresholdLeave == null || string.IsNullOrWhiteSpace(thresholdLeave.CurrentValue))
                missingConfigurations.Add(LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

            if (forleadersnew == null || string.IsNullOrWhiteSpace(forleadersnew.CurrentValue))
                missingConfigurations.Add(LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());

            if (missingConfigurations.Any())
            {
                return Json(new
                {
                    Message = "The following required configuration values are missing or not set: "
                              + string.Join(", ", missingConfigurations),
                    Status = "NOT_OK",
                    MissingConfigurations = missingConfigurations
                }, JsonRequestBehavior.AllowGet);
            }

            int maximumValue = int.Parse(maxLeaveConfig.CurrentValue);
            int LeaveTrusholdValue = int.Parse(thresholdLeave.CurrentValue);
            int leaveTresholdForLeaderdrsNew = int.Parse(forleadersnew.CurrentValue);
            int maximumForLeader = int.Parse(maxLeaderConfig.CurrentValue);

            // 5. Find previous fiscal year
            var previousYear = GetPreviousFiscalYear(currentPeriod);
            if (previousYear == null)
            {
                return Json(new
                {
                    Message = "You can only generate leave entitlement if it has already been generated for the previous fiscal year.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            //// 6. Load and filter data
            //var (activePlacements, prevEntitlements, currentEntitlements)  =  LoadEmployeeData(employeeId, previousYear.GlFiscalYearId, currentPeriod.GlFiscalYearId);
            var allEntitlements = db.LeavesEntitle.AsNoTracking().ToList();

            var activePlacements = db.Placements.Where(x => x.isCurrent).ToList();
            var prevEntitlements = allEntitlements.Where(x => x.GlFiscalYearId == previousYear.GlFiscalYearId).ToList();
            var currentEntitlements = allEntitlements.Where(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId).ToList();

            if (!string.IsNullOrEmpty(employeeId) && int.TryParse(employeeId, out int empId) && empId != 0)
            {
                activePlacements = activePlacements.Where(x => x.EmployeeId == empId).ToList();
                prevEntitlements = prevEntitlements.Where(x => x.EmployeeId == empId).ToList();
                currentEntitlements = currentEntitlements.Where(x => x.EmployeeId == empId).ToList();
                if (empId > 0)
                {
                    var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);

                    if (personEmployee != null && personEmployee.HiredDate.Date > periodDateTo)
                    {
                        return Json(new
                        {
                            Message = $"This Employee is hired on {personEmployee.HiredDate:MMMM dd, yyyy}, so They can't have leave entitlement for {currentPeriod.Name} fiscal year.",
                            Status = "NOT_OK"
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            // 7. Clean up duplicate entitlements for current period FIRST
            CleanupDuplicateEntitlements(currentPeriod.GlFiscalYearId, activePlacements.Select(x => x.EmployeeId).ToList());

            // 8. Process entitlements
            ProcessEntitlements(activePlacements, prevEntitlements, currentEntitlements, currentPeriod,
                               newAwajInitialYear.GlFisicalYearId, maximumValue, maximumForLeader,
                               LeaveTrusholdValue, leaveTresholdForLeaderdrsNew, incrementalAmount);
            db.SaveChanges();
            // 9. Log operation
            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Leave Entitlement",
                "Success",
                User.Identity.Name,
                $"Leave entitlement for fiscal year {currentPeriod?.Name} was generated by {User.Identity.Name}",
                Modules.HRM);

            return Json(new
            {
                Message = "Leave entitlement successfully generated.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException ex)
            {
                var detail = string.Join(" | ", ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                return Json(new
                {
                    Message = "An error occurred while generating leave entitlement: " + detail,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;
                return Json(new
                {
                    Message = "An error occurred while generating leave entitlement: " + detail,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Helper Methods

        private JsonResult ValidateLeaveSettings()
        {
            var requiredSettings = new[]
            {
                    (LeaveSetting.LEAVE_FOR_LEADRES_OLD, "Leave for leaders old initial value"),
                    (LeaveSetting.LEAVE_FOR_PERFORMERS_OLD, "Leave for performers old initial value"),
                    (LeaveSetting.LEAVE_FOR_LEADRES_NEW, "Leave for leaders new initial value"),
                    (LeaveSetting.LEAVE_FOR_PERFORMERS_NEW, "Leave for performers new initial value"),
                    (LeaveSetting.LEAVE_ENTITLE_THRESHOLD, $"Leave {LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString().ToLower()} new initial value"),
                    (LeaveSetting.LEAVE_ENTITLEMENT_INCREEMENT_YEAR, $"Leave {LeaveSetting.LEAVE_ENTITLEMENT_INCREEMENT_YEAR.ToString().ToLower()} new initial value"),

            };

            foreach (var (setting, message) in requiredSettings)
            {
                var config = db.Configurations.FirstOrDefault(c => c.Attribute == setting.ToString());
                if (config == null || config.CurrentValue == null)
                {
                    return Json(new { Message = $"You have to add {message}", Status = "NOT_OK" }, JsonRequestBehavior.AllowGet);
                }
            }

            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
            if (frequency == null || string.IsNullOrEmpty(frequency.CurrentValue))
            {
                return Json(new { Message = "You have to add frequency in HRM Setting with current value", Status = "NOT_OK" }, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        private (Configuration maxLeave, Configuration maxLeader, Configuration threshold, Configuration forLeadersNew) 
            LoadConfigurationValues()
        {
            var maxLeaveConfig = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            var maxLeaderConfig = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());
            var thresholdLeave = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());
            var forleadersnew = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());

            return (maxLeaveConfig, maxLeaderConfig, thresholdLeave, forleadersnew);
        }


        private GlFiscalYear GetPreviousFiscalYear(GlFiscalYear currentPeriod)
        {
            var maxPrevYearFrom = db.GLFiscalYears
                .Where(x => x.DateFrom < currentPeriod.DateFrom)
                .Max(x => (DateTime?)x.DateFrom);

            return db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == maxPrevYearFrom);
        }

     


        private void CleanupDuplicateEntitlements(int currentYearId, List<int> employeeIds)
        {
            try
            {
                using (var freshContext = new ExceedDbContext()) // Replace YourDbContext with your actual DbContext
                {
                    // Find all entitlements for the current year and specified employees
                    var allEntitlements = freshContext.LeavesEntitle
                        .Where(x => x.GlFiscalYearId == currentYearId && employeeIds.Contains(x.EmployeeId))
                        .ToList();

                    // Group by EmployeeId and FiscalYear to find duplicates
                    var employeeGroups = allEntitlements
                        .GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId })
                        .Where(g => g.Count() > 1)
                        .ToList();

                    var duplicatesToRemove = new List<LeaveEntitle>();

                    foreach (var group in employeeGroups)
                    {
                        var entitlements = group.ToList();

                        // Check for entitlements where Balance != Amount (indicating usage)
                        var usedEntitlements = entitlements.Where(e => e.Balance != e.Amount).ToList();

                        if (usedEntitlements.Any())
                        {
                            // If multiple used entitlements, keep the one with most usage (lowest balance)
                            // or if only one used, keep that one
                            var keepEntitlement = usedEntitlements
                                .OrderBy(e => e.Balance) // Lowest balance = most used
                                .ThenByDescending(e => e.DateCreated)
                                .First();

                            var removeInGroup = entitlements.Where(e => e.LeaveEntitleId != keepEntitlement.LeaveEntitleId).ToList();
                            duplicatesToRemove.AddRange(removeInGroup);
                        }
                        else
                        {
                            // No entitlements are used (Balance == Amount), keep the one with highest amount
                            // or if amounts are equal, keep the most recent one
                            var keepEntitlement = entitlements
                                .OrderByDescending(e => e.Amount)
                                .ThenByDescending(e => e.DateCreated)
                                .First();

                            var removeInGroup = entitlements.Where(e => e.LeaveEntitleId != keepEntitlement.LeaveEntitleId).ToList();
                            duplicatesToRemove.AddRange(removeInGroup);
                        }
                    }

                    if (duplicatesToRemove.Any())
                    {
                        freshContext.LeavesEntitle.RemoveRange(duplicatesToRemove);
                        freshContext.SaveChanges();

                        System.Diagnostics.Debug.WriteLine($"Removed {duplicatesToRemove.Count} duplicate entitlements");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cleaning up duplicates: {ex.Message}");
            }
        }
        private void ProcessEntitlements(List<Placement> activePlacements, List<LeaveEntitle> prevEntitlements,
                                          List<LeaveEntitle> currentEntitlements, GlFiscalYear currentPeriod, int initialYearId,
                                          int maximumValue, int maximumForLeader, int LeaveTrusholdValue, int leaveTresholdForLeaderdrsNew, decimal incrementalAmount)
        {
            // Clean up duplicates BEFORE processing
            var employeeIds = activePlacements.Select(x => x.EmployeeId).ToList();
            CleanupDuplicateEntitlements(currentPeriod.GlFiscalYearId, employeeIds);

            // Refresh current entitlements after cleanup
            currentEntitlements = db.LeavesEntitle
                .AsNoTracking()
                .Where(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId &&
                           employeeIds.Contains(x.EmployeeId))
                .ToList();

            foreach (var empPlacement in activePlacements)
            {
                try
                {
                    // New entitlement path
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
                    if (person == null)
                        continue;
                    // Skip if hired after current fiscal year DateTo
                    if (person.HiredDate.Date > currentPeriod.DateTo.Date)
                        continue;

                    var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
                    var currentEntitlement = currentEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);

                    bool isLeader = empPlacement.ResponsibilityLevel != ResponsibilityLevel.STAFF;
                    int toBeIncrementPerYear = HrHelper.GetLeaveIncrementPerYear(empPlacement.ResponsibilityLevel);

                    if (prevEntitlement != null)
                    {
                        ProcessExistingEmployee(empPlacement, prevEntitlement, currentEntitlement, currentPeriod, initialYearId, maximumValue, maximumForLeader, leaveTresholdForLeaderdrsNew, isLeader, toBeIncrementPerYear, incrementalAmount);
                    }
                    else
                    {
                        ProcessNewEmployee(empPlacement, currentEntitlement, currentPeriod, LeaveTrusholdValue, leaveTresholdForLeaderdrsNew, isLeader, toBeIncrementPerYear, incrementalAmount);
                    }

                    // Save changes for each employee
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    HandleConcurrencyException(ex, empPlacement.EmployeeId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing employee {empPlacement.EmployeeId}: {ex.Message}");
                }
            }
        }

        private void HandleConcurrencyException(DbUpdateConcurrencyException ex, int employeeId)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is LeaveEntitle)
                {
                    var databaseValues = entry.GetDatabaseValues();
                    if (databaseValues == null)
                    {
                        // Entity was deleted in the database
                        System.Diagnostics.Debug.WriteLine($"Leave entitlement for employee {employeeId} was deleted in database.");
                        entry.State = EntityState.Detached;
                    }
                    else
                    {
                        // Entity was modified in the database - refresh local entity
                        System.Diagnostics.Debug.WriteLine($"Leave entitlement for employee {employeeId} was modified in database. Refreshing...");
                        entry.OriginalValues.SetValues(databaseValues);

                        // Optionally: You can choose to retry the operation here
                        // or simply skip this employee for now
                    }
                }
            }
        }

        private void ProcessExistingEmployee(Placement empPlacement, LeaveEntitle prevEntitlement, LeaveEntitle currentEntitlement,
                                            GlFiscalYear currentFiscalYear, int initialYearId, int maximumValue,
                                            int maximumForLeader, int minimumForLeaderdrsNew, bool isLeader, int toBeIncrementPerYear, decimal incrementalAmount)
        {
            // Calculate entitlement amount
            //decimal amount = (currentFiscalYear.GlFiscalYearId == initialYearId)
            //    ? prevEntitlement.Amount + 1
            //    : prevEntitlement.Amount;
            bool isIncrementAllowed = toBeIncrementPerYear <= 1;

            if (!isIncrementAllowed)
            {
                DateTime checkDate = currentFiscalYear.DateFrom.AddYears(-toBeIncrementPerYear);

                var fiscalYear = db.GLFiscalYears
                    .FirstOrDefault(x => x.DateFrom <= checkDate && checkDate < x.DateTo);

                var prevPrevEntitlement = fiscalYear == null ? null :
                    db.LeavesEntitle.FirstOrDefault(x =>
                        x.EmployeeId == empPlacement.EmployeeId &&
                        x.GlFiscalYearId == fiscalYear.GlFiscalYearId);

                isIncrementAllowed =
                    prevPrevEntitlement != null &&
                    prevEntitlement != null &&
                    prevPrevEntitlement.Amount == prevEntitlement.Amount;
            }
            decimal entitleAmount = prevEntitlement.Amount;
            if (isLeader && entitleAmount < minimumForLeaderdrsNew)
                entitleAmount = minimumForLeaderdrsNew;

            //decimal amount = isIncrementAllowed? prevEntitlement.Amount + 1: prevEntitlement.Amount;
            //decimal amount = isIncrementAllowed? entitleAmount + 1: entitleAmount;
            decimal amount = isIncrementAllowed ? entitleAmount + incrementalAmount : entitleAmount;

            decimal cappedAmount = isLeader
                ? Math.Min(amount, maximumForLeader)
                : Math.Min(amount, maximumValue);

            if (currentEntitlement == null)
            {
                CreateNewEntitlement(empPlacement, currentFiscalYear, cappedAmount, cappedAmount);
            }
            else
            {
                UpdateExistingEntitlement(currentEntitlement, currentFiscalYear, cappedAmount, cappedAmount);
            }
        }

        private void ProcessNewEmployee(Placement empPlacement, LeaveEntitle currentEntitlement, GlFiscalYear currentPeriod, int LeaveTrusholdValue,
                                            int leaveTresholdForLeaderdrsNew, bool isLeader, int toBeIncrementPerYear, decimal incrementalAmount)
        {
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
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
            if (hireDate.Date > currentPeriod.DateTo.Date) return;

            if (currentEntitlement != null)
            {
                UpdateExistingEntitlement(currentEntitlement, currentPeriod, newEntitle, newAmount);
            }
            else
            {
                CreateNewEntitlement(empPlacement, currentPeriod, newAmount, newEntitle);
            }
        }

        private void CreateNewEntitlement(Placement empPlacement, GlFiscalYear currentPeriod, decimal balance, decimal? amount = null)
        {
            // Check if entitlement already exists to avoid duplicates

            var existing = db.LeavesEntitle.Local.FirstOrDefault(x =>
                x.EmployeeId == empPlacement.EmployeeId && x.GlFiscalYearId == currentPeriod.GlFiscalYearId)
                ?? db.LeavesEntitle.FirstOrDefault(x =>
                    x.EmployeeId == empPlacement.EmployeeId && x.GlFiscalYearId == currentPeriod.GlFiscalYearId);

            if (existing != null)
            {
                // Update existing instead of creating new
                UpdateExistingEntitlement(existing, currentPeriod, amount, balance);
                return;
            }

            var entitle = new LeaveEntitle
            {
                GlFiscalYearId = currentPeriod.GlFiscalYearId,
                EmployeeId = empPlacement.EmployeeId,
                EmployeeCode = empPlacement.EmployeeCode,
                BranchId = empPlacement.BranchId,
                OrgBranchName = empPlacement.BranchId.ToString(),
                Amount = amount ?? balance,
                Balance = balance,
                IsAdded = true,
                IsExpired = false,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name
            };
            db.LeavesEntitle.Add(entitle);
        }

        private void UpdateExistingEntitlement(LeaveEntitle entitlement, GlFiscalYear currentPeriod, decimal? newEntitlement, decimal amount)
        {
            // Reload the entity to ensure we have the latest version
            var freshEntitlement = db.LeavesEntitle.Find(entitlement.LeaveEntitleId);
            if (freshEntitlement == null)
            {
                // Entity was deleted, create new one
                CreateNewEntitlement(
                    db.Placements.First(x => x.EmployeeId == entitlement.EmployeeId && x.isCurrent),
                    currentPeriod, amount);
                return;
            }

            freshEntitlement.IsAdded = true;
            freshEntitlement.IsExpired = false;
            freshEntitlement.Amount = newEntitlement ?? amount;
            freshEntitlement.Balance = amount;
            freshEntitlement.GlFiscalYearId = currentPeriod.GlFiscalYearId;
            freshEntitlement.LastModified = DateTime.Now;
            freshEntitlement.ModifiedBy = User.Identity.Name;

            db.Entry(freshEntitlement).State = EntityState.Modified;
        }

        #endregion

        [AuthorizeRoles( HrmConstants.HRLeaveAdmin)]
        #region Entitlement By New Awaj ... add 1 day per two years and intial 16
        //public JsonResult GetEmployeeLeaveEntitlemntNewAwajExisted(int period, string employeeId)
        //{
        //    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
        //    if (currentPeriod == null)
        //    {
        //        return Json(new
        //        {
        //            Message = "You are trying to generate for undefined fiscal Year",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
        //    var message = "";
        //    #region Validation
        //    if (newAwajInitialYear == null)
        //    {
        //         message = "Please Define Setting for Initial year first";
        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var initialGlFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
        //    if (initialGlFiscalYear ==null)
        //    {
        //        message = "You have to  set  initial year first or Increment will Happen";
        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
            
        //    var validation = Validation_For_Entitlement_Generation();
        //    if (!string.IsNullOrEmpty(validation))
        //    {
        //        return Json(new
        //        {
        //            Message = validation,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());
        //    int leaveThresholdValue = 0;
        //    if(leaveThrashold != null)
        //    {

        //        leaveThresholdValue = Convert.ToInt32(leaveThrashold.CurrentValue);

        //    }
        //    int maximumValue = 0;
        //    int maximumForLeader = 0;
        //    var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
        //    if (maximumLeave != null)
        //    {
        //        maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
        //    }
        //    else
        //    {

        //        message = "You have to set MAXIMUM_LEAVE_ENTITLEMENT first";
        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var maxLeader = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());
        //    if (maxLeader != null)
        //    {
        //        maximumForLeader = Convert.ToInt32(maxLeader.CurrentValue);
        //    }
        //    else
        //    {
        //        message = "You have to set MAXIMUM_Leave_For_Leaders setting first";
        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    #endregion


        //    decimal amount = 0;
        //    //DateTime PrevYearDateFrom = currentPeriod.DateFrom.AddYears(-1);
        //    //DateTime PrevYearDateTo = currentPeriod.DateTo.AddYears(-1);
        //    var maxPrevYearFrom = db.GLFiscalYears.Where(x => x.DateFrom< currentPeriod.DateFrom).Max(x=>x.DateFrom);
        //    var previousYear = db.GLFiscalYears.Where(x => x.DateFrom==maxPrevYearFrom).FirstOrDefault();
        //    if (previousYear == null)
        //    {
        //        return Json(new
        //        {
        //            Message = "You can only generate leave entitlement if it has already been generated for the previous fiscal year.",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    var prevEntitlements = db.LeavesEntitle.Where(x => x.GlFiscalYearId == previousYear.GlFiscalYearId).AsNoTracking().ToList();
        //    var activeplacements = db.Placements.Where(x=>x.isCurrent).ToList();
        //    if(!string.IsNullOrEmpty(employeeId)&&int.TryParse(employeeId,out int empId) && empId != 0)
        //    {
        //        activeplacements = activeplacements.Where(x => x.EmployeeId == empId).ToList();
        //        prevEntitlements = prevEntitlements.Where(x => x.EmployeeId == empId).ToList();
        //    }
        //    foreach (var empPlacement in activeplacements)
        //    {
        //        var profile = db.Person_Employee.Find(empPlacement.EmployeeId); 
        //        var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
        //        var currentEntitle = db.LeavesEntitle
        //                                        .FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId 
        //                                                            && x.EmployeeId == empPlacement.EmployeeId
        //                                                            &&!x.IsExpired);
        //        var icreementSetting = db.Configurations
        //            .Where(x => x.Attribute == LeaveSetting.LEAVE_ENTITLEMENT_INCREEMENT_YEAR.ToString())
        //            .FirstOrDefault()?.CurrentValue; ;
        //        int.TryParse(icreementSetting,out int perYear);
        //        var yearIds = db.GLFiscalYears.OrderByDescending(x => x.GlFiscalYearId).Take(3).Select(x => x.GlFiscalYearId);
        //        var allLeaveEntitlements = db.LeavesEntitle.Where(x => yearIds.Contains(x.GlFiscalYearId));
        //        var isLeader = empPlacement.ResponsibilityLevel != ResponsibilityLevel.STAFF;
        //        var entitle = new LeaveEntitle();
        //        if (currentEntitle == null)
        //        {

        //            if (prevEntitlement != null)
        //            {
        //                //because the increment can be per 2 year or three year
        //                amount = (currentPeriod.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId) ? prevEntitlement.Amount + 1 : prevEntitlement.Amount;
        //                entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                entitle.EmployeeId = empPlacement.EmployeeId;
        //                entitle.EmployeeCode = empPlacement.EmployeeCode;
        //                entitle.BranchId = empPlacement.BranchId;
        //                entitle.OrgBranchName = empPlacement.BranchId.ToString();
        //                entitle.Amount = isLeader? Math.Min(amount, maximumForLeader)
        //                                        : Math.Min(amount, maximumValue);

        //                entitle.Balance = isLeader ? Math.Min(amount, maximumForLeader)
        //                                        : Math.Min(amount, maximumValue);
                      
        //                entitle.IsAdded = true;
        //                entitle.IsExpired = false;
        //                entitle.DateCreated = DateTime.Now;
        //                entitle.LastModified = null;
        //                entitle.CreatedBy = User.Identity.Name;
        //                entitle.ModifiedBy = null;
        //                db.LeavesEntitle.Add(entitle);
        //                db.SaveChanges();
        //            }
        //        }
        //        else if (currentEntitle != null && prevEntitlement != null)
        //        {
        //            //because the increment can be per 2 year or three year
        //            amount = (currentPeriod.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId) ? prevEntitlement.Amount + 1 : prevEntitlement.Amount;
        //            //initial changes
        //            currentEntitle.Amount = isLeader ? Math.Min(amount, maximumForLeader)
        //                                        : Math.Min(amount, maximumValue);
        //            currentEntitle.Balance = isLeader ? Math.Min(amount, maximumForLeader)
        //                                        : Math.Min(amount, maximumValue);
        //            db.Entry(currentEntitle).State = EntityState.Modified;

        //            db.SaveChanges();
        //        }

        //    }
                
               
            
        //    #region operation log

        //    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for fiscal Year of " + currentPeriod?.Name + "  is Generated By  " + User.Identity.Name, Modules.HRM);

        //    #endregion
        //    message = "Leave Successfully Generated";
        //    return Json(new
        //    {
        //        Message = message,
        //        Status = "OK"
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetEmployeeLeaveEntitlemntNewAwajCopy(bool check, int period)
        //{
        //    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
        //    var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();

        //    #region Validation
        //    if (newAwajInitialYear == null)
        //        return Json(new
        //        {
        //            Message = "Please Select Period First!!",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);

        //    var initialGlFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
        //    if (initialGlFiscalYear == null)
        //        return Json(new
        //        {
        //            Message = "You set an invalid initial year",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);

        //    var validationMessage = Validation_For_Entitlement_Generation();
        //    if (!string.IsNullOrEmpty(validationMessage))
        //        return Json(new
        //        {
        //            Message = validationMessage,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);

        //    var leaveThreshold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

        //    // Fetch Maximum Leave Entitlement and Maximum for Leaders
        //    var maxLeaveEntitlementConfig = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
        //    if (maxLeaveEntitlementConfig == null)
        //        return Json(new
        //        {
        //            Message = "You have to set MAXIMUM_LEAVE_ENTITLEMENT first",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);

        //    int maximumValue = Convert.ToInt32(maxLeaveEntitlementConfig.CurrentValue);

        //    var maxLeaveForLeaderConfig = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());
        //    if (maxLeaveForLeaderConfig == null)
        //        return Json(new
        //        {
        //            Message = "You have to set MAXIMUM_Leave_For_Leaders setting first",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);

        //    int maximumForLeader = Convert.ToInt32(maxLeaveForLeaderConfig.CurrentValue);

        //    var employeeList = db.Person_Employee.Where(x => x.IsActive).AsNoTracking().ToList();
        //    #endregion

        //    if (currentPeriod == null)
        //        return Json(new
        //        {
        //            Message = "You are trying to generate for undefined Period",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);

        //    var maxPrevYearFrom = db.GLFiscalYears.Where(x => x.DateFrom < currentPeriod.DateFrom).Max(x => x.DateFrom);
        //    var prevousYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == maxPrevYearFrom);

        //    if (prevousYear != null)
        //    {
        //        var prevEntitlements = db.LeavesEntitle.Where(x => x.GlFiscalYearId == prevousYear.GlFiscalYearId).AsNoTracking().ToList();
        //        var activePlacements = db.Placements.Where(x => x.isCurrent).ToList();

        //        foreach (var empPlacement in activePlacements)
        //        {
        //            var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
        //            var currentEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == empPlacement.EmployeeId);

        //            var entitle = currentEntitle ?? new LeaveEntitle()
        //            {
        //                GlFiscalYearId = currentPeriod.GlFiscalYearId,
        //                EmployeeId = empPlacement.EmployeeId,
        //                EmployeeCode = empPlacement.EmployeeCode,
        //                BranchId = empPlacement.BranchId,
        //                OrgBranchName = empPlacement.BranchId.ToString(),
        //                DateCreated = DateTime.Now,
        //                CreatedBy = User.Identity.Name,
        //                IsAdded = true,
        //                LastModified = null,
        //                ModifiedBy = null
        //            };

        //            if (prevEntitlement != null)
        //            {
        //                entitle.Amount = prevEntitlement.Amount + 1;
        //                entitle.Balance = prevEntitlement.Amount + 1;
        //            }
        //            else
        //            {
        //                double currentExperience = GetCurrentExperienceLeaveDaysNew(empPlacement.EmployeeId, currentPeriod, initialGlFiscalYear);
        //                double externalExperience = check ? GetExternalExperienceLeaveDays(empPlacement.EmployeeId) : 0;
        //                double leaveBaseLine = employeeList.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId) is var emp && emp != null
        //                    ? GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear)
        //                    : empPlacement.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT ? maximumForLeader : maximumValue;

        //                decimal amount = Convert.ToDecimal(currentExperience + externalExperience + leaveBaseLine);

        //                entitle.Amount = Math.Max(0, Math.Min(empPlacement.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT ? maximumForLeader : maximumValue, amount));
        //                entitle.Balance = entitle.Amount;
        //            }

        //            if (currentEntitle == null)
        //            {
        //                db.LeavesEntitle.Add(entitle);
        //            }
        //            else
        //            {
        //                db.Entry(currentEntitle).State = EntityState.Modified;
        //            }
        //            db.SaveChanges();

        //            #region operation log
        //            UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, $"Leave Entitle for {entitle.EmployeeCode} is Created By {User.Identity.Name}", Modules.HRM);
        //            #endregion
        //        }
        //    }

        //    return Json(new
        //    {
        //        Message = "Leave Successfully Generated",
        //        Status = "OK"
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetEmployeeLeaveEntitlementNewAwajLatest(int period, string employeeId)
        //{

        //    // 1. Validate current fiscal year
        //    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
        //    if (currentPeriod == null)
        //    {
        //        return Json(new
        //        {
        //            Message = "You are trying to generate entitlement for an undefined fiscal year.",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var periodDateTo = currentPeriod.DateTo.Date;
        //    // 2. Validate initial Awaj settings
        //    var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
        //    if (newAwajInitialYear == null)
        //    {
        //        return Json(new
        //        {
        //            Message = "Please define the initial year settings first.",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    var initialGlFiscalYear = db.GLFiscalYears
        //        .FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
        //    if (initialGlFiscalYear == null)
        //    {
        //        return Json(new
        //        {
        //            Message = "You must set the initial year before entitlement generation can proceed.",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

            
        //    #region validation leave Entitlement
        //    var forleaders0ld = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
        //    if (forleaders0ld == null || forleaders0ld.CurrentValue == null)
        //    {
        //        var message = "you have to add Leave for leaders old initial value";
        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    var forperformerold = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
        //    if (forperformerold == null || forperformerold.CurrentValue == null)
        //    {
        //        var message = "you have to add Leave for performers old initial value";

        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    var forleadersnew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
        //    if (forleadersnew == null || forleadersnew.CurrentValue == null)
        //    {
        //        var message = "you have to add Leave for leaders new initial value";

        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    var forperformernew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
        //    if (forperformernew == null || forperformernew.CurrentValue == null)
        //    {
        //        var message = "you have to add Leave for performers new initial value";

        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    var thresholdLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());
        //    if (thresholdLeave == null || thresholdLeave.CurrentValue == null)
        //    {
        //        var message = $"you have to add Leave {LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString().ToLower()} new initial value";

        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
        //    if (frequency == null)
        //    {
        //        var message = "you have to add frequency in HRM Setting";

        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
           
        //    else if (string.IsNullOrEmpty(frequency.CurrentValue))
        //    {
        //        var message = "add current value in frequency first";

        //        return Json(new
        //        {
        //            Message = message,
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    #endregion
        //    // 4. Load configuration values
        //    // Get MAXIMUM_LEAVE_ENTITLEMENT
        //    var maxLeaveConfig = db.Configurations
        //        .FirstOrDefault(x => x.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());

        //    int maximumValue = 0;
        //    int LeaveTrusholdValue = 0;
        //    int leaveTresholdForLeaderdrsNew = 0;
        //    if (maxLeaveConfig == null)
        //    {
        //        return Json(new
        //        {
        //            Message = "You must set MAXIMUM_LEAVE_ENTITLEMENT first..",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        int.TryParse(maxLeaveConfig.CurrentValue, out maximumValue);
        //        int.TryParse(thresholdLeave.CurrentValue, out LeaveTrusholdValue);
        //        int.TryParse(forleadersnew.CurrentValue, out leaveTresholdForLeaderdrsNew);
        //    }

        //    // Get MAXIMUM_Leave_For_Leaders
        //    var maxLeaderConfig = db.Configurations
        //        .FirstOrDefault(x => x.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());

        //    int maximumForLeader = 0;
        //    if (maxLeaderConfig == null)
        //    {
        //        return Json(new
        //        {
        //            Message = "You must set MAXIMUM_Leave_For_Leaders first..",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        int.TryParse(maxLeaderConfig.CurrentValue, out maximumForLeader);
        //    }
        //    int perYearIncrement = Convert.ToInt32(
        //        db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLEMENT_INCREEMENT_YEAR.ToString())
        //        ?.CurrentValue ?? "0");

        //    // 5. Find previous fiscal year
        //    var maxPrevYearFrom = db.GLFiscalYears
        //        .Where(x => x.DateFrom < currentPeriod.DateFrom)
        //        .Max(x => (DateTime?)x.DateFrom);

        //    var previousYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == maxPrevYearFrom);
        //    if (previousYear == null)
        //    {
        //        return Json(new
        //        {
        //            Message = "You can only generate leave entitlement if it has already been generated for the previous fiscal year.",
        //            Status = "NOT_OK"
        //        }, JsonRequestBehavior.AllowGet);
        //    }

        //    // 6. Load data
        //    var AllEntitlements = db.LeavesEntitle
        //        .AsNoTracking()
        //        .ToList();
        //     var prevEntitlements = AllEntitlements
        //        .Where(x => x.GlFiscalYearId == previousYear.GlFiscalYearId).ToList();

        //    var activePlacements = db.Placements.Where(x => x.isCurrent).ToList();

        //    if (!string.IsNullOrEmpty(employeeId) && int.TryParse(employeeId, out int empId) && empId != 0)
        //    {
        //        activePlacements = activePlacements.Where(x => x.EmployeeId == empId).ToList();
        //        prevEntitlements = prevEntitlements.Where(x => x.EmployeeId == empId).ToList();
        //        if (empId > 0)
        //        {
        //            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);

        //            if (personEmployee != null && personEmployee.HiredDate.Date > periodDateTo)
        //            {
        //                return Json(new
        //                {
        //                    Message = $"This Employee is hired on {personEmployee.HiredDate:MMMM dd, yyyy}, so They can't have leave entitlement.",
        //                    Status = "Not-OK"
        //                }, JsonRequestBehavior.AllowGet);
        //            }

        //        }
        //    }
        //    // 7. Process entitlements
        //    foreach (var empPlacement in activePlacements)
        //    {
        //        var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
        //        var currentEntitlement = AllEntitlements
        //                                   .FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId
        //                                                     && x.EmployeeId == empPlacement.EmployeeId
        //                                                     /*&& !x.IsExpired*/);

        //        bool isLeader = empPlacement.ResponsibilityLevel != ResponsibilityLevel.STAFF;

        //        // New entitlement path
        //        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
        //        if (person == null)
        //            continue;
        //        // Skip if hired after current fiscal year DateTo
        //        if (person.HiredDate.Date > periodDateTo)
        //            continue;

        //        if (prevEntitlement != null)
        //        {
        //            // Calculate entitlement amount
        //            decimal amount = (currentPeriod.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId)
        //                ? prevEntitlement.Amount + 1
        //                : prevEntitlement.Amount;

        //            decimal cappedAmount = isLeader
        //                ? Math.Min(amount, maximumForLeader)
        //                : Math.Min(amount, maximumValue);
        //            if (currentEntitlement == null)
        //            {
                        
        //                // Insert new entitlement
        //                var entitle = new LeaveEntitle
        //                {
        //                    GlFiscalYearId = currentPeriod.GlFiscalYearId,
        //                    EmployeeId = empPlacement.EmployeeId,
        //                    EmployeeCode = empPlacement.EmployeeCode,
        //                    BranchId = empPlacement.BranchId,
        //                    OrgBranchName = empPlacement.BranchId.ToString(),
        //                    Amount = cappedAmount,
        //                    Balance = cappedAmount,
        //                    IsAdded = true,
        //                    IsExpired = false,
        //                    DateCreated = DateTime.Now,
        //                    CreatedBy = User.Identity.Name
        //                };
        //                db.LeavesEntitle.Add(entitle);
        //            }
        //            else
        //            {
        //                currentEntitlement.IsAdded = true;
        //                currentEntitlement.IsExpired = false;
        //                currentEntitlement.Amount = cappedAmount;
        //                currentEntitlement.Balance = cappedAmount;
        //                currentEntitlement.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                currentEntitlement.GlFiscalYear = null; // prevent double attach
        //                currentEntitlement.Employee = null; // prevent double attach
        //                currentEntitlement.LastModified = DateTime.Now;
        //                currentEntitlement.ModifiedBy = User.Identity.Name;

        //                db.Entry(currentEntitlement).State = EntityState.Modified;
        //            }
        //            db.SaveChanges();


        //        }
        //        else
        //        {
                  
        //            var hireDate = person.HiredDate;
        //            decimal newEntitle = isLeader ? leaveTresholdForLeaderdrsNew : LeaveTrusholdValue;
        //            decimal newAmount = newEntitle; // default full entitlement
        //            if (hireDate.Date > currentPeriod.DateTo.Date)
        //                continue;
        //            if (hireDate.Date > currentPeriod.DateFrom.Date && hireDate.Date < currentPeriod.DateTo)
        //            {
        //                // total days in the current period
        //                var totalPeriodDays = (currentPeriod.DateTo.Date - currentPeriod.DateFrom.Date).TotalDays;

        //                // remaining days from hire date until period end
        //                var eligibleDays = (currentPeriod.DateTo.Date - hireDate.Date).TotalDays;

        //                // pro-rate leave based on remaining fraction of the period
        //                var proratedFraction = (decimal)(eligibleDays / totalPeriodDays);

        //                newAmount = Math.Round(newEntitle * proratedFraction, 2);

        //            }
        //            if (currentEntitlement != null)
        //            {
        //                // Update existing entitlement
        //                currentEntitlement.IsAdded = true;
        //                currentEntitlement.IsExpired = false;
        //                currentEntitlement.Amount = newEntitle;
        //                currentEntitlement.Balance = newAmount;
        //                currentEntitlement.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                currentEntitlement.GlFiscalYear = null; //  prevent double attach
        //                currentEntitlement.Employee = null; //  prevent double attach
        //                currentEntitlement.LastModified = DateTime.Now;
        //                currentEntitlement.ModifiedBy = User.Identity.Name;
        //                db.Entry(currentEntitlement).State = EntityState.Modified;
        //                db.SaveChanges();

        //            }
        //            else
        //            {
                       
        //                // else if hired before the period → keep full entitlement

        //                var entitle = new LeaveEntitle
        //                {
        //                    GlFiscalYearId = currentPeriod.GlFiscalYearId,
        //                    EmployeeId = empPlacement.EmployeeId,
        //                    EmployeeCode = empPlacement.EmployeeCode,
        //                    BranchId = empPlacement.BranchId,
        //                    OrgBranchName = empPlacement.BranchId.ToString(),
        //                    Amount = newEntitle,
        //                    Balance = newAmount,
        //                    IsAdded = true,
        //                    IsExpired = false,
        //                    DateCreated = DateTime.Now,
        //                    CreatedBy = User.Identity.Name
        //                };

        //                db.LeavesEntitle.Add(entitle);
        //                db.SaveChanges();

        //            }

        //        }




        //    }
        //    //db.SaveChanges();

        //    //  Log operation
        //    UserHelper.OperationLog(
        //        Request.UserHostAddress,
        //        "Leave Entitlement",
        //        "Success",
        //        User.Identity.Name,
        //        $"Leave entitlement for fiscal year {currentPeriod?.Name} was generated by {User.Identity.Name}",
        //        Modules.HRM);

        //    return Json(new
        //    {
        //        Message = "Leave entitlement successfully generated.",
        //        Status = "OK"
        //    }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetEmployeeLeaveEntitlemntNewAwajCopy(bool check, int period)
        //{
        //    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
        //    var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();

        //    #region Validation
        //    if (newAwajInitialYear == null)
        //        return Json(new
        //        {
        //            Message = "Please Select Period First!!",
        //            Status = "NOT_OK"
        //        },JsonRequestBehavior.AllowGet);
        //        //return Json(Script = "alert('Please Define Setting for Initial year first');");

        //    var initialGlFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
        //    if (initialGlFiscalYear == null)
        //        return new JavaScriptResult() { Script = "alert('You set an invalid initial year');" };

        //    var validationMessage = Validation_For_Entitlement_Generation();
        //    if (!string.IsNullOrEmpty(validationMessage))
        //        return new JavaScriptResult() { Script = $"alert('{validationMessage}');" };

        //    var leaveThreshold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

        //    // Fetch Maximum Leave Entitlement and Maximum for Leaders
        //    int maximumValue = Convert.ToInt32(db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString())?.CurrentValue
        //        ?? throw new Exception("You have to set MAXIMUM_LEAVE_ENTITLEMENT first"));

        //    int maximumForLeader = Convert.ToInt32(db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString())?.CurrentValue
        //        ?? throw new Exception("You have to set MAXIMUM_Leave_For_Leaders setting first"));

        //    var employeeList = db.Person_Employee.Where(x => x.IsActive).AsNoTracking().ToList();
        //    #endregion

        //    if (currentPeriod == null)
        //        return new JavaScriptResult() { Script = "alert('You are trying to generate for undefined Period');" };

        //    var maxPrevYearFrom = db.GLFiscalYears.Where(x => x.DateFrom < currentPeriod.DateFrom).Max(x => x.DateFrom);
        //    var prevousYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == maxPrevYearFrom);

        //    if (prevousYear != null)
        //    {
        //        var prevEntitlements = db.LeavesEntitle.Where(x => x.GlFiscalYearId == prevousYear.GlFiscalYearId).AsNoTracking().ToList();
        //        var activePlacements = db.Placements.Where(x => x.isCurrent).ToList();

        //        foreach (var empPlacement in activePlacements)
        //        {
        //            var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
        //            var currentEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == empPlacement.EmployeeId);

        //            var entitle = currentEntitle ?? new LeaveEntitle()
        //            {
        //                GlFiscalYearId = currentPeriod.GlFiscalYearId,
        //                EmployeeId = empPlacement.EmployeeId,
        //                EmployeeCode = empPlacement.EmployeeCode,
        //                BranchId = empPlacement.BranchId,
        //                OrgBranchName = empPlacement.BranchId.ToString(),
        //                DateCreated = DateTime.Now,
        //                CreatedBy = User.Identity.Name,
        //                IsAdded = true,
        //                LastModified = null,
        //                ModifiedBy = null
        //            };

        //            if (prevEntitlement != null)
        //            {
        //                entitle.Amount = prevEntitlement.Amount + 1;
        //                entitle.Balance = prevEntitlement.Amount + 1;
        //            }
        //            else
        //            {
        //                double currentExperience = GetCurrentExperienceLeaveDaysNew(empPlacement.EmployeeId, currentPeriod, initialGlFiscalYear);
        //                double externalExperience = check ? GetExternalExperienceLeaveDays(empPlacement.EmployeeId) : 0;
        //                double leaveBaseLine = employeeList.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId) is var emp && emp != null
        //                    ? GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear)
        //                    : empPlacement.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT ? maximumForLeader : maximumValue;

        //                decimal amount = Convert.ToDecimal(currentExperience + externalExperience + leaveBaseLine);

        //                entitle.Amount = Math.Max(0, Math.Min(empPlacement.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT ? maximumForLeader : maximumValue, amount));
        //                entitle.Balance = entitle.Amount;
        //            }

        //            if (currentEntitle == null)
        //            {
        //                db.LeavesEntitle.Add(entitle);
        //            }
        //            else
        //            {
        //                db.Entry(currentEntitle).State = EntityState.Modified;
        //            }
        //            db.SaveChanges();

        //            #region operation log
        //            UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, $"Leave Entitle for {entitle.EmployeeCode} is Created By {User.Identity.Name}", Modules.HRM);
        //            #endregion
        //        }
        //    }

        //    return new JavaScriptResult() { Script = "alert('Leave Successfully Generated');" };
        //}

        //public JavaScriptResult GetEmployeeLeaveEntitlemntNewAwaj(bool check, int period)
        //{
        //    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);

        //    if (currentPeriod != null)
        //    {
        //        var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
        //        if (newAwajInitialYear != null)
        //        {
        //            var initialGlFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
        //            if (initialGlFiscalYear != null)
        //            {
        //                var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

        //                int maximumValue = 0;
        //                int maximumForLeader = 0;
        //                var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
        //                if (maximumLeave != null)
        //                {
        //                    maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
        //                }

        //                var maxLeader = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());
        //                if (maxLeader != null)
        //                {
        //                    maximumForLeader = Convert.ToInt32(maxLeader.CurrentValue);
        //                }

        //                var employeeList = db.Person_Employee.Where(x => x.IsActive).ToList();

        //                #region Validation

        //                var validation = Validation_For_Entitlement_Generation();

        //                if (!string.IsNullOrEmpty(validation))
        //                {
        //                    return new JavaScriptResult() { Script = "alert('" + validation + "');" };
        //                }

        //                #endregion

        //                foreach (var emp in employeeList)
        //                {
        //                    var leaveEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == emp.EmployeeId);

        //                    if (leaveEntitle != null)
        //                    {
        //                        decimal amount = 0;

        //                        if (!leaveEntitle.IsAdded)
        //                        {
        //                            double currentExpriance = GetCurrentExperienceLeaveDaysNew(emp.EmployeeId, currentPeriod, initialGlFiscalYear);

        //                            double externalExperience = 0;
        //                            if (check)
        //                            {
        //                                externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
        //                            }

        //                            double totalExpriance = currentExpriance + externalExperience;

        //                            double LeaveBaseLine = GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear);

        //                            amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);
        //                        }
        //                        else if (leaveEntitle.IsAdded)
        //                        {
        //                            amount = leaveEntitle.Amount;
        //                        }

        //                        leaveEntitle.EmployeeId = emp.EmployeeId;
        //                        leaveEntitle.EmployeeCode = emp.EmployeeCode;

        //                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveEntitle.EmployeeId && x.isCurrent);
        //                        if (placement != null)
        //                        {
        //                            leaveEntitle.BranchId = placement.BranchId;
        //                            leaveEntitle.OrgBranchName = placement.BranchId.ToString();
        //                        }

        //                        decimal leaveTakenAndSubtractedFromThisBudgetYear = GetTakenLeaveInOneBudgetYear(emp.EmployeeId, currentPeriod.GlFiscalYearId);

        //                        leaveEntitle.Amount = amount;
        //                        leaveEntitle.Balance = leaveEntitle.Amount - leaveTakenAndSubtractedFromThisBudgetYear;
        //                        leaveEntitle.IsAdded = true;
        //                        leaveEntitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                        leaveEntitle.LastModified = DateTime.Now;
        //                        leaveEntitle.ModifiedBy = User.Identity.Name;

        //                        if (Convert.ToDecimal(maximumValue) < leaveEntitle.Amount)
        //                        {
        //                            leaveEntitle.Amount = maximumValue;
        //                            leaveEntitle.Balance = leaveEntitle.Amount - leaveTakenAndSubtractedFromThisBudgetYear;
        //                        }
        //                        if (leaveEntitle.Amount < 0)
        //                        {
        //                            leaveEntitle.Amount = 0;
        //                            leaveEntitle.Balance = 0;
        //                        }

        //                        db.Entry(leaveEntitle).State = EntityState.Modified;
        //                        db.SaveChanges();
        //                        #region operation log

        //                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + leaveEntitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                        #endregion

        //                    }

        //                    else if (leaveEntitle == null)
        //                    {
        //                        var entitle = new LeaveEntitle();

        //                        bool IsHireDateBetweenCurrentPeriod = false;
        //                        if (emp.HiredDate >= currentPeriod.DateFrom && emp.HiredDate <= currentPeriod.DateTo)
        //                        {
        //                            IsHireDateBetweenCurrentPeriod = true;
        //                        }

        //                        if (emp.HiredDate <= currentPeriod.DateFrom || IsHireDateBetweenCurrentPeriod)
        //                        {
        //                            var preveDateFrom = currentPeriod.DateFrom.AddYears(-1);
        //                            var prevDateTo = currentPeriod.DateTo.AddYears(-1);

        //                            var prevYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == preveDateFrom && x.DateTo == prevDateTo);
        //                            if (prevYear != null)
        //                            {
        //                                var leaveEntitlePrev = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.GlFiscalYearId == prevYear.GlFiscalYearId);

        //                                var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());

        //                                bool CheckPreviosLeaveAndFrequencyValueIsOne = false;
        //                                if (leaveEntitlePrev != null)
        //                                {
        //                                    if (frequency != null && Convert.ToInt32(frequency.CurrentValue) == 1)
        //                                    {
        //                                        CheckPreviosLeaveAndFrequencyValueIsOne = true;
        //                                    }
        //                                }

        //                                if (leaveEntitlePrev == null || CheckPreviosLeaveAndFrequencyValueIsOne)
        //                                {
        //                                    double currentExpriance = GetCurrentExperienceLeaveDaysNew(emp.EmployeeId, currentPeriod, initialGlFiscalYear);

        //                                    double externalExperience = 0;
        //                                    if (check)
        //                                    {
        //                                        externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
        //                                    }

        //                                    double totalExpriance = currentExpriance + externalExperience;

        //                                    entitle.EmployeeId = emp.EmployeeId;
        //                                    entitle.EmployeeCode = emp.EmployeeCode;

        //                                    double LeaveBaseLine = GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear);

        //                                    decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

        //                                    ResponsibilityLevel res = ResponsibilityLevel.STAFF;
        //                                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == entitle.EmployeeId && x.isCurrent);
        //                                    if (placement != null)
        //                                    {
        //                                        entitle.BranchId = placement.BranchId;
        //                                        entitle.OrgBranchName = placement.BranchId.ToString();
        //                                        res = placement.ResponsibilityLevel;
        //                                    }
        //                                    entitle.Amount = amount;
        //                                    entitle.Balance = amount;

        //                                    entitle.IsAdded = true;
        //                                    entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                                    entitle.DateCreated = DateTime.Now;
        //                                    entitle.LastModified = null;
        //                                    entitle.CreatedBy = User.Identity.Name;
        //                                    entitle.ModifiedBy = null;

        //                                    if (Convert.ToDecimal(maximumValue) < entitle.Amount && res == ResponsibilityLevel.TOP_MANAGMENT)
        //                                    {
        //                                        entitle.Amount = maximumForLeader;
        //                                        entitle.Balance = maximumForLeader;
        //                                    }
        //                                    else if (Convert.ToDecimal(maximumValue) < entitle.Amount)
        //                                    {
        //                                        entitle.Amount = maximumValue;
        //                                        entitle.Balance = maximumValue;
        //                                    }

        //                                    if (entitle.Amount < 0)
        //                                    {
        //                                        entitle.Amount = 0;
        //                                        entitle.Balance = 0;
        //                                    }
        //                                    db.LeavesEntitle.Add(entitle);
        //                                    db.SaveChanges();
        //                                    #region operation log

        //                                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                                    #endregion

        //                                }
        //                                else
        //                                {
        //                                    if (leaveEntitlePrev.IsAdded)
        //                                    {
        //                                        decimal defaultLeave = Convert.ToDecimal(GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear));

        //                                        if (leaveEntitlePrev.Amount < defaultLeave)
        //                                        {
        //                                            entitle.Amount = defaultLeave;
        //                                            entitle.Balance = defaultLeave;
        //                                        }
        //                                        else
        //                                        {
        //                                            entitle.Amount = leaveEntitlePrev.Amount;
        //                                            entitle.Balance = leaveEntitlePrev.Amount;
        //                                        }

        //                                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == entitle.EmployeeId && x.isCurrent);
        //                                        if (placement != null)
        //                                        {
        //                                            entitle.BranchId = placement.BranchId;
        //                                            entitle.OrgBranchName = placement.BranchId.ToString();
        //                                        }

        //                                        entitle.EmployeeId = emp.EmployeeId;
        //                                        entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                                        entitle.IsAdded = false;
        //                                        entitle.DateCreated = DateTime.Now;
        //                                        entitle.LastModified = null;
        //                                        entitle.CreatedBy = User.Identity.Name;
        //                                        entitle.ModifiedBy = null;

        //                                        db.LeavesEntitle.Add(entitle);
        //                                        db.SaveChanges();
        //                                        #region operation log

        //                                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                                        #endregion
        //                                    }
        //                                    else if (!leaveEntitlePrev.IsAdded)
        //                                    {
        //                                        double currentExpriance = GetCurrentExperienceLeaveDaysNew(emp.EmployeeId, currentPeriod, initialGlFiscalYear);

        //                                        double externalExperience = 0;
        //                                        if (check)
        //                                        {
        //                                            externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
        //                                        }

        //                                        entitle = new LeaveEntitle();

        //                                        entitle.EmployeeId = emp.EmployeeId;
        //                                        entitle.EmployeeCode = emp.EmployeeCode;

        //                                        ResponsibilityLevel res = ResponsibilityLevel.STAFF;
        //                                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == entitle.EmployeeId && x.isCurrent);
        //                                        if (placement != null)
        //                                        {
        //                                            entitle.BranchId = placement.BranchId;
        //                                            entitle.OrgBranchName = placement.BranchId.ToString();
        //                                            res = placement.ResponsibilityLevel;
        //                                        }

        //                                        double LeaveBaseLine = GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear);

        //                                        decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

        //                                        // we add 1 to the previous entitle amount
        //                                        //decimal amount = leaveEntitlePrevNotAdded.Amount + 1;

        //                                        // the reason i substrac 1 from amount is because the when we add the experience the year adds 2 day
        //                                        entitle.Amount = amount - 1;
        //                                        entitle.Balance = amount - 1;
        //                                        entitle.IsAdded = true;
        //                                        entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                                        entitle.DateCreated = DateTime.Now;
        //                                        entitle.LastModified = null;
        //                                        entitle.CreatedBy = User.Identity.Name;
        //                                        entitle.ModifiedBy = null;

        //                                        if (Convert.ToDecimal(maximumValue) < entitle.Amount && res == ResponsibilityLevel.TOP_MANAGMENT)
        //                                        {
        //                                            entitle.Amount = maximumForLeader;
        //                                            entitle.Balance = maximumForLeader;
        //                                        }
        //                                        else if (Convert.ToDecimal(maximumValue) < entitle.Amount)
        //                                        {
        //                                            entitle.Amount = maximumValue;
        //                                            entitle.Balance = maximumValue;
        //                                        }
        //                                        if (entitle.Amount < 0)
        //                                        {
        //                                            entitle.Amount = 0;
        //                                            entitle.Balance = 0;
        //                                        }

        //                                        db.LeavesEntitle.Add(entitle);
        //                                        db.SaveChanges();
        //                                        #region operation log

        //                                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                                        #endregion
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                // check if there is only one budget year defined 
        //                                // this happens the system starts for the first time for the company 
        //                                var noOfPeriodDefined = db.GLFiscalYears.ToList();
        //                                var mindate = noOfPeriodDefined.Select(x => x.DateFrom).Min();

        //                                //if (noOfPeriodDefined.Count() > 1 && currentPeriod.DateFrom > mindate)
        //                                //{
        //                                //    string Message = "Check you budget year from and to value";
        //                                //    return new JavaScriptResult() { Script = "alert('" + Message + "');" };
        //                                //}
        //                                //else
        //                                //{
        //                                    double currentExpriance = GetCurrentExperienceLeaveDaysNew(emp.EmployeeId, currentPeriod, initialGlFiscalYear);

        //                                    double externalExperience = 0;
        //                                    if (check)
        //                                    {
        //                                        externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
        //                                    }

        //                                    entitle = new LeaveEntitle();

        //                                    entitle.EmployeeId = emp.EmployeeId;
        //                                    entitle.EmployeeCode = emp.EmployeeCode;

        //                                    ResponsibilityLevel res = ResponsibilityLevel.STAFF;
        //                                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == entitle.EmployeeId && x.isCurrent);
        //                                    if (placement != null)
        //                                    {
        //                                        entitle.BranchId = placement.BranchId;
        //                                        entitle.OrgBranchName = placement.BranchId.ToString();
        //                                        res = placement.ResponsibilityLevel;
        //                                    }

        //                                    double LeaveBaseLine = GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear);

        //                                    decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

        //                                    // we add 1 to the previous entitle amount
        //                                    //decimal amount = leaveEntitlePrevNotAdded.Amount + 1;

        //                                    entitle.Amount = amount - 1;
        //                                    entitle.Balance = amount - 1;
        //                                    entitle.IsAdded = true;
        //                                    entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                                    entitle.DateCreated = DateTime.Now;
        //                                    entitle.LastModified = null;
        //                                    entitle.CreatedBy = User.Identity.Name;
        //                                    entitle.ModifiedBy = null;

        //                                    if (Convert.ToDecimal(maximumValue) < entitle.Amount && res == ResponsibilityLevel.TOP_MANAGMENT)
        //                                    {
        //                                        entitle.Amount = maximumForLeader;
        //                                        entitle.Balance = maximumForLeader;
        //                                    }
        //                                    else if (Convert.ToDecimal(maximumValue) < entitle.Amount)
        //                                    {
        //                                        entitle.Amount = maximumValue;
        //                                        entitle.Balance = maximumValue;
        //                                    }
        //                                    if (entitle.Amount < 0)
        //                                    {
        //                                        entitle.Amount = 0;
        //                                        entitle.Balance = 0;
        //                                    }

        //                                    db.LeavesEntitle.Add(entitle);
        //                                    db.SaveChanges();
        //                                    #region operation log

        //                                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                                    #endregion
        //                               // }
        //                            }
        //                        }

        //                    }

        //                }
        //            }
        //            else
        //            {
        //                var messagesInitial = "You set invalid initial year";

        //                return new JavaScriptResult() { Script = "alert('" + messagesInitial + "');" };
        //            }
        //        }
        //        else
        //        {
        //            var messagesInitialYear = "Please define setting for initial year first";

        //            return new JavaScriptResult() { Script = "alert('" + messagesInitialYear + "');" };
        //        }
        //    }
        //    else
        //    {
        //        var messagesPeriod = "you are trying to generate for undefined Period";

        //        return new JavaScriptResult() { Script = "alert('" + messagesPeriod + "');" };
        //    }

        //    var messages = "Leave Successfully Generated";

        //    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //}
        #endregion
        #region Annual Leave Generating Functions
        public decimal GetTakenLeaveInOneBudgetYear(int empId, int yearId)
        {
            var leaveUsages = db.LeaveUsageTrackingForVoids.Where(x => x.EmployeeId == empId && x.GlFisicalYearId == yearId).ToList();
            return leaveUsages.Sum(x => x.Amount);
        }
        public double GetInitialLeaveValue(Person_Employee emp, GlFiscalYear currentPeriod, GlFiscalYear initialGlFiscalYear)
        {
            double LeaveBaseLine = 0;

            double days = (currentPeriod.DateTo - emp.HiredDate).Days;
            var totalYears = Math.Truncate(days / 365);
            var totalMonths = Math.Truncate((days % 365) / 30);
            var remainingDays = Math.Truncate((days % 365) % 30);

            var role = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
            if (role != null)
            {
                if (emp.HiredDate < initialGlFiscalYear.DateFrom)
                {
                   // if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                    if (role.IsHead)
                    {
                        var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
                        if (leaders != null)
                        {
                            LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                        }
                    }
                    else
                    {
                        var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
                        if (performers != null)
                        {
                            LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                        }
                    }
                    if (totalYears == 0 && emp.HiredDate >= currentPeriod.DateFrom && emp.HiredDate <= currentPeriod.DateTo)
                    {
                        LeaveBaseLine = totalMonths / 12 * LeaveBaseLine;
                    }

                }
                else if (emp.HiredDate >= initialGlFiscalYear.DateFrom)
                {
                    var baseLine = 0;

                   // if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                    if (role.IsHead)
                    {
                        var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
                        if (leaders != null)
                        {
                            LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                            baseLine = Convert.ToInt32(leaders.CurrentValue);
                        }
                    }
                    else
                    {
                        var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
                        if (performers != null)
                        {
                            LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                            baseLine = Convert.ToInt32(performers.CurrentValue);
                        }
                    }
                    if (totalYears == 0 && emp.HiredDate >= currentPeriod.DateFrom && emp.HiredDate <= currentPeriod.DateTo)
                    {
                        LeaveBaseLine = totalMonths / 12 * baseLine;
                    }
                }
            }

            return LeaveBaseLine;
        }
        public string Validation_For_Entitlement_Generation()
        {
            var forleaders0ld = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
            if (forleaders0ld == null || forleaders0ld.CurrentValue == null)
            {
                var message = "you have to add Leave for leaders old initial value";

                return message;
            }

            var forperformerold = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
            if (forperformerold == null || forperformerold.CurrentValue == null)
            {
                var message = "you have to add Leave for performers old initial value";

                return message;
            }

            var forleadersnew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
            if (forleadersnew == null || forleadersnew.CurrentValue == null)
            {
                var message = "you have to add Leave for leaders new initial value";

                return message;
            }

            var forperformernew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
            if (forperformernew == null || forperformernew.CurrentValue == null)
            {
                var message = "you have to add Leave for performers new initial value";

                return message;
            }

            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
            if (frequency == null)
            {
                var message = "you have to add frequency in HRM Setting";

                return message;
            }
            else if (string.IsNullOrEmpty(frequency.CurrentValue))
            {
                var message = "add current value in frequency first";

                return message;
            }

            return null;
        }
        #endregion
        public double GetExternalExperienceLeaveDays(int id)
        {
            double totalDays = 0;
            var awajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
            if (awajInitialYear != null)
            {
                var initialPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == awajInitialYear.GlFisicalYearId);
                if (initialPeriod != null)
                {
                    var expriances = db.Experiences.Where(e => e.EmployeeId == id).ToList();
                    foreach (var exp in expriances)
                    {
                        if (exp.From <= initialPeriod.DateFrom && exp.To <= initialPeriod.DateFrom)
                        {
                            totalDays += (exp.To - exp.From).Days;
                        }
                        else if (exp.From <= initialPeriod.DateFrom && exp.To >= initialPeriod.DateFrom)
                        {
                            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                            if (frequency != null)
                            {
                                double first = (initialPeriod.DateFrom - exp.From).Days;
                                double second = ((exp.To - initialPeriod.DateFrom).Days) / Convert.ToInt32(frequency.CurrentValue);

                                totalDays += first + second;
                            }

                        }
                        else if (exp.From >= initialPeriod.DateFrom)
                        {
                            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                            if (frequency != null)
                            {
                                totalDays += ((exp.To - exp.From).Days) / Convert.ToInt32(frequency.CurrentValue);
                            }
                        }
                    }
                }
            }

            var totalYears = Math.Truncate(totalDays / 365);
            var totalMonths = Math.Truncate((totalDays % 365) / 30);
            var remainingDays = Math.Truncate((totalDays % 365) % 30);

            return totalYears;
        }
        public double GetCurrentExperienceLeaveDaysOld(int id, GlFiscalYear currentPeriod, GlFiscalYear intialYear)
        {
            double currentExpriance = 0;
            double totalDays = 0;
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                double first = 0;
                double second = 0;

                var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

                DateTime hireDate = person.HiredDate;
                if (hireDate <= currentPeriod.DateTo && currentPeriod.DateTo <= intialYear.DateFrom)
                {
                    // experience before the initial year 
                    totalDays = (currentPeriod.DateTo - person.HiredDate).Days;
                }
                if (currentPeriod.DateFrom >= intialYear.DateFrom && hireDate <= intialYear.DateFrom)
                {
                    // first means experience before the initial year of awaj
                    first = (intialYear.DateFrom - hireDate).Days;

                    // second means experience after the initial year of awaj
                    second = (currentPeriod.DateTo - intialYear.DateFrom).Days;

                    var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                    if (frequency != null)
                    {
                        second = second / Convert.ToInt32(frequency.CurrentValue);
                        totalDays = first + second;
                    }
                }
                if (currentPeriod.DateFrom >= intialYear.DateFrom && hireDate >= intialYear.DateFrom)
                {
                    // experience after the initial year 
                    var expYear = (currentPeriod.DateTo - hireDate).Days;

                    var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                    if (frequency != null)
                    {
                        totalDays = expYear / Convert.ToInt32(frequency.CurrentValue);
                    }
                }

                var totalYears = Math.Truncate(totalDays / 365);
                var totalMonths = Math.Truncate((totalDays % 365) / 30);
                var remainingDays = Math.Truncate((totalDays % 365) % 30);

                if (leaveThrashold != null && totalYears == 0 && totalMonths >= Convert.ToInt32(leaveThrashold.CurrentValue))
                {
                    currentExpriance = 1;
                }
                else if (leaveThrashold != null && totalYears == 0 && totalMonths < Convert.ToInt32(leaveThrashold.CurrentValue))
                {
                    currentExpriance = 0;
                }
                else
                {
                    currentExpriance = totalYears;
                }

            }

            return currentExpriance;
        }
        // i wrote this function at 29/07/2021
        public double GetCurrentExperienceLeaveDaysNew(int id, GlFiscalYear currentPeriod, GlFiscalYear intialYear)
        {
            double currentExpriance = 0;
            double totalDays = 0;
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                double first = 0;
                double second = 0;

                var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

                var startDate = db.LeaveEntitleStartingDates.FirstOrDefault();
                if (startDate != null)
                {
                    if (startDate.Start == LeaveEntitleStarting.Fiscal_Year_Initial_Date)
                    {
                        if (person.HiredDate <= currentPeriod.DateFrom)
                        {
                            totalDays = (currentPeriod.DateFrom - person.HiredDate).Days;
                        }
                    }
                    else if (startDate.Start == LeaveEntitleStarting.Fiscal_Year_End_Date)
                    {
                        if (person.HiredDate <= currentPeriod.DateTo)
                        {
                            totalDays = (currentPeriod.DateTo - person.HiredDate).Days;
                        }
                    }

                }


                var totalYears = Math.Truncate(totalDays / 365);
                var totalMonths = Math.Truncate((totalDays % 365) / 30);
                var remainingDays = Math.Truncate((totalDays % 365) % 30);

                if (leaveThrashold != null && totalYears == 0 && totalMonths >= Convert.ToInt32(leaveThrashold.CurrentValue))
                {
                    currentExpriance = 1;
                }
                else if (leaveThrashold != null && totalYears == 0 && totalMonths < Convert.ToInt32(leaveThrashold.CurrentValue))
                {
                    currentExpriance = 0;
                }
                else
                {
                    currentExpriance = totalYears;
                }

            }

            return currentExpriance;
        }
        private decimal GetTakenAmount(int periodId, int empId)
        {
            var taken = db.Leaves.Where(x => x.GlFiscalYearId == periodId && x.EmployeeId == empId).ToList();
            if (taken.Any())
            {
                return taken.Sum(x => x.Amount);
            }
            return 0;
        }
        public int GetBranch(int id)
        {
            int branch = 0;
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                branch = placement.BranchId;
            }
            else
            {
                var place = db.Placements.Where(x => x.EmployeeId == id).OrderByDescending(x => x.AssignmentDate).ToList().FirstOrDefault();
                if (place != null)
                {
                    branch = place.BranchId;
                }
            }

            return branch;
        }
        public string GetEmployeeCode(int id)
        {
            string code = "";
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                code = person.EmployeeCode;
            }
            return code;
        }
        public string GetHiredDate(int id)
        {
            var person =  db.Person_Employee.Find(id);
            if (person == null)
            {
                // Log or handle the case where the employee is not found
                return "Employee not found.";
            }

            return person.HiredDateAmharic ?? "Hired date not available.";
        }
        public decimal GetTotalEntitlement(int id)
        {
            var entitlements = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);
            if (entitlements.Any())
            {
                return entitlements.Sum(x => x.Amount);
            }
            return 0;
        }
        public decimal GetTotalBalance(int id)
        {
            var entitlements = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);
            if (entitlements.Any())
            {
                return entitlements.Sum(x => x.Balance);
            }
            return 0;
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser, HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        public ActionResult LeavesEntitle_Read([DataSourceRequest] DataSourceRequest request)
        {
            var datePrefer = HrHelper.GetDatePreference();
            var employeeIds = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
            var query = db.LeavesEntitle
                .Where(x => employeeIds.Contains(x.EmployeeId) && !x.IsExpired)
                .OrderBy(o => o.GlFiscalYearId)
                .Select(leaveEntitle => new
                {
                    LeaveEntitleId = leaveEntitle.LeaveEntitleId,
                    EmployeeId = leaveEntitle.EmployeeId,
                    Balance = leaveEntitle.Balance,
                    BranchId = leaveEntitle.Employee.BranchId, // Assuming BranchId is accessible from Employee
                    GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                    Amount = leaveEntitle.Amount,
                    Remark = leaveEntitle.Remark,
                    DateCreated = leaveEntitle.DateCreated,
                    LastModified = leaveEntitle.LastModified,
                    CreatedBy = leaveEntitle.CreatedBy,
                    ModifiedBy = leaveEntitle.ModifiedBy,
                    EmployeeCode = leaveEntitle.Employee.EmployeeCode, // Avoid calling helper method
                    TotlalAmount = leaveEntitle.TotlalAmount,
                    OrgBranchName = leaveEntitle.OrgBranchName,
                    ExpiredBy = leaveEntitle.ExpiredBy,
                    ExpiredDate = leaveEntitle.ExpiredDate,
                    ReversedBy = leaveEntitle.ReversedBy,
                    Void = leaveEntitle.Void,
                    ReversedDate = leaveEntitle.ReversedDate,
                    SumOfBalances = db.LeavesEntitle
                        .Where(le => le.EmployeeId == leaveEntitle.EmployeeId && !le.IsExpired)
                        .Sum(le => (decimal?)le.Balance) ?? 0,
                    HireDate = (datePrefer && leaveEntitle.Employee != null)
                                        ? leaveEntitle.Employee.HiredDateAmharic
                                        : leaveEntitle.Employee.HiredDate.ToString("dd.MM.yyyy") ?? "-"

                });

            var result = query.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeavesEntitle_New_Read([DataSourceRequest] DataSourceRequest request,string yearId, string empId)
        {
            var myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var datePrefer = HrHelper.GetDatePreference();

            // Query includes both expired and unexpired for full aggregation
            var allLeaves = db.LeavesEntitle
                .Include(e => e.Employee)
                .Where(x => myBranches.Contains(x.BranchId)).ToList();
            if(!string.IsNullOrWhiteSpace(yearId)&&int.TryParse(yearId,out int fiscalYearId))
                allLeaves = allLeaves
                .Where(x => x.GlFiscalYearId ==fiscalYearId).ToList();

            if (!string.IsNullOrWhiteSpace(yearId)&&int.TryParse(empId,out int employeeId))
                allLeaves = allLeaves
                .Where(x => x.EmployeeId == employeeId).ToList();
            // Prepare grouping (SQL-level aggregation)
            var groupedData = allLeaves
                .GroupBy(x => x.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    // Total of all leaves (expired + unexpired)
                    TotalAmount_All = g.Sum(x => (decimal?)x.Amount) ?? 0,

                    TotalBalance_All = g.Sum(x => (decimal?)x.Balance) ?? 0,
                    // Total of unexpired only
                    TotalAmount_Unexpired = g.Where(x => !x.IsExpired).Sum(x => (decimal?)x.Amount) ?? 0,

                    TotalBalance_Unexpired = g.Where(x => !x.IsExpired).Sum(x => (decimal?)x.Balance) ?? 0,
                    // Latest active (unexpired) record
                    LatestActive = g.Where(x => !x.IsExpired)
                                            .OrderByDescending(x => x.GlFiscalYearId)
                                            .FirstOrDefault()
                })
                .AsEnumerable() // move to memory for formatting
                .Where(x => x.LatestActive != null) // show only employees with unexpired records
                .Select(x => new
                {
                    x.EmployeeId,
                    // Only unexpired info for display
                    Amount = x.TotalAmount_Unexpired,
                    Balance = x.TotalBalance_Unexpired,
                    // Display also total of all (expired + unexpired)
                    SumOfAllEntitlement = x.TotalAmount_All,
                    SumOfAllBalances = x.TotalBalance_All,
                    EmployeeCode = x.LatestActive?.Employee?.EmployeeCode,
                    BranchId = x.LatestActive?.Employee?.BranchId,
                    GlFiscalYearId = x.LatestActive?.GlFiscalYearId,
                    Remark = x.LatestActive?.Remark,
                    OrgBranchName = x.LatestActive?.OrgBranchName,
                    DateCreated = x.LatestActive?.DateCreated,
                    LastModified = x.LatestActive?.LastModified,
                    CreatedBy = x.LatestActive?.CreatedBy,
                    ModifiedBy = x.LatestActive?.ModifiedBy,
                    ExpiredBy = x.LatestActive?.ExpiredBy,
                    ExpiredDate = x.LatestActive?.ExpiredDate,
                    ReversedBy = x.LatestActive?.ReversedBy,
                    ReversedDate = x.LatestActive?.ReversedDate,
                    Void = x.LatestActive?.Void,

                    HireDate = datePrefer
                        ? (x.LatestActive?.Employee?.HiredDateAmharic ?? "-")
                        : x.LatestActive?.Employee?.HiredDate.ToString("dd.MM.yyyy") ?? "-"
                });

            var result = groupedData.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeavEntitleEmployee_Read([DataSourceRequest] DataSourceRequest request, string field)
        {
            int.TryParse(field, out int employeeId);
            var result = db.LeavesEntitle.Where(x=>x.EmployeeId==employeeId).Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeavesEntitle_New_ReadOld([DataSourceRequest] DataSourceRequest request)
        {
            var myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var datePrefer = HrHelper.GetDatePreference();
            var leaveEntitlenes = db.LeavesEntitle;
            // Eagerly load Employee to avoid multiple SQL hits
            var leaves = db.LeavesEntitle
                .Include("Employee")
                .Where(x => myBranches.Contains(x.BranchId) && !x.IsExpired);

            // Grouping done in SQL; switch to in-memory afterwards for formatting
            var groupedQuery = leaves
                .GroupBy(x => x.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    TotalAmount = g.Sum(x => x.Amount),
                    TotalBalance = g.Sum(x => x.Balance),
                    Count = g.Count(),
                    Latest = g.OrderByDescending(x => x.GlFiscalYearId).FirstOrDefault()
                })
                .AsEnumerable() // move to memory for safe object access and date formatting
                .Select(x => new
                {
                    x.EmployeeId,
                    Amount = x.TotalAmount,
                    Balance = x.TotalBalance,
                    x.Count,
                    BranchId = x.Latest?.Employee?.BranchId,
                    GlFiscalYearId = x.Latest?.GlFiscalYearId,
                    Remark = x.Latest?.Remark,
                    EmployeeCode = x.Latest?.Employee?.EmployeeCode,
                    DateCreated = x.Latest?.DateCreated,
                    LastModified = x.Latest?.LastModified,
                    CreatedBy = x.Latest?.CreatedBy,
                    ModifiedBy = x.Latest?.ModifiedBy,
                    OrgBranchName = x.Latest?.OrgBranchName,
                    ExpiredBy = x.Latest?.ExpiredBy,
                    ExpiredDate = x.Latest?.ExpiredDate,
                    ReversedBy = x.Latest?.ReversedBy,
                    Void = x.Latest?.Void,
                    ReversedDate = x.Latest?.ReversedDate,
                    SumOfAllEntitlement = leaveEntitlenes
                        .Where(le => le.EmployeeId == x.EmployeeId)
                        .Sum(le => (decimal?)le.Amount) ?? 0,
                    SumOfAllBalances = leaveEntitlenes
                        .Where(le => le.EmployeeId == x.EmployeeId)
                        .Sum(le => (decimal?)le.Balance) ?? 0,
                    HireDate = datePrefer
                        ? (x.Latest?.Employee?.HiredDateAmharic ?? "-")
                        : x.Latest?.Employee != null
                            ? x.Latest.Employee.HiredDate.ToString("dd.MM.yyyy")
                            : "-"
                });

            var result = groupedQuery.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult LeavesEntitle_New_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

        //    //var employeeIds = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
        //    var datePrefer = HrHelper.GetDatePreference();
        //    var query = db.LeavesEntitle.Where(x => myBranches.Contains(x.BranchId)
        //                                            && !x.IsExpired)
        //        .GroupBy(x => x.EmployeeId)
        //        .Select(g => new
        //        {
        //            EmployeeId = g.Key,
        //            Balance = g.Sum(x => x.Balance),
        //            Amount = g.Sum(x => x.Amount),
        //            Latest = g.OrderByDescending(x => x.GlFiscalYearId).FirstOrDefault(),
        //            Count = g.Count()
        //        })
        //        .Select(x => new
        //        {
        //            EmployeeId=x.EmployeeId,
        //            Amount=x.Amount,
        //            Balance =x.Balance,
        //            //x.TotalAmount,
        //            //x.TotalBalance,
        //            x.Count,
        //            BranchId = x.Latest.Employee.BranchId,
        //            GlFiscalYearId = x.Latest.GlFiscalYearId,
        //            Remark = x.Latest.Remark,
        //            EmployeeCode = x.Latest.Employee.EmployeeCode,
        //            DateCreated = x.Latest.DateCreated,
        //            LastModified = x.Latest.LastModified,
        //            CreatedBy = x.Latest.CreatedBy,
        //            ModifiedBy = x.Latest.ModifiedBy,
        //            OrgBranchName = x.Latest.OrgBranchName,
        //            ExpiredBy = x.Latest.ExpiredBy,
        //            ExpiredDate = x.Latest.ExpiredDate,
        //            ReversedBy = x.Latest.ReversedBy,
        //            Void = x.Latest.Void,
        //            ReversedDate = x.Latest.ReversedDate,
        //            HireDate = datePrefer? x.Latest?.Employee?.HiredDateAmharic ?? "-"
        //                            : x.Latest.Employee?.HiredDate?.ToString("dd.MM.yyyy") ?? "-"



        //        });

        //    var result = query.ToDataSourceResult(request);
        //    return Json(result, JsonRequestBehavior.AllowGet);


        //}
        public ActionResult LeavesEntitle_OnePerson_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var leavesEntitleQuery = db.LeavesEntitle
                .Where(x => x.EmployeeId == id && x.IsExpired==false)
                .OrderByDescending(x => x.GlFiscalYearId)
                .Select(x => new
                {
                    x.LeaveEntitleId,
                    x.EmployeeId,
                    x.Balance,
                    x.GlFiscalYearId,
                    x.Amount,
                    x.Remark,
                    x.DateCreated,
                    x.LastModified,
                    x.CreatedBy,
                    x.ModifiedBy,
                    x.TotlalAmount,
                    x.OrgBranchName,
                    x.ExpiredBy,
                    x.ExpiredDate,
                    x.ReversedBy,
                    x.Void,
                    x.ReversedDate,
                    SumOfBalances= x.Balance,
                    SumOfAllEntitlement = x.Amount,
                    HireDate = x.Employee.HiredDateAmharic,
                    BranchId = x.Employee.BranchId,
                    EmployeeCode = x.Employee.EmployeeCode
                });

            var result = leavesEntitleQuery.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
            //var leavesentitle = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired)/*.Select(x => new { x.LeaveEntitleId, x.EmployeeId, x.Amount, x.Balance, x.GlFiscalYearId, x.Remark })*/.OrderByDescending(x=>x.GlFiscalYearId).ToList();
            //DataSourceResult result = leavesentitle.ToDataSourceResult(request, leaveEntitle => new
            //{
            //    LeaveEntitleId = leaveEntitle.LeaveEntitleId,
            //    EmployeeId = leaveEntitle.EmployeeId,
            //    Balance = leaveEntitle.Balance,
            //    GlFiscalYearId = leaveEntitle.GlFiscalYearId,
            //    Amount = leaveEntitle.Amount,
            //    Remark = leaveEntitle.Remark,
            //    BranchId = GetBranch(leaveEntitle.EmployeeId),
            //    DateCreated = leaveEntitle.DateCreated,
            //    LastModified = leaveEntitle.LastModified,
            //    CreatedBy = leaveEntitle.CreatedBy,
            //    ModifiedBy = leaveEntitle.ModifiedBy,
            //    EmployeeCode = GetEmployeeCode(leaveEntitle.EmployeeId),
            //    TotlalAmount = leaveEntitle.TotlalAmount,
            //    OrgBranchName = leaveEntitle.OrgBranchName,
            //    ExpiredBy = leaveEntitle.ExpiredBy,
            //    ExpiredDate = leaveEntitle.ExpiredDate,
            //    ReversedBy = leaveEntitle.ReversedBy,
            //    Void = leaveEntitle.Void,
            //    ReversedDate = leaveEntitle.ReversedDate,
            //    SumOfBalances = GetTotalBalance(leaveEntitle.EmployeeId),
            //    HireDate = leaveEntitle.Employee.HiredDate.ToShortDateString()
            //});

            //return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Create([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            var year = db.LeavesEntitle.FirstOrDefault(x =>
                    x.GlFiscalYearId == leaveEntitle.GlFiscalYearId &&
                    x.EmployeeId == leaveEntitle.EmployeeId);
            if (year != null)
            {
                if (year.IsExpired)
                {
                    ModelState.AddModelError("",
                        "Cannot register entitlement. The entitlement exists but it is expired.");
                }
                else
                {
                    ModelState.AddModelError("",
                        "Cannot register entitlement more than once per year.");
                }

                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }

            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null && decimal.TryParse(maximumLeave.CurrentValue, out var maxLeaveValue))
            {
                if (leaveEntitle.Balance > maxLeaveValue || leaveEntitle.Amount > maxLeaveValue)
                {
                    ModelState.AddModelError("",
                        $"Entitlement amount can't exceed the maximum allowed ({maxLeaveValue}).");
                    return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
                }
            }

            // First Validation: Balance should not exceed the entitled amount

            if (leaveEntitle.Balance > leaveEntitle.Amount)
            {
                ModelState.AddModelError("", "Balance can't exceed the entitled amount.");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }
            // Second Validation: Both Balance and Amount should not be less than zero
            if (leaveEntitle.Balance < 0 || leaveEntitle.Amount < 0)
            {
                ModelState.AddModelError("", "Both balance and entitled amount cannot be less than zero.");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }
            
            if (ModelState.IsValid)
            {
                var branch = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveEntitle.EmployeeId && x.isCurrent);
                if (branch != null)
                {
                    leaveEntitle.BranchId = branch.BranchId;
                }
                var employee = db.Person_Employee.Find(leaveEntitle.EmployeeId);
                if (employee != null)
                {
                    leaveEntitle.EmployeeCode = employee.EmployeeCode;
                }

                var entity = new LeaveEntitle
                {
                    EmployeeId = leaveEntitle.EmployeeId,
                    BranchId = leaveEntitle.BranchId,
                    TotlalAmount = leaveEntitle.TotlalAmount,
                    GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                    Amount = leaveEntitle.Amount,
                    Balance = leaveEntitle.Balance,
                    Remark = leaveEntitle.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = leaveEntitle.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = leaveEntitle.ModifiedBy,
                    EmployeeCode = leaveEntitle.EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                };

                entity.TotlalAmount = entity.Amount + GetTransfreedAmmount(leaveEntitle.GlFiscalYearId, leaveEntitle.EmployeeId);

                db.LeavesEntitle.Add(entity);
                db.SaveChanges();

                leaveEntitle.LeaveEntitleId = entity.LeaveEntitleId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entity.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }
            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));

        }
        private decimal GetTransfreedAmmount(int periodId, int empId)
        {
            //var transfrred = db.LeaveTransfer.Where(x => x.To == periodId && x.EmployeeId == empId).FirstOrDefault();
            //if (transfrred != null)
            //{

            //    return transfrred.Amount;
            //}
            return 0;                               //
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Update([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            ModelState.Clear();
            // First Validation: Balance should not exceed the entitled amount
            if (leaveEntitle.Balance > leaveEntitle.Amount)
            {
                ModelState.AddModelError("", "Balance can't exceed the entitled amount.");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }

            // Second Validation: Both Balance and Amount should not be less than zero
            if (leaveEntitle.Balance < 0 || leaveEntitle.Amount < 0)
            {
                ModelState.AddModelError("", "Both Balance and entitled amount cannot be less than zero.");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }

            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null && (leaveEntitle.Balance > Convert.ToDecimal(maximumLeave.CurrentValue) || leaveEntitle.Amount > Convert.ToDecimal(maximumLeave.CurrentValue)))
            {
                ModelState.AddModelError("", "Entitle amount can't exceed leave maximum setting");
            }
            if (ModelState.IsValid)
            {
                var entitle = db.LeavesEntitle.FirstOrDefault(x => x.LeaveEntitleId == leaveEntitle.LeaveEntitleId);
                if (entitle != null)
                {
                    entitle.Amount = leaveEntitle.Amount;
                    entitle.Balance = leaveEntitle.Balance;
                    entitle.Remark = leaveEntitle.Remark;
                    db.Entry(entitle).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle of " + entitle.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);
                    #endregion
                }
                else
                {


                    var entity = new LeaveEntitle
                    {
                        LeaveEntitleId = leaveEntitle.LeaveEntitleId,
                        EmployeeId = leaveEntitle.EmployeeId,
                        BranchId = leaveEntitle.BranchId,
                        Balance = leaveEntitle.Balance,
                        TotlalAmount = leaveEntitle.TotlalAmount,
                        GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                        Amount = leaveEntitle.Amount,
                        Remark = leaveEntitle.Remark,
                        DateCreated = leaveEntitle.DateCreated,
                        LastModified = leaveEntitle.LastModified,
                        CreatedBy = leaveEntitle.CreatedBy,
                        ModifiedBy = leaveEntitle.ModifiedBy,
                        EmployeeCode = leaveEntitle.EmployeeCode,
                        OrgBranchName = leaveEntitle.OrgBranchName,
                    };
                    db.LeavesEntitle.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entity.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Destroy([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            ModelState.Clear();
            var year = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
            int selectedToYear = year.DateTo.Year;
            int selectedFromYear = year.DateFrom.Year;
            int currentYear = DateTime.Now.Year;
            var currentEntitlement = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitle.EmployeeId
                                                             && x.GlFiscalYearId == leaveEntitle.GlFiscalYearId).ToList();

            if (selectedFromYear <= currentYear && selectedToYear >= currentYear && currentEntitlement.Count <= 1)
            {
                ModelState.AddModelError("", "You cannot delete current year entitlement");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                // Find the existing LeaveEntitle entity by primary key
                var entityToDelete = db.LeavesEntitle.Find(leaveEntitle.LeaveEntitleId);
                if (entityToDelete != null)
                {
                    // Log deletion
                    var yearDelete = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
                    var deleteYearName = yearDelete.Name;
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name,
                        "Deleted employee leave entitlement of employee " + leaveEntitle.EmployeeCode + " and fiscal year " + deleteYearName);

                    // Remove the entity from the database
                    db.LeavesEntitle.Remove(entityToDelete);
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "The entitlement could not be found.");
                }
            }

            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult LeavesEntitle_DestroyCopy([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            var year = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
            int selectedToYear = year.DateTo.Year;
            int selectedFromYear = year.DateFrom.Year;
            int currentYear = DateTime.Now.Year;
            var currententitele = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitle.EmployeeId
                                                          && x.GlFiscalYearId == leaveEntitle.GlFiscalYearId).ToList();
          
            if (selectedFromYear <= currentYear && selectedToYear >= currentYear&& currententitele.Count() <= 1)
            {
                ModelState.AddModelError("", "You cannot Delete current year Entitlement");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                
                var entity = new LeaveEntitle
                {
                        LeaveEntitleId = leaveEntitle.LeaveEntitleId,
                        EmployeeId = leaveEntitle.EmployeeId,
                        BranchId = leaveEntitle.BranchId,
                        Balance = leaveEntitle.Balance,
                        TotlalAmount = leaveEntitle.TotlalAmount,
                        GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                        Amount = leaveEntitle.Amount,
                        Remark = leaveEntitle.Remark,
                        DateCreated = leaveEntitle.DateCreated,
                        LastModified = leaveEntitle.LastModified,
                        CreatedBy = leaveEntitle.CreatedBy,
                        ModifiedBy = leaveEntitle.ModifiedBy,
                        EmployeeCode = leaveEntitle.EmployeeCode,
                        OrgBranchName = leaveEntitle.OrgBranchName,



                };
                #region delete log
                var yearDelete = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
                var delete = yearDelete.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave entitlement of employee" + leaveEntitle.EmployeeCode + " and fiscal year " + delete);
                #endregion
                db.LeavesEntitle.Attach(entity);
                db.LeavesEntitle.Remove(entity);
                db.SaveChanges();
                
            }
            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
        }
        private List<int> GetActiveEmployeeFromCurrentUserBranch(string user)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(user);

            return db.Placements
                .Where(x => myBranches.Contains(x.BranchId) && x.isCurrent && x.Employee.IsActive)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();
        }
        [HttpPost]
        public JsonResult ExpireEntitle(string period, int? employeeId)
        {
            if (string.IsNullOrWhiteSpace(period))
                return Json(new { Message = "Select valid Fiscal year or Period." });
            int.TryParse(period, out int fiscalYearId);
            var leaveEntitles = db.LeavesEntitle
                .Where(x => x.GlFiscalYearId == fiscalYearId && (!employeeId.HasValue || x.EmployeeId == employeeId))
                .ToList();

            if (!leaveEntitles.Any())
                return Json(new { Message = "No matching entitlements found." });
            if (leaveEntitles.All(x=>x.IsExpired))
                return Json(new { Message = "Already Expired." });

            foreach (var leave in leaveEntitles)
            {
                leave.IsExpired = true;
                leave.ExpiredBy = User.Identity.Name;
                leave.ExpiredDate = DateTime.Today;

                db.Entry(leave).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Json(new { Message = "Entitlements successfully expired." });
        }

        [HttpPost]
        public JsonResult ReverseExpireEntitle(string period, int? employeeId)
        {
            if (string.IsNullOrWhiteSpace(period))
                return Json(new { Message = "Select valid Fiscal year or Period." });
            int.TryParse(period, out int fiscalYearId);
            var leaveEntitles = db.LeavesEntitle
                .Where(x => x.GlFiscalYearId == fiscalYearId && (!employeeId.HasValue || x.EmployeeId == employeeId))
                .ToList();

            if (!leaveEntitles.Any())
                return Json(new { Message = "No matching entitlements found." });
            if (leaveEntitles.All(x => !x.IsExpired))
                return Json(new { Message = "Already Active." });
            foreach (var leave in leaveEntitles)
            {
                leave.IsExpired = false;
                leave.ReversedBy = User.Identity.Name;
                leave.ReversedDate = DateTime.Today;

                db.Entry(leave).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Json(new { Message = "Entitlements successfully reversed." });
        }
        //public JsonResult ExpireEntitle(string period, int? employeeId)
        //{
        //    var leaveEntitles = db.LeavesEntitle.Where(x => x.GlFiscalYearId.ToString() == period).ToList();
        //    if(leaveEntitles.Any()&& employeeId.HasValue()&& employeeId != null)
        //    {
        //        leaveEntitles = leaveEntitles.Where(x => x.EmployeeId == employeeId).ToList();
        //    }
        //    foreach (var leave in leaveEntitles)
        //    {
        //        leave.IsExpired = true;
        //        leave.ExpiredBy = User.Identity.Name;
        //        leave.ExpiredDate = DateTime.Today;

        //        db.Entry(leave).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}
        //public JsonResult ReverseExpireEntitle(string period ,int? employeeId)
        //{
        //    var leaveEntitles = db.LeavesEntitle.Where(x => x.GlFiscalYearId.ToString() == period).ToList();

        //    foreach (var leave in leaveEntitles)
        //    {
        //        leave.IsExpired = false;
        //        leave.ReversedBy = User.Identity.Name;
        //        leave.ReversedDate = DateTime.Today;

        //        db.Entry(leave).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave entitlemnt");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave entitlemnt");
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
