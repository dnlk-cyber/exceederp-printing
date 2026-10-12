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
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin)]
    public class LeavePayableSettingController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/LeavePayableSetting
        public ActionResult Index()
        {
            return View();
        }
        
      
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeavePayableSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LeavePayableSetting> leavePayableSettings = db.LeavePayableSettings;
            DataSourceResult result = leavePayableSettings.ToDataSourceResult(request, leavePayableSetting => new
            {
                LeavePayableSettingId = leavePayableSetting.LeavePayableSettingId,
                IncludeCurrentFiscalYearDays = leavePayableSetting.IncludeCurrentFiscalYearDays,
                EnableTaxableAllowance = leavePayableSetting.EnableTaxableAllowance,
                CurrentValue = leavePayableSetting.CurrentValue,
                CreatedBy = leavePayableSetting.CreatedBy,
                DateCreated = leavePayableSetting.DateCreated,
                LastModified = leavePayableSetting.LastModified,
                ModifiedBy = leavePayableSetting.ModifiedBy,

            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayableSetting_Create([DataSourceRequest]DataSourceRequest request, LeavePayableSetting leavePayableSetting)
        {
            if (ModelState.IsValid)
            {


                var entity = new LeavePayableSetting
                {

                    CurrentValue = leavePayableSetting.CurrentValue,
                    IncludeCurrentFiscalYearDays = leavePayableSetting.IncludeCurrentFiscalYearDays,
                    EnableTaxableAllowance = leavePayableSetting.EnableTaxableAllowance,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = User.Identity.Name


                };
                db.LeavePayableSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                        UserHelper.OperationLog(Request.UserHostAddress, "Leave payable Setting", "Success", User.Identity.Name, "Leave payable Setting is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { leavePayableSetting }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayableSetting_Update([DataSourceRequest]DataSourceRequest request, LeavePayableSetting leavePayableSetting)
        {

            if (ModelState.IsValid)
            {

                var entity = new LeavePayableSetting
                {
                    LeavePayableSettingId = leavePayableSetting.LeavePayableSettingId,
                    CurrentValue = leavePayableSetting.CurrentValue,
                    IncludeCurrentFiscalYearDays = leavePayableSetting.IncludeCurrentFiscalYearDays,
                    EnableTaxableAllowance = leavePayableSetting.EnableTaxableAllowance,
                    CreatedBy = leavePayableSetting.CreatedBy,
                    DateCreated = leavePayableSetting.DateCreated,
                    LastModified = leavePayableSetting.LastModified,
                    ModifiedBy = leavePayableSetting.ModifiedBy,

                };
                db.LeavePayableSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log



                UserHelper.OperationLog(Request.UserHostAddress, "Leave payable Setting", "Success", User.Identity.Name, "Leave payable Setting is Updated By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }


            return Json(new[] { leavePayableSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayableSetting_Destroy([DataSourceRequest]DataSourceRequest request, LeavePayableSetting leavePayableSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeavePayableSetting
                {
                    LeavePayableSettingId = leavePayableSetting.LeavePayableSettingId,
                    CurrentValue = leavePayableSetting.CurrentValue,
                    IncludeCurrentFiscalYearDays = leavePayableSetting.IncludeCurrentFiscalYearDays,
                    CreatedBy = leavePayableSetting.CreatedBy,
                    DateCreated = leavePayableSetting.DateCreated,
                    LastModified = leavePayableSetting.LastModified,
                    ModifiedBy = leavePayableSetting.ModifiedBy,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted leave Payable Setting"+ leavePayableSetting.LeavePayableSettingId);
                #endregion
                db.LeavePayableSettings.Attach(entity);
                db.LeavePayableSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leavePayableSetting }.ToDataSourceResult(request, ModelState));
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
