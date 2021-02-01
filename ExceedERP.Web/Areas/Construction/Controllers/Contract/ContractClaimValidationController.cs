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

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class ContractClaimValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult ClaimValidations_Read([DataSourceRequest]DataSourceRequest request, int claimId)
        {
            IQueryable<ContractClaimValidation> validations = db.ContractClaimValidations.Where(v => v.ContractClaimId == claimId);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.ContractClaimValidationId,
                validation.ContractClaimId,
                validation.RespondedDate,
                validation.ResponseRefNumber,
                validation.ResponseReferanceLatter,

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
        public ActionResult ClaimValidation_Create([DataSourceRequest]DataSourceRequest request, ContractClaimValidation validation, int claimId)
        {
            var record = db.ContractClaim.Find(claimId);

            #region Checking the  record

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                {
                    //if (validation.Status == ApprovalStatuses.Approve)
                    //{
                    //    foreach (WorkInProgressDetail i in record.WorkInProgressItems)
                    //    {
                    //        if (!FixedAssetHelper.IsAssetCurrentLocationStore(i.AssetRegistrationId, i.FixedAssetIssue.StoreCode))
                    //        {
                    //            string message = "Asset with tag number " + i.TagNo + " does not exist in store " + i.FixedAssetIssue.StoreCode;
                    //            ModelState.AddModelError("doesnotexist", message);
                    //        }
                    //    }
                    //}
                    if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
            }
            else
            {
                ModelState.AddModelError("No Found", "Claim record not found");
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

                        record.RespondedDate = validation.RespondedDate;
                        record.ResponseRefNumber = validation.ResponseRefNumber;
                        record.ResponseReferanceLatter = validation.ResponseReferanceLatter;
                        record.TimeConsumed = record.RespondedDate != null ? ((DateTime)record.RespondedDate - record.RequestedDate).Days : 0;

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

                var entity = new ContractClaimValidation
                {
                    //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                    RespondedDate = validation.RespondedDate,
                    ResponseRefNumber = validation.ResponseRefNumber,
                    ResponseReferanceLatter = validation.ResponseReferanceLatter,
                    ContractClaimId = claimId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = validation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.ContractClaimValidations.Add(entity);
                db.SaveChanges();

                validation.ContractClaimValidationId = entity.ContractClaimValidationId;
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClaimValidation_Update([DataSourceRequest]DataSourceRequest request, ContractClaimValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractClaimValidation
                {
                    ContractClaimValidationId = validation.ContractClaimValidationId,
                    RespondedDate = validation.RespondedDate,
                    ResponseRefNumber = validation.ResponseRefNumber,
                    ResponseReferanceLatter = validation.ResponseReferanceLatter,
                    ContractClaimId = validation.ContractClaimId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.ContractClaimValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClaimValidation_Destroy([DataSourceRequest]DataSourceRequest request, ContractClaimValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractClaimValidation
                {
                    ContractClaimValidationId = validation.ContractClaimValidationId,
                    RespondedDate = validation.RespondedDate,
                    ResponseRefNumber = validation.ResponseRefNumber,
                    ResponseReferanceLatter = validation.ResponseReferanceLatter,
                    ContractClaimId = validation.ContractClaimId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.ContractClaimValidations.Attach(entity);
                db.ContractClaimValidations.Remove(entity);
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