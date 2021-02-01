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
    public class ProjectBudgetTransferController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectBudgetTransfer_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectBudgetTransfer> projectbudgettransfer = db.ProjectBudgetTransfer;
            DataSourceResult result = projectbudgettransfer.ToDataSourceResult(request, projectBudgetTransfer => new {
                ProjectBudgetTransferId = projectBudgetTransfer.ProjectBudgetTransferId,
                PreviousTransferred = projectBudgetTransfer.PreviousTransferred,
                CurrentPeriodTransferred = projectBudgetTransfer.CurrentPeriodTransferred,
                ProjectDefineBudgetId = projectBudgetTransfer.ProjectDefineBudgetId,
                OrgBranchName = projectBudgetTransfer.OrgBranchName,
                DateCreated = projectBudgetTransfer.DateCreated,
                LastModified = projectBudgetTransfer.LastModified,
                CreatedBy = projectBudgetTransfer.CreatedBy,
                ModifiedBy = projectBudgetTransfer.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetTransfer_Create([DataSourceRequest]DataSourceRequest request, ProjectBudgetTransfer projectBudgetTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetTransfer
                {
                    PreviousTransferred = projectBudgetTransfer.PreviousTransferred,
                    CurrentPeriodTransferred = projectBudgetTransfer.CurrentPeriodTransferred,
                    ProjectDefineBudgetId = projectBudgetTransfer.ProjectDefineBudgetId,
                    OrgBranchName = projectBudgetTransfer.OrgBranchName,
                    DateCreated = projectBudgetTransfer.DateCreated,
                    LastModified = projectBudgetTransfer.LastModified,
                    CreatedBy = projectBudgetTransfer.CreatedBy,
                    ModifiedBy = projectBudgetTransfer.ModifiedBy
                };

                db.ProjectBudgetTransfer.Add(entity);
                db.SaveChanges();
                projectBudgetTransfer.ProjectBudgetTransferId = entity.ProjectBudgetTransferId;
            }

            return Json(new[] { projectBudgetTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetTransfer_Update([DataSourceRequest]DataSourceRequest request, ProjectBudgetTransfer projectBudgetTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetTransfer
                {
                    ProjectBudgetTransferId = projectBudgetTransfer.ProjectBudgetTransferId,
                    PreviousTransferred = projectBudgetTransfer.PreviousTransferred,
                    CurrentPeriodTransferred = projectBudgetTransfer.CurrentPeriodTransferred,
                    ProjectDefineBudgetId = projectBudgetTransfer.ProjectDefineBudgetId,
                    OrgBranchName = projectBudgetTransfer.OrgBranchName,
                    DateCreated = projectBudgetTransfer.DateCreated,
                    LastModified = projectBudgetTransfer.LastModified,
                    CreatedBy = projectBudgetTransfer.CreatedBy,
                    ModifiedBy = projectBudgetTransfer.ModifiedBy
                };

                db.ProjectBudgetTransfer.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectBudgetTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetTransfer_Destroy([DataSourceRequest]DataSourceRequest request, ProjectBudgetTransfer projectBudgetTransfer)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetTransfer
                {
                    ProjectBudgetTransferId = projectBudgetTransfer.ProjectBudgetTransferId,
                    PreviousTransferred = projectBudgetTransfer.PreviousTransferred,
                    CurrentPeriodTransferred = projectBudgetTransfer.CurrentPeriodTransferred,
                    OrgBranchName = projectBudgetTransfer.OrgBranchName,
                    DateCreated = projectBudgetTransfer.DateCreated,
                    LastModified = projectBudgetTransfer.LastModified,
                    CreatedBy = projectBudgetTransfer.CreatedBy,
                    ModifiedBy = projectBudgetTransfer.ModifiedBy
                };

                db.ProjectBudgetTransfer.Attach(entity);
                db.ProjectBudgetTransfer.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectBudgetTransfer }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
