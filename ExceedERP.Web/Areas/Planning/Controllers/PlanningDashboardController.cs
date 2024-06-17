using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System.Collections.Generic;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [Authorize]
    public class PlanningDashboardController : BaseController
    {
        private readonly DashboardHelper _dashboardHelper = new DashboardHelper();

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PlanningDashboard()
        {
            return View();
        }
       
       
  
        
        
    }

}