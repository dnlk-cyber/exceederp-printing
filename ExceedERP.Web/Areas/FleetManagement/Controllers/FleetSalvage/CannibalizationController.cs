using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using Kendo.Mvc.Extensions;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using System.IO;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
	 //[AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor, FleetManagementRoles.MaintenanceRequestApprover)] 
		public class CannibalizationController : Controller
	{
	  ICannibalizationService _CannibalizationService;
		IFleetService _FleetService;
		public CannibalizationController()
		{
			_CannibalizationService = new CannibalizationService();
			_FleetService = new FleetService();
		}
	 
		//
		// GET: /FleetManagement/Cannibalization/
		public ActionResult Index()
		{
			ViewData["Fleets"] = _FleetService.GetAll();
			return View();
		}
		public ActionResult FollowUp()
		{
			ViewData["Fleets"] = _FleetService.GetAll();
			return View();
		}

		public ActionResult CannibalizationFollowUp_Read([DataSourceRequest] DataSourceRequest request)
		{
			var Cannibalization = _CannibalizationService.GetAll().Where(x => x.IsOnlineApproved == true && x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

			return Json(Cannibalization.ToDataSourceResult(request));
		}
	  
		public ActionResult Cannibalization_Read([DataSourceRequest] DataSourceRequest request)
		{
			var Cannibalization = _CannibalizationService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

			return Json(Cannibalization.ToDataSourceResult(request));
		}
	  

		  [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Cannibalization_Create([DataSourceRequest] DataSourceRequest request, Cannibalization Cannibalization)
		{
			ModelState.Remove("CannibalizationID");
			
			if (Cannibalization != null )
			{
				Cannibalization.CannibalizationID = Guid.NewGuid();
				Cannibalization.Status = MaintenanceConst.Pending;
				Cannibalization.DateCreated = DateTime.Now;
				Cannibalization.CreatedBy = User.Identity.Name;
				Cannibalization.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
			  
				_CannibalizationService.Insert(Cannibalization);
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Canibalization registration", "Success", User.Identity.Name, $"created canibalization {Cannibalization.CannibalizationID}", Modules.FleetManagement);
				#endregion
			}

			return Json(new[] { Cannibalization }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Cannibalization_Update([DataSourceRequest] DataSourceRequest request, Cannibalization Cannibalization)
		{
			if (Cannibalization != null )
			{
				Cannibalization.LastModified = DateTime.Now;
				Cannibalization.ModifiedBy = User.Identity.Name;
				Cannibalization.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
			  
				_CannibalizationService.Update(Cannibalization);
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "Canibalization registration", "Success", User.Identity.Name, $"updated canibalization {Cannibalization.CannibalizationID}", Modules.FleetManagement);
				#endregion
			}

			return Json(new[] {Cannibalization}.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Cannibalization_Destroy([DataSourceRequest] DataSourceRequest request, Cannibalization cannibalization)
		{
			var Cannibalization = _CannibalizationService.GetById(cannibalization.CannibalizationID);
			if (Cannibalization != null)
			{
				_CannibalizationService.Delete(Cannibalization);
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Cannibalization " + cannibalization.CannibalizationID);
		
			}

			return Json(new[] { Cannibalization }.ToDataSourceResult(request));
		}

		public ActionResult UploadPhoto(BindingModel model, IEnumerable<HttpPostedFileBase> AttachmentFile)
		{



			try
			{

				foreach (var fileBase in AttachmentFile)
				{

					string path = Server.MapPath("~/Content/Images/Fleet/Cannibalization/");
					bool folderExists = Directory.Exists(path);
					if (!folderExists)
						Directory.CreateDirectory(path);
					if (System.IO.File.Exists(path))
					{
						System.IO.File.Delete(path);
					}
					fileBase.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Fleet/Cannibalization/" + model.Id )));


				}
				var Cannibalization = _CannibalizationService.GetAll().Where(x => x.CannibalizationID.ToString() == model.Id).FirstOrDefault();

				Cannibalization.OnlineTransferredTime = DateTime.Now;
				Cannibalization.IsOnlineTransferredBy = User.Identity.Name;
				Cannibalization.IsOnlineTransferred = true;
				_CannibalizationService.Update(Cannibalization);
			}
			catch (Exception e)
			{

			}
			return RedirectToAction("Index");
		}
	 

	}
}

