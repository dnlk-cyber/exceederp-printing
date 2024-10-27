using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    [Authorize]
    public class FinanceHomeController : Controller
    {
        // GET: Finance/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}