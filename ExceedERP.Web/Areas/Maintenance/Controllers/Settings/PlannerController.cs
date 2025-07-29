using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.Settings
{
       [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]  
	public class PlannerController : Controller
	{
	   IPlannerService _PlannerService;
		IFleetService _FleetService;
		public PlannerController()
		{
			_PlannerService = new PlannerService();
			_FleetService = new FleetService();
		}
	 
		//
		// GET: /FleetManagement/Planner/
		public ActionResult Index()
		{
			return View();
		}

	   
		public ActionResult Planner_Read([DataSourceRequest] DataSourceRequest request)
		{
			var Planner = _PlannerService.GetAll().Select(p => new Planner 
			{ 
			PlannerName=p.PlannerName,
			PlannerID=p.PlannerID,
			TrackByDate=p.TrackByDate,
			TrackByMileage=p.TrackByMileage,
			DateCrated=p.DateCrated
			});

			return Json(Planner.ToDataSourceResult(request));
		}
	  

		  [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Planner_Create([DataSourceRequest] DataSourceRequest request, Planner Planner)
		{
			ModelState.Remove("PlannerID");
			List<Planner> Planners = _PlannerService.GetAll().Where(f => f.PlannerName == Planner.PlannerName).ToList();
			if (Planners.Any())
			  {
				  ModelState.Clear();
				  ModelState.AddModelError("ExistingName", " Planners with Name Already Exists ");
			  }
			  else
			  {
				 
				  if (Planner != null)
				  {
                      Planner.PlannerID = Guid.NewGuid();
                      Planner.DateCrated = DateTime.Now;
                      Planner.CreatedBy = User.Identity.Name;
                      Planner.DateCreated = DateTime.Now;
				
					  _PlannerService.Insert(Planner);
				  }
			  }

			return Json(new[] { Planner }.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Planner_Update([DataSourceRequest] DataSourceRequest request, Planner Planner)
		{
			if (Planner != null )
			{
                Planner.ModifiedBy = User.Identity.Name;
                Planner.LastModified = DateTime.Now;
				
				_PlannerService.Update(Planner);
			}

			return Json(new[] {Planner}.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Planner_Destroy([DataSourceRequest] DataSourceRequest request, Planner planner)
		{
			var Planner = _PlannerService.GetById(planner.PlannerID);
			if (Planner != null)
			{
				_PlannerService.Delete(Planner);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Plan number " + planner.PlannerID);

			}

			return Json(new[] { Planner }.ToDataSourceResult(request));
		}
          
		  public ActionResult PlannerDDBind()
		{

			var fleets = _PlannerService.GetAll().Select(s => new Planner
			{
				PlannerName = s.PlannerName,
				PlannerID = s.PlannerID
			});

			return Json(fleets, JsonRequestBehavior.AllowGet);
		}


	}
	

	}


