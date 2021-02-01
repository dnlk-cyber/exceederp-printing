using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
 
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
     [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor, FleetManagementRoles.MaintenanceRequestApprover)]
      
    public class OtherCompanyMaintenaceApprovalController : Controller
    {
        ExceedDbContext db =new ExceedDbContext();
        //
        // GET: /FleetManagement/OtherCompanyMaintenaceApproval/

         public ActionResult OtherCompanyMaintenaceApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<OtherCompanyMaintenaceApproval> fleetApproval = db.OtherCompanyMaintenaceApproval.Where(x => x.OtherCompanyMaintenaceId == Id).ToList();
            List<OtherCompanyMaintenance> Category = db.OtherCompanyMaintenance.Where(x => x.OtherCompanyMaintenanceID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new OtherCompanyMaintenaceApproval()
                {
                    OtherCompanyMaintenaceId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new OtherCompanyMaintenaceApproval()
            {
                OtherCompanyMaintenaceApprovalID = approval.OtherCompanyMaintenaceApprovalID,
                OtherCompanyMaintenaceId = approval.OtherCompanyMaintenaceId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestor)]

         public ActionResult OtherCompanyMaintenance_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var OtherCompanyMaintenance = db.OtherCompanyMaintenance.Where(x => x.OtherCompanyMaintenanceID == Id).FirstOrDefault();
            if (OtherCompanyMaintenance != null)
            {
                OtherCompanyMaintenance.IsSendForApproval = true;
                OtherCompanyMaintenance.Status = MaintenanceConst.WaitingApproval;
                db.OtherCompanyMaintenance.Attach(OtherCompanyMaintenance);
                db.Entry(OtherCompanyMaintenance).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "OtherCompanyMaintenance", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OtherCompanyMaintenance_Approve([DataSourceRequest]DataSourceRequest request, OtherCompanyMaintenaceApproval OtherCompanyMaintenaceApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (OtherCompanyMaintenaceApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.OtherCompanyMaintenance.Where(x => x.OtherCompanyMaintenanceID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    db.OtherCompanyMaintenance.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (OtherCompanyMaintenaceApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.OtherCompanyMaintenance.Where(x => x.OtherCompanyMaintenanceID == Id).FirstOrDefault();

                    entity.IsSendForApproval = false;

                    db.OtherCompanyMaintenance.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new OtherCompanyMaintenaceApproval()
                {

                    Status = OtherCompanyMaintenaceApproval.Status,
                    OtherCompanyMaintenaceId = Id,
                    Remark = OtherCompanyMaintenaceApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.OtherCompanyMaintenaceApproval.Add(approval);
                db.SaveChanges();
                OtherCompanyMaintenaceApproval.OtherCompanyMaintenaceApprovalID = approval.OtherCompanyMaintenaceApprovalID;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { OtherCompanyMaintenaceApproval }.ToDataSourceResult(request));
        }




    }
}