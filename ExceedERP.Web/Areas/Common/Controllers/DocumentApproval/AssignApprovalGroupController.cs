﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common.DocumentApproval;
 using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Common.Controllers.DocumentApproval
{
    public class AssignApprovalGroupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var jobtitles = db.Lookups;
            var branches = db.Branch;
            var approvalGroups = db.ApprovalGroups;
            ViewData["ApprovalGroups"] = approvalGroups;
            ViewData["Branches"] = branches;
            ViewData["JobTitles"] = jobtitles;
        }

        public ActionResult AssignApprovalGroups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AssignApprovalGroup> assignapprovalgroups = db.AssignApprovalGroups;
            DataSourceResult result = assignapprovalgroups.ToDataSourceResult(request, assignApprovalGroup => new {
                AssignApprovalGroupId = assignApprovalGroup.AssignApprovalGroupId,
                OpertatingUnit = assignApprovalGroup.OpertatingUnit,
                Organization = assignApprovalGroup.Organization,
                Position = assignApprovalGroup.Position,
                Job = assignApprovalGroup.Job
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroups_Create([DataSourceRequest]DataSourceRequest request, AssignApprovalGroup assignApprovalGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroup
                {
                    OpertatingUnit = assignApprovalGroup.OpertatingUnit,
                    Organization = assignApprovalGroup.Organization,
                    Position = assignApprovalGroup.Position,
                    Job = assignApprovalGroup.Job
                };

                db.AssignApprovalGroups.Add(entity);
                db.SaveChanges();
                assignApprovalGroup.AssignApprovalGroupId = entity.AssignApprovalGroupId;
            }

            return Json(new[] { assignApprovalGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroups_Update([DataSourceRequest]DataSourceRequest request, AssignApprovalGroup assignApprovalGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroup
                {
                    AssignApprovalGroupId = assignApprovalGroup.AssignApprovalGroupId,
                    OpertatingUnit = assignApprovalGroup.OpertatingUnit,
                    Organization = assignApprovalGroup.Organization,
                    Position = assignApprovalGroup.Position,
                    Job = assignApprovalGroup.Job
                };

                db.AssignApprovalGroups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assignApprovalGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroups_Destroy([DataSourceRequest]DataSourceRequest request, AssignApprovalGroup assignApprovalGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroup
                {
                    AssignApprovalGroupId = assignApprovalGroup.AssignApprovalGroupId,
                    OpertatingUnit = assignApprovalGroup.OpertatingUnit,
                    Organization = assignApprovalGroup.Organization,
                    Position = assignApprovalGroup.Position,
                    Job = assignApprovalGroup.Job
                };

                db.AssignApprovalGroups.Attach(entity);
                db.AssignApprovalGroups.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assignApprovalGroup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
