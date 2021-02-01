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
    public class ApprovalGroupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApprovalGroups_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApprovalGroup> approvalgroups = db.ApprovalGroups.Where(x=>x.DocumentApprovalRuleId==id);
            DataSourceResult result = approvalgroups.ToDataSourceResult(request, approvalGroup => new {
                DocumentApprovalRuleId = approvalGroup.DocumentApprovalRuleId,
                ApprovalGroupId = approvalGroup.ApprovalGroupId,
                OpertatingUnit = approvalGroup.OpertatingUnit,
                Name = approvalGroup.Name,
                Enabled = approvalGroup.Enabled,
                Description = approvalGroup.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroups_Create(int id,[DataSourceRequest]DataSourceRequest request, ApprovalGroup approvalGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroup
                {
                    DocumentApprovalRuleId=id,
                    OpertatingUnit = approvalGroup.OpertatingUnit,
                    Name = approvalGroup.Name,
                    Enabled = approvalGroup.Enabled,
                    Description = approvalGroup.Description
                };

                db.ApprovalGroups.Add(entity);
                db.SaveChanges();
                approvalGroup.ApprovalGroupId = entity.ApprovalGroupId;
            }

            return Json(new[] { approvalGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroups_Update([DataSourceRequest]DataSourceRequest request, ApprovalGroup approvalGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroup
                {
                    DocumentApprovalRuleId=approvalGroup.DocumentApprovalRuleId,
                    ApprovalGroupId = approvalGroup.ApprovalGroupId,
                    OpertatingUnit = approvalGroup.OpertatingUnit,
                    Name = approvalGroup.Name,
                    Enabled = approvalGroup.Enabled,
                    Description = approvalGroup.Description
                };

                db.ApprovalGroups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { approvalGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroups_Destroy([DataSourceRequest]DataSourceRequest request, ApprovalGroup approvalGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroup
                {
                    DocumentApprovalRuleId = approvalGroup.DocumentApprovalRuleId,
                    ApprovalGroupId = approvalGroup.ApprovalGroupId,
                    OpertatingUnit = approvalGroup.OpertatingUnit,
                    Name = approvalGroup.Name,
                    Enabled = approvalGroup.Enabled,
                    Description = approvalGroup.Description
                };

                db.ApprovalGroups.Attach(entity);
                db.ApprovalGroups.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { approvalGroup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
