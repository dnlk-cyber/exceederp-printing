using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
     
	
    public class BranchController : Controller
        {

        private IBranchService _BranchService;

        ExceedDbContext db = new ExceedDbContext();

     

        public BranchController()
            {

            _BranchService=new BranchService();

            }

        // GET: FleetManagement/Branch
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult BranchDDBind()
        {
            List<Branch> branches = new List<Branch>();
             branches = db.Branch.ToList();
            return Json(branches, JsonRequestBehavior.AllowGet);
        }
    }
}