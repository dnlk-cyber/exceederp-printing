using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.JobCosting.Controllers.Common
{
    public class JobCostingDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: JobCosting/JobCostingDataProvider
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetJobOrder(int jobId)
        {
            var fetchSetting = db.PrintingJobSetting.FirstOrDefault();
            if (fetchSetting==null||(fetchSetting!=null&&fetchSetting.IsFromPrinting==false))
            {
                var jobOrder = db.Jobs.Find(jobId);
                var TotalQuantity = jobOrder.Quantity;
                var TotalSellingPrice = jobOrder.UnitPrice;
                var StartingDate = jobOrder.ReceivedDate;
                var FormattedData = new
                {
                    Quantity = TotalQuantity,
                    SellingPrice = TotalSellingPrice,
                    Date = StartingDate
                };

                return Json(FormattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var jobOrder=db.PrintingJobOrders.Find(jobId);
                var ProformaInvoice = db.PrintingProformaInvoices.FirstOrDefault(x => x.PrintingProformaInvoiceId == jobOrder.PrintingProformaInvoiceId);
                var ProformaInvoiceItem = db.PrintingProformaInvoiceItems.Where(x => x.PrintingProformaInvoiceId == jobOrder.PrintingProformaInvoiceId).ToList();
                var TotalQuantity = ProformaInvoiceItem.Sum(x => x.Quantity);
                var TotalSellingPrice = ProformaInvoiceItem.Sum(x => x.UnitPrice);
                var StartingDate = jobOrder.JobStartDate;
                var FormattedData = new
                {
                    Quantity= TotalQuantity,
                    SellingPrice= TotalSellingPrice,
                    Date= StartingDate
                };

                return Json(FormattedData, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetPrintingJobOrder(int jobId)
        {
            var jobOrder = db.Jobs.Find(jobId);
            return Json(jobOrder, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllFiscalYears()
        {
            var data = db.GLFiscalYears;
            return Json(data.Select(s => new { s.GlFiscalYearId, s.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllPeriods(int YearId)
        {
            var periods = db.GLPeriods.Where(q=>q.GlFiscalYearId==YearId).Select(s => new
            {
                GLPeriodId = s.GLPeriodId,
                Name = s.Name
            }).ToList();
            return Json(periods, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobOrders()
        {
            var fetchSetting = db.PrintingJobSetting.FirstOrDefault();
            if (fetchSetting == null || (fetchSetting != null && fetchSetting.IsFromPrinting == false))
            {
                
                var jobs = db.Jobs.ToList();

                return Json(jobs.Select(p => new
                {
                    JobId = p.JobId,
                    JobNo = p.JobNo,
                }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var jobs = db.PrintingJobOrders.ToList();
                return Json(jobs.Select(p => new
                {
                    JobId = p.PrintingJobOrderId,
                    JobNo = p.CRVNo,
                }), JsonRequestBehavior.AllowGet);
            }
        }
       
    }
}