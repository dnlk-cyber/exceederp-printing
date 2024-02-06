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

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation

{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpFinishedGoodUser)]
    public class FurnitureFinishedGoodsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            return View();
        }

        public ActionResult FinishedGoods_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureFinishedGoods> finishedgoods = db.FurnitureFinishedGoods.Where(x=>x.FurnitureFinishedGoodReceivingId == id);
            DataSourceResult result = finishedgoods.ToDataSourceResult(request, finishedGoods => new {
                FinishedGoodsId = finishedGoods.FurnitureFinishedGoodsId,
                FinishedGoodReceivingId = finishedGoods.FurnitureFinishedGoodReceivingId,
                CodeNo = finishedGoods.CodeNo,
                Description = finishedGoods.Description,
                Unit = finishedGoods.Unit,
                Quantity = finishedGoods.Quantity,
                Remark = finishedGoods.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoods_Create([DataSourceRequest]DataSourceRequest request, FurnitureFinishedGoods finishedGoods, int id)
        {
            if (ModelState.IsValid)
            {
                
                var entity = new FurnitureFinishedGoods
                {
                    FurnitureFinishedGoodReceivingId = id,
                    CodeNo = finishedGoods.CodeNo,
                    Description = finishedGoods.Description,
                    Quantity = finishedGoods.Quantity,
                    Remark = finishedGoods.Remark
                };

                db.FurnitureFinishedGoods.Add(entity);
                db.SaveChanges();
                finishedGoods.FurnitureFinishedGoodsId = entity.FurnitureFinishedGoodsId;
            }

            return Json(new[] { finishedGoods }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoods_Update([DataSourceRequest]DataSourceRequest request, FurnitureFinishedGoods finishedGoods)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureFinishedGoods
                {
                    FurnitureFinishedGoodsId = finishedGoods.FurnitureFinishedGoodsId,
                    FurnitureFinishedGoodReceivingId = finishedGoods.FurnitureFinishedGoodReceivingId,
                    CodeNo = finishedGoods.CodeNo,
                    Description = finishedGoods.Description,
                    Unit = finishedGoods.Unit,
                    Quantity = finishedGoods.Quantity,
                    Remark = finishedGoods.Remark
                };

                db.FurnitureFinishedGoods.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { finishedGoods }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoods_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureFinishedGoods finishedGoods)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureFinishedGoods
                {
                    FurnitureFinishedGoodsId = finishedGoods.FurnitureFinishedGoodsId,
                    FurnitureFinishedGoodReceivingId = finishedGoods.FurnitureFinishedGoodReceivingId,
                    CodeNo = finishedGoods.CodeNo,
                    Description = finishedGoods.Description,
                    Unit = finishedGoods.Unit,
                    Quantity = finishedGoods.Quantity,
                    Remark = finishedGoods.Remark
                };

                db.FurnitureFinishedGoods.Attach(entity);
                db.FurnitureFinishedGoods.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { finishedGoods }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
