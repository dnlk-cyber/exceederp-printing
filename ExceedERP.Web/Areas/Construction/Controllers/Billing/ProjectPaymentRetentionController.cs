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
﻿using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class ProjectPaymentRetentionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Retentions_Read([DataSourceRequest]DataSourceRequest request, int idR)
        {
            IQueryable<ProjectPaymenRetention> retentions = db.Retentions.Where( x=> x.ProjectInformationId==idR);
            DataSourceResult result = retentions.ToDataSourceResult(request, projectPaymenRetention => new {
                ProjectPaymenRetentionId = projectPaymenRetention.ProjectPaymenRetentionId,
                RetentionBeforeVat = projectPaymenRetention.RetentionBeforeVat,
                RetentionAfterVat = projectPaymenRetention.RetentionAfterVat,
                TotalRetentionBeforeVat = projectPaymenRetention.TotalRetentionBeforeVat,
                TotalRetentionAfterVat = projectPaymenRetention.TotalRetentionAfterVat,
                ProjectInformationId = projectPaymenRetention.ProjectInformationId,
                OrgBranchName = projectPaymenRetention.OrgBranchName,
                DateCreated = projectPaymenRetention.DateCreated,
                LastModified = projectPaymenRetention.LastModified,
                CreatedBy = projectPaymenRetention.CreatedBy,
                ModifiedBy = projectPaymenRetention.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Retentions_Create([DataSourceRequest]DataSourceRequest request, ProjectPaymenRetention projectPaymenRetention,int idRTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymenRetention
                {
                    RetentionBeforeVat = projectPaymenRetention.RetentionBeforeVat,
                    RetentionAfterVat = projectPaymenRetention.RetentionAfterVat,
                    TotalRetentionBeforeVat = projectPaymenRetention.TotalRetentionBeforeVat,
                    TotalRetentionAfterVat = projectPaymenRetention.TotalRetentionAfterVat,
                    ProjectInformationId = idRTwo,
                    OrgBranchName = projectPaymenRetention.OrgBranchName,
                    DateCreated = projectPaymenRetention.DateCreated,
                    LastModified = projectPaymenRetention.LastModified,
                    CreatedBy = projectPaymenRetention.CreatedBy,
                    ModifiedBy = projectPaymenRetention.ModifiedBy
                };

                db.Retentions.Add(entity);
                db.SaveChanges();
                projectPaymenRetention.ProjectPaymenRetentionId = entity.ProjectPaymenRetentionId;
            }

            return Json(new[] { projectPaymenRetention }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Retentions_Update([DataSourceRequest]DataSourceRequest request, ProjectPaymenRetention projectPaymenRetention)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymenRetention
                {
                    ProjectPaymenRetentionId = projectPaymenRetention.ProjectPaymenRetentionId,
                    RetentionBeforeVat = projectPaymenRetention.RetentionBeforeVat,
                    RetentionAfterVat = projectPaymenRetention.RetentionAfterVat,
                    TotalRetentionBeforeVat = projectPaymenRetention.TotalRetentionBeforeVat,
                    TotalRetentionAfterVat = projectPaymenRetention.TotalRetentionAfterVat,
                    ProjectInformationId = projectPaymenRetention.ProjectInformationId,
                    OrgBranchName = projectPaymenRetention.OrgBranchName,
                    DateCreated = projectPaymenRetention.DateCreated,
                    LastModified = projectPaymenRetention.LastModified,
                    CreatedBy = projectPaymenRetention.CreatedBy,
                    ModifiedBy = projectPaymenRetention.ModifiedBy
                };

                db.Retentions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectPaymenRetention }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Retentions_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPaymenRetention projectPaymenRetention)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymenRetention
                {
                    ProjectPaymenRetentionId = projectPaymenRetention.ProjectPaymenRetentionId,
                    RetentionBeforeVat = projectPaymenRetention.RetentionBeforeVat,
                    RetentionAfterVat = projectPaymenRetention.RetentionAfterVat,
                    TotalRetentionBeforeVat = projectPaymenRetention.TotalRetentionBeforeVat,
                    TotalRetentionAfterVat = projectPaymenRetention.TotalRetentionAfterVat,
                    ProjectInformationId = projectPaymenRetention.ProjectInformationId,
                    OrgBranchName = projectPaymenRetention.OrgBranchName,
                    DateCreated = projectPaymenRetention.DateCreated,
                    LastModified = projectPaymenRetention.LastModified,
                    CreatedBy = projectPaymenRetention.CreatedBy,
                    ModifiedBy = projectPaymenRetention.ModifiedBy
                };

                db.Retentions.Attach(entity);
                db.Retentions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectPaymenRetention }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
