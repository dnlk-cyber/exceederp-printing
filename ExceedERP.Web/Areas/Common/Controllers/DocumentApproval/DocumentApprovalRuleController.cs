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
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Common.Controllers.DocumentApproval
{
    [Authorize(Roles = "Admin")]
    public class DocumentApprovalRuleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            UpdateDocTypes();
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            var companyStructures = db.MainCompanyStructures;
            var selectedcompanyStructures = companyStructures.Select(
                i => new
                {
                    Name = i.Name,
                    OrgStructureId = i.OrgStructureId
                }).AsQueryable();
            ViewData["Structures"] = selectedcompanyStructures;

            var branches = db.Branch;
            ViewData["Branches"] = branches;

         
            var bizUnits = db.BusinessUnits.ToList();
            var bizUnitdisplay = bizUnits.ToList();

            ViewData["businesUnits"] = bizUnitdisplay.Select(
              i => new
              {
                  Name = i.Name,
                  BusinessUnitId = i.BusinessUnitId
              }).AsQueryable(); ;
           
        }
        void UpdateDocTypes()
        {
            foreach (ApprovalDocumentType type in Enum.GetValues(typeof(ApprovalDocumentType)))
            {
                var type1 = type;
                var keys = db.DocumentApprovalRules.Where(x => x.DocumentType == type1);
                if (!keys.Any())
                {
                    var entity = new DocumentApprovalRule
                    {
                        DocumentType = type,
                        ForwardMethod = ForwardMethod.Hierarchy
                    };

                    db.DocumentApprovalRules.Add(entity);
                    db.SaveChanges();
                }
            }
        }
        public ActionResult DocumentApprovalRules_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DocumentApprovalRule> documentapprovalrules = db.DocumentApprovalRules;
            DataSourceResult result = documentapprovalrules.ToDataSourceResult(request, documentApprovalRule => new {
                DocumentApprovalRuleId = documentApprovalRule.DocumentApprovalRuleId,
                DocumentType = documentApprovalRule.DocumentType,
                OwnerCanApprove = documentApprovalRule.OwnerCanApprove,
                ApproverCanModify = documentApprovalRule.ApproverCanModify,
                CanChangeForwardTo = documentApprovalRule.CanChangeForwardTo,
                ApprovalWorkFlow = documentApprovalRule.ApprovalWorkFlow,
                WorkFlowStartProcess = documentApprovalRule.WorkFlowStartProcess,
                ForwardMethod = documentApprovalRule.ForwardMethod,
                HierarchyType = documentApprovalRule.HierarchyType,
                DefaultHierarchy = documentApprovalRule.DefaultHierarchy,
                SecurityLevel = documentApprovalRule.SecurityLevel,
                AccessLevel = documentApprovalRule.AccessLevel,
                ArchiveOn = documentApprovalRule.ArchiveOn
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DocumentApprovalRules_Create([DataSourceRequest]DataSourceRequest request, DocumentApprovalRule documentApprovalRule)
        {
            if (ModelState.IsValid)
            {
                var entity = new DocumentApprovalRule
                {
                    DocumentType = documentApprovalRule.DocumentType,
                    OwnerCanApprove = documentApprovalRule.OwnerCanApprove,
                    ApproverCanModify = documentApprovalRule.ApproverCanModify,
                    CanChangeForwardTo = documentApprovalRule.CanChangeForwardTo,
                    ApprovalWorkFlow = documentApprovalRule.ApprovalWorkFlow,
                    WorkFlowStartProcess = documentApprovalRule.WorkFlowStartProcess,
                    ForwardMethod = documentApprovalRule.ForwardMethod,
                    HierarchyType = documentApprovalRule.HierarchyType,
                    DefaultHierarchy = documentApprovalRule.DefaultHierarchy,
                    SecurityLevel = documentApprovalRule.SecurityLevel,
                    AccessLevel = documentApprovalRule.AccessLevel,
                    ArchiveOn = documentApprovalRule.ArchiveOn
                };

                db.DocumentApprovalRules.Add(entity);
                db.SaveChanges();
                documentApprovalRule.DocumentApprovalRuleId = entity.DocumentApprovalRuleId;
            }

            return Json(new[] { documentApprovalRule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DocumentApprovalRules_Update([DataSourceRequest]DataSourceRequest request, DocumentApprovalRule documentApprovalRule)
        {
            if (ModelState.IsValid)
            {
                var entity = new DocumentApprovalRule
                {
                    DocumentApprovalRuleId = documentApprovalRule.DocumentApprovalRuleId,
                    DocumentType = documentApprovalRule.DocumentType,
                    OwnerCanApprove = documentApprovalRule.OwnerCanApprove,
                    ApproverCanModify = documentApprovalRule.ApproverCanModify,
                    CanChangeForwardTo = documentApprovalRule.CanChangeForwardTo,
                    ApprovalWorkFlow = documentApprovalRule.ApprovalWorkFlow,
                    WorkFlowStartProcess = documentApprovalRule.WorkFlowStartProcess,
                    ForwardMethod = documentApprovalRule.ForwardMethod,
                    HierarchyType = documentApprovalRule.HierarchyType,
                    DefaultHierarchy = documentApprovalRule.DefaultHierarchy,
                    SecurityLevel = documentApprovalRule.SecurityLevel,
                    AccessLevel = documentApprovalRule.AccessLevel,
                    ArchiveOn = documentApprovalRule.ArchiveOn
                };

                db.DocumentApprovalRules.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { documentApprovalRule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DocumentApprovalRules_Destroy([DataSourceRequest]DataSourceRequest request, DocumentApprovalRule documentApprovalRule)
        {
            if (ModelState.IsValid)
            {
                var entity = new DocumentApprovalRule
                {
                    DocumentApprovalRuleId = documentApprovalRule.DocumentApprovalRuleId,
                    DocumentType = documentApprovalRule.DocumentType,
                    OwnerCanApprove = documentApprovalRule.OwnerCanApprove,
                    ApproverCanModify = documentApprovalRule.ApproverCanModify,
                    CanChangeForwardTo = documentApprovalRule.CanChangeForwardTo,
                    ApprovalWorkFlow = documentApprovalRule.ApprovalWorkFlow,
                    WorkFlowStartProcess = documentApprovalRule.WorkFlowStartProcess,
                    ForwardMethod = documentApprovalRule.ForwardMethod,
                    HierarchyType = documentApprovalRule.HierarchyType,
                    DefaultHierarchy = documentApprovalRule.DefaultHierarchy,
                    SecurityLevel = documentApprovalRule.SecurityLevel,
                    AccessLevel = documentApprovalRule.AccessLevel,
                    ArchiveOn = documentApprovalRule.ArchiveOn
                };

                db.DocumentApprovalRules.Attach(entity);
                db.DocumentApprovalRules.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { documentApprovalRule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
