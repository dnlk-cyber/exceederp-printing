﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class StandardBillOfLaborController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StandardBillOfLabors_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StandardBillOfLabor> standardbilloflabors = db.StandardBillOfLabors.Where(x => x.FurnitureStandardJobTypeId == id);
            DataSourceResult result = standardbilloflabors.ToDataSourceResult(request, standardBillOfLabor => new StandardBillOfLabor
            {
                StandardBillOfLaborId = standardBillOfLabor.StandardBillOfLaborId,
                ManufacturingTaskCategoryId = standardBillOfLabor.ManufacturingTaskCategoryId,
                WorkShopType = standardBillOfLabor.WorkShopType,
                NumberOfEmloyees = standardBillOfLabor.NumberOfEmloyees,
                EstimatedMinute = standardBillOfLabor.EstimatedMinute,
                DateCreated = standardBillOfLabor.DateCreated,
                LastModified = standardBillOfLabor.LastModified,
                CreatedBy = standardBillOfLabor.CreatedBy,
                ModifiedBy = standardBillOfLabor.ModifiedBy,
                FurnitureStandardJobTypeId = standardBillOfLabor.FurnitureStandardJobTypeId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardBillOfLabors_Create([DataSourceRequest]DataSourceRequest request, StandardBillOfLabor standardBillOfLabor, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new StandardBillOfLabor
                {
                    ManufacturingTaskCategoryId = standardBillOfLabor.ManufacturingTaskCategoryId,
                    WorkShopType = standardBillOfLabor.WorkShopType,
                    NumberOfEmloyees = standardBillOfLabor.NumberOfEmloyees,
                    EstimatedMinute = standardBillOfLabor.EstimatedMinute,
                    DateCreated = DateTime.Today,
                    LastModified = standardBillOfLabor.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = standardBillOfLabor.ModifiedBy,
                    FurnitureStandardJobTypeId = id
                };

                db.StandardBillOfLabors.Add(entity);
                db.SaveChanges();
                standardBillOfLabor.StandardBillOfLaborId = entity.StandardBillOfLaborId;
            }

            return Json(new[] { standardBillOfLabor }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardBillOfLabors_Update([DataSourceRequest]DataSourceRequest request, StandardBillOfLabor standardBillOfLabor)
        {
            if (ModelState.IsValid)
            {
                var entity = new StandardBillOfLabor
                {
                    StandardBillOfLaborId = standardBillOfLabor.StandardBillOfLaborId,
                    ManufacturingTaskCategoryId = standardBillOfLabor.ManufacturingTaskCategoryId,
                    WorkShopType = standardBillOfLabor.WorkShopType,
                    NumberOfEmloyees = standardBillOfLabor.NumberOfEmloyees,
                    EstimatedMinute = standardBillOfLabor.EstimatedMinute,
                    DateCreated = standardBillOfLabor.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = standardBillOfLabor.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    FurnitureStandardJobTypeId = standardBillOfLabor.FurnitureStandardJobTypeId
                };

                db.StandardBillOfLabors.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { standardBillOfLabor }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StandardBillOfLabors_Destroy([DataSourceRequest]DataSourceRequest request, StandardBillOfLabor standardBillOfLabor)
        {
            if (ModelState.IsValid)
            {
                var entity = new StandardBillOfLabor
                {
                    StandardBillOfLaborId = standardBillOfLabor.StandardBillOfLaborId,
                    ManufacturingTaskCategoryId = standardBillOfLabor.ManufacturingTaskCategoryId,
                    WorkShopType = standardBillOfLabor.WorkShopType,
                    NumberOfEmloyees = standardBillOfLabor.NumberOfEmloyees,
                    EstimatedMinute = standardBillOfLabor.EstimatedMinute,
                    DateCreated = standardBillOfLabor.DateCreated,
                    LastModified = standardBillOfLabor.LastModified,
                    CreatedBy = standardBillOfLabor.CreatedBy,
                    ModifiedBy = standardBillOfLabor.ModifiedBy,
                    FurnitureStandardJobTypeId = standardBillOfLabor.FurnitureStandardJobTypeId, 
                    
                };

                db.StandardBillOfLabors.Attach(entity);
                db.StandardBillOfLabors.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { standardBillOfLabor }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
