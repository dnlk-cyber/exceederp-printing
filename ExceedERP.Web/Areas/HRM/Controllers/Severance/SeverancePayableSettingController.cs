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
using ExceedERP.Core.Domain.HRM.Severance;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRAdmin)]
    public class SeverancePayableSettingController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/SickLeaveSetting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SeverancePayableSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SeverancePayableSetting> payableSettings = db.SeverancePayableSettings;
            DataSourceResult result = payableSettings.ToDataSourceResult(request, payableSetting => new
            {
                SeverancePayableSettingId = payableSetting.SeverancePayableSettingId,
                SalaryValue = payableSetting.SalaryValue,
                ResponsibilityLevel = payableSetting.ResponsibilityLevel,
                Description = payableSetting.Description,
                DateCreated = payableSetting.DateCreated,
                CreatedBy = payableSetting.CreatedBy,
                ModifiedBy = payableSetting.ModifiedBy,
                LastModified = payableSetting.LastModified,
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeverancePayableSetting_Create([DataSourceRequest]DataSourceRequest request, SeverancePayableSetting payableSetting)
        {
            if (ModelState.IsValid)
            {


                var entity = GetMappedObject(payableSetting);
                db.SeverancePayableSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                        UserHelper.OperationLog(Request.UserHostAddress, "severance payable Setting", "Success", User.Identity.Name, "severance payable Setting is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { payableSetting }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeverancePayableSetting_Update([DataSourceRequest]DataSourceRequest request, SeverancePayableSetting payableSetting)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(payableSetting);
                db.SeverancePayableSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "severance payable Setting", "Success", User.Identity.Name, "severance payable Setting is Updated By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }


            return Json(new[] { payableSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeverancePayableSetting_Destroy([DataSourceRequest]DataSourceRequest request, SeverancePayableSetting payableSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(payableSetting);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  by "+User.Identity.Name );
                #endregion
                db.SeverancePayableSettings.Attach(entity);
                db.SeverancePayableSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { payableSetting }.ToDataSourceResult(request, ModelState));
        }

        private SeverancePayableSetting GetMappedObject(SeverancePayableSetting payableSetting)
        {
            var entity = new SeverancePayableSetting
            {
                SeverancePayableSettingId = payableSetting.SeverancePayableSettingId,
                SalaryValue = payableSetting.SalaryValue,
                ResponsibilityLevel = payableSetting.ResponsibilityLevel,
                Description = payableSetting.Description,
                DateCreated = payableSetting.DateCreated,
                CreatedBy = payableSetting.CreatedBy,
                ModifiedBy = payableSetting.ModifiedBy,
                LastModified = payableSetting.LastModified,
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
