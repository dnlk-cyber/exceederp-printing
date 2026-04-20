﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Global;
using System.IO;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class LeaveRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        static ApplicationDbContext dbApplication = new ApplicationDbContext();
        private readonly CacheData CacheData = new CacheData();

        HRMInputs inputs = new HRMInputs();
        public static List<string> reportsToEmployee = new List<string>();
        public ActionResult Index()
        {
            //ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            //GetStatusType();
            //GetPeriod();
            PopulateViewData();
            return View();

        }
        private void PopulateViewData()
        {
            //var EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            //ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            //reportsToEmployee = list.Select(x => x.PersonId.ToString()).ToList();
            //reportsToEmployee.Add(EmployeeId.ToString());

            //var employeName = db.Person_Employee.ToList();
            //ViewData["EmployeeName"] = employeName;

            //ViewData["UserName"] = dbApplication.Users.ToList();
            //ViewData["Department"] = db.MainCompanyStructures.Where(x=>x.IsActive).ToList();
            //var branches = db.Branch;
            //ViewData["branches"] = branches;
            //var businessUnits = db.BusinessUnits;
            //ViewData["businessUnits"] = businessUnits;
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
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
        }
        public double GetWeekDays(DateTime startDate, DateTime endDate, int empId)
        {
            double noOfWeek = 0;
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
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

                        if (saturdayweekendSetting != null)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                var weekendExceptions = db.WeekendExceptions.ToList();
                                if (weekendExceptions.Any())
                                {
                                    var weekendException = db.WeekendExceptions
                                              .FirstOrDefault(x => x.EmployeeId == empId);
                                    if (weekendException == null && placement != null)
                                    {
                                        weekendException = db.WeekendExceptions.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);

                                    }
                                    // Skip because this is an exception for those simple employees that have Saturday as a working day, while other workers have it as an off day.

                                    if (weekendException != null)
                                    {
                                        noOfWeek += saturdayweekendSetting.IsHalfDay ? 0.5 : saturdayweekendSetting.IsFullDay ? 0 : 1;

                                    }
                                    else
                                    {
                                        // Skip because this is an exception for those simple employees that have Saturday as a working day, while other workers have it as an off day.
                                    }
                                }
                                else
                                {
                                    if (saturdayweekendSetting.IsHalfDay)
                                    {
                                        noOfWeek += 0.5;
                                    }
                                    else if (saturdayweekendSetting.IsFullDay)
                                    {
                                        noOfWeek += 0;

                                    }
                                    else
                                    {
                                        noOfWeek += 1;

                                    }
                                }

                            }
                        }
                        break;
                    case DayOfWeek.Sunday:

                        if (sundayweekendSetting != null)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                if (sundayweekendSetting.IsHalfDay)
                                {
                                    noOfWeek += 0.5;
                                }
                                else if (sundayweekendSetting.IsFullDay)
                                {
                                    noOfWeek += 0;
                                }
                                else
                                {
                                    noOfWeek += 1;

                                }
                            }
                        }
                        //dayList.Add(testDate);
                        break;
                }
            }
            return noOfWeek;
        }

        private decimal GetTotalLeaveBalance(int id)
        {
            var leaveEntitlements = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);
            return leaveEntitlements.Any() ? leaveEntitlements.Sum(x => x.Balance) : 0;
        }

        private void GetNotiofied(LeaveRequest leaveRequest)
        {
            NotifModels notif = new NotifModels
            {
                UserId = UserInformationHelper.GetEmployeeUserId(leaveRequest.EmployeeId),
                Title = $"You have a leave Request {leaveRequest.FromAmharic} - {leaveRequest.ToAmharic} for {leaveRequest.LeaveDays}",
                Message = "Leave.",
                URL = "/HRM/LeaveRequest"
            };

            Notify.NotifyUser(notif);
        }
        public ActionResult LeaveRequest_Read([DataSourceRequest] DataSourceRequest request)
        {
            //List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);
            // join LeaveRequest with the latest RequestStatus for each Leave
            var query = from lr in db.LeaveRequest
                        where employeeIds.Contains(lr.EmployeeId)
                              && lr.IsSendForApproval
                        join rsGroup in
                            (
                                from r in db.RequestesStatus
                                where r.ReferenceType == "Leave"
                                group r by r.RequestId into g
                                select new
                                {
                                    RequestId = g.Key,
                                    LatestStatus = g.OrderByDescending(x => x.RequestStatusId).FirstOrDefault()
                                }
                            )
                            on lr.LeaveRequestId equals rsGroup.RequestId into joined
                        from rs in joined.DefaultIfEmpty()   // <-- LEFT JOIN
                        orderby lr.RequestDate descending
                        select new
                        {
                            lr.LeaveRequestId,
                            lr.EmployeeId,
                            lr.BranchId,
                            lr.BusinessUnitId,
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
                            // ✅ handle cases where RequestesStatus is missing
                            IsApproved = rs != null && rs.LatestStatus != null && rs.LatestStatus.Status == RequestStatus.APPROVE,
                            IsChangeInDepartment = rs != null && rs.LatestStatus != null && rs.LatestStatus.Status == RequestStatus.DEPARTMENT_APPROVAL,
                            IsPending = rs != null && rs.LatestStatus != null && rs.LatestStatus.Status == RequestStatus.PEND,
                            IsRejected = rs != null && rs.LatestStatus != null && rs.LatestStatus.Status == RequestStatus.REJECT,
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
                            lr.VoidTime
                        };

            DataSourceResult result = query.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveRequest_ReadExisted([DataSourceRequest]DataSourceRequest request)
        {
            var  employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeids = EmployeeHelper.GetSubordinates_FilteredByHead(employeeid);

            IQueryable<LeaveRequest> leaverequests = db.LeaveRequest.AsNoTracking().Where(x => employeeids.Contains(x.EmployeeId) && x.IsSendForApproval).OrderBy(x => x.RequestDate).AsQueryable();
          

            DataSourceResult result = leaverequests.ToDataSourceResult(request, leaveRequest => new
            {
                LeaveRequestId = leaveRequest.LeaveRequestId,
                EmployeeId = leaveRequest.EmployeeId,
                BranchId = leaveRequest.BranchId,
                BusinessUnitId = leaveRequest.BusinessUnitId,
                From = leaveRequest.From,
                IsForOther = leaveRequest.IsForOther,
                To = leaveRequest.To,
                LeaveDays = leaveRequest.LeaveDays,
                Type = leaveRequest.Type,
                Reason = leaveRequest.Reason,
                GlFiscalYearId = leaveRequest.GlFiscalYearId,
                RequestDate = leaveRequest.RequestDate,
                Remark = leaveRequest.Remark,
                LeaveBalance = leaveRequest.LeaveBalance,
                FromAmharic = leaveRequest.FromAmharic,
                ToAmharic = leaveRequest.ToAmharic,
                Day = leaveRequest.Day,
                MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                Hours = leaveRequest.Hours,
                Status = leaveRequest.Status,
                EmployeeCode = GetEmployeeCode(leaveRequest.EmployeeId),
                IsApproved = CheckStatusForBackColor(leaveRequest.LeaveRequestId),
                IsPending = CheckPendStatusForBackColor(leaveRequest.LeaveRequestId),
                IsRejected = CheckRejectStatusForBackColor(leaveRequest.LeaveRequestId),
                IsChangeInDepartment = ChangeInDepartmentForBackColor(leaveRequest.LeaveRequestId),
                DepartmentSendForApprovalBy = leaveRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leaveRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leaveRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = leaveRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leaveRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leaveRequest.IsDepartmentOnlineApproved,
                IsOnlineApproved = leaveRequest.IsOnlineApproved,
                OnlineApprovedBy = leaveRequest.OnlineApprovedBy,
                OnlineApprovedTime = leaveRequest.OnlineApprovedTime,
                IsVoid = leaveRequest.IsVoid,
                VoidBy = leaveRequest.VoidBy,
                VoidTime = leaveRequest.VoidTime,
                CreatedBy = leaveRequest.CreatedBy,
                DateCreated = leaveRequest.DateCreated,
                ModifiedBy = leaveRequest.ModifiedBy,
                LastModified = leaveRequest.LastModified
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Create([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
           
            #region validation
            leaveRequest.EmployeeId = leaveRequest.IsForOther ? leaveRequest.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (leaveRequest.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select an employee first!! Or refresh the page to clear the cache");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            var totalBalance = db.LeavesEntitle.Where(x => !x.IsExpired
                                                           && x.EmployeeId == leaveRequest.EmployeeId)
                                                .Select(x => x.Balance)
                                                .DefaultIfEmpty(0).Sum(x => x);
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(leaveRequest.FromAmharic)
                || string.IsNullOrEmpty(leaveRequest.ToAmharic))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    leaveRequest.From = HrHelper.ParseEthiopicDate(leaveRequest.FromAmharic);
                    leaveRequest.To = HrHelper.ParseEthiopicDate(leaveRequest.ToAmharic);
                }

            }
            else
            {

                leaveRequest.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.From.Day, leaveRequest.From.Month, leaveRequest.From.Year);
                leaveRequest.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.To.Day, leaveRequest.To.Month, leaveRequest.To.Year);

            }
            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
            {
                ModelState.AddModelError("", "Check your Period if it is defined!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            //leaveRequest.LeaveDays = GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type);
            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                if (leaveRequest.From.Date != leaveRequest.To.Date)
                {
                    leaveRequest.LeaveDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
                }
                else if (leaveRequest.From.Date == leaveRequest.To.Date
                        && leaveRequest?.ToTime > leaveRequest?.FromTime)
                {
                    leaveRequest.LeaveDays = (decimal)(leaveRequest?.ToTime.Value - leaveRequest?.FromTime.Value).Value.TotalHours / 8;
                }
                else if (leaveRequest.From.Date == leaveRequest.To.Date
                        && (leaveRequest.ToTime == null || leaveRequest.FromTime == null)
                        && leaveRequest.Hours == 0)
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
                if (leaveRequest.From.Date == leaveRequest.To.Date
                    && leaveRequest?.ToTime > leaveRequest?.FromTime)
                {
                    leaveRequest.LeaveDays = (decimal)(leaveRequest?.ToTime.Value - leaveRequest?.FromTime.Value).Value.TotalHours / 8;

                }
                else if (leaveRequest.From.Date < leaveRequest.To.Date)
                {
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? (decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1 : ((decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1) / 2;

                }
                else if (leaveRequest.From.Date == leaveRequest.To.Date
                        && (leaveRequest.ToTime == null
                        || leaveRequest.FromTime == null)
                        && leaveRequest.Hours == 0)
                {
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? 1 : 0.5m;
                }
                else
                {
                    leaveRequest.LeaveDays = (decimal)leaveRequest.Hours / 8;

                }
            }


            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave Days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave Days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            
            if (leaveRequest.From > leaveRequest.To)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            #endregion
            #region Validations

            if (db.LeaveRequest.Any(x => x.EmployeeId == leaveRequest.EmployeeId && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) || (x.From <= leaveRequest.To && x.To >= leaveRequest.To))))
            {
                ModelState.AddModelError("", "Your request overlaps with another request");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            if (leaveRequest.LeaveDays > totalBalance && leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You requested more than your balance");
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

                        var maternityRequests = db.LeaveRequest.Where(x => x.Type == LeaveType.MATERNITY && x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId&&x.IsOnlineApproved).ToList();
                        if (maternityRequests.Any() && maternityRequests.Sum(x => x.LeaveDays) + leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", $"You can't take maternity leave more than {allowedDays} days in one year, check your previous maternity leave requests");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                if (employee.HiredDate > fiscalYear.DateFrom)
                {
                    var leaveEntitle = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId&&!x.IsExpired);
                    if(leaveEntitle == null)
                    {
                        ModelState.AddModelError("", "Generate or add Annual Leave for this year");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    
                    }
                    if (leaveEntitle.Balance < leaveRequest.LeaveDays)
                    {
                        
                        ModelState.AddModelError("", $"Your have Balance is {leaveEntitle.Balance}  and Your Leave Request is {leaveRequest.LeaveDays} , please add less");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                    decimal monthsServed = (decimal)((DateTime.Now - fiscalYear.DateFrom).TotalDays / 30);
                    decimal allowedLeaveAmont = monthsServed !=0? leaveEntitle.Balance/monthsServed:0;
                    allowedLeaveAmont = Math.Round(allowedLeaveAmont, 1);
                    if (leaveRequest.LeaveDays > allowedLeaveAmont)
                    {
                        ModelState.AddModelError("", $"Your Allowed  Leave amount is  {allowedLeaveAmont} but  Your Leave Requested is {leaveRequest.LeaveDays} , you can't Add more than Allowed amount");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced)
            {
                if (leaveRequest.Type != LeaveType.ANNUAL && leaveRequest.Type != LeaveType.OTHER && leaveRequest.Type != LeaveType.Leave_With_Out_Pay)
                {
                    ModelState.AddModelError("", $"You can't take Forced Leave for {leaveRequest.Type}, it's allowed only for Annual, Leave without pay, and Other leave types");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }
            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                var leavePlan = db.LeavePlans
                       .FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId 
                                    && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (leavePlan != null 
                    && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    if (leaveRequest.From < leavePlan.From 
                        || leaveRequest.To > leavePlan.To 
                        || leaveRequest.LeaveDays > leavePlan.LeaveAmount)
                    {
                        ModelState.AddModelError("", "The requested leave amount or dates are outside of your plan");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
                else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    ModelState.AddModelError("", "You should set a leave plan for this period!!");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }
            var existingLeave = db.LeaveRequest
                .FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId 
                                && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (existingLeave != null)
            {
                if ((leaveRequest.From >= existingLeave.From 
                    && leaveRequest.From <= existingLeave.To) 
                    || (leaveRequest.To >= existingLeave.From
                    && leaveRequest.To <= existingLeave.To))
                {
                    ModelState.AddModelError("", "Selected date range overlaps with other leave taken date. Please select another date range");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            var entitleCurrentYear = db.LeavesEntitle
                .FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId 
                && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (entitleCurrentYear != null&& leaveRequest.Type == LeaveType.ANNUAL)
            {
                var totalEntitle = totalBalance;
                var exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalEntitle);
                var currentYearDeserve = GetCurrentYearDeserve(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount);

                if (exactDeserve == 0 || leaveRequest.LeaveDays > exactDeserve)
                {
                    ModelState.AddModelError("", $"You can't take more than {exactDeserve} amount of leave in this period");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                var leaveRequests = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId 
                             && x.GlFiscalYearId == fiscalYear.GlFiscalYearId 
                             && x.Type == LeaveType.ANNUAL);
                if (leaveRequests.Any())
                {
                    var leaveRequestedDays = leaveRequests.Sum(x => x.LeaveDays);
                    var totalNeeded = leaveRequestedDays + leaveRequest.LeaveDays;
                    if (totalNeeded > exactDeserve)
                    {
                        ModelState.AddModelError("", $"You have other waiting requests so the sum of days ({totalNeeded}) exceeds your deserved amount {exactDeserve} days");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You have no entitlement for this budget year");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.isCurrent == true).FirstOrDefault();

            if (placement != null)
            {
                leaveRequest.BranchId = placement.BranchId;
                leaveRequest.BusinessUnitId = placement.BusinessUnitId;
            }
            else
            {
                ModelState.AddModelError("placement", "Employee does not have a current placement");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == leaveRequest.Type);
            if (leaveTypeSetting != null)
            {
                if (leaveTypeSetting.BlockBackdatedLeave&&leaveTypeSetting.MaxDaysBack.HasValue)
                {
                    // Oldest allowed date
                    var limitDate = DateTime.Today.AddDays(-leaveTypeSetting.MaxDaysBack.Value);

                    if (leaveRequest.From < limitDate)
                    {
                        string errorMessage = $"You cannot request earlier than {limitDate:MMMM dd, yyyy} " +
                                       $"(max {leaveTypeSetting.MaxDaysBack.Value} days back from today).";
                        ModelState.AddModelError("block", $" {errorMessage} ");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

                    }
                }
                else if (leaveTypeSetting.BlockBackdatedLeave && !leaveTypeSetting.MaxDaysBack.HasValue && leaveRequest.From.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("block", " You cannot request leave for a past date. Please adjust your leave Type Setting or contact HR ");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                if (leaveTypeSetting.LeaveType != LeaveType.ANNUAL
                    || leaveTypeSetting.LeaveType != LeaveType.SICK_LEAVE)
                {
                    if (leaveTypeSetting.IsWorkingDays)
                    {
                        leaveRequest.LeaveDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);

                    }
                    else if (leaveTypeSetting.IsConsecutiveDays)
                    {
                        leaveRequest.LeaveDays = Convert.ToDecimal((leaveRequest.To.Date - leaveRequest.From.Date).Days + 1);
                    }
                    var totalleaveRequetPerYear = db.LeaveRequest.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId
                                        && x.EmployeeId == leaveRequest.EmployeeId
                                        && x.Type == leaveTypeSetting.LeaveType);
                    decimal totalleaveRequetedAmount = (totalleaveRequetPerYear.Any() ? totalleaveRequetPerYear.Sum(x => x.LeaveDays) : 0) + leaveRequest.LeaveDays;
                    if (totalleaveRequetedAmount > leaveTypeSetting.MaximumAmount)
                    {
                        ModelState.AddModelError("placement",
                            $"The requested leave of {totalleaveRequetedAmount} days exceeds the maximum allowed amount of {leaveTypeSetting.MaximumAmount} days for {leaveTypeSetting.LeaveType} leave in the fiscal year {fiscalYear.Name}.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

                    }

                }
            }
            if (leaveRequest.Type == LeaveType.Leave_With_Out_Pay && totalBalance > 0)
            {
                ModelState.AddModelError("", "You can't  request Leave with out pay  having annual leave  use your leave first");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            #endregion
            leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;

            if (ModelState.IsValid)
            {


                var entity = new LeaveRequest
                {
                    EmployeeId = leaveRequest.EmployeeId,
                    From = leaveRequest.From,
                    To = leaveRequest.To,
                    LeaveDays = leaveRequest.LeaveDays,
                    Type = leaveRequest.Type,
                    Reason = leaveRequest.Reason,
                    Remark = leaveRequest.Remark,
                    GlFiscalYearId = leaveRequest.GlFiscalYearId,
                    RequestDate = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = leaveRequest.ModifiedBy,
                    LastModified = leaveRequest.LastModified,
                    LeaveBalance = totalBalance,
                    FromAmharic = leaveRequest.FromAmharic,
                    ToAmharic = leaveRequest.ToAmharic,
                    Day = leaveRequest.Day,
                    MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                    Hours = leaveRequest.Hours,
                    IsForOther = leaveRequest.IsForOther,
                    IsSendForApproval = true,
                    SendForApprovalBy = User.Identity.Name,
                    SendForApprovalTime = DateTime.Now,
                    Status = RequestStatus.WAITING_FOR_APPROVAL.ToString(),
                    IsDepartmentSendForApproval = true,
                    DepartmentSendForApprovalBy = User.Identity.Name,
                    DepartmentSendForApprovalTime = DateTime.Now,
                    BranchId = leaveRequest.BranchId,
                    BusinessUnitId = leaveRequest.BusinessUnitId,
                    EmployeeCode = inputs.GetEmployeeCode(leaveRequest.EmployeeId)


                };

                db.LeaveRequest.Add(entity);
                db.SaveChanges();
                leaveRequest.LeaveRequestId = entity.LeaveRequestId;
                var requestStatus = new RequestesStatus
                {
                    RequestId = entity.LeaveRequestId,
                    ApproverDepartment = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name),
                    ApproverId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                    ReferenceType = "Leave",
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };
                db.RequestesStatus.Add(requestStatus);
                db.SaveChanges();
                GetNotiofied(leaveRequest);

            }
            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Update([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {

            ModelState.Clear();
            
            var leavrequest = db.LeaveRequest.Find(leaveRequest.LeaveRequestId);
            #region Pre Conditions for validation

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(leaveRequest.FromAmharic)
                || string.IsNullOrEmpty(leaveRequest.ToAmharic))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    leaveRequest.From = HrHelper.ParseEthiopicDate(leaveRequest.FromAmharic);
                    leaveRequest.To = HrHelper.ParseEthiopicDate(leaveRequest.ToAmharic);
                }

            }
            else
            {
                leaveRequest.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.From.Day, leaveRequest.From.Month, leaveRequest.From.Year);
                leaveRequest.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.To.Day, leaveRequest.To.Month, leaveRequest.To.Year);
            }
            if (leaveRequest.From > leaveRequest.To)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }

          
            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
            {
                ModelState.AddModelError("", "Check your Period if it is defined!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

        
            if (leaveRequest.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select an employee first!! Or refresh the page to clear the cache");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            #endregion

            #region Validations

            var totalBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId)
                                                              .Select(x => x.Balance)
                                                              .DefaultIfEmpty(0)
                                                              .Sum(x => x);
            if (leaveRequest.LeaveDays > totalBalance && leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You requested more than your balance");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            if (db.LeaveRequest.Any(x =>x.LeaveRequestId !=leaveRequest.LeaveRequestId 
                                &&x.EmployeeId == leaveRequest.EmployeeId 
                                && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From)
                                || (x.From <= leaveRequest.To && x.To >= leaveRequest.To))))
            {
                ModelState.AddModelError("", "Your request overlaps with another request");
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

                        var maternityRequests = db.LeaveRequest.Where(x => x.Type == LeaveType.MATERNITY && x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId).ToList();
                        if (maternityRequests.Any() && maternityRequests.Sum(x => x.LeaveDays) + leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", $"You can't take maternity leave more than {allowedDays} days in one year, check your previous maternity leave requests");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
            }

            if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced)
            {
                if (leaveRequest.Type != LeaveType.ANNUAL && leaveRequest.Type != LeaveType.OTHER && leaveRequest.Type != LeaveType.Leave_With_Out_Pay)
                {
                    ModelState.AddModelError("", $"You can't take Forced Leave for {leaveRequest.Type}, it's allowed only for Annual, Leave without pay, and Other leave types");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                var leavePlan = db.LeavePlans.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (leavePlan != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    if (leaveRequest.From < leavePlan.From || leaveRequest.To > leavePlan.To || leaveRequest.LeaveDays > leavePlan.LeaveAmount)
                    {
                        ModelState.AddModelError("", "The requested leave amount or dates are outside of your plan");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
                else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    ModelState.AddModelError("", "You should set a leave plan for this period!!");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            if (leavrequest != null
               && leaveRequest.From.Date == leavrequest.From.Date
               && leaveRequest.To.Date == leavrequest.To.Date
               && leaveRequest.Day == leavrequest.Day)
            {
                leaveRequest.LeaveDays = leavrequest.LeaveDays;
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
            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var existingLeave = db.Leaves.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (existingLeave != null)
            {
                if ((leaveRequest.From >= existingLeave.From 
                    && leaveRequest.From <= existingLeave.To) 
                    || (leaveRequest.To >= existingLeave.From 
                    && leaveRequest.To <= existingLeave.To))
                {
                    ModelState.AddModelError("", "Selected date range overlaps with other leave taken date. Please select another date range");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            var entitleCurrentYear = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (entitleCurrentYear != null && leaveRequest.Type == LeaveType.ANNUAL)
            {
                var totalEntitle = totalBalance;
                var exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalEntitle);
                var currentYearDeserve = GetCurrentYearDeserve(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount);

                if (exactDeserve == 0 || leaveRequest.LeaveDays > exactDeserve)
                {
                    ModelState.AddModelError("", "You can't take this amount of leave in this period");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                var leaveRequests = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId && x.Type == LeaveType.ANNUAL && !x.IsOnlineApproved);
                if (leaveRequests.Any())
                {
                    var leaveRequestedDays = leaveRequests.Sum(x => x.LeaveDays);
                    var totalNeeded = leaveRequestedDays + leaveRequest.LeaveDays;
                    if (totalNeeded > exactDeserve)
                    {
                        ModelState.AddModelError("", "You have other waiting requests so the sum of days exceeds your deserved amount");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You have no entitlement for this budget year");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.isCurrent == true).FirstOrDefault();

            if (placement != null)
            {
                leaveRequest.BranchId = placement.BranchId;
                leaveRequest.BusinessUnitId = placement.BusinessUnitId;
            }
            else
            {
                ModelState.AddModelError("placement", "Employee does not have a current placement");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == leaveRequest.Type);
            if (leaveTypeSetting != null)
            {
                if (leaveTypeSetting.MaxDaysBack.HasValue && leaveTypeSetting.BlockBackdatedLeave)
                {
                    // Oldest allowed date
                    var limitDate = DateTime.Today.AddDays(-leaveTypeSetting.MaxDaysBack.Value);

                    if (leaveRequest.From < limitDate)
                    {
                        string errorMessage = $"You cannot request earlier than {limitDate:MMMM dd, yyyy} " +
                                       $"(max {leaveTypeSetting.MaxDaysBack.Value} days back from today).";
                        ModelState.AddModelError("block", $" {errorMessage} ");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

                    }
                }
                else if (leaveTypeSetting.BlockBackdatedLeave&& !leaveTypeSetting.MaxDaysBack.HasValue && leaveRequest.From.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("block", " You cannot request leave for a past date. Please adjust your leave Type Setting or contact HR ");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                if (leaveTypeSetting.LeaveType != LeaveType.ANNUAL
                    || leaveTypeSetting.LeaveType != LeaveType.SICK_LEAVE)
                {
                    if (leaveTypeSetting.IsWorkingDays)
                    {
                        leaveRequest.LeaveDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);

                    }
                    else if (leaveTypeSetting.IsConsecutiveDays)
                    {
                        leaveRequest.LeaveDays = Convert.ToDecimal((leaveRequest.To.Date - leaveRequest.From.Date).Days + 1);
                    }
                    var totalleaveRequetPerYear = db.LeaveRequest
                                        .Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId
                                                && x.LeaveRequestId != leaveRequest.LeaveRequestId
                                                && x.EmployeeId == leaveRequest.EmployeeId
                                                && x.Type == leaveTypeSetting.LeaveType);
                    decimal totalleaveRequetedAmount = (totalleaveRequetPerYear.Any() ? totalleaveRequetPerYear.Sum(x => x.LeaveDays) : 0) + leaveRequest.LeaveDays;
                    if (totalleaveRequetedAmount > leaveTypeSetting.MaximumAmount)
                    {
                        ModelState.AddModelError("placement",
                            $"The requested leave of {totalleaveRequetedAmount} days exceeds the maximum allowed amount of {leaveTypeSetting.MaximumAmount} days for {leaveTypeSetting.LeaveType} leave in the fiscal year {fiscalYear.Name}.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

                    }

                }
            }

            if (leaveRequest.Type == LeaveType.Leave_With_Out_Pay && totalBalance > 0)
            {
                ModelState.AddModelError("", "You can't  request Leave with out pay  having annual leave  use your leave first");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }

            #endregion
            if (ModelState.IsValid)
            {
                leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;

                if(leavrequest != null)
                {
                     leavrequest.EmployeeId = leaveRequest.EmployeeId;
                     leavrequest.BranchId = leaveRequest.BranchId;
                     leavrequest.BusinessUnitId = leaveRequest.BusinessUnitId;
                     leavrequest.From = leaveRequest.From;
                     leavrequest.To = leaveRequest.To;
                     leavrequest.LeaveDays = leaveRequest.LeaveDays;
                     leavrequest.Type = leaveRequest.Type;
                     leavrequest.Reason = leaveRequest.Reason;
                     leavrequest.GlFiscalYearId = leaveRequest.GlFiscalYearId;
                     leavrequest.Remark = leaveRequest.Remark;
                     leavrequest.RequestDate = leaveRequest.RequestDate;
                     leavrequest.DateCreated = leaveRequest.DateCreated;
                     leavrequest.CreatedBy = leaveRequest.CreatedBy;
                     leavrequest.LastModified = DateTime.Now;
                     leavrequest.ModifiedBy = User.Identity.Name;
                     leavrequest.LeaveBalance = totalBalance;
                     leavrequest.Day = leaveRequest.Day;
                     leavrequest.MorningOrAfternoon = leaveRequest.MorningOrAfternoon;
                     leavrequest.Hours = leaveRequest.Hours;
                     leavrequest.FromAmharic = leaveRequest.FromAmharic;
                     leavrequest.ToAmharic = leaveRequest.ToAmharic;
                     leavrequest.IsForOther = leaveRequest.IsForOther;
                     leavrequest.LeaveActionTypes = leaveRequest.LeaveActionTypes;
                     leavrequest.EmployeeCode = inputs.GetEmployeeCode(leaveRequest.EmployeeId);
                     leavrequest.RequesterId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                     db.Entry(leavrequest).State = EntityState.Modified;
                     db.SaveChanges();
                    GetNotiofied(leavrequest);
                }

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "LeaveRequestPersonal ", "Success", User.Identity.Name, User.Identity.Name + " update leave request of id" + leaveRequest.LeaveRequestId, Modules.HRM);

                #endregion
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }
        #region comment_out
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeaveRequest_Create([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {
        //        var period = db.GLFiscalYears.FirstOrDefault(x => x.Status == FiscalYearStatus.Open);
        //        leaveRequest.LeaveDays = Convert.ToDecimal((To.Date - From.Date).TotalDays);
        //        if (!IsForOther)
        //        {
        //            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //        }
        //        else
        //        {
        //            EmployeeId = leaveRequest.EmployeeId;
        //        }

        //        if (leaveRequest.Type == LeaveType.ANNUAL)
        //        {
        //            string messages = "";
        //            if (EmployeeId == 0)
        //            {
        //                ModelState.AddModelError("", "Please select employee First!!");
        //                //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }


        //            var leavePlans =
        //                db.LeavePlans
        //                    .FirstOrDefault(x => x.EmployeeId == EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId);
        //            if (leavePlans != null)
        //            {
        //                if (From.Date >= leavePlans.From.Date && To.Date <= leavePlans.To.Date)
        //                {
        //                    leaveRequest.LeaveDays = Convert.ToDecimal((To.Date - From.Date).TotalDays);
        //                    if (leaveRequest.LeaveDays > leavePlans.LeaveAmount)
        //                    {
        //                       ModelState.AddModelError("", "The requested amount is greater than your plan!!");
        //                        //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //                    }

        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Please check selected period leave plan date range!!");
        //                    //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //                }

        //                //RedirectToAction("Index");

        //            }
        //            else
        //            {
        //                //RedirectToAction("Index");
        //               ModelState.AddModelError("", "You should set leave plan for this period!!");
        //                //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }

        //        }
        //        if (ModelState.IsValid)
        //        {
        //            if (From.Date > To.Date)
        //            {
        //                ModelState.AddModelError("", "Please Check the date range!!");
        //            }
        //            else if (FromAmharic == null || ToAmharic == null)
        //            {
        //                ModelState.AddModelError("", "Form and To date value and is mandatory!!");
        //            }


        //            else if (EmployeeId == 0)
        //            {
        //                ModelState.AddModelError("", "Please select employee First!!");
        //                // return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }
        //            else if (leaveRequest.LeaveDays == 0)
        //            {
        //                ModelState.AddModelError("", "Leave Days must be greater than 0!!");
        //                // return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }
        //            if (leaveRequest.EmployeeId != 0)
        //            {
        //                var placemnt = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
        //                if (placemnt != null)
        //                {
        //                    leaveRequest.BranchId = placemnt.BranchId;
        //                    leaveRequest.BusinessUnitId = placemnt.BusinessUnitId;
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("placement", "Employee has not current placemnt");
        //                }
        //            }
        //            if (ModelState.IsValid)
        //            {


        //                var entity = new LeaveRequest
        //                {
        //                    EmployeeId = EmployeeId,
        //                    From = From,
        //                    To = To,
        //                    LeaveDays = leaveRequest.LeaveDays,
        //                    Type = leaveRequest.Type,
        //                    Reason = leaveRequest.Reason,
        //                    Remark = leaveRequest.Remark,
        //                    GlFiscalYearId = period.GlFiscalYearId,//leaveRequest.GlFiscalYearId,
        //                    RequestDate = DateTime.Now,
        //                    LeaveBalance = Total,
        //                    FromAmharic = FromAmharic,
        //                    ToAmharic = ToAmharic,
        //                    Day = leaveRequest.Day,
        //                    MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
        //                    Hours = leaveRequest.Hours,
        //                    IsForOther = leaveRequest.IsForOther,
        //                    BranchId =leaveRequest.BranchId,
        //                    BusinessUnitId = leaveRequest.BusinessUnitId,
        //                    EmployeeCode =
        //                    inputs.GetEmployeeCode(EmployeeId)
        //                };

        //                db.LeaveRequest.Add(entity);
        //                db.SaveChanges();
        //                leaveRequest.LeaveRequestId = entity.LeaveRequestId;
        //                IsForOther = false;
        //                //#region notification


        //                //var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //                //var superviorId = EmployeeHelper.GetSupervisor(empid);

        //                //NotifModels notif = new NotifModels
        //                //{

        //                //    UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
        //                //    Title = "Leave Request",
        //                //    Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Leave",
        //                //    URL = "~/HRM/LeaveRequest"
        //                //};

        //                //Notify.NotifyUser(notif);

        //                //#endregion
        //            }
        //        }
        //    }

        //    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        //}
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeaveRequest_Update([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        //{
        //    int count = 0;
        //    double weekDays = 0;
        //    int holidays = 0;
        //    ModelState.Clear();
        //    if (leaveRequest.IsOnlineApproved)
        //    {
        //        ModelState.AddModelError("", "you can't modify approved requestes");
        //    }


        //    #region Pre Requestes 
        //    if (leaveRequest.FromAmharic != null)
        //    {
        //        var splitDate = leaveRequest.FromAmharic.Split('/');
        //        leaveRequest.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "From Date is mandatory");
        //    }
        //    if (leaveRequest.ToAmharic != null)
        //    {
        //        var splitDate = leaveRequest.ToAmharic.Split('/');
        //        leaveRequest.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "To Date is mandatory");
        //    }

        //    var per = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From.Date && x.DateTo >= leaveRequest.From.Date);
        //    if (per == null)
        //    {
        //        var messages = "Check your Period if it defined";
        //        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //    }

        //    leaveRequest.LeaveDays = GetLeaveDays(leaveRequest.From, leaveRequest.To, per.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type);

        //    #endregion

        //    decimal entitleBal = 0;
        //    var entitleBalan = db.LeavesEntitle.Where(x => x.EmployeeId == leaveRequest.EmployeeId && !x.IsExpired);

        //    if (entitleBalan.Any())
        //    {
        //        entitleBal = entitleBalan.Sum(x => x.Balance);
        //    }
        //    if (leaveRequest.LeaveDays > entitleBal && leaveRequest.Type == LeaveType.ANNUAL)
        //    {
        //        ModelState.AddModelError("", "you requested morethan your balance");
        //    }
        //    var leaveReqPrev = db.LeaveRequest.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.LeaveRequestId != leaveRequest.LeaveRequestId && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) || (x.From <= leaveRequest.To && x.To >= leaveRequest.To)));
        //    if (leaveReqPrev != null)
        //    {
        //        ModelState.AddModelError("", "Your request is overlap with other request");
        //    }
        //    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId);
        //    if (emp != null && leaveRequest.Type == LeaveType.MATERNITY && emp.Gender == Gender.MALE)
        //    {
        //        ModelState.AddModelError("", "Male can't take Maternity Leave ");
        //    }
        //    if (leaveRequest.Type == LeaveType.MATERNITY)
        //    {
        //        var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == LeaveHrmSetting.Maternitiy.ToString());
        //        if (config != null)
        //        {
        //            var allowedDays = Convert.ToDecimal(config.CurrentValue);
        //            if (leaveRequest.LeaveDays > allowedDays)
        //            {
        //                ModelState.AddModelError("", "You are requesting maternity leave above the setting value");
        //            }
        //            var maternityRequests = db.LeaveRequest.Where(x => x.Type == LeaveType.MATERNITY && x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId && x.LeaveRequestId != leaveRequest.LeaveRequestId).ToList();
        //            if (maternityRequests.Any())
        //            {
        //                var totalAskedMaternity = maternityRequests.Sum(x => x.LeaveDays) + leaveRequest.LeaveDays;
        //                if (totalAskedMaternity > allowedDays)
        //                {
        //                    ModelState.AddModelError("", "You can't take materinit leave more than " + allowedDays + " in one year, check your previous maternity leave request");
        //                }
        //            }
        //        }
        //    }
        //    if (emp != null && leaveRequest.Type == LeaveType.PATERNITY && emp.Gender == Gender.FEMALE)
        //    {
        //        ModelState.AddModelError("", "Female can't take Paternity Leave ");
        //    }
        //    if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced)
        //    {
        //        if (leaveRequest.Type != LeaveType.ANNUAL && leaveRequest.Type != LeaveType.OTHER && leaveRequest.Type != LeaveType.Leave_With_Out_Pay)
        //        {
        //            ModelState.AddModelError("", "You can't take Forced Leave for " + leaveRequest.Type + " it's allowed for only Annual,Leave without pay and Other leave types");
        //        }
        //    }


        //    if (leaveRequest.Type == LeaveType.ANNUAL)
        //    {

        //        var leavePlans = db.LeavePlans.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId);
        //        if (leavePlans != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
        //        {
        //            if (leaveRequest.From.Date >= leavePlans.From.Date && leaveRequest.To.Date <= leavePlans.To.Date)
        //            {

        //                if (leaveRequest.LeaveDays > leavePlans.LeaveAmount)
        //                {
        //                    ModelState.AddModelError("", "The requested amount is greater than your plan!!");
        //                }


        //            }
        //            else if (leaveRequest.From.Date < leavePlans.From.Date || leaveRequest.To.Date > leavePlans.To.Date)
        //            {
        //                ModelState.AddModelError("", "Date you select is out of your Plan date range!!");
        //            }

        //        }
        //        else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
        //        {
        //            ModelState.AddModelError("", "You should set leave plan for this period!!");
        //        }

        //    }


        //    if (leaveRequest.From.Date > leaveRequest.To.Date)
        //    {
        //        ModelState.AddModelError("", "Date from should less or equal to date to!!");
        //    }
        //    else if (leaveRequest.LeaveDays == 0)
        //    {
        //        ModelState.AddModelError("", "Leave Days must be greater than 0!!");
        //    }


        //    var leave = db.Leaves.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId);
        //    if (leave != null)
        //    {
        //        if (leaveRequest.From.Date >= leave.From && leaveRequest.From.Date <= leave.To)
        //        {
        //            ModelState.AddModelError("", "Intial date overlap other leave taken date. please select other date range");
        //        }
        //        if (leaveRequest.To.Date >= leave.From && leaveRequest.To.Date <= leave.To)
        //        {
        //            ModelState.AddModelError("", "End date over lapother leave taken date. please select other date range");
        //        }
        //    }

        //    var entitelCurrentYear = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId);

        //    if (entitelCurrentYear != null && leaveRequest.Type == LeaveType.ANNUAL)
        //    {
        //        var workedDays = (leaveRequest.To - per.DateFrom.Date).Days;

        //        var totalEntitle = entitleBalan.Sum(x => x.Balance);

        //        var exactDeserve = GetEmployeeDeserveLeaveDays(per.DateFrom, leaveRequest.To, entitelCurrentYear.Amount, entitelCurrentYear.Balance, totalEntitle);

        //        var currentYearDeserve = GetCurrentYearDeserve(per.DateFrom, leaveRequest.To, entitelCurrentYear.Amount);
        //        var currentYearBalance = entitelCurrentYear.Balance;
        //        var currentYearEntitle = entitelCurrentYear.Amount;

        //        var previousLeftBalance = GetPreviosYearLeftBalance(entitelCurrentYear.Balance, totalEntitle);

        //        if (previousLeftBalance == 0 && leaveRequest.LeaveDays > currentYearDeserve && leaveRequest.Type == LeaveType.ANNUAL)
        //        {
        //            ModelState.AddModelError("", "you can't take this amount of leave  in this period ");
        //        }

        //        if (leaveRequest.LeaveDays > previousLeftBalance && leaveRequest.Type == LeaveType.ANNUAL)
        //        {
        //            var neededFromCurrentYear = leaveRequest.LeaveDays - previousLeftBalance;
        //            var takenFromCurrentYear = currentYearEntitle - currentYearBalance;
        //            var needed = takenFromCurrentYear + neededFromCurrentYear;
        //            if (neededFromCurrentYear > currentYearDeserve)
        //            {
        //                ModelState.AddModelError("", "you request exceed your deserve amount");
        //            }
        //            if (needed > currentYearDeserve)
        //            {
        //                ModelState.AddModelError("", "you request exceed your left deserve amount");
        //            }
        //            var leaveRequests = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId && x.Type == LeaveType.ANNUAL && !x.IsOnlineApproved && x.LeaveRequestId != leaveRequest.LeaveRequestId);
        //            if (leaveRequests.Any())
        //            {
        //                var leaveRequestedDays = leaveRequests.Sum(x => x.LeaveDays);
        //                var sumofRequest = leaveRequestedDays + leaveRequest.LeaveDays;

        //                if (sumofRequest > previousLeftBalance)
        //                {
        //                    var neededFromNow = sumofRequest - previousLeftBalance;
        //                    var totalNeeded = neededFromNow + takenFromCurrentYear;
        //                    if (neededFromNow > currentYearDeserve)
        //                    {
        //                        ModelState.AddModelError("", "you have other waiting requests so sum of days exceed your deserve amount");
        //                    }
        //                    if (totalNeeded > currentYearDeserve)
        //                    {
        //                        ModelState.AddModelError("", "you have other waiting requests so sum of days exceed your left deserve amount");
        //                    }
        //                }
        //            }
        //        }

        //        //}               

        //    }
        //    else if (leaveRequest.Type == LeaveType.ANNUAL)
        //    {
        //        ModelState.AddModelError("", "You have no entitlement for this budget year");
        //    }
        //    if (leaveRequest.EmployeeId != 0)
        //    {
        //        var placemnt = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
        //        if (placemnt != null)
        //        {
        //            leaveRequest.BranchId = placemnt.BranchId;
        //            leaveRequest.BusinessUnitId = placemnt.BusinessUnitId;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("placement", "Employee has not current placemnt");
        //        }
        //    }
        //    if (leaveRequest.EmployeeId != 0)
        //    {
        //        var placemnt = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
        //        if (placemnt != null)
        //        {
        //            leaveRequest.BranchId = placemnt.BranchId;
        //            leaveRequest.BusinessUnitId = placemnt.BusinessUnitId;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("placement", "Employee has not current placemnt");
        //        }
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        var totalBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId).Sum(x => x.Balance);
        //        leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;


        //        var entity = new LeaveRequest
        //        {
        //            LeaveRequestId = leaveRequest.LeaveRequestId,
        //            EmployeeId = leaveRequest.EmployeeId,
        //            BranchId = leaveRequest.BranchId,
        //            BusinessUnitId = leaveRequest.BusinessUnitId,
        //            From = leaveRequest.From,
        //            To = leaveRequest.To,
        //            LeaveDays = leaveRequest.LeaveDays,
        //            Type = leaveRequest.Type,
        //            Reason = leaveRequest.Reason,
        //            GlFiscalYearId = leaveRequest.GlFiscalYearId,
        //            Remark = leaveRequest.Remark,
        //            RequestDate = DateTime.Now,
        //            LeaveBalance = totalBalance,
        //            Day = leaveRequest.Day,
        //            MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
        //            Hours = leaveRequest.Hours,
        //            FromAmharic = leaveRequest.FromAmharic,
        //            ToAmharic = leaveRequest.ToAmharic,
        //            IsForOther = leaveRequest.IsForOther,
        //            LeaveActionTypes = leaveRequest.LeaveActionTypes,
        //            EmployeeCode = inputs.GetEmployeeCode(leaveRequest.EmployeeId),
        //            RequesterId = UserInformationHelper.GetEmployeeId(User.Identity.Name)
        //        };


        //        db.LeaveRequest.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        IsForOther = false;

        //        #region operation log

        //        UserHelper.OperationLog(Request.UserHostAddress, "LeaveRequestPersonal ", "Success", User.Identity.Name, User.Identity.Name + " update leave request of id" + entity.LeaveRequestId, Modules.HRM);

        //        #endregion
        //    }

        //    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        //}
        #endregion
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Destroy([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
            if (leaveRequest.IsOnlineApproved)
            {
                ModelState.AddModelError("", "you can't delete approved requestes");
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
                        From = leaveRequest.From,
                        To = leaveRequest.To,
                        LeaveDays = leaveRequest.LeaveDays,
                        Type = leaveRequest.Type,
                        Reason = leaveRequest.Reason,
                        GlFiscalYearId = leaveRequest.GlFiscalYearId,
                        Remark = leaveRequest.Remark,
                        LeaveBalance = leaveRequest.LeaveBalance,
                        FromAmharic = leaveRequest.FromAmharic,
                        ToAmharic = leaveRequest.ToAmharic,
                        EmployeeCode = leaveRequest.EmployeeCode,
                        IsForOther = leaveRequest.IsForOther,
                        Day = leaveRequest.Day,
                        MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                        Hours = leaveRequest.Hours
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
        public int SetEmployee(string x)
        {

            if (x == "true")
            {
                var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                //var list = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeId).ToList();
                var list = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId).ToList();

                return list.Count();

            }
            return 0;
        }
        private JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json((periodList ?? throw new InvalidOperationException()).Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }
        public ActionResult SendForApproval()
        {
            var status =
                      db.RequestesStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();
            string messages = "";
            if (status.Any())
            {
                foreach (var s in status)
                {
                    if (s.Status == RequestStatus.APPROVE)
                    {
                        messages = "The Request is already approved!!";
                        //  return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                        ModelState.AddModelError("xx", messages);
                        return null;
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        // return  ModelState.AddModelError("xx", messages );
                        return null;//new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }

                }
            }
            RequestesStatus requestesStatus;
            db = new ExceedDbContext();
            requestesStatus =
                db.RequestesStatus.Where(
                    x =>
                        x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL).FirstOrDefault();
            db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }
            else
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
                    Remark = requestesStatus.Remark
                };

                db.RequestesStatus.Attach(entity);
                db.RequestesStatus.Remove(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public bool CheckStatusForBackColor(int leaveId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == leaveId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Leave").FirstOrDefault();
            if (reques != null)
            {
                return true;
            }
            return false;
        }
        public bool CheckPendStatusForBackColor(int leaveId)
        {
            var status = db.RequestesStatus.Where(x => x.RequestId == leaveId
                                                    && x.ReferenceType == "Leave")
                                                    .OrderByDescending(x => x.RequestStatusId)
                                                    .FirstOrDefault();
            if (status != null && status.Status == RequestStatus.PEND)
            {


                return true;
            }
            return false;
        }
        public bool CheckRejectStatusForBackColor(int leaveId)
        {
            var status = db.RequestesStatus.Where(x => x.RequestId == leaveId
                                                    && x.ReferenceType == "Leave")
                                                    .OrderByDescending(x => x.RequestStatusId)
                                                    .FirstOrDefault();
            if (status != null && status.Status == RequestStatus.REJECT)
            {


                return true;
            }
            return false;
        }
        public bool ChangeInDepartmentForBackColor(int leaveId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == leaveId && x.Status != RequestStatus.APPROVE && (x.Status == RequestStatus.DEPARTMENT_APPROVAL || x.Status == RequestStatus.PEND || x.Status == RequestStatus.REJECT) && x.ReferenceType == "Leave").FirstOrDefault();
            if (reques != null)
            {
                return true;
            }
            return false;
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
        public string CheckStatus(int masterId)
        {
            var requestStatus = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave").OrderByDescending(x=>x.RequestStatusId).FirstOrDefault();
            if (requestStatus != null)
            {
                if (requestStatus.Status== RequestStatus.APPROVE)
                {
                    return "approved";
                }
                else if (requestStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                {
                    return "Dapproved";
                }
                else if (requestStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                {
                    return "waiting";
                }
                else if (requestStatus.Status == RequestStatus.PEND)
                {
                    return "pend";
                }
                else if (requestStatus.Status == RequestStatus.REJECT)
                {
                    return "reject";
                }
               
                return "false";
            }
            return "false";
        }
        public string CheckStatusexisted(int masterId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave").OrderByDescending(x=>x.RequestStatusId).FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "false";
            }
            return "false";
        }
        public double GetWeekDays(DateTime startDate, DateTime endDate)
        {
            double noOfWeek = 0;
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

                        if (saturdayweekendSetting != null)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                if (saturdayweekendSetting.IsHalfDay)
                                {
                                    noOfWeek += 0.5;
                                }
                                else
                                {
                                    noOfWeek += 1;

                                }
                            }
                        }
                        break;
                    case DayOfWeek.Sunday:

                        if (sundayweekendSetting != null)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                if (sundayweekendSetting.IsHalfDay)
                                {
                                    noOfWeek += 0.5;
                                }
                                else
                                {
                                    noOfWeek += 1;

                                }
                            }
                        }
                        //dayList.Add(testDate);
                        break;
                }
            }
            return noOfWeek;
        }
        public decimal GetLeaveDays(string from, string to, string half, int hour)
        {

            decimal days = 0;

            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {


                var splitDatefrom = from.Split('/');
                DateTime f = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDatefrom[0]), Convert.ToInt32(splitDatefrom[1]), Convert.ToInt32(splitDatefrom[2]));

                var splitDateto = to.Split('/');
                DateTime t = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateto[0]), Convert.ToInt32(splitDateto[1]), Convert.ToInt32(splitDateto[2]));
                var count = 0;
                double weekDays = 0;
                var year = db.GLFiscalYears.FirstOrDefault(x => x.Status == FiscalYearStatus.Open);
                if (year != null)
                {
                    var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == year.GlFiscalYearId).ToList();
                    if (holidayList.Any())
                    {
                        foreach (var h in holidayList)
                        {
                            if (h.Date >= f && h.Date <= t)
                            {
                                count++;
                            }
                        }
                    }
                    weekDays = GetWeekDays(f, t);
                }




                days = t.Subtract(f).Days + 1 - count;
                days = days - Convert.ToDecimal(weekDays);
                if (half == "1")
                {
                    decimal checkWeekEnd = t.Subtract(f).Days + 1 - count;
                    double amt = HrHelper.GetHalf(f, t);
                    double notHalf = HrHelper.GetNotHalf(f, t);
                    bool isHalf = HrHelper.IsHalf(f, t);
                    if (isHalf)
                    {
                        days = (checkWeekEnd - Convert.ToDecimal(notHalf)) / 2;
                    }
                    else
                    {
                        days = days / 2;
                    }

                }

                decimal totalhours = days * 8;
                decimal somehours = days * hour;
                days = (totalhours - somehours) / 8;
            }
            return days;
        }
        public decimal GetBalance(/*int period,*/ /*int empId*/)
        {
            decimal Total = 0;
            //if (empId == 0)
            //{
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //}


            var entitles = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired/*&& x.GlFiscalYearId == period*/);
            if (entitles.Any())
            {
                //var takenLeave = db.Leaves.Where(x => x.EmployeeId == empId && x.GlFiscalYearId == period).ToList();
                //var tranferedList = db.LeaveTransfer.Where(x => x.To == period && x.EmployeeId == empId && x.Check).ToList();
                //var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period && x.EmployeeId == empId).ToList();
                //decimal sumoftaken = takenLeave.Sum(x => x.Amount);
                //decimal sumoftransfer = tranferedList.Sum(x => x.Amount);
                //decimal sumofEntitle = entitles.Amount;
                //decimal sumofpaid = Convert.ToDecimal(paidIncash.Sum(x => x.Amount));
                //decimal subtract = sumoftaken + sumofpaid;

                //Total = (sumofEntitle + sumoftransfer) - subtract;

                Total = entitles.Sum(x => x.Balance);
            }
            return Total;
        }
        public decimal GetLeaveDays(DateTime from, DateTime to, int glFiscalYearId, LeaveDay day, LeaveType leaveType)
        {
            decimal leaveDays = 0;

            decimal holiday = HrHelper.GetHolidays(from, to, glFiscalYearId);
            var weekDays = GetWeekDays(from, to);
            if (leaveType == LeaveType.ANNUAL)
            {
                leaveDays = to.Subtract(from).Days + 1 - holiday;
                leaveDays = leaveDays - Convert.ToDecimal(weekDays);
                if (day == LeaveDay.Half_Day)
                {
                    decimal checkWeekEnd = to.Subtract(from).Days + 1 - holiday;
                    double amt = HrHelper.GetHalf(from, to);
                    double notHalf = HrHelper.GetNotHalf(from, to);
                    bool isHalf = HrHelper.IsHalf(from, to);
                    if (isHalf)
                    {
                        leaveDays = (checkWeekEnd - Convert.ToDecimal(notHalf)) / 2;
                    }
                    else
                    {
                        leaveDays = leaveDays / 2;
                    }
                }

            }
            else
            {
                leaveDays = to.Subtract(from).Days + 1;
                if (day == LeaveDay.Half_Day)
                {
                    leaveDays = leaveDays / 2;
                }
            }

            return leaveDays;
        }
        public decimal GetEmployeeDeserveLeaveDays(DateTime from, DateTime to, decimal amount, decimal balance, decimal totalbalance)
        {
            decimal exactDeserve = 0;

            var workedDays = (to.Date - from.Date).Days;

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

            return exactDeserve;
        }
        public decimal GetCurrentYearDeserve(DateTime from, DateTime to, decimal amount)
        {

            var workedDays = (to.Date - from.Date).Days;

            var deserveAmountCurrentYear = (amount * workedDays) / 365;

            return deserveAmountCurrentYear;
        }
        public decimal GetPreviosYearLeftBalance(decimal balance, decimal totalbalance)
        {
            var leftBalanceFromPrevious = totalbalance - balance;

            return leftBalanceFromPrevious;
        }
        public string GetLeaveBalance(int empId)
        {
            //SalaryJobTitle salary = new SalaryJobTitle();

            if (empId == 0)
            {
                empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }
            var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired);

            if (entitle.Any())
            {
                var balance = entitle.Sum(x => x.Amount).ToString();

                return balance;
            }

            return "0";
        }
        public decimal GetBalance(int period, int empId)
        {
            decimal Total = 0;
            if (empId == 0)
            {
                empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }


            var entitles = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == empId && x.GlFiscalYearId == period);
            //if (entitles != null)
            //{
            //    var takenLeave = db.Leaves.Where(x => x.EmployeeId == empId && x.GlFiscalYearId == period).ToList();
            //    var tranferedList = db.LeaveTransfer.Where(x => x.To == period && x.EmployeeId == empId && x.Check).ToList();
            //    var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period && x.EmployeeId == empId).ToList();
            //    decimal sumoftaken = takenLeave.Sum(x => x.Amount);
            //    decimal sumoftransfer = tranferedList.Sum(x => x.Amount);
            //    decimal sumofEntitle = entitles.Amount;
            //    decimal sumofpaid = Convert.ToDecimal(paidIncash.Sum(x => x.Amount));
            //    decimal subtract = sumoftaken + sumofpaid;

            //    Total = (sumofEntitle + sumoftransfer) - subtract;
            //}
            return Total;
        }
        public decimal GetAmount(int period, int empId)
        {
            var balance = db.LeavesEntitle.Where(x=>x.EmployeeId == empId && !x.IsExpired);
            decimal totalBalance = 0;
            if(balance.Any())
            {
              
                totalBalance = balance.Sum(x => x.Amount);
            }
            return totalBalance;
        }
        public decimal GetCurrentEmpBalance(int period)
        {

            var user = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var balance = db.LeavesEntitle.Where(x => x.EmployeeId == user && !x.IsExpired);
            decimal totalBalance = 0;
            if (balance != null)
            {
                totalBalance = balance.Sum(x => x.Amount);
            }
            return totalBalance;

        }
        //public decimal GetHolidays(DateTime from, DateTime to, int glFiscalYearId)
        //{
        //    decimal count = 0;

        //    var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == glFiscalYearId).ToList();
        //    if (holidayList.Any())
        //    {
        //        foreach (var h in holidayList)
        //        {
        //            if (h.Date >= from && h.Date <= to)
        //            {
        //                count++;
        //            }
        //        }
        //    }
        //    return count;
        //}
        //public double GetHalf(DateTime startDate, DateTime endDate)
        //{
        //    double noOfWeek = 0;
        //    var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
        //    var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
        //    List<DateTime> dayList = new List<DateTime>();
        //    TimeSpan diff = endDate - startDate;
        //    int days = diff.Days;
        //    for (var i = 0; i <= days; i++)
        //    {

        //        var testDate = startDate.AddDays(i);
        //        switch (testDate.DayOfWeek)
        //        {
        //            case DayOfWeek.Saturday:

        //                if (saturdayweekendSetting != null && saturdayweekendSetting.IsHalfDay)
        //                {
        //                    var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
        //                    if (holiday == null)
        //                    {
        //                        noOfWeek = noOfWeek + 1;
        //                    }
        //                }
        //                break;
        //            case DayOfWeek.Sunday:


        //                if (sundayweekendSetting != null && sundayweekendSetting.IsHalfDay)
        //                {
        //                    var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
        //                    if (holiday == null)
        //                    {
        //                        noOfWeek = noOfWeek + 1;
        //                    }
        //                }

        //                break;
        //        }
        //    }
        //    return noOfWeek;
        //}
        //public double GetNotHalf(DateTime startDate, DateTime endDate)
        //{
        //    double noOfWeek = 0;
        //    var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
        //    var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
        //    List<DateTime> dayList = new List<DateTime>();
        //    TimeSpan diff = endDate - startDate;
        //    int days = diff.Days;
        //    for (var i = 0; i <= days; i++)
        //    {

        //        var testDate = startDate.AddDays(i);
        //        switch (testDate.DayOfWeek)
        //        {
        //            case DayOfWeek.Saturday:

        //                if (saturdayweekendSetting != null && !saturdayweekendSetting.IsHalfDay)
        //                {
        //                    var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
        //                    if (holiday == null)
        //                    {
        //                        noOfWeek = noOfWeek + 1;
        //                    }

        //                }

        //                break;
        //            case DayOfWeek.Sunday:


        //                if (sundayweekendSetting != null && !sundayweekendSetting.IsHalfDay)
        //                {
        //                    var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
        //                    if (holiday == null)
        //                    {
        //                        noOfWeek = noOfWeek + 1;
        //                    }
        //                }


        //                break;
        //        }
        //    }
        //    return noOfWeek;
        //}
        //public bool IsHalf(DateTime startDate, DateTime endDate)
        //{
        //    bool isHalf = false;
        //    var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
        //    var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
        //    List<DateTime> dayList = new List<DateTime>();
        //    TimeSpan diff = endDate - startDate;
        //    int days = diff.Days;
        //    for (var i = 0; i <= days; i++)
        //    {

        //        var testDate = startDate.AddDays(i);
        //        switch (testDate.DayOfWeek)
        //        {
        //            case DayOfWeek.Saturday:

        //                if (saturdayweekendSetting != null && saturdayweekendSetting.IsHalfDay)
        //                {
        //                    var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
        //                    if (holiday == null)
        //                    {
        //                        isHalf = true;
        //                    }

        //                }

        //                break;
        //            case DayOfWeek.Sunday:


        //                if (sundayweekendSetting != null && sundayweekendSetting.IsHalfDay)
        //                {
        //                    var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
        //                    if (holiday == null)
        //                    {
        //                        isHalf = true;
        //                    }
        //                }


        //                break;
        //        }
        //    }
        //    return isHalf;
        //}
        public ActionResult UploadDocument(int leaveRequestId, HttpPostedFileBase file, string txtValue)
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
