using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.InspectionUser, FleetManagementRoles.InspectionApprover)] 
	
	public class AnnualEquipmentInventoryInspectionController : Controller
	{
        ExceedDbContext db = new ExceedDbContext();
		IAnnualEquipmentInventoryInspectionService _AnnualEquipmentInventoryInspectionService;
		public AnnualEquipmentInventoryInspectionController()
		{
			_AnnualEquipmentInventoryInspectionService = new AnnualEquipmentInventoryInspectionService();
		}
		//
		// GET: /FleetManagement/AnnualEquipmentInventoryInspection/
		public ActionResult Index()
		{

        
       
			return View();
		}


        public ActionResult AnnualInspectionlog_Read([DataSourceRequest] DataSourceRequest request , string Id)
        {
            var AnnualEquipmentInventoryInspection = db.InspectionLogs.Where(x=>x.InspectionId == Id).Select(a => new   
            {
                InspectionLogId = a.InspectionLogId,
                CategoryName = a.CategoryName + " - " + a.LineName ,
                DateCreated = a.DateCreated,
                IsActive = a.IsActive,
                Status = a.IsActive ? "Available" : "UnAvailable",
                IsSendForApproval = a.IsSendForApproval,
                IsOnlineApproved = a.IsOnlineApproved,
                OrgBranchName = a.OrgBranchName

            }).ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

            return Json(AnnualEquipmentInventoryInspection.ToDataSourceResult(request));
        }


        public ActionResult AnnualInspectionlogLast_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
              var LastInspection = db.InspectionLogs.Where(x=>x.IsActive == true).OrderByDescending(x=>x.DateCreated).FirstOrDefault();

            var AnnualEquipmentInventoryInspection = db.InspectionLogs.Where(x=>x.IsActive == true && x.InspectionId == Id).OrderByDescending(x=>x.DateCreated).Select(a => new
            {
                InspectionLogId = a.InspectionLogId,
                CategoryName = a.CategoryName  + " - " + a.LineName ,
                DateCreated = a.DateCreated,
               

            }).ToList();


            return Json( AnnualEquipmentInventoryInspection , JsonRequestBehavior.AllowGet );
        }
        public ActionResult GenrericInspectionLog_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var LastInspection = db.InspectionLogs.Where(x => x.IsActive == true).OrderByDescending(x => x.DateCreated).FirstOrDefault();

            var AnnualEquipmentInventoryInspection = db.InspectionLogs.Where(x => x.IsActive == true && x.InspectionId == Id).OrderByDescending(x => x.DateCreated).Select(a => new InspectionLog
            {
                InspectionLogId = a.InspectionLogId,
                CategoryName = a.CategoryName.IfNullDefaultValue(),
                LineName = a.LineName.IfNullDefaultValue(),
                DateCreated = a.DateCreated,


            }).ToList();


            return Json(AnnualEquipmentInventoryInspection, JsonRequestBehavior.AllowGet);
        }

		public ActionResult AnnualEquipmentInventoryInspection_Read([DataSourceRequest] DataSourceRequest request)
		{
			var AnnualEquipmentInventoryInspection = _AnnualEquipmentInventoryInspectionService.GetAll().Select(a => new AnnualEquipmentInventoryInspection 
			{
			AnnualEquipmentInventoryInspectionID=a.AnnualEquipmentInventoryInspectionID,
			DateofInspection=a.DateofInspection,
			FindingsOutcome=a.FindingsOutcome,
			FleetsID=a.FleetsID,
            EquipmentTypeId = a.Fleets.EquipmentIdentityID.ToString(),
            Remark=a.Fleets.PlateNumber + " -- " +a.Fleets.EquipmentName,
			InspectedBy=a.InspectedBy,
            VoidBy = a.Fleets.PlateNumber,
            IsOnlineApproved =a.IsOnlineApproved,
             IsSendForApproval = a.IsSendForApproval,
                Status = a.Status

			
			});

			return Json(AnnualEquipmentInventoryInspection.ToDataSourceResult(request));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AnnualEquipmentInventoryInspection_Create([DataSourceRequest] DataSourceRequest request, AnnualEquipmentInventoryInspection AnnualEquipmentInventoryInspection)
		{
		   if (AnnualEquipmentInventoryInspection != null)
			{
				AnnualEquipmentInventoryInspection.AnnualEquipmentInventoryInspectionID = Guid.NewGuid();
                AnnualEquipmentInventoryInspection.Status = MaintenanceConst.Pending;
				AnnualEquipmentInventoryInspection.CreatedBy = User.Identity.Name;
				AnnualEquipmentInventoryInspection.DateCreated = DateTime.Now;
                AnnualEquipmentInventoryInspection.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
              
		   
				_AnnualEquipmentInventoryInspectionService.Insert(AnnualEquipmentInventoryInspection);
			}

			return Json(new[] { AnnualEquipmentInventoryInspection }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AnnualEquipmentInventoryInspection_Update([DataSourceRequest] DataSourceRequest request, AnnualEquipmentInventoryInspection AnnualEquipmentInventoryInspection)
		{
			if (AnnualEquipmentInventoryInspection != null)
			{
				AnnualEquipmentInventoryInspection.LastModified = DateTime.Now;
				AnnualEquipmentInventoryInspection.ModifiedBy = User.Identity.Name;
                AnnualEquipmentInventoryInspection.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
             
				_AnnualEquipmentInventoryInspectionService.Update(AnnualEquipmentInventoryInspection);
			}

			return Json(new[] { AnnualEquipmentInventoryInspection }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AnnualEquipmentInventoryInspection_Destroy([DataSourceRequest] DataSourceRequest request, AnnualEquipmentInventoryInspection annualEquipmentInventoryInspection)
		{
			var AnnualEquipmentInventoryInspection = _AnnualEquipmentInventoryInspectionService.GetById(annualEquipmentInventoryInspection.AnnualEquipmentInventoryInspectionID);
			if (AnnualEquipmentInventoryInspection != null)
			{
               
				_AnnualEquipmentInventoryInspectionService.Delete(AnnualEquipmentInventoryInspection);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Annual Equipment Inventory Inspection number " + annualEquipmentInventoryInspection.AnnualEquipmentInventoryInspectionID);
		
			}

			return Json(new[] { AnnualEquipmentInventoryInspection }.ToDataSourceResult(request));
		}


        public ActionResult AnnualEquipmentInventoryInspectionlog_Read([DataSourceRequest] DataSourceRequest request)
        {
            var logs = db.CheckListLogs.ToList();
            return Json(logs.ToDataSourceResult(request));
        }

	}
}
