using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.RentApprover, FleetManagementRoles.RentUser, FleetManagementRoles.RentRequestor)]
    public class RentOtherSuppliersApprovalController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();


        [AuthorizeRoles(FleetManagementRoles.RentUser, FleetManagementRoles.RentApprover, FleetManagementRoles.RentRequestor)]
        public ActionResult RentOtherSuppliersApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<RentToExternalApproval> fleetApproval = db.RentOtherSupplierApproval.Where(x => x.RentToExternalId == Id).ToList();
            List<RentToExternal> Category = db.RentToExternals.Where(x => x.RentToExternalID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new RentToExternalApproval()
                {
                    RentToExternalId = Id,
                    Remark = string.Empty,
                    Status = ApprovalStatuses.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new RentToExternalApproval()
            {
                RentToExternalApprovalID = approval.RentToExternalApprovalID,
                RentToExternalId = Id,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.RentRequestor , FleetManagementRoles.RentUser)]

        public ActionResult RentOtherSuppliers_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var RentOtherSuppliers = db.RentToExternals.Where(x => x.RentToExternalID == Id).FirstOrDefault();
            if (RentOtherSuppliers != null)
            {
                RentOtherSuppliers.IsSendForApproval = true;
                RentOtherSuppliers.Status = MaintenanceConst.WaitingApproval;
                db.RentToExternals.Attach(RentOtherSuppliers);
                db.Entry(RentOtherSuppliers).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "RentToExternal", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentOtherSuppliers_Approve([DataSourceRequest]DataSourceRequest request, RentToExternalApproval RentOtherSuppliersApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (RentOtherSuppliersApproval.Status == ApprovalStatuses.Approve)
                {
                    var entity = db.RentToExternals.Where(x => x.RentToExternalID == Id).FirstOrDefault();

                     entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    db.RentToExternals.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (RentOtherSuppliersApproval.Status == ApprovalStatuses.Reject)
                {
                    var entity = db.RentToExternals.Where(x => x.RentToExternalID == Id).FirstOrDefault();
                    entity.IsSendForApproval = false;

                    db.RentToExternals.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new RentToExternalApproval()
                {


                    RentToExternalId = Id,
                    Remark = RentOtherSuppliersApproval.Remark,
                    DateCreated = DateTime.Now,
                       Status = RentOtherSuppliersApproval.Status,
                    CreatedBy = User.Identity.Name

                };

                db.RentOtherSupplierApproval.Add(approval);
                db.SaveChanges();
                RentOtherSuppliersApproval.RentToExternalApprovalID = approval.RentToExternalApprovalID;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { RentOtherSuppliersApproval }.ToDataSourceResult(request));
        }




    }
}
