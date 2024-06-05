using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Web.Helpers;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover)] 
	  
	public class AssignEquipmentApprovalController : Controller
	{
		//

		// GET: /FleetManagement/AssignEquipmentApproval/
	 ExceedDbContext db = new  ExceedDbContext();
	 IAssignedEquipmentService _EquipmentPlanandEvaluationsService;

		public AssignEquipmentApprovalController()
		{
			_EquipmentPlanandEvaluationsService = new AssignedEquipmentService();
		}
		public ActionResult Index()
		{
		   
			return View();
		}
		 
	
	}
}