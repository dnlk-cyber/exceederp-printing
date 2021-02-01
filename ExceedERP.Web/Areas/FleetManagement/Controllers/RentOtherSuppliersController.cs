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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
	// [AuthorizeRoles(FleetManagementRoles.RentRequestor)] 
	public class RentOtherSuppliersController : Controller
	{
		IRentOtherSuppliersService _RentOtherSuppliersService;
        ExceedDbContext db = new ExceedDbContext();
		public RentOtherSuppliersController()
		{
			_RentOtherSuppliersService = new RentOtherSuppliersService();
		}
		//
		// GET: /FleetManagement/RentOtherSuppliers/
		public ActionResult Index()
		{
            ViewData["Fleets"] = db.Fleets.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));
			return View();
		}


		public ActionResult RentOtherSuppliers_Read([DataSourceRequest] DataSourceRequest request)
		{
            var RentOtherSuppliers = _RentOtherSuppliersService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

			return Json(RentOtherSuppliers.ToDataSourceResult(request));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentOtherSuppliers_Create([DataSourceRequest] DataSourceRequest request, RentOtherSuppliers RentOtherSuppliers)
		{
			ModelState.Remove("RentOtherSuppliersID");
		  
				 //   RentOtherSuppliers.RequestedBY = "UserIdentity";
				if (RentOtherSuppliers != null)
				{
					RentOtherSuppliers.RentOtherSuppliersID = Guid.NewGuid();
					RentOtherSuppliers.Status = MaintenanceConst.Pending;
                    RentOtherSuppliers.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
					RentOtherSuppliers.CreatedBy = User.Identity.Name;
					RentOtherSuppliers.DateCreated = DateTime.Now;
			
					_RentOtherSuppliersService.Insert(RentOtherSuppliers);
				}
		  

			return Json(new[] { RentOtherSuppliers }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentOtherSuppliers_Update([DataSourceRequest] DataSourceRequest request, RentOtherSuppliers RentOtherSuppliers)
		{
			if (RentOtherSuppliers != null)
			{
				RentOtherSuppliers.ModifiedBy = User.Identity.Name;
				RentOtherSuppliers.LastModified = DateTime.Now;
                RentOtherSuppliers.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
				_RentOtherSuppliersService.Update(RentOtherSuppliers);
			}

			return Json(new[] { RentOtherSuppliers }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentOtherSuppliers_Destroy([DataSourceRequest] DataSourceRequest request, RentOtherSuppliers RentOtherSuppliers)
		{
			var rentOtherSuppliers = _RentOtherSuppliersService.GetById(RentOtherSuppliers.RentOtherSuppliersID);
			if (rentOtherSuppliers != null)
			{
				_RentOtherSuppliersService.Delete(rentOtherSuppliers);

				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Rent from Other number " + RentOtherSuppliers.RentOtherSuppliersID);
			}

			return Json(new[] { RentOtherSuppliers }.ToDataSourceResult(request));
		}



	}
}

