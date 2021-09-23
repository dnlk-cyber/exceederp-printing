﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers
{
    public class CommertialItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CommertialItems_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CommertialItems> commertialitems = db.CommertialItems;
            DataSourceResult result = commertialitems.ToDataSourceResult(request, commertialItems => new {
                CommertialItemsId = commertialItems.CommertialItemsId,
                Job_Type = commertialItems.Job_Type,
                SubCategory = commertialItems.SubCategory,
                Description = commertialItems.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommertialItems_Create([DataSourceRequest]DataSourceRequest request, CommertialItems commertialItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommertialItems
                {
                    Job_Type = commertialItems.Job_Type,
                    SubCategory = commertialItems.SubCategory,
                    Description = commertialItems.Description
                };

                db.CommertialItems.Add(entity);
                db.SaveChanges();
                commertialItems.CommertialItemsId = entity.CommertialItemsId;
            }

            return Json(new[] { commertialItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommertialItems_Update([DataSourceRequest]DataSourceRequest request, CommertialItems commertialItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommertialItems
                {
                    CommertialItemsId = commertialItems.CommertialItemsId,
                    Job_Type = commertialItems.Job_Type,
                    SubCategory = commertialItems.SubCategory,
                    Description = commertialItems.Description
                };

                db.CommertialItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { commertialItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommertialItems_Destroy([DataSourceRequest]DataSourceRequest request, CommertialItems commertialItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new CommertialItems
                {
                    CommertialItemsId = commertialItems.CommertialItemsId,
                    Job_Type = commertialItems.Job_Type,
                    SubCategory = commertialItems.SubCategory,
                    Description = commertialItems.Description
                };

                db.CommertialItems.Attach(entity);
                db.CommertialItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { commertialItems }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
