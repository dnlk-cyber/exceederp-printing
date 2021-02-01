using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class APTSController : Controller
    {
        // GET: APTS/APTS
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DrugStore()
        {
            return View();
        }
        public ActionResult DespensaryStore()
        {
            return View();
        }
        public ActionResult PatientRecored()
        {
            return View();
        }
        public ActionResult MonthlyFinancialReport()
        {
            return View();
        }
        public ActionResult DrugByExpiry()
        {
            return View();
        }
        public ActionResult DrugSale()
        {
            return View();
        }
        public ActionResult DrugSaleWithDrugist()
        {
            return View();
        }
    }
}