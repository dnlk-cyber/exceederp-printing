using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [Authorize]
    public class BudgetHomeController : Controller
    {
        // GET: Budget/BudgetHome
        public ActionResult Index()
        {
            return View();
        }
    }
}