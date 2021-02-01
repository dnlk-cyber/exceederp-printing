//﻿using System;
//using System.Collections.Generic;

//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using ExceedERP.Core.Domain.Construction;
//﻿using ExceedERP.DataAccess.Context;

//namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
//{
//    public class ProjectPaymentAdvanceController : Controller
//    {
//        private ExceedDbContext db = new ExceedDbContext();

//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult AdvancePayments_Read([DataSourceRequest]DataSourceRequest request,int idA)
//        {
//            IQueryable<ProjectPaymenAdvance> advancepayments = db.AdvancePayments.Where( x=> x.ProjectInformationId ==idA);
//            DataSourceResult result = advancepayments.ToDataSourceResult(request, projectPaymenAdvance => new {
//                ProjectPaymenAdvanceId = projectPaymenAdvance.ProjectPaymenAdvanceId,
//                AdvanceTakenBeforeVat = projectPaymenAdvance.AdvanceTakenBeforeVat,
//                AdvanceTakenAfterVat = projectPaymenAdvance.AdvanceTakenAfterVat,
//                TotalAdvanceTakenBeforeVat = projectPaymenAdvance.TotalAdvanceTakenBeforeVat,
//                TotalAdvanceTakenAfterVat = projectPaymenAdvance.TotalAdvanceTakenAfterVat,
//                OutstandingBeforeVat = projectPaymenAdvance.OutstandingBeforeVat,
//                OutstandingAfterVat = projectPaymenAdvance.OutstandingAfterVat,
//                ProjectInformationId = projectPaymenAdvance.ProjectInformationId,
//                OrgBranchName = projectPaymenAdvance.OrgBranchName,
//                DateCreated = projectPaymenAdvance.DateCreated,
//                LastModified = projectPaymenAdvance.LastModified,
//                CreatedBy = projectPaymenAdvance.CreatedBy,
//                ModifiedBy = projectPaymenAdvance.ModifiedBy
//            });

//            return Json(result);
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult AdvancePayments_Create([DataSourceRequest]DataSourceRequest request, ProjectPaymenAdvance projectPaymenAdvance, int idATwo)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = new ProjectPaymenAdvance
//                {
//                    AdvanceTakenBeforeVat = projectPaymenAdvance.AdvanceTakenBeforeVat,
//                    AdvanceTakenAfterVat = projectPaymenAdvance.AdvanceTakenAfterVat,
//                    TotalAdvanceTakenBeforeVat = projectPaymenAdvance.TotalAdvanceTakenBeforeVat,
//                    TotalAdvanceTakenAfterVat = projectPaymenAdvance.TotalAdvanceTakenAfterVat,
//                    OutstandingBeforeVat = projectPaymenAdvance.OutstandingBeforeVat,
//                    OutstandingAfterVat = projectPaymenAdvance.OutstandingAfterVat,
//                    ProjectInformationId = idATwo ,
//                    OrgBranchName = projectPaymenAdvance.OrgBranchName,
//                    DateCreated = projectPaymenAdvance.DateCreated,
//                    LastModified = projectPaymenAdvance.LastModified,
//                    CreatedBy = projectPaymenAdvance.CreatedBy,
//                    ModifiedBy = projectPaymenAdvance.ModifiedBy
//                };

//                db.AdvancePayments.Add(entity);
//                db.SaveChanges();
//                projectPaymenAdvance.ProjectPaymenAdvanceId = entity.ProjectPaymenAdvanceId;
//            }

//            return Json(new[] { projectPaymenAdvance }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult AdvancePayments_Update([DataSourceRequest]DataSourceRequest request, ProjectPaymenAdvance projectPaymenAdvance)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = new ProjectPaymenAdvance
//                {
//                    ProjectPaymenAdvanceId = projectPaymenAdvance.ProjectPaymenAdvanceId,
//                    AdvanceTakenBeforeVat = projectPaymenAdvance.AdvanceTakenBeforeVat,
//                    AdvanceTakenAfterVat = projectPaymenAdvance.AdvanceTakenAfterVat,
//                    TotalAdvanceTakenBeforeVat = projectPaymenAdvance.TotalAdvanceTakenBeforeVat,
//                    TotalAdvanceTakenAfterVat = projectPaymenAdvance.TotalAdvanceTakenAfterVat,
//                    OutstandingBeforeVat = projectPaymenAdvance.OutstandingBeforeVat,
//                    OutstandingAfterVat = projectPaymenAdvance.OutstandingAfterVat,
//                    ProjectInformationId = projectPaymenAdvance.ProjectInformationId,
//                    OrgBranchName = projectPaymenAdvance.OrgBranchName,
//                    DateCreated = projectPaymenAdvance.DateCreated,
//                    LastModified = projectPaymenAdvance.LastModified,
//                    CreatedBy = projectPaymenAdvance.CreatedBy,
//                    ModifiedBy = projectPaymenAdvance.ModifiedBy
//                };

//                db.AdvancePayments.Attach(entity);
//                db.Entry(entity).State = EntityState.Modified;
//                db.SaveChanges();
//            }

//            return Json(new[] { projectPaymenAdvance }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult AdvancePayments_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPaymenAdvance projectPaymenAdvance)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = new ProjectPaymenAdvance
//                {
//                    ProjectPaymenAdvanceId = projectPaymenAdvance.ProjectPaymenAdvanceId,
//                    AdvanceTakenBeforeVat = projectPaymenAdvance.AdvanceTakenBeforeVat,
//                    AdvanceTakenAfterVat = projectPaymenAdvance.AdvanceTakenAfterVat,
//                    TotalAdvanceTakenBeforeVat = projectPaymenAdvance.TotalAdvanceTakenBeforeVat,
//                    TotalAdvanceTakenAfterVat = projectPaymenAdvance.TotalAdvanceTakenAfterVat,
//                    OutstandingBeforeVat = projectPaymenAdvance.OutstandingBeforeVat,
//                    OutstandingAfterVat = projectPaymenAdvance.OutstandingAfterVat,
//                    ProjectInformationId = projectPaymenAdvance.ProjectInformationId,
//                    OrgBranchName = projectPaymenAdvance.OrgBranchName,
//                    DateCreated = projectPaymenAdvance.DateCreated,
//                    LastModified = projectPaymenAdvance.LastModified,
//                    CreatedBy = projectPaymenAdvance.CreatedBy,
//                    ModifiedBy = projectPaymenAdvance.ModifiedBy
//                };

//                db.AdvancePayments.Attach(entity);
//                db.AdvancePayments.Remove(entity);
//                db.SaveChanges();
//            }

//            return Json(new[] { projectPaymenAdvance }.ToDataSourceResult(request, ModelState));
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}
