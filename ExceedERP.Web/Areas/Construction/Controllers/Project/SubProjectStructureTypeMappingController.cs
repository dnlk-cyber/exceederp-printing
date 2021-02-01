using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class SubProjectStructureTypeMappingController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult SubProjectStructureTypeMappings_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SubProjectStructureTypeMapping> mappings = db.SubProjectStructureTypeMapping.Where(x => x.SubProjectTypeId == id);
            DataSourceResult result = mappings.ToDataSourceResult(request, mapping => new
            {
                SubProjectStructureTypeMappingId = mapping.SubProjectStructureTypeMappingId,
                ConfigurationId = mapping.ConfigurationId,
                SubProjectTypeId = mapping.SubProjectTypeId,
                Remark = mapping.Remark
            });

            return Json(result);
        }

        private bool ValueExists(SubProjectStructureTypeMapping model, int subProjectId)
        {
            bool valueExists = false;
            var temp = db.SubProjectStructureTypeMapping.Where(m => subProjectId == m.SubProjectTypeId && m.ConfigurationId == model.ConfigurationId).FirstOrDefault();
            if (temp != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubProjectStructureTypeMapping_Create([DataSourceRequest]DataSourceRequest request, SubProjectStructureTypeMapping mapping, int idTwo)
        {
            if (ModelState.IsValid)
            {
                if (ValueExists(mapping, idTwo))
                {
                    ModelState.AddModelError("", "Duplication of structure types is not allowed!!");
                }
                else
                {
                    var entity = new SubProjectStructureTypeMapping
                    {
                        ConfigurationId = mapping.ConfigurationId,
                        SubProjectTypeId = idTwo,
                        Remark = mapping.Remark
                    };

                    db.SubProjectStructureTypeMapping.Add(entity);
                    db.SaveChanges();
                    mapping.SubProjectStructureTypeMappingId = entity.SubProjectStructureTypeMappingId;
                }

            }

            return Json(new[] { mapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubProjectStructureTypeMapping_Update([DataSourceRequest]DataSourceRequest request, SubProjectStructureTypeMapping mapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubProjectStructureTypeMapping
                {
                    SubProjectStructureTypeMappingId = mapping.SubProjectStructureTypeMappingId,
                    ConfigurationId = mapping.ConfigurationId,
                    SubProjectTypeId = mapping.SubProjectTypeId,
                    Remark = mapping.Remark
                };

                db.SubProjectStructureTypeMapping.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { mapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubProjectStructureTypeMapping_Destroy([DataSourceRequest]DataSourceRequest request, SubProjectStructureTypeMapping mapping)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubProjectStructureTypeMapping
                {
                    SubProjectStructureTypeMappingId = mapping.SubProjectStructureTypeMappingId,
                    ConfigurationId = mapping.ConfigurationId,
                    SubProjectTypeId = mapping.SubProjectTypeId,
                    Remark = mapping.Remark
                };

                db.SubProjectStructureTypeMapping.Attach(entity);
                db.SubProjectStructureTypeMapping.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { mapping }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}