using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
	[AuthorizeRoles(FleetManagementRoles.FleetAdmin)] 
	public class RentRequestController : Controller
	{
		IRentRequestService _RentRequestService;
		IVehicleDemandService _VehicleDemandService;
		public RentRequestController()
		{
			_RentRequestService = new RentRequestService();
			_VehicleDemandService = new VehicleDemandService();
		}
        //
        // GET: /FleetManagement/RentRequest/
        ExceedDbContext db = new ExceedDbContext();

		public ActionResult Index()
		{
			return View();
		}




		public ActionResult RentRequest_Read([DataSourceRequest] DataSourceRequest request)
		{
			//var RentRequest = _RentRequestService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));
			IQueryable<RentRequest> RentRequest = db.RentRequest.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).AsQueryable();

			return Json(RentRequest.ToDataSourceResult(request));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentRequest_Create([DataSourceRequest] DataSourceRequest request, RentRequest RentRequest)
		{
			ModelState.Remove("RentRequestID");
			if (RentRequest.StartingDate > RentRequest.ToDate)
			{
				ModelState.Clear();
				ModelState.AddModelError("DateIncorrect", " Start Date Is Above End Date ");
				 
			}
			else
			{
				int RequestNumber = 1;
				RentRequest rentRequest = _RentRequestService.GetAll().OrderByDescending(x => x.RequestNumber).FirstOrDefault();
				if (rentRequest != null)
				{
					RequestNumber = int.Parse(rentRequest.RequestNumber.ToString()) + 1;
				}
				 if (RentRequest != null)
				{
					RentRequest.RentRequestID = Guid.NewGuid();
					RentRequest.RequestNumber = RequestNumber;
					RentRequest.Status = MaintenanceConst.Pending;
					RentRequest.CreatedBy = User.Identity.Name;
					RentRequest.DateCreated = DateTime.Now;
					RentRequest.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
					_RentRequestService.Insert(RentRequest);
					RentRequest entity = RentRequest;
					RentRequest = entity;
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "Rent Request Registration", "Success", User.Identity.Name, $"created rent request reg {entity.RentRequestID}", Modules.FleetManagement);
					#endregion
				}
			}
			return Json(new[] { RentRequest }.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentRequest_Update([DataSourceRequest] DataSourceRequest request, RentRequest RentRequest)
		{

			if (RentRequest.StartingDate > RentRequest.ToDate)
			{
				ModelState.Clear();
				ModelState.AddModelError("ExistingPlateNumber", " Start Date Is Above End Date ");

			}
			else
			{
				if (RentRequest != null)
				{
					RentRequest.ModifiedBy = User.Identity.Name;
					RentRequest.LastModified = DateTime.Now;
					RentRequest.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
					_RentRequestService.Update(RentRequest);
					#region operation log
					UserHelper.OperationLog(Request.UserHostAddress, "Rent Request Update", "Success", User.Identity.Name, $"updated rent request reg {RentRequest.RentRequestID} ", Modules.FleetManagement);
					#endregion
				}
			}

			return Json(new[] { RentRequest }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RentRequest_Destroy([DataSourceRequest] DataSourceRequest request, RentRequest RentRequest)
		{
			var rentRequest = _RentRequestService.GetById(RentRequest.RentRequestID);
			if (rentRequest != null)
			{
				_RentRequestService.Delete(rentRequest);
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Rent Request number " + RentRequest.RentRequestID);
	 
			}

			return Json(new[] { RentRequest }.ToDataSourceResult(request));
		}
		 
		public JsonResult RentRequestDDBind(string requestType)
			{
				_RentRequestService = new RentRequestService();
				_VehicleDemandService = new VehicleDemandService();
				if (requestType == "Rent")
				{
					var Requests = _RentRequestService.GetAll().Where(x => x.Status == MaintenanceConst.Approved).Select(s => new RentRequest

						{
							Description = s.RequestNumber + " --- " + s.FromBranchName + " --- " + s.FromStructureName,
							RentRequestID = s.RentRequestID,
							RequestNumber = s.RequestNumber
						});

					return Json(Requests, JsonRequestBehavior.AllowGet);
				}
				else if (requestType == "Company")
				{

					var Requests = db.VehicleDemand.Where(x => x.Status == MaintenanceConst.Approved ).Select(s => new RentRequest

					{
						Description = s.RequestNumber + " --- " + s.FromBranchName + " --- " + s.FromStructureName,
						RentRequestID = s.VehicleDemandId,
						RequestNumber = s.RequestNumber
					});

					return Json(Requests, JsonRequestBehavior.AllowGet);
				}
				else
				{
					return Json("", JsonRequestBehavior.AllowGet);
			   
				}
			}

		//public JsonResult RequestDemand(string Text)
		//{
		//	//var request = db.VehicleDemand.Where(x => x.Status == MaintenanceConst.Approved).Select(s => new
		//	//{
		//	//	Id = s.VehicleDemandId,
			  
		//	//	Name = s.RequestNumber + "-" + s.FromBranchName + "-" + s.FromStructureName
		//	//});
  //          var requests = db.VehicleDemand.Where(x => x.Status == MaintenanceConst.Approved&&(Text.ToLower().Contains(x.RequestNumber.ToString()))).ToList();
  //          var distributions = db.VehicleDistribution.Select(x => x.DistrubtionNumber).ToList();
  //          var requestsCheck = db.VehicleDemand.Where(x => x.Status == MaintenanceConst.Approved&& !distributions.Contains(x.RequestNumber)).ToList();

  //          //var UndistributedRequests = requests.Where(x => !distributions.Contains(x.RequestNumber)).Select(s => new
  //          var UndistributedRequests = requests.Select(s => new
  //          {
  //              Id = s.VehicleDemandId,

  //              Name = s.RequestNumber + "-" + s.FromBranchName + "-" + s.FromStructureName
  //          });

		//	//return Json(request, JsonRequestBehavior.AllowGet);
		//	return Json(UndistributedRequests, JsonRequestBehavior.AllowGet);
		//}
        public JsonResult RequestDemand(string Text)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                return Json(Enumerable.Empty<object>(), JsonRequestBehavior.AllowGet);
            }

            var requests = db.VehicleDemand
                .Where(x => x.Status == MaintenanceConst.Approved &&
                            x.RequestNumber.ToString().Contains(Text))
                .Select(s => new
                {
                    Id = s.VehicleDemandId,   // Value (RequestID)
            Name = s.RequestNumber + " - " +
                           s.FromBranchName + " - " +
                           s.FromStructureName   // Text (RequestNumber display)
        })
                .Take(50)   // safety limit
                .ToList();

            return Json(requests, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

