using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMQueueApprover)]
    public class QueueStatusValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<QueueStatusValidation> validations = db.QueueStatusValidation.Where(v => v.QueueStatusId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.QueueStatusValidationId,
                validation.QueueStatusId,


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
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, QueueStatusValidation validation, int id)
        {
            var record = db.QueueStatus.Find(id);

            #region Checking the  record

            if (record != null)
            {
               
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }

                if (!record.IsSendForApproval)
                {
                    string message = "Record is not waiting approval. Operation terminated";
                    ModelState.AddModelError("not_sent", message);
                }

            }
            else
            {
                ModelState.AddModelError("Not Found", "Queue Status record not found");
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
                        else if (validation.Status == ApprovalStatuses.Reject)
                        {
                            record.IsSendForApproval = false;
                            record.Status = "Rejected";
                        }
                        db.QueueStatus.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                        var entity = new QueueStatusValidation
                        {
                            //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                            QueueStatusId = id,
                            Status = validation.Status,
                            Remark = validation.Remark,

                            OrgBranchName = validation.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = validation.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = validation.ModifiedBy,
                        };

                        db.QueueStatusValidation.Add(entity);
                        db.SaveChanges();

                        validation.QueueStatusValidationId = entity.QueueStatusValidationId;
                    }
                


            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, QueueStatusValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueStatusValidation
                {
                    QueueStatusValidationId = validation.QueueStatusValidationId,
                    Status = validation.Status,
                    Remark = validation.Remark,
                    QueueStatusId = validation.QueueStatusId,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.QueueStatusValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, QueueStatusValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueStatusValidation
                {
                    QueueStatusValidationId = validation.QueueStatusValidationId,
                    Status = validation.Status,
                    QueueStatusId = validation.QueueStatusId,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.QueueStatusValidation.Attach(entity);
                db.QueueStatusValidation.Remove(entity);
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