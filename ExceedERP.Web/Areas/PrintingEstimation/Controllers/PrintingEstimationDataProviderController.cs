using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.PrintingEstimation.Controllers
{
    public class PrintingEstimationDataProviderController : Controller
    {
        ExceedDbContext _db = new ExceedDbContext();
        // GET: PrintingEstimation/PrintingEstimationDataProvider
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllStores()
        {
            var stores = _db.InventoryStores.ToList().Select(s => new
            {

                s.StoreCode,
                s.Name

            });
            return Json(stores, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemDetail(string itemCode, string storeCode)
        {
            var item = _db.StoreItemAssignments.FirstOrDefault(q => q.StoreCode == storeCode && q.ItemCode == itemCode);
            if(item!=null)
            {
                return Json(new { Balance = item.Balance,AvgPrice= item.AvgPrice }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Balance = 0, AvgPrice = 0 }, JsonRequestBehavior.AllowGet);
        }
    }
}