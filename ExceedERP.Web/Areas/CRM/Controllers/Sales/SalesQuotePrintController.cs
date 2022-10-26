using ExceedERP.Web.Helpers;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesReport)] 
    public class SalesQuotePrintController : Controller
    {
        //
        // GET: /Common/SalesQuotePrint/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Print(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
	}
}