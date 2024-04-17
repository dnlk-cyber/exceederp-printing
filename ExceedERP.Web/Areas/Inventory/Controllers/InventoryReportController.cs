using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Web.Areas.Inventory.Models;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{
    [AuthorizeRoles(InventoryConstants.PropertyReport )]
    public class InventoryReportController : Controller
    {
        public ActionResult StockBalanceByQTYValueReport()
        {
            return View();
        }
        public ActionResult StockStatusReport()
        {
            return View();
        }
        public ActionResult DeadStockReport()
        {
            return View();
        }
        public ActionResult PurchaseRequistionReport()
        {
            return View();
        }
        public ActionResult ConsumptionByworkUnit()
        {
            return View();
        }
        public ActionResult StoreRequistionItemByDateInterval()
        {
            return View();
        }
        public ActionResult GoodsReceiveByDateInterval()
        {
            return View();
        }

        public ActionResult IssueItemByDateInterval()
        {
            return View();
        }
        public ActionResult ItemTransferByDateInterval()
        {
            return View();
        }
        public ActionResult PurchaseReturnItemByDateInterval()
        {
            return View();
        }
        public ActionResult StockAdjustmentItem()
        {
            return View();
        }
        public ActionResult StockBalanceByCategory()
        {
            return View();
        }
        public ActionResult StoreReturnItemByDateInterval()
        {
            return View();
        } 
        public ActionResult StoreStock()
        {
            return View();
        } 
        
        public ActionResult BinCard()
        {
            return View();
        }
        public ActionResult InventoryCount()
        {
            return View();
        }
        public ActionResult Print()
        {
            return View();
        }
        public ActionResult ItemInStore()
        {
            return View();
        } 
        public ActionResult Variation()
        {
            return View();
        }

        public ActionResult VariationSummary()
        {
            return View();
        }

        public ActionResult Consumption()
        {
            return View();
        }

        public ActionResult ConsumptionByProject()
        {
            return View();
        }
    }
}