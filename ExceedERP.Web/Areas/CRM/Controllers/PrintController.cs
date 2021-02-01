using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.SalesReport)] 
    public class PrintController : Controller
    {
        //
        // GET: /CRM/Print/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QuotePrint(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        public ActionResult OrderPrint(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }


        public ActionResult InvoicePrint(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult SalesInventoryPrint(int Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult CustomerTicketPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}