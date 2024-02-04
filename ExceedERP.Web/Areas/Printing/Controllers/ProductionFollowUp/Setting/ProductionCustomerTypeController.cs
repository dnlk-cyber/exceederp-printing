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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Setting
{
   // [AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class ProductionCustomerTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            
            return View();
        }

       
        public ActionResult ProductionCustomerTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionCustomerType> ProductionCustomerTypes = db.ProductionCustomerTypes;
            DataSourceResult result = ProductionCustomerTypes.ToDataSourceResult(request, ProductionCustomerType => new {
                ProductionCustomerTypeId = ProductionCustomerType.ProductionCustomerTypeId,
                Code = ProductionCustomerType.Code,
                Name = ProductionCustomerType.Name,
                Location = ProductionCustomerType.Location,
                Remark = ProductionCustomerType.Remark

            });

            return Json(result);
        }

        public ActionResult ProductionCustomerTypes_Create([DataSourceRequest]DataSourceRequest request, ProductionCustomerType ProductionCustomerType)
        {
            if (ModelState.IsValid)
            {
                var entity = (ProductionCustomerType)this.GetObject(ProductionCustomerType);
                db.ProductionCustomerTypes.Add(entity);
                db.SaveChanges();
                ProductionCustomerType.ProductionCustomerTypeId = entity.ProductionCustomerTypeId;
            }

            return Json(new[] { ProductionCustomerType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionCustomerTypes_Update([DataSourceRequest]DataSourceRequest request, ProductionCustomerType ProductionCustomerType)
        {
            if (ModelState.IsValid)
            {
                var entity = (ProductionCustomerType)this.GetObject(ProductionCustomerType);

                db.ProductionCustomerTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ProductionCustomerType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionCustomerTypes_Destroy([DataSourceRequest]DataSourceRequest request, ProductionCustomerType ProductionCustomerType)
        {

            if (ModelState.IsValid)
            {
                var entity = (ProductionCustomerType)this.GetObject(ProductionCustomerType);

                db.ProductionCustomerTypes.Attach(entity);
                db.ProductionCustomerTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ProductionCustomerType }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(ProductionCustomerType ProductionCustomerType)
        {
            var entity = new ProductionCustomerType
            {
                ProductionCustomerTypeId = ProductionCustomerType.ProductionCustomerTypeId,
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
