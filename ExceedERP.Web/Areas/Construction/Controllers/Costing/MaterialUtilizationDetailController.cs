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
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class MaterialUtilizationDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MaterialUtilizationDetails_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaterialUtilizationDetail> materialutilizationdetails = db.MaterialUtilizationDetails;
            DataSourceResult result = materialutilizationdetails.ToDataSourceResult(request, materialUtilizationDetail => new {
                MaterialUtilzationDetlId = materialUtilizationDetail.MaterialUtilzationDetlId,
                UtilizedQuantity = materialUtilizationDetail.UtilizedQuantity,
                UtilizationOfItem = materialUtilizationDetail.UtilizationOfItem,
                MaterialWastage = materialUtilizationDetail.MaterialWastage,
                Remark = materialUtilizationDetail.Remark,
                OrgBranchName = materialUtilizationDetail.OrgBranchName,
                DateCreated = materialUtilizationDetail.DateCreated,
                LastModified = materialUtilizationDetail.LastModified,
                CreatedBy = materialUtilizationDetail.CreatedBy,
                ModifiedBy = materialUtilizationDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialUtilizationDetails_Create([DataSourceRequest]DataSourceRequest request, MaterialUtilizationDetail materialUtilizationDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialUtilizationDetail
                {
                    UtilizedQuantity = materialUtilizationDetail.UtilizedQuantity,
                    UtilizationOfItem = materialUtilizationDetail.UtilizationOfItem,
                    MaterialWastage = materialUtilizationDetail.MaterialWastage,
                    Remark = materialUtilizationDetail.Remark,
                    OrgBranchName = materialUtilizationDetail.OrgBranchName,
                    DateCreated = materialUtilizationDetail.DateCreated,
                    LastModified = materialUtilizationDetail.LastModified,
                    CreatedBy = materialUtilizationDetail.CreatedBy,
                    ModifiedBy = materialUtilizationDetail.ModifiedBy
                };

                db.MaterialUtilizationDetails.Add(entity);
                db.SaveChanges();
                materialUtilizationDetail.MaterialUtilzationDetlId = entity.MaterialUtilzationDetlId;
            }

            return Json(new[] { materialUtilizationDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialUtilizationDetails_Update([DataSourceRequest]DataSourceRequest request, MaterialUtilizationDetail materialUtilizationDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialUtilizationDetail
                {
                    MaterialUtilzationDetlId = materialUtilizationDetail.MaterialUtilzationDetlId,
                    UtilizedQuantity = materialUtilizationDetail.UtilizedQuantity,
                    UtilizationOfItem = materialUtilizationDetail.UtilizationOfItem,
                    MaterialWastage = materialUtilizationDetail.MaterialWastage,
                    Remark = materialUtilizationDetail.Remark,
                    OrgBranchName = materialUtilizationDetail.OrgBranchName,
                    DateCreated = materialUtilizationDetail.DateCreated,
                    LastModified = materialUtilizationDetail.LastModified,
                    CreatedBy = materialUtilizationDetail.CreatedBy,
                    ModifiedBy = materialUtilizationDetail.ModifiedBy
                };

                db.MaterialUtilizationDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { materialUtilizationDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialUtilizationDetails_Destroy([DataSourceRequest]DataSourceRequest request, MaterialUtilizationDetail materialUtilizationDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaterialUtilizationDetail
                {
                    MaterialUtilzationDetlId = materialUtilizationDetail.MaterialUtilzationDetlId,
                    UtilizedQuantity = materialUtilizationDetail.UtilizedQuantity,
                    UtilizationOfItem = materialUtilizationDetail.UtilizationOfItem,
                    MaterialWastage = materialUtilizationDetail.MaterialWastage,
                    Remark = materialUtilizationDetail.Remark,
                    OrgBranchName = materialUtilizationDetail.OrgBranchName,
                    DateCreated = materialUtilizationDetail.DateCreated,
                    LastModified = materialUtilizationDetail.LastModified,
                    CreatedBy = materialUtilizationDetail.CreatedBy,
                    ModifiedBy = materialUtilizationDetail.ModifiedBy
                };

                db.MaterialUtilizationDetails.Attach(entity);
                db.MaterialUtilizationDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { materialUtilizationDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
