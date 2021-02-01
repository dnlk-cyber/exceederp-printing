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
    public class ApprovalGroupRuleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApprovalGroupRules_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApprovalGroupRule> approvalgrouprules = db.ApprovalGroupRules.Where(x=>x.ApprovalGroupId==id);
            DataSourceResult result = approvalgrouprules.ToDataSourceResult(request, approvalGroupRule => new {
                ApprovalGroupId = approvalGroupRule.ApprovalGroupId,
                ApprovalGroupRuleId = approvalGroupRule.ApprovalGroupRuleId,
                RuleObject = approvalGroupRule.RuleObject,
                Type = approvalGroupRule.Type,
                AmountLimit = approvalGroupRule.AmountLimit,
                LowValue = approvalGroupRule.LowValue,
                HighValue = approvalGroupRule.HighValue
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroupRules_Create(int id,[DataSourceRequest]DataSourceRequest request, ApprovalGroupRule approvalGroupRule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroupRule
                {
                    ApprovalGroupId = id,
                    RuleObject = approvalGroupRule.RuleObject,
                    Type = approvalGroupRule.Type,
                    AmountLimit = approvalGroupRule.AmountLimit,
                    LowValue = approvalGroupRule.LowValue,
                    HighValue = approvalGroupRule.HighValue
                };

                db.ApprovalGroupRules.Add(entity);
                db.SaveChanges();
                approvalGroupRule.ApprovalGroupRuleId = entity.ApprovalGroupRuleId;
            }

            return Json(new[] { approvalGroupRule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroupRules_Update([DataSourceRequest]DataSourceRequest request, ApprovalGroupRule approvalGroupRule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroupRule
                {
                    ApprovalGroupRuleId = approvalGroupRule.ApprovalGroupRuleId,
                    ApprovalGroupId = approvalGroupRule.ApprovalGroupId,
                    RuleObject = approvalGroupRule.RuleObject,
                    Type = approvalGroupRule.Type,
                    AmountLimit = approvalGroupRule.AmountLimit,
                    LowValue = approvalGroupRule.LowValue,
                    HighValue = approvalGroupRule.HighValue
                };

                db.ApprovalGroupRules.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { approvalGroupRule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroupRules_Destroy([DataSourceRequest]DataSourceRequest request, ApprovalGroupRule approvalGroupRule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroupRule
                {
                    ApprovalGroupRuleId = approvalGroupRule.ApprovalGroupRuleId,
                    ApprovalGroupId = approvalGroupRule.ApprovalGroupId,
                    RuleObject = approvalGroupRule.RuleObject,
                    Type = approvalGroupRule.Type,
                    AmountLimit = approvalGroupRule.AmountLimit,
                    LowValue = approvalGroupRule.LowValue,
                    HighValue = approvalGroupRule.HighValue
                };

                db.ApprovalGroupRules.Attach(entity);
                db.ApprovalGroupRules.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { approvalGroupRule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
