using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveUser)]
    public class LeaveTypeSettingsController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/SickLeaveSetting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LeaveTypeSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LeaveTypeSetting> leaveTypeSettings= db.LeaveTypeSettings;
            DataSourceResult result = leaveTypeSettings.ToDataSourceResult(request, leaveTypeSetting => new
            {
                LeaveTypeSettingId = leaveTypeSetting.LeaveTypeSettingId,
                LeaveType = leaveTypeSetting.LeaveType,
                IsConsecutiveDays = leaveTypeSetting.IsConsecutiveDays,
                BlockBackdatedLeave = leaveTypeSetting.BlockBackdatedLeave,
                IsWorkingDays = leaveTypeSetting.IsWorkingDays,
                MaximumAmount = leaveTypeSetting.MaximumAmount,
                Remark = leaveTypeSetting.Remark,
                CreatedBy = leaveTypeSetting.CreatedBy,
                DateCreated = leaveTypeSetting.DateCreated,
                LastModified = leaveTypeSetting.LastModified,
                ModifiedBy = leaveTypeSetting.ModifiedBy,
                OrgBranchName = leaveTypeSetting.OrgBranchName,
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveTypeSettings_Create([DataSourceRequest]DataSourceRequest request, LeaveTypeSetting leave)
        {
            if (db.LeaveTypeSettings.Any(x => x.LeaveType == leave.LeaveType))
            {
                ModelState.AddModelError("exist", "Record exist dublication is not allowed");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }
            if (leave.IsWorkingDays 
                && leave.IsConsecutiveDays)
            {
                ModelState.AddModelError("exist", "thie leave Type can't be both  conscutive and working days select one only");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }
            if (!leave.IsWorkingDays 
                && !leave.IsConsecutiveDays)
            {
                ModelState.AddModelError("exist", "thie leave Type is either  conscutive and working days choose one");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObect(leave);
                db.LeaveTypeSettings.Add(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Leave Type Setting", "Success", User.Identity.Name, "Sick Leave Setting is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveTypeSettings_Update([DataSourceRequest]DataSourceRequest request, LeaveTypeSetting leave)
        {

            if (db.LeaveTypeSettings.Any(x => x.LeaveType == leave.LeaveType && x.LeaveTypeSettingId != leave.LeaveTypeSettingId))
            {
                ModelState.AddModelError("exist", "Record exist dublication is not allowed");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
                
            }
            if (leave.IsWorkingDays && leave.IsConsecutiveDays)
            {
                ModelState.AddModelError("exist", "thie leave Type can't be both  conscutive and working days select one only");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }
            if (!leave.IsWorkingDays
               && !leave.IsConsecutiveDays)
            {
                ModelState.AddModelError("exist", "thie leave Type is either  conscutive and working days choose one");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {

                var entity = GetMappedObect(leave);
                db.LeaveTypeSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, " Leave Type Setting", "Success", User.Identity.Name, " Leave Type Setting is Updated By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }


            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveTypeSettings_Destroy([DataSourceRequest]DataSourceRequest request, LeaveTypeSetting leave)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObect(leave);

                #region delete log
                //var leaves = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
                //var delete = leaves.Name;
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave Plan of employee" + leavePlan.EmployeeCode + " and fiscal year " + delete);
                #endregion
                db.LeaveTypeSettings.Attach(entity);
                db.LeaveTypeSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }
        private LeaveTypeSetting GetMappedObect(LeaveTypeSetting leaveTypeSetting)
        {
            var entity = new LeaveTypeSetting
            {
                LeaveTypeSettingId=leaveTypeSetting.LeaveTypeSettingId,
                LeaveType = leaveTypeSetting.LeaveType,
                IsConsecutiveDays = leaveTypeSetting.IsConsecutiveDays,
                IsWorkingDays = leaveTypeSetting.IsWorkingDays,
                BlockBackdatedLeave = leaveTypeSetting.BlockBackdatedLeave,
                MaximumAmount = leaveTypeSetting.MaximumAmount,
                Remark = leaveTypeSetting.Remark,
                CreatedBy = leaveTypeSetting.CreatedBy,
                DateCreated = leaveTypeSetting.DateCreated,
                LastModified = leaveTypeSetting.LastModified,
                ModifiedBy = leaveTypeSetting.ModifiedBy,
                OrgBranchName = leaveTypeSetting.OrgBranchName,
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
