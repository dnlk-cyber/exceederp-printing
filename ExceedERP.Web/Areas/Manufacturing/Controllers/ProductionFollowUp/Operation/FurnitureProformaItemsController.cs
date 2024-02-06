using System;
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
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpProformaUser)]
    public class FurnitureProformaItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobCategories"] = db.JobCategories.ToList();
            ViewBag.data = GetData(); 
            return View();
        }
        public JsonResult get()
        {
            var jobCategories = db.FurnitureJobCategories.Where(q => q.HaveParent == false).ToList();
            List<DropDownTreeItemModel> jobs = new List<DropDownTreeItemModel>();
            foreach (var jobCategory in jobCategories)
            {
                int i = 0;
                DropDownTreeItemModel jobType = new DropDownTreeItemModel();
                List<DropDownTreeItemModel> jobTypes = new List<DropDownTreeItemModel>();

                var childs = db.JobCategories.Where(q => q.ParentId == jobCategory.FurnitureJobCategoryId).ToList();
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

                jobType.Value = jobCategory.FurnitureJobCategoryId.ToString();
                jobType.Items = jobTypes;
                jobs.Add(jobType);


            }



            return Json(jobs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProformaItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurniturePFProformaItems> proformaitems = db.FurniturePFProformaItems.Where(x => x.FurnitureProformaId == id);
            DataSourceResult result = proformaitems.ToDataSourceResult(request, proformaItems => new
            {
                PFProformaItemsId = proformaItems.FurniturePFProformaItemsId,
                ProformaId = proformaItems.FurnitureProformaId,
                JobTypeId = proformaItems.FurnitureJobTypeId,
                Quantity = proformaItems.Quantity,
                Size = proformaItems.Size,
                UnitPrice = proformaItems.UnitPrice,
                Tax = proformaItems.Tax,
                BeforeTax = proformaItems.BeforeTax,
                TotalPrice = proformaItems.TotalPrice,               
                Description = proformaItems.Description
            });

            return Json(result);
        }

     

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Create([DataSourceRequest]DataSourceRequest request, FurniturePFProformaItems proformaItems, int id)
        {
            //var jobType = db.JobTypes.FirstOrDefault(x => x.JobTypeId == proformaItems.JobTypeId);
            //if (jobType != null)
            //{
            //    var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
            //    if (tax != null)
            //    {
            //        proformaItems.Tax = tax.Rate;

            //        proformaItems.TotalPrice = (proformaItems.UnitPrice * proformaItems.Quantity) + (proformaItems.UnitPrice * proformaItems.Quantity * tax.CalculatedRate);
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", "Add VAT setting first");
            //    }

            //}
            //else
            //{
            //    ModelState.AddModelError("", "No Such Job Type");
            //}
            if (ModelState.IsValid)
            {
                var entity = new FurniturePFProformaItems
                {
                    FurniturePFProformaItemsId = proformaItems.FurniturePFProformaItemsId,
                    FurnitureProformaId = id,
                    FurnitureJobTypeId = proformaItems.FurnitureJobTypeId,
                    Quantity = proformaItems.Quantity,
                    Size = proformaItems.Size,
                    UnitPrice = proformaItems.UnitPrice,
                    Tax = proformaItems.Tax,
                    BeforeTax = proformaItems.BeforeTax,
                    TotalPrice = 12,
                    Description = proformaItems.Description
                };

                db.FurniturePFProformaItems.Add(entity);
                db.SaveChanges();
                proformaItems.FurniturePFProformaItemsId = entity.FurniturePFProformaItemsId;
            }
            return Json(new[] { proformaItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Update([DataSourceRequest]DataSourceRequest request, FurniturePFProformaItems proformaItems)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new FurniturePFProformaItems
                {
                    FurniturePFProformaItemsId = proformaItems.FurniturePFProformaItemsId,
                    FurnitureProformaId = proformaItems.FurnitureProformaId,
                    FurnitureJobTypeId = proformaItems.FurnitureJobTypeId,
                    Quantity = proformaItems.Quantity,
                    Size = proformaItems.Size,
                    UnitPrice = proformaItems.UnitPrice,
                    Tax = proformaItems.Tax,
                    BeforeTax = proformaItems.BeforeTax,
                    TotalPrice = proformaItems.TotalPrice,
                    Description = proformaItems.Description
                };

                db.FurniturePFProformaItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { proformaItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Destroy([DataSourceRequest]DataSourceRequest request, FurniturePFProformaItems proformaItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurniturePFProformaItems
                {
                    FurniturePFProformaItemsId = proformaItems.FurniturePFProformaItemsId,
                    FurnitureProformaId = proformaItems.FurnitureProformaId,
                    FurnitureJobTypeId = proformaItems.FurnitureJobTypeId,
                    Quantity = proformaItems.Quantity,
                    Size = proformaItems.Size,
                    UnitPrice = proformaItems.UnitPrice,
                    Tax = proformaItems.Tax,
                    BeforeTax = proformaItems.BeforeTax,
                    TotalPrice = proformaItems.TotalPrice,
                    Description = proformaItems.Description
                };

                db.FurniturePFProformaItems.Attach(entity);
                db.FurniturePFProformaItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { proformaItems }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult GetBeforeVatAndTotalPerice(int jobTypeId, int quantity, double unitPrice)
        {
            var jobType = db.FurnitureJobCategories.FirstOrDefault(x => x.FurnitureJobCategoryId == jobTypeId);
            if (jobType != null)
            {
                var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                if (tax != null)
                {

                    double beforeTax = quantity * unitPrice;
                    double taxBirr = quantity * unitPrice * Convert.ToDouble(tax.CalculatedRate);
                    double withTax = (quantity * unitPrice) + (quantity * unitPrice * Convert.ToDouble(tax.CalculatedRate));

                    var calculatedField = Json(new
                    {
                        Status = "OK",
                        BeforeTax = Math.Round(beforeTax, 3),
                        Tax = Math.Round(taxBirr, 3),
                        TotalPrice = Math.Round(withTax, 3)
                    });

                    return calculatedField;
                }
            }

            var nodata = Json(new
            {
                Status = "OK",
                BeforeTax = 0,
                Tax = 0,
                TotalPrice = 0
            });

            return nodata;

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private IEnumerable<DropDownTreeItemModel> GetData()
        {
            var jobCategories = db.FurnitureJobCategories.Where(q=>q.HaveParent== false).ToList();
            List<DropDownTreeItemModel> jobs = new List<DropDownTreeItemModel>();
            foreach (var jobCategory in jobCategories)
            {
                int i = 0;
                DropDownTreeItemModel jobType = new DropDownTreeItemModel();
                List<DropDownTreeItemModel> jobTypes = new List<DropDownTreeItemModel>();

                var childs = db.JobCategories.Where(q => q.ParentId == jobCategory.FurnitureJobCategoryId).ToList();
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
          
                jobType.Value = jobCategory.FurnitureJobCategoryId.ToString();
                jobType.Items = jobTypes;
                jobs.Add(jobType);


            }



            return jobs;
        }

    }
}
