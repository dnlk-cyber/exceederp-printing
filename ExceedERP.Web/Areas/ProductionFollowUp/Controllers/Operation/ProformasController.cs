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

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Operation
{
    public class ProformasController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
          
            ViewData["JobType"] = db.JobTypes.ToList();
            ViewData["JobTypeItem"] = db.JobTypeItems.ToList();
            return View();
        }

        public ActionResult Proformas_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Proforma> proformas = db.Proformas;
            DataSourceResult result = proformas.ToDataSourceResult(request, proforma => new {
                ProformaId = proforma.ProformaId,
                Time = proforma.Time,
                CustomerName = proforma.CustomerName,
                ContactName = proforma.ContactName,
                ProformaNo = proforma.ProformaNo,
                ContactPhone = proforma.ContactPhone
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Proformas_Create([DataSourceRequest]DataSourceRequest request, Proforma proforma)
        {
            if (ModelState.IsValid)
            {
                var entity = new Proforma
                {
                    Time = proforma.Time,
                    CustomerName = proforma.CustomerName,
                    ContactName = proforma.ContactName,
                    ContactPhone = proforma.ContactPhone
                };

                db.Proformas.Add(entity);
                db.SaveChanges();
                proforma.ProformaId = entity.ProformaId;

                #region

                if (proforma.ProformaId.ToString().Length == 1)
                {
                    entity.ProformaNo = "pro-000000" + proforma.ProformaId;
                }
                else if (proforma.ProformaId.ToString().Length == 2)
                {
                    entity.ProformaNo = "pro-00000" + proforma.ProformaId;
                }
                else if (proforma.ProformaId.ToString().Length == 3)
                {
                    entity.ProformaNo = "pro-0000" + proforma.ProformaId;
                }
                else if (proforma.ProformaId.ToString().Length == 4)
                {
                    entity.ProformaNo = "pro-000" + proforma.ProformaId;
                }
                else if (proforma.ProformaId.ToString().Length == 5)
                {
                    entity.ProformaNo = "pro-00" + proforma.ProformaId;
                }
                else if (proforma.ProformaId.ToString().Length == 2)
                {
                    entity.ProformaNo = "pro-0" + proforma.ProformaId;
                }
                else if (proforma.ProformaId.ToString().Length == 2)
                {
                    entity.ProformaNo = "pro-" + proforma.ProformaId;
                }

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #endregion
            }

            return Json(new[] { proforma }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Proformas_Update([DataSourceRequest]DataSourceRequest request, Proforma proforma)
        {
            if (ModelState.IsValid)
            {
                var entity = new Proforma
                {
                    ProformaId = proforma.ProformaId,
                    Time = proforma.Time,
                    CustomerName = proforma.CustomerName,
                    ContactName = proforma.ContactName,
                    ContactPhone = proforma.ContactPhone
                };

                db.Proformas.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { proforma }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Proformas_Destroy([DataSourceRequest]DataSourceRequest request, Proforma proforma)
        {
            if (ModelState.IsValid)
            {
                var entity = new Proforma
                {
                    ProformaId = proforma.ProformaId,
                    Time = proforma.Time,
                    CustomerName = proforma.CustomerName,
                    ContactName = proforma.ContactName,
                    ContactPhone = proforma.ContactPhone
                };

                db.Proformas.Attach(entity);
                db.Proformas.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { proforma }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
