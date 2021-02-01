using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
        [AuthorizeRoles( CRMRoles.CashSalesUser)] 
    public class CashSalesPrintController : Controller
    {
        //
        // GET: /Common/CashSalesPrint/
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