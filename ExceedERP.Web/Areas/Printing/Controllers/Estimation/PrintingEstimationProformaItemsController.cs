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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.Estimation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpProformaUser)]
    public class PrintingEstimationProformaItemsController : Controller
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



            return Json(jobs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProformaItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PFProformaItems> proformaitems = db.PFProformaItems.Where(x => x.ProformaId == id);
            DataSourceResult result = proformaitems.ToDataSourceResult(request, proformaItems => new
            {
                PFProformaItemsId = proformaItems.PFProformaItemsId,
                ProformaId = proformaItems.ProformaId,
                JobTypeId = proformaItems.JobTypeId,
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
        public ActionResult ProformaItems_Create([DataSourceRequest]DataSourceRequest request, PFProformaItems proformaItems, int id)
        {
            proformaItems.JobTypeId = Int16.Parse(proformaItems.JT);

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
                var entity = new PFProformaItems
                {
                    PFProformaItemsId = proformaItems.PFProformaItemsId,
                    ProformaId = id,
                    JobTypeId =  proformaItems.JobTypeId,
                    Quantity = proformaItems.Quantity,
                    Size = proformaItems.Size,
                    UnitPrice = proformaItems.UnitPrice,
                    Tax = proformaItems.Tax,
                    BeforeTax = proformaItems.BeforeTax,
                    TotalPrice = proformaItems.TotalPrice,
                    Description = proformaItems.Description
                };

                db.PFProformaItems.Add(entity);
                db.SaveChanges();
                proformaItems.PFProformaItemsId = entity.PFProformaItemsId;
            }
            return Json(new[] { proformaItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Update([DataSourceRequest]DataSourceRequest request, PFProformaItems proformaItems)
        {
            proformaItems.JobTypeId = Int16.Parse(proformaItems.JT);

            if (ModelState.IsValid)
            {
                var entity = new PFProformaItems
                {
                    PFProformaItemsId = proformaItems.PFProformaItemsId,
                    ProformaId = proformaItems.ProformaId,
                    JobTypeId = proformaItems.JobTypeId,
                    Quantity = proformaItems.Quantity,
                    Size = proformaItems.Size,
                    UnitPrice = proformaItems.UnitPrice,
                    Tax = proformaItems.Tax,
                    BeforeTax = proformaItems.BeforeTax,
                    TotalPrice = proformaItems.TotalPrice,
                    Description = proformaItems.Description
                };

                db.PFProformaItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { proformaItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Destroy([DataSourceRequest]DataSourceRequest request, PFProformaItems proformaItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new PFProformaItems
                {
                    PFProformaItemsId = proformaItems.PFProformaItemsId,
                    ProformaId = proformaItems.ProformaId,
                    JobTypeId = proformaItems.JobTypeId,
                    Quantity = proformaItems.Quantity,
                    Size = proformaItems.Size,
                    UnitPrice = proformaItems.UnitPrice,
                    Tax = proformaItems.Tax,
                    BeforeTax = proformaItems.BeforeTax,
                    TotalPrice = proformaItems.TotalPrice,
                    Description = proformaItems.Description
                };

                db.PFProformaItems.Attach(entity);
                db.PFProformaItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { proformaItems }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult GetBeforeVatAndTotalPerice(int jobTypeId, int quantity, double unitPrice)
        {
            var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobTypeId);
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
            var jobCategories = db.JobCategories.Where(q=>q.HaveParent== false).ToList();
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
