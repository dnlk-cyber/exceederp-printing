using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class ProjectTypeMappingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var types = db.Configurations
                .Where(x => x.Reference == "STRUCTURE_TYPE")
                .Select(c => new { c.ConfigurationId, c.CurrentValue}).ToList();
            ViewData["Types"] = types;
            return View();
        }

        public ActionResult ProjectTypeMappings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectTypeMapping> projectTypeMappings = db.ProjectTypeMappings;
            DataSourceResult result = projectTypeMappings.ToDataSourceResult(request, projectTypeMapping => new
            {
                ProjectTypeMappingId = projectTypeMapping.ProjectTypeMappingId,
                CodeOrSeries = projectTypeMapping.CodeOrSeries,
                ConfigurationId = projectTypeMapping.ConfigurationId,
                ProjectType = projectTypeMapping.ProjectType,

                Remark = projectTypeMapping.Remark
            });

            return Json(result);
        }

        private bool TypeMappingExists(AvailableProjectTypes type, int strId)
        {
            bool exists = false;
            var temp = db.ProjectTypeMappings.Where(tm => tm.ProjectType == type && tm.ConfigurationId == strId).FirstOrDefault();
            if (temp != null)
            {
                exists = true;
            }
            return exists;
        }
        private bool CodeExists(ProjectTypeMapping model)
        {
            var codeExists = false;
            var temp = db.ProjectTypeMappings.FirstOrDefault(m => m.ProjectType == model.ProjectType && m.CodeOrSeries == model.CodeOrSeries);
            if (temp != null)
            {
                codeExists = true;
            }
            return codeExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTypeMappings_Create([DataSourceRequest]DataSourceRequest request, ProjectTypeMapping projectTypeMapping)
        {
            var errorExists = false;
            if (ModelState.IsValid)
            {
                if (TypeMappingExists(projectTypeMapping.ProjectType, projectTypeMapping.ConfigurationId))
                {
                    ModelState.AddModelError("", "This Mapping is Registered already!!!");
                    errorExists = true;
                }
                else if (CodeExists(projectTypeMapping) && projectTypeMapping.ProjectType == AvailableProjectTypes.Road)
                {
                    ModelState.AddModelError("", "Code/Series should be unique for each project type!");
                    errorExists = true;
                }

                if (!errorExists)
                {
                    var entity = new ProjectTypeMapping
                    {
                        //ProjectTypeMappingId = projectTypeMapping.ProjectTypeMappingId,
                        CodeOrSeries = projectTypeMapping.CodeOrSeries,
                        ConfigurationId = projectTypeMapping.ConfigurationId,
                        ProjectType = projectTypeMapping.ProjectType,
                        Remark = projectTypeMapping.Remark
                    };

                    db.ProjectTypeMappings.Add(entity);
                    db.SaveChanges();
                    projectTypeMapping.ProjectTypeMappingId = entity.ProjectTypeMappingId;
                }

            }


            return Json(new[] { projectTypeMapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTypeMappings_Update([DataSourceRequest]DataSourceRequest request, ProjectTypeMapping projectTypeMapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTypeMapping
                {
                    ProjectTypeMappingId = projectTypeMapping.ProjectTypeMappingId,
                    CodeOrSeries = projectTypeMapping.CodeOrSeries,
                    ConfigurationId = projectTypeMapping.ConfigurationId,
                    ProjectType = projectTypeMapping.ProjectType,
                    Remark = projectTypeMapping.Remark
                };

                db.ProjectTypeMappings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectTypeMapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTypeMappings_Destroy([DataSourceRequest]DataSourceRequest request, ProjectTypeMapping projectTypeMapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTypeMapping
                {
                    ProjectTypeMappingId = projectTypeMapping.ProjectTypeMappingId,
                    CodeOrSeries = projectTypeMapping.CodeOrSeries,
                    ConfigurationId = projectTypeMapping.ConfigurationId,
                    ProjectType = projectTypeMapping.ProjectType,
                    Remark = projectTypeMapping.Remark
                };

                db.ProjectTypeMappings.Attach(entity);
                db.ProjectTypeMappings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectTypeMapping }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
