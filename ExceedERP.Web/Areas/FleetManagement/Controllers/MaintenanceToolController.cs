using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
       [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]  
	public class MaintenanceToolController : Controller
	{
	  IMaintenanceToolService _MaintenanceToolService;
		IFleetService _FleetService;
		public MaintenanceToolController()
		{
			_MaintenanceToolService = new MaintenanceToolService();
			_FleetService = new FleetService();
		}
	 
		//
		// GET: /FleetManagement/MaintenanceTool/
		public ActionResult Index()
		{
			return View();
		}

	   
		public ActionResult MaintenanceTool_Read([DataSourceRequest] DataSourceRequest request)
		{
			var MaintenanceTool = _MaintenanceToolService.GetAll();

			return Json(MaintenanceTool.ToDataSourceResult(request));
		}
	  

		  [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult MaintenanceTool_Create([DataSourceRequest] DataSourceRequest request, MaintenanceTool MaintenanceTool)
		{
			ModelState.Remove("MaintenanceToolID");
				 List<MaintenanceTool> maintenanceTool = _MaintenanceToolService.GetAll().Where(f => f.ToolsName == MaintenanceTool.ToolsName).ToList();
				 if (maintenanceTool.Any())
				 {
					 ModelState.Clear();
					 ModelState.AddModelError("ExistingName", " Maintenance Tool with Name Already Exists ");
				 }
				 else
				 {
					 MaintenanceTool.MaintenanceToolID = Guid.NewGuid();
					 if (MaintenanceTool != null)
					 {
						 MaintenanceTool.CreatedBy = User.Identity.Name;
						 MaintenanceTool.DateCreated = DateTime.Now;
						 _MaintenanceToolService.Insert(MaintenanceTool);
					 }
				 }
			return Json(new[] { MaintenanceTool }.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult MaintenanceTool_Update([DataSourceRequest] DataSourceRequest request, MaintenanceTool MaintenanceTool)
		{
					
						   if (MaintenanceTool != null)
						   {
                               MaintenanceTool.ModifiedBy = User.Identity.Name;
                               MaintenanceTool.LastModified = DateTime.Now;
							   _MaintenanceToolService.Update(MaintenanceTool);
						   }
					  
			return Json(new[] {MaintenanceTool}.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult MaintenanceTool_Destroy([DataSourceRequest] DataSourceRequest request, MaintenanceTool maintenanceTool)
		{
			var MaintenanceTool = _MaintenanceToolService.GetById(maintenanceTool.MaintenanceToolID);
			if (MaintenanceTool != null)
			{
				_MaintenanceToolService.Delete(MaintenanceTool);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Maintenance Tool number " + maintenanceTool.MaintenanceToolID);

			}

			return Json(new[] { MaintenanceTool }.ToDataSourceResult(request));
		}
          [AllowAnonymous]
		public ActionResult MaintenanceToolDDBind()
		{

			var fleets = _MaintenanceToolService.GetAll().Select(s => new MaintenanceTool
			{
				MaintenanceToolID = s.MaintenanceToolID,
				ToolsName = s.ToolsName
			});

			return Json(fleets, JsonRequestBehavior.AllowGet);
		}


	}
}

