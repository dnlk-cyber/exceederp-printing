﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
﻿using System.Web.Helpers;

﻿using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
﻿using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectBudgetUtilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var projects = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED).ToList();
            ViewData["ProjectList"] = projects;
            return View();
        }
       
       
        public ActionResult ProjectBudgetUtilization_Read([DataSourceRequest]DataSourceRequest request)
        {
           
            IQueryable<ProjectBudgetUtilization> projectbudgetutilization = db.ProjectBudgetUtilization;
            DataSourceResult result = projectbudgetutilization.ToDataSourceResult(request, projectBudgetUtilization => new {
                ProjectBudgetUtilizationId = projectBudgetUtilization.ProjectBudgetUtilizationId,
                RequestedFor = projectBudgetUtilization.RequestedFor,
                Transferred = projectBudgetUtilization.Transferred,
                Utilized = projectBudgetUtilization.Utilized,
                Difference = projectBudgetUtilization.Difference,
                ProjectDefineBudgetId = projectBudgetUtilization.ProjectDefineBudgetId,
                OrgBranchName = projectBudgetUtilization.OrgBranchName,
                DateCreated = projectBudgetUtilization.DateCreated,
                LastModified = projectBudgetUtilization.LastModified,
                CreatedBy = projectBudgetUtilization.CreatedBy,
                ModifiedBy = projectBudgetUtilization.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetUtilization_Create([DataSourceRequest]DataSourceRequest request, ProjectBudgetUtilization projectBudgetUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetUtilization
                {
                    RequestedFor = projectBudgetUtilization.RequestedFor,
                    Transferred = projectBudgetUtilization.Transferred,
                    Utilized = projectBudgetUtilization.Utilized,
                    Difference = projectBudgetUtilization.Difference,
                    ProjectDefineBudgetId = projectBudgetUtilization.ProjectDefineBudgetId,
                    OrgBranchName = projectBudgetUtilization.OrgBranchName,
                    DateCreated = projectBudgetUtilization.DateCreated,
                    LastModified = projectBudgetUtilization.LastModified,
                    CreatedBy = projectBudgetUtilization.CreatedBy,
                    ModifiedBy = projectBudgetUtilization.ModifiedBy
                };

                db.ProjectBudgetUtilization.Add(entity);
                db.SaveChanges();
                projectBudgetUtilization.ProjectBudgetUtilizationId = entity.ProjectBudgetUtilizationId;
            }

            return Json(new[] { projectBudgetUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetUtilization_Update([DataSourceRequest]DataSourceRequest request, ProjectBudgetUtilization projectBudgetUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetUtilization
                {
                    ProjectBudgetUtilizationId = projectBudgetUtilization.ProjectBudgetUtilizationId,
                    RequestedFor = projectBudgetUtilization.RequestedFor,
                    Transferred = projectBudgetUtilization.Transferred,
                    Utilized = projectBudgetUtilization.Utilized,
                    Difference = projectBudgetUtilization.Difference,
                    ProjectDefineBudgetId = projectBudgetUtilization.ProjectDefineBudgetId,
                    OrgBranchName = projectBudgetUtilization.OrgBranchName,
                    DateCreated = projectBudgetUtilization.DateCreated,
                    LastModified = projectBudgetUtilization.LastModified,
                    CreatedBy = projectBudgetUtilization.CreatedBy,
                    ModifiedBy = projectBudgetUtilization.ModifiedBy
                };

                db.ProjectBudgetUtilization.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectBudgetUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectBudgetUtilization_Destroy([DataSourceRequest]DataSourceRequest request, ProjectBudgetUtilization projectBudgetUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectBudgetUtilization
                {
                    ProjectBudgetUtilizationId = projectBudgetUtilization.ProjectBudgetUtilizationId,
                    RequestedFor = projectBudgetUtilization.RequestedFor,
                    Transferred = projectBudgetUtilization.Transferred,
                    Utilized = projectBudgetUtilization.Utilized,
                    Difference = projectBudgetUtilization.Difference,
                    ProjectDefineBudgetId = projectBudgetUtilization.ProjectDefineBudgetId,
                    OrgBranchName = projectBudgetUtilization.OrgBranchName,
                    DateCreated = projectBudgetUtilization.DateCreated,
                    LastModified = projectBudgetUtilization.LastModified,
                    CreatedBy = projectBudgetUtilization.CreatedBy,
                    ModifiedBy = projectBudgetUtilization.ModifiedBy
                };

                db.ProjectBudgetUtilization.Attach(entity);
                db.ProjectBudgetUtilization.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectBudgetUtilization }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
