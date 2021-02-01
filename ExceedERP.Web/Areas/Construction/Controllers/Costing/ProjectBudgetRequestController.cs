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

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectBudgetRequestController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectBudgetRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectBudgetRequest> projectbudgetrequest = db.ProjectBudgetRequest;
            DataSourceResult result = projectbudgetrequest.ToDataSourceResult(request, projectBudgetRequest => new {
                ProjectBudgetRequestId = projectBudgetRequest.ProjectBudgetRequestId,
                RequestedFor = projectBudgetRequest.RequestedFor,
                Requested = projectBudgetRequest.Requested,
                Approved = projectBudgetRequest.Approved,
                Difference = projectBudgetRequest.Difference,
                ProjectDefineBudgetId = projectBudgetRequest.ProjectDefineBudgetId,
                OrgBranchName = projectBudgetRequest.OrgBranchName,
                DateCreated = projectBudgetRequest.DateCreated,
                LastModified = projectBudgetRequest.LastModified,
                CreatedBy = projectBudgetRequest.CreatedBy,
                ModifiedBy = projectBudgetRequest.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetRequest_Create([DataSourceRequest]DataSourceRequest request, ProjectBudgetRequest projectBudgetRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetRequest
                {
                    RequestedFor = projectBudgetRequest.RequestedFor,
                    Requested = projectBudgetRequest.Requested,
                    Approved = projectBudgetRequest.Approved,
                    Difference = projectBudgetRequest.Difference,
                    ProjectDefineBudgetId = projectBudgetRequest.ProjectDefineBudgetId,
                    OrgBranchName = projectBudgetRequest.OrgBranchName,
                    DateCreated = projectBudgetRequest.DateCreated,
                    LastModified = projectBudgetRequest.LastModified,
                    CreatedBy = projectBudgetRequest.CreatedBy,
                    ModifiedBy = projectBudgetRequest.ModifiedBy
                };

                db.ProjectBudgetRequest.Add(entity);
                db.SaveChanges();
                projectBudgetRequest.ProjectBudgetRequestId = entity.ProjectBudgetRequestId;
            }

            return Json(new[] { projectBudgetRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetRequest_Update([DataSourceRequest]DataSourceRequest request, ProjectBudgetRequest projectBudgetRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetRequest
                {
                    ProjectBudgetRequestId = projectBudgetRequest.ProjectBudgetRequestId,
                    RequestedFor = projectBudgetRequest.RequestedFor,
                    Requested = projectBudgetRequest.Requested,
                    Approved = projectBudgetRequest.Approved,
                    Difference = projectBudgetRequest.Difference,
                    ProjectDefineBudgetId = projectBudgetRequest.ProjectDefineBudgetId,
                    OrgBranchName = projectBudgetRequest.OrgBranchName,
                    DateCreated = projectBudgetRequest.DateCreated,
                    LastModified = projectBudgetRequest.LastModified,
                    CreatedBy = projectBudgetRequest.CreatedBy,
                    ModifiedBy = projectBudgetRequest.ModifiedBy
                };

                db.ProjectBudgetRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectBudgetRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetRequest_Destroy([DataSourceRequest]DataSourceRequest request, ProjectBudgetRequest projectBudgetRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetRequest
                {
                    ProjectBudgetRequestId = projectBudgetRequest.ProjectBudgetRequestId,
                    RequestedFor = projectBudgetRequest.RequestedFor,
                    Requested = projectBudgetRequest.Requested,
                    Approved = projectBudgetRequest.Approved,
                    Difference = projectBudgetRequest.Difference,
                    ProjectDefineBudgetId = projectBudgetRequest.ProjectDefineBudgetId,
                    OrgBranchName = projectBudgetRequest.OrgBranchName,
                    DateCreated = projectBudgetRequest.DateCreated,
                    LastModified = projectBudgetRequest.LastModified,
                    CreatedBy = projectBudgetRequest.CreatedBy,
                    ModifiedBy = projectBudgetRequest.ModifiedBy
                };

                db.ProjectBudgetRequest.Attach(entity);
                db.ProjectBudgetRequest.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectBudgetRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
