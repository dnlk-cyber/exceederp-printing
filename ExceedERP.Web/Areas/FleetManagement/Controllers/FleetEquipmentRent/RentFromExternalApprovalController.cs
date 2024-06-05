using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FleetManagement;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;


namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class RentFromExternalApprovalController : Controller
    {
        //
        // GET: /FleetManagement/RentFromExternalApproval/

        ExceedDbContext db = new ExceedDbContext();

        IRentFromExternalService _RentFromExternalervice;

        public RentFromExternalApprovalController()
        {
            _RentFromExternalervice = new RentFromExternalService();

        }

        [AuthorizeRoles(FleetManagementRoles.RentRequestor, FleetManagementRoles.RentApprover)]
        public ActionResult RentFromExternalApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<RentFromExternalApproval> fleetApproval = db.RentFromExternalApproval.Where(x => x.RentFromExternalId == Id).ToList();
            List<RentFromExternal> Category = db.RentFromExternal.Where(x => x.RentFromExternalID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new RentFromExternalApproval()
                {
                    RentFromExternalId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new RentFromExternalApproval()
            {
                RentFromExternalApprovalID = approval.RentFromExternalApprovalID,

                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.RentRequestor)]

        public ActionResult Rent_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var RentFromExternal = db.RentFromExternal.Where(x => x.RentFromExternalID == Id).FirstOrDefault();
            if (RentFromExternal != null)
            {
                RentFromExternal.IsSendForApproval = true;
                RentFromExternal.Status = MaintenanceConst.WaitingApproval;
                db.RentFromExternal.Attach(RentFromExternal);
                db.Entry(RentFromExternal).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "RentFromExternal", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Rent_Approve([DataSourceRequest]DataSourceRequest request, RentFromExternalApproval RentFromExternalApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (RentFromExternalApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.RentFromExternal.Where(x => x.RentFromExternalID == Id).FirstOrDefault();
                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    db.RentFromExternal.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    UpdateFleet(entity.RentFromExternalID);

                }
                if (RentFromExternalApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.RentFromExternal.Where(x => x.RentFromExternalID == Id).FirstOrDefault();
                    entity.IsSendForApproval = false;

                    db.RentFromExternal.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new RentFromExternalApproval()
                {

                    Status = RentFromExternalApproval.Status,
                    RentFromExternalId = Id,
                    Remark = RentFromExternalApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.RentFromExternalApproval.Add(approval);
                db.SaveChanges();
                RentFromExternalApproval.RentFromExternalApprovalID = approval.RentFromExternalApprovalID;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { RentFromExternalApproval }.ToDataSourceResult(request));
        }

        private void UpdateFleet(Guid id)
        {
            var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == id);
            var rentEquipment = db.RentFromExternalEquipments.Where(x => x.RentFromExternalId == rentFromExternal.RentFromExternalID).ToList();
            foreach (var item in rentEquipment)
            {
                var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetId);
                fleet.RentFromExternalNum = rentFromExternal.Num;
                fleet.RentId = rentFromExternal.RentFromExternalID;
                db.Entry(fleet).State = EntityState.Modified;
            }
            db.SaveChanges();
        }


    }
}