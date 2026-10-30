using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    public class GenerateLeaveEntitlementController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/GenerateLeaveEntitlement
        public ActionResult Index()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}