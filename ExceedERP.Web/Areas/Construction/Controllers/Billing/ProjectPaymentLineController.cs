﻿using System;
using System.Collections.Generic;
﻿using System.ComponentModel.Design.Serialization;
﻿
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
    public class ProjectPaymentLineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LineAmounts_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProjectPaymenCertificatetLine> lineamounts = db.LineAmounts.Where( x=> x.ProjectInformationId ==id);
            DataSourceResult result = lineamounts.ToDataSourceResult(request, projectPaymenCertificatetLine => new {
                ProjectPaymenCertificatetLineId = projectPaymenCertificatetLine.ProjectPaymenCertificatetLineId,
                ContractAmount = projectPaymenCertificatetLine.ContractAmount,
                SupplimentaryContract = projectPaymenCertificatetLine.SupplimentaryContract,
                VariationOrder = projectPaymenCertificatetLine.VariationOrder,
                VAT = projectPaymenCertificatetLine.VAT,
                TotalAmount = projectPaymenCertificatetLine.TotalAmount,
                ProjectInformationId = projectPaymenCertificatetLine.ProjectInformationId,
                OrgBranchName = projectPaymenCertificatetLine.OrgBranchName,
                DateCreated = projectPaymenCertificatetLine.DateCreated,
                LastModified = projectPaymenCertificatetLine.LastModified,
                CreatedBy = projectPaymenCertificatetLine.CreatedBy,
                ModifiedBy = projectPaymenCertificatetLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineAmounts_Create([DataSourceRequest]DataSourceRequest request, ProjectPaymenCertificatetLine projectPaymenCertificatetLine, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymenCertificatetLine
                {
                    ContractAmount = projectPaymenCertificatetLine.ContractAmount,
                    SupplimentaryContract = projectPaymenCertificatetLine.SupplimentaryContract,
                    VariationOrder = projectPaymenCertificatetLine.VariationOrder,
                    VAT = projectPaymenCertificatetLine.VAT,
                    TotalAmount = projectPaymenCertificatetLine.TotalAmount,
                    ProjectInformationId = idTwo,
                    OrgBranchName = projectPaymenCertificatetLine.OrgBranchName,
                    DateCreated = projectPaymenCertificatetLine.DateCreated,
                    LastModified = projectPaymenCertificatetLine.LastModified,
                    CreatedBy = projectPaymenCertificatetLine.CreatedBy,
                    ModifiedBy = projectPaymenCertificatetLine.ModifiedBy
                };

                db.LineAmounts.Add(entity);
                db.SaveChanges();
                projectPaymenCertificatetLine.ProjectPaymenCertificatetLineId = entity.ProjectPaymenCertificatetLineId;
            }

            return Json(new[] { projectPaymenCertificatetLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineAmounts_Update([DataSourceRequest]DataSourceRequest request, ProjectPaymenCertificatetLine projectPaymenCertificatetLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymenCertificatetLine
                {
                    ProjectPaymenCertificatetLineId = projectPaymenCertificatetLine.ProjectPaymenCertificatetLineId,
                    ContractAmount = projectPaymenCertificatetLine.ContractAmount,
                    SupplimentaryContract = projectPaymenCertificatetLine.SupplimentaryContract,
                    VariationOrder = projectPaymenCertificatetLine.VariationOrder,
                    VAT = projectPaymenCertificatetLine.VAT,
                    TotalAmount = projectPaymenCertificatetLine.TotalAmount,
                    ProjectInformationId = projectPaymenCertificatetLine.ProjectInformationId,
                    OrgBranchName = projectPaymenCertificatetLine.OrgBranchName,
                    DateCreated = projectPaymenCertificatetLine.DateCreated,
                    LastModified = projectPaymenCertificatetLine.LastModified,
                    CreatedBy = projectPaymenCertificatetLine.CreatedBy,
                    ModifiedBy = projectPaymenCertificatetLine.ModifiedBy
                };

                db.LineAmounts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectPaymenCertificatetLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineAmounts_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPaymenCertificatetLine projectPaymenCertificatetLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymenCertificatetLine
                {
                    ProjectPaymenCertificatetLineId = projectPaymenCertificatetLine.ProjectPaymenCertificatetLineId,
                    ContractAmount = projectPaymenCertificatetLine.ContractAmount,
                    SupplimentaryContract = projectPaymenCertificatetLine.SupplimentaryContract,
                    VariationOrder = projectPaymenCertificatetLine.VariationOrder,
                    VAT = projectPaymenCertificatetLine.VAT,
                    TotalAmount = projectPaymenCertificatetLine.TotalAmount,
                    ProjectInformationId = projectPaymenCertificatetLine.ProjectInformationId,
                    OrgBranchName = projectPaymenCertificatetLine.OrgBranchName,
                    DateCreated = projectPaymenCertificatetLine.DateCreated,
                    LastModified = projectPaymenCertificatetLine.LastModified,
                    CreatedBy = projectPaymenCertificatetLine.CreatedBy,
                    ModifiedBy = projectPaymenCertificatetLine.ModifiedBy
                };

                db.LineAmounts.Attach(entity);
                db.LineAmounts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectPaymenCertificatetLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
