using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.CRM;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.PriceEstimationApprover)]
    public class PriceEstimationQuoteVoucherValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PriceEstimationQuoteVoucherValidation> validations = db.PriceEstimationQuoteVoucherValidation.Where(v => v.PriceEstimationQuoteVoucherId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.PriceEstimationQuoteVoucherValidationId,
                validation.PriceEstimationQuoteVoucherId,

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
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherValidation validation, int id)
        {
            var record = db.PriceEstimationQuoteVouchers.Find(id);

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
                    ModelState.AddModelError("notSentForApproval", "Record not sent for Approval. Transaction canceled.");
                }
            }
            else
            {
                ModelState.AddModelError("Not Found", "Quote record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                bool quoteVoucherAndLineItemUpdateSuccessful = false;
                if (record != null)
                {
                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        #region update
                        record.Status = CRMVoucherStatusConstatns.Approved;
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now;
                        #endregion
                    }
                    else if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = CRMConstants.Rejected;
                    }
                    db.PriceEstimationQuoteVouchers.Attach(record);
                    db.Entry(record).State = EntityState.Modified;

                    // save changes
                    // Quote voucher changes
                    // Quote voucher line item changes
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        quoteVoucherAndLineItemUpdateSuccessful = true;
                    }
                }

                if (quoteVoucherAndLineItemUpdateSuccessful)
                {
                    var entity = new PriceEstimationQuoteVoucherValidation
                    {
                        PriceEstimationQuoteVoucherId = id,
                        Status = validation.Status,
                        Remark = validation.Remark,

                        OrgBranchName = validation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = validation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = validation.ModifiedBy,
                    };

                    db.PriceEstimationQuoteVoucherValidation.Add(entity);
                    db.SaveChanges();

                    validation.PriceEstimationQuoteVoucherId = entity.PriceEstimationQuoteVoucherId;
                }
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceEstimationQuoteVoucherValidation
                {
                    PriceEstimationQuoteVoucherValidationId = validation.PriceEstimationQuoteVoucherValidationId,
                    PriceEstimationQuoteVoucherId = validation.PriceEstimationQuoteVoucherId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.PriceEstimationQuoteVoucherValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceEstimationQuoteVoucherValidation
                {
                    PriceEstimationQuoteVoucherValidationId = validation.PriceEstimationQuoteVoucherValidationId,
                    PriceEstimationQuoteVoucherId = validation.PriceEstimationQuoteVoucherId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.PriceEstimationQuoteVoucherValidation.Attach(entity);
                db.PriceEstimationQuoteVoucherValidation.Remove(entity);
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