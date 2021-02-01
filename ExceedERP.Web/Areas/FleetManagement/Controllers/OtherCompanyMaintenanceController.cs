using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using System.IO; 

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
       [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor )] 
	public class OtherCompanyMaintenanceController : Controller
	{
	   IOtherCompanyMaintenanceService _OtherCompanyMaintenanceService;
		IFleetService _FleetService;
        IJobOrderService _JobOrderService;
		public OtherCompanyMaintenanceController()
		{
            _JobOrderService = new JobOrderService();
         
			_OtherCompanyMaintenanceService = new OtherCompanyMaintenanceService();
			_FleetService = new FleetService();
		}
	 
		//
		// GET: /FleetManagement/OtherCompanyMaintenance/
		public ActionResult Index()
		{
			return View();
		}

	   
		public ActionResult OtherCompanyMaintenance_Read([DataSourceRequest] DataSourceRequest request)
		{
            var OtherCompanyMaintenance = _OtherCompanyMaintenanceService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

			return Json(OtherCompanyMaintenance.ToDataSourceResult(request));
		}
	  

		  [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult OtherCompanyMaintenance_Create([DataSourceRequest] DataSourceRequest request, OtherCompanyMaintenance OtherCompanyMaintenance)
		{
            ModelState.Remove("OtherCompanyMaintenanceID");
             
			OtherCompanyMaintenance.OtherCompanyMaintenanceID = Guid.NewGuid();
			if (OtherCompanyMaintenance != null )
			{
                OtherCompanyMaintenance.CreatedBy = User.Identity.Name;
                OtherCompanyMaintenance.DateCreated = DateTime.Now;
                OtherCompanyMaintenance.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
				
				_OtherCompanyMaintenanceService.Insert(OtherCompanyMaintenance);
            var  Fleet = _FleetService.GetAll().Where(x=>x.IsExternal).FirstOrDefault();
                int ordernum = _JobOrderService.GetAll().ToList().Count;
                if (Fleet != null)
                {
                    JobOrder jb = new JobOrder();
                    jb.InspectedBy = OtherCompanyMaintenance.NameofCustomer;
                    jb.JobOrderID = Guid.NewGuid();
                    jb.EquipmentName = OtherCompanyMaintenance.EquipmentName;
                    jb.AssetNum = OtherCompanyMaintenance.EquipmentName;
                    jb.FleetsID = Fleet.FleetsID;

                    jb.JobOrderNum = ordernum;
                    jb.ProjectName = " External : Operator " + OtherCompanyMaintenance.OperatorName;
                    jb.InspectorComments = " BankSlip No " + OtherCompanyMaintenance.BankSlipNo;
                    jb.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
               
                    jb.Status = MaintenanceConst.Pending;
                    _JobOrderService.Insert(jb);
                }
			}

			return Json(new[] { OtherCompanyMaintenance }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult OtherCompanyMaintenance_Update([DataSourceRequest] DataSourceRequest request, OtherCompanyMaintenance OtherCompanyMaintenance)
		{
			if (OtherCompanyMaintenance != null )
			{
                OtherCompanyMaintenance.ModifiedBy = User.Identity.Name;
                OtherCompanyMaintenance.LastModified = DateTime.Now;
                OtherCompanyMaintenance.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            
				_OtherCompanyMaintenanceService.Update(OtherCompanyMaintenance);
			}

			return Json(new[] {OtherCompanyMaintenance}.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult OtherCompanyMaintenance_Destroy([DataSourceRequest] DataSourceRequest request, OtherCompanyMaintenance otherCompanyMaintenance)
		{
			var OtherCompanyMaintenance = _OtherCompanyMaintenanceService.GetById(otherCompanyMaintenance.OtherCompanyMaintenanceID);
			if (OtherCompanyMaintenance != null)
			{
				_OtherCompanyMaintenanceService.Delete(OtherCompanyMaintenance);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Other Company Maintenance Id " + otherCompanyMaintenance.OtherCompanyMaintenanceID);

			}

			return Json(new[] { OtherCompanyMaintenance }.ToDataSourceResult(request));
		}
        public ActionResult UploadPhoto(BindingModel model, IEnumerable<HttpPostedFileBase> AttachmentFile)
        {



            try
            {
                foreach (var fileBase in AttachmentFile)
                {

                    string path = Server.MapPath("~/Content/Images/Fleet/Equipment/");
                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    fileBase.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Fleet/OutSourceMaintenace/Recipt/" + model.Id + ".jpg")));


                }
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("Index");
        }

	

	}
}

