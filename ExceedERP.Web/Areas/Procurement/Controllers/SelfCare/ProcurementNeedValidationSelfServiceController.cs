using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.SelfCare
{

    public class ProcurementNeedValidationSelfServiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementPlanApprover, ProcurementRoles.ProcurementPlanUser)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementPlanApprover, ProcurementRoles.ProcurementPlanUser)]

        public ActionResult AnnualNeedAssesmentValidations_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AnnualNeedAssesmentValidation> annualneedassesmentvalidations = db.AnnualNeedAssesmentValidations.Where(x => x.AnnualNeedAssesmentID == id);
            DataSourceResult result = annualneedassesmentvalidations.ToDataSourceResult(request, annualNeedAssesmentValidation => new
            {
                AnnualNeedAssesmentValidationID = annualNeedAssesmentValidation.AnnualNeedAssesmentValidationID,
                Status = annualNeedAssesmentValidation.Status,
                Remark = annualNeedAssesmentValidation.Remark,

                OrgBranchName = annualNeedAssesmentValidation.OrgBranchName,
                DateCreated = annualNeedAssesmentValidation.DateCreated,
                LastModified = annualNeedAssesmentValidation.LastModified,
                CreatedBy = annualNeedAssesmentValidation.CreatedBy,
                ModifiedBy = annualNeedAssesmentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesmentValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentValidation annualNeedAssesmentValidation)
        {

            annualNeedAssesmentValidation.CreatedBy = User.Identity.Name;
            annualNeedAssesmentValidation.DateCreated = DateTime.Now.Date;
            AnnualNeedAssesment record = db.AnnualNeedAssesments.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (annualNeedAssesmentValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = "User Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                    }

                    if (annualNeedAssesmentValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                    }
                    db.AnnualNeedAssesments.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();

                    var entity = new AnnualNeedAssesmentValidation
                    {
                        Status = annualNeedAssesmentValidation.Status,
                        Remark = annualNeedAssesmentValidation.Remark,
                        AnnualNeedAssesmentID = id,
                        OrgBranchName = annualNeedAssesmentValidation.OrgBranchName,
                        DateCreated = annualNeedAssesmentValidation.DateCreated,
                        LastModified = annualNeedAssesmentValidation.LastModified,
                        CreatedBy = annualNeedAssesmentValidation.CreatedBy,
                        ModifiedBy = annualNeedAssesmentValidation.ModifiedBy
                    };

                    db.AnnualNeedAssesmentValidations.Add(entity);
                    db.SaveChanges();
                    annualNeedAssesmentValidation.AnnualNeedAssesmentValidationID =
                        entity.AnnualNeedAssesmentValidationID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "procurement need approval self care", "Success", User.Identity.Name, $"approved procurement need {entity.AnnualNeedAssesmentID} ", Modules.Procurement);
                    #endregion
                }

            }

            return Json(new[] { annualNeedAssesmentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesmentValidations_Update([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentValidation annualNeedAssesmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesmentValidation
                {
                    AnnualNeedAssesmentValidationID = annualNeedAssesmentValidation.AnnualNeedAssesmentValidationID,
                    Status = annualNeedAssesmentValidation.Status,
                    Remark = annualNeedAssesmentValidation.Remark,
                    OrgBranchName = annualNeedAssesmentValidation.OrgBranchName,
                    DateCreated = annualNeedAssesmentValidation.DateCreated,
                    LastModified = annualNeedAssesmentValidation.LastModified,
                    CreatedBy = annualNeedAssesmentValidation.CreatedBy,
                    ModifiedBy = annualNeedAssesmentValidation.ModifiedBy
                };

                db.AnnualNeedAssesmentValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { annualNeedAssesmentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPlanApprover)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesmentValidations_Destroy([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesmentValidation annualNeedAssesmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesmentValidation
                {
                    AnnualNeedAssesmentValidationID = annualNeedAssesmentValidation.AnnualNeedAssesmentValidationID,
                    Status = annualNeedAssesmentValidation.Status,
                    Remark = annualNeedAssesmentValidation.Remark,
                    OrgBranchName = annualNeedAssesmentValidation.OrgBranchName,
                    DateCreated = annualNeedAssesmentValidation.DateCreated,
                    LastModified = annualNeedAssesmentValidation.LastModified,
                    CreatedBy = annualNeedAssesmentValidation.CreatedBy,
                    ModifiedBy = annualNeedAssesmentValidation.ModifiedBy
                };

                db.AnnualNeedAssesmentValidations.Attach(entity);
                db.AnnualNeedAssesmentValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { annualNeedAssesmentValidation }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

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
