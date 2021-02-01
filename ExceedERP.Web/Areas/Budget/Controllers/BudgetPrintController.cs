using ExceedERP.Core.Domain.Budget;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     


namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetReport)]
    public class BudgetPrintController : Controller
    {
        public ActionResult BudgetRequest(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }

        public ActionResult BudgetRequestTransfer(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult BudgetRequestTransferNoHeader(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        public ActionResult BudgetTransfer(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
          public ActionResult BudgetTransferNoHeader(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        
    }
}