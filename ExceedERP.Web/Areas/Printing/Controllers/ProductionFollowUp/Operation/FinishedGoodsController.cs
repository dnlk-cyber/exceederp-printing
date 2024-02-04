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

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpFinishedGoodUser)]
    public class FinishedGoodsController : Controller
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
            IQueryable<FinishedGoods> finishedgoods = db.FinishedGoods.Where(x=>x.FinishedGoodReceivingId == id);
            DataSourceResult result = finishedgoods.ToDataSourceResult(request, finishedGoods => new {
                FinishedGoodsId = finishedGoods.FinishedGoodsId,
                FinishedGoodReceivingId = finishedGoods.FinishedGoodReceivingId,
                CodeNo = finishedGoods.CodeNo,
                Description = finishedGoods.Description,
                Unit = finishedGoods.Unit,
                Quantity = finishedGoods.Quantity,
                Remark = finishedGoods.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoods_Create([DataSourceRequest]DataSourceRequest request, FinishedGoods finishedGoods, int id)
        {
            if (ModelState.IsValid)
            {
                
                var entity = new FinishedGoods
                {
                    FinishedGoodReceivingId = id,
                    CodeNo = finishedGoods.CodeNo,
                    Description = finishedGoods.Description,
                    Quantity = finishedGoods.Quantity,
                    Remark = finishedGoods.Remark
                };

                db.FinishedGoods.Add(entity);
                db.SaveChanges();
                finishedGoods.FinishedGoodsId = entity.FinishedGoodsId;
            }

            return Json(new[] { finishedGoods }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoods_Update([DataSourceRequest]DataSourceRequest request, FinishedGoods finishedGoods)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinishedGoods
                {
                    FinishedGoodsId = finishedGoods.FinishedGoodsId,
                    FinishedGoodReceivingId = finishedGoods.FinishedGoodReceivingId,
                    CodeNo = finishedGoods.CodeNo,
                    Description = finishedGoods.Description,
                    Unit = finishedGoods.Unit,
                    Quantity = finishedGoods.Quantity,
                    Remark = finishedGoods.Remark
                };

                db.FinishedGoods.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { finishedGoods }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinishedGoods_Destroy([DataSourceRequest]DataSourceRequest request, FinishedGoods finishedGoods)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinishedGoods
                {
                    FinishedGoodsId = finishedGoods.FinishedGoodsId,
                    FinishedGoodReceivingId = finishedGoods.FinishedGoodReceivingId,
                    CodeNo = finishedGoods.CodeNo,
                    Description = finishedGoods.Description,
                    Unit = finishedGoods.Unit,
                    Quantity = finishedGoods.Quantity,
                    Remark = finishedGoods.Remark
                };

                db.FinishedGoods.Attach(entity);
                db.FinishedGoods.Remove(entity);
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
