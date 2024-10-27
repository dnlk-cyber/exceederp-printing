using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{

    public class InventoryPrintController : Controller
    {
        // GET: Inventory/InventoryPrint
        public ActionResult Index(int id  , string report)
        {   
            return View();
        }

        public ActionResult GoodSelling(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult Goodreceive(int id)
            {
                ViewBag.Id = id;
                return View();
            }
        public ActionResult IssuePrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }
        public ActionResult IssueFuelPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ItemTransferPrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }

        public ActionResult ItemTransferReceivePrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ServiceRequistionPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult PurchaseRequistionPrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }
        public ActionResult StoreRequistionPrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }
        public ActionResult StoreReturnPrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }

    }
}