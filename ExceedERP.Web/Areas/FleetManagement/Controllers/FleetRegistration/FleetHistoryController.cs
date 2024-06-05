using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
              
	public class FleetHistoryController : Controller
	{
		IFleetHistoryService _FleetHistoryService;
		IFleetService _FleetService;
		public FleetHistoryController()
		{
		_FleetHistoryService=new FleetHistoryService();
		_FleetService = new FleetService();
		}
		//
		// GET: /FleetManagement/FleetHistory/
		public ActionResult Index()
		{
			return View();
		}

		public JsonResult FleetHistory_Read([DataSourceRequest] DataSourceRequest request)
		{
			var accessories = _FleetHistoryService.GetAll();

			return Json(accessories.ToDataSourceResult(request));
		}
		public JsonResult FleetHistory_Create([DataSourceRequest] DataSourceRequest request, FleetHistory accessories)
		{
			
			if (accessories != null)
            {
                accessories.FleetHistoryID = Guid.NewGuid();
              
		
				_FleetHistoryService.Insert(accessories);
			}

			return Json(new[] { accessories }.ToDataSourceResult(request));
		}
		public JsonResult FleetHistoryByFleet(string fleetID, [DataSourceRequest] DataSourceRequest request)
		{
			_FleetHistoryService = new FleetHistoryService();
			return Json(_FleetHistoryService.GetAll().Where(fleetHistory => fleetHistory.FleetsID.ToString() == fleetID).Select(f => new FleetHistory { FleetHistoryID=f.FleetHistoryID,AnnualDepreciationCost=f.AnnualDepreciationCost , DateCreated= f.DateCreated,CreatedBy=f.CreatedBy,FleetsID=f.FleetsID,FuelStandard=f.FuelStandard,RentalRate=f.RentalRate}).ToDataSourceResult(request));
		}
	}
}