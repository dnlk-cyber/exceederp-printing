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

		public ActionResult GetBranch()
		{
			return Json(db.Branch.ToList().Select(s => new
			{
				Id = s.BranchId,
				Name = s.Name
			}), JsonRequestBehavior.AllowGet);
		}

		// Ajax req
		public ActionResult GetBranchAjax(int empId)
		{
			if (empId == 0) return Json(null);
			var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
			if (placement == null) return Json(null);
			var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
			
			var orgStruct = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
			if (branch == null && orgStruct == null) return Json(null);
			var data = new
			{
				BranchId = branch.BranchId,
				BranchName = branch.Name,
				OrgName = orgStruct.Name
				
			};
			return Json(data, JsonRequestBehavior.AllowGet);
		}
	}
}