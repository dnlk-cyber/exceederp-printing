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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveUser)]
    public class SickLeaveSettingController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/SickLeaveSetting
        public ActionResult Index()
        {
            GenerateLeaveStartDateSetting();
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["FisicalYear"] = db.GLFiscalYears.AsQueryable();
            var LeaveSetting = Enum.GetValues(typeof(LeaveSetting))
                .Cast<LeaveSetting>()
                .Select(v => v.ToString())
                .ToList();
            var LeaveSettings = LeaveSetting.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["LeaveSetting"] = LeaveSettings;
            ViewData["FiscalYeaer"] = db.GLFiscalYears.AsQueryable();
        }
        public void GenerateLeaveStartDateSetting()
        {
            var leaveStartDate = db.LeaveEntitleStartingDates.FirstOrDefault();

            if (leaveStartDate == null)
            {
                var entity = new LeaveEntitleStartingDate
                {
                    Start = LeaveEntitleStarting.Fiscal_Year_End_Date,
                    Remark = "Default Setting"
                };

                db.LeaveEntitleStartingDates.Add(entity);
                db.SaveChanges();
            }
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SickLeaveSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SickLeaveSetting> sickLeave = db.SickLeaveSettings;
            DataSourceResult result = sickLeave.ToDataSourceResult(request, leave => new
            {
                SickLeaveSettingId = leave.SickLeaveSettingId,
                CurrentValue = leave.CurrentValue
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SickLeaveSetting_Create([DataSourceRequest]DataSourceRequest request, SickLeaveSetting leave)
        {
            if (ModelState.IsValid)
            {


                var entity = new SickLeaveSetting
                {

                    CurrentValue = leave.CurrentValue,
                };
                db.SickLeaveSettings.Add(entity);
                db.SaveChanges();

                #region operation log
                
                  
                        UserHelper.OperationLog(Request.UserHostAddress, "Sick Leave Setting", "Success", User.Identity.Name, "Sick Leave Setting is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SickLeaveSetting_Update([DataSourceRequest]DataSourceRequest request, SickLeaveSetting leave)
        {

            if (ModelState.IsValid)
            {

                var entity = new SickLeaveSetting
                {
                    SickLeaveSettingId = leave.SickLeaveSettingId,
                    CurrentValue = leave.CurrentValue,
                };
                db.SickLeaveSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
              
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Sick Leave Setting", "Success", User.Identity.Name, "Sick Leave Setting is Updated By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }


            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SickLeaveSetting_Destroy([DataSourceRequest]DataSourceRequest request, SickLeaveSetting leave)
        {
            if (ModelState.IsValid)
            {
                var entity = new SickLeaveSetting
                {
                    SickLeaveSettingId = leave.SickLeaveSettingId,
                    CurrentValue = leave.CurrentValue,
                };
                #region delete log
                //var leaves = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
                //var delete = leaves.Name;
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave Plan of employee" + leavePlan.EmployeeCode + " and fiscal year " + delete);
                #endregion
                db.SickLeaveSettings.Attach(entity);
                db.SickLeaveSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
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
