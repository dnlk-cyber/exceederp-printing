using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{

    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin)]
    public class MainCompanyEvalutionSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult MainCompanyEvalutionSettings_Read([DataSourceRequest]DataSourceRequest request, int masterId)
        {
            IQueryable<MainCompanyEvalutionSetting> mainCompanies = db.MainCompanyEvalutionSettings.Where(x => x.MeasurementDefinitionId == masterId);
            DataSourceResult result = mainCompanies.ToDataSourceResult(request, mainCompany => new
            {
                MainCompanyEvalutionSettingId = mainCompany.MainCompanyEvalutionSettingId,
                OrgStructureId = mainCompany.OrgStructureId,
                LookUpId = mainCompany.LookUpId,
                MeasurementDefinitionId = mainCompany.MeasurementDefinitionId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MainCompanyEvalutionSettings_Create([DataSourceRequest]DataSourceRequest request, MainCompanyEvalutionSetting mainCompany, int masterId)
        {
            mainCompany.MeasurementDefinitionId = masterId;
            var definition = db.MainCompanyEvalutionSettings.FirstOrDefault(x => x.MeasurementDefinitionId == masterId &&x.LookUpId==mainCompany.LookUpId && x.OrgStructureId==mainCompany.OrgStructureId);
            if (definition != null)
            {
                
                    ModelState.AddModelError("", "Registered Already");
               

            }
           
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(mainCompany);
                db.MainCompanyEvalutionSettings.Add(entity);
                db.SaveChanges();
                mainCompany.MainCompanyEvalutionSettingId = entity.MainCompanyEvalutionSettingId;
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MainCompanyEvalutionSettings_Update([DataSourceRequest]DataSourceRequest request, MainCompanyEvalutionSetting mainCompany)
        {
            var definition = db.MainCompanyEvalutionSettings.FirstOrDefault(x => x.MeasurementDefinitionId == mainCompany.MeasurementDefinitionId && x.LookUpId == mainCompany.LookUpId 
                                                                    && x.OrgStructureId == mainCompany.OrgStructureId&&x.MainCompanyEvalutionSettingId !=mainCompany.MainCompanyEvalutionSettingId);
            if (definition != null)
            {

                ModelState.AddModelError("", "Registered Already");
            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(mainCompany);
                db.MainCompanyEvalutionSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MainCompanyEvalutionSettings_Destroy([DataSourceRequest]DataSourceRequest request, MainCompanyEvalutionSetting mainCompany)
        {
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(mainCompany);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted MainCompanyEvalutionSetting information  Name " + mainCompany.OrgStructureId);
                #endregion
                db.MainCompanyEvalutionSettings.Attach(entity);
                db.MainCompanyEvalutionSettings.Remove(entity);
                db.SaveChanges();
                
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        private MainCompanyEvalutionSetting GetMappedObject(MainCompanyEvalutionSetting mainCompany)
        {
            var entity = new MainCompanyEvalutionSetting
            {
                MainCompanyEvalutionSettingId = mainCompany.MainCompanyEvalutionSettingId,
                OrgStructureId = mainCompany.OrgStructureId,
                LookUpId = mainCompany.LookUpId,
                MeasurementDefinitionId = mainCompany.MeasurementDefinitionId,
               
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
