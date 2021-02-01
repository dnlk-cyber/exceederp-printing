﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class MaterialDemandController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MaterialDemands_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaterialDemand> materialdemands = db.MaterialDemands;
            DataSourceResult result = materialdemands.ToDataSourceResult(request, materialDemand => new {
                MaterialDemandId = materialDemand.MaterialDemandId,
                AdditionalDemand = materialDemand.AdditionalDemand,
                Remark = materialDemand.Remark,
                OrgBranchName = materialDemand.OrgBranchName,
                DateCreated = materialDemand.DateCreated,
                LastModified = materialDemand.LastModified,
                CreatedBy = materialDemand.CreatedBy,
                ModifiedBy = materialDemand.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialDemands_Create([DataSourceRequest]DataSourceRequest request, MaterialDemand materialDemand)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialDemand
                {
                    AdditionalDemand = materialDemand.AdditionalDemand,
                    Remark = materialDemand.Remark,
                    OrgBranchName = materialDemand.OrgBranchName,
                    DateCreated = materialDemand.DateCreated,
                    LastModified = materialDemand.LastModified,
                    CreatedBy = materialDemand.CreatedBy,
                    ModifiedBy = materialDemand.ModifiedBy
                };

                db.MaterialDemands.Add(entity);
                db.SaveChanges();
                materialDemand.MaterialDemandId = entity.MaterialDemandId;
            }

            return Json(new[] { materialDemand }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialDemands_Update([DataSourceRequest]DataSourceRequest request, MaterialDemand materialDemand)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialDemand
                {
                    MaterialDemandId = materialDemand.MaterialDemandId,
                    AdditionalDemand = materialDemand.AdditionalDemand,
                    Remark = materialDemand.Remark,
                    OrgBranchName = materialDemand.OrgBranchName,
                    DateCreated = materialDemand.DateCreated,
                    LastModified = materialDemand.LastModified,
                    CreatedBy = materialDemand.CreatedBy,
                    ModifiedBy = materialDemand.ModifiedBy
                };

                db.MaterialDemands.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { materialDemand }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialDemands_Destroy([DataSourceRequest]DataSourceRequest request, MaterialDemand materialDemand)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialDemand
                {
                    MaterialDemandId = materialDemand.MaterialDemandId,
                    AdditionalDemand = materialDemand.AdditionalDemand,
                    Remark = materialDemand.Remark,
                    OrgBranchName = materialDemand.OrgBranchName,
                    DateCreated = materialDemand.DateCreated,
                    LastModified = materialDemand.LastModified,
                    CreatedBy = materialDemand.CreatedBy,
                    ModifiedBy = materialDemand.ModifiedBy
                };

                db.MaterialDemands.Attach(entity);
                db.MaterialDemands.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { materialDemand }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
