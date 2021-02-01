using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.SalesReport)] 
    public class SalesOrderPrintController : Controller
    {
        //
        // GET: /Common/SalesOrderPrint/
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