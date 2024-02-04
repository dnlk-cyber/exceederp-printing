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
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using ExceedERP.Core.Domain.printing.PrintingEstimation;
using ExceedERP.Web.Areas.Printing.Models;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpProformaUser)]
    public class ProformaController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
             
            ViewData["EstimationForms"] = db.EstimationForms.ToList();                   
             var jobCategories = db.JobCategories.Select(s => new {
                JobTypeId = s.JobCategoryId,
                type = s.HaveParent ?s.ParentId + " " + s.JobName : s.JobName
            }).ToList();
            ViewData["JobType"] = jobCategories;
            ViewBag.data = GetData();
            var customers = db.OrganizationCustomers.Select(s => new
            {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            ViewData["Customers"] = customers;

            return View();
        }
        //public IList<OrganizationCustomer> getCustomers(Proforma proforma=null)
        //{
        //    var customers = db.OrganizationCustomers.ToList();
        //    if(proforma != null&& !proforma.HaveEstimation && !proforma.IsExistingCustomer)
        //    {
        //        var entity = new OrganizationCustomer()
        //        {
        //            Code = proforma.CustomerId,
        //            BrandName = proforma.BrandName,
        //            NAme = proforma.CustomerName
        //        };
        //        customers.Add(entity);
        //    }

        //    ViewData["Customers"] = customers;
        //    return customers;
        //}
        public JsonResult GetAllProformas()
        {
            var data = db.Proformas;
            return Json(data.Select(s => new { s.ProformaId, s.ProformaNo }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Proformas_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Proforma> proformas = db.Proformas;
            DataSourceResult result = proformas.ToDataSourceResult(request, proforma => new {
                ProformaId = proforma.ProformaId,
                Time = proforma.Time,
                EstimationFormId = proforma.EstimationFormId,
                CustomerId = proforma.CustomerId,
                HaveEstimation = proforma.HaveEstimation,
                IsExistingCustomer = proforma.IsExistingCustomer,
                InvoiceDate = proforma.InvoiceDate,
                DeliveryDate = proforma.DeliveryDate,
                ValidityDate = proforma.ValidityDate,
                PaymentTerm = proforma.PaymentTerm,
                InvoiceNo = proforma.InvoiceNo,
                CustomerName = proforma.CustomerName,
                BrandName = proforma.BrandName,
                ProformaNo = proforma.ProformaNo,
               TinNo = proforma.TinNo
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Proformas_Create([DataSourceRequest]DataSourceRequest request, Proforma proforma)
        {
            if (ModelState.IsValid)
            {
                var estimationForm = new EstimationForm();
                var customer = new OrganizationCustomer();
                if (!proforma.HaveEstimation && !proforma.IsExistingCustomer)
                {
                    customer = AddNewCustomer.Save(proforma);
                    db.OrganizationCustomers.Add(customer);
                    db.SaveChanges();
                }
                else if (proforma.HaveEstimation)
                {
                     estimationForm = db.EstimationForms.Find(proforma.EstimationFormId);
                    
                }
                var entity = new Proforma
                {
                    Time = proforma.Time,
                    HaveEstimation = proforma.HaveEstimation,
                    IsExistingCustomer = proforma.IsExistingCustomer,
                    CustomerId = proforma.HaveEstimation ? estimationForm.CustomerId : customer.OrganizationCustomerID | proforma.CustomerId,
                    EstimationFormId = proforma.EstimationFormId,
                    InvoiceDate = proforma.InvoiceDate,
                    DeliveryDate = proforma.DeliveryDate,
                    ValidityDate = proforma.ValidityDate,
                    PaymentTerm = proforma.PaymentTerm,
                    InvoiceNo = proforma.InvoiceNo,
                    CustomerName = proforma.CustomerName,
                    BrandName = proforma.BrandName,
                    ProformaNo = proforma.ProformaNo,
                    TinNo = proforma.TinNo
                };
                db.Proformas.Add(entity);
                db.SaveChanges();
                proforma.ProformaId = entity.ProformaId;
                proforma.CustomerId = entity.CustomerId;
                //Index();

              //  return RedirectToAction("Index", "Proforma", new { area = "ProductionFollowUp" });

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
                var estimationForm = new EstimationForm();
                var customer = new OrganizationCustomer();
                if (!proforma.HaveEstimation && !proforma.IsExistingCustomer && proforma.CustomerId.ToString() == null)
                {
                    customer = AddNewCustomer.Save(proforma);
                    db.OrganizationCustomers.Add(customer);

                }
                else if (proforma.HaveEstimation)
                {
                    estimationForm = db.EstimationForms.Find(proforma.EstimationFormId);

                }

                var entity = new Proforma
                {
                    ProformaId = proforma.ProformaId,
                    Time = proforma.Time,
                    HaveEstimation = proforma.HaveEstimation,
                    IsExistingCustomer = proforma.IsExistingCustomer,
                    CustomerId = proforma.HaveEstimation ? estimationForm.CustomerId : customer.OrganizationCustomerID|proforma.CustomerId,
                    EstimationFormId = proforma.EstimationFormId,
                    InvoiceDate = proforma.InvoiceDate,
                    DeliveryDate = proforma.DeliveryDate,
                    ValidityDate = proforma.ValidityDate,
                    PaymentTerm = proforma.PaymentTerm,
                    InvoiceNo = proforma.InvoiceNo,
                    BrandName = proforma.BrandName,
                    CustomerName = proforma.CustomerName,
                    ProformaNo = proforma.ProformaNo,
                    TinNo = proforma.TinNo
                };
                db.Proformas.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Proforma", new { area = "ProductionFollowUp" });

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
                    HaveEstimation = proforma.HaveEstimation,
                    IsExistingCustomer = proforma.IsExistingCustomer,
                    CustomerId = proforma.CustomerId,
                    EstimationFormId = proforma.EstimationFormId,
                    InvoiceDate = proforma.InvoiceDate,
                    DeliveryDate = proforma.DeliveryDate,
                    ValidityDate = proforma.ValidityDate,
                    PaymentTerm = proforma.PaymentTerm,
                    InvoiceNo = proforma.InvoiceNo,
                    ProformaNo = proforma.ProformaNo,
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
        private IEnumerable<DropDownTreeItemModel> GetData()
        {
            var jobCategories = db.JobCategories.Where(q => q.HaveParent == false).ToList();
            List<DropDownTreeItemModel> jobs = new List<DropDownTreeItemModel>();
            foreach (var jobCategory in jobCategories)
            {
                int i = 0;
                DropDownTreeItemModel jobType = new DropDownTreeItemModel();
                List<DropDownTreeItemModel> jobTypes = new List<DropDownTreeItemModel>();

                var childs = db.JobCategories.Where(q => q.ParentId == jobCategory.JobCategoryId).ToList();
                foreach (var child in childs)
                {
                    i++;
                    DropDownTreeItemModel prt = new DropDownTreeItemModel();
                    prt.Text = child.JobName;
                    prt.HasChildren = false;
                    prt.Value = child.JobCategoryId.ToString();
                    jobTypes.Add(prt);
                }
                jobType.Text = jobCategory.JobName;

                jobType.Value = jobCategory.JobCategoryId.ToString();
                jobType.Items = jobTypes;
                jobs.Add(jobType);


            }



            return jobs;
        }

    }
}
