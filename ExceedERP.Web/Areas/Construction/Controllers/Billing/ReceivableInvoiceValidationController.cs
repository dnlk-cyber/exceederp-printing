using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class ReceivableInvoiceValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult InvoiceValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ConsReceivableInvoiceValidation> validations = db.ConsReceivableInvoiceValidation.Where(iv => iv.ConsReceivableInvoiceId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.ConsReceivableInvoiceId,
                validation.ConsReceivableInvoiceValidationId,

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
        public ActionResult InvoiceValidation_Create([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceValidation validation, int id)
        {
            var record = db.ConsReceivableInvoices.Find(id);

            #region Checking the invoice

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

                var entity = new ConsReceivableInvoiceValidation
                {
                    //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                    ConsReceivableInvoiceId = id,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = validation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = validation.ModifiedBy,


                };

                db.ConsReceivableInvoiceValidation.Add(entity);
                db.SaveChanges();

                validation.ConsReceivableInvoiceValidationId = entity.ConsReceivableInvoiceValidationId;

            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvoiceValidation_Update([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsReceivableInvoiceValidation
                {
                    ConsReceivableInvoiceValidationId = validation.ConsReceivableInvoiceValidationId,
                    ConsReceivableInvoiceId = validation.ConsReceivableInvoiceId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,


                };

                db.ConsReceivableInvoiceValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvoiceValidation_Destroy([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsReceivableInvoiceValidation
                {
                    ConsReceivableInvoiceValidationId = validation.ConsReceivableInvoiceValidationId,
                    ConsReceivableInvoiceId = validation.ConsReceivableInvoiceId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.ConsReceivableInvoiceValidation.Attach(entity);
                db.ConsReceivableInvoiceValidation.Remove(entity);
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