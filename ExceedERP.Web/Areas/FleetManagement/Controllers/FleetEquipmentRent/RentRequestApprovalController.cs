using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;

using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class RentRequestApprovalController : Controller
    {
        //
        ExceedDbContext db = new ExceedDbContext();


        [AuthorizeRoles(FleetManagementRoles.RentUser, FleetManagementRoles.RentApprover , FleetManagementRoles.RentRequestor)]
        public ActionResult RentRequestApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<RentRequestApproval> fleetApproval = db.RentRequestApproval.Where(x => x.RentRequestId == Id).ToList();
            List<RentRequest> Category = db.RentRequest.Where(x => x.RentRequestID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new RentRequestApproval()
                {
                    RentRequestId = Id,
                    Remark = string.Empty,
                    Status = ApprovalStatuses.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new RentRequestApproval()
            {
                RentRequestApprovalID = approval.RentRequestApprovalID,

                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.RentRequestor)]

        public ActionResult RentRequest_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var RentRequest = db.RentRequest.Where(x => x.RentRequestID == Id).FirstOrDefault();
            if (RentRequest != null)
            {
                RentRequest.IsSendForApproval = true;
                RentRequest.Status = MaintenanceConst.WaitingApproval;
                db.RentRequest.Attach(RentRequest);
                db.Entry(RentRequest).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "RentRequest", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentRequest_Approve([DataSourceRequest]DataSourceRequest request, RentRequestApproval RentRequestApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (RentRequestApproval.Status == ApprovalStatuses.Approve)
                {
                    var entity = db.RentRequest.Where(x => x.RentRequestID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    db.RentRequest.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var approval = new RentRequestApproval()
                {

                    Status = RentRequestApproval.Status,
                    RentRequestId = Id,
                    Remark = RentRequestApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.RentRequestApproval.Add(approval);
                db.SaveChanges();
                RentRequestApproval.RentRequestApprovalID = approval.RentRequestApprovalID;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { RentRequestApproval }.ToDataSourceResult(request));
        }




    }
}