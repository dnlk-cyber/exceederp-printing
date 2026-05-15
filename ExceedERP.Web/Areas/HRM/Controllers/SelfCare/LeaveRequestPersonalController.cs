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

        public static List<string> reportsToEmployee = new List<string>();
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
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
        }

        //[HttpPost]
        //public JsonResult SetEmployee(bool x)
        //{
        //    if (x)
        //    {
        //        var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //        return Json(employeeId, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(0, JsonRequestBehavior.AllowGet);
        //}

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

            //bool hasOverlappingRequest = db.LeaveRequest.Any(x =>
            //                                  x.EmployeeId == leaveRequest.EmployeeId
            //                                  &&!x.IsDepartmentOnlineApproved
            //                                  &&!x.IsOnlineApproved
            //                                  && x.LeaveRequestId != leaveRequest.LeaveRequestId
            //                                  && x.From <= leaveRequest.To
            //                                  && x.To >= leaveRequest.From);
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
               
                //ModelState.AddModelError("", "Your request overlaps with another existing leave request.");
                //return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
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
       
        private bool CheckLatestStatus(int leaveId, RequestStatus expectedStatus)
        {
            var latestStatus = db.RequestesStatus
                .Where(x => x.RequestId == leaveId && x.ReferenceType == "Leave")
                .OrderByDescending(x => x.RequestStatusId)
                .Select(x => x.Status)
                .FirstOrDefault();

            return latestStatus == expectedStatus;
        }

        public bool CheckPendStatusForBackColor(int leaveId) =>
            CheckLatestStatus(leaveId, RequestStatus.PEND);

        public bool CheckRejectStatusForBackColor(int leaveId) =>
            CheckLatestStatus(leaveId, RequestStatus.REJECT);

        public bool CheckWaitStatusForBackColor(int leaveId) =>
            CheckLatestStatus(leaveId, RequestStatus.WAITING_FOR_APPROVAL);
        public bool CheckStatusForBackColor(int leaveId) =>
            CheckLatestStatus(leaveId, RequestStatus.APPROVE);
       
        public ActionResult RefreshPreviousLeaveData()
        {
            var leaveRequests = db.LeaveRequest.ToList();
            foreach (var leave in leaveRequests)
            {
                if (leave.Type != LeaveType.ANNUAL|| leave.Type != LeaveType.DayOff)
                {
                    var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leave.LeaveRequestId && x.Status == RequestStatus.APPROVE);
                    if (requestStatus != null)
                    {
                        var leaveTaken = db.Leaves.FirstOrDefault(x => x.RequestId == leave.LeaveRequestId);

                        if (leaveTaken == null)
                        {

                            int count = 0;
                            double weekDays = 0;
                            var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == leave.GlFiscalYearId).ToList();
                            if (holidayList.Any())
                            {
                                foreach (var h in holidayList)
                                {
                                    if (h.Date >= leave.From && h.Date <= leave.To)
                                    {
                                        count++;
                                    }
                                }
                            }
                            weekDays = GetWeekDays(leave.From, leave.To,leave.EmployeeId);
                            var entity = new Core.Domain.HRM.LeaveManagment.Leave
                            {
                                EmployeeId = leave.EmployeeId,
                                LeaveType = leave.Type,
                                Amount = leave.LeaveDays,
                                GlFiscalYearId = leave.GlFiscalYearId,
                                From = leave.From,
                                To = leave.To,
                                FromAmharic = leave.FromAmharic,
                                ToAmharic = leave.ToAmharic,
                                OrgName = null,
                                Before = null,
                                After = null,
                                Remark = leave.Remark,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                EmployeeCode = leave.Employee.EmployeeCode,
                                ModifiedBy = null,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                RequestId = leave.LeaveRequestId
                            };

                            db.Leaves.Add(entity);
                            db.SaveChanges();

                        }
                    }
                }
            }
            return RedirectToAction("Index");
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
      
        private decimal GetTotalLeaveEntitleBalance(int id)
        {
            var leaveEntitlements = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);
            return leaveEntitlements.Any() ? leaveEntitlements.Sum(x => x.Balance) : 0;
        }

        public ActionResult LeaveRequestPersonal_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);
            // join LeaveRequest with the latest RequestStatus for each Leave
            var query = from lr in db.LeaveRequest
                        where ((employeeIds.Contains(lr.EmployeeId)&&lr.IsForOther)||lr.EmployeeId== employeeId)
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
                            lr.CreatedBy,
                            lr.LastModified,
                            lr.DateCreated,
                            lr.VoidTime
                        };

            DataSourceResult result = query.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveRequestPersonal_Read_Existed([DataSourceRequest]DataSourceRequest request)
        {
            var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(employeeid);

            IQueryable<LeaveRequest> leaverequest = db.LeaveRequest.Where(x => x.EmployeeId == employeeid||(x.IsForOther&& employeeIds.Contains(x.EmployeeId))).OrderBy(x => x.RequestDate);

            DataSourceResult result = leaverequest.ToDataSourceResult(request, leaveRequest => new
            {
                LeaveRequestId = leaveRequest.LeaveRequestId,
                EmployeeId = leaveRequest.EmployeeId,
                BranchId = leaveRequest.BranchId,
                BusinessUnitId = leaveRequest.BusinessUnitId,
                LookupId = leaveRequest.LookupId,
                GlFiscalYearId = leaveRequest.GlFiscalYearId,
                From = leaveRequest.From,
                IsForOther = leaveRequest.IsForOther,
                To = leaveRequest.To,
                FromTime = leaveRequest.FromTime,
                ToTime = leaveRequest.ToTime,
                LeaveDays = leaveRequest.LeaveDays,
                ExactDeserveLeaveDays = leaveRequest.ExactDeserveLeaveDays,
                Type = leaveRequest.Type,
                Reason = leaveRequest.Reason,
                RequestDate = leaveRequest.RequestDate,
                Remark = leaveRequest.Remark,
                //LeaveBalance = leaveRequest.LeaveBalance,
                FromAmharic = leaveRequest.FromAmharic,
                ToAmharic = leaveRequest.ToAmharic,
                EmployeeCode = GetEmployeeCode(leaveRequest.EmployeeId),
                Day = leaveRequest.Day,
                LeaveBalance = GetTotalLeaveEntitleBalance(leaveRequest.EmployeeId),
                MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                IsApproved = CheckStatusForBackColor(leaveRequest.LeaveRequestId),
                IsPending = CheckPendStatusForBackColor(leaveRequest.LeaveRequestId),
                IsRejected = CheckRejectStatusForBackColor(leaveRequest.LeaveRequestId),
                IsWaiting = CheckWaitStatusForBackColor(leaveRequest.LeaveRequestId),
                Hours = leaveRequest.Hours,
                RequesterId = leaveRequest.RequesterId,
                LeaveActionTypes = leaveRequest.LeaveActionTypes,
                Status = leaveRequest.Status,
                IsSendForApproval = leaveRequest.IsSendForApproval,
                DepartmentSendForApprovalBy = leaveRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leaveRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leaveRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = leaveRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leaveRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leaveRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = leaveRequest.OnlineApprovedBy,
                OnlineApprovedTime = leaveRequest.OnlineApprovedTime,
                IsOnlineApproved = leaveRequest.IsOnlineApproved,
                IsVoid = leaveRequest.IsVoid,
                VoidBy = leaveRequest.VoidBy,
                CreatedBy = leaveRequest.CreatedBy,
                DateCreated = leaveRequest.DateCreated,
                ModifiedBy = leaveRequest.ModifiedBy,
                LastModified = leaveRequest.LastModified,
                VoidTime = leaveRequest.VoidTime,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequestPersonal_Create([DataSourceRequest] DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
            leaveRequest.EmployeeId = leaveRequest.IsForOther ? leaveRequest.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (leaveRequest.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select an employee first!! Or refresh the page to clear the cache");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            var totalBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId)
                                               .Select(x => x.Balance).DefaultIfEmpty(0).Sum(x => x);
            #region Pre Conditions for validation


            bool useEthiopian = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;

            if (useEthiopian)
            {
                // User inputs Ethiopian dates → must validate
                if (string.IsNullOrWhiteSpace(leaveRequest.FromAmharic) ||
                    string.IsNullOrWhiteSpace(leaveRequest.ToAmharic))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                // Convert Ethiopic → Gregorian
                leaveRequest.From = HrHelper.ParseEthiopicDate(leaveRequest.FromAmharic);
                leaveRequest.To = HrHelper.ParseEthiopicDate(leaveRequest.ToAmharic);
            }
            else
            {
                // User inputs Gregorian dates → must validate
                if (leaveRequest.From == default(DateTime) || leaveRequest.To == default(DateTime))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                // Convert Gregorian → Ethiopic for storage
                leaveRequest.FromAmharic = XAPI.EthiopicDateTime
                    .GetEthiopicDate(leaveRequest.From.Day, leaveRequest.From.Month, leaveRequest.From.Year);

                leaveRequest.ToAmharic = XAPI.EthiopicDateTime
                    .GetEthiopicDate(leaveRequest.To.Day, leaveRequest.To.Month, leaveRequest.To.Year);
            }

            var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == leaveRequest.Type);
            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
            {
                ModelState.AddModelError("", "Check your Period if it is defined!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            leaveRequest.GlFiscalYearId = fiscalYear.GlFiscalYearId;
            var existingLeave = db.LeaveRequest.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId 
                                                             && x.GlFiscalYearId == fiscalYear.GlFiscalYearId 
                                                             && !x.IsVoid);

            if (leaveRequest.From.Date == leaveRequest.To.Date)
            {
                
                if (existingLeave != null && leaveRequest.To.Date == leaveRequest.From.Date)
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
                if (db.LeaveRequest
               .Any(x => !x.IsVoid && x.EmployeeId == leaveRequest.EmployeeId
               && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From)
               || (x.From <= leaveRequest.To && x.To >= leaveRequest.To))))
                {
                    ModelState.AddModelError("", "Your request overlaps with another request");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

                }

               
           
            }

            if (leaveRequest.Type == LeaveType.Leave_With_Out_Pay 
                && totalBalance > 0)
            {
                ModelState.AddModelError("", "You can't  request Leave with out pay  having annual leave  use your leave first");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            if (leaveRequest.LeaveDays > totalBalance  && leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", $"You requested {leaveRequest.LeaveDays} more than your balance {totalBalance}");
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

                        var maternityRequests = db.LeaveRequest
                                                   .Where(x => x.Type == LeaveType.MATERNITY 
                                                   && x.EmployeeId == leaveRequest.EmployeeId 
                                                   && x.GlFiscalYearId == fiscalYear.GlFiscalYearId 
                                                   && x.IsOnlineApproved).ToList();
                        if (maternityRequests.Any() 
                            && maternityRequests.Sum(x => x.LeaveDays) + leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", $"You can't take maternity leave more than {allowedDays} days in one year, check your previous maternity leave requests");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
                if(leaveTypeSetting !=null 
                   && !leaveTypeSetting.AllowFullAnnualLeave 
                   && leaveTypeSetting.LeaveType == LeaveType.ANNUAL)
                {
                    if ((employee.HiredDate < fiscalYear.DateFrom || employee.HiredDate < fiscalYear.DateTo)
                             && leaveRequest.From.Date >= fiscalYear.DateFrom.Date)
                    {
                        var leaveEntitle = db.LeavesEntitle
                            .FirstOrDefault(x => x.EmployeeId == employee.EmployeeId
                            && x.GlFiscalYearId == fiscalYear.GlFiscalYearId
                            && !x.IsExpired);
                        var leaveEntitleExpired = db.LeavesEntitle
                            .FirstOrDefault(x => x.EmployeeId == employee.EmployeeId
                            && x.GlFiscalYearId == fiscalYear.GlFiscalYearId
                            && x.IsExpired);

                        if (leaveEntitle == null && leaveRequest.Type == LeaveType.ANNUAL)
                        {
                            string message = leaveEntitleExpired != null
                                ? "You have used all your annual leave balance."
                                : "Please generate or assign annual leave for this year.";

                            ModelState.AddModelError("", message);
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }


                        if (leaveEntitle != null
                            && leaveRequest.Type == LeaveType.ANNUAL
                            && totalBalance < leaveRequest.LeaveDays)
                        {

                            ModelState.AddModelError("", $"Your have Balance is {totalBalance}  and Your Leave Request is {leaveRequest.LeaveDays} , please add less");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                        DateTime today = DateTime.Now;
                        decimal monthsServed = Math.Abs((decimal)(today - fiscalYear.DateFrom).TotalDays / 30);

                        decimal allowedLeaveAmount = monthsServed > 0 ? Math.Round(totalBalance / monthsServed, 1) : 0;

                        bool isAnnualLeave = leaveRequest.Type == LeaveType.ANNUAL;
                        bool isNotWithoutPay = leaveRequest.Type != LeaveType.Leave_With_Out_Pay;

                        //if (leaveRequest.LeaveDays > allowedLeaveAmount && isAnnualLeave && isNotWithoutPay)
                        //{
                        //    ModelState.AddModelError(
                        //        "",
                        //        $"Your allowed leave amount is {allowedLeaveAmount}, but your leave request is for {leaveRequest.LeaveDays}. You can't request more than the allowed amount."
                        //    );
                        //    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        //}

                    }
                    else
                    {
                        ModelState.AddModelError("", $"You can't ask  leave request  for this fiscal year  {fiscalYear.Name} b/c it ends {fiscalYear.DateTo.ToString("dd:MM:yyyy")} and you hired after this date which is {employee.HiredDate.ToString("dd:MM:yyyy")}");
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
            //existed
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
            //new validation
            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                // Attempt to find the employee's leave plan for the given fiscal year
                var leavePlan = db.LeavePlans.FirstOrDefault(x =>
                    x.EmployeeId == leaveRequest.EmployeeId &&
                    x.GlFiscalYearId == fiscalYear.GlFiscalYearId &&
                    (leaveRequest.From >= x.From || leaveRequest.To <= x.To));

                // Check if the leave action is 'With_Plan' and a leave plan exists
                if (leavePlan != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    // 1. Check if the requested 'From' date is before the planned 'From' date
                    if (leaveRequest.From < leavePlan.From)
                    {
                        ModelState.AddModelError("", "The start date of your leave is before the start date of your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }

                    // 2. Check if the requested 'To' date is after the planned 'To' date
                    if (leaveRequest.To > leavePlan.To)
                    {
                        ModelState.AddModelError("", "The end date of your leave is after the end date of your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }

                    // 3. Check if the requested number of leave days exceeds the planned amount
                    if (leaveRequest.LeaveDays > leavePlan.LeaveAmount)
                    {
                        ModelState.AddModelError("", "The requested number of leave days exceeds the amount allocated in your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
                // If the leave action is 'With_Plan' but no leave plan is found for the period
                else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    ModelState.AddModelError("", "You must set a leave plan for this period.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                // If the leave action is 'Without_Plan', it would proceed without these checks
            }

            

            #endregion

            #region Validations
            if (leaveRequest.From.Date > leaveRequest.To.Date)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
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
                else if(leaveRequest.From.Date == leaveRequest.To.Date
                        && (leaveRequest.ToTime==null || leaveRequest.FromTime==null)
                        &&leaveRequest.Hours==0)
                {

                    leaveRequest.LeaveDays = leaveRequest.Day==LeaveDay.Full_Day? 1:0.5m;
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
                    leaveRequest.LeaveDays = leaveRequest.Day ==LeaveDay.Full_Day?(decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1:((decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1)/2;

                }
                else if (leaveRequest.From.Date == leaveRequest.To.Date
                        && (leaveRequest.ToTime == null 
                        || leaveRequest.FromTime == null)
                        && leaveRequest.Hours == 0)
                {
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day? 1:0.5m;
                }
                else
                {
                    leaveRequest.LeaveDays = (decimal)leaveRequest.Hours / 8;

                }
            }
          
            var entitleCurrentYear = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (entitleCurrentYear != null 
                && leaveRequest.Type == LeaveType.ANNUAL)
            {
                if(leaveTypeSetting !=null && !leaveTypeSetting.AllowFullAnnualLeave)
                {
                    var totalEntitle = totalBalance;
                    var exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalEntitle);
                    var currentYearDeserve = GetCurrentYearDeserve(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount);

                    if (exactDeserve == 0 || leaveRequest.LeaveDays > Math.Abs(exactDeserve))
                    {
                        ModelState.AddModelError("", $"You can't take this amount ({leaveRequest.LeaveDays}) of leave in this period at most {exactDeserve}");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }

                    var leaveRequests = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId
                                                                    && x.GlFiscalYearId == fiscalYear.GlFiscalYearId
                                                                    && x.Type == LeaveType.ANNUAL
                                                                    && !x.IsOnlineApproved && !x.IsVoid);
                    if (leaveRequests.Any())
                    {
                        var leaveRequestedDays = leaveRequests.Sum(x => x.LeaveDays);
                        var totalNeeded = leaveRequestedDays + leaveRequest.LeaveDays;
                        if (totalNeeded > Math.Abs(exactDeserve))
                        {
                            ModelState.AddModelError("", $"You have other waiting requests {totalNeeded} so the sum of days exceeds your deserved amount {exactDeserve}");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
              
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You have no entitlement for this budget year");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements
                .Where(x => x.EmployeeId == leaveRequest.EmployeeId 
                           && x.isCurrent).FirstOrDefault();
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
                if (leaveTypeSetting.BlockBackdatedLeave && !leaveTypeSetting.MaxDaysBack.HasValue && leaveRequest.From.Date < DateTime.Now.Date)
                {
                    ModelState.AddModelError("block", " You cannot request leave for a past date. Please adjust your leave Type Setting or contact HR ");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                if (leaveTypeSetting.LeaveType != LeaveType.ANNUAL
                    && leaveTypeSetting.LeaveType != LeaveType.SICK_LEAVE)
                {
                    if (leaveTypeSetting.IsWorkingDays)
                    {
                        leaveRequest.LeaveDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);

                    }
                    else if (leaveTypeSetting.IsConsecutiveDays)
                    {
                        leaveRequest.LeaveDays = Convert.ToDecimal((leaveRequest.To.Date - leaveRequest.From.Date).Days + 1);
                    }

                    var totalLeaveRequestPerYear = db.LeaveRequest
                        .Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId
                                 && x.EmployeeId == leaveRequest.EmployeeId
                                 && x.Type == leaveTypeSetting.LeaveType);

                    decimal totalRequested = (totalLeaveRequestPerYear.Any()
                                              ? totalLeaveRequestPerYear.Sum(x => x.LeaveDays)
                                              : 0) + leaveRequest.LeaveDays;

                    if (totalRequested > leaveTypeSetting.MaximumAmount)
                    {
                        ModelState.AddModelError("placement",
                            $"Total requested leave of {totalRequested} days exceeds the maximum allowed amount of {leaveTypeSetting.MaximumAmount} days for {leaveTypeSetting.LeaveType} leave in the fiscal year {fiscalYear.Name}.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

                    }
                }
                
            }


            #endregion
            leaveRequest.From = leaveRequest.From;
            leaveRequest.To = leaveRequest.To;
            leaveRequest.FromAmharic = leaveRequest.FromAmharic;
            leaveRequest.ToAmharic = leaveRequest.ToAmharic;
            decimal afternoonOffDays = 0;
            if(leaveRequest.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
            {
                afternoonOffDays = IsHalfDays(leaveRequest.From, leaveRequest.To);
                 
            }
            if (afternoonOffDays > 0)
            {
                leaveRequest.LeaveDays -= afternoonOffDays;
            }
            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave Days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

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
                    EmployeeCode = employee?.EmployeeCode/*inputs.GetEmployeeCode(leaveRequest.EmployeeId)*/,
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
        public ActionResult LeaveRequestPersonal_Update([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
           

            var leavrequest = db.LeaveRequest.Find(leaveRequest.LeaveRequestId);
            leaveRequest.EmployeeId = leaveRequest.IsForOther ? leaveRequest.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);
            if (leaveRequest.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select an employee first!! Or refresh the page to clear the cache");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            var totalBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId)
                                                  .Select(x => x.Balance)
                                                  .DefaultIfEmpty(0)
                                                  .Sum(x => x);
            #region Pre Conditions for validation

            bool useEthiopian = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (useEthiopian)
            {
                // User inputs Ethiopian dates → must validate
                if (string.IsNullOrWhiteSpace(leaveRequest.FromAmharic) ||
                    string.IsNullOrWhiteSpace(leaveRequest.ToAmharic))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                // Convert Ethiopic → Gregorian
                leaveRequest.From = HrHelper.ParseEthiopicDate(leaveRequest.FromAmharic);
                leaveRequest.To = HrHelper.ParseEthiopicDate(leaveRequest.ToAmharic);
            }
            else
            {
                // User inputs Gregorian dates → must validate
                if (leaveRequest.From == default(DateTime) || leaveRequest.To == default(DateTime))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                // Convert Gregorian → Ethiopic for storage
                leaveRequest.FromAmharic = XAPI.EthiopicDateTime
                    .GetEthiopicDate(leaveRequest.From.Day, leaveRequest.From.Month, leaveRequest.From.Year);

                leaveRequest.ToAmharic = XAPI.EthiopicDateTime
                    .GetEthiopicDate(leaveRequest.To.Day, leaveRequest.To.Month, leaveRequest.To.Year);
            }

            if (leaveRequest.From.Date > leaveRequest.To.Date)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
           

            var fiscalYear = db.GLFiscalYears
                .FirstOrDefault(x => x.DateFrom <= leaveRequest.From 
                && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
            {
                ModelState.AddModelError("",
                    "The selected leave date does not fall within any defined fiscal year. Please verify that the fiscal period is properly configured.");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == leaveRequest.Type);

            if (leavrequest != null 
                && leaveRequest.From.Date == leavrequest.From.Date
                && leaveRequest.To.Date == leavrequest.To.Date
                &&leaveRequest.Day == leavrequest.Day)
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
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day? 1 : 0.5m;
                }
                else
                {
                    leaveRequest.LeaveDays = (decimal)leaveRequest.Hours / 8;
                }
            }
            else
            {
                leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? (decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1: ((decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1)/2;
            }
            leaveRequest.LeaveDays = Math.Abs(leaveRequest.LeaveDays);
            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }


            #endregion

            #region Validations
            if (leaveRequest.Type == LeaveType.Leave_With_Out_Pay 
                && totalBalance > 0)
            {
                ModelState.AddModelError("", $"You can't  request Leave with out pay  having annual leave Amount = {totalBalance}  use your leave first");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            if (leaveRequest.LeaveDays > totalBalance 
                && leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You requested more than your balance");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            if (db.LeaveRequest.Any(x =>!x.IsVoid 
                                     &&x.EmployeeId == leaveRequest.EmployeeId 
                                     && x.LeaveRequestId != leavrequest.LeaveRequestId 
                                     &&( (x.From <= leaveRequest.From && x.To >= leaveRequest.From) ||
                                        (x.From <= leaveRequest.To && x.To >= leaveRequest.To)
                                    )))
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

            var allowedTypes = new[] { LeaveType.ANNUAL, LeaveType.OTHER, LeaveType.Leave_With_Out_Pay };

            if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced &&
                !allowedTypes.Contains(leaveRequest.Type))
            {
                ModelState.AddModelError(
                    "Type",
                    $"Forced Leave is only allowed for: {string.Join(", ", allowedTypes)}. You selected: {leaveRequest.Type}."
                );

                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }


            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                // Attempt to find the employee's leave plan for the given fiscal year
                var leavePlan = db.LeavePlans.FirstOrDefault(x =>
                    x.EmployeeId == leaveRequest.EmployeeId &&
                    x.GlFiscalYearId == fiscalYear.GlFiscalYearId &&
                    (leaveRequest.From >= x.From || leaveRequest.To <= x.To));

                // Check if the leave action is 'With_Plan' and a leave plan exists
                if (leavePlan != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    // 1. Check if the requested 'From' date is before the planned 'From' date
                    if (leaveRequest.From < leavePlan.From)
                    {
                        ModelState.AddModelError("", "The start date of your leave is before the start date of your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }

                    // 2. Check if the requested 'To' date is after the planned 'To' date
                    if (leaveRequest.To > leavePlan.To)
                    {
                        ModelState.AddModelError("", "The end date of your leave is after the end date of your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }

                    // 3. Check if the requested number of leave days exceeds the planned amount
                    if (leaveRequest.LeaveDays > leavePlan.LeaveAmount)
                    {
                        ModelState.AddModelError("", "The requested number of leave days exceeds the amount allocated in your leave plan.");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
                // If the leave action is 'With_Plan' but no leave plan is found for the period
                else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    ModelState.AddModelError("", "You must set a leave plan for this period.");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
                // If the leave action is 'Without_Plan', it would proceed without these checks
            }

            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                var leavePlan = db.LeavePlans
                    .FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId 
                    && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (leavePlan != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
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

            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave Days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var existingLeave = db.LeaveRequest.FirstOrDefault(x => !x.IsVoid
                                                                && x.EmployeeId == leaveRequest.EmployeeId 
                                                                && x.LeaveRequestId != leaveRequest.LeaveRequestId 
                                                                && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (existingLeave != null)
            {
                if ((leaveRequest.From >= existingLeave.From && leaveRequest.From <= existingLeave.To) 
                    || (leaveRequest.To >= existingLeave.From && leaveRequest.To <= existingLeave.To))
                {
                    ModelState.AddModelError("", "Selected date range overlaps with other leave taken date. Please select another date range");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }
            var entitleCurrentYear = db.LeavesEntitle
                                .FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId 
                                                  && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (entitleCurrentYear != null && leaveRequest.Type == LeaveType.ANNUAL)
            {
                if (leaveTypeSetting != null && !leaveTypeSetting.AllowFullAnnualLeave)
                {
                    var totalEntitle = totalBalance;
                    var exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalEntitle);
                    var currentYearDeserve = GetCurrentYearDeserve(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount);

                    if (exactDeserve == 0 || leaveRequest.LeaveDays > Math.Abs(exactDeserve))
                    {
                        ModelState.AddModelError("", $"You can't take this amount ({leaveRequest.LeaveDays}) of leave in this period at most {exactDeserve}");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }

                    var leaveRequests = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId
                                                                    && x.GlFiscalYearId == fiscalYear.GlFiscalYearId
                                                                    && x.Type == LeaveType.ANNUAL
                                                                    && !x.IsOnlineApproved && !x.IsVoid);
                    if (leaveRequests.Any())
                    {
                        var leaveRequestedDays = leaveRequests.Sum(x => x.LeaveDays);
                        var totalNeeded = leaveRequestedDays + leaveRequest.LeaveDays;
                        if (totalNeeded > Math.Abs(exactDeserve))
                        {
                            ModelState.AddModelError("", $"You have other waiting requests {totalNeeded} so the sum of days exceeds your deserved amount {exactDeserve}");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }

            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You have no entitlement for this budget year");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var placement = db.Placements
                .Where(x => x.EmployeeId == leaveRequest.EmployeeId 
                && x.isCurrent).FirstOrDefault();

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
                if (leaveTypeSetting.MaxDaysBack.HasValue&& leaveTypeSetting.BlockBackdatedLeave)
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
                if (leaveTypeSetting.BlockBackdatedLeave&& !leaveTypeSetting.MaxDaysBack.HasValue && leaveRequest.From.Date < DateTime.Now.Date)
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
                    leaveRequest.LeaveDays = Math.Abs(leaveRequest.LeaveDays);
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
            decimal afternoonOffDays = 0;
            if (leaveRequest.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
            {
                afternoonOffDays = IsHalfDays(leaveRequest.From, leaveRequest.To);

            }
            if (afternoonOffDays > 0)
            {
                leaveRequest.LeaveDays -= afternoonOffDays;
            }
            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            #endregion
            if (leavrequest !=null)
            {
                
                leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;
                leavrequest.EmployeeId = leaveRequest.EmployeeId;
                leavrequest.BusinessUnitId = leaveRequest.BusinessUnitId;
                leavrequest.BranchId = leaveRequest.BranchId;
                leavrequest.LookupId = leaveRequest.LookupId;
                leavrequest.From = leaveRequest.From;
                leavrequest.To = leaveRequest.To;
                leavrequest.FromTime = leaveRequest.FromTime;
                leavrequest.ToTime = leaveRequest.ToTime;
                leavrequest.LeaveDays = leaveRequest.LeaveDays;
                leavrequest.ExactDeserveLeaveDays = leaveRequest.ExactDeserveLeaveDays;
                leavrequest.Type = leaveRequest.Type;
                leavrequest.Reason = leaveRequest.Reason;
                leavrequest.GlFiscalYearId = leaveRequest.GlFiscalYearId;
                leavrequest.Remark = leaveRequest.Remark;
                leavrequest.DateCreated = leaveRequest.DateCreated;
                leavrequest.CreatedBy = leaveRequest.CreatedBy;
                leavrequest.RequestDate = DateTime.Now;
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
                leavrequest.EmployeeCode = employee?.EmployeeCode;
                leavrequest.RequesterId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                db.Entry(leavrequest).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "LeaveRequestPersonal ", "Success", User.Identity.Name, User.Identity.Name + " update leave request of id" + leaveRequest.LeaveRequestId, Modules.HRM);
                #endregion
                GetNotiofied(leavrequest);
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
        public double GetWeekDays(DateTime startDate, DateTime endDate,int empId)
        {
            double noOfWeek = 0;
            var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId==empId&&x.isCurrent);
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
                                        noOfWeek += saturdayweekendSetting.IsHalfDay ? 0.5 : saturdayweekendSetting.IsFullDay?0:1;

                                    }
                                    else
                                    {
                                        // Skip because this is an exception for those simple employees that have Saturday as a working day, while other workers have it as an off day.
                                        noOfWeek += 1;
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
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    weekDays = GetWeekDays(f, t,empId);
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
        public string GetLeaveDaysToPulate(string empId, DateTime? from, DateTime? to, string fromAm, string toAm, string half, int hour, MorningOrAfternoon morningOrAfternoon)
        {


            decimal days = 0;
            decimal exactDeserve = 0;
            DateTime fromDate=from.HasValue?from.Value:DateTime.Now;
            DateTime toDate=to.HasValue?to.Value:DateTime.Now;
            var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == LeaveType.ANNUAL);
            if (!string.IsNullOrEmpty(fromAm) && !string.IsNullOrEmpty(toAm))
            {
                var splitDatefrom = fromAm.Split('/');
                fromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDatefrom[0]), Convert.ToInt32(splitDatefrom[1]), Convert.ToInt32(splitDatefrom[2]));

                var splitDateto = toAm.Split('/');
                toDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateto[0]), Convert.ToInt32(splitDateto[1]), Convert.ToInt32(splitDateto[2]));
            }
            int.TryParse(empId, out int employeeId);

            if (employeeId == 0)
            {
                employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }
                
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
                        if (h.Date >= fromDate && h.Date <= toDate)
                        {
                            count++;
                        }
                    }
                }
                weekDays = GetWeekDays(fromDate, toDate, employeeId);
            }

            var balanceQuery = db.LeavesEntitle.Where(x => x.EmployeeId == employeeId && !x.IsExpired);
            decimal totalBalance = balanceQuery.Any() ? balanceQuery.Sum(x => x.Balance) : 0;

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= fromDate && x.DateTo >= fromDate);
            if (fiscalYear != null 
                && leaveTypeSetting != null
               && !leaveTypeSetting.AllowFullAnnualLeave)
            {
                    

                var entitleCurrentYear = balanceQuery.FirstOrDefault(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (entitleCurrentYear != null)
                {
                    exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, toDate, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalBalance);
                    if (exactDeserve >= totalBalance)
                        exactDeserve = totalBalance;
                }

            }
            days = toDate.Subtract(fromDate).Days + 1 - count;
            days = days - Convert.ToDecimal(weekDays);
            if (half == "1")
            {
                decimal checkWeekEnd = toDate.Subtract(fromDate).Days + 1 - count;
                double amt = HrHelper.GetHalf(fromDate, toDate);
                double notHalf = HrHelper.GetNotHalf(fromDate, toDate);
                bool isHalf = HrHelper.IsHalf(fromDate, toDate);
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
            decimal afternoonOffDays = 0m;
            days = (totalhours - somehours) / 8;
            if (morningOrAfternoon == MorningOrAfternoon.Afternoon)
            {
                afternoonOffDays = IsHalfDays(fromDate, toDate);

            }
            if (afternoonOffDays > 0)
            {
                days -= afternoonOffDays;
            }
            if (exactDeserve == 0 && totalBalance > 0)
            {
                exactDeserve = totalBalance;
            }
            //return days;
            return $"{days}-{exactDeserve}";
        }

        //[HttpPost]
        //public decimal GetBalance()
        //{
        //    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //    // Directly calculate the sum from the filtered query to avoid double enumeration
        //    return db.LeavesEntitle
        //             .Where(x => x.EmployeeId == empId && !x.IsExpired)
        //             .Sum(x => (decimal?)x.Balance) ?? 0m;
        //}
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
            if (exactDeserve >= totalbalance)
                exactDeserve = totalbalance;

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
        public string GetEmpId()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);

            if (entitle.Any())
            {
                var balance = entitle.Sum(x => x.Amount).ToString();

                return balance;
            }

            return "0";
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
        //public string GetTotalLeaveAmount( int empId, string from, string to)
        //{
        //    if(empId ==0)
        //    {
        //        empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //    }
        //    var balance = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired);
        //    decimal totalBalance = 0;
        //    if (balance.Any())
        //    {

        //        totalBalance = balance.Sum(x => x.Balance);


        //    }
        //    decimal exactDeserve = 0;
        //    DateTime fromDate = DateTime.Now;
        //    DateTime toDate = DateTime.Now;
        //    if (!string.IsNullOrEmpty(from))
        //    {
        //        var splitDatefrom = from.Split('/');
        //        fromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDatefrom[0]), Convert.ToInt32(splitDatefrom[1]), Convert.ToInt32(splitDatefrom[2]));

        //    }
        //    if (!string.IsNullOrEmpty(to))
        //    {
        //        var splitDateto = to.Split('/');
        //        toDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateto[0]), Convert.ToInt32(splitDateto[1]), Convert.ToInt32(splitDateto[2]));

        //    }

        //    var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= fromDate && x.DateTo >= fromDate);
        //    if (fiscalYear != null)
        //    {
        //        if (string.IsNullOrEmpty(to))
        //        {
        //            toDate = fiscalYear.DateTo;
        //        }

        //        var entitleCurrentYear = balance.FirstOrDefault(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
        //        if (entitleCurrentYear != null)
        //        {
        //            exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, toDate, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalBalance);

        //        }

        //    }

        //    return $"{totalBalance}-{exactDeserve}";
        //    // return totalBalance;
        //}
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
            if (empId == 0)
            {
                empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }

            var balanceQuery = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired);
            decimal totalBalance = balanceQuery.Any() ? balanceQuery.Sum(x => x.Balance) : 0;

            decimal exactDeserve = 0;
            decimal leaveDays = 0;
            decimal afternoonOffDays = 0;
            DateTime fromDate = from.HasValue ? from.Value : DateTime.Now,  toDate = to.HasValue?to.Value: DateTime.Now;

            if (!string.IsNullOrEmpty(fromAm))
            {
                var splitDateFrom = fromAm.Split('/');
                fromDate = XAPI.EthiopicDateTime.GetGregorianDate(
                    Convert.ToInt32(splitDateFrom[0]),
                    Convert.ToInt32(splitDateFrom[1]),
                    Convert.ToInt32(splitDateFrom[2])
                );
            }

            if (!string.IsNullOrEmpty(toAm))
            {
                var splitDateTo = toAm.Split('/');
                toDate = XAPI.EthiopicDateTime.GetGregorianDate(
                    Convert.ToInt32(splitDateTo[0]),
                    Convert.ToInt32(splitDateTo[1]),
                    Convert.ToInt32(splitDateTo[2])
                );
            }
            
            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= fromDate && x.DateTo >= fromDate);
            if (fiscalYear != null)
            {
                
                var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == LeaveType.ANNUAL);
                var entitleCurrentYear = balanceQuery.FirstOrDefault(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (entitleCurrentYear != null
                    &&leaveTypeSetting !=null 
                    &&!leaveTypeSetting.AllowFullAnnualLeave)
                {
                    exactDeserve = GetEmployeeDeserveLeaveDays(
                        fiscalYear.DateFrom, toDate,
                        entitleCurrentYear.Amount,
                        entitleCurrentYear.Balance,
                        totalBalance
                    );
                }


                if ((exactDeserve == 0 && totalBalance > 0)|| exactDeserve>=totalBalance)
                {
                    exactDeserve = totalBalance;
                }

                leaveDays = HrHelper.GetLeaveDays(fromDate, toDate, fiscalYear.GlFiscalYearId, LeaveDay.Full_Day, LeaveType.ANNUAL, empId);
                if (morningOrAfternoon == MorningOrAfternoon.Afternoon)
                {
                    afternoonOffDays = IsHalfDays(fromDate, toDate);

                }
                if (afternoonOffDays > 0&& leaveDays > 0)
                {
                    leaveDays -= afternoonOffDays;
                }
            }

            return $"{totalBalance}-{exactDeserve}-{leaveDays}";
        }
        [HttpPost]
        public string GetTotalLeaveAmountGorg(int empId, DateTime? from, DateTime? to,  MorningOrAfternoon morningOrAfternoon)
        {
            if (empId == 0)
            {
                empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }

            var balanceQuery = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired);
            decimal totalBalance = balanceQuery.Any() ? balanceQuery.Sum(x => x.Balance) : 0;

            decimal exactDeserve = 0;
            decimal leaveDays = 0;
            decimal afternoonOffDays = 0;
            DateTime fromDate = from.HasValue ? from.Value : DateTime.Now;
            DateTime toDate =to.HasValue?to.Value: DateTime.Now;

           
            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= fromDate && x.DateTo >= fromDate);
            if (fiscalYear != null)
            {

                var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == LeaveType.ANNUAL);

                var entitleCurrentYear = balanceQuery.FirstOrDefault(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (entitleCurrentYear != null 
                    && leaveTypeSetting != null
                    && !leaveTypeSetting.AllowFullAnnualLeave)
                {
                    exactDeserve = GetEmployeeDeserveLeaveDays(
                        fiscalYear.DateFrom, toDate,
                        entitleCurrentYear.Amount,
                        entitleCurrentYear.Balance,
                        totalBalance
                    );
                }


                if (exactDeserve == 0 && totalBalance > 0)
                {
                    exactDeserve = totalBalance;
                }
                if (exactDeserve >= totalBalance)
                    exactDeserve = totalBalance;

                leaveDays = HrHelper.GetLeaveDays(fromDate, toDate, fiscalYear.GlFiscalYearId, LeaveDay.Full_Day, LeaveType.ANNUAL, empId);
               
                if (morningOrAfternoon == MorningOrAfternoon.Afternoon)
                {
                    afternoonOffDays = IsHalfDays(fromDate, toDate);

                }
                if (afternoonOffDays > 0)
                {
                    leaveDays -= afternoonOffDays;
                }
            }

            return $"{totalBalance}-{exactDeserve}-{leaveDays}";
        }

        public string GetDservedExactLeaveAmount( int empId,string from,string to)
        {
            decimal totalBalance = 0;
            decimal exactDeserve = 0;

            if (empId ==0)
            {
                empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }
            var balance = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired);
            if (balance.Any())
            {

                totalBalance = balance.Sum(x => x.Balance);


            }
            DateTime fromDate = DateTime.Now;
            DateTime toDate = DateTime.Now;
            if (!string.IsNullOrEmpty(from))
            {
                var splitDatefrom = from.Split('/');
                 fromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDatefrom[0]), Convert.ToInt32(splitDatefrom[1]), Convert.ToInt32(splitDatefrom[2]));

            }
            if (!string.IsNullOrEmpty(to))
            {
                var splitDateto = to.Split('/');
                toDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateto[0]), Convert.ToInt32(splitDateto[1]), Convert.ToInt32(splitDateto[2]));

            }

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= fromDate && x.DateTo >= fromDate);
            if (fiscalYear != null)
            {
                if (string.IsNullOrEmpty(to))
                {
                    toDate = fiscalYear.DateTo;
                }
                var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == LeaveType.ANNUAL);

                var entitleCurrentYear = balance.FirstOrDefault(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if(entitleCurrentYear != null
                    && leaveTypeSetting != null
                    && !leaveTypeSetting.AllowFullAnnualLeave)
                {
                    exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, toDate, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalBalance);

                }
                if (exactDeserve == 0 && totalBalance > 0)
                {
                    exactDeserve = totalBalance;
                }
                if (exactDeserve >= totalBalance)
                    exactDeserve = totalBalance;

            }
           
            return $"{totalBalance}-{exactDeserve}";
           //return exactDeserve;
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

        //public ActionResult DownloadDoument(string filename, string attachId)
        //{
        //    if (attachId == "0")
        //    {
        //        string messages = "Please select the row first!!";
        //        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //    }

        //    db = new ExceedDbContext();
        //    //try
        //    //{
        //    int.TryParse(attachId, out int attachmentId);
        //    var attachment =
        //            db.Attachments.FirstOrDefault(x => x.Reference == filename && x.ReferenceType == AttachmentType.DOCUMENT && x.AttachmentId == attachmentId);

        //    if (attachment != null)
        //    {
        //        WebClient req = new WebClient();
        //        HttpResponse response = System.Web.HttpContext.Current.Response;
        //        response.Clear();
        //        response.ClearContent();
        //        response.ClearHeaders();
        //        response.Buffer = true;
        //        response.AddHeader("Content-Disposition",
        //            "attachment;filename=\"" + attachment.Description + "\"");
        //        byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + attachment.Remark.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));

        //        // byte[] datas11 = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO + "/" + attachment.Description));

        //        response.BinaryWrite(datas);
        //        response.End();
        //        HRMInputs.SelectedFile = 0;
        //        return File(filename, attachment.FileType);
        //    }
        //    //}
        //    //catch (Exception)
        //    //{

        //    //    // throw;
        //    //}
        //    HRMInputs.SelectedFile = 0;
        //    return null;
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
