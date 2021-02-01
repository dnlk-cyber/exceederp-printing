using ExceedERP.Core.Domain.CRM;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.CRM
{
 
    public class CRMReportController : Controller
    {
           [AuthorizeRoles( CRMRoles.SalesReport)] 
        public ActionResult QuoteStatusReport()
        {
            return View();
        }
            [AuthorizeRoles( CRMRoles.SalesReport)] 
        public ActionResult OrderStatusReport()
        {
            return View();
        }
            [AuthorizeRoles( CRMRoles.SalesReport)]
            public ActionResult OrderIssueReport()
        {
            return View();
        }
        
            [AuthorizeRoles( CRMRoles.SalesReport)] 
        public ActionResult SalesInvoiceReport()
        {
            return View();
        }

            [AuthorizeRoles( CRMRoles.SalesReport)] 
        public ActionResult CashSalesInvoiceListReport()
        {
            return View();
        }
            [AuthorizeRoles( CRMRoles.MarketingReport)] 
        public ActionResult CustomerTicketReport()
        {
            return View();
        }
            [AuthorizeRoles( CRMRoles.SalesReport)] 
        public ActionResult InvoiceStatusReport()
        {
            return View();
        }

                [AuthorizeRoles( CRMRoles.MarketingReport)] 
        public ActionResult CampaignPerformanceReport()
        {
            return View();
        }
                [AuthorizeRoles( CRMRoles.SalesReport)] 
        public ActionResult ServiceUsageReport()
        {
            return View();
        }


            [AuthorizeRoles( CRMRoles.SalesReport)] 
       public ActionResult  SalesInventoryPrint()
        {
            return View();
        }


      
    }
}