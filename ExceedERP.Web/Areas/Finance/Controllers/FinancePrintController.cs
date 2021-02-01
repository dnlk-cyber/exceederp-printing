using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    public class FinancePrintController : Controller
    {
        // GET: Finance/FinancePrint
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApTravelOrder(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ApTravelExpenseClaim(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult GlJournalVoucher(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ApPayment(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ApMiscPayment(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ApPaymentSettlement(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ApDirectPayment(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ArSalesOrder(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ApPettyCash(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ApPaymentRequest(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ArSalesOrderInvoice(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ArMiscCollection(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ArInvoice(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ArCollection(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ApInvoice(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        
    }
}