using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{
    public class InventoryHomeController : Controller
    {
        // GET: Inventory/InventoryHome
        public ActionResult Index()
        {
            return View();
        }
    }
}