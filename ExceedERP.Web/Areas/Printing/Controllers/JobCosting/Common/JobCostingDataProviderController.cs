using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting.Common
{
    public class JobCostingDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: JobCosting/JobCostingDataProvider
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetJobOrder(int jobId)
        {
            var jobOrder = db.Jobs.Find(jobId);
            return Json(jobOrder, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllFiscalYears()
        {
            var data = db.GLFiscalYears;
            return Json(data.Select(s => new { s.GlFiscalYearId, s.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllPeriods(int YearId)
        {
            var periods = db.GLPeriods.Where(q=>q.GlFiscalYearId==YearId).Select(s => new
            {
                GLPeriodId = s.GLPeriodId,
                Name = s.Name
            }).ToList();
            return Json(periods, JsonRequestBehavior.AllowGet);
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