using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class ReportController : Controller
    {
       
        public ActionResult QuoteStatusReport()
        {
            return View();
        }
        public ActionResult OrderStatusReport()
        {
            return View();
        }

        public ActionResult SalesInvoiceReport()
        {
            return View();
        }

        public ActionResult CashSalesInvoiceListReport()
        {
            return View();
        }
        public ActionResult CustomerTicketReport()
        {
            return View();
        }
        public ActionResult InvoiceStatusReport()
        {
            return View();
        }


        public ActionResult CampaignPerformanceReportViewer()
        {
            return View();
        }
      
	}
}