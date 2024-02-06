﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Setting

{
    // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class FurnitureProductionCustomerTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            
            return View();
        }

       
        public ActionResult ProductionCustomerTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureProductionCustomerType> ProductionCustomerTypes = db.FurnitureProductionCustomerTypes;
            DataSourceResult result = ProductionCustomerTypes.ToDataSourceResult(request, ProductionCustomerType => new {
                ProductionCustomerTypeId = ProductionCustomerType.FurnitureProductionCustomerTypeId,
                Code = ProductionCustomerType.Code,
                Name = ProductionCustomerType.Name,
                Location = ProductionCustomerType.Location,
                Remark = ProductionCustomerType.Remark

            });

            return Json(result);
        }

        public ActionResult ProductionCustomerTypes_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductionCustomerType ProductionCustomerType)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionCustomerType)this.GetObject(ProductionCustomerType);
                db.FurnitureProductionCustomerTypes.Add(entity);
                db.SaveChanges();
                ProductionCustomerType.FurnitureProductionCustomerTypeId = entity.FurnitureProductionCustomerTypeId;
            }

            return Json(new[] { ProductionCustomerType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionCustomerTypes_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductionCustomerType ProductionCustomerType)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionCustomerType)this.GetObject(ProductionCustomerType);

                db.FurnitureProductionCustomerTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ProductionCustomerType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionCustomerTypes_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionCustomerType ProductionCustomerType)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureProductionCustomerType)this.GetObject(ProductionCustomerType);

                db.FurnitureProductionCustomerTypes.Attach(entity);
                db.FurnitureProductionCustomerTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionCustomerType }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureProductionCustomerType ProductionCustomerType)
        {
            var entity = new FurnitureProductionCustomerType
            {
                FurnitureProductionCustomerTypeId = ProductionCustomerType.FurnitureProductionCustomerTypeId,
                Code = ProductionCustomerType.Code,
                Name = ProductionCustomerType.Name,
                Location = ProductionCustomerType.Location,
                Remark = ProductionCustomerType.Remark

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
