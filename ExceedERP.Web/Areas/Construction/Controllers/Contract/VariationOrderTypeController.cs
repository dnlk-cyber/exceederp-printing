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

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class VariationOrderTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/VariationOrderType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VariationOrderType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<VariationOrderType> changeOrderTypes = db.VariationOrderType;
            DataSourceResult result = changeOrderTypes.ToDataSourceResult(request, variationType => new {
                VariationOrderTypeId = variationType.VariationOrderTypeId,
                Name = variationType.Name,
                Type = variationType.Type,
                Description = variationType.Description,

                OrgBranchName = variationType.OrgBranchName,
                DateCreated = variationType.DateCreated,
                LastModified = variationType.LastModified,
                CreatedBy = variationType.CreatedBy,
                ModifiedBy = variationType.ModifiedBy
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VariationOrderType_Create([DataSourceRequest]DataSourceRequest request, VariationOrderType variationType)
        {
            if (ModelState.IsValid)
            {

                var entity = new VariationOrderType
                {
                    VariationOrderTypeId = variationType.VariationOrderTypeId,
                    Name = variationType.Name,
                    Type = variationType.Type,
                    Description = variationType.Description,

                    OrgBranchName = variationType.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = variationType.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = variationType.ModifiedBy
                };

                db.VariationOrderType.Add(entity);
                db.SaveChanges();
                variationType.VariationOrderTypeId = entity.VariationOrderTypeId;
            }

            return Json(new[] { variationType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VariationOrderType_Update([DataSourceRequest]DataSourceRequest request, VariationOrderType variationType)
        {
            if (ModelState.IsValid)
            {
                var entity = new VariationOrderType
                {
                    VariationOrderTypeId = variationType.VariationOrderTypeId,
                    Name = variationType.Name,
                    Type = variationType.Type,
                    Description = variationType.Description,

                    OrgBranchName = variationType.OrgBranchName,
                    DateCreated = variationType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = variationType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.VariationOrderType.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { variationType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VariationOrderType_Destroy([DataSourceRequest]DataSourceRequest request, VariationOrderType variationType)
        {
            if (ModelState.IsValid)
            {
                var entity = new VariationOrderType
                {
                    VariationOrderTypeId = variationType.VariationOrderTypeId,
                    Name = variationType.Name,
                    Type = variationType.Type,
                    Description = variationType.Description,

                    OrgBranchName = variationType.OrgBranchName,
                    DateCreated = variationType.DateCreated,
                    LastModified = variationType.LastModified,
                    CreatedBy = variationType.CreatedBy,
                    ModifiedBy = variationType.ModifiedBy
                };

                db.VariationOrderType.Attach(entity);
                db.VariationOrderType.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { variationType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}