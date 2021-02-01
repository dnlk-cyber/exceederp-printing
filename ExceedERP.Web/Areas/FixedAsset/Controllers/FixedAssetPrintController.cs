using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers
{

    public class FixedAssetPrintController : Controller
    {
        
        public ActionResult Index(int id  , string report)
        {   
            return View();
        }

        public ActionResult FixedAssetReturn(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult FixedAssetReceive(int id)
            {
                ViewBag.Id = id;
                return View();
            }
        public ActionResult MachineryAndVehicleTransfer(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult MachineryAndVehicleTransferReceive(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult FixedAssetIssuePrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }
        public ActionResult FixedAssetTransferPrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }
        public ActionResult FixedAssetTransferReceivePrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult FixedAssetPurchaseRequistionPrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }
        public ActionResult FixedAssetRequistionPrint(int id)
            {
                ViewBag.Id = id;
                return View();
            }
       
        public ActionResult PurchaseRequistionPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

    }
}