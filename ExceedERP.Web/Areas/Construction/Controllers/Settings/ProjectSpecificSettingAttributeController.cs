using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class ProjectSpecificSettingAttributeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult SettingAttributes_Read([DataSourceRequest]DataSourceRequest request, int idd)
        {
            IQueryable<ProjectSpecificSettingAttribute> attributes = db.ProjectSpecificSettingAttributes.Where(x => x.ProjectInformationId == idd);
            DataSourceResult result = attributes.ToDataSourceResult(request, settingAttribute => new {
                ProjectSpecificSettingAttributeId = settingAttribute.ProjectSpecificSettingAttributeId,
                ProjectInformationId = settingAttribute.ProjectInformationId,
                Attribute = settingAttribute.Attribute,
                Value = settingAttribute.Value,

                OrgBranchName = settingAttribute.OrgBranchName,
                DateCreated = settingAttribute.DateCreated,
                LastModified = settingAttribute.LastModified,
                CreatedBy = settingAttribute.CreatedBy,
                ModifiedBy = settingAttribute.ModifiedBy
            });

            return Json(result);
        }

        private bool ValueExists(ProjectSpecificSettingAttribute model, int projectId)
        {
            bool valueExists = false;
            var temp = db.ProjectSpecificSettingAttributes.FirstOrDefault(a => a.Attribute == model.Attribute && a.ProjectInformationId == projectId);
            if (temp != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SettingAttribute_Create([DataSourceRequest]DataSourceRequest request,ProjectSpecificSettingAttribute settingAttribute, int iddTwo)
        {
            if (ModelState.IsValid)
            {
                if (ValueExists(settingAttribute, iddTwo))
                {
                    ModelState.AddModelError("", "Value Exists!!");
                }
                else
                {
                    var entity = new ProjectSpecificSettingAttribute
                    {
                        ProjectSpecificSettingAttributeId = settingAttribute.ProjectSpecificSettingAttributeId,
                        ProjectInformationId = iddTwo,
                        Attribute = settingAttribute.Attribute,
                        Value = settingAttribute.Value,

                        OrgBranchName = settingAttribute.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = settingAttribute.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = settingAttribute.ModifiedBy
                    };

                    db.ProjectSpecificSettingAttributes.Add(entity);
                    db.SaveChanges();
                    settingAttribute.ProjectSpecificSettingAttributeId = entity.ProjectSpecificSettingAttributeId;

                    ConsSettingHelper.UpdateProjectAfterSettingChanges(entity.ProjectInformationId);
                }
            }

            return Json(new[] { settingAttribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SettingAttribute_Update([DataSourceRequest]DataSourceRequest request, ProjectSpecificSettingAttribute settingAttribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectSpecificSettingAttribute
                {
                    ProjectSpecificSettingAttributeId = settingAttribute.ProjectSpecificSettingAttributeId,
                    ProjectInformationId = settingAttribute.ProjectInformationId,
                    Attribute = settingAttribute.Attribute,
                    Value = settingAttribute.Value,

                    OrgBranchName = settingAttribute.OrgBranchName,
                    DateCreated = settingAttribute.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = settingAttribute.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProjectSpecificSettingAttributes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ConsSettingHelper.UpdateProjectAfterSettingChanges(entity.ProjectInformationId);
            }

            return Json(new[] { settingAttribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SettingAttribute_Destroy([DataSourceRequest]DataSourceRequest request, ProjectSpecificSettingAttribute settingAttribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectSpecificSettingAttribute
                {
                    ProjectSpecificSettingAttributeId = settingAttribute.ProjectSpecificSettingAttributeId,
                    ProjectInformationId = settingAttribute.ProjectInformationId,
                    Attribute = settingAttribute.Attribute,
                    Value = settingAttribute.Value,

                    OrgBranchName = settingAttribute.OrgBranchName,
                    DateCreated = settingAttribute.DateCreated,
                    LastModified = settingAttribute.LastModified,
                    CreatedBy = settingAttribute.CreatedBy,
                    ModifiedBy = settingAttribute.ModifiedBy
                };

                db.ProjectSpecificSettingAttributes.Attach(entity);
                db.ProjectSpecificSettingAttributes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { settingAttribute }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}