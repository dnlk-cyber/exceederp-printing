using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
	// [AuthorizeRoles(FleetManagementRoles.RentRequestor)] 
	public class RentToExternalController : Controller
	{
		IRentOtherSuppliersService _RentOtherSuppliersService;
		ExceedDbContext db = new ExceedDbContext();
		public RentToExternalController()
		{
			_RentOtherSuppliersService = new RentOtherSuppliersService();
		}
		//
		// GET: /FleetManagement/RentOtherSuppliers/
		public ActionResult Index()
		{
			ViewData["Fleets"] = db.Fleets.ToList().Select(s=>new {
			Id = s.FleetsID.ToString(),
			s.OrgBranchName,
			Name = s.EquipmentName
			}).Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

			ViewData["EquipmentCategories"] = db.EquipmentIdentity.ToList().Select(s => new {
				Id = s.EquipmentIdentityID,
				Name = s.EquipmentCategoryName
			});
			ViewData["EquipmentTypes"] = db.EquipmentType.ToList().Select(s => new {
				Id = s.EquipmentTypeID,
				Name = s.EquipmentTypeName
			});
			ViewData["EquipmentNames"] = db.EquipmentName.ToList().Select(s => new {
				Id = s.EquipmentNameID,
				Name = s.Name
			});
			return View();
		}


		public ActionResult RentOtherSuppliers_Read([DataSourceRequest] DataSourceRequest request)
		{
			var RentOtherSuppliers = _RentOtherSuppliersService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

			return Json(RentOtherSuppliers.ToDataSourceResult(request));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentOtherSuppliers_Create([DataSourceRequest] DataSourceRequest request, RentToExternal RentOtherSuppliers)
		{
			ModelState.Remove("RentToExternalID");
		  
				 //   RentOtherSuppliers.RequestedBY = "UserIdentity";
				if (RentOtherSuppliers != null)
				{
					RentOtherSuppliers.RentToExternalID = Guid.NewGuid();
					RentOtherSuppliers.Status = MaintenanceConst.Pending;
					RentOtherSuppliers.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
					RentOtherSuppliers.CreatedBy = User.Identity.Name;
					RentOtherSuppliers.DateCreated = DateTime.Now;
			
					_RentOtherSuppliersService.Insert(RentOtherSuppliers);
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Rent to external registartion", "Success", User.Identity.Name, $"created rent to external reg {RentOtherSuppliers.RentToExternalID}", Modules.FleetManagement);
				#endregion
			}


			return Json(new[] { RentOtherSuppliers }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentOtherSuppliers_Update([DataSourceRequest] DataSourceRequest request, RentToExternal RentOtherSuppliers)
		{
			if (RentOtherSuppliers != null)
			{
				RentOtherSuppliers.ModifiedBy = User.Identity.Name;
				RentOtherSuppliers.LastModified = DateTime.Now;
				RentOtherSuppliers.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
				_RentOtherSuppliersService.Update(RentOtherSuppliers);
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Rent other supliers service", "Success", User.Identity.Name, $"updated other supliers reg {RentOtherSuppliers.RentToExternalID} ", Modules.FleetManagement);
				#endregion
			}

			return Json(new[] { RentOtherSuppliers }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentOtherSuppliers_Destroy([DataSourceRequest] DataSourceRequest request, RentToExternal RentOtherSuppliers)
		{
			var rentOtherSuppliers = _RentOtherSuppliersService.GetById(RentOtherSuppliers.RentToExternalID);
			if (rentOtherSuppliers != null)
			{
				_RentOtherSuppliersService.Delete(rentOtherSuppliers);

				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Rent to other number " + RentOtherSuppliers.RentToExternalID);
			}

			return Json(new[] { RentOtherSuppliers }.ToDataSourceResult(request));
		}



	}
}

