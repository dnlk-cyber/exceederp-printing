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

namespace ExceedERP.Web.Areas.Printing.Controllers.Estimation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class PrintingEstimationChangeOfOrdersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeOrders_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ChangeOrder> changeorders = db.ChangeOrders.Where(x=>x.JobId == id);
            DataSourceResult result = changeorders.ToDataSourceResult(request, changeOrder => new {
                ChangeOrderId = changeOrder.ChangeOrderId,
                JobId = changeOrder.JobId,
                Date = changeOrder.Date,
                Reason = changeOrder.Reason,
                Quantity = changeOrder.Quantity,
                Size = changeOrder.Size,
                Price = changeOrder.Price,
                Tax = changeOrder.Tax,
                Total = changeOrder.Total,
                InvoiceNo = changeOrder.InvoiceNo,
                OrderNo = changeOrder.OrderNo,
                PreparedBy = changeOrder.PreparedBy,
                TypeAndQuantityOfMaterialNeeded = changeOrder.TypeAndQuantityOfMaterialNeeded,
                Checked = changeOrder.Checked
            });

            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
