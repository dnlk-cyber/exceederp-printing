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
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();
		public RentToExternalController()
		{
			_RentOtherSuppliersService = new RentOtherSuppliersService();
		}
		//
		// GET: /FleetManagement/RentOtherSuppliers/
		public ActionResult Index()
		{
			
                var defaultBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            ViewData["Fleets"] = db.Fleets
                .Where(s => s.OrgBranchName == defaultBranch)
                .Select(s => new
                {
                    Id = s.FleetsID.ToString(),
                    s.OrgBranchName,
                    Name = s.PlateNumber + " " + s.SerialNo
                })
                .ToList();
            ViewData["EquipmentCategories"] = CacheData.GetEquipmentIdentityCaches().Select(
      i => new
      {
          Name = i.Name,
          Id = i.Id
      }).AsQueryable();
            ViewData["EquipmentTypes"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Name = i.Name,
            Id = i.Id
        }).AsQueryable();
            ViewData["EquipmentNames"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Name = i.Name,
            Id = i.Id
        }).AsQueryable();
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            string isECWC = (fetchSetting != null && fetchSetting.Remark == "ECWC") ? "ECWC" : "Not";

            ViewData["IsECWC"] = isECWC;
            return View();
		}


		public ActionResult RentOtherSuppliers_Read([DataSourceRequest] DataSourceRequest request)
		{
            //var RentOtherSuppliers = _RentOtherSuppliersService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));
            var userBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            IQueryable<RentToExternal> RentOtherSuppliers = db.RentToExternals.Where(x => x.OrgBranchName == userBranch).AsQueryable();

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


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

