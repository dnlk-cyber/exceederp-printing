using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    public class ScheduleSettingsController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ScheduleSettings
        public ActionResult Index()
        {

            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;
            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();
            var branches = db.Branch.ToList();
            ViewData["Branch"] = branches;

            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();

            return View();
        }
    }
}