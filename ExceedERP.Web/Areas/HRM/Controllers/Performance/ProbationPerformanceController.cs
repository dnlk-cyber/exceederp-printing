using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Global;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead,
                    HrmConstants.HRPerformanceUser, 
                    HrmConstants.HRPerformanceAdmin,
                    HrmConstants.HREmployeeSelfService)]
    public class ProbationPerformanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ProbationPerformance
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Employees"] = db.Person_Employee.Select(x=>new
            {
                EmployeeId =x.EmployeeId,
                FullNameaEnglish = x.EmployeeCode+"-"+x.FirstNameEnglish+" "+x.MiddleNameEnglish+" "+x.LastNameEnglish
            }).ToList();

            ViewData["Org"] = db.MainCompanyStructures.ToList();
            ViewData["Jobtitle"] = db.Lookups.ToList();
            ViewData["Measurments"] = db.Lookups.Where(x=>x.LookupType == Core.Domain.Common.LookUpTypes.PROBATION_MEASUREMENT).ToList();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;

        }
        public ActionResult ProbationPerformanceEvaluation_Read([DataSourceRequest]DataSourceRequest request, int empId)
        {
            IQueryable<ProbationPerformance> probations = db.ProbationPerformances.Where(x=>x.EmployeeId==empId);
            DataSourceResult result = probations.ToDataSourceResult(request, probation => new {

                ProbationPerformanceId = probation.ProbationPerformanceId,
                EmployeeId = probation.EmployeeId,
                EmployeeStatus = probation.EmployeeStatus,
                Result = probation.Result,
                ResultType = probation.ResultType,
                HireDate = probation.HireDate,
                HireDateAm = probation.HireDateAm,
                LookupId = probation.LookupId,
                OrgStructureId = probation.OrgStructureId,
                ProbationEndDate = probation.ProbationEndDate,
                ProbationEndDateAm = probation.ProbationEndDateAm,
                Status = probation.Status
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceEvaluation_Create([DataSourceRequest]DataSourceRequest request, ProbationPerformance probation, int empId)
        {
            ModelState.Clear();
            probation.EmployeeId = empId;
            var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == empId);
            var hda = probation.HireDateAm;
            if(person != null)
            {
                probation.HireDate = person.HiredDate;
                probation.HireDateAm = person.HiredDateAmharic;
                probation.EmployeeStatus = person.Status;

            }
            var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == empId && x.isCurrent);
            if(place != null)
            {
                 probation.LookupId = place.LookupId;
                probation.OrgStructureId = place.OrgStructureId;
            }

            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(probation);
                db.ProbationPerformances.Add(entity);
                db.SaveChanges();

                #region operation log
                var person1 = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == entity.EmployeeId);
                UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance ", "Success", User.Identity.Name, User.Identity.Name + " add Probation Performance for " + person1?.FullNameaEnglish??null + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                        
                #endregion

            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ProbationPerformance_Read([DataSourceRequest] DataSourceRequest request)
        {
            var probations = db.ProbationPerformances.AsNoTracking();

            var result = probations.ToDataSourceResult(request, probation => new 
            {
                ProbationPerformanceId = probation.ProbationPerformanceId,
                EmployeeId = probation.EmployeeId,
                EmployeeStatus = probation.EmployeeStatus,
                Result = probation.Result,
                ResultType = probation.ResultType,
                HireDate = probation.HireDate,
                HireDateAm = probation.HireDateAm,
                LookupId = probation.LookupId,
                OrgStructureId = probation.OrgStructureId,
                ProbationEndDate = probation.ProbationEndDate,
                ProbationEndDateAm = probation.ProbationEndDateAm,

                DepartmentOnlineApprovedBy = probation.DepartmentOnlineApprovedBy,
                DepartmentSendForApprovalBy = probation.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = probation.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = probation.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedTime = probation.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = probation.IsDepartmentOnlineApproved,
                OnlineApprovedBy = probation.OnlineApprovedBy,
                OnlineApprovedTime = probation.OnlineApprovedTime,
                IsOnlineApproved = probation.IsOnlineApproved,
                IsSendForApproval = probation.IsSendForApproval,
                SendForApprovalBy = probation.SendForApprovalBy,
                SendForApprovalTime = probation.SendForApprovalTime,
                Status = probation.Status,

                IsOnlineTransferred = probation.IsOnlineTransferred,
                IsOnlineTransferredBy = probation.IsOnlineTransferredBy,
                OnlineTransferredTime = probation.OnlineTransferredTime,

                CurrentApprovalStep = probation.CurrentApprovalStep,
                ForwardedFrom = probation.ForwardedFrom,
                ForwardedTo = probation.ForwardedTo,
                Remark = probation.Remark,
                OrgBranchName = probation.OrgBranchName,
                IsFinalApproval = probation.IsFinalApproval,

                ModifiedBy = probation.ModifiedBy,
                LastModified = probation.LastModified,
                CreatedBy = probation.CreatedBy,
                DateCreated = probation.DateCreated,

                IsVoid = probation.IsVoid,
                VoidBy = probation.VoidBy,
                VoidTime = probation.VoidTime
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformance_Create([DataSourceRequest]DataSourceRequest request, ProbationPerformance probation)
        {
            ModelState.Clear();
            DateTime? hd = null;
            DateTime? ed = null;
            if(probation.HireDateAm != null)
            {
                var splitDate = probation.HireDateAm.Split('/');
                 hd = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            if (probation.ProbationEndDateAm != null)
            {
                var splitDateEnd = probation.ProbationEndDateAm.Split('/');
                 ed = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateEnd[0]), Convert.ToInt32(splitDateEnd[1]), Convert.ToInt32(splitDateEnd[2]));

            }

            var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == probation.EmployeeId);
            var hda = probation.HireDateAm;
            if(person != null)
            {
                hd = person.HiredDate;
                hda = person.HiredDateAmharic;

            }
            var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == probation.EmployeeId && x.isCurrent);
            var look = 0;
            var org = 0;
            if(place != null)
            {
                 look = place.LookupId;
                org = place.OrgStructureId;
            }

            if (ModelState.IsValid)
            {
                
                probation.HireDate = hd;
                probation.HireDateAm = hda;
                probation.LookupId = look;
                probation.OrgStructureId = org;
                probation.ProbationEndDate = ed; 
                var entity = GetMappedObject(probation);
                db.ProbationPerformances.Add(entity);
                db.SaveChanges();

                #region operation log
                var person1 = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == entity.EmployeeId);
                if(person1 != null)
                {
                    if(person1 != null)
                    {
                       
                            UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance ", "Success", User.Identity.Name, User.Identity.Name + " add Probation Performance for " + person1.FullNameaEnglish + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                        
                    }

                }

                #endregion

            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformance_Update([DataSourceRequest]DataSourceRequest request, ProbationPerformance probation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var splitDate = probation.HireDateAm.Split('/');
                DateTime hd = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                var splitDateEnd = probation.ProbationEndDateAm.Split('/');
                DateTime ed = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateEnd[0]), Convert.ToInt32(splitDateEnd[1]), Convert.ToInt32(splitDateEnd[2]));
                
                probation.HireDate = hd;
                probation.ProbationEndDate = ed;
                var entity = GetMappedObject(probation);
                db.ProbationPerformances.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person1 = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person1 != null)
                {
                    if (person1 != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance ", "Success", User.Identity.Name, User.Identity.Name + " update Probation Performance of " + person1.FullNameaEnglish + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                       

                    }

                }

                #endregion
            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformance_Destroy([DataSourceRequest]DataSourceRequest request, ProbationPerformance probation)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(probation);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Probation Performance " + probation.ProbationPerformanceId);
                #endregion
                db.ProbationPerformances.Attach(entity);
                db.ProbationPerformances.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        private ProbationPerformance GetMappedObject(ProbationPerformance probation)
        {
            var entity = new ProbationPerformance
            {
                ProbationPerformanceId = probation.ProbationPerformanceId,
                EmployeeId = probation.EmployeeId,
                EmployeeStatus = probation.EmployeeStatus,
                Result = probation.Result,
                ResultType = probation.ResultType,
                HireDate = probation.HireDate,
                HireDateAm = probation.HireDateAm,
                LookupId = probation.LookupId,
                OrgStructureId = probation.OrgStructureId,
                ProbationEndDate = probation.ProbationEndDate,
                ProbationEndDateAm = probation.ProbationEndDateAm,

                IsOnlineTransferred = probation.IsOnlineTransferred,
                IsOnlineTransferredBy = probation.IsOnlineTransferredBy,
                OnlineTransferredTime = probation.OnlineTransferredTime,

                CurrentApprovalStep = probation.CurrentApprovalStep,
                ForwardedFrom = probation.ForwardedFrom,
                ForwardedTo = probation.ForwardedTo,
                Remark = probation.Remark,
                OrgBranchName = probation.OrgBranchName,
                IsFinalApproval = probation.IsFinalApproval,

                DepartmentOnlineApprovedBy = probation.DepartmentOnlineApprovedBy,
                DepartmentSendForApprovalBy = probation.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = probation.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = probation.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedTime = probation.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = probation.IsDepartmentOnlineApproved,
                OnlineApprovedBy = probation.OnlineApprovedBy,
                OnlineApprovedTime = probation.OnlineApprovedTime,
                IsOnlineApproved = probation.IsOnlineApproved,
                IsSendForApproval = probation.IsSendForApproval,
                SendForApprovalBy = probation.SendForApprovalBy,
                SendForApprovalTime = probation.SendForApprovalTime,
                Status = probation.Status,

                ModifiedBy = probation.ModifiedBy,
                LastModified = probation.LastModified,
                CreatedBy = probation.CreatedBy,
                DateCreated = probation.DateCreated,

                IsVoid = probation.IsVoid,
                VoidBy = probation.VoidBy,
                VoidTime = probation.VoidTime
            };
            return entity;
        }

        public JsonResult GetEmployeesUnderLogedInUserDepartment()
        {
            var today = DateTime.Today;
            int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var subordinates = EmployeeHelper.GetSubordinatesOfSubortinates_FilteredByHead(empId);
            subordinates.Add(empId);

            // Fetch active employees under the logged-in user's purview in a single query
            var employees = db.Person_Employee
                .AsNoTracking()
                .Where(x => subordinates.Contains(x.EmployeeId) && x.IsActive)
                .Select(x => new
                {
                    x.EmployeeId,
                    x.EmployeeCode,
                    x.FirstNameEnglish,
                    x.MiddleNameEnglish,
                    x.LastNameEnglish,
                    x.HiredDate,
                    x.Status
                })
                .ToList();

            var employeeIds = employees.Select(x => x.EmployeeId).ToList();

            // Placements (bulk loaded into a dictionary)
            var placements = db.Placements
                .Where(p => employeeIds.Contains(p.EmployeeId) && p.isCurrent)
                .GroupBy(p => p.EmployeeId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault());

            // Probation settings
            var probationSettings = db.ProbationsSettings.ToList();

            // Holidays
            var holidays = new HashSet<DateTime>(
                db.Holidays.Select(x => x.Date).ToList()
            );

            var result = employees
                .Where(emp =>
                {
                    placements.TryGetValue(emp.EmployeeId, out var placement);

                    int probationDays = probationSettings.FirstOrDefault()?.ProbationDays ?? 90;

                    if (placement != null)
                    {
                        var setting = probationSettings.FirstOrDefault(x => x.ResponsibilityLevel == placement.ResponsibilityLevel);
                        if (setting != null && setting.ProbationDays > 0)
                        {
                            probationDays = setting.ProbationDays;
                        }
                    }

                    int workedDays = CalculateWorkingDays(emp.HiredDate.Date, today, holidays);

                    return emp.Status == EmployeeStatus.PROBATION || workedDays < probationDays;
                })
                .Select(p => new
                {
                    EmployeeId = p.EmployeeId,
                    FullNameaEnglish = (p.EmployeeCode ?? "") + " - " +
                                       (p.FirstNameEnglish ?? "") + " " +
                                       (p.MiddleNameEnglish ?? "") + " " +
                                       (p.LastNameEnglish ?? "")
                })
                .OrderBy(x => x.FullNameaEnglish)
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllActiveProbationEmployees()
        {
            var today = DateTime.Now.Date;
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var subordinates = EmployeeHelper
                .GetSubordinatesOfSubortinates_FilteredByHead(id);

            subordinates.Add(id);

            // Employees under me
            var employees = db.Person_Employee
                .AsNoTracking()
                .Where(x =>
                    subordinates.Contains(x.EmployeeId)&& 
                    x.IsActive )
                .Select(x => new
                {
                    x.EmployeeId,
                    x.EmployeeCode,
                    x.FirstNameEnglish,
                    x.MiddleNameEnglish,
                    x.LastNameEnglish,
                    x.HiredDate,
                    x.Status
                })
                .ToList();

            var employeeIds = employees
                .Select(x => x.EmployeeId)
                .ToList();

            // Placements
            var placements = db.Placements
                    .Where(p => employeeIds.Contains(p.EmployeeId) && p.isCurrent)
                    .GroupBy(p => p.EmployeeId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.FirstOrDefault()
                    );

            // Probation settings
            var probationSettings = db.ProbationsSettings
                .ToList();

            // Holidays
            var holidays = new HashSet<DateTime>(
                db.Holidays
                  .Select(x => x.Date)
                  .ToList()
            );

            var result = employees
                .Where(emp =>
                {
                    placements.TryGetValue(emp.EmployeeId, out var placement);

            // Default probation days
            int probationDays =
                        probationSettings.FirstOrDefault()?.ProbationDays ?? 60;

            // Responsibility level based probation
            if (placement != null)
                    {
                        var setting = probationSettings.FirstOrDefault(x =>
                            x.ResponsibilityLevel == placement.ResponsibilityLevel);

                        if (setting != null && setting.ProbationDays > 0)
                        {
                            probationDays = setting.ProbationDays;
                        }
                    }

            // Working days calculation
            int workedDays = CalculateWorkingDays(
                        emp.HiredDate.Date,
                        today,
                        holidays);

            // Rule:
            // Include employee if:
            // 1. Status is PROBATION
            // OR
            // 2. Employee has NOT completed probation days yet

            return emp.Status == Core.Domain.Common.EmployeeStatus.PROBATION
                           || workedDays < probationDays;
                })
                .Select(emp => new
                {
                    EmployeeId = emp.EmployeeId,
                    FullNameaEnglish =
                        emp.EmployeeCode + " - " +
                        emp.FirstNameEnglish + " " +
                        emp.MiddleNameEnglish + " " +
                        emp.LastNameEnglish
                })
                .OrderBy(x => x.FullNameaEnglish)
                .ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private int CalculateWorkingDays( DateTime startDate,DateTime endDate, HashSet<DateTime> holidays)
        {
            int workingDays = 0;

            for (var date = startDate.AddDays(1); date <= endDate; date = date.AddDays(1))
            {
                // Skip Sunday
                if (date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                // Skip holiday
                if (holidays.Contains(date.Date))
                    continue;

                workingDays++;
            }

            return workingDays;
        }


        [HttpPost]
        public JsonResult CheckFitUnfit(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var probationPerformance = db.ProbationPerformances.FirstOrDefault(x => x.ProbationPerformanceId == id);
                    if (probationPerformance == null)
                        return Json(new { Message = "Master probation record not found.", Status = false });

                    if (!probationPerformance.IsOnlineApproved)
                        return Json(new { Message = "Cannot update: Record has to be approved first.", Status = false });

                    if (probationPerformance.ResultType == EvaluateProbation.Fit
                        || probationPerformance.ResultType == EvaluateProbation.UnFit)
                            return Json(new { Message = $"Cannot update: Record has already {probationPerformance.ResultType.ToString()} .", Status = false });

                    var details = db.ProbationPerformanceDetails.Where(x => x.ProbationPerformanceId == id).ToList();
                    if (!details.Any())
                        return Json(new { Message = "No evaluation details found to process.", Status = false });

                    var weightEarned = details.Sum(x => x.Score);

                    var probationSettings = db.ProbationPerformanceSettings.FirstOrDefault();
                    if (probationSettings == null)
                        return Json(new { Message = "Please set probation performance settings first.", Status = false });

                    // Corrected check: fail if placement IS NULL
                    var placement = db.Placements.Where(x => x.EmployeeId == probationPerformance.EmployeeId).OrderByDescending(x=>x.AssignmentDate).FirstOrDefault();
                    if (placement == null)
                        return Json(new { Message = "Please make placement first.", Status = false });

                    // Corrected check: fail if employee IS NULL
                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == probationPerformance.EmployeeId);
                    if (employee == null)
                        return Json(new { Message = "The Employee record was not found or is inactive.", Status = false });

                    // Update score result on master probation record
                    probationPerformance.Result = weightEarned;

                    // UNFIT: Score is lower than required threshold
                    if (weightEarned < probationSettings.IntialWeight)
                    {
                        probationPerformance.ResultType = EvaluateProbation.UnFit;

                        placement.isCurrent = false;
                        employee.IsActive = false;

                        db.Entry(probationPerformance).State = EntityState.Modified;
                        db.Entry(placement).State = EntityState.Modified;
                        db.Entry(employee).State = EntityState.Modified;

                        var terminationTypeLookup = db.Lookups.FirstOrDefault(x => x.LookupType == Core.Domain.Common.LookUpTypes.TERMINATIONTYPE);

                        var terminationEntity = new Core.Domain.HRM.EmployeeDetail.Termination
                        {
                            EmployeeId = employee.EmployeeId,
                            BranchId = placement.BranchId,
                            BusinessUnitId = placement.BusinessUnitId,
                            OrgStructureId = placement.OrgStructureId,
                            LookupId = terminationTypeLookup != null ? terminationTypeLookup.LookupId : 0,
                            Reason = "Probation Unfit",
                            ApplicationDate = DateTime.UtcNow,
                            TerminationDate = DateTime.UtcNow,
                            Remark = "Probation Unfit",
                            ApplicationDateAm = HrHelper.GetECDate(DateTime.UtcNow),
                            TerminationDateAm = HrHelper.GetECDate(DateTime.UtcNow),
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            IsOnlineApproved = true,
                            IsSendForApproval = true,
                            IsDirect = true,
                            IsEarlyRetirement = false,
                            IsApplied = true,
                            EmployeeCode = employee.EmployeeCode,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                        };

                        db.Terminations.Add(terminationEntity);
                    }
                    // FIT: Score meets or exceeds threshold
                    else
                    {
                        probationPerformance.ResultType = EvaluateProbation.Fit;

                        placement.EmployementType = Core.Domain.Common.EmployeeStatus.PERMANENT;
                        employee.Status = Core.Domain.Common.EmployeeStatus.PERMANENT;
                        employee.IsProbationEnd = true;
                        employee.ProhabitionState = 0; // clears the grid's stale "in probation" (yellow) background

                        db.Entry(probationPerformance).State = EntityState.Modified;
                        db.Entry(placement).State = EntityState.Modified;
                        db.Entry(employee).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    return Json(new
                    {
                        Message = $"Probation performance evaluation {probationPerformance.ResultType.ToString()} processed successfully .",
                        Status = true
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new
                    {
                        Message = "An error occurred while updating the records: " + ex.Message,
                        Status = false
                    });
                }
            }
        }


        //public JsonResult GetAllActiveProbationEmployees()
        //{
        //    var myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
        //    var today = DateTime.Now.Date;
        //    var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //    var subordinates = EmployeeHelper.GetSubordinatesOfSubortinates_FilteredByHead(id);
        //    subordinates.Add(id);
        //    // Step 1: Filter in DB (IMPORTANT)
        //    var employees = db.Person_Employee
        //        .AsNoTracking()
        //        .ToList()
        //        .Where(x => subordinates.Contains(x.EmployeeId) 
        //                    && myBranches.Contains(x.BranchId)
        //                    && x.IsActive
        //                    && x.IsPlaced)
        //                    .Where(x => ((today - x.HiredDate.Date).TotalDays < 90)||x.Status==Core.Domain.Common.EmployeeStatus.PROBATION)
        //        .Select(x => new
        //        {
        //            x.EmployeeId,
        //            x.EmployeeCode,
        //            x.FirstNameEnglish,
        //            x.MiddleNameEnglish,
        //            x.LastNameEnglish,
        //            x.HiredDate
        //        })
        //        .ToList();

        //    // Step 2: Get holidays as HashSet (FAST lookup)
        //    var holidays = new HashSet<DateTime>(
        //        db.Holidays.Select(x => x.Date)
        //    );

        //    // Step 3: Process probation logic
        //    var result = employees
        //        .Where(emp => IsInProbationRange(emp.HiredDate, holidays, today))
        //        .Select(p => new
        //        {
        //            EmployeeId = p.EmployeeId,
        //            FullNameaEnglish = p.EmployeeCode + "-" +
        //                               p.FirstNameEnglish + " " +
        //                               p.MiddleNameEnglish + " " +
        //                               p.LastNameEnglish
        //        })
        //        .ToList();

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //private bool IsInProbationRange(DateTime hiredDate, HashSet<DateTime> holidays, DateTime today)
        //{
        //    int probationDays = 0;
        //    DateTime date = hiredDate;

        //    while (date < today && probationDays < 90)
        //    {
        //        date = date.AddDays(1);

        //        if (date.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(date))
        //        {
        //            probationDays++;
        //        }

        //        if (probationDays >= 60 && probationDays < 90)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
