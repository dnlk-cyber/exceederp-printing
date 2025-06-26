using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;


namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class PlanAndEvaluationApprovalController : Controller
    {
        //
        // GET: /FleetManagement/PlanAndEvaluationApproval/

        ExceedDbContext db = new  ExceedDbContext();
        
        [AuthorizeRoles(FleetManagementRoles.Equipment_Plan_Approver, FleetManagementRoles.Equipment_Plan_User)]

        public ActionResult EquipmentPlanEvaluationApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<EquipmentPlanEvaluationApproval> fleetApproval = db.EquipmentPlanEvaluationApprovals.Where(x => x.EquipmentPlanandEvaluationsId == Id).ToList();
            List<EquipmentPlanandEvaluations> Category = db.EquipmentPlanandEvaluation.Where(x => x.EquipmentPlanandEvaluationsID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new EquipmentPlanEvaluationApproval()
                {
                    EquipmentPlanandEvaluationsId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new EquipmentPlanEvaluationApproval()
            {
                EquipmentPlanEvaluationApprovalId = approval.EquipmentPlanEvaluationApprovalId,
                EquipmentPlanandEvaluationsId = approval.EquipmentPlanandEvaluationsId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.Equipment_Plan_User)]

        public ActionResult EquipmentPlanandEvaluations_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var EquipmentPlanandEvaluations = db.EquipmentPlanandEvaluation.Where(x => x.EquipmentPlanandEvaluationsID == Id).FirstOrDefault();
            if (EquipmentPlanandEvaluations != null)
            {
                EquipmentPlanandEvaluations.IsSendForApproval = true;
                EquipmentPlanandEvaluations.Status =MaintenanceConst.WaitingApproval;

                db.EquipmentPlanandEvaluation.Attach(EquipmentPlanandEvaluations);
                db.Entry(EquipmentPlanandEvaluations).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "EquipmentPlanandEvaluations", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.Equipment_Plan_Approver)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentPlanandEvaluations_Approve([DataSourceRequest]DataSourceRequest request, EquipmentPlanEvaluationApproval EquipmentPlanEvaluationApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (EquipmentPlanEvaluationApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.EquipmentPlanandEvaluation.Where(x => x.EquipmentPlanandEvaluationsID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    db.EquipmentPlanandEvaluation.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (EquipmentPlanEvaluationApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.EquipmentPlanandEvaluation.Where(x => x.EquipmentPlanandEvaluationsID == Id).FirstOrDefault();

                    entity.IsSendForApproval = false;

                    db.EquipmentPlanandEvaluation.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new EquipmentPlanEvaluationApproval()
                {

                    Status = EquipmentPlanEvaluationApproval.Status,
                    EquipmentPlanandEvaluationsId = Id,
                    Remark = EquipmentPlanEvaluationApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.EquipmentPlanEvaluationApprovals.Add(approval);
                db.SaveChanges();
                EquipmentPlanEvaluationApproval.EquipmentPlanEvaluationApprovalId = approval.EquipmentPlanEvaluationApprovalId;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { EquipmentPlanEvaluationApproval }.ToDataSourceResult(request));
        }




    }
}