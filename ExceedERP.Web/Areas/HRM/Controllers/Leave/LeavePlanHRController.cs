using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;

using ExceedERP.DataAccess.Context;
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
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin,HrmConstants.HRLeaveUser)]
    public class LeavePlanHRController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        
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
            ViewData["Period"] = db.GLPeriods.ToList();
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
       
        // Utility function for safer Amharic-to-Gregorian conversion
        public ActionResult LeavePlans_Read([DataSourceRequest]DataSourceRequest request)
        {
           
            IQueryable<LeavePlan> leaveplans = db.LeavePlans.Where(x =>x.IsDepartmentOnlineApproved);
            DataSourceResult result = leaveplans.ToDataSourceResult(request, leavePlan => new
            {
                LeavePlanId = leavePlan.LeavePlanId,
                EmployeeId = leavePlan.EmployeeId,
                GlFiscalYearId = leavePlan.GlFiscalYearId,
                GLPeriodId = leavePlan.GLPeriodId,
                From = leavePlan.From,
                To = leavePlan.To,
                LeaveAmount = leavePlan.LeaveAmount,
                Day = leavePlan.Day,
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
        public ActionResult LeavePlans_Create([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {

            // Validation
            if ((string.IsNullOrEmpty(leavePlan.FromAmharic) || string.IsNullOrEmpty(leavePlan.ToAmharic)) &&
                (!leavePlan.GLPeriodId.HasValue || leavePlan.GLPeriodId == 0))
            {
                ModelState.AddModelError("", "Select (From and To Dates) or Period!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // If GLPeriodId is provided, override dates using GLPeriod
            if (leavePlan.GLPeriodId.HasValue && leavePlan.GLPeriodId != 0)
            {
                var glPeriod = db.GLPeriods.Find(leavePlan.GLPeriodId);
                if (glPeriod != null)
                {
                    leavePlan.From = glPeriod.DateFrom;
                    leavePlan.To = glPeriod.DateTo;
                    leavePlan.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(glPeriod.DateFrom.Day, glPeriod.DateFrom.Month, glPeriod.DateFrom.Year);
                    leavePlan.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(glPeriod.DateTo.Day, glPeriod.DateTo.Month, glPeriod.DateTo.Year);
                }
            }
            else
            {
            
                    // Convert Amharic (Ethiopian) dates to Gregorian
                    leavePlan.From = HrHelper.ParseEthiopicDate(leavePlan.FromAmharic);
                    leavePlan.To = HrHelper.ParseEthiopicDate(leavePlan.ToAmharic);
             
               
            }


            if (leavePlan.From > leavePlan.To)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }
           
            // Check if 'From' date is after 'To' date
            if (leavePlan.From.Date > leavePlan.To.Date)
            {
                ModelState.AddModelError("", "Please check the date range!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }
            // Calculate the total leave balance for the employee

            // Retrieve the fiscal year information
            var year = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
            int selectedToYear = year.DateTo.Year;
            int selectedFromYear = year.DateFrom.Year;
            int currentYear = DateTime.Now.Year;

            // Check if the leave plan is for the current year
            if ((selectedFromYear < currentYear 
                && selectedToYear < currentYear) 
                ||(selectedFromYear > currentYear 
                && selectedToYear > currentYear))
            {
                ModelState.AddModelError("", "You cannot plan for other years");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // Calculate the total number of days in the leave period
            decimal resultDate = HrHelper.GetLeaveDays(leavePlan.From, leavePlan.To, leavePlan.GlFiscalYearId, leavePlan.Day, LeaveType.ANNUAL, leavePlan.EmployeeId);
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
            if (leavePlan.LeaveAmount==0)
            {
                leavePlan.LeaveAmount = resultDate;
            }
            // Set additional properties for the leave plan
            leavePlan.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            leavePlan.EmployeeCode = inputs.GetEmployees()
                .FirstOrDefault(x => x.PersonId == leavePlan.EmployeeId)?.EmployeeCode;
            leavePlan.CreatedBy = User.Identity.Name;
            leavePlan.DateCreated = DateTime.Now;
            leavePlan.IsSendForApproval = true;
            leavePlan.IsVoid = false;
            leavePlan.IsDepartmentOnlineApproved = true;
            leavePlan.DepartmentOnlineApprovedBy = User.Identity.Name;
            leavePlan.DepartmentOnlineApprovedTime = DateTime.Now;
            // If the model state is valid, save the leave plan to the database
            if (ModelState.IsValid)
            {




                var entity = GetMappedObject(leavePlan);
                db.LeavePlans.Add(entity);
                db.SaveChanges();
                leavePlan.LeavePlanId = entity.LeavePlanId;
                var planValidation = new LeavePlanValidation
                {
                    LeavePlanId = entity.LeavePlanId,
                    Status = ApprovalStatuses.Approve,
                    Remark = "HR",
                    DateCreated = DateTime.Now,
                    LastModified = null,
                    CreatedBy =User.Identity.Name,
                    ModifiedBy = null
                };
                db.LeavePlanValidation.Add(planValidation);
                db.SaveChanges();
                planValidation.LeavePlanValidationId = planValidation.LeavePlanValidationId;
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        
                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Plan", "Success", User.Identity.Name, "Leave Plan for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                

            }

            return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePlans_Update([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
        {
            
            // Validation
            if ((string.IsNullOrEmpty(leavePlan.FromAmharic) || string.IsNullOrEmpty(leavePlan.ToAmharic)) &&
                (!leavePlan.GLPeriodId.HasValue || leavePlan.GLPeriodId == 0))
            {
                ModelState.AddModelError("", "Select (From and To Dates) or Period!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));
            }

            // If GLPeriodId is provided, override dates using GLPeriod
            if (leavePlan.GLPeriodId.HasValue && leavePlan.GLPeriodId != 0)
            {
                var glPeriod = db.GLPeriods.Find(leavePlan.GLPeriodId);
                if (glPeriod != null)
                {
                    leavePlan.From = glPeriod.DateFrom;
                    leavePlan.To = glPeriod.DateTo;
                    leavePlan.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(glPeriod.DateFrom.Day, glPeriod.DateFrom.Month, glPeriod.DateFrom.Year);
                    leavePlan.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(glPeriod.DateTo.Day, glPeriod.DateTo.Month, glPeriod.DateTo.Year);
                }
            }
            else
            {
                
                    // Convert Amharic (Ethiopian) dates to Gregorian
                    leavePlan.From = HrHelper.ParseEthiopicDate(leavePlan.FromAmharic);
                    leavePlan.To = HrHelper.ParseEthiopicDate(leavePlan.ToAmharic);
                
            }



            decimal resultDate = HrHelper.GetLeaveDays(leavePlan.From, leavePlan.To, leavePlan.GlFiscalYearId, leavePlan.Day, LeaveType.ANNUAL, leavePlan.EmployeeId);
        
            var totalSum = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leavePlan.EmployeeId).ToList();
            var totalBalance = totalSum.Sum(x => x.Balance);
            if (leavePlan.LeaveAmount > totalBalance)
            {
                ModelState.AddModelError("", "You can not plan such amount of leave!!");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));

            }
            else if (leavePlan.LeaveAmount > resultDate)
            {
                ModelState.AddModelError("", "Leave Date cannot be out of range");
                return Json(new[] { leavePlan }.ToDataSourceResult(request, ModelState));

            }
            if (leavePlan.LeaveAmount == 0)
            {
                leavePlan.LeaveAmount = resultDate;
            }
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
        public ActionResult LeavePlans_Destroy([DataSourceRequest]DataSourceRequest request, LeavePlan leavePlan)
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
                GLPeriodId = leavePlan.GLPeriodId,
                From = leavePlan.From,
                To = leavePlan.To,
                Day = leavePlan.Day,
                LeaveAmount = leavePlan.LeaveAmount,
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
