using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
     [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor)] 
	public class OutsourceMaintenanceController : Controller
	{
	  IOutsourceMaintenanceService _OutsourceMaintenanceService;
		IFleetService _FleetService;

        ExceedDbContext db = new ExceedDbContext();
		public OutsourceMaintenanceController()
		{
			_OutsourceMaintenanceService = new OutsourceMaintenanceService();
			_FleetService = new FleetService();
		}
	 
		//
		// GET: /FleetManagement/OutsourceMaintenance/
		public ActionResult Index()
		{
			return View();
		}

	   
		public ActionResult OutsourceMaintenance_Read([DataSourceRequest] DataSourceRequest request)
		{
            var OutsourceMaintenance = _OutsourceMaintenanceService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

			return Json(OutsourceMaintenance.ToDataSourceResult(request));
		}
        public string GetJobStatus(string Id)
        {
            JobOrder JobRequests = db.JobOrder.Where(j => j.JobOrderID.ToString() == Id && (j.IsOnlineTransferred == true || j.IsOnlineApproved == true || j.IsSecondOnlineApproved == true)).FirstOrDefault();
            if (JobRequests != null)
            {
                return JobRequests.Status;
            }

            return "Pending";


        }



		  [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult OutsourceMaintenance_Create([DataSourceRequest] DataSourceRequest request, OutsourceMaintenance OutsourceMaintenance)
		{
			ModelState.Remove("OutsourceMaintenanceID");
			if (OutsourceMaintenance.fromDate > OutsourceMaintenance.toDate)
				  {
					  ModelState.Clear();
					  ModelState.AddModelError("", "Start Date is Above End Date");
				  }
			  else
				  {
					  OutsourceMaintenance.OutsourceMaintenanceID = Guid.NewGuid();
					  if (OutsourceMaintenance != null)
						  {
                              OutsourceMaintenance.CreatedBy = User.Identity.Name;
                              OutsourceMaintenance.DateCreated = DateTime.Now;
				
				
		
							  _OutsourceMaintenanceService.Insert(OutsourceMaintenance);
						  }

				  }
			  return Json(new[] { OutsourceMaintenance }.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult OutsourceMaintenance_Update([DataSourceRequest] DataSourceRequest request, OutsourceMaintenance OutsourceMaintenance)
		{
						ModelState.Remove("OutsourceMaintenanceID");
			if (OutsourceMaintenance.fromDate > OutsourceMaintenance.toDate)
				{
					ModelState.Clear();
					ModelState.AddModelError("", "Start Date is Above End Date");
				}
			else
				{

					if (OutsourceMaintenance != null)
						{
                            OutsourceMaintenance.ModifiedBy = User.Identity.Name;
                            OutsourceMaintenance.LastModified = DateTime.Now;
				
			
							_OutsourceMaintenanceService.Update(OutsourceMaintenance);
						}

				}
			return Json(new[] {OutsourceMaintenance}.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult OutsourceMaintenance_Destroy([DataSourceRequest] DataSourceRequest request, OutsourceMaintenance outsourceMaintenance)
		{
			var OutsourceMaintenance = _OutsourceMaintenanceService.GetById(outsourceMaintenance.OutsourceMaintenanceID);
			if (OutsourceMaintenance != null)
			{
				_OutsourceMaintenanceService.Delete(OutsourceMaintenance);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Outsource Maintenance Id " + outsourceMaintenance.OutsourceMaintenanceID);

			}

			return Json(new[] { OutsourceMaintenance }.ToDataSourceResult(request));
		}

	

	}

	}

