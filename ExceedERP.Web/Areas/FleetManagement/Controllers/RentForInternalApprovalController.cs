using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Repository;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;


using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class RentForInternalApprovalController : Controller
    {
        //
        IRentInternalProjectService _RentInternalProjectService;
        ExceedDbContext db = new ExceedDbContext();

        public RentForInternalApprovalController()
        {
            _RentInternalProjectService = new RentInternalProjectService();
        }


        [AuthorizeRoles(FleetManagementRoles.RentRequestor, FleetManagementRoles.RentApprover)]
        public ActionResult RentForInternalApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<RentForInternalApproval> fleetApproval = db.RentForInternalApproval.Where(x => x.RentInternalProjectId == Id).ToList();
            List<RentInternalProject> Category = db.RentInternalProject.Where(x => x.RentInternalProjectID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
           
           
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false  )
            {
                fleetApproval.Add(new RentForInternalApproval()
                {
                    RentInternalProjectId = Id,
                    Remark = string.Empty,
                    AssignedQuantity = 0,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new RentForInternalApproval()
            {
                RentForInternalApprovalID = approval.RentForInternalApprovalID,
                RentInternalProjectId = Id,
                Remark = approval.Remark,
                AssignedQuantity = approval.AssignedQuantity,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.RentRequestor)]

        public ActionResult Rent_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var RentInternalProject = db.RentInternalProject.Where(x => x.RentInternalProjectID == Id).FirstOrDefault();
            if (RentInternalProject != null)
            {
                RentInternalProject.IsSendForApproval = true;
                RentInternalProject.Status = MaintenanceConst.WaitingApproval;

                db.RentInternalProject.Attach(RentInternalProject);
                db.Entry(RentInternalProject).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "RentInternalProject", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Rent_Approve([DataSourceRequest]DataSourceRequest request, RentForInternalApproval RentForInternalApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (RentForInternalApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.RentInternalProject.Where(x => x.RentInternalProjectID == Id).FirstOrDefault();
                    entity.AssignedQuantity = RentForInternalApproval.AssignedQuantity;
                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;

                    db.RentInternalProject.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (RentForInternalApproval.Status == MaintenanceConst.Rejected)
                {

                    var entity = db.RentInternalProject.Where(x => x.RentInternalProjectID == Id).FirstOrDefault();
                  
                    entity.IsSendForApproval = false;

                    db.RentInternalProject.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new RentForInternalApproval()
                {

                    Status = RentForInternalApproval.Status,
                    RentInternalProjectId = Id,
                    Remark = RentForInternalApproval.Remark,
                    AssignedQuantity = RentForInternalApproval.AssignedQuantity,
                    DateCreated = DateTime.Now,
                  
                    CreatedBy = User.Identity.Name

                };

                db.RentForInternalApproval.Add(approval);
                db.SaveChanges();
                RentForInternalApproval.RentForInternalApprovalID = approval.RentForInternalApprovalID;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { RentForInternalApproval }.ToDataSourceResult(request));
        }




    }
}