using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Termination
{
    [AuthorizeRoles(
        HrmConstants.HRTerminationUser,
        HrmConstants.HRAdmin,
        HrmConstants.ProjectHRAdmin,
        HrmConstants.ProjectManager,
        HrmConstants.ProjectDepartmentHead,
        HrmConstants.HRDepartmentHead,
        HrmConstants.HRManager,
        HrmConstants.GeneralManager,
        HrmConstants.DeputyGeneralManager,
        HrmConstants.DepartmentManager,
        HrmConstants.HumanResourceAdmin,
        HrmConstants.HRAdministrationManager)]
    public class TerminationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            SetValues();
            return View();
        }

        private bool CheckTerminationValidation(int id)
        {
            var ExitInterviews = db.ExitInterviews.Where(x => x.EmployeeId == id).FirstOrDefault();
            if (ExitInterviews != null)
            {
                return false;
            }
            return true;
        }

        private void SetValues()
        {
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["fiscalYear"] = db.GLFiscalYears.Select(x => new {
                x.GlFiscalYearId,
                x.Name
            }).AsQueryable();
            ViewData["PeriodList"] = db.GLPeriods.Select(x => new {
                x.GLPeriodId,
                x.Name
            }).AsQueryable();
            ViewData["TerminationTypes"] = db.Lookups
                .Where(l => l.LookupType == LookUpTypes.TERMINATIONTYPE)
                .AsQueryable();

            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
             i => new
             {
                 FullNameAmharic = i.AmharicName,
                 FullNameaEnglish = i.Name,
                 EmployeeId = i.Id,
             }).AsQueryable();

            ViewData["branches"] = CacheData.GetBranchCaches().Select(
                i => new
                {
                    Name = i.Name,
                    BranchId = i.Id,
                }).AsQueryable();

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
                i => new
                {
                    Name = i.Name,
                    BusinessUnitId = i.Id,
                }).AsQueryable();

            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
                i => new
                {
                    Name = i.Name,
                    OrgStructureId = i.Id,
                }).AsQueryable();

            var approvalConfigs = db.CompanyApprovalSettings
                                    .Where(x => x.IsEnabled && x.Module == ApprovalModule.Termination)
                                    .OrderBy(x => x.StepOrder)
                                    .ToList();

            ViewData["ApprovalConfigs"] = approvalConfigs;
        }

        [HttpPost]
        public JsonResult TerminateEmployee(int id)
        {
            var termination = db.Terminations.Include(x => x.Employee).FirstOrDefault(x => x.TerminationId == id);
            if (termination == null)
            {
                return Json(new { Message = "Termination not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            if (termination.IsOnlineApproved)
            {
                return Json(new { Message = "This request is already Approved (terminated).", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            // 1. Fetch active approval configurations for Termination ordered sequentially by StepOrder
            var approvalSettings = db.CompanyApprovalSettings
                .Where(x => x.IsEnabled && x.Module == ApprovalModule.Termination)
                .OrderBy(x => x.StepOrder)
                .ToList();

            // 2. If approval configurations exist, validate order-by-order sequentially
            if (approvalSettings.Any())
            {
                foreach (var setting in approvalSettings)
                {
                    bool isStepDone = CheckIfApprovalRoleCompleted(termination, setting.Role);
                    if (!isStepDone)
                    {
                        return Json(new
                        {
                            Message = string.Format("Sequential approval failed: Step {0} ({1}) must be completed first.", setting.StepOrder, setting.Role),
                            Status = "Not-Ok"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            // Mark as fully approved / terminated (Final step)
            termination.IsOnlineApproved = true;
            termination.OnlineApprovedBy = User.Identity.Name;
            termination.OnlineApprovedTime = DateTime.Now;
            db.Entry(termination).State = EntityState.Modified;

            #region change request status 
            var status = new RequestesStatus
            {
                RequestId = termination.TerminationId,
                Date = DateTime.Now.Date,
                ReferenceType = "Termination",
                Status = RequestStatus.APPROVE
            };
            db.RequestesStatus.Add(status);

            if (!UserHelper.DisableUser(termination.EmployeeId))
            {
                var users = UserInformationHelper.GetUsersInRole("Admin");
                var terminatedUserId = UserInformationHelper.GetEmployeeUserId(termination.EmployeeId);
                var terminatedUserName = UserInformationHelper.GetUserFullNamebyId(terminatedUserId) ?? "Unknown";

                foreach (var user in users)
                {
                    NotifModels notif = new NotifModels
                    {
                        UserId = user.UserId,
                        Title = string.Format("This User Must Be Disabled: {0} ({1})", terminatedUserId, terminatedUserName),
                        Message = string.Format("This Employee UserId: {0}, Name: {1}, Code: {2}", terminatedUserId, terminatedUserName, termination.Employee.EmployeeCode),
                        URL = "/HRM/Termination/Index"
                    };
                    Notify.NotifyUser(notif);
                }
            }
            #endregion

            db.SaveChanges();

            #region Notify Committees
            var commitment = db.Commitments.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
            if (commitment != null)
            {
                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
                if (emp != null)
                {
                    var superviorId = EmployeeHelper.GetSupervisor(emp.EmployeeId);
                    NotifModels notif = new NotifModels
                    {
                        UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                        Title = "Termination",
                        Message = string.Format(" {0} has terminated, remove their commitment.", UserInformationHelper.GetEmployeeFullName(User.Identity.Name)),
                        URL = "/HRM/Termination"
                    };
                    Notify.NotifyUser(notif);
                }
            }
            #endregion

            return Json(new { Message = "The employee has been terminated successfully.", Status = "Ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult TerminationApprovalStep(int id, string roleName)
        {
            var termination = db.Terminations.Find(id);
            if (termination == null)
            {
                return Json(new { Message = "Record not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            ApprovalRole roleEnum;
            if (!Enum.TryParse(roleName, out roleEnum))
            {
                return Json(new { Message = "Invalid approval role specified.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            // Load active approval configurations for the Termination module in sequential order
            var activeConfigs = db.CompanyApprovalSettings
                                  .Where(x => x.IsEnabled && x.Module == ApprovalModule.Termination)
                                  .OrderBy(x => x.StepOrder)
                                  .ToList();

            var currentConfig = activeConfigs.FirstOrDefault(x => x.Role == roleEnum);

            // Verify user authorization for this step
            bool isAuthorized = User.IsInRole(roleName)
                             || User.IsInRole(ApprovalRole.HRAdmin.ToString())
                             || User.IsInRole("HR_Admin");

            if (!isAuthorized)
            {
                return Json(new { Message = "You are not authorized to perform this approval step.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            // Dynamic Preceding Step Validation:
            // Ensure all enabled predecessor steps defined prior to this step are completed
            if (currentConfig != null)
            {
                var precedingSteps = activeConfigs.Where(x => x.StepOrder < currentConfig.StepOrder).ToList();
                foreach (var prevStep in precedingSteps)
                {
                    if (!CheckIfApprovalRoleCompleted(termination, prevStep.Role))
                    {
                        return Json(new
                        {
                            Message = string.Format("Preceding step '{0}' must be completed first.", prevStep.Role),
                            Status = "Not-Ok"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            string currentUser = User.Identity.Name;
            DateTime now = DateTime.Now;

            // Apply the approval state according to the ApprovalRole enum
            switch (roleEnum)
            {
                case ApprovalRole.ProjectHRAdmin:
                case ApprovalRole.HROfficer:
                    termination.IsProjectHRApproved = true;
                    termination.ProjectHRApprovedBy = currentUser;
                    termination.ProjectHRApprovedDate = now;
                    break;

                case ApprovalRole.ProjectManager:
                case ApprovalRole.GM:
                    termination.IsPMApproved = true;
                    termination.PMApprovedBy = currentUser;
                    termination.PMApprovedDate = now;
                    break;

                case ApprovalRole.DepartmentHead:
                case ApprovalRole.DepartmentManager:
                case ApprovalRole.ProjectDepartmentHead:
                    termination.IsDepartmentHeadApproved = true;
                    termination.DepartmentHeadApprovedBy = currentUser;
                    termination.DepartmentHeadApprovedDate = now;
                    break;

                case ApprovalRole.HRManager:
                case ApprovalRole.HRAdministrationManager:
                case ApprovalRole.HumanResourceAdmin:
                    termination.IsHRManagerApproved = true;
                    termination.HRManagerApprovedBy = currentUser;
                    termination.HRManagerApprovedDate = now;
                    break;

                default:
                    // Custom fallback (e.g., DailLabour, DGM, Board)
                    termination.IsAdditionalApproved = true;
                    termination.AdditionalApprovedBy = string.Format("{0} ({1})", currentUser, roleEnum);
                    termination.AdditionalApprovedDate = now;
                    break;
            }

            db.Entry(termination).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new
            {
                Message = string.Format("Approval step for {0} executed successfully.", roleEnum),
                Status = "Ok"
            }, JsonRequestBehavior.AllowGet);
        }

        // Helper method to accurately check completion status for each ApprovalRole
        private bool CheckIfApprovalRoleCompleted(Core.Domain.HRM.EmployeeDetail.Termination termination, ApprovalRole role)
        {
            switch (role)
            {
                case ApprovalRole.ProjectHRAdmin:
                case ApprovalRole.HROfficer:
                    return termination.IsProjectHRApproved;

                case ApprovalRole.ProjectManager:
                case ApprovalRole.GM:
                    return termination.IsPMApproved;

                case ApprovalRole.DepartmentHead:
                case ApprovalRole.DepartmentManager:
                case ApprovalRole.ProjectDepartmentHead:
                    return termination.IsDepartmentHeadApproved;

                case ApprovalRole.HRManager:
                case ApprovalRole.HRAdministrationManager:
                case ApprovalRole.HumanResourceAdmin:
                    return termination.IsHRManagerApproved;

                case ApprovalRole.DailLabour:
                case ApprovalRole.DGM:
                    return termination.IsAdditionalApproved;

                default:
                    return true;
            }
        }

        public JsonResult ForceTerminateEmployee(int id)
        {
            var termination = db.Terminations.FirstOrDefault(x => x.TerminationId == id);
            if (termination == null)
            {
                return Json(new { Message = "Termination not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            var garantee = db.GuarantorOrSuretyDetails.Any(x => x.Guarantor.EmployeeId == termination.EmployeeId && x.IsActive);
            if (garantee)
            {
                return Json(new { Message = "Request become Guarantee for others You have to stop it first ", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            if (termination.IsOnlineApproved && termination.IsForciblyTerminated)
            {
                return Json(new { Message = "This request is already Terminated", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            termination.IsOnlineApproved = true;
            termination.IsForciblyTerminated = true;
            termination.Remark += string.Format("Terminated back by {0} ", User.Identity.Name);

            var statusUpdates = new List<RequestesStatus>
            {
                new RequestesStatus
                {
                    RequestId = termination.TerminationId,
                    Date = DateTime.Now.Date,
                    ReferenceType = "Termination",
                    Status = RequestStatus.DEPARTMENT_APPROVAL
                },
                new RequestesStatus
                {
                    RequestId = termination.TerminationId,
                    Date = DateTime.Now.Date,
                    ReferenceType = "Termination",
                    Status = RequestStatus.APPROVE
                }
            };
            db.RequestesStatus.AddRange(statusUpdates);

            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
            if (termination.TerminationDate.HasValue && DateTime.Now.Date >= termination.TerminationDate.Value.Date)
            {
                termination.IsApplied = true;
                if (person != null)
                {
                    person.IsActive = false;
                    db.Entry(person).State = EntityState.Modified;
                }

                var placements = db.Placements.Where(x => x.EmployeeId == termination.EmployeeId).ToList();
                foreach (var place in placements)
                {
                    place.isCurrent = false;
                    db.Entry(place).State = EntityState.Modified;
                }
                using (var adb = new ApplicationDbContext())
                {
                    var user = adb.Users.Where(x => x.EmployeeId == termination.EmployeeId).FirstOrDefault();
                    if (user != null)
                        user.IsEnabled = false;
                    adb.SaveChanges();
                }
            }
            db.Entry(termination).State = EntityState.Modified;
            db.SaveChanges();

            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Termination",
                "Success",
                User.Identity.Name,
                string.Format("termination (ID: {0}) for employee {1} - {2} {3} {4}",
                    termination.TerminationId,
                    person != null ? person.EmployeeCode : "",
                    person != null ? person.FirstNameEnglish : "",
                    person != null ? person.MiddleNameEnglish : "",
                    person != null ? person.LastNameEnglish : ""),
                Modules.HRM
            );

            if (!UserHelper.DisableUser(termination.EmployeeId))
            {
                var users = UserInformationHelper.GetUsersInRole("Admin");
                var terminatedUserId = UserInformationHelper.GetEmployeeUserId(termination.EmployeeId);
                var terminatedUserName = UserInformationHelper.GetUserFullNamebyId(terminatedUserId) ?? "Unknown";
                foreach (var user in users)
                {
                    Notify.NotifyUser(new NotifModels
                    {
                        UserId = user.UserId,
                        Title = string.Format("User Must Be Disabled: {0} ({1})", terminatedUserId, terminatedUserName),
                        Message = string.Format("Employee UserId: {0}, Name: {1}, Code: {2}", terminatedUserId, terminatedUserName, termination.Employee.EmployeeCode),
                        URL = "/HRM/Termination/Index"
                    });
                }
            }
            return Json(new { Message = "This employee has been successfully terminated.", Status = "Ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UnTerminateEmployee(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var termination = db.Terminations.FirstOrDefault(x =>
                        x.TerminationId == id &&
                        (x.IsForciblyTerminated || x.IsOnlineApproved));

                    if (termination == null)
                    {
                        return Json(new
                        {
                            Message = "Termination not found or already active.",
                            Status = "Not-Ok"
                        }, JsonRequestBehavior.AllowGet);
                    }

                    var person = db.Person_Employee
                        .FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);

                    if (person == null)
                    {
                        return Json(new
                        {
                            Message = "Employee record not found.",
                            Status = "Not-Ok"
                        }, JsonRequestBehavior.AllowGet);
                    }

                    person.IsActive = true;
                    db.Entry(person).State = EntityState.Modified;

                    if (!person.IsReHired || termination.IsOnlineApproved || termination.IsForciblyTerminated)
                    {
                        termination.IsOnlineApproved = false;
                        termination.IsForciblyTerminated = false;
                        termination.Remark = (termination.Remark ?? "")
                            + string.Format("Un-terminated by {0} on {1},", User.Identity.Name, DateTime.Now);

                        db.Entry(termination).State = EntityState.Modified;

                        db.RequestesStatus.Add(new RequestesStatus
                        {
                            RequestId = termination.TerminationId,
                            Date = DateTime.Now,
                            ReferenceType = "Termination",
                            Status = RequestStatus.REJECT
                        });
                    }

                    var placement = db.Placements
                        .Where(x => x.EmployeeId == termination.EmployeeId)
                        .OrderByDescending(x => x.AssignmentDate)
                        .FirstOrDefault();

                    if (placement != null)
                    {
                        placement.isCurrent = true;
                        db.Entry(placement).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    using (var adb = new ApplicationDbContext())
                    {
                        var user = adb.Users.Where(x => x.EmployeeId == termination.EmployeeId).FirstOrDefault();
                        if (user != null)
                            user.IsEnabled = true;
                        adb.SaveChanges();
                    }
                    transaction.Commit();

                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "Un-terminate Termination",
                        "Success",
                        User.Identity.Name,
                        string.Format("un-termination (ID: {0}) for employee {1} - {2} {3} {4}",
                            termination.TerminationId,
                            person.EmployeeCode,
                            person.FirstNameEnglish,
                            person.MiddleNameEnglish,
                            person.LastNameEnglish),
                        Modules.HRM
                    );

                    return Json(new
                    {
                        Message = "Employee has been successfully un-terminated.",
                        Status = "OK"
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new
                    {
                        Message = "An error occurred while un-terminating the employee.",
                        Error = ex.Message,
                        Status = "Not-Ok"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public JavaScriptResult UpdateTerminationStatus()
        {
            var terminations = db.Terminations.ToList();
            foreach (var term in terminations)
            {
                var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == term.TerminationId && x.ReferenceType == "Termination");
                if (requestStatus != null && requestStatus.Status == RequestStatus.APPROVE)
                {
                    term.IsDepartmentOnlineApproved = true;
                    term.IsOnlineApproved = true;

                    db.Entry(term).State = EntityState.Modified;
                    if (!UserHelper.DisableUser(term.EmployeeId))
                    {
                        var users = UserInformationHelper.GetUsersInRole("Admin");
                        var terminatedUserId = UserInformationHelper.GetEmployeeUserId(term.EmployeeId);
                        var terminatedUserName = UserInformationHelper.GetUserFullNamebyId(terminatedUserId) ?? "Unknown";

                        foreach (var user in users)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = user.UserId,
                                Title = string.Format("This User Must Be Disabled: {0} ({1})", terminatedUserId, terminatedUserName),
                                Message = string.Format("This Employee UserId: {0}, Name: {1}, Code: {2}", terminatedUserId, terminatedUserName, term.Employee.EmployeeCode),
                                URL = "/HRM/Termination/Index"
                            };
                            Notify.NotifyUser(notif);
                        }
                    }
                }
                db.SaveChanges();
            }
            string message = "All termination status are updated successfully";
            return new JavaScriptResult() { Script = "alert('" + message + "');" };
        }

        public bool CheckIfActive(int empId)
        {
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (person != null && person.IsActive)
            {
                return true;
            }
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if (placement != null)
            {
                return true;
            }
            return false;
        }

        public ActionResult Loans_Read_Per_Employee([DataSourceRequest]DataSourceRequest request, int employeeId)
        {
            db.Database.CommandTimeout = 7200;
            IQueryable<Core.Domain.HRM.Payroll.Loan> loans = db.Loans.Where(x => x.EmployeeId == employeeId);
            DataSourceResult result = loans.ToDataSourceResult(request, loan => new {
                LoanId = loan.LoanId,
                EmployeeId = loan.EmployeeId,
                LoanAmount = loan.LoanAmount,
                Months = loan.Months,
                Balance = loan.Balance,
                MonthlyAmount = loan.MonthlyAmount,
                JumpHoliday = loan.JumpHoliday,
                Description = loan.Description,
                GLPeriodId = loan.GLPeriodId,
                Remark = loan.Remark,
                DateCreated = loan.DateCreated,
                LastModified = loan.LastModified,
                CreatedBy = loan.CreatedBy,
                OrgBranchName = loan.OrgBranchName,
                ModifiedBy = loan.ModifiedBy,
                IsActive = loan.IsActive
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTerminationUser, HrmConstants.HRAdmin, "HR_Admin")]
        public ActionResult Terminations_Read([DataSourceRequest]DataSourceRequest request)
        {
            db.Database.CommandTimeout = 7200;
            IQueryable<Core.Domain.HRM.EmployeeDetail.Termination> terminations = db.Terminations.Where(x => x.IsSendForApproval);

            DataSourceResult result = terminations.ToDataSourceResult(request, termination => new
            {
                TerminationId = termination.TerminationId,
                EmployeeId = termination.EmployeeId,
                BranchId = termination.BranchId,
                BusinessUnitId = termination.BusinessUnitId,
                OrgStructureId = termination.OrgStructureId,
                LookupId = termination.LookupId,
                Reason = termination.Reason,
                TerminationDate = termination.TerminationDate,
                ApplicationDate = termination.ApplicationDate,
                Remark = termination.Remark,
                TerminationDateAm = termination.TerminationDateAm,
                ApplicationDateAm = termination.ApplicationDateAm,
                DateCreated = termination.DateCreated,
                LastModified = termination.LastModified,
                CreatedBy = termination.CreatedBy,
                ModifiedBy = termination.ModifiedBy,
                OrgBranchName = termination.OrgBranchName,
                EmployeeCode = termination.EmployeeCode,
                IsDirect = termination.IsDirect,
                IsForciblyTerminated = termination.IsForciblyTerminated,
                IsApplied = termination.IsApplied,
                IsEarlyRetirement = termination.IsEarlyRetirement,
                IsActive = CheckIfActive(termination.EmployeeId),
                Status = termination.Status,
                SendForApprovalBy = termination.SendForApprovalBy,
                SendForApprovalTime = termination.SendForApprovalTime,
                IsSendForApproval = termination.IsSendForApproval,
                DepartmentSendForApprovalBy = termination.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = termination.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = termination.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = termination.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = termination.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = termination.IsDepartmentOnlineApproved,
                IsOnlineApproved = termination.IsOnlineApproved,
                OnlineApprovedBy = termination.OnlineApprovedBy,
                OnlineApprovedTime = termination.OnlineApprovedTime,
                IsVoid = termination.IsVoid,
                VoidBy = termination.VoidBy,
                VoidTime = termination.VoidTime,

                // Tier 1: Project HR Admin
                IsProjectHRApproved = termination.IsProjectHRApproved,
                ProjectHRApprovedBy = termination.ProjectHRApprovedBy,
                ProjectHRApprovedDate = termination.ProjectHRApprovedDate,

                // Tier 2: Project Manager
                IsPMApproved = termination.IsPMApproved,
                PMApprovedBy = termination.PMApprovedBy,
                PMApprovedDate = termination.PMApprovedDate,

                // Tier 3 (Optional): Department Head
                IsDepartmentHeadApproved = termination.IsDepartmentHeadApproved,
                DepartmentHeadApprovedBy = termination.DepartmentHeadApprovedBy,
                DepartmentHeadApprovedDate = termination.DepartmentHeadApprovedDate,

                // Tier 4 (Optional): HR Manager
                IsHRManagerApproved = termination.IsHRManagerApproved,
                HRManagerApprovedBy = termination.HRManagerApprovedBy,
                HRManagerApprovedDate = termination.HRManagerApprovedDate,

                // Tier Fallback / Additional Steps
                IsAdditionalApproved = termination.IsAdditionalApproved,
                AdditionalApprovedBy = termination.AdditionalApprovedBy,
                AdditionalApprovedDate = termination.AdditionalApprovedDate
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTerminationUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();
            bool isEthiopic = HrHelper.GetDatePreference();

            if (isEthiopic)
            {
                if (string.IsNullOrWhiteSpace(termination.ApplicationDateAm))
                {
                    ModelState.AddModelError("", "Application date is required.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }

                termination.ApplicationDate = HrHelper.ParseEthiopicDate(termination.ApplicationDateAm);

                if (!string.IsNullOrWhiteSpace(termination.TerminationDateAm))
                {
                    termination.TerminationDate = HrHelper.ParseEthiopicDate(termination.TerminationDateAm);
                }
            }
            else
            {
                if (termination.ApplicationDate == null || termination.ApplicationDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("", "Application date is required.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }

                termination.ApplicationDateAm = HrHelper.GetECDate(termination.ApplicationDate);

                if (termination.TerminationDate != null)
                {
                    termination.TerminationDateAm = HrHelper.GetECDate(termination.TerminationDate.Value);
                }
            }

            if (termination.ApplicationDate.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Application date must be today or earlier.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            bool exists = db.Terminations.Any(x => x.EmployeeId == termination.EmployeeId && !x.Employee.IsReHired);
            if (exists)
            {
                ModelState.AddModelError("", "Termination request already exists.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            var placement = db.Placements
                .Where(x => x.EmployeeId == termination.EmployeeId && x.isCurrent)
                .Select(x => new { x.BranchId, x.BusinessUnitId, x.OrgStructureId, x.EmployeeId, x.ResponsibilityLevel })
                .FirstOrDefault();

            if (placement == null)
            {
                ModelState.AddModelError("", "No active placement found.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            int noticeDays = db.TerminationSettings
                .Where(x => x.IsActive && x.ResponsibilityLevel == placement.ResponsibilityLevel)
                .Select(x => (int?)x.NoticePeriodDays)
                .FirstOrDefault() ?? 30;

            if (string.IsNullOrWhiteSpace(termination.TerminationDateAm) && termination.TerminationDate == null)
            {
                termination.TerminationDate = termination.ApplicationDate.AddDays(noticeDays);
            }
            else
            {
                if (termination.TerminationDate.Value.Date < termination.ApplicationDate.Date)
                {
                    ModelState.AddModelError("", "Termination date must be after application date.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }
            }

            termination.TerminationDateAm = HrHelper.GetECDate(termination.TerminationDate.Value);

            var diffDays = (termination.TerminationDate.Value - termination.ApplicationDate).TotalDays;
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);

            if (termination.IsEarlyRetirement && employee != null)
            {
                var age = DateTime.Now.Year - employee.DateOfBirth.Year;
                if (employee.DateOfBirth > DateTime.Now.AddYears(-age))
                {
                    age--;
                }
                if (age < 55)
                {
                    ModelState.AddModelError("", "Employee is not eligible for early retirement before age 55.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }
            }

            if (ModelState.IsValid)
            {
                var diff = (termination.TerminationDate.Value - termination.ApplicationDate).Days;

                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    EmployeeId = termination.EmployeeId,
                    BranchId = placement.BranchId,
                    BusinessUnitId = placement.BusinessUnitId,
                    OrgStructureId = placement.OrgStructureId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    ApplicationDate = termination.ApplicationDate,
                    TerminationDate = termination.TerminationDate,
                    Remark = termination.Remark,
                    ApplicationDateAm = termination.ApplicationDateAm,
                    TerminationDateAm = termination.TerminationDateAm,
                    DateCreated = DateTime.Now,
                    LastModified = termination.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = termination.ModifiedBy,
                    IsOnlineApproved = termination.IsOnlineApproved,
                    IsSendForApproval = true,
                    IsDirect = true,
                    IsEarlyRetirement = termination.IsEarlyRetirement,
                    IsApplied = termination.IsApplied,
                    EmployeeCode = db.Person_Employee.Find(termination.EmployeeId).EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                };

                db.Terminations.Add(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                termination.TerminationId = entity.TerminationId;

                UserHelper.OperationLog(Request.UserHostAddress, "Termination ", "Success", User.Identity.Name, User.Identity.Name + " add termination of id " + entity.TerminationId, Modules.HRM);

                if (diff < 30)
                {
                    string messages = "Your termination request date length is less than 30. You need to return one month salary to finance!!";
                    termination.Remark = "The Date Length is less than 30.";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }
            }
            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTerminationUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(termination.TerminationDateAm)
                || string.IsNullOrEmpty(termination.ApplicationDateAm))
                {
                    ModelState.AddModelError("", "Both application and Termination Date fields are mandatory");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }

                termination.TerminationDate = HrHelper.ParseEthiopicDate(termination.TerminationDateAm);
                termination.ApplicationDate = HrHelper.ParseEthiopicDate(termination.ApplicationDateAm);
            }
            else
            {
                termination.TerminationDateAm = HrHelper.GetECDate(termination.TerminationDate.Value);
                termination.ApplicationDateAm = HrHelper.GetECDate(termination.ApplicationDate);
            }

            var placement = db.Placements
             .Where(x => x.EmployeeId == termination.EmployeeId && x.isCurrent)
             .Select(x => new { x.BranchId, x.BusinessUnitId, x.OrgStructureId, x.EmployeeId, x.ResponsibilityLevel })
             .FirstOrDefault();

            if (placement == null)
            {
                ModelState.AddModelError("", "No active placement found.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            if (termination.TerminationDate.Value.Date < termination.ApplicationDate.Date)
            {
                ModelState.AddModelError("", "Termination Date Can't Be Below Application Date");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            if (termination.ApplicationDate.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            var checkExistance = db.Terminations.FirstOrDefault(x =>
                                    x.EmployeeId == termination.EmployeeId &&
                                    x.TerminationDate == termination.TerminationDate
                                    && x.TerminationId != termination.TerminationId);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This employee already request the termination!!");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);

            if (termination.IsEarlyRetirement && employee != null)
            {
                var age = DateTime.Now.Year - employee.DateOfBirth.Year;
                if (employee.DateOfBirth > DateTime.Now.AddYears(-age))
                {
                    age--;
                }
                if (age < 55)
                {
                    ModelState.AddModelError("", "Employee is not eligible for early retirement before age 55.");
                    return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    TerminationId = termination.TerminationId,
                    EmployeeId = termination.EmployeeId,
                    BranchId = placement.BranchId,
                    BusinessUnitId = placement.BusinessUnitId,
                    OrgStructureId = placement.OrgStructureId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    TerminationDate = termination.TerminationDate,
                    ApplicationDate = termination.ApplicationDate,
                    Remark = termination.Remark,
                    TerminationDateAm = termination.TerminationDateAm,
                    ApplicationDateAm = termination.ApplicationDateAm,
                    DateCreated = termination.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = termination.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = termination.OrgBranchName,
                    EmployeeCode = db.Person_Employee.Find(termination.EmployeeId)?.EmployeeCode,
                    IsDirect = termination.IsDirect,
                    IsApplied = termination.IsApplied,
                    IsEarlyRetirement = termination.IsEarlyRetirement,
                    IsOnlineApproved = termination.IsOnlineApproved,
                    IsSendForApproval = termination.IsSendForApproval,
                    SendForApprovalBy = termination.SendForApprovalBy,
                    SendForApprovalTime = termination.SendForApprovalTime,
                    DepartmentOnlineApprovedBy = termination.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = termination.DepartmentOnlineApprovedTime,
                    DepartmentSendForApprovalTime = termination.DepartmentSendForApprovalTime,
                };

                db.Terminations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var diff = (termination.TerminationDate.Value - termination.ApplicationDate).TotalDays;

                UserHelper.OperationLog(Request.UserHostAddress, "Termination ", "Success", User.Identity.Name, User.Identity.Name + " update termination of id " + entity.TerminationId, Modules.HRM);

                if (diff < 30)
                {
                    string messages = "Your termination request date length is less than 30. You need to return one month salary to finance!!";
                    termination.Remark = "The Date Length is less than 30.";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }
            }

            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRAdmin, "HR_Admin")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terminations_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.EmployeeDetail.Termination termination)
        {
            ModelState.Clear();
            if (!CheckTerminationValidation(termination.EmployeeId))
            {
                ModelState.AddModelError("", "You cannot delete this Termination data.Please delete Exit Interviewl information first.");
                return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                {
                    TerminationId = termination.TerminationId,
                    EmployeeId = termination.EmployeeId,
                    BranchId = termination.BranchId,
                    BusinessUnitId = termination.BusinessUnitId,
                    OrgStructureId = termination.OrgStructureId,
                    LookupId = termination.LookupId,
                    Reason = termination.Reason,
                    ApplicationDate = termination.ApplicationDate,
                    TerminationDate = termination.TerminationDate,
                    Remark = termination.Remark,
                    DateCreated = termination.DateCreated,
                    LastModified = termination.LastModified,
                    CreatedBy = termination.CreatedBy,
                    ModifiedBy = termination.ModifiedBy,
                    OrgBranchName = termination.OrgBranchName,
                    IsOnlineApproved = termination.IsOnlineApproved,
                    IsDirect = termination.IsDirect,
                    IsApplied = termination.IsApplied,
                    IsEarlyRetirement = termination.IsEarlyRetirement,
                    EmployeeCode = termination.EmployeeCode
                };

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted termination information of employee " + termination.EmployeeCode);

                db.Terminations.Attach(entity);
                db.Terminations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { termination }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult UserCardItems_Read([DataSourceRequest]DataSourceRequest request, int employeeId)
        {
            IQueryable<UserCardItems> usercarditems = db.UserCardItems.Where(x => x.Active && x.EmployeeId == employeeId);
            DataSourceResult result = usercarditems.ToDataSourceResult(request, userCardItems => new
            {
                UserCardItemsID = userCardItems.UserCardItemsID,
                EmployeeId = userCardItems.EmployeeId,
                EmployeeFullName = userCardItems.EmployeeFullName,
                Name = userCardItems.Name,
                UOM = userCardItems.UOM,
                Quanity = userCardItems.Quanity,
                UnitPrice = userCardItems.UnitPrice,
                Tag = userCardItems.Tag,
                SerialNo = userCardItems.SerialNo,
                IssueDate = userCardItems.IssueDate,
                SIVNo = userCardItems.SIVNo,
                GRNDate = userCardItems.GRNDate,
                GRN = userCardItems.GRN,
                Remark = userCardItems.Remark,
                Active = userCardItems.Active,
                OrgBranchName = userCardItems.OrgBranchName,
                DateCreated = userCardItems.DateCreated,
                LastModified = userCardItems.LastModified,
                CreatedBy = userCardItems.CreatedBy,
                ModifiedBy = userCardItems.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                     AttachmentType.CLEARANCE.ToString()), fileName);
                    var mainPath =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                     AttachmentType.CLEARANCE.ToString()));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {
                            Reference = HRMInputs.EmployeeId.ToString(),
                            Description = file.FileName,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.CLEARANCE,
                            Index = 0,
                            URL = path,
                            Remark = HRMInputs.Owner
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

        public ActionResult DownloadFile(string filename)
        {
            if (HRMInputs.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }
            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.CLEARANCE && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
                            .FirstOrDefault();
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.CLEARANCE + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception) { }
            HRMInputs.SelectedFile = 0;
            RedirectToAction("Index");
            return null;
        }

        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;
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
        public bool Paid(int employeeId, int fiscalYearId)
        {
            var leave = db.LeavesEntitle
                          .FirstOrDefault(x => x.EmployeeId == employeeId
                                               && x.GlFiscalYearId == fiscalYearId);

            if (leave == null)
                return false;

            leave.IsExpired = true;
            leave.LastModified = DateTime.Now;
            leave.ModifiedBy = User.Identity.Name;

            db.Entry(leave).State = EntityState.Modified;
            db.SaveChanges();

            return true;
        }

        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    AttachmentId = attachment.AttachmentId,
                    ReferenceType = attachment.ReferenceType,
                    Reference = attachment.Reference,
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    Remark = attachment.Remark
                };

                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public JsonResult DeactivateTerminatedEmployees()
        {
            int processed = 0;
            int skipped = 0;
            var errors = new List<string>();

            try
            {
                using (var db = new ExceedDbContext())
                using (var adb = new ApplicationDbContext())
                {
                    var unUpdatedTerminations = db.Terminations
                        .Where(x => !x.BranchId.HasValue || !x.OrgStructureId.HasValue)
                        .ToList();

                    if (unUpdatedTerminations.Any())
                    {
                        foreach (var termination in unUpdatedTerminations)
                        {
                            var placement = db.Placements
                                .Where(x => x.EmployeeId == termination.EmployeeId)
                                .OrderByDescending(x => x.AssignmentDate)
                                .FirstOrDefault();

                            if (placement != null)
                            {
                                termination.BranchId = placement.BranchId;
                                termination.BusinessUnitId = placement.BusinessUnitId;
                                termination.OrgStructureId = placement.OrgStructureId;
                                db.Entry(termination).State = EntityState.Modified;
                            }
                        }
                        db.SaveChanges();
                    }

                    var today = DateTime.Today;
                    var terminations = db.Terminations
                        .Where(x => (x.IsOnlineApproved || x.IsForciblyTerminated)
                                    && !x.IsApplied
                                    && x.TerminationDate <= today)
                        .ToList();

                    if (!terminations.Any())
                    {
                        return Json(new
                        {
                            success = true,
                            message = "No terminated employees found to process."
                        }, JsonRequestBehavior.AllowGet);
                    }

                    foreach (var termination in terminations)
                    {
                        try
                        {
                            var person = db.Person_Employee
                                .FirstOrDefault(x => x.EmployeeId == termination.EmployeeId && x.IsActive);

                            if (person == null || person.IsReHired)
                            {
                                skipped++;
                                continue;
                            }

                            termination.IsApplied = true;
                            db.Entry(termination).State = EntityState.Modified;

                            person.IsActive = false;
                            db.Entry(person).State = EntityState.Modified;

                            var placements = db.Placements
                                .Where(x => x.EmployeeId == termination.EmployeeId && x.isCurrent)
                                .ToList();

                            foreach (var placement in placements)
                            {
                                placement.isCurrent = false;
                                db.Entry(placement).State = EntityState.Modified;
                            }

                            db.SaveChanges();

                            var user = adb.Users.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId);
                            if (user != null)
                            {
                                user.IsEnabled = false;
                                adb.Entry(user).State = EntityState.Modified;
                                adb.SaveChanges();
                            }

                            processed++;

                            UserHelper.OperationLog(
                                Request.UserHostAddress,
                                "Termination deactivation",
                                "Success",
                                User.Identity.Name,
                                string.Format("Deactivated termination (ID: {0}) for employee {1} - {2} {3} {4}",
                                    termination.TerminationId,
                                    person.EmployeeCode,
                                    person.FirstNameEnglish,
                                    person.MiddleNameEnglish,
                                    person.LastNameEnglish),
                                Modules.HRM
                            );
                        }
                        catch (Exception ex)
                        {
                            skipped++;
                            errors.Add(string.Format("EmployeeId {0}: {1}", termination.EmployeeId, ex.Message));
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    message = string.Format("Process completed. {0} employee(s) deactivated. {1} skipped.", processed, skipped),
                    errors
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "A critical error occurred while processing terminations.",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}