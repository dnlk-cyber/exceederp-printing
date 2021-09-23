using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.JobCosting.Common
{
    public class JobCostingDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: JobCosting/JobCostingDataProvider
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetJobOrders()
        {
            var jobs = db.Jobs.ToList();

            return Json(jobs.Select(p => new
            {
                JobId = p.JobId,
                JobNo = p.JobNo,
            }), JsonRequestBehavior.AllowGet);
        }
    }
}