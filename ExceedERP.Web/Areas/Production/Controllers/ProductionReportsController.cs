using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers
{
    public class ProductionReportsController : Controller
    {
        public ActionResult ProductionInputs()
        {
            return View();
        }
        public ActionResult ProductionInputsDetail()
        {
            return View();
        }
    }
}