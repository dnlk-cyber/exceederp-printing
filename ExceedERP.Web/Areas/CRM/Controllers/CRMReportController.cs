using ExceedERP.Core.Domain.CRM;
using ExceedERP.Web.Helpers;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM
{
    [Authorize]
    public class CRMReportController : Controller
    {
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult QuoteStatusReport()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult OrderStatusReport()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult OrderIssueReport()
        {
            return View();
        }

        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesInvoiceReport()
        {
            return View();
        }

        

        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult CashSalesInvoiceListReport()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.MarketingReport)]
        public ActionResult CustomerTicketReport()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult InvoiceStatusReport()
        {
            return View();
        }

        [AuthorizeRoles(CRMRoles.MarketingReport)]
        public ActionResult CampaignPerformanceReport()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult ServiceUsageReport()
        {
            return View();
        }


        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesInventoryPrint()
        {
            return View();
        }

        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult CustomerList()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesQuoteDetail()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesOrderSummary()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesOrderDetail()
        {
            return View();
        }

        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesOrderStatusDetail()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult ShipmentDetail()
        {
            return View();
        }

        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult CustomerOrderSummary()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesDetailReport()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesSummary()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SummaryByBranch()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesPlanVsActual()
        {
            return View();
        }
        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult SalesDescription()
        {
            return View();
        }

        [AuthorizeRoles(CRMRoles.SalesReport)]
        public ActionResult TenderFollowup()
        {
            return View();
        }
    }
}