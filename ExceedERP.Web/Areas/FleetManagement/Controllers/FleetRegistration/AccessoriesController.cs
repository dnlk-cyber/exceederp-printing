using System; 
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Helpers;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
	
	//[AuthorizeRoles(FleetManagementRoles.RegistrationUser ,FleetManagementRoles.RegistrationApprover,HrmConstants.HRDepartmentHead,HrmConstants.HREmployeeSelfService)] 
	public class AccessoriesController : Controller
	{
	   IAccessoriesService _AccessoriesService;
	   IFleetService _FleetService;
        ExceedDbContext db = new ExceedDbContext();
        public AccessoriesController()
	   {
		   _AccessoriesService = new AccessoriesService();
		   _FleetService = new FleetService();
          
	   }
		public ActionResult Index()
		{
		 
			return View();
		}
		 public JsonResult Accessories_Read([DataSourceRequest] DataSourceRequest request)
		{
			var Accessories = db.FleetAccessories.Where(x=>x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).AsQueryable();

			return Json(Accessories.ToDataSourceResult(request));
		}
		 public JsonResult Accessories_Create(string fleetID, [DataSourceRequest] DataSourceRequest request, Accessories Accessories)
		 {
			 _AccessoriesService = new AccessoriesService();
			 ModelState.Remove("AccessoriesID");
			 ModelState.Remove("FleetsID");
	List<Accessories> accessories = _AccessoriesService.GetAll().Where(f => f.AccessoryName == Accessories.AccessoryName  && f.AccessoriesID==Accessories.FleetsID).ToList();
	if (accessories.Any())
	{
		ModelState.Clear();
		ModelState.AddModelError("ExistingName", " Accessories with Name Already Exists ");
	}
	else
	{
		Accessories.AccessoriesID = Guid.NewGuid();
		Accessories.FleetsID = Guid.Parse(fleetID);
		Accessories.CreatedBy = User.Identity.Name;
		Accessories.DateCreated = DateTime.Now;
		Accessories.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
		   
		if (Accessories != null)
		{
			_AccessoriesService.Insert(Accessories);

			IFleetService _FleetService = new FleetService();
			Fleets fleet = _FleetService.GetAll().Where(x => x.FleetsID.ToString() == fleetID).FirstOrDefault();
			fleet.LastModified = DateTime.Now;
			_FleetService.Update(fleet);
			   #region operation log
			   UserHelper.OperationLog(Request.UserHostAddress, "Accessory registration", "Success", User.Identity.Name, $"created accissory {Accessories.AccessoriesID}", Modules.FleetManagement);
			   #endregion
				}
			}

			 return Json(new[] { Accessories }.ToDataSourceResult(request,ModelState));
		 }

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Accessories_Update([DataSourceRequest] DataSourceRequest request, Accessories Accessories)
		{
		   
				   if (Accessories != null)
				   {
					   Accessories.LastModified = DateTime.Now;
					   Accessories.ModifiedBy = User.Identity.Name;
					   Accessories.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
					   _AccessoriesService.Update(Accessories);
					   #region operation log
					   UserHelper.OperationLog(Request.UserHostAddress, "Accessory registration", "Success", User.Identity.Name, $"updated Accessory {Accessories.AccessoriesID}", Modules.FleetManagement);
					   #endregion
			}

			return Json(new[] { Accessories }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult Accessories_Destroy([DataSourceRequest] DataSourceRequest request, Accessories accessories)
		{
			var Accessories = _AccessoriesService.GetById(accessories.AccessoriesID);

			if (Accessories != null)
			{
				_AccessoriesService.Delete(Accessories);
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Accessory " + accessories.AccessoriesID);
			}

			return Json(new[] { Accessories }.ToDataSourceResult(request));
		}
		public JsonResult AccessoriesByFleetInspection(string fleetID)
		{
			_AccessoriesService = new AccessoriesService();
			return Json(_AccessoriesService.GetAll().Where(accessories => accessories.FleetsID.ToString() == fleetID).Select(a => new Accessories { SerialNo = a.SerialNo, AccessoriesID = a.AccessoriesID, AccessoryName = a.AccessoryName, Description = a.Description, FleetsID = a.FleetsID, IssuedDate = a.IssuedDate, Quantity = a.Quantity, UnitofMeasurement = a.UnitofMeasurement, UnitPrice = a.UnitPrice }), JsonRequestBehavior.AllowGet);
		}
	  
	

		public JsonResult AccessoriesByFleet(string fleetID, [DataSourceRequest] DataSourceRequest request)
		{
			_AccessoriesService = new AccessoriesService();

			return Json(_AccessoriesService.GetAll().Where(accessories => accessories.FleetsID == Guid.Parse(fleetID.ToString())).Select(a => new Accessories { SerialNo = a.SerialNo, AccessoriesID=a.AccessoriesID,AccessoryName=a.AccessoryName,Description=a.Description,FleetsID=a.FleetsID,IssuedDate=a.IssuedDate,Quantity=a.Quantity,UnitofMeasurement=a.UnitofMeasurement,UnitPrice=a.UnitPrice}).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
		}


		
		public ActionResult ApprovalDDBind()
		{
			var approvals = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();
			var approval = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
			{
				Name = MaintenanceConst.Approved,
				Id = 1,
			};
			approvals.Add(approval);

			var approval2 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
			{
				Name = MaintenanceConst.Pending,
				Id = 2,
			};
			approvals.Add(approval2);
			var approval3 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
			{
				Name = MaintenanceConst.Rejected,
				Id = 3,
			};
			approvals.Add(approval3);
			return Json(approvals, JsonRequestBehavior.AllowGet);
		}
	 
	
	}
}