using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
   
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class EmployeeRewardsController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();


        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
               i => new
               {
                   FullNameAmharic = i.AmharicName,
                   FullNameaEnglish = i.Name,
                   EmployeeId = i.Id,
                   ToEmployeeId = i.Id,
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
            ViewData["AllowanceList"] = db.AllowanceDefinations.AsNoTracking().AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["fiscalYear"] = CacheData.GetFiscalYearCaches().Select(
             i => new
             {
                 Name = i.Name,
                 GlFiscalYearId = i.Id,
             }).AsQueryable();
            ViewData["PeriodList"] = db.GLPeriods.OrderByDescending(g => g.DateTo).Select(x => new { x.GLPeriodId, x.Name }).AsQueryable();

        }
        public JsonResult GetCurrentYearPeriods()
        {
            var periods = db.GLPeriods
                .Where(x => x.Status == FiscalYearStatus.Open)
                .OrderByDescending(x => x.DateFrom)
                .Select(x => new
                {
                    GLPeriodId = x.GLPeriodId,
                    Name = x.Name
                })
                .ToList();

            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
        public ActionResult EmployeeRewards_Read([DataSourceRequest]DataSourceRequest request)
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<EmployeeReward> employeeRewards = db.EmployeeRewards.Where(x => myBranches.Contains(x.BranchId));
            DataSourceResult result = employeeRewards.ToDataSourceResult(request, employeeReward => new
            {
                EmployeeRewardId = employeeReward.EmployeeRewardId,
                EmployeeId = employeeReward.EmployeeId,
                BranchId = employeeReward.BranchId,
                GlFiscalYearId = employeeReward.GlFiscalYearId,
                GLPeriodId = employeeReward.GLPeriodId,
                RewardType = employeeReward.RewardType,
                Salary = employeeReward.Salary,
                PersonOfSalary = employeeReward.PersonOfSalary,
                Amount = employeeReward.Amount,
                Remark = employeeReward.Remark,
                Date = employeeReward.Date,
                DateAm = employeeReward.DateAm,
                ReasonForReward = employeeReward.ReasonForReward,
                SubBranch = employeeReward.SubBranch,
                Unit = employeeReward.Unit,
                PayrollAccount = employeeReward.PayrollAccount,
                AwardedBy = employeeReward.AwardedBy,
                CreatedBy = employeeReward.CreatedBy,
                DateCreated = employeeReward.DateCreated,
                ModifiedBy = employeeReward.ModifiedBy,
                LastModified = employeeReward.LastModified,
                OrgBranchName = employeeReward.OrgBranchName
            });

            return Json(result);
        }
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeRewards_Create([DataSourceRequest]DataSourceRequest request, EmployeeReward employeeReward)
        {
            ModelState.Clear();
            DateTime now = DateTime.Now;

          
            // Use Ethiopian date preference if set (default true)
            bool useEthiopianDate = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            if (useEthiopianDate)
            {
                if (string.IsNullOrWhiteSpace(employeeReward.DateAm))
                {
                    ModelState.AddModelError("", "Date field is mandatory");
                    return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
                }

                employeeReward.Date = HrHelper.ParseEthiopicDate(employeeReward.DateAm);
            }
            else
            {
                employeeReward.DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                    employeeReward.Date.Day, employeeReward.Date.Month, employeeReward.Date.Year
                );
            }
            if (employeeReward.GLPeriodId.HasValue)
            {
                var period = db.GLPeriods.Find(employeeReward.GLPeriodId);

                if (period != null)
                {
                    // Ensure reward date is within the period range
                    if (employeeReward.Date.Date < period.DateFrom.Date || employeeReward.Date.Date > period.DateTo.Date)
                    {
                        ModelState.AddModelError(
                            "",
                            $"Date must be within the period range: {period.DateFrom:MM.dd.yy} - {period.DateTo:MM.dd.yy}"
                        );

                        return Json(
                            new[] { employeeReward }.ToDataSourceResult(request, ModelState)
                        );
                    }
                }
            }
            else
            {
                // Auto-select the GL period based on the reward date
                var matchingPeriod = db.GLPeriods
                    .FirstOrDefault(x => employeeReward.Date >= x.DateFrom && employeeReward.Date <= x.DateTo);

                if (matchingPeriod != null)
                {
                    employeeReward.GlFiscalYearId = matchingPeriod.GlFiscalYearId;
                    employeeReward.GLPeriodId = matchingPeriod.GLPeriodId;
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "No GL Period found for the given date. Please verify the date or period setup."
                    );

                    return Json(
                        new[] { employeeReward }.ToDataSourceResult(request, ModelState)
                    );
                }
            }
            // --- Validate employee existence ---
            var personEmployee = db.Person_Employee.Find(employeeReward.EmployeeId);
            if (personEmployee == null)
            {
                ModelState.AddModelError("", "Employee is deleted");
                return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements
                .FirstOrDefault(x => x.EmployeeId == employeeReward.EmployeeId && x.isCurrent)
                ?? db.Placements
                    .Where(x => x.EmployeeId == employeeReward.EmployeeId)
                    .OrderByDescending(x => x.PlacementId)
                    .FirstOrDefault();
            if (placement == null)
            {
                ModelState.AddModelError("", "Employee has no  placement");
                return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            }

            //var setting = db.RewardSettings.FirstOrDefault();
            //double allowedDays = setting != null &&!setting.AllowNewEmployees ? setting.MinimumMonthsRequired * 30 : 180;
            //// --- Validate employee service length (must be > 180 days) ---
            //if ((now - personEmployee.HiredDate).TotalDays <= allowedDays)
            //{
            //    ModelState.AddModelError("", "You must serve at least 6 months before receiving a reward.");
            //    return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            //}

            // Get reward settings (default if not configured)
            var setting = db.RewardSettings.FirstOrDefault() ?? new RewardSetting();

            // Convert months to days (approx.)
            double requiredDays = setting.MinimumMonthsRequired * 30;

            // Determine if this employee must be checked for service length
            bool requireServiceCheck = !setting.AllowNewEmployees;

            // Calculate how long the employee has served
            var serviceDays = (DateTime.Now - personEmployee.HiredDate).TotalDays;

            // Validate minimum service duration
            if (requireServiceCheck && serviceDays < requiredDays)
            {
                ModelState.AddModelError("",
                    $"You must serve at least {setting.MinimumMonthsRequired} month(s) before receiving a reward.");
                return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            }

            // --- Validate disciplinary case ---
            // Get active disciplinary case for this employee
            var displineCase = db.DisciplinaryCases
                .FirstOrDefault(x => x.EmployeeId == employeeReward.EmployeeId && x.IsActive);

            if (displineCase != null)
            {
                // If employees with discipline are not allowed, check the duration
                if (!setting.AllowDisciplinedEmployees)
                {
                    var daysSinceDiscipline = (now - displineCase.TransgressionDate).TotalDays;
                    var waitDays = setting.MaximumDisciplinedMonthsRequired * 30;

                    if (daysSinceDiscipline < waitDays)
                    {
                        ModelState.AddModelError("",
                            $"You have an active disciplinary case within the last {setting.MaximumDisciplinedMonthsRequired} month(s), " +
                            $"and are not eligible for a reward yet.");
                        return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            //var displineCase = db.DisciplinaryCases
            //    .FirstOrDefault(x => x.EmployeeId == employeeReward.EmployeeId && x.IsActive);

            //if (displineCase != null && (now - displineCase.TransgressionDate).TotalDays <= 180)
            //{
            //    ModelState.AddModelError("", "You have an active disciplinary case within the last 6 months, not eligible for a reward.");
            //    return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            //}
            if (employeeReward.PersonOfSalary.HasValue|| employeeReward.PersonOfSalary == 0)
            {
                ModelState.AddModelError("", "You have an active disciplinary case within the last 6 months, not eligible for a reward.");
                return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            }
            decimal amount = Math.Round((((employeeReward.PersonOfSalary ?? 0) * placement.Salary) / 100), 2);
            employeeReward.Amount = amount;
            employeeReward.Salary = placement.Salary;

            if (ModelState.IsValid)
            {

                
                employeeReward.CreatedBy = User.Identity.Name;
                employeeReward.DateCreated = DateTime.Now;
                employeeReward.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var entity = GetMappedObject(employeeReward);
                db.EmployeeRewards.Add(entity);
                db.SaveChanges();
                employeeReward.EmployeeRewardId = entity.EmployeeRewardId;
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee employeeReward", "Success", User.Identity.Name, "Employee employeeReward for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeRewards_Update([DataSourceRequest]DataSourceRequest request, EmployeeReward employeeReward)
        {

            ModelState.Clear();
            DateTime now = DateTime.Now;
            // Use Ethiopian date preference if set (default true)
            bool useEthiopianDate = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (useEthiopianDate)
            {
                if (string.IsNullOrWhiteSpace(employeeReward.DateAm))
                {
                    ModelState.AddModelError("", "Date field is mandatory");
                    return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
                }
                employeeReward.Date = HrHelper.ParseEthiopicDate(employeeReward.DateAm);
            }
            else
            {

                employeeReward.DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                    employeeReward.Date.Day, employeeReward.Date.Month, employeeReward.Date.Year
                );
            }

            if (employeeReward.GLPeriodId.HasValue)
            {
                var period = db.GLPeriods.Find(employeeReward.GLPeriodId);

                if (period != null)
                {
                    // Ensure reward date is within the period range
                    if (employeeReward.Date.Date < period.DateFrom.Date || employeeReward.Date.Date > period.DateTo.Date)
                    {
                        ModelState.AddModelError(
                            "",
                            $"Date must be within the period range: {period.DateFrom:MM.dd.yy} - {period.DateTo:MM.dd.yy}"
                        );

                        return Json(
                            new[] { employeeReward }.ToDataSourceResult(request, ModelState)
                        );
                    }
                }
            }
            else
            {
                // Auto-select the GL period based on the reward date
                var matchingPeriod = db.GLPeriods
                    .FirstOrDefault(x => employeeReward.Date >= x.DateFrom && employeeReward.Date <= x.DateTo);

                if (matchingPeriod != null)
                {
                    employeeReward.GlFiscalYearId = matchingPeriod.GlFiscalYearId;
                    employeeReward.GLPeriodId = matchingPeriod.GLPeriodId;
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "No GL Period found for the given date. Please verify the date or period setup."
                    );

                    return Json(
                        new[] { employeeReward }.ToDataSourceResult(request, ModelState)
                    );
                }
            }
            // --- Validate employee existence ---
            var personEmployee = db.Person_Employee.Find(employeeReward.EmployeeId);
            if (personEmployee == null)
            {
                ModelState.AddModelError("", "Employee is deleted");
                return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements
                 .FirstOrDefault(x => x.EmployeeId == employeeReward.EmployeeId 
                                    && x.isCurrent)
                 ?? db.Placements
                     .Where(x => x.EmployeeId == employeeReward.EmployeeId)
                     .OrderByDescending(x => x.PlacementId)
                     .FirstOrDefault();
            if (placement == null)
            {
                ModelState.AddModelError("", "Employee has no  placement");
                return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            }
            // --- Validate employee service length (must be > 180 days) ---
            // Get reward settings (default if not configured)
            var setting = db.RewardSettings.FirstOrDefault() ?? new RewardSetting();

            // Convert months to days (approx.)
            double requiredDays = setting.MinimumMonthsRequired * 30;

            // Determine if this employee must be checked for service length
            bool requireServiceCheck = !setting.AllowNewEmployees;

            // Calculate how long the employee has served
            var serviceDays = (DateTime.Now - personEmployee.HiredDate).TotalDays;

            // Validate minimum service duration
            if (requireServiceCheck && serviceDays < requiredDays)
            {
                ModelState.AddModelError("",
                    $"You must serve at least {setting.MinimumMonthsRequired} month(s) before receiving a reward.");
                return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            }

            // --- Validate disciplinary case ---
            // Get active disciplinary case for this employee
            var displineCase = db.DisciplinaryCases
                .FirstOrDefault(x => x.EmployeeId == employeeReward.EmployeeId && x.IsActive);

            if (displineCase != null)
            {
                // If employees with discipline are not allowed, check the duration
                if (!setting.AllowDisciplinedEmployees)
                {
                    var daysSinceDiscipline = (now - displineCase.TransgressionDate).TotalDays;
                    var waitDays = setting.MaximumDisciplinedMonthsRequired * 30;

                    if (daysSinceDiscipline < waitDays)
                    {
                        ModelState.AddModelError("",
                            $"You have an active disciplinary case within the last {setting.MaximumDisciplinedMonthsRequired} month(s), " +
                            $"and are not eligible for a reward yet.");
                        return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            if (employeeReward.PersonOfSalary.HasValue || employeeReward.PersonOfSalary == 0)
            {
                ModelState.AddModelError("", "You have an active disciplinary case within the last 6 months, not eligible for a reward.");
                return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
            }
            decimal amount = Math.Round((((employeeReward.PersonOfSalary ?? 0) * placement.Salary) / 100), 2);
            employeeReward.Amount = amount;
            employeeReward.Salary = placement.Salary;

            if (ModelState.IsValid)
            {
               
                var entity = GetMappedObject(employeeReward);
                db.EmployeeRewards.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {

                    UserHelper.OperationLog(Request.UserHostAddress, "Employee employee Reward", "Success", User.Identity.Name, "Emoployee employeeReward of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin,HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeRewards_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeReward employeeReward)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(employeeReward);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employeeReward information of employee Id" + employeeReward.Employee.EmployeeCode);
                #endregion
                db.EmployeeRewards.Attach(entity);
                db.EmployeeRewards.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeReward }.ToDataSourceResult(request, ModelState));
        }
        private EmployeeReward GetMappedObject(EmployeeReward employeeReward)
        {
            var entity = new EmployeeReward
            {
                EmployeeRewardId = employeeReward.EmployeeRewardId,
                EmployeeId = employeeReward.EmployeeId,
                BranchId = employeeReward.BranchId,
                GlFiscalYearId = employeeReward.GlFiscalYearId,
                GLPeriodId = employeeReward.GLPeriodId,
                RewardType = employeeReward.RewardType,
                Salary = employeeReward.Salary,
                PersonOfSalary = employeeReward.PersonOfSalary,
                Amount = employeeReward.Amount,
                Remark = employeeReward.Remark,
                Date = employeeReward.Date,
                DateAm = employeeReward.DateAm,
                ReasonForReward = employeeReward.ReasonForReward,
                SubBranch = employeeReward.SubBranch,
                Unit = employeeReward.Unit,
                AwardedBy = employeeReward.AwardedBy,
                CreatedBy = employeeReward.CreatedBy,
                DateCreated = employeeReward.DateCreated,
                ModifiedBy = employeeReward.ModifiedBy,
                LastModified = employeeReward.LastModified,
                OrgBranchName = employeeReward.OrgBranchName
            };
            return entity;
        }
        public JsonResult GenerateEmployeeRewardCopy(int branchId)
        {
            DateTime now = DateTime.Now;

            var placementsList = db.Placements.Where(x => x.BranchId == branchId && x.isCurrent).ToList();

            if (placementsList.Any())
            {
                var empIds = db.DisciplinaryCases.AsNoTracking().ToList()
                    .Where(dc => placementsList.Any(p => p.EmployeeId == dc.EmployeeId && dc.TransgressionDate >= p.AssignmentDate))
                    .Select(dc => dc.EmployeeId)
                    .ToList();

                // Remove employees with disciplinary cases
                placementsList = placementsList.Where(x => !empIds.Contains(x.EmployeeId)).ToList();

                // Further filter employees assigned for more than 180 days
                placementsList = placementsList.Where(x => (now - x.AssignmentDate).TotalDays > 180).ToList();


            }

            // Return a failure response if placement is not found
            if (!placementsList.Any())
            {
                return Json(new
                {
                    Message = "No record found  .",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }
           
            foreach (var placement in placementsList)
            {
                var entity = new EmployeeReward
                {
                    EmployeeId = placement.EmployeeId,
                    BranchId = placement.BranchId,
                    RewardType = RewardType.Money,
                    Amount = 0,
                    Remark = "Generated By Branch ",
                    Date =now,
                    DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(now.Day,now.Month,now.Year),
                    ReasonForReward = placement.AssignmentType.ToString(),
                    SubBranch = placement.OrgBranchName,
                    Unit = "Money",
                    AwardedBy = "r",
                    CreatedBy = User.Identity.Name,
                    DateCreated = now,
                    ModifiedBy = null,
                    LastModified = null,
                    OrgBranchName = placement.OrgBranchName
                };
                db.EmployeeRewards.Add(entity);
                db.SaveChanges();


            }
            db.SaveChanges();
            
            var formattedData = new
            {
                Message = "You successfully Generated",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GenerateEmployeeReward(int branchId,int? fiscalYearId, int? periodId,DateTime rewardDate, int rewardType,decimal percentOfSalary)
        {
            string rewardDateAmh = XAPI.EthiopicDateTime.GetEthiopicDate(rewardDate.Day, rewardDate.Month, rewardDate.Year);

            // Get current placements for the branch
            var placementsList = db.Placements
                .Where(x => x.BranchId == branchId && x.isCurrent)
                .ToList();

            if (!placementsList.Any())
            {
                return Json(new { Message = "No record found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            if (periodId.HasValue&&fiscalYearId.HasValue)
            {
                var period = db.GLPeriods.Find(periodId);

                if (period != null)
                {
                    // Ensure reward date is within the period range
                    if (rewardDate.Date < period.DateFrom.Date || rewardDate.Date.Date > period.DateTo.Date)
                    {
                        return Json(new { Message = $"Date must be within the period range: {period.DateFrom:MM.dd.yy} - {period.DateTo:MM.dd.yy}", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
                        
                    }
                }
            }
            else
            {
                // Auto-select the GL period based on the reward date
                var matchingPeriod = db.GLPeriods
                    .FirstOrDefault(x => rewardDate >= x.DateFrom && rewardDate.Date <= x.DateTo);

                if (matchingPeriod != null)
                {
                   fiscalYearId = matchingPeriod.GlFiscalYearId;
                   periodId = matchingPeriod.GLPeriodId;
                }
                else
                {
                    return Json(new { Message = "No GL Period found for the given date. Please verify the date or period setup.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);

                  
                }
            }
            // Get employees with disciplinary cases (optimized)
            var placementDict = placementsList.ToDictionary(p => p.EmployeeId, p => p.AssignmentDate);

            var empIds = db.DisciplinaryCases.AsNoTracking().ToList()
                .Where(dc => placementDict.ContainsKey(dc.EmployeeId)  // Ensure EmployeeId exists in placementDict
                             && dc.TransgressionDate >= placementDict[dc.EmployeeId]) // Compare dates safely
                .Select(dc => dc.EmployeeId)
                .Distinct()
                .ToList();

            // Remove employees with disciplinary cases and filter based on 180-day criteria
            placementsList = placementsList
                .Where(x => !empIds.Contains(x.EmployeeId) && (rewardDate - x.AssignmentDate).TotalDays > 180)
                .ToList();

            if (!placementsList.Any())
            {
                return Json(new { Message = "No eligible employees found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            // Check if EmployeeId exists in HRM.Employee
            var validEmployeeIds = db.Person_Employee
                .AsNoTracking()
                .ToList()
                .Where(e => placementsList.Select(p => p.EmployeeId).Contains(e.EmployeeId))
                .Select(e => e.EmployeeId)
                .Distinct()
                .ToList(); // Use HashSet for fast lookups
            var rewardsToInsert = new List<EmployeeReward>();
            DateTime now = DateTime.Now;
            var setting = db.RewardSettings.FirstOrDefault() ?? new RewardSetting();

            foreach (var p in placementsList.Where(p => validEmployeeIds.Contains(p.EmployeeId)))
            {
                var personEmployee = db.Person_Employee.Find(p.EmployeeId);


                // Convert months to days (approx.)
                double requiredDays = setting.MinimumMonthsRequired * 30;

                // Determine if this employee must be checked for service length
                bool requireServiceCheck = !setting.AllowNewEmployees;

                // Calculate how long the employee has served
                var serviceDays = (now - personEmployee.HiredDate).TotalDays;

                // Validate minimum service duration
                if (requireServiceCheck && serviceDays < requiredDays)
                    continue;

                // --- Validate disciplinary case ---
                var displineCase = db.DisciplinaryCases
                .FirstOrDefault(x => x.EmployeeId == p.EmployeeId && x.IsActive);

                //if (displineCase != null && (now - displineCase.TransgressionDate).TotalDays <= 180)
                if (displineCase != null)
                {
                    // If employees with discipline are not allowed, check the duration
                    if (!setting.AllowDisciplinedEmployees)
                    {
                        var daysSinceDiscipline = (now - displineCase.TransgressionDate).TotalDays;
                        var waitDays = setting.MaximumDisciplinedMonthsRequired * 30;

                        if (daysSinceDiscipline < waitDays)
                            continue;


                    }
                }

                var existingReward = db.EmployeeRewards.FirstOrDefault(r =>
                    r.EmployeeId == p.EmployeeId &&
                    r.BranchId == p.BranchId &&
                    r.GlFiscalYearId == fiscalYearId &&
                    r.GLPeriodId == periodId &&
                    r.RewardType == (RewardType)rewardType &&
                    r.Date.Month == rewardDate.Month);
                decimal amount = Math.Round(((percentOfSalary * p.Salary) / 100), 2);
                if (existingReward != null)
                {
                    // Update existing record
                    existingReward.Salary = p.Salary; // Update amount as needed
                    existingReward.PersonOfSalary = percentOfSalary; // Update amount as needed
                    existingReward.GLPeriodId = periodId; // Update amount as needed
                    existingReward.GlFiscalYearId = fiscalYearId; // Update amount as needed
                    existingReward.Amount = amount; // Update amount as needed
                    existingReward.Date = rewardDate; // Update amount as needed
                    existingReward.Remark = "Updated By Branch";
                    existingReward.Unit = ((RewardType)rewardType).ToString();
                    existingReward.ModifiedBy = User.Identity.Name;
                    existingReward.LastModified = DateTime.Now;
                    db.Entry(existingReward).State = EntityState.Modified;

                }
                else
                {
                    // Prepare new reward record
                    var newReward = new EmployeeReward
                    {
                        EmployeeId = p.EmployeeId,
                        GlFiscalYearId = fiscalYearId,
                        GLPeriodId = periodId,
                        BranchId = p.BranchId,
                        RewardType = (RewardType)rewardType,
                        Salary = p.Salary,
                        PersonOfSalary = percentOfSalary,
                        Amount = amount,
                        Remark = "Generated By Branch",
                        Date = rewardDate,
                        DateAm = rewardDateAmh,
                        ReasonForReward = p.AssignmentType.ToString(),
                        SubBranch = p.OrgBranchName,
                        Unit = ((RewardType)rewardType).ToString(),
                        AwardedBy = "r",
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        ModifiedBy = null,
                        LastModified = null,
                        OrgBranchName = p.OrgBranchName
                    };

                    rewardsToInsert.Add(newReward);
                }
            }

            // Add new records
            if (rewardsToInsert.Any())
            {
                db.EmployeeRewards.AddRange(rewardsToInsert);
            }

            // Save changes for both updates and inserts
            db.SaveChanges();

            return Json(new { Message = "You successfully generated rewards.", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }

      
        [HttpPost]
        public JsonResult UpdateAllPayrollAccounts(int fiscalYearId, int periodId)
        {
            try
            {
                // Get employee rewards
                var employeeRewards = db.EmployeeRewards
                    .Where(x => x.GlFiscalYearId == fiscalYearId && x.GLPeriodId == periodId)
                    .ToList();

                // Get all payroll bank references once
                var bankRefs = db.BankReferences
                                .Where(br => br.Type == BankAccountType.PAYROLL)
                                .GroupBy(br => br.Reference)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.OrderByDescending(x => x.BankReferenceId).FirstOrDefault().AccountNumber
                                );

                foreach (var emp in employeeRewards)
                {
                    var key = emp.EmployeeId.ToString();

                    if (bankRefs.ContainsKey(key))
                    {
                        emp.PayrollAccount = bankRefs[key];
                    }
                    else
                    {
                        emp.PayrollAccount = emp.PayrollAccount; // or keep existing if needed
                    }
                }

                db.SaveChanges();

                return Json(new { success = true, message = "Payroll accounts updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Employee Profile");
            #endregion
            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Employee Profile");
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