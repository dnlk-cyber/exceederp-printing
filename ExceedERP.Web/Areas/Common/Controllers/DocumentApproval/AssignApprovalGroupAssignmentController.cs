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
    public class AssignApprovalGroupAssignmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AssignApprovalGroupAssignments_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AssignApprovalGroupAssignment> assignapprovalgroupassignments = db.AssignApprovalGroupAssignments.Where(x=>x.AssignApprovalGroupId==id);
            DataSourceResult result = assignapprovalgroupassignments.ToDataSourceResult(request, assignApprovalGroupAssignment => new {
                AssignApprovalGroupAssignmentId = assignApprovalGroupAssignment.AssignApprovalGroupAssignmentId,
                AssignApprovalGroupId = assignApprovalGroupAssignment.AssignApprovalGroupId,
                ApprovalGroupId = assignApprovalGroupAssignment.ApprovalGroupId,
                DocumentType = assignApprovalGroupAssignment.DocumentType,
                From = assignApprovalGroupAssignment.From,
                To = assignApprovalGroupAssignment.To
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroupAssignments_Create(int id,[DataSourceRequest]DataSourceRequest request, AssignApprovalGroupAssignment assignApprovalGroupAssignment)
        {

            
            var approvalgroup = db.ApprovalGroups.Find(assignApprovalGroupAssignment.ApprovalGroupId);
            if (approvalgroup != null)
            {
                var document = db.DocumentApprovalRules.Find(approvalgroup.DocumentApprovalRuleId);
                if (document != null) assignApprovalGroupAssignment.DocumentType = document.DocumentType;
            }

            var exist = db.AssignApprovalGroupAssignments
                .Where(x => x.DocumentType == assignApprovalGroupAssignment.DocumentType)
                .Where(x => x.AssignApprovalGroupId == id).ToList();

            if(exist.Any())
                ModelState.AddModelError("exist","Assignment for the same document type already exisits.");

            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroupAssignment
                {
                    AssignApprovalGroupId = id,
                    ApprovalGroupId = assignApprovalGroupAssignment.ApprovalGroupId,
                    DocumentType = assignApprovalGroupAssignment.DocumentType,
                    From = assignApprovalGroupAssignment.From,
                    To = assignApprovalGroupAssignment.To
                };

                db.AssignApprovalGroupAssignments.Add(entity);
                db.SaveChanges();
                assignApprovalGroupAssignment.AssignApprovalGroupAssignmentId = entity.AssignApprovalGroupAssignmentId;
            }

            return Json(new[] { assignApprovalGroupAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroupAssignments_Update([DataSourceRequest]DataSourceRequest request, AssignApprovalGroupAssignment assignApprovalGroupAssignment)
        {
            var approvalgroup = db.ApprovalGroups.Find(assignApprovalGroupAssignment.ApprovalGroupId);
            if (approvalgroup != null)
            {
                var document = db.DocumentApprovalRules.Find(approvalgroup.DocumentApprovalRuleId);
                if (document != null) assignApprovalGroupAssignment.DocumentType = document.DocumentType;
            }

            var exist = db.AssignApprovalGroupAssignments
                .Where(x => x.AssignApprovalGroupAssignmentId != assignApprovalGroupAssignment.AssignApprovalGroupAssignmentId)
                .Where(x => x.DocumentType == assignApprovalGroupAssignment.DocumentType)
                .Where(x => x.AssignApprovalGroupId == assignApprovalGroupAssignment.AssignApprovalGroupId).ToList();

            if (exist.Any())
                ModelState.AddModelError("exist", "Assignment for the same document type already exisits.");
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroupAssignment
                {
                    AssignApprovalGroupAssignmentId = assignApprovalGroupAssignment.AssignApprovalGroupAssignmentId,
                    AssignApprovalGroupId = assignApprovalGroupAssignment.AssignApprovalGroupId,
                    ApprovalGroupId = assignApprovalGroupAssignment.ApprovalGroupId,
                    DocumentType = assignApprovalGroupAssignment.DocumentType,
                    From = assignApprovalGroupAssignment.From,
                    To = assignApprovalGroupAssignment.To
                };

                db.AssignApprovalGroupAssignments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assignApprovalGroupAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroupAssignments_Destroy([DataSourceRequest]DataSourceRequest request, AssignApprovalGroupAssignment assignApprovalGroupAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroupAssignment
                {
                    AssignApprovalGroupAssignmentId = assignApprovalGroupAssignment.AssignApprovalGroupAssignmentId,
                    AssignApprovalGroupId = assignApprovalGroupAssignment.AssignApprovalGroupId,
                    ApprovalGroupId = assignApprovalGroupAssignment.ApprovalGroupId,
                    DocumentType = assignApprovalGroupAssignment.DocumentType,
                    From = assignApprovalGroupAssignment.From,
                    To = assignApprovalGroupAssignment.To
                };

                db.AssignApprovalGroupAssignments.Attach(entity);
                db.AssignApprovalGroupAssignments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assignApprovalGroupAssignment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
