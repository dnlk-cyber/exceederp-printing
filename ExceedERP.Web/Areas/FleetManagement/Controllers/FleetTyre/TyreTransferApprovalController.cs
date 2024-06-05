using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
	public class TyreTransferApprovalController : Controller
	{
	  
		   //
		// GET: /FleetManagement/TyreTransferApproval/
	   ExceedDbContext db = new  ExceedDbContext();
	   IFleetService _FleetService;
	 public TyreTransferApprovalController()
		{
			_FleetService = new FleetService();
		}
				 
		public ActionResult Index()
		{
		   
			return View();
		}

		  public ActionResult TyreTransferApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
		{

			Guid Id = Guid.Parse(EId);

			List<TyreTransferApproval> fleetApproval = db.TyreTransferApprovals.Where(x => x.TyreId == Id).ToList();
			List<TyreTransfer> Category = db.TyreTransfers.Where(x => x.TyreTransferId == Id).ToList();
			bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
			if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
			{
				fleetApproval.Add(new TyreTransferApproval()
				{
					TyreId = Id,
					Remark = string.Empty,
					Status = MaintenanceConst.Pending,
					DateCreated = DateTime.Now,
					CreatedBy = User.Identity.Name
				});
			}


			return Json(fleetApproval.ToDataSourceResult(request, approval => new TyreTransferApproval()
			{
				TyreTransferApprovalId = approval.TyreTransferApprovalId,
				TyreId = approval.TyreId,
				Remark = approval.Remark,
				Status = approval.Status,
				DateCreated = DateTime.Now,
				CreatedBy = User.Identity.Name


			}));
		}



	
		public ActionResult TyreTransfer_SendForOnlineApproval(string EId)
		{

			Guid Id = Guid.Parse(EId);
			var TyreTransfer = db.TyreTransfers.Where(x => x.TyreTransferId == Id).FirstOrDefault();
			if (TyreTransfer != null)
			{
				TyreTransfer.IsSendForApproval = true;
				TyreTransfer.Status = MaintenanceConst.WaitingApproval;

				db.TyreTransfers.Attach(TyreTransfer);
				db.Entry(TyreTransfer).State = EntityState.Modified;
				db.SaveChanges();
			}

		
			return RedirectToActionPermanent("Index", "TyreTransfer", new { area = "FleetManagement" });
		}
		 


		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult TyreTransfer_Approve([DataSourceRequest]DataSourceRequest request, TyreTransferApproval TyreTransferApproval, string EId)
		{
			try
			{

			   


					Guid Id = Guid.Parse(EId);
					if (TyreTransferApproval.Status == MaintenanceConst.Approved)
					{
						var entity = db.TyreTransfers.Where(x => x.TyreTransferId == Id).FirstOrDefault();

						entity.IsOnlineApproved = true;
						entity.OnlineApprovedBy = User.Identity.Name;
						entity.OnlineApprovedTime = DateTime.Now;
						entity.Status = MaintenanceConst.Approved;
						db.TyreTransfers.Attach(entity);
						db.Entry(entity).State = EntityState.Modified;
						db.SaveChanges();


						var FromFleet = db.Fleets.Where(x => x.UnitNum == entity.FromEquipment).FirstOrDefault();
						var ToFleet = db.Fleets.Where(x => x.UnitNum == entity.ToEquipment).FirstOrDefault();
						var EquipmentTyre = db.EquipmentTyres.Where(x => x.SerialNo == entity.TyreSerialNo).FirstOrDefault();

						EquipmentTyre.fleetId = ToFleet.FleetsID;
						EquipmentTyre.LastModified = DateTime.Now;

						db.EquipmentTyres.Attach(EquipmentTyre);
						db.Entry(EquipmentTyre).State = EntityState.Modified;
						db.SaveChanges();


					}
					if (TyreTransferApproval.Status == MaintenanceConst.Rejected)
					{
							 var entity = db.TyreTransfers.Where(x => x.TyreTransferId == Id).FirstOrDefault();
					 entity.IsSendForApproval = false;

					 db.TyreTransfers.Attach(entity);
						db.Entry(entity).State = EntityState.Modified;
						db.SaveChanges();

					}
					var approval = new TyreTransferApproval()
					{

						Status = TyreTransferApproval.Status,
						TyreId = Id,
						Remark = TyreTransferApproval.Remark,
						DateCreated = DateTime.Now,
						CreatedBy = User.Identity.Name

					};

					db.TyreTransferApprovals.Add(approval);
					db.SaveChanges();
					TyreTransferApproval.TyreTransferApprovalId = approval.TyreTransferApprovalId;
				}
			
			catch (Exception e)
			{

				string ee = e.Message;
			}
			return Json(new[] { TyreTransferApproval }.ToDataSourceResult(request,ModelState));
		}


	
	
	}
}