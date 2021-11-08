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
using ExceedERP.Web.Areas.ProductionFollowUp.Controllers.ViewModel;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.Core.Domain.PrintingEstimation;
using ExceedERP.Web.Areas.ProductionFollowUp.Models;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Operation
{
    public class JobOrdersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Proforma"] = db.Proformas.ToList();
            ViewData["JobType"] = db.JobTypes.ToList();
            ViewData["Customers"] = db.Customers.ToList();
            ViewData["JobTypeItem"] = db.JobTypeItems.ToList();
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
            ViewData["InventoryItems"] = selecteditems;
            ViewData["ItemCategorys"] = categories;
            ViewBag.data = GetData();

            return View();
        }
        public JsonResult GetAllJobOrders()
        {
            var data = db.Jobs;
            return Json(data.Select(s => new { s.JobId, s.JobNo }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Jobs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Job> jobs = db.Jobs;
            DataSourceResult result = jobs.ToDataSourceResult(request, job => new
            {
                JobId = job.JobId,
                Time = job.Time,
                hv = job.hv,
                IsExistingCustomer = job.IsExistingCustomer,
                ProformaId = job.ProformaId,
                JobNo = job.JobNo,
                CustomerName = job.CustomerName,
                OrderType = job.OrderType,
                CustomerId = job.CustomerId,
                Estimatedby = job.Estimatedby,
                Code = job.Code,
                Approvedby = job.Approvedby,
                ContactName = job.ContactName,
                ContactPhone = job.ContactPhone,
                City = job.City,
                SubCity = job.SubCity,
                Woreda = job.Woreda,
                POBox = job.POBox,
                JobTypeId = job.JobTypeId,
                JobTypeItemId = job.JobTypeItemId,
                UnitPrice = job.UnitPrice,
                BeforeTax = job.BeforeTax,
                Quant = job.Quant,
                Pages = job.Pages,
                Copies = job.Copies,
                TotalPrice = job.TotalPrice,
                Contract = job.Contract,
                TotalImpression = job.TotalImpression,
                DeliveryDate = job.DeliveryDate,
                TextWeight = job.TextWeight,
                CoverWeight = job.CoverWeight,
                ColorText = job.ColorText,
                Colorcover = job.Colorcover,
                FinishingSize = job.FinishingSize,
                PrintingLine = job.PrintingLine,
                PrintingMachine = job.PrintingMachine,
                Sewing = job.Sewing,
                New = job.New,
                PerfectBinding = job.PerfectBinding,
                Repeat = job.Repeat,
                Lamnating = job.Lamnating,
                Varnish = job.Varnish,
                Perforate = job.Perforate,
                Camsur = job.Camsur,
                GraphicDesignExpectedCompletetion = job.GraphicDesignExpectedCompletetion,
                PrintingExpectedCompletetion = job.PrintingExpectedCompletetion,
                FinishExpectedCompletetion = job.FinishExpectedCompletetion,
                Status = job.Status,
                FinishingWorkflow = job.FinishingWorkflow
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Create([DataSourceRequest]DataSourceRequest request, Job job)
        {
            if (ModelState.IsValid)
            {
                var proforma = new Proforma();
                var customer = new Customer();
                if (!job.hv && !proforma.IsExistingCustomer)
                {
                    customer = AddNewCustomer.Add(job);
                    db.Customers.Add(customer);

                }
                else if (job.hv)
                {
                    proforma = db.Proformas.Find(job.ProformaId);

                }
                job.CustomerId = job.hv ? proforma.CustomerId : customer.CustomerId| job.CustomerId;
                var entity = (Job)this.GetObject(job);

                db.Jobs.Add(entity);
                db.SaveChanges();
            }
                return Json(new[] { job }.ToDataSourceResult(request, ModelState));
            
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Update([DataSourceRequest]DataSourceRequest request, Job job)
        {
            if (ModelState.IsValid)
            {

                var proforma = new Proforma();
                var customer = new Customer();
                if (!job.hv && !proforma.IsExistingCustomer)
                {
                    customer = AddNewCustomer.Add(job);
                    db.Customers.Add(customer);

                }
                else if (job.hv)
                {
                    proforma = db.Proformas.Find(job.ProformaId);

                }
                job.CustomerId = job.hv ? proforma.CustomerId : customer.CustomerId | job.CustomerId;
                var entity = (Job)this.GetObject(job);
                if(job.JobId!=0)
                {
                    db.Jobs.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                if(job.JobId==0)
                {
                    db.Jobs.Add(entity);

                }

                db.SaveChanges();
            }

            return Json(new[] { job }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Destroy([DataSourceRequest]DataSourceRequest request, Job job)
        {
            if (ModelState.IsValid)
            {

                var entity = (Job)this.GetObject(job);
                db.Jobs.Attach(entity);
                db.Jobs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { job }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetCustomer(int proformaId)
        {
            var proforma = db.Proformas.FirstOrDefault(x => x.ProformaId == proformaId);

            if (proforma != null)
            {
                var customerData = Json(new
                {
                    Status = "OK",
                    CustomerName = proforma.CustomerName,
                    ContactName = proforma.ContactName,
                    ContactPhone = proforma.ContactPhone
                });

                return customerData;
            }

            var customerNoData = Json(new
            {
                Status = "OK",
                CustomerName = "",
                ContactName = "",
                ContactPhone = ""
            });

            return customerNoData;
        }
        public ActionResult ChangeOrder(ChangeOfOrderVM changeOfOrder)
        {
            var jobOrder = db.Jobs.FirstOrDefault(x => x.JobId == changeOfOrder.JobId);

            if (jobOrder != null)
            {
                //jobOrder.Quant = changeOfOrder.Quantity;
                //jobOrder.FinishingSize = changeOfOrder.Size;
                //jobOrder.TotalPrice = changeOfOrder.Total;
                //jobOrder.Reason = changeOfOrder.Reason;

                //db.Entry(jobOrder).State = EntityState.Modified;
                //db.SaveChanges();

                var changeOrder = new ChangeOrder();

                changeOrder.Date = DateTime.Now;// changeOfOrder.Date;
                changeOrder.Quantity = changeOfOrder.Quantity;
                changeOrder.Size = changeOfOrder.Size;
                changeOrder.JobId = changeOfOrder.JobId;
                changeOrder.Tax = changeOfOrder.Tax;
                changeOrder.Reason = changeOfOrder.Reason;
                changeOrder.Price = changeOfOrder.Price;
                changeOrder.Total = changeOfOrder.Total;
                changeOrder.TypeAndQuantityOfMaterialNeeded = changeOfOrder.TypeAndQuantityOfMaterialNeeded;

                db.ChangeOrders.Add(changeOrder);
                db.SaveChanges();

                TempData["Success"] = "Successfully Changed";

                return RedirectToAction("Index");
            }

            TempData["Error"] = "Invalid Data ";

            return RedirectToAction("Index");
        }
        public ActionResult GetBeforeVatAndTotalPerice(int jobTypeId, int quant, double unitPrice)
        {
            var jobType = db.JobTypes.FirstOrDefault(x => x.JobTypeId == jobTypeId);
            if (jobType != null)
            {
                var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                if (tax != null)
                {
                    double beforeTax = quant * unitPrice;
                    double withTax = (quant * unitPrice) + (quant * unitPrice * Convert.ToDouble(tax.CalculatedRate));

                    var calculatedField = Json(new
                    {
                        Status = "OK",
                        BeforeTax = beforeTax,
                        TotalPrice = withTax
                    });

                    return calculatedField;
                }
            }

            var nodata = Json(new
            {
                Status = "OK",
                BeforeTax = 0,
                TotalPrice = 0
            });

            return nodata;

        }
        public ActionResult GetTaxAndTotalPriceChangeOfOrder(int jobId, int quantity, double unitPrice)
        {
            var job = db.Jobs.FirstOrDefault(x => x.JobId == jobId);
            if (job != null)
            {
                var jobType = db.JobTypes.FirstOrDefault(x => x.JobTypeId == job.JobTypeId);
                if (jobType != null)
                {
                    var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                    if (tax != null)
                    {
                        double taxBirr = quantity * unitPrice * Convert.ToDouble(tax.CalculatedRate);
                        double withTax = (quantity * unitPrice) + (quantity * unitPrice * Convert.ToDouble(tax.CalculatedRate));

                        var calculatedField = Json(new
                        {
                            Status = "OK",
                            Tax = taxBirr,
                            Total = withTax
                        });

                        return calculatedField;
                    }
                }

            }
            var nodata = Json(new
            {
                Status = "OK",
                Tax = 0,
                Total = 0
            });

            return nodata;

        }
        private object GetObject(Job job)
        {
            var entity = new Job
            {
                JobId = job.JobId,
                hv = job.hv,
                IsExistingCustomer = job.IsExistingCustomer,
                Time = job.Time,
                ProformaId = job.ProformaId,
                JobNo = job.JobNo,
                CustomerName = job.CustomerName,
                CustomerId = job.CustomerId,
                Estimatedby = job.Estimatedby,
                Code = job.Code,
                Approvedby = job.Approvedby,
                JobTypeId = job.JobTypeId,
                BeforeTax = job.BeforeTax,
                UnitPrice = job.UnitPrice,
                Quant = job.Quant,
                Pages = job.Pages,
                Copies = job.Copies,
                TotalPrice = job.TotalPrice,
                Contract = job.Contract,
                TotalImpression = job.TotalImpression,
                DeliveryDate = job.DeliveryDate,
                TextWeight = job.TextWeight,
                CoverWeight = job.CoverWeight,
                ColorText = job.ColorText,
                Colorcover = job.Colorcover,
                FinishingSize = job.FinishingSize,
                PrintingLine = job.PrintingLine,
                PrintingMachine = job.PrintingMachine,
                Sewing = job.Sewing,
                New = job.New,
                PerfectBinding = job.PerfectBinding,
                Repeat = job.Repeat,
                Lamnating = job.Lamnating,
                Varnish = job.Varnish,
                Perforate = job.Perforate,
                Camsur = job.Camsur,
                GraphicDesignExpectedCompletetion = job.GraphicDesignExpectedCompletetion,
                PrintingExpectedCompletetion = job.PrintingExpectedCompletetion,
                FinishExpectedCompletetion = job.FinishExpectedCompletetion,
                Status = job.Status,
                CustomerDescription = job.CustomerDescription,
                OrderType = job.OrderType,
                FinishingWorkflow = job.FinishingWorkflow
            };
            return entity;
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
