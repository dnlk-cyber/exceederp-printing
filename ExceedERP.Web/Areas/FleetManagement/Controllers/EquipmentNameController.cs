using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
        [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]  
	public class EquipmentNameController : Controller
	{
	   IEquipmentNameService _EquipmentNameService;

		public EquipmentNameController()
		{
			_EquipmentNameService = new EquipmentNameService();
		}
		//
		// GET: /FleetManagement/EquipmentName/
		public ActionResult Index()
		{

			return View();
		}


		public JsonResult EquipmentName_Read([DataSourceRequest] DataSourceRequest request)
		{
			_EquipmentNameService = new EquipmentNameService();
			var oper = _EquipmentNameService.GetAll();
			var Opers = oper.Select(o => new EquipmentName
			{
				EquipmentNameID = o.EquipmentNameID,
				Name = o.Name,
				Description = o.Description

			});

			return this.Json(Opers.ToDataSourceResult(request,ModelState));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult EquipmentName_Create([DataSourceRequest] DataSourceRequest request, EquipmentName EquipmentName)
		{
			ModelState.Remove("EquipmentNameID");
			List<EquipmentName> EquipmentNames = _EquipmentNameService.GetAll().Where(f => f.Name == EquipmentName.Name).ToList();
			if (EquipmentNames.Any())
			{
				ModelState.Clear();
				ModelState.AddModelError("ExistingName", " Equipment Name  Already Exists ");
			}
			else
			{
				

				if (EquipmentName != null)
				{
					EquipmentName.EquipmentNameID = Guid.NewGuid();
					EquipmentName.CreatedBy = User.Identity.Name;
					EquipmentName.DateCreated = DateTime.Now;
					EquipmentName.IsAssigned = false;
					_EquipmentNameService.Insert(EquipmentName);
				}
			}
			return this.Json(new[] { EquipmentName }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult EquipmentName_Update([DataSourceRequest] DataSourceRequest request, EquipmentName EquipmentName)
		{
		   
				if (EquipmentName != null)
				{
					EquipmentName.ModifiedBy = User.Identity.Name;
					EquipmentName.LastModified = DateTime.Now;
				
					_EquipmentNameService.Update(EquipmentName);
				}
			

			return this.Json(new[] { EquipmentName }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult EquipmentName_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentName EquipmentName)
		{
			var EquipmentNames = _EquipmentNameService.GetById(EquipmentName.EquipmentNameID);
			if (EquipmentNames != null)
			{
				_EquipmentNameService.Delete(EquipmentNames);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Name number " + EquipmentName.EquipmentNameID);

			}

			return this.Json(new[] { EquipmentName }.ToDataSourceResult(request));
		}
             [AllowAnonymous]
		public ActionResult EquipmentNameDDBind()
		{

			var fleets = _EquipmentNameService.GetAll().Select(s => new EquipmentName
			{
				Name = s.Name,
				EquipmentNameID = s.EquipmentNameID
			});

			return Json(fleets, JsonRequestBehavior.AllowGet);
		}
		

	}
}