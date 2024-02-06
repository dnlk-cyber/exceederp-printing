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
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class FurnitureCategoriesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobTypes"] = db.JobCategories.Select(s => new {
                JobTypeId = s.JobCategoryId,
                Type = s.HaveParent ? db.JobCategories.Where(q => q.JobCategoryId == s.ParentId).FirstOrDefault().JobName + "-" + s.JobName : s.JobName
            }).ToList();
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
            ViewData["Categorys"] = categories;
            var myCustomers = db.OrganizationCustomers.Select(
                s=>new {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
                }).ToList();
            ViewData["Customers"] = myCustomers;

            //IList<Inventory> inventories = db.Inventories.ToList();
            ViewData["Inventories"] = selecteditems;

            IList<FurnitureStep> printingSteps = db.FurnitureSteps.ToList();
            ViewData["PrintingSteps"] = printingSteps;
            ViewBag.data = GetData();

            return View();

        }

        public JsonResult GetAllEstimationForms()
        {
            var data = db.EstimationForms;
            return Json(data.Select(s => new { s.EstimationFormId, s.JobNumber }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCustomers()
        {
            var data = db.OrganizationCustomers;
            return Json(data.Select(s => new { Code = s.OrganizationCustomerID, Name = s.TradeName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FurnitureCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            
            IQueryable<FurnitureCategory> furnitureCategories = db.FurnitureCategories;
            DataSourceResult result = furnitureCategories.ToDataSourceResult(request, furnitureCategory => new {
                FurnitureCategoryId = furnitureCategory.FurnitureCategoryId,
                Name = furnitureCategory.Name,
                Value = furnitureCategory.Value,
                WorkingTime = furnitureCategory.WorkingTime,
                CostPerHour = furnitureCategory.CostPerHour,
                CreatedBy = furnitureCategory.CreatedBy,
                DateCreated = furnitureCategory.DateCreated,
                ModifiedBy = furnitureCategory.ModifiedBy,
                LastModified =furnitureCategory.LastModified
            });

            return Json(result);
        }
        public ActionResult FurnitureCategories_Create([DataSourceRequest]DataSourceRequest request, FurnitureCategory furnitureCategory)
        {
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(furnitureCategory);
                db.FurnitureCategories.Add(entity);
                db.SaveChanges();
                furnitureCategory.FurnitureCategoryId = entity.FurnitureCategoryId;

            }

            return Json(new[] { furnitureCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureCategories_Update([DataSourceRequest]DataSourceRequest request, FurnitureCategory furnitureCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(furnitureCategory);
                db.FurnitureCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureCategories_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureCategory furnitureCategory)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(furnitureCategory);
                db.FurnitureCategories.Attach(entity);
                db.FurnitureCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureCategory }.ToDataSourceResult(request, ModelState));
        }

        public FurnitureCategory GetMappedObject(FurnitureCategory furnitureCategory)
        {
            var entity = new FurnitureCategory
            {
                FurnitureCategoryId = furnitureCategory.FurnitureCategoryId,
                Name = furnitureCategory.Name,
                Value = furnitureCategory.Value,
                WorkingTime = furnitureCategory.WorkingTime,
                CostPerHour = furnitureCategory.CostPerHour,
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now,
                ModifiedBy = User.Identity.Name,
                LastModified = DateTime.Now,
            };
            return entity;

        }
        public JsonResult CheckEstimation(int id)
        {
            var estimation = db.EstimationForms.Find(id);
            object response = null;

            if (estimation!=null)
                {
                estimation.IsChecked = true;
                estimation.CheckedBy = User.Identity.Name;
                db.EstimationForms.Attach(estimation);
                db.Entry(estimation).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true,Message= "Estimation has been checked Successfully!" };

            }
            else
            {
                response= new { Success = false, Message = "Estimation not found!" };
            }

            return Json(response);
           
        }

        public JsonResult RejectEstimation(int id)
        {
            var estimation = db.EstimationForms.Find(id);
            object response = null;

            if (estimation != null)
            {
                estimation.IsChecked = false;
                db.EstimationForms.Attach(estimation);
                db.Entry(estimation).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Estimation has been rejected Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Estimation not found!" };
            }

            return Json(response);

        }

        public JsonResult ApproveEstimation(int id)
        {
            var estimation = db.EstimationForms.Find(id);
            object response = null;
            if (estimation != null)
            {
                if(estimation.IsChecked)
                {
                    estimation.IsApproved = true;
                    estimation.ApprovedBy = User.Identity.Name;

                    var oc = db.OverallCosts.Where(q => q.EstimationFormId == estimation.EstimationFormId).FirstOrDefault();
                    db.EstimationForms.Attach(estimation);
                    db.Entry(estimation).State = EntityState.Modified;
                    //add new proforma 
                    var proforma = new FurnitureProforma();
                    proforma.HaveEstimation = true;
                    proforma.IsExistingCustomer = true;
                    proforma.FurnitureProformaId = proforma.FurnitureProformaId;
                    proforma.ProformaNo = estimation.EstimationFormId.ToString();
                    proforma.CustomerId = estimation.CustomerId;
                    proforma.EstimationFormId = estimation.EstimationFormId;
                    proforma.DateCreated = DateTime.Now;
                    proforma.CreatedBy = User.Identity.Name;
                    db.FurnitureProformas.Add(proforma);
                    db.SaveChanges();
                    decimal beforeTax = 0;
                    decimal withTax = 0;
                    decimal vat = 0;
                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId.ToString() == estimation.JobTypeId.ToString());
                    if (jobType != null)
                    {
                        var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                        if (tax != null)
                        {
                             beforeTax = estimation.Quantity * oc.EstimatedNetPrice;
                             withTax = beforeTax + (beforeTax * Convert.ToDecimal(tax.CalculatedRate));
                             vat = withTax - beforeTax;
                        }
                    }

                    var proformaItem = new FurniturePFProformaItems();
                    proformaItem.FurniturePFProformaItemsId = proformaItem.FurniturePFProformaItemsId;
                    proformaItem.FurnitureProformaId = proforma.FurnitureProformaId;
                    proformaItem.FurnitureProformaId = proforma.FurnitureProformaId;
                    proformaItem.FurnitureJobTypeId = estimation.JobTypeId;
                    proformaItem.Quantity = estimation.Quantity;
                    proformaItem.UnitPrice = (decimal)oc?.EstimatedNetPrice;
                    proformaItem.BeforeTax = beforeTax;
                    proformaItem.TotalPrice = withTax;
                    proformaItem.Size = estimation.Size ;
                    proformaItem.Tax = vat;
                    db.FurniturePFProformaItems.Add(proformaItem);
                    db.SaveChanges();
                    response = new { Success = true, Message = "Estimation has been checked Successfully!" };
                }    
                else
                {
                    response = new { Success = false, Message = "Estimation not found!" };
                }
            }
            else
            {
                response = new { Success = false, Message = "Estimation not found!" };

            }

            return Json(response);
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
                    prt.Selected = true;
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
