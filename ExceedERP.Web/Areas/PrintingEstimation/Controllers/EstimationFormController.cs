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
    public class EstimationFormController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var selecteditems = db.InventoryItems.Select(
               i => new
               {
                   Name = i.ItemCode + "_" + i.Name,
                   Code = i.ItemCode
               }).ToList();
            var categories = db.ItemCategorys.Select(
               i => new
               {
                   Name = i.ItemCategoryCode + "_" + i.Name,
                   Code = i.ItemCategoryCode
               }).ToList();
            //ViewData["Items"] = selecteditems;
            //ViewData["Category"] = categories;
            IList<Customer> myCustomers = db.Customers.ToList();
            ViewData["Customers"] = myCustomers;

            //IList<Inventory> inventories = db.Inventories.ToList();
            ViewData["Inventories"] = selecteditems;

            IList<PrintingStep> printingSteps = db.PrintingSteps.ToList();
            ViewData["PrintingSteps"] = printingSteps;
            
            return View();

        }

        public JsonResult GetAllEstimationForms()
        {
            var data = db.EstimationForms;
            return Json(data.Select(s => new { s.EstimationFormId, s.JobNumber }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EstimationForms_Read([DataSourceRequest]DataSourceRequest request)
        {
            
            IQueryable<EstimationForm> estimationforms = db.EstimationForms;
            DataSourceResult result = estimationforms.ToDataSourceResult(request, estimationForm => new {
                EstimationFormId = estimationForm.EstimationFormId,
                CustomerId = estimationForm.CustomerId,
                Date = estimationForm.Date,
                JobNumber = estimationForm.JobNumber,
                Quantity = estimationForm.Quantity,
                Size = estimationForm.Size,
                TextPaper = estimationForm.TextPaper,
                TextPrint = estimationForm.TextPrint,
                CoverPrint = estimationForm.CoverPrint,
                BindingStyle = estimationForm.BindingStyle,
                Others = estimationForm.Others
            });

            return Json(result);
        }
        public ActionResult EstimationForms_Create([DataSourceRequest]DataSourceRequest request, EstimationForm estimationForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationForm
                {
                    EstimationFormId = estimationForm.EstimationFormId,
                    CustomerId = estimationForm.CustomerId,
                    Date = estimationForm.Date,
                    JobNumber = estimationForm.JobNumber,
                    Quantity = estimationForm.Quantity,
                    Size = estimationForm.Size,
                    TextPaper = estimationForm.TextPaper,
                    TextPrint = estimationForm.TextPrint,
                    CoverPrint = estimationForm.CoverPrint,
                    BindingStyle = estimationForm.BindingStyle,
                    Others = estimationForm.Others

                };

                db.EstimationForms.Add(entity);
                db.SaveChanges();
                estimationForm.EstimationFormId = entity.EstimationFormId;

            }

            return Json(new[] { estimationForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationForms_Update([DataSourceRequest]DataSourceRequest request, EstimationForm estimationForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationForm
                {
                    EstimationFormId = estimationForm.EstimationFormId,
                    CustomerId = estimationForm.CustomerId,
                    Date = estimationForm.Date,
                    JobNumber = estimationForm.JobNumber,
                    Quantity = estimationForm.Quantity,
                    Size = estimationForm.Size,
                    TextPaper = estimationForm.TextPaper,
                    TextPrint = estimationForm.TextPrint,
                    CoverPrint = estimationForm.CoverPrint,
                    BindingStyle = estimationForm.BindingStyle,
                    Others = estimationForm.Others

                };

                db.EstimationForms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { estimationForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationForms_Destroy([DataSourceRequest]DataSourceRequest request, EstimationForm estimationForm)
        {

            if (ModelState.IsValid)
            {
                var entity = new EstimationForm
                {
                    EstimationFormId = estimationForm.EstimationFormId,
                    CustomerId = estimationForm.CustomerId,
                    Date = estimationForm.Date,
                    JobNumber = estimationForm.JobNumber,
                    Quantity = estimationForm.Quantity,
                    Size = estimationForm.Size,
                    TextPaper = estimationForm.TextPaper,
                    TextPrint = estimationForm.TextPrint,
                    CoverPrint = estimationForm.CoverPrint,
                    BindingStyle = estimationForm.BindingStyle,
                    Others = estimationForm.Others


                };

                db.EstimationForms.Attach(entity);
                db.EstimationForms.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { estimationForm }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
