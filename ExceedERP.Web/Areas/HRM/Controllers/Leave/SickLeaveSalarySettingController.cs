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
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveUser)]
    public class SickLeaveSalarySettingController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/SickLeaveSalarySetting
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult SickLeaveSalarySetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SickLeaveSalarySetting> sickLeave = db.SickLeaveSalarySettings;
            DataSourceResult result = sickLeave.ToDataSourceResult(request, salarySetting => new
            {
                SickLeaveSalarySettingId = salarySetting.SickLeaveSalarySettingId,
                FromDays = salarySetting.FromDays,
                ToDays = salarySetting.ToDays,
                PayableSalaryInPercent = salarySetting.PayableSalaryInPercent,
                DateCreated = salarySetting.DateCreated,
                CreatedBy = salarySetting.CreatedBy,
                LastModified = salarySetting.LastModified,
                ModifiedBy = salarySetting.ModifiedBy
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SickLeaveSalarySetting_Create([DataSourceRequest]DataSourceRequest request, SickLeaveSalarySetting leave)
        {
            if (leave.FromDays >= leave.ToDays)
            {
                ModelState.AddModelError("", $"In the days range, From days{leave.FromDays} should be less than To Days{leave.ToDays}");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }
            if (db.SickLeaveSalarySettings.Any(x => x.FromDays == leave.FromDays
                                                 || x.ToDays == leave.ToDays))
            {
                ModelState.AddModelError("", $"we can't have Dublicate values {leave.FromDays} or  {leave.ToDays}");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {

                leave.DateCreated = DateTime.Now;
                leave.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(leave);
                db.SickLeaveSalarySettings.Add(entity);
                db.SaveChanges();

                #region operation log
                
                  
                        UserHelper.OperationLog(Request.UserHostAddress, "Sick Leave Setting", "Success", User.Identity.Name, "Sick Leave Setting is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SickLeaveSalarySetting_Update([DataSourceRequest]DataSourceRequest request, SickLeaveSalarySetting leave)
        {
            if (leave.FromDays >= leave.ToDays)
            {
                ModelState.AddModelError("", $"In the days range, From days{leave.FromDays} should be less than To Days{leave.ToDays}");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }
            if (db.SickLeaveSalarySettings.Any(x=>(x.FromDays==leave.FromDays
                                    ||x.ToDays==leave.ToDays)
                                    &&x.SickLeaveSalarySettingId !=leave.SickLeaveSalarySettingId))
            {
                ModelState.AddModelError("", $"we can't have Dublicate values {leave.FromDays} or  {leave.ToDays}");
                return Json(new[] { leave }.ToDataSourceResult(request, ModelState));

            }

            if (ModelState.IsValid)
            {
                leave.LastModified = DateTime.Now;
                leave.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(leave);
                db.SickLeaveSalarySettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
              
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Sick Leave Setting", "Success", User.Identity.Name, "Sick Leave Setting is Updated By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }


            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SickLeaveSalarySetting_Destroy([DataSourceRequest]DataSourceRequest request, SickLeaveSalarySetting leave)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(leave);
                #region delete log
                //var leaves = db.GLFiscalYears.Find(leavePlan.GlFiscalYearId);
                //var delete = leaves.Name;
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave Plan of employee" + leavePlan.EmployeeCode + " and fiscal year " + delete);
                #endregion
                db.SickLeaveSalarySettings.Attach(entity);
                db.SickLeaveSalarySettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        private SickLeaveSalarySetting GetMappedObject(SickLeaveSalarySetting salarySetting)
        {
            var entity = new SickLeaveSalarySetting
            {
                SickLeaveSalarySettingId= salarySetting.SickLeaveSalarySettingId,
                FromDays= salarySetting.FromDays,
                ToDays= salarySetting.ToDays,
                PayableSalaryInPercent= salarySetting.PayableSalaryInPercent,
                DateCreated= salarySetting.DateCreated,
                CreatedBy= salarySetting.CreatedBy,
                LastModified= salarySetting.LastModified,
                ModifiedBy= salarySetting.ModifiedBy
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
