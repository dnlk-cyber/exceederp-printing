using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Assetregistration
{
    public class AssetDisposalValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AssetDisposalValidation_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AssetDisposalValidation> assetdisposalvalidation = db.AssetDisposalValidation.Where(x=>x.AssetDisposalId==id);
            DataSourceResult result = assetdisposalvalidation.ToDataSourceResult(request, assetDisposalValidation => new {
                AssetDisposalValidationId = assetDisposalValidation.AssetDisposalValidationId,
                AssetDisposalId = assetDisposalValidation.AssetDisposalId,
                Status = assetDisposalValidation.Status,
                Remark = assetDisposalValidation.Remark,
                DateCreated = assetDisposalValidation.DateCreated,
                LastModified = assetDisposalValidation.LastModified,
                CreatedBy = assetDisposalValidation.CreatedBy,
                ModifiedBy = assetDisposalValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetDisposalValidation_Create(int id,[DataSourceRequest]DataSourceRequest request, AssetDisposalValidation assetDisposalValidation)
        {
            var assetDisposal = db.AssetDisposals.Find(id);

            if (ModelState.IsValid)
            {
                if (assetDisposal != null)
                {
                    if (assetDisposalValidation.Status == ApprovalStatuses.Approve)
                    {
                        assetDisposal.Status = "Approved";
                        assetDisposal.IsOnlineApproved = true;
                        assetDisposal.OnlineApprovedBy = User.Identity.Name;
                        assetDisposal.OnlineApprovedTime = DateTime.Now;

                        //update asset status to disposed
                        FixedAssetHelper.UpdateAssetRegistrationAfterDisposal(assetDisposal, User.Identity.Name);
                    }

                    if (assetDisposalValidation.Status == ApprovalStatuses.Reject)
                    {
                        assetDisposal.IsSendForApproval = false;
                        assetDisposal.Status = "Rejected";
                    }

                    var entity = new AssetDisposalValidation
                    {
                        AssetDisposalId = id,
                        Status = assetDisposalValidation.Status,
                        Remark = assetDisposalValidation.Remark,
                        DateCreated = assetDisposalValidation.DateCreated,
                        LastModified = assetDisposalValidation.LastModified,
                        CreatedBy = assetDisposalValidation.CreatedBy,
                        ModifiedBy = assetDisposalValidation.ModifiedBy
                    };

                    db.AssetDisposalValidation.Add(entity);
                    db.SaveChanges();
                    assetDisposalValidation.AssetDisposalValidationId = entity.AssetDisposalValidationId;
                }
            }

            return Json(new[] { assetDisposalValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetDisposalValidation_Update([DataSourceRequest]DataSourceRequest request, AssetDisposalValidation assetDisposalValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetDisposalValidation
                {
                    AssetDisposalValidationId = assetDisposalValidation.AssetDisposalValidationId,
                    Status = assetDisposalValidation.Status,
                    Remark = assetDisposalValidation.Remark,
                    DateCreated = assetDisposalValidation.DateCreated,
                    LastModified = assetDisposalValidation.LastModified,
                    CreatedBy = assetDisposalValidation.CreatedBy,
                    ModifiedBy = assetDisposalValidation.ModifiedBy
                };

                db.AssetDisposalValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { assetDisposalValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetDisposalValidation_Destroy([DataSourceRequest]DataSourceRequest request, AssetDisposalValidation assetDisposalValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetDisposalValidation
                {
                    AssetDisposalValidationId = assetDisposalValidation.AssetDisposalValidationId,
                    Status = assetDisposalValidation.Status,
                    Remark = assetDisposalValidation.Remark,
                    DateCreated = assetDisposalValidation.DateCreated,
                    LastModified = assetDisposalValidation.LastModified,
                    CreatedBy = assetDisposalValidation.CreatedBy,
                    ModifiedBy = assetDisposalValidation.ModifiedBy
                };

                db.AssetDisposalValidation.Attach(entity);
                db.AssetDisposalValidation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { assetDisposalValidation }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
