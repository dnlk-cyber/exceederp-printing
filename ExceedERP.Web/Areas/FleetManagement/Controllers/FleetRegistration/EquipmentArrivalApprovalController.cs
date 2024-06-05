 
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]
    public class EquipmentArrivalApprovalController : Controller
    {
        //
        // GET: /FleetManagement/EquipmentArrivalApproval/
        ExceedDbContext db = new ExceedDbContext();
 

        public EquipmentArrivalApprovalController()
        {
     
        }

        //    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
        public ActionResult EquipmentArrivalApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<EquipmentArrivalApproval> fleetApproval = db.EquipmentArrivalApproval.Where(x => x.EquipmentArrivalId == Id).ToList();
            List<EquipmentArrival> Category = db.EquipmentArrival.Where(x => x.EquipmentArrivalId == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new EquipmentArrivalApproval()
                {
                    EquipmentArrivalId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name


                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new EquipmentArrivalApproval()
            {
                EquipmentArrivalApprovalId = approval.EquipmentArrivalApprovalId,
                EquipmentArrivalId = approval.EquipmentArrivalId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        public ActionResult EquipmentArrival_SendForOnlineApproval(Guid Id)
        {

             var EquipmentArrival = db.EquipmentArrival.Where(x => x.EquipmentArrivalId == Id).FirstOrDefault();
            if (EquipmentArrival != null)
            {
                EquipmentArrival.IsSendForApproval = true;
                EquipmentArrival.Status = MaintenanceConst.WaitingApproval;
                db.EquipmentArrival.Attach(EquipmentArrival);
                db.Entry(EquipmentArrival).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "EquipmentArrival", new { area = "FleetManagement" });
        }

        //        [AuthorizeRoles(FleetManagementRoles.EquipmentArrivalApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentArrival_Approve([DataSourceRequest]DataSourceRequest request, EquipmentArrivalApproval EquipmentArrivalApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (EquipmentArrivalApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.EquipmentArrival.Where(x => x.EquipmentArrivalId == Id).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.IsOnlineApproved = true;
                        entity.OnlineApprovedBy = User.Identity.Name;
                        entity.OnlineApprovedTime = DateTime.Now;
                        entity.Status = MaintenanceConst.Approved;

                        db.EquipmentArrival.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();

                        var Assignment = db.AssignedEquipment.Where(x => x.AssignedEquipmentID == entity.AssignmentId).FirstOrDefault();
                        if (Assignment != null)
                        {
                            Assignment.IsOnlineTransferred = true;
                            Assignment.IsOnlineTransferredBy = User.Identity.Name;
                            Assignment.OnlineTransferredTime = DateTime.Now;
                            Assignment.IsArrived = true;

                            db.AssignedEquipment.Attach(Assignment);
                            db.Entry(Assignment).State = EntityState.Modified;
                            db.SaveChanges();
                        }






                    }
                    if (EquipmentArrivalApproval.Status == MaintenanceConst.Rejected)
                    {
                        var entityArr = db.EquipmentArrival.Where(x => x.EquipmentArrivalId == Id).FirstOrDefault();

                        entityArr.IsSendForApproval = false;

                        db.EquipmentArrival.Attach(entityArr);
                        db.Entry(entityArr).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var approval = new EquipmentArrivalApproval()
                    {
                        EquipmentArrivalApprovalId = Guid.NewGuid(),
                        Status = EquipmentArrivalApproval.Status,
                        EquipmentArrivalId = Id,
                        Remark = EquipmentArrivalApproval.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.EquipmentArrivalApproval.Add(approval);
                    db.SaveChanges();
                    EquipmentArrivalApproval.EquipmentArrivalApprovalId = approval.EquipmentArrivalApprovalId;

                }
            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { EquipmentArrivalApproval }.ToDataSourceResult(request));
        }




    }
}