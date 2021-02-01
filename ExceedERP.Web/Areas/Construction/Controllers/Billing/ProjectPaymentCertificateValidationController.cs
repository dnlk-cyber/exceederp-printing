using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class ProjectPaymentCertificateValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProjectPaymentCertificateValidation> validations = db.PaymentCertificateValidations.Where(v => v.ProjectPaymentCertificateId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.ProjectPaymentCertificateValidationId,
                validation.ProjectPaymentCertificateId,

                validation.OrgBranchName,
                validation.DateCreated,
                validation.LastModified,
                validation.CreatedBy,
                validation.ModifiedBy,

                validation.Status,
                validation.Remark
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, ProjectPaymentCertificateValidation validation, int id)
        {
            var record = db.PaymentCertificates.Find(id);

            #region Checking the  record

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                {
                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        //foreach (WorkInProgressDetail i in record.WorkInProgressItems)
                        //{
                        if (record.NetPayableAmount <= 0)
                        {
                            string message = "The Net Payable Amount, " + record.NetPayableAmount + ", is Invalid";
                            ModelState.AddModelError("invalidamount", message);
                        }
                        //}
                    }
                    if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "Payment Certificate record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        #region update
                        record.Status = "Approved";
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;

                       
                        #endregion
                    }
                    if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var entity = new ProjectPaymentCertificateValidation
                {                    
                    ProjectPaymentCertificateId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = validation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.PaymentCertificateValidations.Add(entity);
                db.SaveChanges();

                validation.ProjectPaymentCertificateValidationId = entity.ProjectPaymentCertificateValidationId;
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, ProjectPaymentCertificateValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymentCertificateValidation
                {
                    ProjectPaymentCertificateValidationId = validation.ProjectPaymentCertificateValidationId,
                    ProjectPaymentCertificateId = validation.ProjectPaymentCertificateId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.PaymentCertificateValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPaymentCertificateValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymentCertificateValidation
                {
                    ProjectPaymentCertificateValidationId = validation.ProjectPaymentCertificateValidationId,
                    ProjectPaymentCertificateId = validation.ProjectPaymentCertificateId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.PaymentCertificateValidations.Attach(entity);
                db.PaymentCertificateValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}