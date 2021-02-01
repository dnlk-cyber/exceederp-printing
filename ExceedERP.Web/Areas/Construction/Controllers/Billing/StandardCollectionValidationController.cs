using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class StandardCollectionValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult CollectionValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ConsARStandardCollectionValidation> validations = db.ConsARStandardCollectionValidations.Where(v => v.ConsARStandardCollectionId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.ConsARStandardCollectionValidationId,
                validation.ConsARStandardCollectionId,

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
        public ActionResult CollectionValidation_Create([DataSourceRequest]DataSourceRequest request, ConsARStandardCollectionValidation validation, int id)
        {
            var record = db.ConsARStandardCollections.Find(id);

            #region Checking the collection record

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
                        //foreach (ReceivableInvoiceDetail i in record.InvoiceItems)
                        //{
                        //    if (!FixedAssetHelper.IsAssetCurrentLocationStore(i.AssetRegistrationId, i.FixedAssetIssue.StoreCode))
                        //    {
                        //        string message = "Asset with tag number " + i.TagNo + " does not exist in store " + i.FixedAssetIssue.StoreCode;
                        //        ModelState.AddModelError("doesnotexist", message);
                        //    }
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
                ModelState.AddModelError("No Found", "Invoice record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        #region update stock based on the issue
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

                var entity = new ConsARStandardCollectionValidation
                {
                    //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                    ConsARStandardCollectionId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = validation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.ConsARStandardCollectionValidations.Add(entity);
                db.SaveChanges();

                validation.ConsARStandardCollectionValidationId = entity.ConsARStandardCollectionValidationId;
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CollectionValidation_Update([DataSourceRequest]DataSourceRequest request, ConsARStandardCollectionValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsARStandardCollectionValidation
                {
                    ConsARStandardCollectionValidationId = validation.ConsARStandardCollectionValidationId,
                    ConsARStandardCollectionId = validation.ConsARStandardCollectionId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.ConsARStandardCollectionValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CollectionValidation_Destroy([DataSourceRequest]DataSourceRequest request, ConsARStandardCollectionValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsARStandardCollectionValidation()
                {
                    ConsARStandardCollectionValidationId = validation.ConsARStandardCollectionValidationId,
                    ConsARStandardCollectionId = validation.ConsARStandardCollectionId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.ConsARStandardCollectionValidations.Attach(entity);
                db.ConsARStandardCollectionValidations.Remove(entity);
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