using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.MainTools
{
    public class MaintenanceToolsReturnController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            var mechanicCrews = db.MechanicCrew.Select(mc => new
            {
                Code = mc.MechanicCrewID,
                Name = mc.Person_Employee.FirstName + " " + mc.Person_Employee.MiddleName + " " + mc.Person_Employee.LastName
            }).ToList();
            ViewData["MechanicCrews"] = mechanicCrews;
            ViewData["MeaintenanceTools"] = db.MaintenanceTool.Select( mc => new { mc.MaintenanceToolID, mc.ToolsName}).ToList();
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}