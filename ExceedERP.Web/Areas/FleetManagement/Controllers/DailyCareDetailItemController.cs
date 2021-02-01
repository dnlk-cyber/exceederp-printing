﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class DailyCareDetailItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DailyCareDetailItems_Read([DataSourceRequest]DataSourceRequest request , string Id)
        {
            IQueryable<DailyCareDetailItem> DailyCareDetailItems = db.DailyCareDetailItems.Where(x=>x.DailyCareDetailLogId.ToString() == Id);
            DataSourceResult result = DailyCareDetailItems.ToDataSourceResult(request, DailyCareDetailItem => new {
                DailyCareDetailItemId = DailyCareDetailItem.DailyCareDetailItemId,
                 Description = DailyCareDetailItem.Description,
                Day = DailyCareDetailItem.Day,
                DateCreated = DailyCareDetailItem.DateCreated,
                LastModified = DailyCareDetailItem.LastModified,
                CreatedBy = DailyCareDetailItem.CreatedBy,
                ModifiedBy = DailyCareDetailItem.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareDetailItems_Create([DataSourceRequest]DataSourceRequest request, DailyCareDetailItem DailyCareDetailItem, string Id)
        {
            
                var entity = new DailyCareDetailItem
                {
                    DailyCareDetailItemId = Guid.NewGuid(),
                    DailyCareDetailLogId = Guid.Parse(Id),
                     Description = DailyCareDetailItem.Description,
                    Day = DailyCareDetailItem.Day,
                    DateCreated = DateTime.Now,
                    CreatedBy =User.Identity.Name,
                    LastModified = DailyCareDetailItem.LastModified,
                      ModifiedBy = DailyCareDetailItem.ModifiedBy
                };

                db.DailyCareDetailItems.Add(entity);
                db.SaveChanges();
                DailyCareDetailItem.DailyCareDetailItemId = entity.DailyCareDetailItemId;
            
            return Json(new[] { DailyCareDetailItem }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareDetailItems_Update([DataSourceRequest]DataSourceRequest request, DailyCareDetailItem DailyCareDetailItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCareDetailItem
                {
                    DailyCareDetailItemId = DailyCareDetailItem.DailyCareDetailItemId,
                    Day = DailyCareDetailItem.Day,
                    Description = DailyCareDetailItem.Description,
                     DateCreated = DailyCareDetailItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = DailyCareDetailItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.DailyCareDetailItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { DailyCareDetailItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DailyCareDetailItems_Destroy([DataSourceRequest]DataSourceRequest request, DailyCareDetailItem DailyCareDetailItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new DailyCareDetailItem
                {
                    DailyCareDetailItemId = DailyCareDetailItem.DailyCareDetailItemId,
                     Description = DailyCareDetailItem.Description,
                    Day = DailyCareDetailItem.Day,
                    DateCreated = DailyCareDetailItem.DateCreated,
                    LastModified = DailyCareDetailItem.LastModified,
                    CreatedBy = DailyCareDetailItem.CreatedBy,
                    ModifiedBy = DailyCareDetailItem.ModifiedBy
                };

                db.DailyCareDetailItems.Attach(entity);
                db.DailyCareDetailItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { DailyCareDetailItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
