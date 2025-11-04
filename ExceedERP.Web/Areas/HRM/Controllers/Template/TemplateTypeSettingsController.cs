using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Templates;

namespace ExceedERP.Web.Areas.HRM.Controllers.Template
{

    public class TemplateTypeSettingsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ProbationsSettings
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult TemplateTypeSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            var templateTypes = db.TemplateTypeSettings.ToList();
            DataSourceResult result = templateTypes.ToDataSourceResult(request, templateType => new
            {
                TemplateTypeSettingId = templateType.TemplateTypeSettingId,
                TemplateType = templateType.TemplateType,
                OrgBranchName = templateType.OrgBranchName,
                CreatedBy = templateType.CreatedBy,
                DateCreated = templateType.DateCreated,
                LastModified = templateType.LastModified,
                ModifiedBy = templateType.ModifiedBy,

            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TemplateTypeSettings_Create([DataSourceRequest]DataSourceRequest request, TemplateTypeSetting templateTypeSetting)
        {
            var check =db.TemplateTypeSettings.Where(x=>x.TemplateType.Trim().ToLower()==templateTypeSetting.TemplateType.Trim().ToLower()).FirstOrDefault();
            if (check != null)
            {
                ModelState.AddModelError("", "you can't add more than one records");
                return Json(new[] { templateTypeSetting }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                templateTypeSetting.CreatedBy = User.Identity.Name;
                templateTypeSetting.DateCreated = DateTime.Now;
                var entity = GetMappedObject(templateTypeSetting);

                db.TemplateTypeSettings.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Template Type Setting", "Success", User.Identity.Name, "Template Type Setting is Created By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { templateTypeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TemplateTypeSettings_Update([DataSourceRequest]DataSourceRequest request, TemplateTypeSetting templateType)
        {
            var check = db.TemplateTypeSettings
                                    .Where(x => x.TemplateType.Trim().ToLower() == templateType.TemplateType.Trim().ToLower()
                                                &&x.TemplateTypeSettingId!=templateType.TemplateTypeSettingId)
                                                .FirstOrDefault();
            if (check != null)
            {
                ModelState.AddModelError("", "you can't add more than one records");
                return Json(new[] { templateType }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                templateType.ModifiedBy = User.Identity.Name;
                templateType.LastModified = DateTime.Now;
                var entity = GetMappedObject(templateType);
                db.TemplateTypeSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Template Type Setting", "Success", User.Identity.Name, "Template Type Setting is Updated By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }

            return Json(new[] { templateType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TemplateTypeSettings_Destroy([DataSourceRequest]DataSourceRequest request, TemplateTypeSetting templateTypeSetting)
        {

            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(templateTypeSetting);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Template Type setting " + templateTypeSetting.TemplateType);
                #endregion
                db.TemplateTypeSettings.Attach(entity);
                db.TemplateTypeSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { templateTypeSetting }.ToDataSourceResult(request, ModelState));
        }

        private TemplateTypeSetting GetMappedObject(TemplateTypeSetting templateType)
        {
            var entity = new TemplateTypeSetting
            {
                TemplateTypeSettingId = templateType.TemplateTypeSettingId,
                TemplateType = templateType.TemplateType,
                OrgBranchName = templateType.OrgBranchName,
                CreatedBy = templateType.CreatedBy,
                DateCreated = templateType.DateCreated,
                LastModified = templateType.LastModified,
                ModifiedBy = templateType.ModifiedBy,

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