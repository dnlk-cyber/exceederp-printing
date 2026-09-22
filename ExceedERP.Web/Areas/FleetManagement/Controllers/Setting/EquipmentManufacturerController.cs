using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
       [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]  
	public class EquipmentManufacturerController : Controller
	{
			  IManufacturerService _ManufacturerService;

		public EquipmentManufacturerController()
		{
			_ManufacturerService = new ManufacturerService();
		}
		//
		// GET: /FleetManagement/Manufacturer/
		public ActionResult Index()
		{

			return View();
		}


		public JsonResult Manufacturer_Read([DataSourceRequest] DataSourceRequest request)
		{
			_ManufacturerService = new ManufacturerService();
			var oper = _ManufacturerService.GetAll();
			var Opers = oper.Select(o => new Manufacturer
			{
				ManufacturerID = o.ManufacturerID,
				Name = o.Name,
				Description = o.Description
			
			});

			return this.Json(Opers.ToDataSourceResult(request,ModelState));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Manufacturer_Create([DataSourceRequest] DataSourceRequest request, Manufacturer Manufacturer)
		{
			ModelState.Remove("ManufacturerID");
			List<Manufacturer> Manufacturers = _ManufacturerService.GetAll().Where(f => f.Name == Manufacturer.Name).ToList();
			if (Manufacturers.Any())
			{
				ModelState.Clear();
				ModelState.AddModelError("ExistingName", "Equipment Manufacturer Name Already Exists ");
			}
			else
			{
				Manufacturer.ManufacturerID = Guid.NewGuid();

				if (Manufacturer != null)
				{
					Manufacturer.CreatedBy = User.Identity.Name;
					Manufacturer.DateCreated = DateTime.Now;
							
					_ManufacturerService.Insert(Manufacturer);
				}
			}
			return this.Json(new[] { Manufacturer }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Manufacturer_Update([DataSourceRequest] DataSourceRequest request, Manufacturer Manufacturer)
		{
		   
				if (Manufacturer != null)
				{
					Manufacturer.ModifiedBy = User.Identity.Name;
					Manufacturer.LastModified = DateTime.Now;
				
					_ManufacturerService.AddOrUpdate(Manufacturer);
				}
			

			return this.Json(new[] { Manufacturer }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Manufacturer_Destroy([DataSourceRequest] DataSourceRequest request, Manufacturer Manufacturer)
		{
			var Manufacturers = _ManufacturerService.GetById(Manufacturer.ManufacturerID);
			if (Manufacturers != null)
			{
				_ManufacturerService.Delete(Manufacturers);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Manufacturer number " + Manufacturer.ManufacturerID);

			}

			return this.Json(new[] { Manufacturer }.ToDataSourceResult(request));
		}
            
		public ActionResult ManufacturerDDBind()
		{

			var fleets = _ManufacturerService.GetAll().Select(s => new Manufacturer
			{
				Name = s.Name,
				ManufacturerID = s.ManufacturerID
			});

			return Json(fleets, JsonRequestBehavior.AllowGet);
		}

        }
}