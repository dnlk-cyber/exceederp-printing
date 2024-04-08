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
    public class FurnitureStandardJobTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["ItemCode"] = db.InventoryItems.Select(s => new
            {
                Value = s.ItemCode,
                Text = s.Name
            }).ToList();

            ViewData["MaterialCategory"] = db.ManufacturingMaterialCategories.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryId,
                Text = s.ManufacturingCategory
            }).ToList();

            ViewData["MaterialCategoryItem"] = db.ManufacturingMaterialCategoryItems.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();

            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });

            return View();
        }

        public ActionResult FurnitureStandardJobTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureStandardJobType> furniturestandardjobtypes = db.FurnitureStandardJobTypes;
            DataSourceResult result = furniturestandardjobtypes.ToDataSourceResult(request, furnitureStandardJobType => new FurnitureStandardJobType
            {
                FurnitureStandardJobTypeId = furnitureStandardJobType.FurnitureStandardJobTypeId,
                JobCode = furnitureStandardJobType.JobCode,
                JobTypeName = furnitureStandardJobType.JobTypeName,
                Unit = furnitureStandardJobType.Unit,
                Remark = furnitureStandardJobType.Remark,
                DateCreated = furnitureStandardJobType.DateCreated,
                LastModified = furnitureStandardJobType.LastModified,
                CreatedBy = furnitureStandardJobType.CreatedBy,
                ModifiedBy = furnitureStandardJobType.ModifiedBy,
                IsStandard = furnitureStandardJobType.IsStandard
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureStandardJobTypes_Create([DataSourceRequest]DataSourceRequest request, FurnitureStandardJobType furnitureStandardJobType)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureStandardJobType
                {
                    JobCode = furnitureStandardJobType.JobCode,
                    JobTypeName = furnitureStandardJobType.JobTypeName,
                    Unit = furnitureStandardJobType.Unit,
                    Remark = furnitureStandardJobType.Remark,
                    DateCreated = DateTime.Today,
                    LastModified = furnitureStandardJobType.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = furnitureStandardJobType.ModifiedBy,
                    IsStandard = true
                };

                db.FurnitureStandardJobTypes.Add(entity);
                db.SaveChanges();
                furnitureStandardJobType.FurnitureStandardJobTypeId = entity.FurnitureStandardJobTypeId;
            }

            return Json(new[] { furnitureStandardJobType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureStandardJobTypes_Update([DataSourceRequest]DataSourceRequest request, FurnitureStandardJobType furnitureStandardJobType)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureStandardJobType
                {
                    FurnitureStandardJobTypeId = furnitureStandardJobType.FurnitureStandardJobTypeId,
                    JobCode = furnitureStandardJobType.JobCode,
                    JobTypeName = furnitureStandardJobType.JobTypeName,
                    Unit = furnitureStandardJobType.Unit,
                    Remark = furnitureStandardJobType.Remark,
                    DateCreated = furnitureStandardJobType.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = furnitureStandardJobType.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    IsStandard = true 
                };

                db.FurnitureStandardJobTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureStandardJobType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureStandardJobTypes_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureStandardJobType furnitureStandardJobType)
        {
            if (ModelState.IsValid)
            {
                var jobType = db.FurnitureStandardJobTypes.Find(furnitureStandardJobType.FurnitureStandardJobTypeId);
                var materials = db.StandardBillOfMaterials.Where(x => x.FurnitureStandardJobTypeId == jobType.FurnitureStandardJobTypeId);
                var labors = db.StandardBillOfLabors.Where(x => x.FurnitureStandardJobTypeId == jobType.FurnitureStandardJobTypeId);

                db.StandardBillOfLabors.RemoveRange(labors);
                db.StandardBillOfMaterials.RemoveRange(materials);

                db.SaveChanges();

                db.FurnitureStandardJobTypes.Remove(jobType);
                
                db.SaveChanges();
            }

            return Json(new[] { furnitureStandardJobType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
