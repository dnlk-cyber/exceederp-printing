using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]
    public class VehicleDemandApprovalController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();


        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
        public ActionResult VehicleDemandApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            IQueryable<VehicleDemandApproval> fleetApproval = db.VehicleDemandApproval.Where(x => x.VehicleDemandId == Id);

            var dsResult = fleetApproval.ToDataSourceResult(request, model => new VehicleDemandApproval
            {
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                OrgBranchName = model.OrgBranchName,
                Remark = model.Remark,
                Status = model.Status,
                VehicleDemand = model.VehicleDemand,
                VehicleDemandApprovalID = model.VehicleDemandApprovalID,
                VehicleDemandId = model.VehicleDemandId
            });
            return Json(dsResult);

        }


        [AuthorizeRoles(FleetManagementRoles.RentRequestor)]

        public ActionResult VehicleDemand_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            
            var vehicleDemand = db.VehicleDemand.Where(x => x.VehicleDemandId == Id).FirstOrDefault();
            
            if (vehicleDemand != null)
            {
                vehicleDemand.IsSendForApproval = true;
                vehicleDemand.Status = MaintenanceConst.WaitingApproval;
                db.VehicleDemand.Attach(vehicleDemand);
                db.Entry(vehicleDemand).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "EquipmentDemand", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemand_Approve([DataSourceRequest]DataSourceRequest request, VehicleDemandApproval VehicleDemandApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (VehicleDemandApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.VehicleDemand.Where(x => x.VehicleDemandId == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.IsDepartmentOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.Status = MaintenanceConst.Approved;
                    entity.OnlineApprovedTime = DateTime.Now;

                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (VehicleDemandApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.VehicleDemand.Where(x => x.VehicleDemandId == Id).FirstOrDefault();
                    entity.IsSendForApproval = false;
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
                var approval = new VehicleDemandApproval()
                {

                    Status = VehicleDemandApproval.Status,
                    VehicleDemandId = Id,
                    Remark = VehicleDemandApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.VehicleDemandApproval.Add(approval);
                db.SaveChanges();
                VehicleDemandApproval.VehicleDemandApprovalID = approval.VehicleDemandApprovalID;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { VehicleDemandApproval }.ToDataSourceResult(request));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}