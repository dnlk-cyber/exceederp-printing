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
    [AuthorizeRoles(CRMRoles.CreditSettlementUser, CRMRoles.CreditSettlementApprover)]
    public class CreditSettlementValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CreditSettlementValidation> validations = db.CreditSettlementValidation.Where(v => v.CreditSettlementId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.CreditSettlementValidationId,
                validation.CreditSettlementId,

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
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, CreditSettlementValidation validation, int id)
        {
            var record = db.CreditSettlement.Find(id);

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
                //{
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
                //}
            }
            else
            {
                ModelState.AddModelError("Not Found", "Shipment record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        var success = UpdateChangesOnCreditSetlementApprove(record);
                        if (success)
                        {
                            #region update
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            #endregion
                            //CRMSalesHelper.SendCreditSettlementToBankReconciliation(record, User.Identity.Name);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Something Went Wrong in Processing the Transaction!");
                        }
                    }
                    if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                    db.CreditSettlement.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var entity = new CreditSettlementValidation
                {
                    //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                    CreditSettlementId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = validation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.CreditSettlementValidation.Add(entity);
                db.SaveChanges();

                validation.CreditSettlementValidationId = entity.CreditSettlementValidationId;
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, CreditSettlementValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new CreditSettlementValidation
                {
                    CreditSettlementValidationId = validation.CreditSettlementValidationId,
                    CreditSettlementId = validation.CreditSettlementId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.CreditSettlementValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, CreditSettlementValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new CreditSettlementValidation
                {
                    CreditSettlementValidationId = validation.CreditSettlementValidationId,
                    CreditSettlementId = validation.CreditSettlementId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.CreditSettlementValidation.Attach(entity);
                db.CreditSettlementValidation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        private bool UpdateChangesOnCreditSetlementApprove(CreditSettlement settlement)
        {


            bool transactionCompletedSuccessfully = true;


            return transactionCompletedSuccessfully;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}