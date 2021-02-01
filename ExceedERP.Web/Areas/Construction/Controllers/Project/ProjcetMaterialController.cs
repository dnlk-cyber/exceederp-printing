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

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjcetMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectMaterial_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectMaterial> projectmaterial = db.ProjectMaterial;
            DataSourceResult result = projectmaterial.ToDataSourceResult(request, projectMaterial => new {
                ProjectMaterialId = projectMaterial.ProjectMaterialId,
                Unit = projectMaterial.Unit,
                Quantity = projectMaterial.Quantity,
                UnitPrice = projectMaterial.UnitPrice,
                TotalCost = projectMaterial.TotalCost,
                MaterialUnitCost = projectMaterial.MaterialUnitCost,
                OrgBranchName = projectMaterial.OrgBranchName,
                DateCreated = projectMaterial.DateCreated,
                LastModified = projectMaterial.LastModified,
                CreatedBy = projectMaterial.CreatedBy,
                ModifiedBy = projectMaterial.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Create([DataSourceRequest]DataSourceRequest request, ProjectMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMaterial
                {
                    Unit = projectMaterial.Unit,
                    Quantity = projectMaterial.Quantity,
                    UnitPrice = projectMaterial.UnitPrice,
                    TotalCost = projectMaterial.TotalCost,
                    MaterialUnitCost = projectMaterial.MaterialUnitCost,
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectMaterial.LastModified,
                    CreatedBy =User.Identity .Name ,
                    ModifiedBy = projectMaterial.ModifiedBy 
                };

                db.ProjectMaterial.Add(entity);
                db.SaveChanges();
                projectMaterial.ProjectMaterialId = entity.ProjectMaterialId;
            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Update([DataSourceRequest]DataSourceRequest request, ProjectMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMaterial
                {
                    ProjectMaterialId = projectMaterial.ProjectMaterialId,
                    Unit = projectMaterial.Unit,
                    Quantity = projectMaterial.Quantity,
                    UnitPrice = projectMaterial.UnitPrice,
                    TotalCost = projectMaterial.TotalCost,
                    MaterialUnitCost = projectMaterial.MaterialUnitCost,
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = projectMaterial.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectMaterial.CreatedBy,
                    ModifiedBy =User.Identity .Name 
                };

                db.ProjectMaterial.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Destroy([DataSourceRequest]DataSourceRequest request, ProjectMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMaterial
                {
                    ProjectMaterialId = projectMaterial.ProjectMaterialId,
                    Unit = projectMaterial.Unit,
                    Quantity = projectMaterial.Quantity,
                    UnitPrice = projectMaterial.UnitPrice,
                    TotalCost = projectMaterial.TotalCost,
                    MaterialUnitCost = projectMaterial.MaterialUnitCost,
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = projectMaterial.DateCreated,
                    LastModified = projectMaterial.LastModified,
                    CreatedBy = projectMaterial.CreatedBy,
                    ModifiedBy = projectMaterial.ModifiedBy 
                };

                db.ProjectMaterial.Attach(entity);
                db.ProjectMaterial.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
