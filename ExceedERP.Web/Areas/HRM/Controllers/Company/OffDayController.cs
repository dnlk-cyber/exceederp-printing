using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    
    public class OffDayController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/OffDay
        public ActionResult Index()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();
            return View();
        }
    }
}