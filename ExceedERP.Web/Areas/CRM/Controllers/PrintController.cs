using ExceedERP.Web.Helpers;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesReport)] 
    public class PrintController : Controller
    {
        //
        // GET: /CRM/Print/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QuotePrint(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult PriceEstimation(int Id)
        {
            var db = new ExceedDbContext();
            var estimation = db.PriceEstimationQuoteVouchers.Find(Id);
            ViewBag.Id = Id;
            return View();
        }

        public ActionResult OrderPrint(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }


        public ActionResult InvoicePrint(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult CashReceiptVoucher(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult CreditReceiptVoucher(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult SalesInventoryPrint(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult CustomerTicketPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult Shipment(int id)
        {
            ViewBag.ShipmentId = id;
            return View();
        }
        public ActionResult CreditSettlement(int id)
        {
            ViewBag.CreditSettlement = id;
            return View();
        }
    }
}