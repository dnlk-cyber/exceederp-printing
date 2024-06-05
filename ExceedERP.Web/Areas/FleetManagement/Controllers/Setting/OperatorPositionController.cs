using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;            
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement; 

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
	   [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]  
	public class OperatorPositionController : Controller
	{
		  IOperatorPositionService _OperatorPositionService;

		public OperatorPositionController()
		{
			_OperatorPositionService = new OperatorPositionService();
		}
		//
		// GET: /FleetManagement/OperatorPosition/
		public ActionResult Index()
		{

			return View();
		}


		public JsonResult OperatorPosition_Read([DataSourceRequest] DataSourceRequest request)
		{
			_OperatorPositionService = new OperatorPositionService();
			var oper = _OperatorPositionService.GetAll();
			var Opers = oper.Select(o => new OperatorPosition
			{
				OperatorPositionID = o.OperatorPositionID,
				Name = o.Name,
				Description = o.Description


			});

			return this.Json(Opers.ToDataSourceResult(request,ModelState));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult OperatorPosition_Create([DataSourceRequest] DataSourceRequest request, OperatorPosition OperatorPosition)
		{
			ModelState.Remove("OperatorPositionID");
			List<OperatorPosition> OperatorPositions = _OperatorPositionService.GetAll().Where(f => f.Name == OperatorPosition.Name).ToList();
			if (OperatorPositions.Any())
			{
				ModelState.Clear();
				ModelState.AddModelError("ExistingName", " Operator Position Already Exists ");
				return this.Json(new[] { OperatorPosition }.ToDataSourceResult(request, ModelState));
			}
			 
			if (OperatorPosition != null)
				{
					 OperatorPosition.OperatorPositionID = Guid.NewGuid();
					OperatorPosition.CreatedBy = User.Identity.Name;
					OperatorPosition.DateCreated = DateTime.Now;
				
				
					_OperatorPositionService.Insert(OperatorPosition);
				}
		 
			return this.Json(new[] { OperatorPosition }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult OperatorPosition_Update([DataSourceRequest] DataSourceRequest request, OperatorPosition OperatorPosition)
		{

			List<OperatorPosition> OperatorPositions = _OperatorPositionService.GetAll().Where(f => f.Name == OperatorPosition.Name && f.OperatorPositionID != OperatorPosition.OperatorPositionID).ToList();
			if (OperatorPositions.Any())
			{
				ModelState.Clear();
				ModelState.AddModelError("ExistingName", " Operator Position Already Exists ");
				return this.Json(new[] { OperatorPosition }.ToDataSourceResult(request, ModelState));
			}
			 
				if (OperatorPosition != null)
				{
					OperatorPosition.ModifiedBy = User.Identity.Name;
					OperatorPosition.LastModified = DateTime.Now;
				
					_OperatorPositionService.AddOrUpdate(OperatorPosition);
				}
			

			return this.Json(new[] { OperatorPosition }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult OperatorPosition_Destroy([DataSourceRequest] DataSourceRequest request, OperatorPosition OperatorPosition)
		{
			var OperatorPositions = _OperatorPositionService.GetById(OperatorPosition.OperatorPositionID);
			if (OperatorPositions != null)
			{
				_OperatorPositionService.Delete(OperatorPositions);
			}

			return this.Json(new[] { OperatorPosition }.ToDataSourceResult(request));
		}
		  [AllowAnonymous]
		public ActionResult OperatorPositionDDBind()
		{

			var fleets = _OperatorPositionService.GetAll().Select(s => new OperatorPosition
			{
				Name = s.Name,
				OperatorPositionID = s.OperatorPositionID
			});

			return Json(fleets, JsonRequestBehavior.AllowGet);
		}
		

	}
}