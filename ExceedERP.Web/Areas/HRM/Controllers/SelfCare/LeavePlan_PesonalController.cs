using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRSelfServiceManager)]
    public class LeavePlan_PesonalController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public static DateTime To { get; set; }
        //public static string ToAmharic { get; set; }
        //public static DateTime From { get; set; }
        //public static string FromAmharic { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {

            GetPeriod();
            PopulateViewData();

            return View();
        }

        private void PopulateViewData()
        {
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
    
        public ActionResult LeavePlan_Pesonal_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            IQueryable<LeavePlan> leaveplans = db.LeavePlans.Where(x => x.EmployeeId==employeeId);
            DataSourceResult result = leaveplans.ToDataSourceResult(request, leavePlan => new
            {
                LeavePlanId = leavePlan.LeavePlanId,
                EmployeeId = leavePlan.EmployeeId,
                GlFiscalYearId = leavePlan.GlFiscalYearId,
                From = leavePlan.From,
                To = leavePlan.To,
                LeaveAmount = leavePlan.LeaveAmount,
                Status = leavePlan.Status,
                Remark = leavePlan.Remark,
                FromAmharic = leavePlan.FromAmharic,
                ToAmharic = leavePlan.ToAmharic,
                EmployeeCode = leavePlan.EmployeeCode,
                CurrentApprovalStep = leavePlan.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = leavePlan.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leavePlan.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leavePlan.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = leavePlan.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leavePlan.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leavePlan.IsDepartmentSendForApproval,
                ForwardedFrom = leavePlan.ForwardedFrom,
                ForwardedTo = leavePlan.ForwardedTo,
                IsFinalApproval = leavePlan.IsFinalApproval,
                IsSendForApproval = leavePlan.IsSendForApproval,
                SendForApprovalTime = leavePlan.SendForApprovalTime,
                IsOnlineApproved = leavePlan.IsOnlineApproved,
                IsOnlineTransferred = leavePlan.IsOnlineTransferred,
                IsOnlineTransferredBy = leavePlan.IsOnlineTransferredBy,
                IsVoid = leavePlan.IsVoid,
                OnlineApprovedBy = leavePlan.OnlineApprovedBy,
                OnlineApprovedTime = leavePlan.OnlineApprovedTime,
                OnlineTransferredTime = leavePlan.OnlineTransferredTime,
                SendForApprovalBy = leavePlan.SendForApprovalBy,
                VoidTime = leavePlan.VoidTime,
                OrgBranchName = leavePlan.OrgBranchName,
                VoidBy = leavePlan.VoidBy,
                DateCreated = leavePlan.DateCreated,
                CreatedBy = leavePlan.CreatedBy,
                ModifiedBy = leavePlan.ModifiedBy,
                LastModified = leavePlan.LastModified
            });

            return Json(result);
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlan_Pesonal_Create([DataSourceRequest] DataSourceRequest request, LeavePlan leavePlan)
        {
            // Clear any existing model state errors
            ModelState.Clear();
            leavePlan.EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            if(leavePlan.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Map your  Employee Id and User first!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }
            // Check if both FromAmharic and ToAmharic dates are provided
            if (leavePlan.FromAmharic == null || leavePlan.ToAmharic == null)
            {
                ModelState.AddModelError("", "Both From and To Dates are required!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Convert FromAmharic date to Gregorian
            if (leavePlan.FromAmharic != null)
            {
                var splitDate = leavePlan.FromAmharic.Split('/');
                leavePlan.From = XAPI.EthiopicDateTime.GetGregorianDate(
                    Convert.ToInt32(splitDate[0]),
                    Convert.ToInt32(splitDate[1]),
                    Convert.ToInt32(splitDate[2]));
            }

            // Convert ToAmharic date to Gregorian
            if (leavePlan.ToAmharic != null)
            {
                var splitDate = leavePlan.ToAmharic.Split('/');
                leavePlan.To = XAPI.EthiopicDateTime.GetGregorianDate(
                    Convert.ToInt32(splitDate[0]),
                    Convert.ToInt32(splitDate[1]),
                    Convert.ToInt32(splitDate[2]));
            }

            // Check if the 'From' date is after the 'To' date
            if (leavePlan.From.Date > leavePlan.To.Date)
            {
                ModelState.AddModelError("", "Please check the date range!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Calculate the total number of days in the leave period
            double resultDate = GetWeekDays(leavePlan.From,leavePlan.To);

            // Retrieve the fiscal year information
            var year = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
            int selectedToYear = year.DateTo.Year;
            int selectedFromYear = year.DateFrom.Year;
            int currentYear = DateTime.Now.Year;

            // Check if the leave plan is for the current year
            if ((selectedFromYear < currentYear && selectedToYear < currentYear) ||
                (selectedFromYear > currentYear && selectedToYear > currentYear))
            {
                ModelState.AddModelError("", "You cannot plan for other years");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Calculate the total leave balance for the employee
            var totalSum = db.LeavesEntitle
                .Where(x => !x.IsExpired && x.EmployeeId == leavePlan.EmployeeId)
                .ToList();
            var totalBalance = totalSum.Sum(x => x.Balance);

            // Check if the requested leave amount exceeds the employee's total balance
            if (leavePlan.LeaveAmount > totalBalance)
            {
                ModelState.AddModelError("", "You cannot plan such an amount of leave!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            if (leavePlan.LeaveAmount > totalBalance)
            {
                ModelState.AddModelError("", "You cannot plan such an amount of leave!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Check if the requested leave amount exceeds the number of days in the period
            else if (leavePlan.LeaveAmount > (decimal)resultDate)
            {
                ModelState.AddModelError("", "Leave date cannot be out of range");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Set additional properties for the leave plan
            leavePlan.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            leavePlan.EmployeeCode = inputs.GetEmployees()
                .FirstOrDefault(x => x.PersonId == leavePlan.EmployeeId)?.EmployeeCode;
            leavePlan.CreatedBy = User.Identity.Name;
            leavePlan.DateCreated = DateTime.Now;
            //leavePlan.LeaveAmount = (decimal)resultDate;
            // If the model state is valid, save the leave plan to the database
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(leavePlan);
                db.LeavePlans.Add(entity);
                db.SaveChanges();
                leavePlan.LeavePlanId = entity.LeavePlanId;

                #region operation log
                // Log the operation in the system for auditing
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Plan", "Success", User.Identity.Name,
                        "Leave Plan for " + person.EmployeeCode + " is created by " + User.Identity.Name,
                        Modules.HRM);
                }
                #endregion
            }

            // Return the result as JSON
            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }

        //public ActionResult LeavePlan_Pesonal_Create([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        //{
        //    ModelState.Clear();
        //    if (leavePlan.FromAmharic == null || leavePlan.ToAmharic == null)
        //    {
        //        ModelState.AddModelError("", "Both From and to Dates are required!!");
        //        return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        //    }
        //    if (leavePlan.FromAmharic != null)
        //    {
        //        var splitDate = leavePlan.FromAmharic.Split('/');
        //        leavePlan.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    if (leavePlan.ToAmharic != null)
        //    {
        //        var splitDate = leavePlan.ToAmharic.Split('/');
        //        leavePlan.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    if (leavePlan.From.Date > leavePlan.To.Date)
        //    {
        //        ModelState.AddModelError("", "Please Check the date range!!");
        //        return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));

        //    }

        //    int resultDate = leavePlan.To.Subtract(leavePlan.From).Days;

        //    var year = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
        //    int selectedToYear = year.DateTo.Year;
        //    int selectedFromYear = year.DateFrom.Year;
        //    int currentYear = DateTime.Now.Year;



        //    if ((selectedFromYear < currentYear && selectedToYear < currentYear) || (selectedFromYear > currentYear && selectedToYear > currentYear))
        //    {
        //        ModelState.AddModelError("", "You can not Plan for other years");
        //        return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));

        //    }
        //    var totalSum = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePlan.EmployeeId).ToList();
        //    var totalBalance = totalSum.Sum(x => x.Balance);
        //    if (leavePlan.LeaveAmount > totalBalance)
        //    {

        //        ModelState.AddModelError("", "You can not plan such amount of leave!!");
        //        return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));


        //    }
        //    else if (leavePlan.LeaveAmount > resultDate)
        //    {
        //        ModelState.AddModelError("", "Leave Date cannot be out of range");
        //        return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));

        //    }
        //    leavePlan.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
        //    leavePlan.EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leavePlan.EmployeeId)?.EmployeeCode;
        //    leavePlan.CreatedBy = User.Identity.Name;
        //    leavePlan.DateCreated = DateTime.Now;
        //    if (ModelState.IsValid)
        //    {

        //        var entity = GetMappedObject(leavePlan);
        //            db.LeavePlans.Add(entity);
        //            db.SaveChanges();
        //            leavePlan.LeavePlanId = entity.LeavePlanId;

        //            #region operation log
        //            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
        //            if (person != null)
        //            {
        //                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Plan", "Success", User.Identity.Name, "Leave Plan for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);

        //            }
        //            #endregion


        //    }

        //    return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        //}


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlan_Pesonal_Update([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {
            ModelState.Clear();
            // Check if both FromAmharic and ToAmharic dates are provided
            if (leavePlan.FromAmharic == null || leavePlan.ToAmharic == null)
            {
                ModelState.AddModelError("", "Both From and To Dates are required!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Convert FromAmharic date to Gregorian
            if (leavePlan.FromAmharic != null)
            {
                var splitDate = leavePlan.FromAmharic.Split('/');
                leavePlan.From = XAPI.EthiopicDateTime.GetGregorianDate(
                    Convert.ToInt32(splitDate[0]),
                    Convert.ToInt32(splitDate[1]),
                    Convert.ToInt32(splitDate[2]));
            }

            // Convert ToAmharic date to Gregorian
            if (leavePlan.ToAmharic != null)
            {
                var splitDate = leavePlan.ToAmharic.Split('/');
                leavePlan.To = XAPI.EthiopicDateTime.GetGregorianDate(
                    Convert.ToInt32(splitDate[0]),
                    Convert.ToInt32(splitDate[1]),
                    Convert.ToInt32(splitDate[2]));
            }

            // Check if the 'From' date is after the 'To' date
            if (leavePlan.From.Date > leavePlan.To.Date)
            {
                ModelState.AddModelError("", "Please check the date range!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Calculate the total number of days in the leave period
            double resultDate = GetWeekDays(leavePlan.From, leavePlan.To);

            // Retrieve the fiscal year information
            var year = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
            int selectedToYear = year.DateTo.Year;
            int selectedFromYear = year.DateFrom.Year;
            int currentYear = DateTime.Now.Year;

            // Check if the leave plan is for the current year
            if ((selectedFromYear < currentYear && selectedToYear < currentYear) ||
                (selectedFromYear > currentYear && selectedToYear > currentYear))
            {
                ModelState.AddModelError("", "You cannot plan for other years");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Calculate the total leave balance for the employee
            var totalSum = db.LeavesEntitle
                .Where(x => !x.IsExpired && x.EmployeeId == leavePlan.EmployeeId)
                .ToList();
            var totalBalance = totalSum.Sum(x => x.Balance);

            // Check if the requested leave amount exceeds the employee's total balance
            if (leavePlan.LeaveAmount > totalBalance)
            {
                ModelState.AddModelError("", "You cannot plan such an amount of leave!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            if (leavePlan.LeaveAmount > totalBalance)
            {
                ModelState.AddModelError("", "You cannot plan such an amount of leave!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Check if the requested leave amount exceeds the number of days in the period
            else if (leavePlan.LeaveAmount > (decimal)resultDate)
            {
                ModelState.AddModelError("", "Leave date cannot be out of range");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Set additional properties for the leave plan
            leavePlan.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            leavePlan.EmployeeCode = inputs.GetEmployees()
                .FirstOrDefault(x => x.PersonId == leavePlan.EmployeeId)?.EmployeeCode;
            //leavePlan.LeaveAmount = (decimal)resultDate;
            // If the model state is valid, save the leave plan to the database
            leavePlan.ModifiedBy = User.Identity.Name;
            leavePlan.LastModified = DateTime.Now;
            if (ModelState.IsValid)
            {
               
               

                    var entity = GetMappedObject(leavePlan);
                    
                    db.LeavePlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                       
                            UserHelper.OperationLog(Request.UserHostAddress, "Leave Plan", "Success", User.Identity.Name, "Leave Plan of "+person.EmployeeCode +" is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                
            }

            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }
  
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlan_Pesonal_Destroy([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(leavePlan);
                #region delete log
                var leaves = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
                var delete = leaves.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave Plan of employee" + leavePlan.EmployeeCode + " and fiscal year " + delete);
                #endregion
                db.LeavePlans.Attach(entity);
                db.LeavePlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }
        private LeavePlan GetMappedObject(LeavePlan leavePlan)
        {
            var entity = new LeavePlan
            {
                LeavePlanId = leavePlan.LeavePlanId,
                EmployeeId = leavePlan.EmployeeId,
                GlFiscalYearId = leavePlan.GlFiscalYearId,
                From = leavePlan.From,
                To = leavePlan.To,
                LeaveAmount = leavePlan.LeaveAmount,
                Status = leavePlan.Status,
                Remark = leavePlan.Remark,
                FromAmharic = leavePlan.FromAmharic,
                ToAmharic = leavePlan.ToAmharic,
                EmployeeCode = leavePlan.EmployeeCode,
                CurrentApprovalStep = leavePlan.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = leavePlan.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leavePlan.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leavePlan.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = leavePlan.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leavePlan.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leavePlan.IsDepartmentSendForApproval,
                ForwardedFrom = leavePlan.ForwardedFrom,
                ForwardedTo = leavePlan.ForwardedTo,
                IsFinalApproval = leavePlan.IsFinalApproval,
                IsSendForApproval = leavePlan.IsSendForApproval,
                SendForApprovalTime = leavePlan.SendForApprovalTime,
                IsOnlineApproved = leavePlan.IsOnlineApproved,
                IsOnlineTransferred = leavePlan.IsOnlineTransferred,
                IsOnlineTransferredBy = leavePlan.IsOnlineTransferredBy,
                IsVoid = leavePlan.IsVoid,
                OnlineApprovedBy = leavePlan.OnlineApprovedBy,
                OnlineApprovedTime = leavePlan.OnlineApprovedTime,
                OnlineTransferredTime = leavePlan.OnlineTransferredTime,
                SendForApprovalBy = leavePlan.SendForApprovalBy,
                VoidTime = leavePlan.VoidTime,
                OrgBranchName = leavePlan.OrgBranchName,
                VoidBy = leavePlan.VoidBy,
                DateCreated = leavePlan.DateCreated,
                CreatedBy = leavePlan.CreatedBy,
                ModifiedBy = leavePlan.ModifiedBy,
                LastModified = leavePlan.LastModified
            };
            return entity;
        }
        private double GetWeekDays(DateTime startDate, DateTime endDate)
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

        public JsonResult SendForApprovalRequest(int id)
        {
            var leavePlan = db.LeavePlans.Find(id);
            if (leavePlan != null && !leavePlan.IsSendForApproval)
            {
                leavePlan.IsSendForApproval = true;
                leavePlan.SendForApprovalTime = DateTime.Now;
                leavePlan.SendForApprovalBy = User.Identity.Name;
                db.Entry(leavePlan).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "You have Sent successfully for  approval.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }
            var formattedData1 = new
            {
                Message = "it is sent Once you can't do again.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave plan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave plan");
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
