using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Plan;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.NeedAndPlan
{
  
    public class ProcurementPlanValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanUser, ProcurementRoles.ProcurementPlanApprover)]
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementPlanUser, ProcurementRoles.ProcurementPlanApprover)]
        public ActionResult PlanValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProcurementPlanValidation> planvalidations = db.ProcurementPlanValidations.Where(x => x.ProcurementPlanId == id);
            DataSourceResult result = planvalidations.ToDataSourceResult(request, planValidation => new
            {
                ProcurementPlanValidationId = planValidation.ProcurementPlanValidationId,
                Status = planValidation.Status,
                Remark = planValidation.Remark,
                ProcurementPlanId = planValidation.ProcurementPlanId,

                OrgBranchName = planValidation.OrgBranchName,
                DateCreated = planValidation.DateCreated,
                LastModified = planValidation.LastModified,
                CreatedBy = planValidation.CreatedBy,
                ModifiedBy = planValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]
        public ActionResult PlanValidations_Create(int id, ProcurementPlanValidation planValidation)
        {
            planValidation.CreatedBy = User.Identity.Name;
            planValidation.DateCreated = DateTime.Now.Date;
            ProcurementPlan record = db.ProcurementPlans.Find(id);
            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (planValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = Enum.GetName(typeof(OperationStatuses), 1);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.ProcurementPlans.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (planValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        db.ProcurementPlans.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                   

                    var entity = new ProcurementPlanValidation
                    {
                        Status = planValidation.Status,
                        Remark = planValidation.Remark,
                        ProcurementPlanId = id,
                        OrgBranchName = planValidation.OrgBranchName,
                        DateCreated = planValidation.DateCreated,
                        LastModified = planValidation.LastModified,
                        CreatedBy = planValidation.CreatedBy,
                        ModifiedBy = planValidation.ModifiedBy
                    };

                    db.ProcurementPlanValidations.Add(entity);
                    db.SaveChanges();
                    planValidation.ProcurementPlanValidationId = entity.ProcurementPlanValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "procurement plan approval", "Success", User.Identity.Name, $"approved plan {entity.ProcurementPlanId} ", Modules.Procurement);
                    #endregion
                }
            }


            return Json(new[] { planValidation }.ToDataSourceResult(new DataSourceRequest(), ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanValidations_Update([DataSourceRequest]DataSourceRequest request, ProcurementPlanValidation planValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlanValidation
                {
                    ProcurementPlanValidationId = planValidation.ProcurementPlanValidationId,
                    Status = planValidation.Status,
                    Remark = planValidation.Remark,
                    OrgBranchName = planValidation.OrgBranchName,
                    DateCreated = planValidation.DateCreated,
                    LastModified = planValidation.LastModified,
                    CreatedBy = planValidation.CreatedBy,
                    ModifiedBy = planValidation.ModifiedBy
                };

                db.ProcurementPlanValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { planValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanValidations_Destroy([DataSourceRequest]DataSourceRequest request, ProcurementPlanValidation planValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlanValidation
                {
                    ProcurementPlanValidationId = planValidation.ProcurementPlanValidationId,
                    Status = planValidation.Status,
                    Remark = planValidation.Remark,
                    OrgBranchName = planValidation.OrgBranchName,
                    DateCreated = planValidation.DateCreated,
                    LastModified = planValidation.LastModified,
                    CreatedBy = planValidation.CreatedBy,
                    ModifiedBy = planValidation.ModifiedBy
                };

                db.ProcurementPlanValidations.Attach(entity);
                db.ProcurementPlanValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { planValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
