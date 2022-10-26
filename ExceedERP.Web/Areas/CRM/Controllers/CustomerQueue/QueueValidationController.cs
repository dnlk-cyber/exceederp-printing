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
    public class QueueValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<QueueValidation> validations = db.QueueValidation.Where(v => v.QueueId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.QueueValidationId,
                validation.QueueId,
                //validation.RequestedQuantity,
                validation.ApprovedQuantity,


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
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, QueueValidation validation, int id)
        {
            var record = db.Queue.Find(id);

            #region Checking the  record

            if (record != null)
            {
                //if(validation.ApprovedQuantity > record.TotalRequestedAmount)
                //{
                //    string Message = "Approved Quantity";
                //}
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
                ModelState.AddModelError("Not Found", "Queue record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                if (validation.ApprovedQuantity <= 0)
                {
                    ModelState.AddModelError("InvalidQuantity", "Approved Quantity Should be greater than 0");
                }
                else
                {
                    if (record != null)
                    {
                        if (validation.Status == ApprovalStatuses.Approve)
                        {

                            #region update
                            record.Status = "Approved";
                            record.ApprovedQuantity = validation.ApprovedQuantity;
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
                        db.Queue.Attach(record);
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                        var entity = new QueueValidation
                        {
                            //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                            QueueId = id,
                            Status = validation.Status,
                            //RequestedQuantity = validation.RequestedQuantity,
                            ApprovedQuantity = validation.ApprovedQuantity,
                            Remark = validation.Remark,

                            OrgBranchName = validation.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = validation.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = validation.ModifiedBy,
                        };

                        db.QueueValidation.Add(entity);
                        db.SaveChanges();

                        validation.QueueValidationId = entity.QueueValidationId;
                    }
                }


            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, QueueValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueValidation
                {
                    QueueValidationId = validation.QueueValidationId,
                    Status = validation.Status,
                    //RequestedQuantity = validation.RequestedQuantity,
                    ApprovedQuantity = validation.ApprovedQuantity,
                    Remark = validation.Remark,
                    QueueId = validation.QueueId,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.QueueValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, QueueValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueValidation
                {
                    QueueValidationId = validation.QueueValidationId,
                    Status = validation.Status,
                    //RequestedQuantity = validation.RequestedQuantity,
                    ApprovedQuantity = validation.ApprovedQuantity,
                    Remark = validation.Remark,
                    QueueId = validation.QueueId,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.QueueValidation.Attach(entity);
                db.QueueValidation.Remove(entity);
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