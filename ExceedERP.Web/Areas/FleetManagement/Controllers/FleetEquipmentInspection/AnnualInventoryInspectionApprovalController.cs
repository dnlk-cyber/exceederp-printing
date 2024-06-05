using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection;
using System.Data.Entity;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{


    public class AnnualInventoryInspectionApprovalController : Controller
	{
		//
		// GET: /FleetManagement/AnnualInventoryInspectionApproval/
  
	
		ExceedDbContext db = new  ExceedDbContext();
		IAnnualEquipmentInventoryInspectionService _AnnualEquipmentInventoryInspectionsService;

		public AnnualInventoryInspectionApprovalController()
		{
			_AnnualEquipmentInventoryInspectionsService = new AnnualEquipmentInventoryInspectionService();
		}
		public ActionResult Index()
		{
		   
			return View();
		}
		public ActionResult AnnualInventoryInspectionApproval_Create([DataSourceRequest] DataSourceRequest request, AnnualInventoryInspectionApproval AnnualInventoryInspectionApproval)
		{
			ModelState.Remove("AnnualInventoryInspectionApprovalID");
			AnnualEquipmentInventoryInspection AnnualEquipmentInventoryInspection = _AnnualEquipmentInventoryInspectionsService.GetAll().Where(x => x.AnnualEquipmentInventoryInspectionID == AnnualInventoryInspectionApproval.AnnualEquipmentInventoryInspectionId).FirstOrDefault();
			if (AnnualInventoryInspectionApproval != null)
			{
				AnnualInventoryInspectionApproval.DateCreated = DateTime.Now;
				AnnualInventoryInspectionApproval.CreatedBy = User.Identity.Name;

				db.AnnualInventoryInspectionApproval.Add(AnnualInventoryInspectionApproval);
				db.SaveChanges();
				if (AnnualInventoryInspectionApproval.Status == ApprovalStatuses.Reject)
				{

					AnnualEquipmentInventoryInspection.IsSendForApproval = false;

				}
				if (AnnualInventoryInspectionApproval.Status == ApprovalStatuses.Approve)
				{
					AnnualEquipmentInventoryInspection.IsOnlineApproved = true;
					AnnualEquipmentInventoryInspection.OnlineApprovedBy = User.Identity.Name;
					AnnualEquipmentInventoryInspection.OnlineApprovedTime = DateTime.Now;
				}
				_AnnualEquipmentInventoryInspectionsService.Update(AnnualEquipmentInventoryInspection);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Annual inventory inspection approval", "Success", User.Identity.Name, $"created annual inventory inspection approval {AnnualInventoryInspectionApproval.AnnualInventoryInspectionApprovalID} ", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { AnnualInventoryInspectionApproval }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult AnnualInventoryInspectionApproval_Update([DataSourceRequest] DataSourceRequest request, AnnualInventoryInspectionApproval AnnualInventoryInspectionApproval)
		{
			AnnualEquipmentInventoryInspection AnnualEquipmentInventoryInspection = _AnnualEquipmentInventoryInspectionsService.GetAll().Where(x => x.AnnualEquipmentInventoryInspectionID == AnnualInventoryInspectionApproval.AnnualEquipmentInventoryInspectionId).FirstOrDefault();
		 
			if (AnnualInventoryInspectionApproval != null)
			{
				AnnualInventoryInspectionApproval.LastModified = DateTime.Now;
				AnnualInventoryInspectionApproval.ModifiedBy = User.Identity.Name;
				 db.AnnualInventoryInspectionApproval.Attach(AnnualInventoryInspectionApproval);
				db.Entry(AnnualInventoryInspectionApproval).State = EntityState.Modified;
				db.SaveChanges();


				AnnualEquipmentInventoryInspection.OnlineApprovedBy = User.Identity.Name;
				AnnualEquipmentInventoryInspection.OnlineApprovedTime = DateTime.Now;
				_AnnualEquipmentInventoryInspectionsService.Update(AnnualEquipmentInventoryInspection);
               
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Annual inventory inspection approval", "Success", User.Identity.Name, $"updated annual inventery inspection approval {AnnualInventoryInspectionApproval.AnnualInventoryInspectionApprovalID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { AnnualInventoryInspectionApproval }.ToDataSourceResult(request, ModelState));
		}

	   [AuthorizeRoles(FleetManagementRoles.InspectionApprover, FleetManagementRoles.InspectionUser)] 
	
		public ActionResult Approval_Read([DataSourceRequest]DataSourceRequest request, string EId)
		{

			Guid Id = Guid.Parse(EId);

			List<InspectionApproval> fleetApproval = db.InspectionApprovals.Where(x => x.InspectionId == EId).ToList();
			List<AnnualEquipmentInventoryInspection> Category = db.AnnualEquipmentInventoryInspection.Where(x => x.AnnualEquipmentInventoryInspectionID == Id).ToList();
			bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
			if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
			{
				fleetApproval.Add(new InspectionApproval()
				{
					InspectionId = EId,
					Remark = string.Empty,
					Status = MaintenanceConst.Pending,
					DateCreated = DateTime.Now,
					CreatedBy = User.Identity.Name


				});
			}


			return Json(fleetApproval.ToDataSourceResult(request, approval => new InspectionApproval()
			{
				InspectionApprovalId = approval.InspectionApprovalId,
				InspectionId = approval.InspectionId,
				Remark = approval.Remark,
				Status = approval.Status,
				DateCreated = DateTime.Now,
				CreatedBy = User.Identity.Name


			}));
		}


		  [AuthorizeRoles( FleetManagementRoles.InspectionUser)] 
	
		public ActionResult SendForOnlineApproval(string EId)
		{
			//List<InspectionLog> log = db.InspectionLogs.Where(x => x.InspectionId == EId).ToList();
			//if (log.Any())
			//{
				Guid Id = Guid.Parse(EId);
				var fleet = db.AnnualEquipmentInventoryInspection.Where(x => x.AnnualEquipmentInventoryInspectionID == Id).FirstOrDefault();
				if (fleet != null)
				{
					fleet.IsSendForApproval = true;
					fleet.Status = MaintenanceConst.WaitingApproval;
					db.AnnualEquipmentInventoryInspection.Attach(fleet);
					db.Entry(fleet).State = EntityState.Modified;
					db.SaveChanges();
				}
			//}
			return RedirectToAction("Index", "AnnualEquipmentInventoryInspection");
		}


	[AuthorizeRoles(FleetManagementRoles.InspectionApprover)] 
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Approve([DataSourceRequest]DataSourceRequest request, InspectionApproval InspectionApproval, string EId)
		{
			try
			{

				Guid Id =Guid.Parse(EId);
				 if (InspectionApproval.Status == MaintenanceConst.Approved)
				{
					var entity = db.AnnualEquipmentInventoryInspection.Where(x => x.AnnualEquipmentInventoryInspectionID == Id).FirstOrDefault();

					entity.IsOnlineApproved = true;
					entity.OnlineApprovedBy = User.Identity.Name;
					entity.OnlineApprovedTime = DateTime.Now;
					entity.Status = MaintenanceConst.Approved;
					db.AnnualEquipmentInventoryInspection.Attach(entity);
					db.Entry(entity).State = EntityState.Modified;
					db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Approve", "Success", User.Identity.Name, $"created issue {entity.AnnualEquipmentInventoryInspectionID} ", Modules.FleetManagement);
                    #endregion
                }

                var approval = new InspectionApproval()
				{
					
					Status = InspectionApproval.Status,
					InspectionId = EId,
					Remark = InspectionApproval.Remark,
					DateCreated = DateTime.Now,
					CreatedBy = User.Identity.Name

				};

				db.InspectionApprovals.Add(approval);
				db.SaveChanges();
				InspectionApproval.InspectionApprovalId = approval.InspectionApprovalId;

			}
			catch (Exception e)
			{

				string ee = e.Message;
			}
			return Json(new[] { InspectionApproval }.ToDataSourceResult(request));
		}

	  
	
	}
}