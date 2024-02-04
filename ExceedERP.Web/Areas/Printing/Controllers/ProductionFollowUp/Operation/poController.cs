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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{
    public class poController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Proformas_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Proforma> proformas = db.Proformas;
            DataSourceResult result = proformas.ToDataSourceResult(request, proforma => new {
                ProformaId = proforma.ProformaId,
                Time = proforma.Time,
                InvoiceDate = proforma.InvoiceDate,
                DeliveryDate = proforma.DeliveryDate,
                ValidityDate = proforma.ValidityDate,
                InvoiceNo = proforma.InvoiceNo,
                CustomerName = proforma.CustomerName,
                BrandName = proforma.BrandName,
                TinNo = proforma.TinNo,
                ProformaNo = proforma.ProformaNo
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
