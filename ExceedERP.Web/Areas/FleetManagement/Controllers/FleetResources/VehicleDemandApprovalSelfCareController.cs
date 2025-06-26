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
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class VehicleDemandApprovalSelfCareController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();


        //[AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
        public ActionResult VehicleDemandApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<VehicleDemandApproval> fleetApproval = db.VehicleDemandApproval.Where(x => x.VehicleDemandId == Id).ToList();
            List<VehicleDemand> Category = db.VehicleDemand.Where(x => x.VehicleDemandId == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new VehicleDemandApproval()
                {
                    VehicleDemandId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,

                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new VehicleDemandApproval()
            {
                VehicleDemandApprovalID = approval.VehicleDemandApprovalID,

                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }


        
        public ActionResult VehicleDemand_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var fleet = db.VehicleDemand.Where(x => x.VehicleDemandId == Id).FirstOrDefault();
            if (fleet != null)
            {
                fleet.IsSendForApproval = true;
                fleet.Status = MaintenanceConst.WaitingApproval;
                db.VehicleDemand.Attach(fleet);
                db.Entry(fleet).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "EquipmentDemandSelfCare", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
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
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.Status = MaintenanceConst.Approved;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;

                    db.VehicleDemand.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (VehicleDemandApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.VehicleDemand.Where(x => x.VehicleDemandId == Id).FirstOrDefault();
                    entity.IsSendForApproval = false;

                    db.VehicleDemand.Attach(entity);
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




    }
}