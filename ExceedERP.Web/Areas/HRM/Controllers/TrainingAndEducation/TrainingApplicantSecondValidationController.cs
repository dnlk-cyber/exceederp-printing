using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingEducation
{
    public class TrainingApplicantSecondValidationController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrainingApplicantSecondValidations_Read(int id, [DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<TrainingApplicantSecondValidation> validations = _db.TrainingApplicantSecondValidations
                .Where(x => x.TrainingApplicantId == id);

            DataSourceResult result = validations.ToDataSourceResult(request, v => new
            {
                TrainingApplicantSecondValidationId = v.TrainingApplicantSecondValidationId,
                TrainingApplicantId = v.TrainingApplicantId,
                Status = v.Status,
                Remark = v.Remark,
                OrgBranchName = v.OrgBranchName,
                DateCreated = v.DateCreated,
                LastModified = v.LastModified,
                CreatedBy = v.CreatedBy,
                ModifiedBy = v.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicantSecondValidations_Create(int id, [DataSourceRequest] DataSourceRequest request, TrainingApplicantSecondValidation model)
        {
            var trainingApplicant = _db.TrainingApplicants.Find(id);

            if (trainingApplicant != null && trainingApplicant.IsMiddleOnlineApproved)
            {
                ModelState.AddModelError(string.Empty, "You can not manipulate this Record: It is already Approved");
            }

            if (ModelState.IsValid)
            {
                if (trainingApplicant != null)
                {
                    if (model.Status == ApprovalStatuses.Approve)
                    {
                        trainingApplicant.Status = "Second Approved";
                        trainingApplicant.IsMiddleOnlineApproved = true;
                        trainingApplicant.MiddleOnlineApprovedBy = User.Identity.Name;
                        trainingApplicant.MiddleOnlineApprovedTime = DateTime.Now;
                        _db.Entry(trainingApplicant).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else if (model.Status == ApprovalStatuses.Reject)
                    {
                        trainingApplicant.MiddleOnlineApprovedBy = "";
                        trainingApplicant.Status = "Second Approval Rejected";
                        _db.Entry(trainingApplicant).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    var entity = new TrainingApplicantSecondValidation
                    {
                        TrainingApplicantId = id,
                        Status = model.Status,
                        Remark = model.Remark,
                        OrgBranchName = model.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.TrainingApplicantSecondValidations.Add(entity);
                    _db.SaveChanges();

                    model.TrainingApplicantSecondValidationId = entity.TrainingApplicantSecondValidationId;

                    #region Operation Log
                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "TrainingApplicantSecondValidation",
                        "Success",
                        User.Identity.Name,
                        $"created TrainingApplicantSecondValidation {entity.TrainingApplicantSecondValidationId} Tag {entity.TrainingApplicantId}",
                        Modules.HRM
                    );
                    #endregion
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicantSecondValidations_Update([DataSourceRequest] DataSourceRequest request, TrainingApplicantSecondValidation model)
        {
            var trainingApplicant = _db.TrainingApplicants.Find(model.TrainingApplicantId);
            if (trainingApplicant != null && trainingApplicant.IsMiddleOnlineApproved)
            {
                ModelState.AddModelError(string.Empty, "You can not manipulate this Record: It is already Approved");
            }

            if (ModelState.IsValid)
            {
                var entity = _db.TrainingApplicantSecondValidations.Find(model.TrainingApplicantSecondValidationId);
                if (entity != null)
                {
                    entity.Status = model.Status;
                    entity.Remark = model.Remark;
                    entity.OrgBranchName = model.OrgBranchName;
                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;

                    if (trainingApplicant != null)
                    {
                        if (model.Status == ApprovalStatuses.Approve)
                        {
                            trainingApplicant.Status = "Second Approved";
                            trainingApplicant.IsMiddleOnlineApproved = true;
                            trainingApplicant.MiddleOnlineApprovedBy = User.Identity.Name;
                            trainingApplicant.MiddleOnlineApprovedTime = DateTime.Now;
                        }
                        else if (model.Status == ApprovalStatuses.Reject)
                        {
                            trainingApplicant.MiddleOnlineApprovedBy = "";
                            trainingApplicant.Status = "Second Approval Rejected";
                            trainingApplicant.IsMiddleOnlineApproved = false;
                        }
                        _db.Entry(trainingApplicant).State = EntityState.Modified;
                    }

                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region Operation Log
                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "TrainingApplicantSecondValidation",
                        "Success",
                        User.Identity.Name,
                        $"updated TrainingApplicantSecondValidation {entity.TrainingApplicantSecondValidationId} Tag {entity.TrainingApplicantId}",
                        Modules.HRM
                    );
                    #endregion
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicantSecondValidations_Destroy([DataSourceRequest] DataSourceRequest request, TrainingApplicantSecondValidation model)
        {
            var entity = _db.TrainingApplicantSecondValidations.Find(model.TrainingApplicantSecondValidationId);
            if (entity != null)
            {
                var trainingApplicant = _db.TrainingApplicants.Find(entity.TrainingApplicantId);
                if (trainingApplicant != null && trainingApplicant.IsMiddleOnlineApproved)
                {
                    ModelState.AddModelError(string.Empty, "You can not manipulate this Record: It is already Approved");
                }

                if (ModelState.IsValid)
                {
                    _db.TrainingApplicantSecondValidations.Remove(entity);
                    _db.SaveChanges();

                    #region Operation Log
                    UserHelper.OperationLog(
                        Request.UserHostAddress,
                        "TrainingApplicantSecondValidation",
                        "Success",
                        User.Identity.Name,
                        $"deleted TrainingApplicantSecondValidation {entity.TrainingApplicantSecondValidationId} Tag {entity.TrainingApplicantId}",
                        Modules.HRM
                    );
                    #endregion
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}