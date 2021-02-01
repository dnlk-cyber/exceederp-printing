using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction.Extensions;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    public class WorkCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            ViewData["StructureType"] = lis;
            return View();
        }

        public ActionResult WorkCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<WorkCategory> workcategory = db.WorkCategory;
            DataSourceResult result = workcategory.ToDataSourceResult(request, workCategory => new
            {
                WorkCategoryId = workCategory.WorkCategoryId,
                Name = workCategory.Name,
                Description = workCategory.Description,
                ConfigurationId = workCategory.ConfigurationId,
                Index = workCategory.Index
            });

            return Json(result);
        }
        public ActionResult WorkCategoryPerStructure_Read([DataSourceRequest]DataSourceRequest request, int stId)
        {
            IQueryable<WorkCategory> workcategory = db.WorkCategory.Where(wc => wc.ConfigurationId == stId);
            DataSourceResult result = workcategory.ToDataSourceResult(request, workCategory => new
            {
                WorkCategoryId = workCategory.WorkCategoryId,
                Name = workCategory.Name,
                Description = workCategory.Description,
                ConfigurationId = workCategory.ConfigurationId,
                Index = workCategory.Index
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkCategory_Create([DataSourceRequest]DataSourceRequest request, WorkCategory workCategory)
        {
            if (ModelState.IsValid)
            {
                //var category = db.WorkCategory.Where(x => x.ConfigurationId == workCategory.ConfigurationId && x.Index == workCategory.Index).FirstOrDefault();
                //if (category == null)
                //{
                if (IsInvalidCode(workCategory))
                {
                    ModelState.AddModelError("", " Index/Code doesn't seem to fall with in the range!");
                }
                else
                {
                    var entity = new WorkCategory
                    {
                        Name = !string.IsNullOrEmpty(workCategory.Name) ? workCategory.Name.RemoveUnnecessaryCharactors() : "",
                        Description = workCategory.Description,
                        ConfigurationId = workCategory.ConfigurationId,
                        Index = workCategory.Index
                    };

                    db.WorkCategory.Add(entity);
                    db.SaveChanges();
                    workCategory.WorkCategoryId = entity.WorkCategoryId;
                }

                //}
                //else
                //{
                //    ModelState.AddModelError("", "This Index value is already registered for this structure type.\n Please change it!!");
                //}
            }

            return Json(new[] { workCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkCategory_Update([DataSourceRequest]DataSourceRequest request, WorkCategory workCategory)
        {
            if (ModelState.IsValid)
            {
                //var category = db.WorkCategory.Where(x => x.ConfigurationId == workCategory.ConfigurationId && x.Index == workCategory.Index).FirstOrDefault();
                //if (category == null)
                //{
                var entity = new WorkCategory
                {
                    WorkCategoryId = workCategory.WorkCategoryId,
                    Name = !string.IsNullOrEmpty(workCategory.Name) ? workCategory.Name.RemoveUnnecessaryCharactors() : "",
                    Description = workCategory.Description,
                    ConfigurationId = workCategory.ConfigurationId,
                    Index = workCategory.Index
                };

                //var local = db.Set<WorkCategory>()
                //         .Local
                //         .FirstOrDefault(f => f.WorkCategoryId == workCategory.WorkCategoryId);
                //if (local != null)
                //{
                //    db.Entry(local).State = EntityState.Detached;
                //}
                //db.Entry(entity).State = EntityState.Modified;

                db.WorkCategory.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                //}

            }

            return Json(new[] { workCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkCategory_Destroy([DataSourceRequest]DataSourceRequest request, WorkCategory workCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkCategory
                {
                    WorkCategoryId = workCategory.WorkCategoryId,
                    Name = workCategory.Name,
                    Description = workCategory.Description,
                    ConfigurationId = workCategory.ConfigurationId,
                    Index = workCategory.Index
                };

                db.WorkCategory.Attach(entity);
                db.WorkCategory.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { workCategory }.ToDataSourceResult(request, ModelState));
        }
        private int GetStructureTypeCode(int configId)
        {
            var indexValue = 0;
            var currentMapping = db.ProjectTypeMappings.FirstOrDefault(m => m.ConfigurationId == configId && m.ProjectType == AvailableProjectTypes.Road);
            if (currentMapping != null && currentMapping.CodeOrSeries != null)
            {
                var categories = db.WorkCategory.Where(c => c.ConfigurationId == currentMapping.ConfigurationId).ToList();
                if (categories.Any())
                {
                    indexValue = categories.Max(wc => wc.Index) + 100;
                }
                else
                {
                    indexValue = (int)currentMapping.CodeOrSeries + 100;
                }
            }
            return indexValue;
        }
        private bool IsInvalidCode(WorkCategory model)
        {
            bool IsInvalid = false;
            var currentMapping = db.ProjectTypeMappings.FirstOrDefault(m => m.ConfigurationId == model.ConfigurationId && m.ProjectType == AvailableProjectTypes.Road);
            if (currentMapping != null && currentMapping.CodeOrSeries != null)
            {
                // check if the assigned index is with in the ragne that is assigned to the top level
                // structure in project mapping
                if (model.Index <= currentMapping.CodeOrSeries || model.Index >= (currentMapping.CodeOrSeries + 1000))
                {
                    IsInvalid = true;
                }
            }

            return IsInvalid;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
