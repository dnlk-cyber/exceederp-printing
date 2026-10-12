using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Global;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Models;
using System.IO;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService)]
    public class LeaveRequestPersonalController : BaseController
    {
        // GET: HRM/LeaveRequestPersonal
        private ExceedDbContext db = new ExceedDbContext();
        static ApplicationDbContext dbApplication = new ApplicationDbContext();
        private readonly CacheData CacheData = new CacheData();

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
            ViewData["EmployeeName"] = CacheData.GetEmployeeCaches().Select(
          i => new
          {
              FullNameAmharic = i.AmharicName,
              FullNameaEnglish = i.Name,
              FullName = i.Name,
              FullNameCode = i.Code + " " + i.Name,
              EmployeeCode = i.Code,
              EmployeeId = i.Id,

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
               }).AsQueryable();
            ViewData["Department"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
                i => new
                {
                    Description = i.Name,
                    LookupId = i.Id,
                }).AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? false;
        }

        public JsonResult SendForApproval(int id)
        {
            var leaveRequest = db.LeaveRequest.Find(id);
            if (leaveRequest == null)
            {
                return Json(new { Message = "Leave request not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            bool isAlreadyWaitingForApproval = db.RequestesStatus.Any(x => x.RequestId == id && x.Status == RequestStatus.WAITING_FOR_APPROVAL);

            if ((leaveRequest.IsSendForApproval || leaveRequest.IsDepartmentSendForApproval)
                && isAlreadyWaitingForApproval)
            {
                return Json(new { Message = "The Leave Request is already sent for approval.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            if (db.LeaveRequest.Any(x => !x.IsVoid
                                     && x.EmployeeId == leaveRequest.EmployeeId
                                     && x.LeaveRequestId != leaveRequest.LeaveRequestId
                                     && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) ||
                                        (x.From <= leaveRequest.To && x.To >= leaveRequest.To)
                                    )))
            {

                    return Json(new
                    {
                        Message = "You can't send this request; it overlaps with another leave request you already submitted.",
                        Status = "Not-Ok"
                    }, JsonRequestBehavior.AllowGet);

            }

            var requestStatus = new RequestesStatus
            {
                RequestId = id,
                ApproverDepartment = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name),
                ApproverId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                ReferenceType = "Leave",
                Status = RequestStatus.WAITING_FOR_APPROVAL,
                Date = DateTime.Now,
                DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
            };

            leaveRequest.IsSendForApproval = true;
            leaveRequest.IsDepartmentSendForApproval = true;
            leaveRequest.SendForApprovalBy = User.Identity.Name;
            leaveRequest.Status = RequestStatus.WAITING_FOR_APPROVAL.ToString();
            leaveRequest.SendForApprovalTime = DateTime.Now;

            db.RequestesStatus.Add(requestStatus);
            db.Entry(leaveRequest).State = EntityState.Modified;
            db.SaveChanges();

            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var headId = EmployeeHelper.GetHeadId(empId);
            if (headId !=0)
            {
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(headId),
                    Title = "Leave Request",
                    Message = $"{User.Identity.Name} has submitted a leave request that requires your approval.",
                    URL = "/HRM/LeaveRequest"
                };

                Notify.NotifyUser(notif);
            }
            return Json(new { Message = "Request successfully sent for approval.", Status = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveRequestPersonal_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);
            // targeted correlated subquery instead of grouping the whole RequestesStatus table (was very slow on first load)
            var query = from lr in db.LeaveRequest.AsNoTracking()
                        where ((employeeIds.Contains(lr.EmployeeId)&&lr.IsForOther)||lr.EmployeeId== employeeId)
                        let latestStatus = db.RequestesStatus
                            .Where(r => r.ReferenceType == "Leave" && r.RequestId == lr.LeaveRequestId)
                            .OrderByDescending(r => r.RequestStatusId)
                            .Select(r => (RequestStatus?)r.Status)
                            .FirstOrDefault()
                        orderby lr.RequestDate descending
                        select new
                        {
                            lr.LeaveRequestId,
                            lr.EmployeeId,
                            lr.BranchId,
                            lr.BusinessUnitId,
                            lr.LookupId,
                            lr.From,
                            lr.GlFiscalYearId,
                            lr.To,
                            lr.LeaveDays,
                            lr.LeaveActionTypes,
                            lr.Type,
                            lr.LeaveBalance,
                            lr.Hours,
                            lr.Day,
                            lr.ExactDeserveLeaveDays,
                            lr.Reason,
                            lr.Remark,
                            lr.RequestDate,
                            lr.FromAmharic,
                            lr.ToAmharic,
                            // handle cases where RequestesStatus is missing
                            IsApproved = latestStatus == RequestStatus.APPROVE,
                            IsChangeInDepartment = latestStatus == RequestStatus.DEPARTMENT_APPROVAL,
                            IsPending = latestStatus == RequestStatus.PEND,
                            IsRejected = latestStatus == RequestStatus.REJECT,
                            EmployeeCode = lr.Employee.EmployeeCode, // navigation property
                            lr.DepartmentOnlineApprovedBy,
                            lr.Status,
                            lr.DepartmentSendForApprovalBy,
                            lr.DepartmentSendForApprovalTime,
                            lr.IsDepartmentSendForApproval,
                            lr.DepartmentOnlineApprovedTime,
                            lr.IsForOther,
                            lr.IsDepartmentOnlineApproved,
                            lr.OnlineApprovedBy,
                            lr.OnlineApprovedTime,
                            lr.IsOnlineApproved,
                            lr.MorningOrAfternoon,
                            lr.IsSendForApproval,
                            lr.ToTime,
                            lr.IsVoid,
                            lr.VoidBy,
                            lr.CreatedBy,
                            lr.LastModified,
                            lr.DateCreated,
                            lr.VoidTime
                        };

            DataSourceResult result = query.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequestPersonal_Create([DataSourceRequest] DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();

            // 1. Employee Resolution
            leaveRequest.EmployeeId = leaveRequest.IsForOther ? leaveRequest.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (leaveRequest.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select an employee first!! Or refresh the page to clear the cache");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var totalBalance = db.LeavesEntitle
                .Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId)
                .Select(x => x.Balance)
                .DefaultIfEmpty(0)
                .Sum(x => x);

            #region Pre Conditions & Date Parsing
            bool useEthiopian = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? false;

            if (useEthiopian)
            {
                if (string.IsNullOrWhiteSpace(leaveRequest.FromAmharic) || string.IsNullOrWhiteSpace(leaveRequest.ToAmharic))
                {
                    ModelState.AddModelError("", "Both From and To Ethiopian Date fields are mandatory.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                leaveRequest.From = HrHelper.ParseEthiopicDate(leaveRequest.FromAmharic);
                leaveRequest.To = HrHelper.ParseEthiopicDate(leaveRequest.ToAmharic);
            }
            else
            {
                if (leaveRequest.From == default(DateTime) || leaveRequest.To == default(DateTime))
                {
                    ModelState.AddModelError("", "Both From and To Gregorian Date fields are mandatory.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                leaveRequest.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.From.Day, leaveRequest.From.Month, leaveRequest.From.Year);
                leaveRequest.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.To.Day, leaveRequest.To.Month, leaveRequest.To.Year);
            }

            if (leaveRequest.From.Date > leaveRequest.To.Date)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
            {
                ModelState.AddModelError("", "No fiscal year is defined for the selected leave start date. Please define the fiscal period or select a valid date.");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            leaveRequest.GlFiscalYearId = fiscalYear.GlFiscalYearId;
            #endregion

            #region Overlap Validations
            if (leaveRequest.From.Date == leaveRequest.To.Date)
            {
                var existingLeave = db.LeaveRequest.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId && !x.IsVoid);
                if (existingLeave != null)
                {
                    if ((leaveRequest.FromTime?.TimeOfDay >= existingLeave.FromTime?.TimeOfDay && leaveRequest.FromTime?.TimeOfDay < existingLeave.ToTime?.TimeOfDay) ||
                        (leaveRequest.ToTime?.TimeOfDay > existingLeave.FromTime?.TimeOfDay && leaveRequest.ToTime?.TimeOfDay <= existingLeave.ToTime?.TimeOfDay) ||
                        (leaveRequest.FromTime?.TimeOfDay <= existingLeave.FromTime?.TimeOfDay && leaveRequest.ToTime?.TimeOfDay >= existingLeave.ToTime?.TimeOfDay))
                    {
                        ModelState.AddModelError("", "Selected time range overlaps with an existing leave request. Please select another time range.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            else
            {
                if (db.LeaveRequest.Any(x => !x.IsVoid && x.EmployeeId == leaveRequest.EmployeeId && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) || (x.From <= leaveRequest.To && x.To >= leaveRequest.To))))
                {
                    ModelState.AddModelError("", "Your request overlaps with another request");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }
            #endregion

            #region Leave Type & Policy Validations
            var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == leaveRequest.Type);
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.IsActive);

            if (employee != null)
            {
                if (leaveRequest.Type == LeaveType.MATERNITY && employee.Gender == Gender.MALE)
                {
                    ModelState.AddModelError("", "Males can't take Maternity Leave");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                if (leaveRequest.Type == LeaveType.PATERNITY && employee.Gender == Gender.FEMALE)
                {
                    ModelState.AddModelError("", "Females can't take Paternity Leave");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                if (leaveRequest.Type == LeaveType.MATERNITY)
                {
                    var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == LeaveHrmSetting.Maternitiy.ToString());
                    if (config != null)
                    {
                        var allowedDays = Convert.ToDecimal(config.CurrentValue);
                        if (leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", "You are requesting maternity leave above the setted Maximum value");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }

                        var maternityRequests = db.LeaveRequest.Where(x => x.Type == LeaveType.MATERNITY && x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId && x.IsOnlineApproved).ToList();
                        if (maternityRequests.Any() && maternityRequests.Sum(x => x.LeaveDays) + leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", $"You can't take maternity leave more than {allowedDays} days in one year, check your previous maternity leave requests");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
            }

            if (leaveRequest.Type == LeaveType.Leave_With_Out_Pay && totalBalance > 0)
            {
                ModelState.AddModelError("", "You can't request Leave with out pay having annual leave use your leave first");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced)
            {
                if (leaveRequest.Type != LeaveType.ANNUAL && leaveRequest.Type != LeaveType.OTHER && leaveRequest.Type != LeaveType.Leave_With_Out_Pay)
                {
                    ModelState.AddModelError("", $"You can't take Forced Leave for {leaveRequest.Type}, it's allowed only for Annual, Leave without pay, and Other leave types");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }
            #endregion

            #region Leave Days Calculation (Including Mixed Support)
            if (leaveRequest.Day == LeaveDay.Mixed)
            {
                // For Mixed day type, leaveRequest.LeaveDays already holds the user-entered amount directly.
                // We only validate that a valid positive amount has been entered.
                if (leaveRequest.LeaveDays <= 0)
                {
                    ModelState.AddModelError("", "Please enter a valid amount of leave days for Mixed day type.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                if (leaveRequest.Type == LeaveType.ANNUAL)
                {
                    decimal leaveDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
                    if(leaveRequest.LeaveDays> leaveDays)
                    {
                        ModelState.AddModelError("", "Please enter a valid amount of leave days for Mixed day type.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                if (leaveRequest.From.Date != leaveRequest.To.Date)
                {
                    leaveRequest.LeaveDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
                }
                else if (leaveRequest.From.Date == leaveRequest.To.Date && leaveRequest?.ToTime > leaveRequest?.FromTime)
                {
                    leaveRequest.LeaveDays = (decimal)(leaveRequest?.ToTime.Value - leaveRequest?.FromTime.Value).Value.TotalHours / 8;
                }
                else if (leaveRequest.From.Date == leaveRequest.To.Date && (leaveRequest.ToTime == null || leaveRequest.FromTime == null) && leaveRequest.Hours == 0)
                {
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? 1 : 0.5m;
                }
                else
                {
                    leaveRequest.LeaveDays = (decimal)leaveRequest.Hours / 8;
                }
            }
            else
            {
                if (leaveRequest.From.Date == leaveRequest.To.Date && leaveRequest?.ToTime > leaveRequest?.FromTime)
                {
                    leaveRequest.LeaveDays = (decimal)(leaveRequest?.ToTime.Value - leaveRequest?.FromTime.Value).Value.TotalHours / 8;
                }
                else if (leaveRequest.From.Date < leaveRequest.To.Date)
                {
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? (decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1 : ((decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1) / 2;
                }
                else if (leaveRequest.From.Date == leaveRequest.To.Date && (leaveRequest.ToTime == null || leaveRequest.FromTime == null) && leaveRequest.Hours == 0)
                {
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? 1 : 0.5m;
                }
                else
                {
                    leaveRequest.LeaveDays = (decimal)leaveRequest.Hours / 8;
                }
            }

            // Time segment adjustment
            decimal afternoonOffDays = 0;
            if (leaveRequest.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
            {
                afternoonOffDays = IsHalfDays(leaveRequest.From, leaveRequest.To);
            }
            if (afternoonOffDays > 0)
            {
                leaveRequest.LeaveDays -= afternoonOffDays;
            }

            if (leaveRequest.LeaveDays <= 0)
            {
                ModelState.AddModelError("", "Leave days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            #endregion

            #region Balance & Plan Validations
            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                if (leaveRequest.LeaveDays > totalBalance)
                {
                    ModelState.AddModelError("", $"The requested leave days ({leaveRequest.LeaveDays}) cannot exceed the available leave balance ({totalBalance}).");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                var leavePlan = db.LeavePlans.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId && (leaveRequest.From >= x.From || leaveRequest.To <= x.To));
                if (leavePlan != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    if (leaveRequest.From < leavePlan.From)
                    {
                        ModelState.AddModelError("", "The start date of your leave is before the start date of your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                    if (leaveRequest.To > leavePlan.To)
                    {
                        ModelState.AddModelError("", "The end date of your leave is after the end date of your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                    if (leaveRequest.LeaveDays > leavePlan.LeaveAmount)
                    {
                        ModelState.AddModelError("", "The requested number of leave days exceeds the amount allocated in your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
                else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    ModelState.AddModelError("", "You must set a leave plan for this period.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL && totalBalance <= 0)
            {
                ModelState.AddModelError("", "You have no entitlement for this budget year");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            #endregion

            #region Placement & Settings Validations
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                leaveRequest.BranchId = placement.BranchId;
                leaveRequest.BusinessUnitId = placement.BusinessUnitId;
                leaveRequest.LookupId = placement.LookupId;
            }
            else
            {
                ModelState.AddModelError("placement", "Employee does not have a current placement");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            if (leaveTypeSetting != null)
            {
                if (leaveTypeSetting.MaxDaysBack.HasValue && leaveTypeSetting.BlockBackdatedLeave)
                {
                    var limitDate = DateTime.Today.AddDays(-leaveTypeSetting.MaxDaysBack.Value);
                    if (leaveRequest.From < limitDate)
                    {
                        string errorMessage = $"You cannot request earlier than {limitDate:MMMM dd, yyyy} (max {leaveTypeSetting.MaxDaysBack.Value} days back from today).";
                        ModelState.AddModelError("block", errorMessage);
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
                if (leaveTypeSetting.BlockBackdatedLeave && !leaveTypeSetting.MaxDaysBack.HasValue && leaveRequest.From.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("block", "You cannot request leave for a past date. Please adjust your leave Type Setting or contact HR");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;

                var newRequest = new LeaveRequest
                {
                    EmployeeId = leaveRequest.EmployeeId,
                    BranchId = leaveRequest.BranchId,
                    BusinessUnitId = leaveRequest.BusinessUnitId,
                    LookupId = leaveRequest.LookupId,
                    From = leaveRequest.From,
                    To = leaveRequest.To,
                    FromTime = leaveRequest.FromTime,
                    ToTime = leaveRequest.ToTime,
                    LeaveDays = leaveRequest.LeaveDays,
                    Type = leaveRequest.Type,
                    LeaveActionTypes = leaveRequest.LeaveActionTypes,
                    Reason = leaveRequest.Reason,
                    Remark = leaveRequest.Remark,
                    GlFiscalYearId = fiscalYear.GlFiscalYearId,
                    RequestDate = DateTime.Now,
                    LeaveBalance = totalBalance,
                    FromAmharic = leaveRequest.FromAmharic,
                    ToAmharic = leaveRequest.ToAmharic,
                    Day = leaveRequest.Day,
                    ExactDeserveLeaveDays = leaveRequest.ExactDeserveLeaveDays,
                    MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                    Hours = leaveRequest.Hours,
                    IsForOther = leaveRequest.IsForOther,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = leaveRequest.ModifiedBy,
                    LastModified = leaveRequest.LastModified,
                    EmployeeCode = employee?.EmployeeCode,
                    RequesterId = UserInformationHelper.GetEmployeeId(User.Identity.Name)
                };

                db.LeaveRequest.Add(newRequest);
                db.SaveChanges();
                leaveRequest.LeaveRequestId = newRequest.LeaveRequestId;

                UserHelper.OperationLog(Request.UserHostAddress, "LeaveRequestPersonal", "Success", User.Identity.Name, $"{User.Identity.Name} requested leave of type {newRequest.Type}, Leave Request Id {newRequest.LeaveRequestId}", Modules.HRM);
                GetNotiofied(leaveRequest);
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequestPersonal_Update([DataSourceRequest] DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();

            var leaveRequestEntity = db.LeaveRequest.Find(leaveRequest.LeaveRequestId);
            if (leaveRequestEntity == null)
            {
                ModelState.AddModelError("", "Leave request not found.");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            // 1. Employee Resolution
            leaveRequest.EmployeeId = leaveRequest.IsForOther ? leaveRequest.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);
            if (leaveRequest.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select an employee first!! Or refresh the page to clear the cache");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var totalBalance = db.LeavesEntitle
                .Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId)
                .Select(x => x.Balance)
                .DefaultIfEmpty(0)
                .Sum(x => x);

            #region Pre Conditions & Date Parsing
            bool useEthiopian = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? false;

            if (useEthiopian)
            {
                if (string.IsNullOrWhiteSpace(leaveRequest.FromAmharic) || string.IsNullOrWhiteSpace(leaveRequest.ToAmharic))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                leaveRequest.From = HrHelper.ParseEthiopicDate(leaveRequest.FromAmharic);
                leaveRequest.To = HrHelper.ParseEthiopicDate(leaveRequest.ToAmharic);
            }
            else
            {
                if (leaveRequest.From == default(DateTime) || leaveRequest.To == default(DateTime))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                leaveRequest.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.From.Day, leaveRequest.From.Month, leaveRequest.From.Year);
                leaveRequest.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.To.Day, leaveRequest.To.Month, leaveRequest.To.Year);
            }

            if (leaveRequest.From.Date > leaveRequest.To.Date)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
            {
                ModelState.AddModelError("", "No fiscal year is defined for the selected leave start date. Please define the fiscal period or select a valid date.");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            leaveRequest.GlFiscalYearId = fiscalYear.GlFiscalYearId;
            #endregion

            #region Leave Days Calculation & Mixed Support
            var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == leaveRequest.Type);

            if (leaveRequest.Day == LeaveDay.Mixed)
            {
                if (leaveRequest.LeaveDays <= 0)
                {
                    ModelState.AddModelError("", "Please enter a valid amount of leave days for Mixed day type.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                if (leaveRequest.Type == LeaveType.ANNUAL)
                {
                    decimal maxCalculatedDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
                    if (leaveRequest.LeaveDays > maxCalculatedDays)
                    {
                        ModelState.AddModelError("", "The entered leave amount exceeds the maximum calculated days for the selected date range.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            else if (leaveRequest.From.Date == leaveRequest.To.Date && leaveRequest.From.Date == leaveRequestEntity.From.Date && leaveRequest.To.Date == leaveRequestEntity.To.Date && leaveRequest.Day == leaveRequestEntity.Day)
            {
                leaveRequest.LeaveDays = leaveRequestEntity.LeaveDays;
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                if (leaveRequest.From.Date < leaveRequest.To.Date)
                {
                    leaveRequest.LeaveDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
                }
                else if (leaveRequest?.ToTime > leaveRequest?.FromTime)
                {
                    leaveRequest.LeaveDays = (decimal)(leaveRequest?.ToTime.Value - leaveRequest?.FromTime.Value).Value.TotalHours / 8;
                }
                else if (leaveRequest.To.Date == leaveRequest.From.Date)
                {
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? 1 : 0.5m;
                }
                else
                {
                    leaveRequest.LeaveDays = (decimal)leaveRequest.Hours / 8;
                }
            }
            else
            {
                leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? (decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1 : ((decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1) / 2;
            }

            leaveRequest.LeaveDays = Math.Abs(leaveRequest.LeaveDays);

            // Afternoon offset adjustment
            decimal afternoonOffDays = 0;
            if (leaveRequest.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
            {
                afternoonOffDays = IsHalfDays(leaveRequest.From, leaveRequest.To);
            }
            if (afternoonOffDays > 0)
            {
                leaveRequest.LeaveDays -= afternoonOffDays;
            }

            if (leaveRequest.LeaveDays <= 0)
            {
                ModelState.AddModelError("", "Leave days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            #endregion

            #region Validations & Policy Checks
            if (leaveRequest.Type == LeaveType.Leave_With_Out_Pay && totalBalance > 0)
            {
                ModelState.AddModelError("", $"You can't request Leave without pay having annual leave Amount = {totalBalance}, use your leave first");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            if (leaveRequest.LeaveDays > totalBalance && leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", $"The requested leave days ({leaveRequest.LeaveDays}) cannot exceed the available leave balance ({totalBalance}).");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            // Overlap validation excluding current record
            if (db.LeaveRequest.Any(x => !x.IsVoid && x.EmployeeId == leaveRequest.EmployeeId && x.LeaveRequestId != leaveRequestEntity.LeaveRequestId && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) || (x.From <= leaveRequest.To && x.To >= leaveRequest.To))))
            {
                ModelState.AddModelError("", "Your request overlaps with another existing leave request.");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.IsActive);
            if (employee != null)
            {
                if (leaveRequest.Type == LeaveType.MATERNITY && employee.Gender == Gender.MALE)
                {
                    ModelState.AddModelError("", "Males can't take Maternity Leave");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                if (leaveRequest.Type == LeaveType.PATERNITY && employee.Gender == Gender.FEMALE)
                {
                    ModelState.AddModelError("", "Females can't take Paternity Leave");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            var allowedTypes = new[] { LeaveType.ANNUAL, LeaveType.OTHER, LeaveType.Leave_With_Out_Pay };
            if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced && !allowedTypes.Contains(leaveRequest.Type))
            {
                ModelState.AddModelError("Type", $"Forced Leave is only allowed for: {string.Join(", ", allowedTypes)}. You selected: {leaveRequest.Type}.");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                leaveRequest.BranchId = placement.BranchId;
                leaveRequest.BusinessUnitId = placement.BusinessUnitId;
                leaveRequest.LookupId = placement.LookupId;
            }
            else
            {
                ModelState.AddModelError("placement", "Employee does not have a current placement");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            #endregion

            if (ModelState.IsValid)
            {
                leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;

                leaveRequestEntity.EmployeeId = leaveRequest.EmployeeId;
                leaveRequestEntity.BusinessUnitId = leaveRequest.BusinessUnitId;
                leaveRequestEntity.BranchId = leaveRequest.BranchId;
                leaveRequestEntity.LookupId = leaveRequest.LookupId;
                leaveRequestEntity.From = leaveRequest.From;
                leaveRequestEntity.To = leaveRequest.To;
                leaveRequestEntity.FromTime = leaveRequest.FromTime;
                leaveRequestEntity.ToTime = leaveRequest.ToTime;
                leaveRequestEntity.LeaveDays = leaveRequest.LeaveDays;
                leaveRequestEntity.ExactDeserveLeaveDays = leaveRequest.ExactDeserveLeaveDays;
                leaveRequestEntity.Type = leaveRequest.Type;
                leaveRequestEntity.Reason = leaveRequest.Reason;
                leaveRequestEntity.GlFiscalYearId = fiscalYear.GlFiscalYearId;
                leaveRequestEntity.Remark = leaveRequest.Remark;
                leaveRequestEntity.RequestDate = DateTime.Now;
                leaveRequestEntity.LastModified = DateTime.Now;
                leaveRequestEntity.ModifiedBy = User.Identity.Name;
                leaveRequestEntity.LeaveBalance = totalBalance;
                leaveRequestEntity.Day = leaveRequest.Day;
                leaveRequestEntity.MorningOrAfternoon = leaveRequest.MorningOrAfternoon;
                leaveRequestEntity.Hours = leaveRequest.Hours;
                leaveRequestEntity.FromAmharic = leaveRequest.FromAmharic;
                leaveRequestEntity.ToAmharic = leaveRequest.ToAmharic;
                leaveRequestEntity.IsForOther = leaveRequest.IsForOther;
                leaveRequestEntity.LeaveActionTypes = leaveRequest.LeaveActionTypes;
                leaveRequestEntity.EmployeeCode = employee?.EmployeeCode;
                leaveRequestEntity.RequesterId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                db.Entry(leaveRequestEntity).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "LeaveRequestPersonal", "Success", User.Identity.Name, $"{User.Identity.Name} updated leave request of id {leaveRequest.LeaveRequestId}", Modules.HRM);
                GetNotiofied(leaveRequestEntity);
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequestPersonal_Destroy([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
            if (leaveRequest.IsOnlineApproved)
            {
                ModelState.AddModelError("", "you can't delete approved requestes");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new LeaveRequest
                    {
                        LeaveRequestId = leaveRequest.LeaveRequestId,
                        EmployeeId = leaveRequest.EmployeeId,
                        BranchId = leaveRequest.BranchId,
                        BusinessUnitId = leaveRequest.BusinessUnitId,
                        LookupId = leaveRequest.LookupId,
                        From = leaveRequest.From,
                        To = leaveRequest.To,
                        LeaveDays = leaveRequest.LeaveDays,
                        ExactDeserveLeaveDays = leaveRequest.ExactDeserveLeaveDays,
                        RequestDate = leaveRequest.RequestDate,
                        FromTime = leaveRequest.FromTime,
                        ToTime = leaveRequest.ToTime,
                        Type = leaveRequest.Type,
                        Reason = leaveRequest.Reason,
                        GlFiscalYearId = leaveRequest.GlFiscalYearId,
                        Remark = leaveRequest.Remark,
                        LeaveBalance = leaveRequest.LeaveBalance,
                        Day = leaveRequest.Day,
                        MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                        Hours = leaveRequest.Hours,
                        FromAmharic = leaveRequest.FromAmharic,
                        ToAmharic = leaveRequest.ToAmharic,
                        EmployeeCode = leaveRequest.EmployeeCode,
                        IsForOther = leaveRequest.IsForOther,
                        RequesterId = leaveRequest.RequesterId,
                        LeaveActionTypes = leaveRequest.LeaveActionTypes
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted leave request of employee code  " + leaveRequest.EmployeeCode);
                    #endregion
                    db.LeaveRequest.Attach(entity);
                    db.LeaveRequest.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }
        private void GetNotiofied(LeaveRequest leaveRequest)
        {
            NotifModels notif = new NotifModels
            {
                UserId = UserInformationHelper.GetEmployeeUserId(leaveRequest.EmployeeId),
                Title = $"You have a leave Request {leaveRequest.FromAmharic} - {leaveRequest.ToAmharic} for {leaveRequest.LeaveDays}",
                Message = $"You have a leave Request {leaveRequest.FromAmharic} - {leaveRequest.ToAmharic} for {leaveRequest.LeaveDays}",
                URL = "/HRM/LeaveRequestPersonal"
            };

            Notify.NotifyUser(notif);
        }

        public decimal GetEmployeeDeserveLeaveDays(DateTime from, DateTime to, decimal amount, decimal balance, decimal totalbalance)
        {
            decimal exactDeserve = 0;

            var workedDays = HrHelper.GetWorkingDays(from, to);

            var deserveAmountCurrentYear = (amount * workedDays) / 365;

            var previousEntitle = totalbalance - balance;

            exactDeserve = previousEntitle + deserveAmountCurrentYear;

            var modul = exactDeserve % 1;
            decimal added = 0;

            if (modul >= Convert.ToDecimal(0.7))
            {
                added = 1;
            }
            else if (modul <= Convert.ToDecimal(0.3))
            {
                added = 0;
            }
            else
            {
                added = Convert.ToDecimal(0.5);
            }

            exactDeserve = (int)exactDeserve + added;
            if (exactDeserve > totalbalance)
            {
                exactDeserve = totalbalance;
            }
            return exactDeserve;
        }
        public decimal GetCurrentYearDeserve(DateTime from, DateTime to, decimal amount)
        {

            var workedDays = (to.Date - from.Date).Days;

            var deserveAmountCurrentYear = (amount * workedDays) / 365;

            return deserveAmountCurrentYear;
        }
        [HttpPost]
        public JsonResult SetEmployee(bool isForOther)
        {
            if (isForOther)
            {
                // Checked: user wants to request for another employee
                // Verify if user has subordinates
                var currentEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var subordinates = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(currentEmpId);

                if (!subordinates.Any())
                {
                    // No subordinates, cannot request for others
                    return Json(0, JsonRequestBehavior.AllowGet);
                }

                return Json(1, JsonRequestBehavior.AllowGet); // okay to request for others
            }

            // Unchecked: get the logged-in employee ID
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            return Json(empId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string GetTotalLeaveAmount(int empId, DateTime? from, DateTime? to, string fromAm, string toAm, MorningOrAfternoon morningOrAfternoon)
        {
            DateTime fromDate = !string.IsNullOrEmpty(fromAm) ? HrHelper.ParseEthiopicDate(fromAm) : from ?? DateTime.Now;
            DateTime toDate = !string.IsNullOrEmpty(toAm) ? HrHelper.ParseEthiopicDate(toAm) : to ?? DateTime.Now;
            return new LeaveRequestEditService(db).CalculateAmounts(ResolveEmployeeId(empId), fromDate, toDate, morningOrAfternoon);
        }

        [HttpPost]
        public string GetTotalLeaveAmountGorg(int empId, DateTime? from, DateTime? to, MorningOrAfternoon morningOrAfternoon)
        {
            return new LeaveRequestEditService(db).CalculateAmounts(ResolveEmployeeId(empId), from ?? DateTime.Now, to ?? DateTime.Now, morningOrAfternoon);
        }

        // Falls back to the logged-in employee when no employee is selected.
        private int ResolveEmployeeId(int empId)
        {
            return empId != 0 ? empId : UserInformationHelper.GetEmployeeId(User.Identity.Name);
        }

        public decimal GetCurrentEmpBalance(int period)
        {

            var user = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var balance = db.LeavesEntitle.Where(x => x.EmployeeId == user && !x.IsExpired);
            decimal totalBalance = 0;
            if (balance != null)
            {
                totalBalance = balance.Sum(x => x.Balance);
            }
            return totalBalance;

        }
        public ActionResult UpdateIsOnlineApproved()
        {
            var leaveRequests = db.LeaveRequest.Where(x => !x.IsOnlineApproved).ToList();
            foreach (var leaveRequest in leaveRequests)
            {
                var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Leave");
                if (requestStatus != null)
                {
                    leaveRequest.IsOnlineApproved = true;
                    leaveRequest.IsDepartmentOnlineApproved = true;
                    leaveRequest.IsDepartmentSendForApproval = true;

                    db.Entry(leaveRequest).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var requestSt = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Leave");
                    if (requestSt != null)
                    {
                        leaveRequest.IsDepartmentOnlineApproved = true;
                        leaveRequest.IsDepartmentSendForApproval = true;

                        db.Entry(leaveRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var requestS = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leaveRequest.LeaveRequestId && x.Status == RequestStatus.WAITING_FOR_APPROVAL && x.ReferenceType == "Leave");
                        if (requestS != null)
                        {
                            leaveRequest.IsDepartmentSendForApproval = true;

                            db.Entry(leaveRequest).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }
        public ActionResult UploadDocument(int leaveRequestId,HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + leaveRequestId.ToString() + "-" + AttachmentType.DOCUMENT), leaveRequestId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + leaveRequestId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = leaveRequestId.ToString(),
                            Description = leaveRequestId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = leaveRequestId.ToString()
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
        public ActionResult DownloadDoument(string filename, string attachId)
        {
            int.TryParse(attachId, out int attachmentId);
            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        private  decimal IsHalfDays(DateTime startDate, DateTime endDate)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal half = 0;
            var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
            var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {

                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null && saturdayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                half += 0.5m;
                            }

                        }

                        break;
                    case DayOfWeek.Sunday:

                        if (sundayweekendSetting != null && sundayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                half += 0.5m;
                            }
                        }

                        break;
                }
            }
            return half;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
