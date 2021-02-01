
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Repository;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]
    public class VehicleReturnApprovalController : Controller
    {
        //
        ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
        public ActionResult VehicleReturnApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<VehicleReturnApproval> fleetApproval = db.VehicleReturnApproval.Where(x => x.VehicleReturnId == Id).ToList();
            List<VehicleReturn> Category = db.VehicleReturn.Where(x => x.VehicleReturnID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new VehicleReturnApproval()
                {
                    VehicleReturnId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,

                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new VehicleReturnApproval()
            {
                VehicleReturnApprovalID = approval.VehicleReturnApprovalID,
                  VehicleReturnId = Id,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.RentRequestor)]

        public ActionResult VehicleReturn_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var fleet = db.VehicleReturn.Where(x => x.VehicleReturnID == Id).FirstOrDefault();
            if (fleet != null)
            {
                fleet.IsSendForApproval = true;
                fleet.Status = MaintenanceConst.Approved;
                db.VehicleReturn.Attach(fleet);
                db.Entry(fleet).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "EquipmentReturn", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReturn_Approve([DataSourceRequest]DataSourceRequest request, VehicleReturnApproval VehicleReturnApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (VehicleReturnApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.VehicleReturn.Where(x => x.VehicleReturnID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;

                    db.VehicleReturn.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (VehicleReturnApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.VehicleReturn.Where(x => x.VehicleReturnID == Id).FirstOrDefault();
                    entity.IsSendForApproval = false;

                    db.VehicleReturn.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
                var approval = new VehicleReturnApproval()
                {

                    Status = VehicleReturnApproval.Status,
                    VehicleReturnId = Id,
                    Remark = VehicleReturnApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.VehicleReturnApproval.Add(approval);
                db.SaveChanges();
                VehicleReturnApproval.VehicleReturnApprovalID = approval.VehicleReturnApprovalID;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { VehicleReturnApproval }.ToDataSourceResult(request));
        }




    }
}