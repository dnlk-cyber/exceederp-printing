﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.PrintingEstimation;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.PrintingEstimation.Controllers
{
    public class CustomerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            IList<Customer> myCustomers = db.Customers.ToList();
            ViewData["Customers"] = myCustomers;
            return View();
        }

        public JsonResult GetAllCustomers()
        {
            var data = db.Customers;
            return Json(data.Select(s => new { s.CustomerId, s.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Customers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Customer> customers = db.Customers;
            DataSourceResult result = customers.ToDataSourceResult(request, customer => new {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Phone = customer.Phone,
                Pobox = customer.Pobox,
                VAT = customer.VAT,
                Margin = customer.Margin,
                OverHead = customer.OverHead,
                Description = customer.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Customers_Create([DataSourceRequest]DataSourceRequest request, Customer Customer)
        {
            if (ModelState.IsValid)
            {
                var entity = new Customer
                {
                    Name = Customer.Name,
                    Phone = Customer.Phone,
                    Pobox = Customer.Pobox,
                    VAT = Customer.VAT,
                    Margin = Customer.Margin,
                    OverHead = Customer.OverHead,
                    Description = Customer.Description
                };

                db.Customers.Add(entity);
                db.SaveChanges();
                Customer.CustomerId = entity.CustomerId;
            }

            return Json(new[] { Customer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Customers_Update([DataSourceRequest]DataSourceRequest request, Customer Customer)
        {
            if (ModelState.IsValid)
            {
                var entity = new Customer
                {
                    CustomerId = Customer.CustomerId,
                    Name = Customer.Name,
                    Phone = Customer.Phone,
                    Pobox = Customer.Pobox,
                    VAT = Customer.VAT,
                    Margin = Customer.Margin,
                    OverHead = Customer.OverHead,
                    Description = Customer.Description
                };

                db.Customers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { Customer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Customers_Destroy([DataSourceRequest]DataSourceRequest request, Customer Customer)
        {

            if (ModelState.IsValid)
            {
                var entity = new Customer
                {
                    CustomerId = Customer.CustomerId,
                    Name = Customer.Name,
                    Phone = Customer.Phone,
                    Pobox = Customer.Pobox,
                    VAT = Customer.VAT,
                    Margin = Customer.Margin,
                    OverHead = Customer.OverHead,
                    Description = Customer.Description

                };

                db.Customers.Attach(entity);
                db.Customers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { Customer }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
