using System;
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
using ExceedERP.Web.Areas.ProductionFollowUp.Controllers.ViewModel;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Operation
{
    public class JobOrdersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Proforma"] = db.Proformas.ToList();
            ViewData["JobType"] = db.JobTypes.ToList();
            ViewData["JobTypeItem"] = db.JobTypeItems.ToList();
            return View();
        }

        public ActionResult Jobs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Job> jobs = db.Jobs;
            DataSourceResult result = jobs.ToDataSourceResult(request, job => new
            {
                JobId = job.JobId,
                Time = job.Time,
                HaveProforma = job.HaveProforma,
                ProformaId = job.ProformaId,
                JobNo = job.JobNo,
                CustomerName = job.CustomerName,
                Estimatedby = job.Estimatedby,
                Code = job.Code,
                Approvedby = job.Approvedby,
                ContactName = job.ContactName,
                ContactPhone = job.ContactPhone,
                City = job.City,
                SubCity = job.SubCity,
                Wereda = job.Wereda,
                Pobox = job.Pobox,
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
                var entity = new Job
                {
                    Time = job.Time,
                    ProformaId = job.ProformaId,
                    HaveProforma = job.HaveProforma,
                    JobNo = job.JobNo,
                    CustomerName = job.CustomerName,
                    ContactName = job.ContactName,
                    ContactPhone = job.ContactPhone,
                    Estimatedby = job.Estimatedby,
                    Code = job.Code,
                    Approvedby = job.Approvedby,
                    City = job.City,
                    SubCity = job.SubCity,
                    Wereda = job.Wereda,
                    Pobox = job.Pobox,
                    JobTypeId = job.JobTypeId,
                    JobTypeItemId = job.JobTypeItemId,
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
                    FinishingWorkflow = job.FinishingWorkflow
                };

                db.Jobs.Add(entity);
                db.SaveChanges();
                job.JobId = entity.JobId;

                #region

                if (job.JobId.ToString().Length == 1)
                {
                    entity.JobNo = "jo-000000" + job.JobId;
                }
                else if (job.JobId.ToString().Length == 2)
                {
                    entity.JobNo = "jo-00000" + job.JobId;
                }
                else if (job.JobId.ToString().Length == 3)
                {
                    entity.JobNo = "jo-0000" + job.JobId;
                }
                else if (job.JobId.ToString().Length == 4)
                {
                    entity.JobNo = "jo-000" + job.JobId;
                }
                else if (job.JobId.ToString().Length == 5)
                {
                    entity.JobNo = "jo-00" + job.JobId;
                }
                else if (job.JobId.ToString().Length == 2)
                {
                    entity.JobNo = "jo-0" + job.JobId;
                }
                else if (job.JobId.ToString().Length == 2)
                {
                    entity.JobNo = "jo-" + job.JobId;
                }

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #endregion
            }

            return Json(new[] { job }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Update([DataSourceRequest]DataSourceRequest request, Job job)
        {
            if (ModelState.IsValid)
            {
                var entity = new Job
                {
                    JobId = job.JobId,
                    HaveProforma = job.HaveProforma,
                    Time = job.Time,
                    ProformaId = job.ProformaId,
                    JobNo = job.JobNo,
                    CustomerName = job.CustomerName,
                    Estimatedby = job.Estimatedby,
                    Code = job.Code,
                    Approvedby = job.Approvedby,
                    ContactName = job.ContactName,
                    ContactPhone = job.ContactPhone,
                    City = job.City,
                    SubCity = job.SubCity,
                    Wereda = job.Wereda,
                    Pobox = job.Pobox,
                    JobTypeId = job.JobTypeId,
                    JobTypeItemId = job.JobTypeItemId,
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
                    FinishingWorkflow = job.FinishingWorkflow
                };

                db.Jobs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { job }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Destroy([DataSourceRequest]DataSourceRequest request, Job job)
        {
            if (ModelState.IsValid)
            {
                var entity = new Job
                {
                    JobId = job.JobId,
                    HaveProforma = job.HaveProforma,
                    Time = job.Time,
                    ProformaId = job.ProformaId,
                    JobNo = job.JobNo,
                    CustomerName = job.CustomerName,
                    Estimatedby = job.Estimatedby,
                    Code = job.Code,
                    Approvedby = job.Approvedby,
                    ContactName = job.ContactName,
                    ContactPhone = job.ContactPhone,
                    City = job.City,
                    SubCity = job.SubCity,
                    Wereda = job.Wereda,
                    Pobox = job.Pobox,
                    JobTypeId = job.JobTypeId,
                    JobTypeItemId = job.JobTypeItemId,
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
                    FinishingWorkflow = job.FinishingWorkflow
                };

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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
