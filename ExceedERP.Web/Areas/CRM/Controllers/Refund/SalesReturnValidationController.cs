using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesReturnUser, CRMRoles.SalesReturnApprover)]
    public class SalesReturnValidationController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SalesReturnValidation> validations = db.SalesReturnValidation.Where(v => v.SalesReturnId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.SalesReturnValidationId,
                validation.SalesReturnId,

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

        [AuthorizeRoles(CRMRoles.SalesReturnApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, SalesReturnValidation validation, int id)
        {
            var record = db.SalesReturn.Find(id);

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
                    ModelState.AddModelError("Not sent for approval", "This record is not waiting approval");
                }

                if (validation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.Status = "Rejected";
                }
            }
            else
            {
                ModelState.AddModelError("Not Found", "record not found");
            }
            #endregion

            if (ModelState.IsValid && record != null)
            {
                if (validation.Status == ApprovalStatuses.Approve)
                {
                    #region update
                    record.Status = CRMConstants.Approved;
                    record.IsOnlineApproved = true;
                    record.OnlineApprovedBy = User.Identity.Name;
                    record.OnlineApprovedTime = DateTime.Now;
                    #endregion
                }
                if (validation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.Status = CRMConstants.Rejected;
                }
                db.SalesReturn.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                var res = db.SaveChanges();

                if (res > 0)
                {
                    //CRMSalesHelper.SendDepositToBankReconciliation(record, User.Identity.Name);
                    var entity = new SalesReturnValidation
                    {
                        SalesReturnId = id,
                        Status = validation.Status,
                        Remark = validation.Remark,

                        OrgBranchName = validation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = validation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = validation.ModifiedBy,
                    };

                    db.SalesReturnValidation.Add(entity);
                    db.SaveChanges();

                    validation.SalesReturnValidationId = entity.SalesReturnValidationId;
                }

            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, SalesReturnValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesReturnValidation
                {
                    SalesReturnValidationId = validation.SalesReturnValidationId,
                    SalesReturnId = validation.SalesReturnId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.SalesReturnValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, SalesReturnValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesReturnValidation
                {
                    SalesReturnValidationId = validation.SalesReturnValidationId,
                    SalesReturnId = validation.SalesReturnId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.SalesReturnValidation.Attach(entity);
                db.SalesReturnValidation.Remove(entity);
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